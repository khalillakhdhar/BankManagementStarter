# Bank Management Starter — .NET 9

Squelette backend destiné au projet final de formation **ASP.NET Core .NET 9 + Angular 21**.

Le projet est volontairement préparé pour que les candidats puissent commencer directement l'exercice :
- architecture créée ;
- fichiers nommés ;
- packages déclarés ;
- SQL Server LocalDB configuré ;
- Swagger configuré ;
- Identity préparé ;
- services enregistrés dans l'injection de dépendances ;
- contrôleurs déjà créés ;
- Models et DTO laissés à compléter.

---

## 1. Prérequis

- Visual Studio 2022 **17.12 ou supérieur**
- Workload **ASP.NET and web development**
- .NET 9 SDK
- SQL Server LocalDB / SQL Server Express / SQL Server Developer
- Git

Pour vérifier .NET :

```powershell
dotnet --version
```

---

## 2. Ouvrir le projet

Ouvrir :

```text
BankManagement.sln
```

dans Visual Studio 2022.

Puis :

**Build > Build Solution**

ou dans PowerShell :

```powershell
dotnet restore
dotnet build
```

---

## 3. Restaurer les dépendances

Les dépendances sont déjà déclarées dans `Bank.Api.csproj`.

Il suffit normalement de lancer :

```powershell
dotnet restore
```

Packages prévus :

```powershell
dotnet add Bank.Api/Bank.Api.csproj package Microsoft.EntityFrameworkCore.SqlServer --version 9.0.0
dotnet add Bank.Api/Bank.Api.csproj package Microsoft.EntityFrameworkCore.Design --version 9.0.0
dotnet add Bank.Api/Bank.Api.csproj package Microsoft.EntityFrameworkCore.Tools --version 9.0.0
dotnet add Bank.Api/Bank.Api.csproj package Microsoft.AspNetCore.Identity.EntityFrameworkCore --version 9.0.0
dotnet add Bank.Api/Bank.Api.csproj package Microsoft.AspNetCore.Authentication.JwtBearer --version 9.0.0
dotnet add Bank.Api/Bank.Api.csproj package Swashbuckle.AspNetCore --version 7.2.0
```

Ces commandes servent surtout si un candidat recrée le projet manuellement.

---

## 4. Base de données

La chaîne de connexion par défaut utilise SQL Server LocalDB :

```json
"DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=BankFormationDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
```

Elle fonctionne généralement directement avec Visual Studio sous Windows.

Si vous utilisez SQL Server Express :

```json
"DefaultConnection": "Server=.\\SQLEXPRESS;Database=BankFormationDb;Trusted_Connection=True;TrustServerCertificate=True"
```

---

# EXERCICE 1

Compléter :

```text
Models/
DTOs/
Enums/TypeTransaction.cs
Data/AppDbContext.cs
```

Les fichiers contiennent des commentaires `TODO EXERCICE`.

Après avoir ajouté les propriétés `Id` dans les entités, décommenter les `DbSet` dans `AppDbContext.cs`.

---

## 5. Migrations avec Visual Studio

Ouvrir :

```text
Tools
> NuGet Package Manager
> Package Manager Console
```

Vérifier :

```text
Default project = Bank.Api
```

Puis :

```powershell
Add-Migration InitialCreate
Update-Database
```

Pour annuler la dernière migration non appliquée :

```powershell
Remove-Migration
```

Pour créer une nouvelle migration :

```powershell
Add-Migration NomDeLaMigration
Update-Database
```

---

## 6. Migrations avec dotnet CLI

Installer l'outil EF si nécessaire :

```powershell
dotnet tool install --global dotnet-ef --version 9.*
```

ou mettre à jour :

```powershell
dotnet tool update --global dotnet-ef --version 9.*
```

Créer la migration :

```powershell
dotnet ef migrations add InitialCreate --project Bank.Api
```

Appliquer la migration :

```powershell
dotnet ef database update --project Bank.Api
```

Lister les migrations :

```powershell
dotnet ef migrations list --project Bank.Api
```

---

## 7. Lancer le backend

Visual Studio :

```text
F5
```

ou :

```text
Ctrl + F5
```

En CLI :

```powershell
dotnet run --project Bank.Api
```

Swagger doit s'ouvrir automatiquement.

URL prévue :

```text
https://localhost:7176/swagger
```

Test rapide :

```text
GET /api/health
```

Réponse attendue :

```json
{
  "status": "OK",
  "project": "Bank.Api",
  "message": "Backend skeleton is running"
}
```

---

## 8. Structure du projet

```text
BankManagementStarter/
├── BankManagement.sln
├── README.md
├── .gitignore
└── Bank.Api/
    ├── Controllers/
    │   ├── AuthController.cs
    │   ├── ClientsController.cs
    │   ├── ComptesController.cs
    │   ├── GuichetsController.cs
    │   ├── HealthController.cs
    │   ├── TransactionsController.cs
    │   └── TypesComptesController.cs
    │
    ├── Data/
    │   └── AppDbContext.cs
    │
    ├── DTOs/
    │   ├── Auth/
    │   ├── Clients/
    │   ├── Comptes/
    │   ├── Guichets/
    │   ├── Transactions/
    │   └── TypesComptes/
    │
    ├── Enums/
    │   └── TypeTransaction.cs
    │
    ├── Models/
    │   ├── ApplicationUser.cs
    │   ├── Client.cs
    │   ├── Compte.cs
    │   ├── Guichet.cs
    │   ├── Transaction.cs
    │   └── TypeCompte.cs
    │
    ├── Services/
    │   ├── Interfaces/
    │   └── Implementations/
    │
    ├── Properties/
    │   └── launchSettings.json
    │
    ├── appsettings.json
    ├── appsettings.Development.json
    ├── Bank.Api.csproj
    ├── GlobalUsings.cs
    └── Program.cs
```

---

## 9. Git — première publication

Dans le dossier racine :

```powershell
git init
git add .
git commit -m "chore: initialize bank management backend skeleton"
git branch -M main
git remote add origin URL_DU_REPOSITORY
git push -u origin main
```

Vérifier :

```powershell
git status
git log --oneline
```

---

## 10. Récupération sur le VPS

Première fois :

```bash
sudo apt update
sudo apt install git -y

mkdir -p ~/apps
cd ~/apps

git clone URL_DU_REPOSITORY bank-management
cd bank-management
git status
```

Mises à jour suivantes :

```bash
cd ~/apps/bank-management
git pull origin main
```

La dockerisation sera faite dans la séance de déploiement.

---

## 11. Travail demandé au candidat

### Models

Compléter :

- `ApplicationUser`
- `Guichet`
- `Client`
- `TypeCompte`
- `Compte`
- `Transaction`
- `TypeTransaction`

Ajouter :
- propriétés ;
- clés étrangères ;
- collections ;
- DataAnnotations ;
- relations.

### DTO

Compléter les DTO contenus dans :

```text
DTOs/Auth
DTOs/Guichets
DTOs/Clients
DTOs/TypesComptes
DTOs/Comptes
DTOs/Transactions
```

### DbContext

Compléter :

```text
Data/AppDbContext.cs
```

Ajouter :
- DbSet ;
- index uniques ;
- précision `decimal(18,2)` ;
- relations ;
- `DeleteBehavior.Restrict`.

### Migration

Créer :

```powershell
Add-Migration InitialCreate
Update-Database
```

---

## 12. Vérification avant remise

Le candidat doit pouvoir exécuter :

```powershell
dotnet restore
dotnet build
dotnet run --project Bank.Api
```

Puis vérifier Swagger et :

```text
GET /api/health
```

Après réalisation des Models :

```powershell
Add-Migration InitialCreate
Update-Database
```

Enfin :

```powershell
git status
git add .
git commit -m "feat: complete models DTOs and initial migration"
git push
```

---

## Important

Le projet contient déjà l'infrastructure minimale mais **les réponses de l'exercice ne sont pas écrites**.

Les fichiers Models et DTO sont volontairement laissés sous forme de squelette avec des commentaires TODO afin que les candidats réalisent eux-mêmes la modélisation.
