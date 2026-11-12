using Microsoft.EntityFrameworkCore;
using SarkariExamIntelligence.Application.Common.Exceptions;
using SarkariExamIntelligence.Application.Common.Interfaces;
using SarkariExamIntelligence.Application.DTOs.Eligibility;
using SarkariExamIntelligence.Domain.Entities;
using SarkariExamIntelligence.Domain.Enums;

namespace SarkariExamIntelligence.Application.Services;

public class EligibilityEngineService : IEligibilityEngine
{
    private readonly IApplicationDbContext _db;
    private readonly IDateTimeProvider _dateTime;

    private static readonly Dictionary<string, string> BranchAliases = new(StringComparer.OrdinalIgnoreCase)
    {
        ["it"] = "IT",
        ["information technology"] = "IT",
        ["b.tech it"] = "IT",
        ["cs"] = "CS",
        ["cse"] = "CS",
        ["computer science"] = "CS",
        ["computer science and engineering"] = "CS",
        ["computer science & engineering"] = "CS",
        ["computer engineering"] = "CS",
        ["computer science & it"] = "CS_IT",
        ["ece"] = "ECE",
        ["electronics and communication"] = "ECE",
        ["electronics & communication"] = "ECE",
        ["electronics and communication engineering"] = "ECE",
        ["electronics and telecommunication"] = "ECE",
        ["ee"] = "EE",
        ["eee"] = "EE",
        ["electrical"] = "EE",
        ["electrical engineering"] = "EE",
        ["electrical and electronics"] = "EE",
        ["me"] = "ME",
        ["mechanical"] = "ME",
        ["mechanical engineering"] = "ME",
        ["ce"] = "CE",
        ["civil"] = "CE",
        ["civil engineering"] = "CE",
        ["ai"] = "AI",
        ["artificial intelligence"] = "AI",
        ["data science"] = "DS",
        ["ai & ds"] = "AI",
        ["mca"] = "MCA",
        ["instrumentation"] = "EI",
        ["eie"] = "EI",
        ["chemical"] = "CH",
        ["chemical engineering"] = "CH"
    };

    public EligibilityEngineService(IApplicationDbContext db, IDateTimeProvider dateTime)
    {
        _db = db;
        _dateTime = dateTime;
    }

    public async Task<EligibilitySummaryDto> EvaluateAllForUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var profile = await _db.UserProfiles
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken)
            ?? throw new NotFoundException("Candidate profile not found.");

        return await EvaluateProfileAgainstAllExamsAsync(profile, cancellationToken);
    }

    public async Task<EligibilityCheckResultDto> EvaluateSingleForUserAsync(Guid examId, Guid userId, CancellationToken cancellationToken = default)
    {
        var profile = await _db.UserProfiles
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken)
            ?? throw new NotFoundException("Candidate profile not found.");

        var exam = await LoadExamsQuery()
            .FirstOrDefaultAsync(e => e.Id == examId, cancellationToken)
            ?? throw new NotFoundException("Exam not found.");

        var notification = exam.Notifications.FirstOrDefault(n => n.IsCurrentCycle)
            ?? exam.Notifications.OrderByDescending(n => n.CycleYear).FirstOrDefault();

        return EvaluateExam(exam, notification, profile, _dateTime.TodayIst);
    }

    public async Task<EligibilitySummaryDto> EvaluateAllAdHocAsync(AdHocEligibilityProfileRequest req, CancellationToken cancellationToken = default)
    {
        var profile = new UserProfile
        {
            UserId = Guid.Empty,
            FullName = "Candidate Preview",
            DateOfBirth = req.DateOfBirth,
            Gender = req.Gender,
            Category = req.Category,
            IsPwBD = req.IsPwBD,
            IsExServiceman = req.IsExServiceman,
            DomicileState = req.DomicileState,
            GraduationQualification = req.GraduationQualification,
            EngineeringBranch = req.EngineeringBranch,
            GraduationPercentage = req.GraduationPercentage,
            GraduationCgpa = req.GraduationCgpa,
            PassingYear = req.PassingYear,
            WorkExperienceMonths = req.WorkExperienceMonths,
            AdditionalQualifications = req.AdditionalQualifications,
            HasValidGateScore = req.HasValidGateScore,
            GateSubject = req.GateSubject,
            GateScore = req.GateScore,
            HasBiharDomicileCertificate = req.HasBiharDomicileCertificate,
            HasValidNclOrEwsCertificate = req.HasValidNclOrEwsCertificate,
            Nationality = req.Nationality
        };

        return await EvaluateProfileAgainstAllExamsAsync(profile, cancellationToken);
    }

    private async Task<EligibilitySummaryDto> EvaluateProfileAgainstAllExamsAsync(UserProfile profile, CancellationToken cancellationToken)
    {
        var exams = await LoadExamsQuery()
            .Where(e => e.IsApprovedForPublish)
            .OrderBy(e => e.Category.DisplayOrder)
            .ThenBy(e => e.Name)
            .ToListAsync(cancellationToken);

        var today = _dateTime.TodayIst;
        var results = exams.Select(exam =>
        {
            var notification = exam.Notifications.FirstOrDefault(n => n.IsCurrentCycle)
                ?? exam.Notifications.OrderByDescending(n => n.CycleYear).FirstOrDefault();
            return EvaluateExam(exam, notification, profile, today);
        }).ToList();

        return new EligibilitySummaryDto
        {
            TotalExamsEvaluated = results.Count,
            EligibleCount = results.Count(r => r.Status == EligibilityResultStatus.Eligible),
            PotentiallyEligibleCount = results.Count(r => r.Status == EligibilityResultStatus.PotentiallyEligibleVerificationRequired),
            NotEligibleCount = results.Count(r => r.Status == EligibilityResultStatus.NotEligible),
            Results = results
        };
    }

    private IQueryable<Exam> LoadExamsQuery() =>
        _db.Exams
            .AsNoTracking()
            .Include(e => e.Category)
            .Include(e => e.Organization)
            .Include(e => e.Notifications)
            .Include(e => e.EligibilityRules)
            .Include(e => e.AgeRelaxations);

    public EligibilityCheckResultDto EvaluateExam(Exam exam, ExamNotification? notification, UserProfile profile, DateOnly referenceToday)
    {
        var rule = notification?.EligibilityRules.FirstOrDefault()
            ?? exam.EligibilityRules.FirstOrDefault();

        var checks = new List<EligibilityCriterionCheckDto>();
        var reasons = new List<string>();
        var verificationActions = new List<string>();
        var hasHardFailure = false;
        var needsVerification = false;

        if (rule is null)
        {
            return new EligibilityCheckResultDto
            {
                ExamId = exam.Id,
                ExamName = exam.Name,
                ExamSlug = exam.Slug,
                CategoryName = exam.Category?.Name ?? string.Empty,
                OrganizationCode = exam.Organization?.ShortCode ?? string.Empty,
                CurrentStatus = notification?.Status ?? ExamStatus.UpcomingNotification,
                ApplicationEndDate = notification?.ApplicationEndDate,
                IsBiharSpecific = exam.IsBiharSpecific,
                IsItSpecific = exam.IsItSpecific,
                Status = EligibilityResultStatus.PotentiallyEligibleVerificationRequired,
                StatusLabel = "Potentially eligible – additional verification required",
                Summary = "Detailed eligibility rules for this cycle are awaiting official notification verification.",
                Reasons = new[] { "Eligibility criteria have not been verified against an active notification yet." },
                VerificationActionsRequired = new[] { "Review the official notification PDF once published." }
            };
        }

        // 1. Nationality check
        if (!string.IsNullOrWhiteSpace(profile.Nationality) &&
            !profile.Nationality.Contains("Indian", StringComparison.OrdinalIgnoreCase) &&
            rule.NationalityRequirement.Contains("Citizen of India", StringComparison.OrdinalIgnoreCase))
        {
            hasHardFailure = true;
            var msg = $"Nationality mismatch: requires {rule.NationalityRequirement}; profile states '{profile.Nationality}'.";
            checks.Add(new EligibilityCriterionCheckDto { Criterion = "Nationality", Passed = false, Detail = msg });
            reasons.Add(msg);
        }
        else
        {
            checks.Add(new EligibilityCriterionCheckDto { Criterion = "Nationality", Passed = true, Detail = $"Satisfies {rule.NationalityRequirement}." });
        }

        // 2. Domicile & Bihar reservation / certificate verification
        var isBiharCandidate = profile.DomicileState.Equals("Bihar", StringComparison.OrdinalIgnoreCase);
        var isReservedCategory = profile.Category != CategoryType.General;

        if (!string.IsNullOrWhiteSpace(rule.DomicileRequirement) &&
            !rule.DomicileRequirement.Contains("All India", StringComparison.OrdinalIgnoreCase) &&
            !rule.DomicileRequirement.Contains(profile.DomicileState, StringComparison.OrdinalIgnoreCase))
        {
            if (rule.DomicileRequirement.Contains("Required", StringComparison.OrdinalIgnoreCase) &&
                !rule.DomicileRequirement.Contains("Reservation", StringComparison.OrdinalIgnoreCase))
            {
                hasHardFailure = true;
                var msg = $"Domicile requirement not met: {rule.DomicileRequirement}; candidate domicile is {profile.DomicileState}.";
                checks.Add(new EligibilityCriterionCheckDto { Criterion = "Domicile", Passed = false, Detail = msg });
                reasons.Add(msg);
            }
        }

        var canClaimBiharStateReservation = !rule.BiharDomicileRequiredForReservation || isBiharCandidate;
        if (rule.BiharDomicileRequiredForReservation)
        {
            if (!isBiharCandidate && isReservedCategory)
            {
                needsVerification = true;
                var msg = "Bihar state reservation and upper age relaxation apply only to permanent Bihar domiciles; evaluated under Unreserved (General) category rules.";
                checks.Add(new EligibilityCriterionCheckDto { Criterion = "Bihar Reservation", Passed = true, RequiresVerification = true, Detail = msg });
                reasons.Add(msg);
                verificationActions.Add("Non-Bihar reserved candidates must apply under General/UR category with UR age and cutoff norms.");
            }
            else if (isBiharCandidate && isReservedCategory)
            {
                if (!profile.HasBiharDomicileCertificate || !profile.HasValidNclOrEwsCertificate)
                {
                    needsVerification = true;
                    var msg = $"Bihar domicile and valid {(profile.Category == CategoryType.EWS ? "EWS" : "NCL/Caste")} certificate verification required to claim Bihar state reservation & relaxation.";
                    checks.Add(new EligibilityCriterionCheckDto { Criterion = "Bihar Certificate Check", Passed = true, RequiresVerification = true, Detail = msg });
                    verificationActions.Add(rule.BiharCertificateConditions ?? "Keep valid Bihar Residential (Domicile) and Category/NCL/EWS certificate issued by competent Bihar authority ready.");
                }
                else
                {
                    checks.Add(new EligibilityCriterionCheckDto { Criterion = "Bihar Certificate Check", Passed = true, Detail = "Bihar domicile & category certificate status confirmed in profile." });
                }
            }
            else
            {
                checks.Add(new EligibilityCriterionCheckDto { Criterion = "Bihar Domicile", Passed = true, Detail = isBiharCandidate ? "Bihar domicile candidate." : "Open to All India candidates under Unreserved (UR) quota." });
            }
        }

        // 3. Gender condition (if exam restricts gender, e.g., certain posts)
        if (!string.IsNullOrWhiteSpace(rule.GenderRequirement) &&
            profile.Gender.HasValue &&
            !rule.GenderRequirement.Contains(profile.Gender.Value.ToString(), StringComparison.OrdinalIgnoreCase))
        {
            hasHardFailure = true;
            var msg = $"Gender restriction: notification specifies '{rule.GenderRequirement}'.";
            checks.Add(new EligibilityCriterionCheckDto { Criterion = "Gender", Passed = false, Detail = msg });
            reasons.Add(msg);
        }

        // 4. Qualification & Engineering Branch check (NEVER assume B.Tech is automatically eligible)
        EvaluateQualificationAndBranch(rule, profile, checks, reasons, verificationActions, ref hasHardFailure, ref needsVerification);

        // 5. Marks / Percentage / CGPA check
        EvaluateMarks(rule, profile, checks, reasons, verificationActions, ref hasHardFailure, ref needsVerification);

        // 6. Work Experience check
        if (rule.RequiresWorkExperience)
        {
            if (profile.WorkExperienceMonths < rule.MinWorkExperienceMonths)
            {
                hasHardFailure = true;
                var msg = $"Experience requirement not met: requires at least {rule.MinWorkExperienceMonths} months; profile has {profile.WorkExperienceMonths} months ({rule.ExperienceRequirementNotes}).";
                checks.Add(new EligibilityCriterionCheckDto { Criterion = "Experience", Passed = false, Detail = msg });
                reasons.Add(msg);
            }
            else
            {
                needsVerification = true;
                var msg = $"Experience duration ({profile.WorkExperienceMonths} months) meets minimum {rule.MinWorkExperienceMonths} months, but relevant domain/post-qualification nature must be verified ({rule.ExperienceRequirementNotes}).";
                checks.Add(new EligibilityCriterionCheckDto { Criterion = "Experience", Passed = true, RequiresVerification = true, Detail = msg });
                verificationActions.Add($"Verify experience certificate matches: {rule.ExperienceRequirementNotes}");
            }
        }
        else
        {
            checks.Add(new EligibilityCriterionCheckDto { Criterion = "Experience", Passed = true, Detail = "No mandatory prior work experience required." });
        }

        // 7. GATE Score check
        if (rule.RequiresValidGateScore)
        {
            if (!profile.HasValidGateScore)
            {
                hasHardFailure = true;
                var msg = $"Valid GATE score required in [{rule.AcceptedGatePapersCsv}]; profile shows no active GATE score.";
                checks.Add(new EligibilityCriterionCheckDto { Criterion = "GATE Score", Passed = false, Detail = msg });
                reasons.Add(msg);
            }
            else
            {
                var papers = SplitCsv(rule.AcceptedGatePapersCsv);
                var paperMatch = papers.Count == 0 || (!string.IsNullOrWhiteSpace(profile.GateSubject) &&
                    papers.Any(p => profile.GateSubject.Contains(p, StringComparison.OrdinalIgnoreCase)));

                if (!paperMatch)
                {
                    hasHardFailure = true;
                    var msg = $"GATE paper mismatch: requires {rule.AcceptedGatePapersCsv}; profile has '{profile.GateSubject ?? "Not specified"}'.";
                    checks.Add(new EligibilityCriterionCheckDto { Criterion = "GATE Score", Passed = false, Detail = msg });
                    reasons.Add(msg);
                }
                else if (rule.MinGateScore.HasValue && (!profile.GateScore.HasValue || profile.GateScore.Value < rule.MinGateScore.Value))
                {
                    hasHardFailure = true;
                    var msg = $"GATE score ({profile.GateScore?.ToString() ?? "Missing"}) is below minimum threshold {rule.MinGateScore}.";
                    checks.Add(new EligibilityCriterionCheckDto { Criterion = "GATE Score", Passed = false, Detail = msg });
                    reasons.Add(msg);
                }
                else
                {
                    needsVerification = true;
                    var msg = $"Valid GATE {profile.GateSubject} score ({profile.GateScore?.ToString() ?? "present"}) satisfies paper check; final PSU shortlisting cutoff depends on individual organization notification.";
                    checks.Add(new EligibilityCriterionCheckDto { Criterion = "GATE Score", Passed = true, RequiresVerification = true, Detail = msg });
                    verificationActions.Add("Check organization-specific GATE cutoff score and valid GATE year window.");
                }
            }
        }

        // 8. Age calculation on EXACT notification cutoff date + supported category relaxation
        var ageEval = EvaluateAge(exam, notification, rule, profile, referenceToday, canClaimBiharStateReservation, checks, reasons, verificationActions, ref hasHardFailure, ref needsVerification);

        // 9. Notification-specific additional verification notes
        if (!string.IsNullOrWhiteSpace(rule.VerificationNotes) && !hasHardFailure)
        {
            needsVerification = true;
            verificationActions.Add(rule.VerificationNotes);
        }

        // 10. Determine final status
        var finalStatus = hasHardFailure
            ? EligibilityResultStatus.NotEligible
            : needsVerification
                ? EligibilityResultStatus.PotentiallyEligibleVerificationRequired
                : EligibilityResultStatus.Eligible;

        var statusLabel = finalStatus switch
        {
            EligibilityResultStatus.Eligible => "Eligible",
            EligibilityResultStatus.PotentiallyEligibleVerificationRequired => "Potentially eligible – additional verification required",
            _ => "Not eligible"
        };

        var summary = finalStatus switch
        {
            EligibilityResultStatus.Eligible => $"All checked criteria (age on cutoff date, qualification, branch, marks, domicile) satisfy {exam.ShortName}.",
            EligibilityResultStatus.PotentiallyEligibleVerificationRequired => verificationActions.FirstOrDefault() ?? "Core criteria look aligned, but additional verification is required against the official notification.",
            _ => reasons.FirstOrDefault() ?? $"Does not satisfy mandatory eligibility criteria for {exam.ShortName}."
        };

        if (reasons.Count == 0)
            reasons.Add(summary);

        return new EligibilityCheckResultDto
        {
            ExamId = exam.Id,
            ExamName = exam.Name,
            ExamSlug = exam.Slug,
            CategoryName = exam.Category?.Name ?? string.Empty,
            OrganizationCode = exam.Organization?.ShortCode ?? string.Empty,
            CurrentStatus = notification?.Status ?? ExamStatus.UpcomingNotification,
            ApplicationEndDate = notification?.ApplicationEndDate,
            IsBiharSpecific = exam.IsBiharSpecific,
            IsItSpecific = exam.IsItSpecific,
            Status = finalStatus,
            StatusLabel = statusLabel,
            Summary = summary,
            AgeEvaluation = ageEval,
            CriteriaChecks = checks,
            Reasons = reasons.Distinct().ToList(),
            VerificationActionsRequired = verificationActions.Distinct().ToList()
        };
    }

    private void EvaluateQualificationAndBranch(
        EligibilityRule rule,
        UserProfile profile,
        List<EligibilityCriterionCheckDto> checks,
        List<string> reasons,
        List<string> verificationActions,
        ref bool hasHardFailure,
        ref bool needsVerification)
    {
        var qual = profile.GraduationQualification?.Trim() ?? string.Empty;
        var isEngineeringDegree = qual.Contains("Tech", StringComparison.OrdinalIgnoreCase)
            || qual.Contains("B.E", StringComparison.OrdinalIgnoreCase)
            || qual.Contains("Engineering", StringComparison.OrdinalIgnoreCase)
            || qual.Contains("M.E", StringComparison.OrdinalIgnoreCase);

        var isGraduateOrHigher = isEngineeringDegree
            || qual.Contains("B.Sc", StringComparison.OrdinalIgnoreCase)
            || qual.Contains("B.A", StringComparison.OrdinalIgnoreCase)
            || qual.Contains("B.Com", StringComparison.OrdinalIgnoreCase)
            || qual.Contains("BCA", StringComparison.OrdinalIgnoreCase)
            || qual.Contains("BBA", StringComparison.OrdinalIgnoreCase)
            || qual.Contains("Graduate", StringComparison.OrdinalIgnoreCase)
            || qual.Contains("MCA", StringComparison.OrdinalIgnoreCase)
            || qual.Contains("M.Sc", StringComparison.OrdinalIgnoreCase)
            || qual.Contains("MBA", StringComparison.OrdinalIgnoreCase)
            || qual.Contains("Master", StringComparison.OrdinalIgnoreCase);

        var isPostGraduate = qual.Contains("M.Tech", StringComparison.OrdinalIgnoreCase)
            || qual.Contains("M.E", StringComparison.OrdinalIgnoreCase)
            || qual.Contains("MCA", StringComparison.OrdinalIgnoreCase)
            || qual.Contains("M.Sc", StringComparison.OrdinalIgnoreCase)
            || qual.Contains("MBA", StringComparison.OrdinalIgnoreCase)
            || qual.Contains("Master", StringComparison.OrdinalIgnoreCase)
            || (!string.IsNullOrWhiteSpace(profile.AdditionalQualifications) &&
                (profile.AdditionalQualifications.Contains("M.Tech", StringComparison.OrdinalIgnoreCase)
                 || profile.AdditionalQualifications.Contains("MCA", StringComparison.OrdinalIgnoreCase)
                 || profile.AdditionalQualifications.Contains("M.Sc", StringComparison.OrdinalIgnoreCase)
                 || profile.AdditionalQualifications.Contains("PG", StringComparison.OrdinalIgnoreCase)));

        // Check degree level requirement
        if (rule.MinimumQualificationLevel.Equals("PostGraduate", StringComparison.OrdinalIgnoreCase))
        {
            if (!isPostGraduate && !rule.RequiresEngineeringOrTechDegree)
            {
                hasHardFailure = true;
                var msg = $"Qualification mismatch: requires {rule.QualificationSummary}; profile qualification is '{qual}'.";
                checks.Add(new EligibilityCriterionCheckDto { Criterion = "Qualification", Passed = false, Detail = msg });
                reasons.Add(msg);
                return;
            }
        }
        else if (rule.MinimumQualificationLevel.Equals("Diploma", StringComparison.OrdinalIgnoreCase) && !isGraduateOrHigher && !qual.Contains("Diploma", StringComparison.OrdinalIgnoreCase))
        {
            hasHardFailure = true;
            var msg = $"Qualification mismatch: requires Diploma/Degree as specified in '{rule.QualificationSummary}'.";
            checks.Add(new EligibilityCriterionCheckDto { Criterion = "Qualification", Passed = false, Detail = msg });
            reasons.Add(msg);
            return;
        }
        else if ((rule.MinimumQualificationLevel.Equals("Graduate", StringComparison.OrdinalIgnoreCase) || rule.RequiresEngineeringOrTechDegree) && !isGraduateOrHigher)
        {
            hasHardFailure = true;
            var msg = $"Qualification mismatch: requires graduation ({rule.QualificationSummary}); profile states '{qual}'.";
            checks.Add(new EligibilityCriterionCheckDto { Criterion = "Qualification", Passed = false, Detail = msg });
            reasons.Add(msg);
            return;
        }

        // Check Engineering-specific degree & branch rules
        if (rule.RequiresEngineeringOrTechDegree && !rule.AcceptsAnyGraduateDegree)
        {
            var allowsMcaOrMscIt = !string.IsNullOrWhiteSpace(rule.QualificationSummary) &&
                (rule.QualificationSummary.Contains("MCA", StringComparison.OrdinalIgnoreCase) ||
                 rule.QualificationSummary.Contains("M.Sc", StringComparison.OrdinalIgnoreCase) ||
                 rule.QualificationSummary.Contains("DOEACC", StringComparison.OrdinalIgnoreCase));

            var qualifiesByTechDegree = isEngineeringDegree || (allowsMcaOrMscIt &&
                (qual.Contains("MCA", StringComparison.OrdinalIgnoreCase) ||
                 (!string.IsNullOrWhiteSpace(profile.AdditionalQualifications) && profile.AdditionalQualifications.Contains("MCA", StringComparison.OrdinalIgnoreCase))));

            if (!qualifiesByTechDegree)
            {
                hasHardFailure = true;
                var msg = $"Technical degree required: {rule.QualificationSummary}. Candidate degree '{qual}' does not meet the engineering/technical degree requirement.";
                checks.Add(new EligibilityCriterionCheckDto { Criterion = "Qualification", Passed = false, Detail = msg });
                reasons.Add(msg);
                return;
            }
        }

        // Exact branch validation when AcceptedEngineeringBranchesCsv is defined
        var acceptedBranches = SplitCsv(rule.AcceptedEngineeringBranchesCsv);
        if (acceptedBranches.Count > 0 && !acceptedBranches.Any(b => b.Equals("ANY", StringComparison.OrdinalIgnoreCase)))
        {
            var candidateBranch = profile.EngineeringBranch?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(candidateBranch))
            {
                if (rule.RequiresEngineeringOrTechDegree)
                {
                    needsVerification = true;
                    var msg = $"Accepted branches are [{rule.AcceptedEngineeringBranchesCsv}], but candidate engineering branch is not filled in profile.";
                    checks.Add(new EligibilityCriterionCheckDto { Criterion = "Engineering Branch", Passed = true, RequiresVerification = true, Detail = msg });
                    verificationActions.Add("Update your exact engineering branch in profile settings.");
                }
            }
            else
            {
                var normalizedCandidate = NormalizeBranch(candidateBranch);
                var normalizedAccepted = acceptedBranches.Select(NormalizeBranch).ToList();

                var branchMatched = normalizedAccepted.Any(a =>
                    a.Equals(normalizedCandidate, StringComparison.OrdinalIgnoreCase) ||
                    (a == "CS_IT" && (normalizedCandidate == "CS" || normalizedCandidate == "IT")));

                if (!branchMatched)
                {
                    hasHardFailure = true;
                    var msg = $"Engineering branch '{candidateBranch}' is not accepted. Official notification accepts: {rule.AcceptedEngineeringBranchesCsv}.";
                    checks.Add(new EligibilityCriterionCheckDto { Criterion = "Engineering Branch", Passed = false, Detail = msg });
                    reasons.Add(msg);
                    return;
                }

                checks.Add(new EligibilityCriterionCheckDto
                {
                    Criterion = "Engineering Branch",
                    Passed = true,
                    Detail = $"Branch '{candidateBranch}' matches accepted stream ({rule.AcceptedEngineeringBranchesCsv})."
                });
            }
        }
        else if (rule.AcceptsAnyGraduateDegree && isEngineeringDegree)
        {
            checks.Add(new EligibilityCriterionCheckDto
            {
                Criterion = "Qualification",
                Passed = true,
                Detail = $"{qual} ({profile.EngineeringBranch ?? "Any branch"}) satisfies graduation requirement ({rule.QualificationSummary})."
            });
        }
        else
        {
            checks.Add(new EligibilityCriterionCheckDto
            {
                Criterion = "Qualification",
                Passed = true,
                Detail = $"Meets baseline qualification ({rule.QualificationSummary})."
            });
        }
    }

    private void EvaluateMarks(
        EligibilityRule rule,
        UserProfile profile,
        List<EligibilityCriterionCheckDto> checks,
        List<string> reasons,
        List<string> verificationActions,
        ref bool hasHardFailure,
        ref bool needsVerification)
    {
        decimal? requiredMin = profile.Category is CategoryType.SC or CategoryType.ST || profile.IsPwBD
            ? rule.MinPercentageReserved ?? rule.MinPercentageGeneral
            : rule.MinPercentageGeneral;

        if (!requiredMin.HasValue)
        {
            checks.Add(new EligibilityCriterionCheckDto { Criterion = "Marks / Percentage", Passed = true, Detail = "No minimum graduation percentage cutoff specified." });
            return;
        }

        if (!profile.GraduationPercentage.HasValue && !profile.GraduationCgpa.HasValue)
        {
            needsVerification = true;
            var msg = $"Minimum {requiredMin:0.##}% aggregate marks required; please update Graduation Percentage/CGPA in your profile.";
            checks.Add(new EligibilityCriterionCheckDto { Criterion = "Marks / Percentage", Passed = true, RequiresVerification = true, Detail = msg });
            verificationActions.Add("Enter exact Graduation Percentage or CGPA to confirm marks eligibility.");
            return;
        }

        var usedCgpaConversion = !profile.GraduationPercentage.HasValue && profile.GraduationCgpa.HasValue;
        var effectivePct = profile.GraduationPercentage ?? Math.Round(profile.GraduationCgpa!.Value * 9.5m, 2);

        if (effectivePct < requiredMin.Value)
        {
            hasHardFailure = true;
            var msg = $"Minimum marks cutoff not met: requires {requiredMin:0.##}% ({profile.Category} norm); profile reflects {effectivePct:0.##}%.";
            checks.Add(new EligibilityCriterionCheckDto { Criterion = "Marks / Percentage", Passed = false, Detail = msg });
            reasons.Add(msg);
            return;
        }

        if (usedCgpaConversion && effectivePct < requiredMin.Value + 3m)
        {
            needsVerification = true;
            var msg = $"Converted CGPA ({profile.GraduationCgpa:0.##} ≈ {effectivePct:0.##}%) is close to {requiredMin:0.##}% threshold; university conversion certificate required.";
            checks.Add(new EligibilityCriterionCheckDto { Criterion = "Marks / Percentage", Passed = true, RequiresVerification = true, Detail = msg });
            verificationActions.Add("Verify university's official CGPA-to-percentage conversion formula meets the required percentage.");
        }
        else
        {
            checks.Add(new EligibilityCriterionCheckDto
            {
                Criterion = "Marks / Percentage",
                Passed = true,
                Detail = $"Marks ({effectivePct:0.##}%) meet minimum {requiredMin:0.##}% requirement."
            });
        }
    }

    private AgeEvaluationDto EvaluateAge(
        Exam exam,
        ExamNotification? notification,
        EligibilityRule rule,
        UserProfile profile,
        DateOnly referenceToday,
        bool canClaimBiharStateReservation,
        List<EligibilityCriterionCheckDto> checks,
        List<string> reasons,
        List<string> verificationActions,
        ref bool hasHardFailure,
        ref bool needsVerification)
    {
        var eval = new AgeEvaluationDto
        {
            CandidateDob = profile.DateOfBirth,
            BaseMinAge = rule.MinAgeYears,
            BaseMaxAge = rule.MaxAgeYears,
            CutoffDateBasis = rule.AgeCutoffBasisDescription
        };

        var cutoffDate = rule.AgeCutoffDate ?? notification?.AgeCutoffDate;
        if (!cutoffDate.HasValue)
        {
            eval.IsCutoffDateTentative = true;
            var cycleYear = notification?.CycleYear ?? referenceToday.Year;
            cutoffDate = new DateOnly(cycleYear, 8, 1);
            eval.CutoffDateBasis = $"Age cutoff date not officially announced yet; tentative evaluation against {cutoffDate:yyyy-MM-dd} ({rule.AgeCutoffBasisDescription}).";
            needsVerification = true;
            verificationActions.Add("Verify exact age cutoff date in the upcoming official notification PDF.");
        }
        eval.CutoffDateUsed = cutoffDate;

        // Calculate applicable relaxation ONLY when supported by notification & domicile rules
        var relaxationYears = 0;
        var relaxationReasons = new List<string>();

        if (rule.MaxAgeYears.HasValue)
        {
            var effectiveCategoryForRelaxation = canClaimBiharStateReservation ? profile.Category : CategoryType.General;

            var catRule = exam.AgeRelaxations.FirstOrDefault(r =>
                r.IsSupportedByNotification &&
                r.Category == effectiveCategoryForRelaxation &&
                string.IsNullOrWhiteSpace(r.SubCategory));

            if (catRule is not null && (!catRule.RequiresDomicileForStateQuota || canClaimBiharStateReservation))
            {
                relaxationYears = Math.Max(relaxationYears, catRule.RelaxationYears);
                relaxationReasons.Add($"{effectiveCategoryForRelaxation}: +{catRule.RelaxationYears} yrs ({catRule.Conditions})");
            }

            if (profile.IsPwBD)
            {
                var pwbdRule = exam.AgeRelaxations.FirstOrDefault(r =>
                    r.IsSupportedByNotification && r.Category == CategoryType.PwBD);
                if (pwbdRule is not null)
                {
                    var pwbdExtra = pwbdRule.RelaxationYears;
                    if (pwbdExtra > relaxationYears)
                    {
                        relaxationYears = pwbdExtra;
                        relaxationReasons.Add($"PwBD: +{pwbdExtra} yrs");
                    }
                }
            }

            if (profile.IsExServiceman)
            {
                var exRule = exam.AgeRelaxations.FirstOrDefault(r =>
                    r.IsSupportedByNotification && r.Category == CategoryType.ExServicemen);
                if (exRule is not null && exRule.RelaxationYears > relaxationYears)
                {
                    relaxationYears = exRule.RelaxationYears;
                    relaxationReasons.Add($"Ex-Serviceman: +{exRule.RelaxationYears} yrs");
                }
            }

            // Bihar special female / BC / EBC handling if female candidate from Bihar
            if (rule.BiharDomicileRequiredForReservation && canClaimBiharStateReservation &&
                profile.Gender == GenderType.Female && profile.Category == CategoryType.General)
            {
                var biharFemaleRule = exam.AgeRelaxations.FirstOrDefault(r =>
                    r.IsSupportedByNotification && r.Category == CategoryType.FemaleReservation);
                if (biharFemaleRule is not null && biharFemaleRule.RelaxationYears > relaxationYears)
                {
                    relaxationYears = biharFemaleRule.RelaxationYears;
                    relaxationReasons.Add($"Bihar UR Female: +{biharFemaleRule.RelaxationYears} yrs");
                }
            }
        }

        eval.AppliedRelaxationYears = relaxationYears;
        eval.EffectiveMaxAge = rule.MaxAgeYears.HasValue ? rule.MaxAgeYears.Value + relaxationYears : null;
        eval.RelaxationExplanation = relaxationReasons.Count > 0
            ? string.Join("; ", relaxationReasons)
            : "No category age relaxation applied (standard unreserved limits or no upper age cap).";

        if (!profile.DateOfBirth.HasValue)
        {
            needsVerification = true;
            eval.PassesAgeRule = true;
            eval.CalculatedAgeText = "Date of Birth missing";
            checks.Add(new EligibilityCriterionCheckDto
            {
                Criterion = "Age Limit",
                Passed = true,
                RequiresVerification = true,
                Detail = "Date of birth is not set in profile; add DOB to calculate exact age on notification cutoff date."
            });
            verificationActions.Add("Add your Date of Birth in Profile to verify exact age against notification cutoff date.");
            return eval;
        }

        var (years, months, days) = CalculateExactAge(profile.DateOfBirth.Value, cutoffDate.Value);
        eval.CalculatedAgeText = $"{years} yrs {months} mos {days} days (as on {cutoffDate:dd MMM yyyy})";
        eval.CalculatedAgeYearsDecimal = Math.Round(years + months / 12m + days / 365.25m, 2);

        if (years < rule.MinAgeYears)
        {
            hasHardFailure = true;
            eval.PassesAgeRule = false;
            var msg = $"Under minimum age limit: age on {cutoffDate:dd MMM yyyy} is {eval.CalculatedAgeText}, minimum required is {rule.MinAgeYears} years.";
            checks.Add(new EligibilityCriterionCheckDto { Criterion = "Age Limit", Passed = false, Detail = msg });
            reasons.Add(msg);
            return eval;
        }

        if (eval.EffectiveMaxAge.HasValue && (years > eval.EffectiveMaxAge.Value || (years == eval.EffectiveMaxAge.Value && (months > 0 || days > 0))))
        {
            hasHardFailure = true;
            eval.PassesAgeRule = false;
            var msg = $"Exceeds upper age limit: age on {cutoffDate:dd MMM yyyy} is {eval.CalculatedAgeText}; effective maximum age is {eval.EffectiveMaxAge} years (Base {rule.MaxAgeYears} + {relaxationYears} relaxation).";
            checks.Add(new EligibilityCriterionCheckDto { Criterion = "Age Limit", Passed = false, Detail = msg });
            reasons.Add(msg);
            return eval;
        }

        eval.PassesAgeRule = true;
        checks.Add(new EligibilityCriterionCheckDto
        {
            Criterion = "Age Limit",
            Passed = true,
            RequiresVerification = eval.IsCutoffDateTentative,
            Detail = eval.EffectiveMaxAge.HasValue
                ? $"Age {eval.CalculatedAgeText} is within allowed {rule.MinAgeYears}–{eval.EffectiveMaxAge} yrs (Base {rule.MaxAgeYears} + {relaxationYears} yrs relaxation)."
                : $"Age {eval.CalculatedAgeText} meets minimum {rule.MinAgeYears} yrs (no upper age limit)."
        });

        return eval;
    }

    public static (int Years, int Months, int Days) CalculateExactAge(DateOnly dob, DateOnly asOfDate)
    {
        if (asOfDate < dob)
            return (0, 0, 0);

        var years = asOfDate.Year - dob.Year;
        var months = asOfDate.Month - dob.Month;
        var days = asOfDate.Day - dob.Day;

        if (days < 0)
        {
            var prevMonth = asOfDate.AddMonths(-1);
            days += DateTime.DaysInMonth(prevMonth.Year, prevMonth.Month);
            months--;
        }

        if (months < 0)
        {
            months += 12;
            years--;
        }

        return (years, months, days);
    }

    private static string NormalizeBranch(string raw)
    {
        var cleaned = raw.Trim().ToLowerInvariant();
        if (BranchAliases.TryGetValue(cleaned, out var mapped))
            return mapped;

        if (cleaned.Contains("information tech")) return "IT";
        if (cleaned.Contains("computer")) return "CS";
        if (cleaned.Contains("electronics") && cleaned.Contains("comm")) return "ECE";
        if (cleaned.Contains("electrical")) return "EE";
        if (cleaned.Contains("mechanical")) return "ME";
        if (cleaned.Contains("civil")) return "CE";
        return cleaned.ToUpperInvariant();
    }

    private static List<string> SplitCsv(string? csv) =>
        string.IsNullOrWhiteSpace(csv)
            ? new List<string>()
            : csv.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();
}
