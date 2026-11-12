using SarkariExamIntelligence.Domain.Enums;

namespace SarkariExamIntelligence.Infrastructure.Persistence.Seed;

public static class ExamCatalogPart1
{
    public static List<ExamSeedSpec> GetSpecs() => new()
    {
        // ==================== A. UPSC (8) ====================
        new ExamSeedSpec
        {
            Code = "UPSC-CSE", Slug = "upsc-civil-services", Name = "UPSC Civil Services Examination (IAS/IPS/IFS)", ShortName = "UPSC CSE",
            CategorySlug = "upsc", OrgCode = "UPSC", Department = "DoPT / All India & Central Civil Services", Designation = "IAS / IPS / IFS / IRS Group A Officer",
            Group = GroupClassification.GroupA, OfficialWebsite = "https://upsc.gov.in",
            CycleYear = 2027, Status = ExamStatus.UpcomingNotification,
            ExpectedNotificationDate = new DateOnly(2027, 1, 20), ExpectedAppStart = new DateOnly(2027, 1, 20), ExpectedAppEnd = new DateOnly(2027, 2, 11),
            ExpectedPrelimDate = new DateOnly(2027, 5, 23), AgeCutoffDate = new DateOnly(2027, 8, 1), AgeCutoffBasis = "1st August of examination year (as per UPSC CSE Rules)",
            Confidence = DataConfidence.OfficialCalendarTentative, VerificationStatus = VerificationStatus.TentativeCalendar,
            VacancyType = VacancyType.NotAnnounced, VacancyNotes = "2027 vacancies awaiting official UPSC notification; 2026 notified 933 vacancies.",
            MinAge = 21, MaxAge = 32, MinQualLevel = "Graduate", AcceptsAnyGraduate = true, FinalYearEligible = true,
            QualificationSummary = "Degree from a recognized Indian university (any stream including B.Tech IT)",
            FeeSummary = "₹100 (General/OBC/EWS Male); Exempted for SC/ST/PwBD/Female",
            SalaryPayLevel = "Level 10 (₹56,100–₹1,77,500) + Central DA & Allowances",
            SelectionProcedure = "Prelims (GS + CSAT) -> Mains (9 Descriptive Papers) -> Personality Test (Interview)",
            ExamMode = ExamMode.PenAndPaperOMR, NegativeMarking = "1/3rd negative marking in Prelims Objective papers",
            SyllabusSubjectCodes = new[] { "HISTORY", "GEOGRAPHY", "POLITY", "ECONOMY", "GEN_AWARENESS", "CURRENT_AFFAIRS", "QUANT", "REASONING", "ENGLISH" },
            Patterns = new() { ("Prelims", "General Studies Paper I", 100, 200, 120, "1/3rd", false), ("Prelims", "CSAT Paper II (33% Qualifying)", 80, 200, 120, "1/3rd", true) },
            History = new()
            {
                (2022, HistoricalDataStatus.Verified, 1022, null, new DateOnly(2022, 2, 2), new DateOnly(2022, 6, 5), "Official UPSC CSE 2022 final vacancy figure"),
                (2023, HistoricalDataStatus.Verified, 1105, null, new DateOnly(2023, 2, 1), new DateOnly(2023, 5, 28), "Official UPSC CSE 2023 notification"),
                (2024, HistoricalDataStatus.Verified, 1056, null, new DateOnly(2024, 2, 14), new DateOnly(2024, 6, 16), "Official UPSC CSE 2024 notification"),
                (2025, HistoricalDataStatus.Verified, 979, null, new DateOnly(2025, 1, 22), new DateOnly(2025, 5, 25), "Official UPSC CSE 2025 notification"),
                (2026, HistoricalDataStatus.Verified, 933, null, new DateOnly(2026, 1, 21), new DateOnly(2026, 5, 24), "Official UPSC CSE 2026 notification"),
                (2027, HistoricalDataStatus.AwaitingOfficialNotification, null, null, null, null, "Awaiting official 2027 notification")
            },
            Cutoffs = new() { (2024, "Prelims GS", CategoryType.General, 87.98m, 200m), (2024, "Prelims GS", CategoryType.OBCNCL, 87.28m, 200m), (2024, "Prelims GS", CategoryType.EWS, 85.92m, 200m) }
        },
        new ExamSeedSpec
        {
            Code = "UPSC-IFOS", Slug = "upsc-indian-forest-service", Name = "UPSC Indian Forest Service (IFoS)", ShortName = "UPSC IFoS",
            CategorySlug = "upsc", OrgCode = "UPSC", Department = "Ministry of Environment, Forest and Climate Change", Designation = "Indian Forest Service Officer (Group A)",
            Group = GroupClassification.GroupA, OfficialWebsite = "https://upsc.gov.in",
            CycleYear = 2027, Status = ExamStatus.UpcomingNotification, ExpectedNotificationDate = new DateOnly(2027, 1, 20), ExpectedPrelimDate = new DateOnly(2027, 5, 23),
            AgeCutoffDate = new DateOnly(2027, 8, 1), AgeCutoffBasis = "1st August of exam year (common Prelims with CSE)",
            Confidence = DataConfidence.OfficialCalendarTentative, VerificationStatus = VerificationStatus.TentativeCalendar, VacancyType = VacancyType.NotAnnounced,
            MinAge = 21, MaxAge = 32, MinQualLevel = "Graduate", AcceptsAnyGraduate = false, RequiresEngineering = false,
            QualificationSummary = "Bachelor's degree with Animal Husbandry/Botany/Chemistry/Geology/Maths/Physics/Stats/Zoology OR Agriculture/Forestry/Engineering",
            VerificationNotes = "Verify that your specific B.Tech degree satisfies UPSC IFoS Engineering Bachelor's degree clause & chosen Mains optional subjects.",
            SyllabusSubjectCodes = new[] { "GEOGRAPHY", "GEN_AWARENESS", "CURRENT_AFFAIRS", "ENGLISH", "POLITY", "ECONOMY" },
            History = new()
            {
                (2022, HistoricalDataStatus.Verified, 151, null, new DateOnly(2022, 2, 2), new DateOnly(2022, 6, 5), "Official IFoS 2022"),
                (2023, HistoricalDataStatus.Verified, 150, null, new DateOnly(2023, 2, 1), new DateOnly(2023, 5, 28), "Official IFoS 2023"),
                (2024, HistoricalDataStatus.Verified, 150, null, new DateOnly(2024, 2, 14), new DateOnly(2024, 6, 16), "Official IFoS 2024"),
                (2025, HistoricalDataStatus.Verified, 150, null, new DateOnly(2025, 1, 22), new DateOnly(2025, 5, 25), "Official IFoS 2025"),
                (2026, HistoricalDataStatus.Verified, 150, null, null, null, "Official IFoS 2026"),
                (2027, HistoricalDataStatus.AwaitingOfficialNotification, null, null, null, null, "Awaiting official notification")
            }
        },
        new ExamSeedSpec
        {
            Code = "UPSC-CAPF", Slug = "upsc-capf-assistant-commandant", Name = "UPSC CAPF Assistant Commandant (AC)", ShortName = "UPSC CAPF AC",
            CategorySlug = "upsc", OrgCode = "UPSC", Department = "Ministry of Home Affairs (BSF, CRPF, CISF, ITBP, SSB)", Designation = "Assistant Commandant (Group A Gazetted)",
            Group = GroupClassification.GroupA, OfficialWebsite = "https://upsc.gov.in",
            CycleYear = 2027, Status = ExamStatus.UpcomingNotification, ExpectedNotificationDate = new DateOnly(2027, 4, 21), ExpectedPrelimDate = new DateOnly(2027, 8, 8),
            AgeCutoffDate = new DateOnly(2027, 8, 1), Confidence = DataConfidence.OfficialCalendarTentative, VerificationStatus = VerificationStatus.TentativeCalendar,
            VacancyType = VacancyType.NotAnnounced, MinAge = 20, MaxAge = 25, AcceptsAnyGraduate = true,
            QualificationSummary = "Bachelor's degree from a recognized university + Physical/Medical standards",
            VerificationNotes = "Requires clearing Physical Efficiency Test (PET) and Medical Standards.",
            SalaryPayLevel = "Level 10 (₹56,100–₹1,77,500)", SelectionProcedure = "Written (Paper I & II) -> PET/PST & Medical -> Interview",
            SyllabusSubjectCodes = new[] { "HISTORY", "GEOGRAPHY", "POLITY", "ECONOMY", "CURRENT_AFFAIRS", "QUANT", "REASONING", "ENGLISH" },
            History = new()
            {
                (2022, HistoricalDataStatus.Verified, 253, null, null, null, "Official CAPF 2022"),
                (2023, HistoricalDataStatus.Verified, 322, null, null, null, "Official CAPF 2023"),
                (2024, HistoricalDataStatus.Verified, 506, null, null, null, "Official CAPF 2024"),
                (2025, HistoricalDataStatus.Verified, 357, null, null, null, "Official CAPF 2025"),
                (2026, HistoricalDataStatus.AwaitingOfficialNotification, null, null, null, null, "Verify 2026 final count")
            }
        },
        new ExamSeedSpec
        {
            Code = "UPSC-CDS", Slug = "upsc-cds", Name = "UPSC Combined Defence Services (CDS)", ShortName = "UPSC CDS",
            CategorySlug = "upsc", OrgCode = "UPSC", Department = "Ministry of Defence (IMA, INA, AFA, OTA)", Designation = "Commissioned Officer",
            Group = GroupClassification.GroupA, OfficialWebsite = "https://upsc.gov.in",
            CycleYear = 2027, Status = ExamStatus.UpcomingNotification, ExpectedNotificationDate = new DateOnly(2026, 12, 16), ExpectedPrelimDate = new DateOnly(2027, 4, 11),
            AgeCutoffDate = new DateOnly(2028, 1, 1), AgeCutoffBasis = "Age as on 1st Jan/July of course commencement",
            Confidence = DataConfidence.OfficialCalendarTentative, VerificationStatus = VerificationStatus.TentativeCalendar, VacancyType = VacancyType.NotAnnounced,
            MinAge = 19, MaxAge = 25, SupportRelaxations = false, AcceptsAnyGraduate = true,
            QualificationSummary = "Graduate (IMA/OTA); Engineering degree for INA; Physics & Maths in 10+2 or B.E./B.Tech for AFA",
            VerificationNotes = "Unmarried criteria and strict upper age cap (24 for IMA/AFA/INA, 25 for OTA) with NO caste age relaxation.",
            SyllabusSubjectCodes = new[] { "QUANT", "ENGLISH", "GEN_AWARENESS", "HISTORY", "GEOGRAPHY", "POLITY", "CURRENT_AFFAIRS" }
        },
        new ExamSeedSpec
        {
            Code = "UPSC-NDA", Slug = "upsc-nda", Name = "UPSC National Defence Academy & Naval Academy (NDA & NA)", ShortName = "UPSC NDA",
            CategorySlug = "upsc", OrgCode = "UPSC", Department = "Ministry of Defence", Designation = "Cadet (Army / Navy / Air Force)",
            Group = GroupClassification.GroupA, IsGraduate = false, OfficialWebsite = "https://upsc.gov.in",
            CycleYear = 2027, Status = ExamStatus.UpcomingNotification, ExpectedNotificationDate = new DateOnly(2026, 12, 16), ExpectedPrelimDate = new DateOnly(2027, 4, 11),
            AgeCutoffDate = new DateOnly(2027, 7, 1), AgeCutoffBasis = "Born within NDA notification birth-date window (16.5–19.5 years)",
            Confidence = DataConfidence.OfficialCalendarTentative, VerificationStatus = VerificationStatus.TentativeCalendar, VacancyType = VacancyType.NotAnnounced,
            MinAge = 16, MaxAge = 19, MinQualLevel = "12th", SupportRelaxations = false, AcceptsAnyGraduate = false,
            QualificationSummary = "10+2 Pass (with Physics, Chemistry, Maths for Air Force/Navy); unmarried candidates aged 16.5–19.5 yrs",
            SyllabusSubjectCodes = new[] { "QUANT", "ENGLISH", "GEN_AWARENESS", "HISTORY", "GEOGRAPHY" }
        },
        new ExamSeedSpec
        {
            Code = "UPSC-EPFO-EOAO", Slug = "upsc-epfo-eo-ao", Name = "UPSC EPFO Enforcement Officer / Accounts Officer (EO/AO)", ShortName = "UPSC EPFO EO/AO",
            CategorySlug = "upsc", OrgCode = "UPSC", Department = "Employees' Provident Fund Organisation (Ministry of Labour)", Designation = "Enforcement Officer / Accounts Officer (Group B)",
            Group = GroupClassification.GroupBGazetted, OfficialWebsite = "https://upsc.gov.in",
            CycleYear = 2026, Status = ExamStatus.UpcomingNotification, Confidence = DataConfidence.NotAnnounced, VerificationStatus = VerificationStatus.TentativeCalendar, VacancyType = VacancyType.NotAnnounced,
            MinAge = 18, MaxAge = 30, AcceptsAnyGraduate = true,
            QualificationSummary = "Bachelor's degree in any subject from a recognized university",
            SalaryPayLevel = "Level 8 (₹47,600–₹1,51,100)", SelectionProcedure = "Recruitment Test (RT) + Interview (75:25 weightage)",
            SyllabusSubjectCodes = new[] { "ENGLISH", "HISTORY", "POLITY", "ECONOMY", "QUANT", "REASONING", "COMPUTER_AWARENESS", "CURRENT_AFFAIRS" },
            History = new()
            {
                (2023, HistoricalDataStatus.Verified, 418, null, new DateOnly(2023, 2, 25), new DateOnly(2023, 7, 2), "UPSC EPFO EO/AO 2023 cycle"),
                (2026, HistoricalDataStatus.AwaitingOfficialNotification, null, null, null, null, "Notified intermittently; awaiting next official cycle")
            }
        },
        new ExamSeedSpec
        {
            Code = "UPSC-EPFO-APFC", Slug = "upsc-epfo-apfc", Name = "UPSC EPFO Assistant Provident Fund Commissioner (APFC)", ShortName = "UPSC EPFO APFC",
            CategorySlug = "upsc", OrgCode = "UPSC", Department = "Employees' Provident Fund Organisation", Designation = "Assistant Provident Fund Commissioner (Group A)",
            Group = GroupClassification.GroupA, OfficialWebsite = "https://upsc.gov.in",
            CycleYear = 2026, Status = ExamStatus.UpcomingNotification, Confidence = DataConfidence.NotAnnounced, VerificationStatus = VerificationStatus.TentativeCalendar, VacancyType = VacancyType.NotAnnounced,
            MinAge = 18, MaxAge = 35, AcceptsAnyGraduate = true,
            QualificationSummary = "Degree from a recognized university (desirable: Diploma in Company Law/Labour Laws/Public Admin)",
            SalaryPayLevel = "Level 10 (₹56,100–₹1,77,500)", SelectionProcedure = "Recruitment Test + Interview",
            SyllabusSubjectCodes = new[] { "ENGLISH", "HISTORY", "POLITY", "ECONOMY", "QUANT", "REASONING", "COMPUTER_AWARENESS", "CURRENT_AFFAIRS" },
            History = new()
            {
                (2023, HistoricalDataStatus.Verified, 159, null, new DateOnly(2023, 2, 25), new DateOnly(2023, 7, 2), "UPSC EPFO APFC 2023 cycle"),
                (2026, HistoricalDataStatus.AwaitingOfficialNotification, null, null, null, null, "Awaiting official notification")
            }
        },
        new ExamSeedSpec
        {
            Code = "UPSC-ESE", Slug = "upsc-engineering-services", Name = "UPSC Engineering Services Examination (IES/ESE)", ShortName = "UPSC ESE",
            CategorySlug = "upsc", OrgCode = "UPSC", Department = "Central Engineering Services (Civil, Mechanical, Electrical, E&T)", Designation = "Assistant Executive Engineer (Group A)",
            Group = GroupClassification.GroupA, OfficialWebsite = "https://upsc.gov.in",
            CycleYear = 2027, Status = ExamStatus.ApplicationsClosed, Confidence = DataConfidence.OfficialCalendarTentative, VerificationStatus = VerificationStatus.TentativeCalendar,
            ExpectedPrelimDate = new DateOnly(2027, 2, 7), AgeCutoffDate = new DateOnly(2027, 1, 1), VacancyType = VacancyType.NotAnnounced,
            MinAge = 21, MaxAge = 30, MinQualLevel = "BTechBE", RequiresEngineering = true, AcceptsAnyGraduate = false,
            AcceptedBranches = "Civil Engineering,Mechanical Engineering,Electrical Engineering,Electronics & Communication",
            QualificationSummary = "Degree in Engineering (Civil, Mechanical, Electrical, or Electronics & Telecommunication)",
            SalaryPayLevel = "Level 10 (₹56,100–₹1,77,500)", SyllabusSubjectCodes = new[] { "CURRENT_AFFAIRS", "GEN_AWARENESS", "DIGITAL_LOGIC", "COA" }
        },

        // ==================== B. SSC (7) ====================
        new ExamSeedSpec
        {
            Code = "SSC-CGL", Slug = "ssc-cgl", Name = "SSC Combined Graduate Level Examination (CGL)", ShortName = "SSC CGL",
            CategorySlug = "ssc", OrgCode = "SSC", Department = "Central Ministries, CBDT, CBIC, CAG, CSS, ED, CBI", Designation = "ASO / Income Tax Inspector / GST Inspector / Auditor",
            Group = GroupClassification.GroupBNonGazetted, OfficialWebsite = "https://ssc.gov.in",
            CycleYear = 2026, NotificationNumber = "SSC CGL 2026", Status = ExamStatus.ExaminationScheduled,
            NotificationDate = new DateOnly(2026, 5, 21), AppStart = new DateOnly(2026, 5, 21), AppEnd = new DateOnly(2026, 6, 25),
            PrelimDate = new DateOnly(2026, 9, 30), ExpectedMainsDate = new DateOnly(2026, 12, 20), AgeCutoffDate = new DateOnly(2026, 8, 1),
            TotalVacancies = 10731, VacancyType = VacancyType.Official, VacancyNotes = "Tentatively ~10,731+ posts reported in SSC CGL 2026 notification.",
            MinAge = 18, MaxAge = 30, AcceptsAnyGraduate = true,
            QualificationSummary = "Bachelor's degree in any discipline (max age 27/30/32 varies by post)",
            SalaryPayLevel = "Level 4 to Level 8 (₹25,500–₹1,51,100)",
            SelectionProcedure = "Tier-I (Qualifying CBT) -> Tier-II (Merit CBT + Computer Knowledge + DEST)",
            NegativeMarking = "0.50 marks in Tier-I; 1 mark per wrong answer in Tier-II Section I/II",
            SyllabusSubjectCodes = new[] { "QUANT", "GEN_INTEL", "REASONING", "ENGLISH", "GEN_AWARENESS", "CURRENT_AFFAIRS", "COMPUTER_AWARENESS", "HISTORY", "POLITY", "ECONOMY", "GEOGRAPHY" },
            Patterns = new() { ("Tier-I", "Gen Intelligence, GK, Quant, English", 100, 200, 60, "0.50 per wrong", true), ("Tier-II", "Maths + Reasoning + English + GK + Computer", 150, 450, 150, "1.0 per wrong", false) },
            History = new()
            {
                (2022, HistoricalDataStatus.Verified, 36001, null, null, null, "Record SSC CGL 2022 final vacancies"),
                (2023, HistoricalDataStatus.Verified, 8415, null, null, null, "Official SSC CGL 2023 final vacancies"),
                (2024, HistoricalDataStatus.Verified, 17727, null, null, null, "Official SSC CGL 2024 final vacancies"),
                (2025, HistoricalDataStatus.Verified, 15130, null, null, null, "Official SSC CGL 2025 vacancies"),
                (2026, HistoricalDataStatus.Verified, 10731, null, new DateOnly(2026, 5, 21), new DateOnly(2026, 9, 30), "SSC CGL 2026 tentative vacancies")
            },
            Cutoffs = new() { (2024, "Tier-I (Other Posts)", CategoryType.General, 153.18m, 200m), (2024, "Tier-I (Other Posts)", CategoryType.OBCNCL, 146.26m, 200m), (2024, "Tier-I (Other Posts)", CategoryType.EWS, 142.02m, 200m) }
        },
        new ExamSeedSpec
        {
            Code = "SSC-CPO", Slug = "ssc-cpo", Name = "SSC Sub-Inspector in Delhi Police & CAPFs (CPO)", ShortName = "SSC CPO",
            CategorySlug = "ssc", OrgCode = "SSC", Department = "Delhi Police & CAPFs (MHA)", Designation = "Sub-Inspector (Executive / GD)",
            Group = GroupClassification.GroupBNonGazetted, OfficialWebsite = "https://ssc.gov.in",
            CycleYear = 2026, Status = ExamStatus.UpcomingNotification, Confidence = DataConfidence.HistoricalCycleExpected, VerificationStatus = VerificationStatus.TentativeCalendar, VacancyType = VacancyType.NotAnnounced,
            MinAge = 20, MaxAge = 25, AcceptsAnyGraduate = true, QualificationSummary = "Bachelor's degree from a recognized university + Physical Standards & PET",
            VerificationNotes = "Requires clearing PET/PST & Medical; Delhi Police SI requires valid LMV Driving License for males.",
            SyllabusSubjectCodes = new[] { "QUANT", "GEN_INTEL", "ENGLISH", "GEN_AWARENESS", "CURRENT_AFFAIRS" }
        },
        new ExamSeedSpec
        {
            Code = "SSC-CHSL", Slug = "ssc-chsl", Name = "SSC Combined Higher Secondary Level (CHSL)", ShortName = "SSC CHSL",
            CategorySlug = "ssc", OrgCode = "SSC", Department = "Central Ministries & Offices", Designation = "LDC / JSA / Data Entry Operator (DEO)",
            Group = GroupClassification.GroupC, IsGraduate = false, OfficialWebsite = "https://ssc.gov.in",
            CycleYear = 2026, Status = ExamStatus.ExaminationCompleted, Confidence = DataConfidence.OfficialCalendarTentative, VerificationStatus = VerificationStatus.TentativeCalendar, VacancyType = VacancyType.NotAnnounced,
            MinAge = 18, MaxAge = 27, MinQualLevel = "12th", AcceptsAnyGraduate = true,
            QualificationSummary = "12th Standard Pass (Graduates are eligible to appear; DEO in CAG requires Science+Maths in 12th)",
            SyllabusSubjectCodes = new[] { "QUANT", "GEN_INTEL", "ENGLISH", "GEN_AWARENESS", "COMPUTER_AWARENESS" }
        },
        new ExamSeedSpec
        {
            Code = "SSC-MTS", Slug = "ssc-mts", Name = "SSC Multi-Tasking Staff & Havaldar (MTS)", ShortName = "SSC MTS",
            CategorySlug = "ssc", OrgCode = "SSC", Department = "Central Ministries & CBIC/CBN", Designation = "MTS / Havaldar",
            Group = GroupClassification.GroupC, IsGraduate = false, OfficialWebsite = "https://ssc.gov.in",
            CycleYear = 2026, Status = ExamStatus.UpcomingNotification, Confidence = DataConfidence.HistoricalCycleExpected, VerificationStatus = VerificationStatus.TentativeCalendar, VacancyType = VacancyType.NotAnnounced,
            MinAge = 18, MaxAge = 25, MinQualLevel = "10th", AcceptsAnyGraduate = true,
            QualificationSummary = "10th / Matriculation pass (higher degree holders may apply within age limit 18–25/27 yrs)",
            SyllabusSubjectCodes = new[] { "QUANT", "REASONING", "ENGLISH", "GEN_AWARENESS" }
        },
        new ExamSeedSpec
        {
            Code = "SSC-GD", Slug = "ssc-gd-constable", Name = "SSC Constable (GD) in CAPFs, SSF & Rifleman", ShortName = "SSC GD",
            CategorySlug = "ssc", OrgCode = "SSC", Department = "Ministry of Home Affairs", Designation = "Constable (General Duty)",
            Group = GroupClassification.GroupC, IsGraduate = false, OfficialWebsite = "https://ssc.gov.in",
            CycleYear = 2027, Status = ExamStatus.UpcomingNotification, ExpectedNotificationDate = new DateOnly(2026, 12, 1),
            Confidence = DataConfidence.HistoricalCycleExpected, VerificationStatus = VerificationStatus.TentativeCalendar, VacancyType = VacancyType.NotAnnounced,
            MinAge = 18, MaxAge = 23, MinQualLevel = "10th", AcceptsAnyGraduate = true,
            QualificationSummary = "Matriculation (10th) pass + Physical & Medical standards (max age 23 yrs UR)",
            SyllabusSubjectCodes = new[] { "QUANT", "GEN_INTEL", "ENGLISH", "GEN_AWARENESS" }
        },
        new ExamSeedSpec
        {
            Code = "SSC-SEL-POST", Slug = "ssc-selection-post", Name = "SSC Selection Post (Phase Recruitment)", ShortName = "SSC Selection Post",
            CategorySlug = "ssc", OrgCode = "SSC", Department = "Various Central Govt Departments (IT/Technical/Graduate Posts)", Designation = "Junior Technical Assistant / Scientific Assistant / Data Processing Asst",
            Group = GroupClassification.GroupBNonGazetted, IsIt = true, OfficialWebsite = "https://ssc.gov.in",
            CycleYear = 2026, Status = ExamStatus.UpcomingNotification, Confidence = DataConfidence.NotAnnounced, VerificationStatus = VerificationStatus.TentativeCalendar, VacancyType = VacancyType.NotAnnounced,
            MinAge = 18, MaxAge = 30, AcceptsAnyGraduate = true,
            QualificationSummary = "Post-specific qualification (Matric / 12th / Graduate / B.Tech IT & Computer posts included)",
            VerificationNotes = "Each Post Code has distinct qualification & experience rules — verify specific Post Code in official Selection Post annexure.",
            SyllabusSubjectCodes = new[] { "QUANT", "GEN_INTEL", "ENGLISH", "GEN_AWARENESS", "COMPUTER_AWARENESS" }
        },
        new ExamSeedSpec
        {
            Code = "SSC-STENO", Slug = "ssc-stenographer", Name = "SSC Stenographer Grade C & D", ShortName = "SSC Stenographer",
            CategorySlug = "ssc", OrgCode = "SSC", Department = "Central Ministries & Attached Offices", Designation = "Stenographer Grade C / Grade D",
            Group = GroupClassification.GroupC, IsGraduate = false, OfficialWebsite = "https://ssc.gov.in",
            CycleYear = 2026, Status = ExamStatus.UpcomingNotification, Confidence = DataConfidence.HistoricalCycleExpected, VerificationStatus = VerificationStatus.TentativeCalendar, VacancyType = VacancyType.NotAnnounced,
            MinAge = 18, MaxAge = 30, MinQualLevel = "12th", AcceptsAnyGraduate = true,
            QualificationSummary = "12th pass + Mandatory Stenography Skill Test (80/100 wpm)",
            VerificationNotes = "Requires mandatory shorthand/stenography transcription speed at 80 wpm (Grade D) or 100 wpm (Grade C).",
            SyllabusSubjectCodes = new[] { "GEN_INTEL", "ENGLISH", "GEN_AWARENESS" }
        },

        // ==================== C. BANKING (9) ====================
        new ExamSeedSpec
        {
            Code = "IBPS-PO", Slug = "ibps-po", Name = "IBPS Probationary Officer / Management Trainee (CRP PO/MT)", ShortName = "IBPS PO",
            CategorySlug = "banking", OrgCode = "IBPS", Department = "11 Public Sector Banks", Designation = "Probationary Officer (Scale I)",
            Group = GroupClassification.GroupA, OfficialWebsite = "https://www.ibps.in",
            CycleYear = 2026, NotificationNumber = "CRP PO/MT-XVI", Status = ExamStatus.ExaminationCompleted,
            NotificationDate = new DateOnly(2026, 6, 30), AppStart = new DateOnly(2026, 7, 1), AppEnd = new DateOnly(2026, 7, 26),
            PrelimDate = new DateOnly(2026, 8, 22), MainsDate = new DateOnly(2026, 10, 4), AgeCutoffDate = new DateOnly(2026, 7, 1),
            TotalVacancies = 7565, VacancyType = VacancyType.Official,
            MinAge = 20, MaxAge = 30, AcceptsAnyGraduate = true,
            QualificationSummary = "Graduation in any discipline from a recognized university",
            SalaryPayLevel = "JMGS-I Basic ₹48,480 + DA/HRA/Lease",
            SelectionProcedure = "Prelims -> Mains (Objective + Descriptive) -> Interview",
            NegativeMarking = "0.25 penalty per wrong answer",
            SyllabusSubjectCodes = new[] { "QUANT", "REASONING", "ENGLISH", "BANKING_AWARENESS", "CURRENT_AFFAIRS", "COMPUTER_AWARENESS" },
            History = new()
            {
                (2023, HistoricalDataStatus.Verified, 3049, null, null, null, "CRP PO/MT-XIII"),
                (2024, HistoricalDataStatus.Verified, 4455, null, null, null, "CRP PO/MT-XIV"),
                (2025, HistoricalDataStatus.Verified, 5208, null, null, null, "CRP PO/MT-XV"),
                (2026, HistoricalDataStatus.Verified, 7565, null, new DateOnly(2026, 6, 30), new DateOnly(2026, 8, 22), "CRP PO/MT-XVI official vacancies"),
                (2027, HistoricalDataStatus.AwaitingOfficialNotification, null, null, null, null, "Awaiting IBPS 2027-28 Calendar")
            }
        },
        new ExamSeedSpec
        {
            Code = "IBPS-CSA", Slug = "ibps-csa-clerk", Name = "IBPS Customer Service Associate (Clerk / CRP CSA)", ShortName = "IBPS CSA",
            CategorySlug = "banking", OrgCode = "IBPS", Department = "Public Sector Banks", Designation = "Customer Service Associate (Clerical)",
            Group = GroupClassification.GroupC, OfficialWebsite = "https://www.ibps.in",
            CycleYear = 2026, Status = ExamStatus.ExaminationCompleted, Confidence = DataConfidence.OfficialCalendarTentative, VerificationStatus = VerificationStatus.TentativeCalendar, VacancyType = VacancyType.NotAnnounced,
            MinAge = 20, MaxAge = 28, AcceptsAnyGraduate = true,
            QualificationSummary = "Bachelor's degree in any discipline + Computer literacy + proficiency in local language of applying state (Hindi for Bihar)",
            SyllabusSubjectCodes = new[] { "QUANT", "REASONING", "ENGLISH", "BANKING_AWARENESS", "COMPUTER_AWARENESS", "CURRENT_AFFAIRS" }
        },
        new ExamSeedSpec
        {
            Code = "IBPS-SO-IT", Slug = "ibps-so-it-officer", Name = "IBPS Specialist Officer – IT Officer (Scale I)", ShortName = "IBPS SO IT",
            CategorySlug = "banking", OrgCode = "IBPS", Department = "Public Sector Banks (IT Infrastructure & Core Banking)", Designation = "IT Officer (Scale I)",
            Group = GroupClassification.GroupA, IsIt = true, OfficialWebsite = "https://www.ibps.in",
            CycleYear = 2026, NotificationNumber = "CRP SPL-XIV / 2026", Status = ExamStatus.ExaminationCompleted,
            NotificationDate = new DateOnly(2026, 7, 1), AppStart = new DateOnly(2026, 7, 1), AppEnd = new DateOnly(2026, 7, 21),
            PrelimDate = new DateOnly(2026, 8, 29), MainsDate = new DateOnly(2026, 11, 1), ExpectedResultDate = new DateOnly(2026, 12, 10),
            AgeCutoffDate = new DateOnly(2026, 7, 1), TotalVacancies = 301, ItVacancies = 301, VacancyType = VacancyType.Official,
            VacancyNotes = "301 IT Officer (Scale I) posts out of 1,035 total IBPS SO 2026 vacancies.",
            MinAge = 20, MaxAge = 30, MinQualLevel = "BTechBE", RequiresEngineering = true, AcceptsAnyGraduate = false,
            AcceptedBranches = "Computer Science,Computer Applications,Information Technology,Electronics,Electronics & Telecommunications,Electronics & Communication,Electronics & Instrumentation",
            QualificationSummary = "4-year Engineering/Tech Degree in CS/CA/IT/ECE/E&I OR Post Graduate Degree in Electronics/CS/IT/CA OR DOEACC 'B' level",
            SalaryPayLevel = "JMGS-I Basic ₹48,480 + Banking Allowances",
            SelectionProcedure = "Prelims -> Mains (Professional IT Knowledge) -> Interview",
            NegativeMarking = "0.25 marks penalty in Prelims & Mains",
            SyllabusSubjectCodes = new[] { "DBMS", "CN", "OS", "DSA", "OOP", "SE", "CYBERSECURITY", "WEB_TECH", "QUANT", "REASONING", "ENGLISH" },
            Patterns = new() { ("Prelims", "English + Reasoning + Quantitative Aptitude", 150, 125, 120, "0.25", true), ("Mains", "Professional Knowledge (IT & CS)", 60, 60, 45, "0.25", false) },
            History = new()
            {
                (2022, HistoricalDataStatus.Verified, 44, 44, null, null, "IBPS SO IT 2022"),
                (2023, HistoricalDataStatus.Verified, 120, 120, null, null, "IBPS SO IT 2023"),
                (2024, HistoricalDataStatus.Verified, 170, 170, null, null, "IBPS SO IT 2024"),
                (2025, HistoricalDataStatus.Verified, 203, 203, null, null, "IBPS SO IT 2025"),
                (2026, HistoricalDataStatus.Verified, 301, 301, new DateOnly(2026, 7, 1), new DateOnly(2026, 8, 29), "IBPS SO IT 2026 official count"),
                (2027, HistoricalDataStatus.AwaitingOfficialNotification, null, null, null, null, "Awaiting IBPS 2027 calendar")
            }
        },
        new ExamSeedSpec
        {
            Code = "IBPS-RRB-PO", Slug = "ibps-rrb-po", Name = "IBPS RRB Officer Scale I (PO)", ShortName = "IBPS RRB PO",
            CategorySlug = "banking", OrgCode = "IBPS", Department = "Regional Rural Banks (Dakshin/Uttar Bihar Gramin Bank etc.)", Designation = "Officer Scale I",
            Group = GroupClassification.GroupB, OfficialWebsite = "https://www.ibps.in",
            CycleYear = 2026, Status = ExamStatus.ExaminationCompleted, Confidence = DataConfidence.OfficialCalendarTentative, VerificationStatus = VerificationStatus.TentativeCalendar, VacancyType = VacancyType.NotAnnounced,
            MinAge = 18, MaxAge = 30, AcceptsAnyGraduate = true,
            QualificationSummary = "Bachelor's degree in any discipline + Proficiency in local language (Hindi for Bihar RRBs)",
            SyllabusSubjectCodes = new[] { "QUANT", "REASONING", "BANKING_AWARENESS", "CURRENT_AFFAIRS", "COMPUTER_AWARENESS", "ENGLISH" }
        },
        new ExamSeedSpec
        {
            Code = "IBPS-RRB-CLERK", Slug = "ibps-rrb-clerk", Name = "IBPS RRB Office Assistant (Multipurpose / Clerk)", ShortName = "IBPS RRB Clerk",
            CategorySlug = "banking", OrgCode = "IBPS", Department = "Regional Rural Banks", Designation = "Office Assistant (Multipurpose)",
            Group = GroupClassification.GroupC, OfficialWebsite = "https://www.ibps.in",
            CycleYear = 2026, Status = ExamStatus.ExaminationCompleted, Confidence = DataConfidence.OfficialCalendarTentative, VerificationStatus = VerificationStatus.TentativeCalendar, VacancyType = VacancyType.NotAnnounced,
            MinAge = 18, MaxAge = 28, AcceptsAnyGraduate = true,
            QualificationSummary = "Bachelor's degree + Proficiency in local language of applying RRB state",
            SyllabusSubjectCodes = new[] { "QUANT", "REASONING", "COMPUTER_AWARENESS", "BANKING_AWARENESS", "CURRENT_AFFAIRS" }
        },
        new ExamSeedSpec
        {
            Code = "SBI-PO", Slug = "sbi-po", Name = "SBI Probationary Officer (PO)", ShortName = "SBI PO",
            CategorySlug = "banking", OrgCode = "SBI", Department = "State Bank of India", Designation = "Probationary Officer (JMGS-I)",
            Group = GroupClassification.GroupA, OfficialWebsite = "https://sbi.co.in/web/careers",
            CycleYear = 2026, NotificationNumber = "CRPD/PO/2026-27/09", Status = ExamStatus.ExaminationCompleted,
            NotificationDate = new DateOnly(2026, 6, 18), AppStart = new DateOnly(2026, 6, 18), AppEnd = new DateOnly(2026, 7, 8),
            PrelimDate = new DateOnly(2026, 8, 1), AgeCutoffDate = new DateOnly(2026, 4, 1),
            TotalVacancies = 1500, VacancyType = VacancyType.Official,
            MinAge = 21, MaxAge = 30, AcceptsAnyGraduate = true,
            QualificationSummary = "Graduation in any discipline from a recognized university",
            SalaryPayLevel = "JMGS-I Basic ₹48,480 + 4 advance increments + SBI Perks",
            SelectionProcedure = "Prelims -> Mains -> Psychometric Test + Group Exercise & Interview",
            SyllabusSubjectCodes = new[] { "QUANT", "REASONING", "ENGLISH", "BANKING_AWARENESS", "FINANCIAL_AWARENESS", "CURRENT_AFFAIRS", "COMPUTER_AWARENESS" },
            History = new()
            {
                (2022, HistoricalDataStatus.Verified, 1673, null, null, null, "SBI PO 2022"),
                (2023, HistoricalDataStatus.Verified, 2000, null, null, null, "SBI PO 2023"),
                (2024, HistoricalDataStatus.Verified, 600, null, null, null, "SBI PO 2024"),
                (2025, HistoricalDataStatus.Verified, 541, null, null, null, "SBI PO 2025"),
                (2026, HistoricalDataStatus.Verified, 1500, null, new DateOnly(2026, 6, 18), new DateOnly(2026, 8, 1), "CRPD/PO/2026-27/09 official count")
            }
        },
        new ExamSeedSpec
        {
            Code = "SBI-JA", Slug = "sbi-junior-associate", Name = "SBI Junior Associate (Customer Support & Sales)", ShortName = "SBI Clerk (JA)",
            CategorySlug = "banking", OrgCode = "SBI", Department = "State Bank of India", Designation = "Junior Associate (Clerical)",
            Group = GroupClassification.GroupC, OfficialWebsite = "https://sbi.co.in/web/careers",
            CycleYear = 2026, Status = ExamStatus.UpcomingNotification, ExpectedNotificationDate = new DateOnly(2026, 12, 15),
            Confidence = DataConfidence.HistoricalCycleExpected, VerificationStatus = VerificationStatus.TentativeCalendar, VacancyType = VacancyType.NotAnnounced,
            MinAge = 20, MaxAge = 28, AcceptsAnyGraduate = true,
            QualificationSummary = "Graduation in any discipline + proficiency in opted local language (Hindi for Patna Circle)",
            SyllabusSubjectCodes = new[] { "QUANT", "REASONING", "ENGLISH", "BANKING_AWARENESS", "CURRENT_AFFAIRS", "COMPUTER_AWARENESS" }
        },
        new ExamSeedSpec
        {
            Code = "RBI-GRADE-B", Slug = "rbi-grade-b", Name = "RBI Grade B Officer (DR General / DEPR / DSIM)", ShortName = "RBI Grade B",
            CategorySlug = "banking", OrgCode = "RBI", Department = "Reserve Bank of India", Designation = "Manager / Officer in Grade 'B'",
            Group = GroupClassification.GroupA, OfficialWebsite = "https://opportunities.rbi.org.in",
            CycleYear = 2026, NotificationNumber = "RBISB/DA/01/2026-27", Status = ExamStatus.ExaminationCompleted,
            NotificationDate = new DateOnly(2026, 4, 29), AppStart = new DateOnly(2026, 4, 29), AppEnd = new DateOnly(2026, 5, 20),
            TotalVacancies = 60, VacancyType = VacancyType.Official,
            MinAge = 21, MaxAge = 30, AcceptsAnyGraduate = true, MinPctGen = 60m, MinPctRes = 50m,
            QualificationSummary = "Graduation in any discipline with minimum 60% marks (50% for SC/ST/PwBD) OR Post-Graduation with 55%",
            SalaryPayLevel = "Starting Basic ₹55,200 (Approx Gross ₹1.2L+/month + RBI Quarters)",
            SelectionProcedure = "Phase-I Objective -> Phase-II (ESI + Finance & Management + English) -> Interview",
            SyllabusSubjectCodes = new[] { "ECONOMY", "FINANCIAL_AWARENESS", "BANKING_AWARENESS", "QUANT", "REASONING", "ENGLISH", "CURRENT_AFFAIRS" },
            History = new()
            {
                (2022, HistoricalDataStatus.Verified, 294, null, null, null, "RBI Grade B 2022"),
                (2023, HistoricalDataStatus.Verified, 291, null, null, null, "RBI Grade B 2023"),
                (2024, HistoricalDataStatus.Verified, 94, null, null, null, "RBI Grade B 2024"),
                (2025, HistoricalDataStatus.Verified, 120, null, null, null, "RBI Grade B 2025 (approx)"),
                (2026, HistoricalDataStatus.Verified, 60, null, new DateOnly(2026, 4, 29), null, "Advt RBISB/DA/01/2026-27")
            }
        },
        new ExamSeedSpec
        {
            Code = "RBI-ASSISTANT", Slug = "rbi-assistant", Name = "RBI Assistant Recruitment", ShortName = "RBI Assistant",
            CategorySlug = "banking", OrgCode = "RBI", Department = "Reserve Bank of India", Designation = "Assistant",
            Group = GroupClassification.GroupC, OfficialWebsite = "https://opportunities.rbi.org.in",
            CycleYear = 2026, Status = ExamStatus.UpcomingNotification, Confidence = DataConfidence.NotAnnounced, VerificationStatus = VerificationStatus.TentativeCalendar, VacancyType = VacancyType.NotAnnounced,
            MinAge = 20, MaxAge = 28, AcceptsAnyGraduate = true, MinPctGen = 50m, MinPctRes = 45m,
            QualificationSummary = "Bachelor's degree in any discipline with >=50% marks (Pass class for SC/ST/PwBD) + LPT in local language",
            SyllabusSubjectCodes = new[] { "QUANT", "REASONING", "ENGLISH", "COMPUTER_AWARENESS", "BANKING_AWARENESS", "CURRENT_AFFAIRS" }
        },

        // ==================== D. FINANCIAL REGULATORS & INSURANCE (11) ====================
        new ExamSeedSpec
        {
            Code = "SEBI-GRADE-A-IT", Slug = "sebi-grade-a-it", Name = "SEBI Grade A – Information Technology (IT) Stream", ShortName = "SEBI Grade A IT",
            CategorySlug = "financial-regulators-insurance", OrgCode = "SEBI", Department = "Securities and Exchange Board of India (IT Dept)", Designation = "Assistant Manager (Grade A) – IT",
            Group = GroupClassification.GroupA, IsIt = true, OfficialWebsite = "https://www.sebi.gov.in",
            CycleYear = 2027, Status = ExamStatus.UpcomingNotification, Confidence = DataConfidence.NotAnnounced, VerificationStatus = VerificationStatus.TentativeCalendar, VacancyType = VacancyType.NotAnnounced,
            VacancyNotes = "2027 cycle awaiting official SEBI notification; 2025 cycle notified 22 IT vacancies.",
            MinAge = 21, MaxAge = 30, MinQualLevel = "BTechBE", RequiresEngineering = true, AcceptsAnyGraduate = false,
            AcceptedBranches = "ANY", // SEBI Grade A IT officially accepts Bachelor's in Engineering in ANY branch OR Bachelor's in any discipline + PG in CS/CA/IT!
            QualificationSummary = "Bachelor's Degree in Engineering (any branch) OR Bachelor's Degree + Post Graduate Qualification in CS/CA/IT",
            SalaryPayLevel = "Grade A ₹44,500–₹89,150 (CTC ~₹1.5L+/month with Mumbai/local allowances)",
            SelectionProcedure = "Phase-I (Paper 1 General + Paper 2 IT) -> Phase-II (Paper 1 English Descriptive + Paper 2 Coding/IT) -> Interview",
            NegativeMarking = "1/4th negative marking in objective papers",
            SyllabusSubjectCodes = new[] { "DSA", "DBMS", "OS", "CN", "OOP", "SE", "CYBERSECURITY", "PROG_LANG", "QUANT", "REASONING", "ENGLISH", "FINANCIAL_AWARENESS" },
            Patterns = new() { ("Phase-I", "Paper 1 General + Paper 2 IT Stream", 130, 200, 100, "1/4th", false), ("Phase-II", "English Descriptive + IT Stream Technical", 53, 200, 120, "1/4th Objective", false) },
            History = new()
            {
                (2022, HistoricalDataStatus.Verified, 120, 24, new DateOnly(2022, 1, 5), null, "SEBI Grade A 2022: 120 total, 24 IT"),
                (2023, HistoricalDataStatus.Verified, 25, 0, new DateOnly(2023, 6, 22), null, "2023 notification was Legal stream only (0 IT)"),
                (2024, HistoricalDataStatus.Verified, 97, 24, new DateOnly(2024, 6, 11), null, "SEBI Grade A 2024: 97 total, 24 IT"),
                (2025, HistoricalDataStatus.Verified, 135, 22, new DateOnly(2025, 10, 30), null, "SEBI Grade A 2025 cycle: 135 total, 22 IT"),
                (2026, HistoricalDataStatus.AwaitingOfficialNotification, null, null, null, null, "2025 notification exams concluded early 2026; awaiting next notification"),
                (2027, HistoricalDataStatus.AwaitingOfficialNotification, null, null, null, null, "Awaiting official notification")
            }
        },
        new ExamSeedSpec
        {
            Code = "SEBI-GRADE-A-GEN", Slug = "sebi-grade-a-general", Name = "SEBI Grade A – General Stream", ShortName = "SEBI Grade A Gen",
            CategorySlug = "financial-regulators-insurance", OrgCode = "SEBI", Department = "Securities and Exchange Board of India", Designation = "Assistant Manager (Grade A) – General",
            Group = GroupClassification.GroupA, OfficialWebsite = "https://www.sebi.gov.in",
            CycleYear = 2027, Status = ExamStatus.UpcomingNotification, Confidence = DataConfidence.NotAnnounced, VerificationStatus = VerificationStatus.TentativeCalendar, VacancyType = VacancyType.NotAnnounced,
            MinAge = 21, MaxAge = 30, MinQualLevel = "PostGraduate", RequiresEngineering = true, AcceptsAnyGraduate = false,
            AcceptedBranches = "ANY",
            QualificationSummary = "Master's Degree in any discipline OR Bachelor's Degree in Law OR Bachelor's Degree in Engineering (B.Tech/B.E.) OR CA/CFA/CS/CWA",
            VerificationNotes = "B.Tech/B.E. graduates are explicitly eligible for SEBI Grade A General stream as per SEBI Rules.",
            SalaryPayLevel = "Grade A ₹44,500–₹89,150",
            SyllabusSubjectCodes = new[] { "FINANCIAL_AWARENESS", "ECONOMY", "QUANT", "REASONING", "ENGLISH", "CURRENT_AFFAIRS" }
        },
        new ExamSeedSpec
        {
            Code = "NABARD-GRADE-A", Slug = "nabard-grade-a", Name = "NABARD Grade A (Assistant Manager – RDBS / IT)", ShortName = "NABARD Grade A",
            CategorySlug = "financial-regulators-insurance", OrgCode = "NABARD", Department = "NABARD", Designation = "Assistant Manager (Grade A)",
            Group = GroupClassification.GroupA, IsIt = true, OfficialWebsite = "https://www.nabard.org",
            CycleYear = 2026, Status = ExamStatus.UpcomingNotification, ExpectedNotificationDate = new DateOnly(2026, 11, 25),
            Confidence = DataConfidence.HistoricalCycleExpected, VerificationStatus = VerificationStatus.TentativeCalendar, VacancyType = VacancyType.NotAnnounced,
            MinAge = 21, MaxAge = 30, AcceptsAnyGraduate = true, MinPctGen = 60m, MinPctRes = 55m,
            QualificationSummary = "Bachelor's degree with >=60% marks (55% SC/ST/PwBD) for RDBS General; CS/IT B.Tech eligible for IT sub-discipline",
            SyllabusSubjectCodes = new[] { "QUANT", "REASONING", "ENGLISH", "COMPUTER_AWARENESS", "ECONOMY", "FINANCIAL_AWARENESS", "CURRENT_AFFAIRS" }
        },
        new ExamSeedSpec
        {
            Code = "NABARD-GRADE-B", Slug = "nabard-grade-b", Name = "NABARD Grade B (Manager – RDBS)", ShortName = "NABARD Grade B",
            CategorySlug = "financial-regulators-insurance", OrgCode = "NABARD", Department = "NABARD", Designation = "Manager in Grade B",
            Group = GroupClassification.GroupA, OfficialWebsite = "https://www.nabard.org",
            CycleYear = 2026, Status = ExamStatus.UpcomingNotification, Confidence = DataConfidence.NotAnnounced, VerificationStatus = VerificationStatus.TentativeCalendar, VacancyType = VacancyType.NotAnnounced,
            MinAge = 25, MaxAge = 32, MinQualLevel = "PostGraduate", RequiresExp = true, MinExpMonths = 36,
            ExpNotes = "Typically requires 3 years relevant experience + Post Graduate degree (verify active advt).",
            QualificationSummary = "Post-Graduate degree with >=60% marks & 3 years relevant experience",
            SyllabusSubjectCodes = new[] { "ECONOMY", "FINANCIAL_AWARENESS", "QUANT", "REASONING", "ENGLISH", "CURRENT_AFFAIRS" }
        },
        new ExamSeedSpec
        {
            Code = "PFRDA-GRADE-A", Slug = "pfrda-grade-a", Name = "PFRDA Grade A (Assistant Manager – General & IT)", ShortName = "PFRDA Grade A",
            CategorySlug = "financial-regulators-insurance", OrgCode = "PFRDA", Department = "Pension Fund Regulatory & Development Authority", Designation = "Officer Grade A",
            Group = GroupClassification.GroupA, IsIt = true, OfficialWebsite = "https://www.pfrda.org.in",
            CycleYear = 2026, Status = ExamStatus.UpcomingNotification, Confidence = DataConfidence.NotAnnounced, VerificationStatus = VerificationStatus.TentativeCalendar, VacancyType = VacancyType.NotAnnounced,
            MinAge = 21, MaxAge = 30, RequiresEngineering = true, AcceptsAnyGraduate = false, AcceptedBranches = "CS,IT,ECE",
            QualificationSummary = "Master's / B.Tech / B.E. (CS/IT/ECE for IT stream; Engineering/Master's/Law/CA for General stream)",
            SyllabusSubjectCodes = new[] { "FINANCIAL_AWARENESS", "ECONOMY", "QUANT", "REASONING", "ENGLISH", "DBMS", "CN", "CYBERSECURITY" }
        },
        new ExamSeedSpec
        {
            Code = "IRDAI-AM", Slug = "irdai-assistant-manager", Name = "IRDAI Assistant Manager", ShortName = "IRDAI AM",
            CategorySlug = "financial-regulators-insurance", OrgCode = "IRDAI", Department = "Insurance Regulatory & Development Authority of India", Designation = "Assistant Manager (General / IT)",
            Group = GroupClassification.GroupA, OfficialWebsite = "https://irdai.gov.in",
            CycleYear = 2026, Status = ExamStatus.UpcomingNotification, Confidence = DataConfidence.NotAnnounced, VerificationStatus = VerificationStatus.TentativeCalendar, VacancyType = VacancyType.NotAnnounced,
            MinAge = 21, MaxAge = 30, AcceptsAnyGraduate = true, MinPctGen = 60m, MinPctRes = 60m,
            QualificationSummary = "Graduation with >=60% marks in any stream",
            SyllabusSubjectCodes = new[] { "FINANCIAL_AWARENESS", "ECONOMY", "QUANT", "REASONING", "ENGLISH", "CURRENT_AFFAIRS" }
        },
        new ExamSeedSpec
        {
            Code = "IFSCA-GRADE-A", Slug = "ifsca-grade-a", Name = "IFSCA Grade A (Assistant Manager)", ShortName = "IFSCA Grade A",
            CategorySlug = "financial-regulators-insurance", OrgCode = "IFSCA", Department = "International Financial Services Centres Authority (GIFT City)", Designation = "Officer Grade A",
            Group = GroupClassification.GroupA, OfficialWebsite = "https://ifsca.gov.in",
            CycleYear = 2026, Status = ExamStatus.UpcomingNotification, Confidence = DataConfidence.NotAnnounced, VerificationStatus = VerificationStatus.TentativeCalendar, VacancyType = VacancyType.NotAnnounced,
            MinAge = 21, MaxAge = 30, AcceptsAnyGraduate = true,
            QualificationSummary = "Master's in Economics/Commerce/Finance OR Bachelor's in Law / Information Technology / Engineering / CA",
            SyllabusSubjectCodes = new[] { "FINANCIAL_AWARENESS", "ECONOMY", "QUANT", "REASONING", "ENGLISH", "CURRENT_AFFAIRS" }
        },
        new ExamSeedSpec
        {
            Code = "SIDBI-GRADE-AB", Slug = "sidbi-grade-a-b", Name = "SIDBI Grade A & Grade B (General / Specialist IT)", ShortName = "SIDBI Grade A/B",
            CategorySlug = "financial-regulators-insurance", OrgCode = "SIDBI", Department = "Small Industries Development Bank of India", Designation = "Assistant Manager (Grade A) / Manager (Grade B)",
            Group = GroupClassification.GroupA, IsIt = true, OfficialWebsite = "https://www.sidbi.in",
            CycleYear = 2026, Status = ExamStatus.UpcomingNotification, Confidence = DataConfidence.NotAnnounced, VerificationStatus = VerificationStatus.TentativeCalendar, VacancyType = VacancyType.NotAnnounced,
            MinAge = 21, MaxAge = 30, AcceptsAnyGraduate = true, MinPctGen = 60m, MinPctRes = 55m, RequiresExp = true, MinExpMonths = 24,
            ExpNotes = "SIDBI Grade A/B frequently stipulates 2+ years banking/financial/IT domain experience; check active notification.",
            QualificationSummary = "Bachelor's in Engineering/Law/Commerce or PG >=60% + post-qualification experience (check cycle rules)",
            SyllabusSubjectCodes = new[] { "FINANCIAL_AWARENESS", "BANKING_AWARENESS", "QUANT", "REASONING", "ENGLISH", "CURRENT_AFFAIRS" }
        },
        new ExamSeedSpec
        {
            Code = "LIC-AAO", Slug = "lic-aao", Name = "LIC Assistant Administrative Officer (AAO – Generalist & IT)", ShortName = "LIC AAO",
            CategorySlug = "financial-regulators-insurance", OrgCode = "LIC", Department = "Life Insurance Corporation of India", Designation = "Assistant Administrative Officer (Class I)",
            Group = GroupClassification.GroupA, IsIt = true, OfficialWebsite = "https://licindia.in",
            CycleYear = 2026, Status = ExamStatus.UpcomingNotification, Confidence = DataConfidence.NotAnnounced, VerificationStatus = VerificationStatus.TentativeCalendar, VacancyType = VacancyType.NotAnnounced,
            MinAge = 21, MaxAge = 30, AcceptsAnyGraduate = true,
            QualificationSummary = "Bachelor's degree in any discipline (Generalist); B.Tech/B.E. in CS/IT/ECE or MCA for AAO (IT)",
            SyllabusSubjectCodes = new[] { "QUANT", "REASONING", "ENGLISH", "FINANCIAL_AWARENESS", "CURRENT_AFFAIRS", "DBMS", "CN", "OS" }
        },
        new ExamSeedSpec
        {
            Code = "NIACL-AO", Slug = "niacl-ao", Name = "NIACL Administrative Officer (Scale I – Generalist & IT)", ShortName = "NIACL AO",
            CategorySlug = "financial-regulators-insurance", OrgCode = "NIACL", Department = "The New India Assurance Co. Ltd.", Designation = "Administrative Officer (Scale I)",
            Group = GroupClassification.GroupA, IsIt = true, OfficialWebsite = "https://www.newindia.co.in",
            CycleYear = 2026, Status = ExamStatus.UpcomingNotification, Confidence = DataConfidence.NotAnnounced, VerificationStatus = VerificationStatus.TentativeCalendar, VacancyType = VacancyType.NotAnnounced,
            MinAge = 21, MaxAge = 30, AcceptsAnyGraduate = true, MinPctGen = 60m, MinPctRes = 55m,
            QualificationSummary = "Graduate/PG with >=60% (55% SC/ST/PwBD) for Generalist; B.E./B.Tech CS/IT or MCA >=60% for IT Specialist",
            SyllabusSubjectCodes = new[] { "QUANT", "REASONING", "ENGLISH", "FINANCIAL_AWARENESS", "CURRENT_AFFAIRS", "DBMS", "CN" }
        },
        new ExamSeedSpec
        {
            Code = "GIC-AM", Slug = "gic-assistant-manager", Name = "GIC Re Assistant Manager (Scale I – General & IT)", ShortName = "GIC Assistant Manager",
            CategorySlug = "financial-regulators-insurance", OrgCode = "GIC", Department = "General Insurance Corporation of India", Designation = "Assistant Manager (Scale I)",
            Group = GroupClassification.GroupA, IsIt = true, OfficialWebsite = "https://www.gicre.in",
            CycleYear = 2026, Status = ExamStatus.UpcomingNotification, Confidence = DataConfidence.NotAnnounced, VerificationStatus = VerificationStatus.TentativeCalendar, VacancyType = VacancyType.NotAnnounced,
            MinAge = 21, MaxAge = 30, AcceptsAnyGraduate = true, MinPctGen = 60m, MinPctRes = 55m,
            QualificationSummary = "Graduation >=60% (55% SC/ST); B.E./B.Tech CS/IT >=60% for IT stream",
            SyllabusSubjectCodes = new[] { "QUANT", "REASONING", "ENGLISH", "FINANCIAL_AWARENESS", "CURRENT_AFFAIRS", "OS", "CN", "DBMS" }
        }
    };
}
