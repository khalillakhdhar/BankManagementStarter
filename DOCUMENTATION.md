# Bank Management API — documentation complète

## 1. Présentation

Cette application est une API REST ASP.NET Core .NET 9 utilisant Entity Framework Core, SQL Server, ASP.NET Core Identity, JWT et Swagger.

Docker est **optionnel**. Deux modes sont disponibles :

- exécution locale avec le SDK .NET et un SQL Server local ;
- exécution isolée avec Docker Compose et SQL Server en conteneur.

## 2. Fonctionnalités

- gestion des clients ;
- gestion des guichets ;
- gestion des types de comptes ;
- ouverture et activation/désactivation des comptes ;
- dépôts, retraits et virements ;
- historique des transactions ;
- authentification JWT ;
- autorisation par rôles `Admin` et `Agent` ;
- création automatique d'un administrateur initial ;
- documentation interactive Swagger.

## 3. Prérequis locaux

- .NET 9 SDK ;
- SQL Server Express, Developer ou LocalDB ;
- l'outil `dotnet-ef` pour gérer les migrations.

Installation de l'outil EF si nécessaire :

```powershell
dotnet tool install --global dotnet-ef --version 9.*
```

Les packages NuGet sont déjà référencés dans `Bank.Api.csproj`. Aucune installation manuelle supplémentaire n'est requise.

## 4. Exécution locale sans Docker

Vérifier la chaîne de connexion `ConnectionStrings:DefaultConnection` dans `Bank.Api/appsettings.json`, puis lancer :

```powershell
dotnet restore
dotnet ef database update --project Bank.Api
dotnet run --project Bank.Api
```

Adresses locales :

- Swagger HTTPS : `https://localhost:7176/swagger`
- Swagger HTTP : `http://localhost:5176/swagger`
- Health-check : `https://localhost:7176/api/health`

L'application applique aussi automatiquement les migrations au démarrage. La commande `database update` reste utile pour voir immédiatement les erreurs de migration.

## 5. Administrateur initial

Au premier démarrage, l'application :

1. applique les migrations EF Core ;
2. crée les rôles `Admin` et `Agent` s'ils n'existent pas ;
3. crée l'administrateur initial s'il n'existe pas ;
4. lui attribue le rôle `Admin`.

Identifiants locaux de démonstration :

```text
Email : admin@bank.local
Mot de passe : Admin123!
```

Ces valeurs proviennent de la section `DefaultAdmin` de `appsettings.json`. Elles doivent être remplacées avant toute utilisation réelle. Le seed ne remplace pas le mot de passe d'un administrateur déjà créé.

## 6. Authentification dans Swagger

1. Ouvrir Swagger.
2. Exécuter `POST /api/Auth/login` avec :

```json
{
  "email": "admin@bank.local",
  "password": "Admin123!"
}
```

3. Copier la valeur `token` de la réponse.
4. Cliquer sur **Authorize** en haut de Swagger.
5. Coller uniquement le token, sans ajouter le mot `Bearer`.
6. Valider.

Le token contient l'identifiant utilisateur et ses rôles. Sa durée par défaut est de 120 minutes.

## 7. Autorisations

- `POST /api/Auth/login` et `GET /api/Health` sont publics.
- Les contrôleurs métier exigent un utilisateur authentifié.
- La création d'agents exige le rôle `Admin`.
- La création, modification et désactivation des guichets et types de comptes exigent le rôle `Admin`.
- Les transactions utilisent automatiquement l'identifiant du token comme `AgentId`.

Création d'un agent par l'administrateur :

```http
POST /api/Auth/agents
Authorization: Bearer <token-admin>
```

```json
{
  "nom": "Ben Ali",
  "prenom": "Sami",
  "email": "agent@bank.local",
  "password": "Agent123!",
  "guichetId": null
}
```

## 8. Configuration JWT

La section `Jwt` configure :

- `Secret` : clé de signature ;
- `Issuer` : émetteur du token ;
- `Audience` : destinataire du token ;
- `ExpirationMinutes` : durée de validité.

La clé présente dans `appsettings.json` est uniquement destinée au développement. En production, utiliser une variable d'environnement ou un gestionnaire de secrets :

```powershell
$env:Jwt__Secret = "une-cle-secrete-longue-et-aleatoire"
dotnet run --project Bank.Api
```

Le double underscore `__` représente `:` dans la configuration ASP.NET Core.

## 9. Exécution optionnelle avec Docker

Docker n'est pas utilisé par le lancement local. Il intervient uniquement quand une commande Docker est exécutée.

Copier le modèle de variables :

```powershell
Copy-Item .env.example .env
```

Modifier obligatoirement les secrets dans `.env`, puis lancer :

```powershell
docker compose up --build -d
```

Swagger Docker :

```text
http://localhost:8080/swagger
```

Commandes utiles :

```powershell
docker compose ps
docker compose logs -f api
docker compose stop
docker compose down
```

Pour supprimer aussi les données SQL du conteneur :

```powershell
docker compose down -v
```

Attention : l'option `-v` supprime le volume de base de données et les données deviennent irrécupérables sans sauvegarde.

## 10. Variables Docker

| Variable | Usage | Valeur de démonstration |
|---|---|---|
| `API_PORT` | Port HTTP exposé par l'API | `8080` |
| `SQL_PORT` | Port SQL Server exposé | `1433` |
| `MSSQL_SA_PASSWORD` | Mot de passe administrateur SQL | à remplacer |
| `JWT_SECRET` | Signature des JWT | à remplacer |
| `ADMIN_EMAIL` | Email de l'administrateur initial | `admin@bank.local` |
| `ADMIN_PASSWORD` | Mot de passe initial | à remplacer |

Le fichier `.env` est exclu de l'image Docker et ne doit pas être versionné. `.env.example` ne contient que des exemples.

## 11. Architecture Docker

Compose démarre deux services :

- `sqlserver` : SQL Server 2022 Developer avec un volume persistant ;
- `api` : image ASP.NET Core .NET 9 construite en plusieurs étapes.

L'API attend que le health-check SQL Server réussisse, applique les migrations, initialise Identity puis démarre sur le port interne `8080`.

## 12. Ordre conseillé pour tester le métier

1. Se connecter comme administrateur.
2. Créer un guichet.
3. Créer un type de compte.
4. Créer un client.
5. Ouvrir un compte pour ce client.
6. Créer un agent si nécessaire.
7. Effectuer un dépôt, retrait ou virement.
8. Consulter l'historique du compte.

## 13. Compilation et diagnostic

```powershell
dotnet clean
dotnet restore
dotnet build BankManagement.sln
dotnet ef migrations list --project Bank.Api
```

Si Docker ne démarre pas :

```powershell
docker compose config
docker compose ps
docker compose logs sqlserver
docker compose logs api
```

Si le port `1433` est déjà utilisé localement, modifier `SQL_PORT` dans `.env`, par exemple `SQL_PORT=1434`. La connexion interne entre les conteneurs continue d'utiliser `1433`.

## 14. Recommandations de production

- ne jamais conserver les mots de passe ou la clé JWT du projet de démonstration ;
- utiliser un coffre de secrets ou des variables injectées par la plateforme ;
- activer HTTPS via un reverse proxy ;
- limiter CORS aux domaines du frontend ;
- appliquer une politique de renouvellement des secrets ;
- sauvegarder le volume SQL Server ;
- ne pas exposer directement le port SQL Server si ce n'est pas nécessaire ;
- désactiver Swagger en production si l'organisation l'exige.
