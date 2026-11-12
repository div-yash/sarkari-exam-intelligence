# Sarkari Exam Intelligence – Complete Government Exam Tracker

Production-quality Indian Government Exam discovery, eligibility evaluation, deadline tracking, syllabus mastery, historical vacancy analytics, notification engine, and admin governance platform.

## Architecture & Tech Stack

- **Frontend:** Angular 21 (Standalone Components, Signals, RxJS), TypeScript, Tailwind CSS v4, Chart.js
- **Backend:** ASP.NET Core Web API (.NET 10), Clean Architecture (`Domain`, `Application`, `Infrastructure`, `API`), C#, Entity Framework Core 8, ASP.NET Core Identity, JWT Bearer Authentication, Serilog, Swagger / OpenAPI
- **Database:** Microsoft SQL Server with EF Core Code First Migrations (`InitialCreate`)
- **Background Processing:** Scheduled notification & deadline alert engine (`NotificationCheckBackgroundService`) with strict deduplication (`DeduplicationKey`)

## Tracked Examination Families (65 Exams across 7 Categories)

1. **UPSC (8):** CSE, IFoS, CAPF AC, CDS, NDA, EPFO EO/AO, EPFO APFC, ESE
2. **SSC (7):** CGL, CPO, CHSL, MTS, GD Constable, Selection Post, Stenographer
3. **Banking (9):** IBPS PO, IBPS CSA, IBPS SO IT Officer, IBPS RRB PO, IBPS RRB Clerk, SBI PO, SBI Junior Associate, RBI Grade B, RBI Assistant
4. **Financial Regulators & Insurance (11):** SEBI Grade A IT, SEBI Grade A General, NABARD Grade A/B, PFRDA Grade A, IRDAI AM, IFSCA Grade A, SIDBI Grade A/B, LIC AAO, NIACL AO, GIC Assistant Manager
5. **Railways (6):** RRB NTPC Graduate, RRB NTPC Undergraduate, RRB JE, RRB ALP, RRB Technician, RRB Group D
6. **Bihar Government (12):** BPSC CCE, BPSC ASO, BPSC Auditor, BPSC Technical AE, BSSC CGL, BSSC Secretariat Assistant, BTSC Technical, Bihar Police SI, Bihar Police Constable, Bihar Vidhan Sabha, Bihar Secretariat, Bihar IT & Programmer
7. **Technical & IT Recruitment (12):** GATE CS, NIC Scientist B, NIELIT Scientist B, ISRO Scientist/Engineer, DRDO RAC Scientist B, C-DAC Project Engineer, BEL, ECIL, RailTel, C-DOT, PSU via GATE, Central/State IT Cadres

## Quick Start

### 1. Database & Backend API
```powershell
cd backend
dotnet restore
dotnet ef database update --project src/SarkariExamIntelligence.Infrastructure --startup-project src/SarkariExamIntelligence.API
dotnet run --project src/SarkariExamIntelligence.API
```
API listens on `http://localhost:5019` with Swagger UI at `http://localhost:5019/swagger`.

### 2. Angular Frontend
```powershell
cd frontend
npm install
npm start
```
Frontend runs on `http://localhost:4200` and proxies `/api` to `http://localhost:5019`.

### 3. Cloud & Container Deployment Guides
- **100% Free Public Hosting (Neon.tech + Render.com + Vercel):** See [docs/FREE_CLOUD_DEPLOYMENT.md](docs/FREE_CLOUD_DEPLOYMENT.md)
- **Docker Compose / IIS / Linux VPS:** See [docs/DEPLOYMENT.md](docs/DEPLOYMENT.md)

### 4. Seeded Accounts for Evaluation
- **Candidate (B.Tech IT – Bihar Working Professional):** `candidate@sarkariexam.in` / `Candidate@12345`
- **Admin (Source Verification & Governance):** `admin@sarkariexam.in` / `Admin@12345`
- All Indian candidates can register freely via `/auth/register`.
