# Authentification à deux facteurs — conception

## Intention

Ajouter une authentification à deux facteurs TOTP destinée à la formation. Les
utilisateurs pourront associer Google Authenticator à leur compte, puis fournir
un code temporaire après leur mot de passe lors des connexions suivantes.

## Objectifs

- Utiliser le fournisseur Authenticator intégré à ASP.NET Core Identity.
- Permettre l’activation depuis la page de profil après authentification.
- Afficher la clé manuelle et un QR code `otpauth://` compatible avec Google
  Authenticator.
- Exiger un premier code valide avant d’activer réellement le 2FA.
- Transformer la connexion en deux étapes lorsque le 2FA est actif.
- Fournir des codes de récupération à usage unique lors de l’activation.
- Permettre la régénération des codes et la désactivation du 2FA après saisie
  d’un code TOTP valide.
- Documenter dans les README qu’aucune API key Google n’est nécessaire et
  expliquer l’installation et l’association de Google Authenticator.

## Non-objectifs

- SMS, e-mail, notifications push ou OAuth Google.
- Appareils de confiance persistants et contournement automatique du 2FA.
- Réinitialisation administrative du 2FA dans cette première version.
- Modification du mécanisme JWT utilisé après authentification complète.

## Approches considérées

### ASP.NET Core Identity — retenue

Utiliser `ResetAuthenticatorKeyAsync`, `GetAuthenticatorKeyAsync`,
`VerifyTwoFactorTokenAsync`, `SetTwoFactorEnabledAsync` et les recovery codes
fournis par `UserManager`. Cette solution réutilise la sécurité déjà présente,
évite une bibliothèque cryptographique supplémentaire et reste pédagogique.

### Otp.NET personnalisé — écartée

Générer et vérifier nous-mêmes les secrets TOTP donnerait plus de contrôle mais
dupliquerait les responsabilités d’Identity et augmenterait le code sensible à
maintenir sans bénéfice pour le besoin de formation.

## Architecture backend

`POST /api/auth/login` conserve la vérification e-mail/mot de passe. Pour un
compte sans 2FA, il renvoie la session JWT actuelle. Pour un compte avec 2FA,
il renvoie un challenge opaque court, sans JWT. Le challenge est un jeton signé
et limité dans le temps contenant uniquement l’identifiant utilisateur et un
marqueur d’usage `two-factor-login`.

`POST /api/auth/login/2fa` reçoit le challenge et un code Authenticator ou un
code de récupération. Après validation, il émet le JWT normal. Le challenge
n’est jamais un jeton d’accès aux routes métier.

Les routes authentifiées suivantes complètent le cycle :

- `GET /api/auth/2fa/status` : état courant ;
- `POST /api/auth/2fa/setup` : crée/réinitialise la clé tant que le 2FA n’est
  pas actif et renvoie clé manuelle + URI `otpauth` ;
- `POST /api/auth/2fa/enable` : vérifie un TOTP, active le 2FA et renvoie les
  codes de récupération une seule fois ;
- `POST /api/auth/2fa/recovery-codes` : régénère les codes après vérification
  d’un TOTP ;
- `POST /api/auth/2fa/disable` : vérifie un TOTP, désactive le 2FA et réinitialise
  la clé ainsi que les codes de récupération.

Les codes acceptent les espaces et tirets d’affichage, qui sont retirés avant
validation. Les erreurs ne révèlent pas si l’adresse e-mail, le challenge ou le
code était la partie incorrecte.

## Architecture frontend

Le modèle de connexion devient une union : session JWT complète ou challenge
2FA. Après le mot de passe, l’écran de connexion affiche un second formulaire
pour le code à six chiffres, avec une option explicite permettant d’utiliser un
code de récupération. Aucun challenge n’est écrit dans `localStorage` ; il
reste uniquement en mémoire pendant l’étape de connexion.

La page Profil affiche l’état du 2FA. Quand il est désactivé, l’utilisateur peut
demander la configuration, scanner le QR code, recopier la clé si nécessaire,
puis confirmer un code. Après activation, les recovery codes sont affichés une
seule fois avec une invitation claire à les conserver hors de l’application.
Quand il est actif, la page permet de régénérer les codes ou de désactiver le
2FA après validation d’un code Authenticator.

Le QR code est produit localement dans le navigateur depuis l’URI `otpauth` ;
aucune donnée n’est envoyée à un service de QR code externe.

## Données et sécurité

Les secrets Authenticator et les codes de récupération sont stockés par les
tables ASP.NET Core Identity existantes. Aucune nouvelle table métier n’est
nécessaire. Les réponses contenant la clé ou les recovery codes ne doivent pas
être journalisées. Le challenge de connexion expire après cinq minutes et est
signé avec ASP.NET Core Data Protection.

## Tests attendus

- Contrats d’autorisation de toutes les nouvelles routes.
- Connexion normale inchangée pour un utilisateur sans 2FA.
- Un utilisateur 2FA ne reçoit aucun JWT après le seul mot de passe.
- Challenge expiré, altéré ou destiné à un autre usage refusé.
- TOTP invalide refusé ; TOTP valide accepté.
- Recovery code valide consommé une seule fois.
- Activation impossible sans premier TOTP valide.
- Désactivation et régénération protégées par un TOTP valide.
- Frontend : transitions mot de passe → challenge → session, erreurs et retour
  au formulaire initial.
- Build Angular, tests .NET et smoke tests des routes existantes sans régression.

## Contraintes

- Backend .NET 9 et ASP.NET Core Identity existants.
- Frontend Angular 21 standalone et formulaires template-driven existants.
- Deux branches du même nom dans les deux dépôts : `feature/two-factor-auth`.
- Aucun déploiement automatique de cette branche en production.

