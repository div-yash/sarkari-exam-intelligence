# Deployment Guide – Sarkari Exam Intelligence

## Option A: Docker Compose (Recommended — SQL Server + ASP.NET Core API + Angular Nginx)

### 1. Prepare Environment Secrets
```powershell
Copy-Item .env.example .env
```
Edit `.env` and set strong values for:
- `MSSQL_SA_PASSWORD`
- `JWT_KEY` (at least 32+ random characters)
- `PUBLIC_ORIGIN` (e.g., `http://localhost:8080` or `https://exams.yourdomain.in`)
- Optional `SMTP_HOST`, `SMTP_USER`, `SMTP_PASSWORD` for deadline email notifications.

### 2. Build & Start Production Containers
```powershell
docker compose up -d --build
docker compose ps
```
On startup:
- `sei-sqlserver` boots SQL Server 2022 Express with persistent `sqlserver_data` volume.
- `sei-api` waits for SQL Server healthcheck, applies EF Core migrations (`InitialCreate`), seeds all 65 exam families + 26 syllabus subjects + 30 official sources, and launches Quartz.NET deadline checks.
- `sei-web` serves the compiled Angular 21 production bundle via Nginx on `http://localhost:8080` and proxies `/api/*` and `/notifications/*` to `sei-api:8080`.

### 3. Verify Running Stack
```powershell
curl http://localhost:8080/api/health
```
To view logs or stop:
```powershell
docker compose logs -f api
docker compose down
```
*(Volumes `sqlserver_data`, `api_uploads`, and `api_logs` remain preserved unless `--volumes` is passed.)*

---

## Option B: Windows Server / IIS or Linux VPS (Non-Docker)

### 1. Build Self-Contained Publish Folder
```powershell
.\deploy\scripts\publish.ps1
```
This outputs:
- `publish/api` (ASP.NET Core Release build + `web.config` for IIS `AspNetCoreModuleV2`)
- `publish/frontend` (Compiled Angular browser bundle + `web.config` SPA fallback & ARR proxy rules)
- `publish/schema-idempotent.sql` (Idempotent SQL Server DDL script)

### 2. Deploy to Windows IIS
1. Install **.NET 10 Hosting Bundle** + **URL Rewrite** + **Application Request Routing (ARR)** on IIS.
2. Apply `publish/schema-idempotent.sql` (or run `dotnet ef database update`).
3. Create an IIS site for `publish/api` (No Managed Code AppPool) on `127.0.0.1:5019` and set environment variables (`ConnectionStrings__DefaultConnection`, `Jwt__Key`, `ASPNETCORE_ENVIRONMENT=Production`).
4. Create the public IIS site pointing to `publish/frontend`.

### 3. Deploy to Linux VPS (Nginx + systemd)
Use `deploy/nginx/sarkari-exam-vps.conf` with Certbot SSL certificates and proxy `/api/` and `/notifications/` to Kestrel on `127.0.0.1:5019`.
