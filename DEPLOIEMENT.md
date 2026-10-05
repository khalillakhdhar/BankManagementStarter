# Déploiement Docker et HTTPS

Le projet est installé sur le VPS dans :

```text
/home/ubuntu/khalillakhdhar/BankManagementStarter
```

Les commandes suivantes sont à exécuter depuis ce dossier :

```bash
cd /home/ubuntu/khalillakhdhar/BankManagementStarter
```

## Changer le port local de l'application

Le port publié sur le VPS est défini dans le fichier `.env` :

```env
API_PORT=5081
```

Pour utiliser, par exemple, le port `5090` :

```bash
sed -i 's/^API_PORT=.*/API_PORT=5090/' .env
docker compose up -d --force-recreate api
docker compose ps
curl http://127.0.0.1:5090/api/health
```

Le port `8080` situé à droite dans `docker-compose.yml` est le port interne du
conteneur. Il n'est normalement pas nécessaire de le modifier :

```text
127.0.0.1:PORT_DU_VPS -> conteneur:8080
```

Après un changement de port, modifier aussi `proxy_pass` dans Nginx, puis
recharger Nginx.

## Démarrer et arrêter le backend

```bash
docker compose start
docker compose stop
docker compose ps -a
docker compose logs -f --tail=200
```

`docker compose stop` conserve le volume et les données SQL.

## Premier déploiement

Créer la configuration privée :

```bash
cp .env.example .env
nano .env
chmod 600 .env
```

Définir un mot de passe SQL complexe et unique :

```env
API_PORT=5081
MSSQL_SA_PASSWORD=RemplacerParUnMotDePasseComplexe
```

Valider, construire et démarrer :

```bash
docker compose config --quiet
docker compose pull sqlserver
docker compose build --pull api
docker compose up -d
docker compose ps
curl --fail http://127.0.0.1:5081/api/health
```

Les migrations Entity Framework sont appliquées automatiquement au démarrage de
l'API. Le premier démarrage peut donc prendre quelques secondes.

## Redéployer une nouvelle version

Après avoir envoyé ou récupéré le nouveau code :

```bash
cd /home/ubuntu/khalillakhdhar/BankManagementStarter
git pull --ff-only
docker compose config --quiet
docker compose build --pull api
docker compose up -d --remove-orphans
docker compose ps
curl --fail http://127.0.0.1:5081/api/health
```

Pour reconstruire sans cache :

```bash
docker compose build --no-cache --pull api
docker compose up -d --remove-orphans
```

Ne pas utiliser `docker compose down -v` sauf si la suppression définitive de la
base SQL est souhaitée. L'option `-v` supprime le volume de données.

## Configurer HTTPS avec Nginx et Let's Encrypt

### 1. Préparer le DNS

Créer un enregistrement DNS de type `A`, par exemple :

```text
bank.example.com -> 135.125.182.42
```

Remplacer `bank.example.com` par le vrai domaine. Vérifier sa résolution :

```bash
getent hosts bank.example.com
```

### 2. Autoriser HTTP et HTTPS dans le pare-feu

```bash
sudo ufw allow OpenSSH
sudo ufw allow 'Nginx Full'
sudo ufw status
```

Ne pas activer UFW à distance avant d'avoir autorisé `OpenSSH`.

### 3. Installer Nginx et Certbot

```bash
sudo apt update
sudo apt install -y nginx certbot python3-certbot-nginx
```

### 4. Créer le reverse proxy

```bash
sudo nano /etc/nginx/sites-available/bank-api
```

Contenu pour `API_PORT=5081` :

```nginx
server {
    listen 80;
    listen [::]:80;
    server_name bank.example.com;

    location / {
        proxy_pass http://127.0.0.1:5081;
        proxy_http_version 1.1;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }
}
```

Activer et vérifier la configuration :

```bash
sudo ln -s /etc/nginx/sites-available/bank-api /etc/nginx/sites-enabled/bank-api
sudo nginx -t
sudo systemctl reload nginx
curl --fail http://bank.example.com/api/health
```

Si le lien existe déjà, `ln` peut afficher `File exists`; ne pas créer un second
lien.

### 5. Obtenir le certificat HTTPS

```bash
sudo certbot --nginx -d bank.example.com
sudo nginx -t
sudo systemctl reload nginx
curl --fail https://bank.example.com/api/health
```

Tester le renouvellement automatique :

```bash
sudo certbot renew --dry-run
systemctl status certbot.timer --no-pager
```

L'API reste en HTTP uniquement sur `127.0.0.1`. Le chiffrement HTTPS est terminé
par Nginx, ce qui est la configuration habituelle avec Docker.

## Vérifications et diagnostic

```bash
docker compose ps -a
docker compose logs --tail=200 api
docker compose logs --tail=200 sqlserver
sudo nginx -t
sudo journalctl -u nginx --since '15 minutes ago' --no-pager
curl -v http://127.0.0.1:5081/api/health
curl -v https://bank.example.com/api/health
```
