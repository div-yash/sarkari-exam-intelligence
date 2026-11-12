# Sarkari Exam Intelligence – System Architecture

## Clean Architecture Layers

1. **SarkariExamIntelligence.Domain** (`net10.0`, zero external dependencies)
   - `BaseEntity` with `Id`, `CreatedAtUtc`, `UpdatedAtUtc`, `CreatedBy`, `UpdatedBy`, `IsDeleted`
   - 19 strongly typed domain enums covering all 11 lifecycle statuses, verification status, data confidence, eligibility results, job nature, group classification, and notification channels
   - 29 domain entities modeling UserProfiles, Categories, ExamOrganizations, OfficialSources, SourceVerificationLogs, Exams, ExamNotifications, ExamDates, EligibilityRules, EducationalQualifications, AgeRelaxations, ExamVacancies, CategoryWiseVacancies, SyllabusSubjects, SyllabusTopics, ExamSyllabi, ExamPatterns, HistoricalRecruitments, HistoricalCategoryVacancies, PreviousYearCutoffs, PreviousYearPapers, SavedExams, UserApplications, UserStudyPlans, UserStudyProgresses, MockTestResults, Notifications, NotificationPreferences, and AuditLogs

2. **SarkariExamIntelligence.Application**
   - Service interfaces (`IEligibilityEngine`, `IExamQueryService`, `IDashboardService`, `ICalendarService`, `IPreparationService`, `IAnalyticsService`, `INotificationOrchestratorService`, `IAdminService`, `ISourceVerificationService`)
   - Deterministic `EligibilityEngineService` enforcing exact cutoff-date age calculation, branch-specific validation, CGPA/percentage checks, GATE checks, work experience validation, and Bihar domicile/certificate verification
   - `ApiResponse<T>` & `PagedResult<T>` wrappers and structured domain exceptions

3. **SarkariExamIntelligence.Infrastructure**
   - `AppDbContext` inheriting `IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>` with explicit SQL Server Fluent configurations, unique indexes, decimal precision, and cascade-safe delete behavior
   - EF Core Code First Migration `InitialCreate` in `Persistence/Migrations`
   - Idempotent seeders (`SyllabusLibrarySeeder`, `OfficialSourceSeeder`, `ExamCatalogPart1`, `ExamCatalogPart2`, `DatabaseSeeder`)
   - Respectful rate-limited `SourceVerificationService` with SHA-256 content-hash change detection and CAPTCHA/bot-wall respect
   - `NotificationCheckBackgroundService` executing deduplicated alert sweeps across 30d/7d/3d/1d deadlines, newly published notifications, open windows, admit cards, results, and date updates

4. **SarkariExamIntelligence.API**
   - 13 RESTful controllers under `/api/*`, Swagger UI with JWT Bearer Auth, Serilog structured console + rolling file logging, and `GlobalExceptionMiddleware`

5. **Frontend (`frontend/`, Angular 21 + Tailwind CSS v4 + Chart.js + RxJS)**
   - Standalone components, functional JWT interceptor, `authGuard` / `adminGuard`, dark navy & white theme with semantic status badges, responsive sidebar with all 12 core routes + Admin Console.
