# Database Backup, Recovery & Operations Guidance (SQL Server)

## 1. Automated Backup Strategy

### Daily Full Backup (T-SQL / SQL Server Agent)
```sql
BACKUP DATABASE [SarkariExamIntelligenceDb]
TO DISK = N'D:\Backups\SQL\SarkariExamIntelligence_Full.bak'
WITH COMPRESSION, CHECKSUM, STATS = 10, INIT;
```

### Hourly Transaction Log Backup (Full Recovery Model)
```sql
ALTER DATABASE [SarkariExamIntelligenceDb] SET RECOVERY FULL;

BACKUP LOG [SarkariExamIntelligenceDb]
TO DISK = N'D:\Backups\SQL\SarkariExamIntelligence_Log.trn'
WITH COMPRESSION, CHECKSUM;
```

## 2. Integrity Verification Before Retention
```sql
RESTORE VERIFYONLY
FROM DISK = N'D:\Backups\SQL\SarkariExamIntelligence_Full.bak'
WITH CHECKSUM;
```

## 3. Point-in-Time Restore Procedure
```sql
RESTORE DATABASE [SarkariExamIntelligenceDb]
FROM DISK = N'D:\Backups\SQL\SarkariExamIntelligence_Full.bak'
WITH NORECOVERY, REPLACE;

RESTORE LOG [SarkariExamIntelligenceDb]
FROM DISK = N'D:\Backups\SQL\SarkariExamIntelligence_Log.trn'
WITH RECOVERY, STOPAT = '2026-11-11T14:00:00';
```

## 4. Historical Audit & Non-Destructive Rules
- `HistoricalRecruitments` enforces unique index `(ExamId, CycleYear)` and `AdminService` prevents silently overwriting `Verified` historical cycles.
- `AuditLogs` records every create, update, approval, postponement, cancellation, PDF upload, and source verification event.
- Back up `backend/src/SarkariExamIntelligence.API/bin/.../wwwroot/notifications/` alongside the SQL Server database so uploaded official PDFs remain synced with `ExamNotifications.OfficialNotificationPdfUrl`.
