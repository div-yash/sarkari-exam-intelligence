# Sarkari Exam Intelligence – Complete Government Exam Tracker

A full-stack Indian Government Exam discovery, eligibility verification, deadline tracking, syllabus mastery, vacancy analytics, and notification platform built for **all Indian candidates** — with a pre-configured evaluation profile for a **B.Tech Information Technology graduate from Bihar (working professional)**.

---

## Live Production Deployment

* **Web Application (Vercel):** https://sarkari-exam-intelligence.vercel.app
* **Backend API (Render):** https://sarkari-exam-intelligence.onrender.com
* **API Health Check:** https://sarkari-exam-intelligence.onrender.com/api/health
* **Swagger / OpenAPI Docs:** https://sarkari-exam-intelligence.onrender.com/swagger

> **Free Hosting Note:** The backend runs on Render's free tier, which spins down after 15 minutes of inactivity. The very first sign-in or API request after an idle period can take ~45–60 seconds while the container wakes up.

---

## Seeded Accounts & Sign-In

You can register your own candidate profile at `/auth/register` or sign in directly using the two seeded accounts (also available as one-click buttons on the login page):

| Role | Email | Password | Profile Details |
|---|---|---|---|
| **Candidate** | `candidate@sarkariexam.in` | `Candidate@12345` | B.Tech Information Technology, Bihar Domicile, OBC-NCL, 36 months IT work experience |
| **Admin** | `admin@sarkariexam.in` | `Admin@12345` | System Admin & Verification Console access + Candidate profile |

---

## Key Features

### 1. Executive Dashboard
* Real-time KPI widgets: Total Exams Tracked (`65`), Currently Open Applications, Upcoming Notifications, Closing Within 7 Days, Closing Within 30 Days, Exams Matching My Qualification, Saved Examinations, Newly Released Notifications, Total Vacancies Announced, and Overall Preparation Progress.
* Prominent urgent deadline alerts banner and interactive Chart.js visualizations.

### 2. Strict Rule-Based Eligibility Engine
* Does **not** blindly mark B.Tech graduates eligible for every exam.
* Validates **exact degree & engineering branch** (e.g., IT vs. Civil/Mechanical/Electrical), **minimum percentage/CGPA**, **age cutoff** calculated accurately on the notification's cutoff date (`AgeRelaxation` rules including category and domicile constraints such as Bihar-only OBC/state relaxation), **GATE score requirements**, **work experience**, and **nationality**.
* Clearly reports `Eligible`, `PotentiallyEligibleVerificationRequired`, or `NotEligible` with specific reasons.

### 3. Exam Directory & Detailed Exam Pages
* Search, filter, bookmark/save, and inspect **65 examination families** across **7 categories** with official links, selection process, group classification (Group A/B/C), salary pay level, age limits, fee structure, negative marking, application/exam/admit card/result dates, and multi-year historical vacancy cycles.

### 4. Preparation, Syllabus & Analytics
* **26 structured syllabus subjects** with topic-level completion, confidence, and revision tracking.
* Study plan hours, mock test score logging & percentage analytics, and application stage tracker (`Planning` → `Applied` → `AdmitCard` → `Appeared` → `Selected`).
* **Calendar & Export:** Monthly timeline of application deadlines and exam dates with one-click **`.ics` calendar export**.
* **Vacancy Trends:** Multi-year historical vacancy comparison across cycles and categories.

### 5. Admin Verification & Audit Governance
* Create/update exam records, application windows, vacancies, statuses, syllabus links, and upload official PDFs.
* Protected by `AdminOnly` authorization policy with full `AuditLogs` change history and protection against silently overwriting verified historical cycles.

---

## How Notifications & Upcoming Opportunities Are Maintained

The platform combines **curated official-source verification** with an **automated Quartz.NET alert orchestrator** so users receive accurate, duplicate-free reminders without relying on fragile scraping of unverified blogs.

### Stage 1: Discovering & Verifying Exam Opportunities
1. **Pre-Seeded Catalog (`DatabaseSeeder`):**
   * Seeds **65 exam families**, **30 official government portals** (`OfficialSources` for UPSC, SSC, IBPS, SBI, RBI, SEBI, RRB, BPSC, BSSC, ISRO, DRDO, NIC, NIELIT, etc.), **26 syllabus subjects**, and current/historical recruitment cycles.
2. **Respectful Official Portal Change Detection (`SourceVerificationService`):**
   * From the Admin Verification Console, an Admin triggers **Verify Source** on an official portal.
   * `SourceVerificationService` enforces per-source rate limits (`RateLimitSeconds`), sends a respectful HTTP `GET` (`SarkariExamIntelligence-VerificationBot/1.0`), halts cleanly if a portal returns `403`/`429` or uses CAPTCHA/anti-bot protection (`ManualVerificationRequired`), and computes a **SHA-256 content hash** (`LastContentHash`).
   * If the portal content hash changes (`ChangesDetected`), linked current-cycle notifications are flagged `NeedsReverification` (`IsPendingApproval = true`) for Admin review.
3. **Admin Curation & Publishing (`AdminService`):**
   * Admins review official notices/PDFs and create or update exam dates, vacancies, and statuses (`UpcomingNotification`, `ApplicationsOpen`, `ClosingSoon`, `AdmitCardReleased`, `ResultDeclared`, etc.).
   * As soon as an exam is approved (`IsApprovedForPublish = true`), it appears across the Dashboard, Directory, Calendar, Eligibility Engine, and Notification Engine.

### Stage 2: Automated User Notification Engine (`QuartzNotificationCheckJob`)
* **Background Schedule:** Powered by **Quartz.NET**, `QuartzNotificationCheckJob` executes **15 seconds after API startup** and repeats **every 6 hours** (and can also be invoked manually via `/api/notifications/run-check` or `/api/admin/notifications/dispatch-check`).
* **Eligibility + Saved Filtering (`NotificationOrchestratorService`):**
  * Runs using IST date (`TodayIst`) and evaluates each published exam against each candidate's profile.
  * Generates alerts **only** for exams the user **Saved** or is **not `NotEligible`** for.
* **9 Automated Alert Triggers:**
  1. **New Official Notification** (`NotificationDate` within the last 7 days)
  2. **Applications Opened** (today within `[ApplicationStartDate, ApplicationEndDate]`)
  3. **30-Day Deadline Reminder** (`8–30` days before `ApplicationEndDate`)
  4. **7-Day Urgent Reminder** (`4–7` days before `ApplicationEndDate`)
  5. **3-Day Critical Alert** (`2–3` days before `ApplicationEndDate`)
  6. **1-Day / Final Day Alert** (`0–1` day before `ApplicationEndDate`)
  7. **Exam Date / Notice Change** (`LastChangedAtUtc` updated within the last 7 days with a change summary)
  8. **Admit Card Released** (`Status == AdmitCardReleased`)
  9. **Result Declared** (`Status == ResultDeclared`)
* **Strict Deduplication:** Every alert is assigned a deterministic `DeduplicationKey` (e.g., `deadline-7d:{notifId}:{yyyyMMdd}`), guaranteeing repeated 6-hour runs never create duplicate alerts.

### Important Operational Notes on Alerts in Free Cloud Hosting
* **In-App vs. Email Delivery:** In-app notifications are always stored in the database and visible inside the app. Real SMTP email delivery (`EmailSenderService`) is sent only when **both** `Email__SmtpHost` (plus SMTP port/credentials) is configured on the server **and** the user has set an `OverrideEmail` in their notification preferences (otherwise email delivery is simulated in logs).
* **Render Free Tier Sleep:** Because Render free web services sleep after 15 minutes of inactivity, the in-process Quartz 6-hour schedule runs whenever the service is awake and 15 seconds after each wake-up boot.

---

## Architecture & Complete Tech Stack

### Frontend (`/frontend`)
* **Framework:** Angular `^21.2.0` (Standalone Components, Signals, Route Guards, HTTP Interceptors, Lazy-loaded Routes)
* **Language & Reactive State:** TypeScript `~5.9.2`, RxJS `~7.8.0`
* **Styling & UI:** Tailwind CSS `^4.1.0` (`@tailwindcss/postcss`)
* **Charts & Analytics:** Chart.js `^4.4.7`
* **Unit Testing:** Vitest `^4.0.8` + jsdom
* **Hosting:** Vercel (`frontend/vercel.json` SPA rewrites)

### Backend (`/backend`)
* **Framework & Runtime:** ASP.NET Core Web API targeting **.NET 10** (`net10.0`)
* **Architecture:** Clean Architecture split into 4 layers:
  * `SarkariExamIntelligence.Domain` (Entities, Value Enums, BaseEntity)
  * `SarkariExamIntelligence.Application` (Services, Eligibility Engine, Notification Orchestrator, DTOs, Exceptions, Interfaces)
  * `SarkariExamIntelligence.Infrastructure` (EF Core DbContext, Identity, JWT, Quartz.NET Job, Source Verifier, Email Sender, Seeders)
  * `SarkariExamIntelligence.API` (Controllers, `GlobalExceptionMiddleware`, Serilog, CORS, ForwardedHeaders, Swagger)
* **ORM & Database Providers:** Entity Framework Core `8.0.0` supporting **both**:
  * **PostgreSQL** (`Npgsql.EntityFrameworkCore.PostgreSQL` `8.0.0`) — auto-detected from `postgres://`, `postgresql://`, or `Host=` connection strings (used in production with **Neon.tech**)
  * **Microsoft SQL Server** (`Microsoft.EntityFrameworkCore.SqlServer` `8.0.0`) with EF Core Code First Migrations (`InitialCreate`) for local/enterprise SQL Server deployments
* **Authentication & Authorization:** ASP.NET Core Identity + JWT Bearer (`Microsoft.AspNetCore.Authentication.JwtBearer` `8.0.0`) + role-based `AdminOnly` policy
* **Background Processing:** Quartz.NET (`Quartz.Extensions.Hosting` `3.13.1`)
* **Logging & API Docs:** Serilog (`Serilog.AspNetCore` `8.0.1`, Console + rolling daily file logs), Swashbuckle / OpenAPI
* **Containerization & Hosting:** Multi-stage root `Dockerfile` (`mcr.microsoft.com/dotnet/sdk:10.0` → `aspnet:10.0`), `render.yaml`

---

## Tracked Examination Database (65 Exams across 7 Categories)

1. **UPSC (8):** Civil Services Examination (CSE), Indian Forest Service (IFoS), CAPF Assistant Commandant, CDS, NDA, EPFO EO/AO, EPFO APFC, Engineering Services Examination (ESE)
2. **SSC (7):** SSC CGL, SSC CPO, SSC CHSL, SSC MTS, SSC GD Constable, SSC Selection Post, SSC Stenographer
3. **Banking (9):** IBPS PO, IBPS CSA, IBPS SO IT Officer, IBPS RRB PO, IBPS RRB Clerk, SBI PO, SBI Junior Associate, RBI Grade B, RBI Assistant
4. **Financial Regulators & Insurance (11):** SEBI Grade A IT, SEBI Grade A General, NABARD Grade A, NABARD Grade B, PFRDA Grade A, IRDAI Assistant Manager, IFSCA Grade A, SIDBI Grade A/B, LIC AAO, NIACL AO, GIC Assistant Manager
5. **Railways (6):** RRB NTPC Graduate, RRB NTPC Undergraduate, RRB JE, RRB ALP, RRB Technician, RRB Group D
6. **Bihar Government (12):** BPSC CCE, BPSC Assistant Section Officer, BPSC Auditor, BPSC Technical Recruitment, BSSC CGL, BSSC Secretariat Assistant, BTSC Technical Recruitment, Bihar Police SI, Bihar Police Constable, Bihar Vidhan Sabha Recruitment, Bihar Secretariat Recruitment, Bihar Government IT & Programmer Posts
7. **Technical & IT Recruitment (12):** GATE CS, NIC Scientist B, NIELIT Scientist B, ISRO Scientist/Engineer, DRDO RAC Scientist B, C-DAC Project Engineer, BEL Recruitment, ECIL Recruitment, RailTel Recruitment, C-DOT Recruitment, PSU Recruitment through GATE, Central & State IT Cadres

---

## Local Development Quick Start

### Option A: Using Local SQL Server (Default `appsettings.json`)
1. **Run Backend API:**
   ```powershell
   cd backend
   dotnet restore
   dotnet ef database update --project src/SarkariExamIntelligence.Infrastructure --startup-project src/SarkariExamIntelligence.API
   dotnet run --project src/SarkariExamIntelligence.API
   ```
   API runs on `http://localhost:5019` (Swagger UI at `http://localhost:5019/swagger`).

2. **Run Angular Frontend:**
   ```powershell
   cd frontend
   npm install
   npm start
   ```
   Frontend runs on `http://localhost:4200` and proxies `/api` calls to `http://localhost:5019` via `frontend/proxy.conf.json`.

### Option B: Using PostgreSQL / Neon.tech Locally
Set `ConnectionStrings__DefaultConnection` before starting the API (no `appsettings.json` edits required):
```powershell
$env:ConnectionStrings__DefaultConnection="postgresql://USER:PASSWORD@HOST/neondb?sslmode=require"
dotnet run --project backend/src/SarkariExamIntelligence.API
```
`DependencyInjection.cs` automatically detects the PostgreSQL URI, normalizes it for `Npgsql`, and `DatabaseSeeder` creates the schema and seeds all data on first boot.

---

## Testing & Verification

```powershell
# Frontend unit tests (Vitest) & production build
npm --prefix frontend test -- --watch=false
npm --prefix frontend run build

# Backend NUnit Unit & Integration Tests
dotnet test backend/tests/SarkariExamIntelligence.UnitTests/SarkariExamIntelligence.UnitTests.csproj -c Release
dotnet test backend/tests/SarkariExamIntelligence.IntegrationTests/SarkariExamIntelligence.IntegrationTests.csproj -c Release

# Full 32-check Backend Verification Suite
dotnet run --project backend/tests/SarkariExamIntelligence.VerificationSuite/SarkariExamIntelligence.VerificationSuite.csproj -c Release
```

---

## Additional Documentation

* **Free Cloud Deployment (Neon.tech + Render.com + Vercel):** [docs/FREE_CLOUD_DEPLOYMENT.md](docs/FREE_CLOUD_DEPLOYMENT.md)
* **Docker / VPS / IIS Deployment:** [docs/DEPLOYMENT.md](docs/DEPLOYMENT.md)
* **System Architecture:** [docs/ARCHITECTURE.md](docs/ARCHITECTURE.md)
* **REST API Reference:** [docs/API_REFERENCE.md](docs/API_REFERENCE.md)
* **Database Backup & Recovery:** [docs/DATABASE_BACKUP_AND_RECOVERY.md](docs/DATABASE_BACKUP_AND_RECOVERY.md)
