# iBank — ajout de Google Authenticator (2FA)

Ce dépôt contient le backend ASP.NET Core .NET 9 de l’application de formation
iBank. La branche `feature/two-factor-auth` est consacrée à l’ajout d’une
authentification à deux facteurs compatible avec Google Authenticator.

> État actuel : la conception 2FA est validée, mais le code fonctionnel reste à
> implémenter. Ce README décrit pas à pas les changements prévus et ne prétend
> pas que les endpoints sont déjà disponibles.

## 1. Récupérer la branche

```powershell
git fetch origin
git switch feature/two-factor-auth
git pull --ff-only
dotnet restore
dotnet build BankManagement.sln
dotnet test BankManagement.sln
```

La conception complète se trouve dans
[`docs/superpowers/specs/2026-10-30-two-factor-authentication-design.md`](docs/superpowers/specs/2026-10-30-two-factor-authentication-design.md).

## 2. Comprendre Google Authenticator

Google Authenticator utilise le standard TOTP : l’application mobile et le
backend partagent une clé secrète, puis calculent indépendamment un code à six
chiffres renouvelé environ toutes les trente secondes.

### Faut-il une API key Google ?

Non. Google Authenticator ne nécessite ni projet Google Cloud, ni OAuth, ni API
Google, ni API key. Le backend génère une clé avec ASP.NET Core Identity et
construit une URI comme :

```text
otpauth://totp/iBank:utilisateur@example.com?secret=CLE&issuer=iBank
```

Le frontend transforme cette URI en QR code localement. La clé secrète n’est
envoyée à aucun service QR externe.

## 3. Préparer le backend

Le projet utilise déjà `Microsoft.AspNetCore.Identity`. Son fournisseur de
codes Authenticator est inclus ; aucune bibliothèque TOTP externe ne sera
ajoutée.

Les modifications concerneront :

```text
Bank.Api/
├── Controllers/AuthController.cs
├── DTOs/Auth/
│   ├── LoginResponseDto.cs
│   ├── TwoFactorLoginDto.cs
│   ├── TwoFactorSetupDto.cs
│   └── TwoFactorCodeDto.cs
├── Services/Interfaces/IAuthService.cs
└── Services/Implementations/AuthService.cs
```

Les clés Authenticator et les codes de récupération seront stockés dans les
tables Identity existantes. Aucune nouvelle table métier n’est nécessaire.

## 4. Modifier la première étape de connexion

La route existante reste :

```http
POST /api/auth/login
```

Elle vérifie d’abord l’e-mail et le mot de passe.

- Sans 2FA, elle renvoie le JWT et l’utilisateur comme avant.
- Avec 2FA, elle ne renvoie aucun JWT métier. Elle renvoie un challenge signé
  valable cinq minutes et `requiresTwoFactor: true`.

```json
{
  "requiresTwoFactor": true,
  "twoFactorChallenge": "challenge-signe-temporaire"
}
```

Le challenge contient seulement l’identifiant utilisateur et un marqueur
d’usage. Il ne donne pas accès aux routes protégées par `[Authorize]`.

## 5. Ajouter la seconde étape de connexion

```http
POST /api/auth/login/2fa
```

Avec Google Authenticator :

```json
{
  "challenge": "challenge-signe-temporaire",
  "code": "123456",
  "useRecoveryCode": false
}
```

Avec un code de récupération :

```json
{
  "challenge": "challenge-signe-temporaire",
  "code": "abcd-efgh",
  "useRecoveryCode": true
}
```

Après validation, cette route émet le JWT normal. Un recovery code est à usage
unique. Un challenge expiré, altéré ou incorrect est refusé sans indiquer quelle
partie de la demande était invalide.

## 6. Ajouter les routes de gestion

Toutes les routes suivantes exigent le JWT normal.

### État du 2FA

```http
GET /api/auth/2fa/status
```

Indique si le 2FA est actif et combien de recovery codes restent.

### Préparation

```http
POST /api/auth/2fa/setup
```

Génère ou réinitialise la clé tant que le 2FA n’est pas actif :

```json
{
  "sharedKey": "CLE-MANUELLE",
  "authenticatorUri": "otpauth://totp/..."
}
```

### Activation

```http
POST /api/auth/2fa/enable
```

```json
{
  "code": "123456"
}
```

Le premier code doit être valide. Le backend active ensuite le 2FA et renvoie
les recovery codes une seule fois.

### Régénération des recovery codes

```http
POST /api/auth/2fa/recovery-codes
```

La demande contient un code Authenticator valide. Les anciens recovery codes
deviennent inutilisables.

### Désactivation

```http
POST /api/auth/2fa/disable
```

Après validation d’un code Authenticator, Identity désactive le 2FA, supprime
les recovery codes et renouvelle la clé secrète.

## 7. Changements frontend Angular

Le frontend se trouve dans
[`khalillakhdhar/ibankfront`](https://github.com/khalillakhdhar/ibankfront), sur
la même branche `feature/two-factor-auth`.

Les changements prévus sont :

1. reconnaître `requiresTwoFactor` dans `AuthService.login()` ;
2. conserver le challenge uniquement en mémoire, jamais dans `localStorage` ;
3. afficher la saisie du code après le mot de passe ;
4. permettre l’utilisation d’un recovery code ;
5. ajouter au profil la configuration et la désactivation du 2FA ;
6. générer le QR code localement depuis `authenticatorUri` ;
7. afficher les recovery codes une seule fois après activation ;
8. créer la session locale seulement après validation des deux facteurs.

## 8. Parcours utilisateur

### Activation

1. L’utilisateur se connecte normalement.
2. Il ouvre **Mon profil**, puis **Activer le 2FA**.
3. Le frontend demande la configuration au backend.
4. L’utilisateur ouvre Google Authenticator sur son téléphone.
5. Il appuie sur `+`, puis **Scanner un code QR**.
6. Il scanne le QR affiché par iBank.
7. Il saisit dans iBank le code à six chiffres généré sur le téléphone.
8. Le backend valide le code et active le 2FA.
9. L’utilisateur conserve les recovery codes hors de l’application.

### Connexions suivantes

1. L’utilisateur saisit son e-mail et son mot de passe.
2. Le backend renvoie un challenge temporaire, sans JWT.
3. L’utilisateur saisit le code Google Authenticator.
4. Le backend valide le code et renvoie la session JWT.
5. Le frontend ouvre le tableau de bord correspondant au rôle.

## 9. Tests à ajouter

Les tests backend devront vérifier :

- la connexion existante sans 2FA ;
- l’absence de JWT après le mot de passe lorsque le 2FA est actif ;
- le refus d’un challenge expiré ou altéré ;
- le refus d’un TOTP invalide et l’acceptation d’un TOTP valide ;
- la consommation unique d’un recovery code ;
- l’impossibilité d’activer le 2FA sans premier code valide ;
- la protection JWT des routes de gestion ;
- la désactivation et la régénération avec un code valide.

Vérifications backend :

```powershell
dotnet restore BankManagement.sln
dotnet build BankManagement.sln --configuration Release -warnaserror
dotnet test BankManagement.sln --configuration Release
dotnet publish Bank.Api/Bank.Api.csproj --configuration Release
```

Vérifications frontend :

```powershell
npm ci
npm run build
```

## 10. Lancement local

```powershell
dotnet run --project Bank.Api
```

- Swagger : `http://localhost:5176/swagger`
- Santé : `http://localhost:5176/api/health`

Identifiants de formation :

```text
admin@bank.local
Admin123!
```

Ces identifiants sont réservés à la formation et ne doivent pas être utilisés
avec des données réelles.
