using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Quartz;
using SarkariExamIntelligence.Application.Common.Exceptions;
using SarkariExamIntelligence.Application.Common.Interfaces;
using SarkariExamIntelligence.Application.DTOs.Admin;
using SarkariExamIntelligence.Domain.Entities;
using SarkariExamIntelligence.Domain.Enums;
using SarkariExamIntelligence.Infrastructure.Identity;

namespace SarkariExamIntelligence.Infrastructure.Services;

public class AdminUserService : IAdminUserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IApplicationDbContext _db;
    private readonly IAuditService _audit;

    public AdminUserService(UserManager<ApplicationUser> userManager, IApplicationDbContext db, IAuditService audit)
    {
        _userManager = userManager;
        _db = db;
        _audit = audit;
    }

    public async Task<IReadOnlyList<AdminUserSummaryDto>> GetUsersAsync(CancellationToken cancellationToken = default)
    {
        var users = await _userManager.Users.OrderByDescending(u => u.CreatedAtUtc).ToListAsync(cancellationToken);
        var profiles = (await _db.UserProfiles.AsNoTracking().ToListAsync(cancellationToken))
            .ToDictionary(p => p.UserId);

        var result = new List<AdminUserSummaryDto>();
        foreach (var u in users)
        {
            var roles = (await _userManager.GetRolesAsync(u)).ToList();
            profiles.TryGetValue(u.Id, out var prof);
            result.Add(new AdminUserSummaryDto
            {
                Id = u.Id,
                Email = u.Email ?? string.Empty,
                FullName = u.FullName,
                IsActive = u.IsActive,
                Roles = roles,
                DomicileState = prof?.DomicileState,
                GraduationQualification = prof?.GraduationQualification,
                CreatedAtUtc = u.CreatedAtUtc
            });
        }
        return result;
    }

    public async Task ToggleUserActiveAsync(Guid userId, bool isActive, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString())
            ?? throw new NotFoundException("User not found.");

        user.IsActive = isActive;
        await _userManager.UpdateAsync(user);
        await _audit.LogAsync("ApplicationUser", user.Id.ToString(), isActive ? "ActivateUser" : "DeactivateUser", null, new { isActive }, null, cancellationToken);
    }
}

public class SourceVerificationService : ISourceVerificationService
{
    private readonly IApplicationDbContext _db;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IAuditService _audit;

    public SourceVerificationService(IApplicationDbContext db, IHttpClientFactory httpClientFactory, IAuditService audit)
    {
        _db = db;
        _httpClientFactory = httpClientFactory;
        _audit = audit;
    }

    public async Task<AdminSourceVerificationResultDto> VerifySourceAsync(Guid officialSourceId, string? verifiedByUserId, CancellationToken cancellationToken = default)
    {
        var source = await _db.OfficialSources
            .Include(s => s.Exams)
                .ThenInclude(e => e.Notifications)
            .FirstOrDefaultAsync(s => s.Id == officialSourceId, cancellationToken)
            ?? throw new NotFoundException("Official source not found.");

        var now = DateTime.UtcNow;

        // Respectful rate-limit enforcement
        if (source.DateLastCheckedUtc.HasValue &&
            (now - source.DateLastCheckedUtc.Value).TotalSeconds < source.RateLimitSeconds)
        {
            return new AdminSourceVerificationResultDto
            {
                OfficialSourceId = source.Id,
                SourceName = source.SourceName,
                HttpStatusCode = source.LastHttpStatusCode,
                ChangesDetected = false,
                Outcome = VerificationOutcome.ManualVerificationRequired,
                Notes = $"Rate limit active ({source.RateLimitSeconds}s cooldown). Last checked at {source.DateLastCheckedUtc:u}.",
                CheckedAtUtc = source.DateLastCheckedUtc.Value
            };
        }

        int? statusCode = null;
        string? newHash = null;
        bool changesDetected = false;
        VerificationOutcome outcome;
        string notes;

        try
        {
            var client = _httpClientFactory.CreateClient("OfficialSourceVerifier");
            using var req = new HttpRequestMessage(HttpMethod.Get, source.CalendarOrFeedUrl ?? source.OfficialSourceUrl);
            req.Headers.UserAgent.ParseAdd("SarkariExamIntelligence-VerificationBot/1.0 (Respectful Public Official Feed Check)");

            using var resp = await client.SendAsync(req, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            statusCode = (int)resp.StatusCode;

            if (!resp.IsSuccessStatusCode)
            {
                outcome = statusCode is 403 or 429
                    ? VerificationOutcome.ManualVerificationRequired
                    : VerificationOutcome.SourceUnreachable;
                notes = $"HTTP {statusCode} returned. Respecting portal access control; manual verification advised.";
            }
            else
            {
                var body = await resp.Content.ReadAsStringAsync(cancellationToken);
                if (body.Contains("captcha", StringComparison.OrdinalIgnoreCase) || body.Contains("cf-turnstile", StringComparison.OrdinalIgnoreCase))
                {
                    outcome = VerificationOutcome.ManualVerificationRequired;
                    notes = "Portal uses anti-bot/CAPTCHA gate; automated scraping halted per compliance rules. Verify manually.";
                }
                else
                {
                    var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(body));
                    newHash = Convert.ToHexString(bytes);

                    changesDetected = !string.IsNullOrEmpty(source.LastContentHash) && !string.Equals(source.LastContentHash, newHash, StringComparison.OrdinalIgnoreCase);
                    outcome = changesDetected ? VerificationOutcome.ChangesDetected : VerificationOutcome.ConfirmedUnchanged;
                    notes = changesDetected
                        ? "Official page hash changed since last check — review pending updates for linked examinations."
                        : "Official portal reachable and content hash unchanged.";

                    if (changesDetected)
                    {
                        source.DateLastUpdatedUtc = now;
                        foreach (var exam in source.Exams)
                        {
                            foreach (var notif in exam.Notifications.Where(n => n.IsCurrentCycle))
                            {
                                notif.IsPendingApproval = true;
                                notif.VerificationStatus = VerificationStatus.NeedsReverification;
                                notif.LastChangedAtUtc = now;
                                notif.LastChangeSummary = "Source portal hash changed; requires admin verification.";
                            }
                        }
                    }

                    source.LastContentHash = newHash;
                    source.VerificationStatus = changesDetected ? VerificationStatus.NeedsReverification : VerificationStatus.VerifiedOfficial;
                }
            }
        }
        catch (Exception ex)
        {
            outcome = VerificationOutcome.SourceUnreachable;
            notes = $"Network check failed ({ex.Message}). Offline or restricted network environment — verify manually.";
        }

        source.DateLastCheckedUtc = now;
        source.LastHttpStatusCode = statusCode;

        _db.SourceVerificationLogs.Add(new SourceVerificationLog
        {
            OfficialSourceId = source.Id,
            CheckedAtUtc = now,
            CheckMethod = "RespectfulHttpCheck",
            HttpStatusCode = statusCode,
            ContentHash = newHash,
            ChangesDetected = changesDetected,
            Outcome = outcome,
            Notes = notes,
            VerifiedByUserId = verifiedByUserId
        });

        await _db.SaveChangesAsync(cancellationToken);
        await _audit.LogAsync("OfficialSource", source.Id.ToString(), "VerifySource", null, new { statusCode, changesDetected, outcome }, notes, cancellationToken);

        return new AdminSourceVerificationResultDto
        {
            OfficialSourceId = source.Id,
            SourceName = source.SourceName,
            HttpStatusCode = statusCode,
            ChangesDetected = changesDetected,
            Outcome = outcome,
            Notes = notes,
            CheckedAtUtc = now
        };
    }
}

[DisallowConcurrentExecution]
public class QuartzNotificationCheckJob : IJob
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<QuartzNotificationCheckJob> _logger;

    public QuartzNotificationCheckJob(
        IServiceScopeFactory scopeFactory,
        ILogger<QuartzNotificationCheckJob> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var orchestrator = scope.ServiceProvider.GetRequiredService<INotificationOrchestratorService>();
            var generated = await orchestrator.RunScheduledNotificationChecksAsync(context.CancellationToken);
            _logger.LogInformation("Quartz NotificationCheckJob finished. Generated {Count} deduplicated alerts.", generated);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Quartz NotificationCheckJob failed.");
        }
    }
}
