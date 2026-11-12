param(
    [string]$Server = "localhost",
    [string]$Database = "SarkariExamIntelligenceDb",
    [string]$BackupDirectory = "D:\Backups\SQL"
)

$ErrorActionPreference = "Stop"
New-Item -ItemType Directory -Force -Path $BackupDirectory | Out-Null
$Timestamp = Get-Date -Format "yyyyMMdd_HHmmss"
$BackupFile = Join-Path $BackupDirectory "${Database}_${Timestamp}.bak"

$Sql = @"
BACKUP DATABASE [$Database]
TO DISK = N'$BackupFile'
WITH COMPRESSION, CHECKSUM, STATS = 10, INIT;
RESTORE VERIFYONLY FROM DISK = N'$BackupFile' WITH CHECKSUM;
"@

sqlcmd -S $Server -E -C -Q $Sql
Write-Host "Verified backup created at: $BackupFile" -ForegroundColor Green
