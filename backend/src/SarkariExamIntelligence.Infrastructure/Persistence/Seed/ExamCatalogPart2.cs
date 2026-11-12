using SarkariExamIntelligence.Domain.Enums;

namespace SarkariExamIntelligence.Infrastructure.Persistence.Seed;

public static class ExamCatalogPart2
{
    public static List<ExamSeedSpec> GetSpecs() => new()
    {
        // ==================== E. RAILWAYS (6) ====================
        new ExamSeedSpec
        {
            Code = "RRB-NTPC-GRAD", Slug = "rrb-ntpc-graduate", Name = "RRB NTPC Graduate Level (CEN 06/2026)", ShortName = "RRB NTPC Graduate",
            CategorySlug = "railways", OrgCode = "RRB", Department = "Ministry of Railways (Commercial, Traffic, Accounts)", Designation = "Station Master / Chief Commercial Ticket Supervisor / Goods Train Manager",
            Group = GroupClassification.GroupC, OfficialWebsite = "https://www.rrbapply.gov.in",
            CycleYear = 2026, NotificationNumber = "CEN 06/2026", Status = ExamStatus.ApplicationsClosed,
            NotificationDate = new DateOnly(2026, 10, 7), AppStart = new DateOnly(2026, 10, 8), AppEnd = new DateOnly(2026, 11, 8),
            AgeCutoffDate = new DateOnly(2027, 1, 1), AgeCutoffBasis = "1st January 2027 as per CEN 06/2026",
            TotalVacancies = 3548, VacancyType = VacancyType.Official,
            MinAge = 18, MaxAge = 33, AcceptsAnyGraduate = true,
            QualificationSummary = "University Degree or equivalent from a recognized university",
            SalaryPayLevel = "Level 5 & Level 6 (₹29,200–₹35,400 initial basic + Railway Allowances)",
            SelectionProcedure = "CBT-1 -> CBT-2 -> CBAT/Typing Skill Test (post-specific) -> DV & Medical",
            NegativeMarking = "1/3rd mark deducted per wrong answer",
            SyllabusSubjectCodes = new[] { "QUANT", "GEN_INTEL", "REASONING", "GEN_AWARENESS", "CURRENT_AFFAIRS", "HISTORY", "POLITY", "GEOGRAPHY" },
            History = new()
            {
                (2024, HistoricalDataStatus.Verified, 8113, null, null, null, "CEN 05/2024 Graduate posts"),
                (2025, HistoricalDataStatus.Verified, 5810, null, null, null, "2025 NTPC Graduate posts"),
                (2026, HistoricalDataStatus.Verified, 3548, null, new DateOnly(2026, 10, 7), null, "CEN 06/2026 official notification")
            }
        },
        new ExamSeedSpec
        {
            Code = "RRB-NTPC-UG", Slug = "rrb-ntpc-undergraduate", Name = "RRB NTPC Undergraduate Level (CEN 07/2026)", ShortName = "RRB NTPC UG",
            CategorySlug = "railways", OrgCode = "RRB", Department = "Ministry of Railways", Designation = "Commercial cum Ticket Clerk / Junior Clerk cum Typist / Accounts Clerk",
            Group = GroupClassification.GroupC, IsGraduate = false, OfficialWebsite = "https://www.rrbapply.gov.in",
            CycleYear = 2026, NotificationNumber = "CEN 07/2026", Status = ExamStatus.ClosingSoon,
            NotificationDate = new DateOnly(2026, 10, 15), AppStart = new DateOnly(2026, 10, 15), AppEnd = new DateOnly(2026, 11, 13),
            AgeCutoffDate = new DateOnly(2027, 1, 1), AgeCutoffBasis = "1st January 2027 as per CEN 07/2026",
            TotalVacancies = 1688, VacancyType = VacancyType.Official,
            MinAge = 18, MaxAge = 30, MinQualLevel = "12th", AcceptsAnyGraduate = true,
            QualificationSummary = "12th (+2 stage) or equivalent with >=50% aggregate (Graduates also eligible within age limit)",
            FeeSummary = "₹500 General/OBC (₹400 refunded after CBT-1); ₹250 SC/ST/EBC/Female (full refund after CBT-1)",
            SalaryPayLevel = "Level 2 & Level 3 (₹19,900–₹21,700 basic)",
            SyllabusSubjectCodes = new[] { "QUANT", "GEN_INTEL", "REASONING", "GEN_AWARENESS", "CURRENT_AFFAIRS" },
            History = new()
            {
                (2024, HistoricalDataStatus.Verified, 3445, null, null, null, "CEN 06/2024 UG posts"),
                (2025, HistoricalDataStatus.Verified, 3058, null, null, null, "2025 NTPC UG posts"),
                (2026, HistoricalDataStatus.Verified, 1688, null, new DateOnly(2026, 10, 15), null, "CEN 07/2026 active application window")
            }
        },
        new ExamSeedSpec
        {
            Code = "RRB-JE", Slug = "rrb-je", Name = "RRB Junior Engineer, DMS & CMA (CEN 04/2026)", ShortName = "RRB JE",
            CategorySlug = "railways", OrgCode = "RRB", Department = "Indian Railways (Engineering, S&T, IT, Stores)", Designation = "Junior Engineer (IT / Civil / Mech / Elec / ECE) / DMS / CMA",
            Group = GroupClassification.GroupC, IsIt = true, OfficialWebsite = "https://www.rrbapply.gov.in",
            CycleYear = 2026, NotificationNumber = "CEN 04/2026", Status = ExamStatus.ExaminationCompleted,
            NotificationDate = new DateOnly(2026, 8, 13), AppStart = new DateOnly(2026, 8, 14), AppEnd = new DateOnly(2026, 9, 13),
            PrelimDate = new DateOnly(2026, 10, 27), AgeCutoffDate = new DateOnly(2027, 1, 1),
            TotalVacancies = 4487, VacancyType = VacancyType.Official, VacancyNotes = "Revised total 4,487 posts across Zonal Railways (JE + DMS + CMA).",
            MinAge = 18, MaxAge = 33, MinQualLevel = "BTechBE", RequiresEngineering = true, AcceptsAnyGraduate = false,
            AcceptedBranches = "IT,CS,ECE,EE,ME,CE",
            QualificationSummary = "3-year Diploma / B.E. / B.Tech in specified Engineering branch (JE-IT accepts PGDCA/B.Sc CS/BCA/B.Tech IT/CS); DMS accepts any engineering degree",
            VerificationNotes = "B.Tech IT is eligible for JE (IT) and Depot Material Superintendent (DMS) posts, NOT for Civil/Mechanical/Electrical JE posts — check post-code eligibility.",
            SyllabusSubjectCodes = new[] { "QUANT", "GEN_INTEL", "GEN_AWARENESS", "COMPUTER_AWARENESS", "DBMS", "CN", "OS" },
            History = new()
            {
                (2024, HistoricalDataStatus.Verified, 7951, null, null, null, "CEN 03/2024"),
                (2025, HistoricalDataStatus.Verified, 2569, null, null, null, "2025 CEN initial"),
                (2026, HistoricalDataStatus.Verified, 4487, null, new DateOnly(2026, 8, 13), new DateOnly(2026, 10, 27), "CEN 04/2026 revised total")
            }
        },
        new ExamSeedSpec
        {
            Code = "RRB-ALP", Slug = "rrb-alp", Name = "RRB Assistant Loco Pilot (ALP)", ShortName = "RRB ALP",
            CategorySlug = "railways", OrgCode = "RRB", Department = "Indian Railways (Mechanical / Electrical)", Designation = "Assistant Loco Pilot",
            Group = GroupClassification.GroupC, IsGraduate = false, OfficialWebsite = "https://www.rrbapply.gov.in",
            CycleYear = 2026, Status = ExamStatus.UpcomingNotification, Confidence = DataConfidence.HistoricalCycleExpected, VerificationStatus = VerificationStatus.TentativeCalendar, VacancyType = VacancyType.NotAnnounced,
            MinAge = 18, MaxAge = 30, MinQualLevel = "Diploma", RequiresEngineering = true, AcceptsAnyGraduate = false,
            AcceptedBranches = "Mechanical Engineering,Electrical Engineering,Electronics & Communication,Automobile Engineering",
            QualificationSummary = "Matriculation + ITI OR 3-yr Diploma / Engineering Degree in Mechanical, Electrical, Electronics or Automobile Engineering (CS/IT NOT accepted)",
            SyllabusSubjectCodes = new[] { "QUANT", "GEN_INTEL", "GEN_AWARENESS", "CURRENT_AFFAIRS" }
        },
        new ExamSeedSpec
        {
            Code = "RRB-TECH", Slug = "rrb-technician", Name = "RRB Technician Grade I Signal & Grade III", ShortName = "RRB Technician",
            CategorySlug = "railways", OrgCode = "RRB", Department = "Indian Railways (S&T / Traction / C&W)", Designation = "Technician Grade I (Signal) / Grade III",
            Group = GroupClassification.GroupC, IsIt = true, IsGraduate = false, OfficialWebsite = "https://www.rrbapply.gov.in",
            CycleYear = 2026, Status = ExamStatus.UpcomingNotification, Confidence = DataConfidence.HistoricalCycleExpected, VerificationStatus = VerificationStatus.TentativeCalendar, VacancyType = VacancyType.NotAnnounced,
            MinAge = 18, MaxAge = 33, MinQualLevel = "BTechBE", RequiresEngineering = true, AcceptsAnyGraduate = false,
            AcceptedBranches = "Physics,Electronics,Computer Science,Information Technology,Instrumentation",
            QualificationSummary = "Technician Grade I Signal accepts B.Sc (Physics/Electronics/CS/IT) or B.E./B.Tech in CS/IT/Electronics/Instrumentation; Grade III requires ITI",
            VerificationNotes = "B.Tech IT qualifies specifically for Technician Grade I (Signal), NOT ITI-only Grade III trades.",
            SyllabusSubjectCodes = new[] { "QUANT", "GEN_INTEL", "GEN_AWARENESS", "COMPUTER_AWARENESS", "DIGITAL_LOGIC" }
        },
        new ExamSeedSpec
        {
            Code = "RRB-GROUP-D", Slug = "rrb-group-d", Name = "RRB Level-1 (Group D) Recruitment", ShortName = "RRB Group D",
            CategorySlug = "railways", OrgCode = "RRB", Department = "Indian Railways", Designation = "Track Maintainer / Pointsman / Assistant",
            Group = GroupClassification.GroupC, IsGraduate = false, OfficialWebsite = "https://www.rrbapply.gov.in",
            CycleYear = 2026, Status = ExamStatus.UpcomingNotification, Confidence = DataConfidence.NotAnnounced, VerificationStatus = VerificationStatus.TentativeCalendar, VacancyType = VacancyType.NotAnnounced,
            MinAge = 18, MaxAge = 33, MinQualLevel = "10th", AcceptsAnyGraduate = true,
            QualificationSummary = "10th Pass OR ITI from institutions recognized by NCVT/SCVT",
            SyllabusSubjectCodes = new[] { "QUANT", "GEN_INTEL", "GEN_AWARENESS", "CURRENT_AFFAIRS" }
        },

        // ==================== F. BIHAR GOVERNMENT (12) ====================
        new ExamSeedSpec
        {
            Code = "BPSC-CCE", Slug = "bpsc-cce", Name = "BPSC Integrated Combined Competitive Examination (72nd CCE)", ShortName = "BPSC CCE",
            CategorySlug = "bihar-government", OrgCode = "BPSC", Department = "Government of Bihar (SDM, DSP, Commercial Tax, Revenue)", Designation = "Bihar Administrative / Police / Finance Service Officer",
            Group = GroupClassification.GroupA, IsBihar = true, OfficialWebsite = "https://bpsc.bihar.gov.in",
            CycleYear = 2026, NotificationNumber = "72nd Integrated CCE", Status = ExamStatus.ExaminationScheduled,
            NotificationDate = new DateOnly(2026, 5, 5), AppStart = new DateOnly(2026, 5, 7), AgeCutoffDate = new DateOnly(2026, 8, 1),
            TotalVacancies = 1189, VacancyType = VacancyType.Official,
            MinAge = 20, MaxAge = 37, ObcRelaxation = 3, ScStRelaxation = 5, BiharDomicileForReservation = true,
            BiharCertConditions = "Bihar permanent residential (domicile) + valid Bihar BC/EBC Non-Creamy Layer or EWS/SC/ST certificate mandatory for state quota & 40/42 yrs age limit.",
            AcceptsAnyGraduate = true, QualificationSummary = "Bachelor's degree in any discipline from a recognized university",
            SalaryPayLevel = "Level 7 & Level 9 (Bihar State Gazetted Pay Scale)",
            SelectionProcedure = "Prelims (150 Qs, 1/3rd negative) -> Mains (Hindi, GS-I, GS-II, Essay, Optional) -> Interview",
            SyllabusSubjectCodes = new[] { "BIHAR_GK", "HISTORY", "GEOGRAPHY", "POLITY", "ECONOMY", "CURRENT_AFFAIRS", "QUANT", "REASONING" },
            History = new()
            {
                (2022, HistoricalDataStatus.Verified, 324, null, null, null, "68th BPSC CCE"),
                (2023, HistoricalDataStatus.Verified, 475, null, null, null, "69th BPSC Integrated CCE"),
                (2024, HistoricalDataStatus.Verified, 2035, null, null, new DateOnly(2024, 12, 13), "70th BPSC Integrated CCE"),
                (2025, HistoricalDataStatus.Verified, 1298, null, null, new DateOnly(2025, 9, 13), "71st BPSC Integrated CCE"),
                (2026, HistoricalDataStatus.Verified, 1189, null, new DateOnly(2026, 5, 5), null, "72nd BPSC Integrated CCE")
            },
            Cutoffs = new() { (2024, "70th Prelims", CategoryType.General, 91m, 150m), (2024, "70th Prelims", CategoryType.OBCNCL, 87m, 150m) }
        },
        new ExamSeedSpec
        {
            Code = "BPSC-ASO", Slug = "bpsc-assistant-section-officer", Name = "BPSC Assistant Section Officer (ASO)", ShortName = "BPSC ASO",
            CategorySlug = "bihar-government", OrgCode = "BPSC", Department = "BPSC Secretariat / Bihar Govt", Designation = "Assistant Section Officer",
            Group = GroupClassification.GroupB, IsBihar = true, OfficialWebsite = "https://bpsc.bihar.gov.in",
            CycleYear = 2026, Status = ExamStatus.UpcomingNotification, Confidence = DataConfidence.NotAnnounced, VerificationStatus = VerificationStatus.TentativeCalendar, VacancyType = VacancyType.NotAnnounced,
            MinAge = 21, MaxAge = 37, BiharDomicileForReservation = true,
            BiharCertConditions = "Bihar Domicile & NCL/EWS/Caste certificate required for reservation and upper age relaxation (40 yrs BC/EBC/UR-F, 42 yrs SC/ST).",
            AcceptsAnyGraduate = true, QualificationSummary = "Bachelor's degree in any discipline from a recognized university",
            SyllabusSubjectCodes = new[] { "BIHAR_GK", "QUANT", "REASONING", "POLITY", "HISTORY", "CURRENT_AFFAIRS", "GEN_AWARENESS" }
        },
        new ExamSeedSpec
        {
            Code = "BPSC-AUDITOR", Slug = "bpsc-auditor", Name = "BPSC Auditor (Bihar Panchayati Raj Audit Service)", ShortName = "BPSC Auditor",
            CategorySlug = "bihar-government", OrgCode = "BPSC", Department = "Panchayati Raj Department, Govt of Bihar", Designation = "Auditor",
            Group = GroupClassification.GroupB, IsBihar = true, OfficialWebsite = "https://bpsc.bihar.gov.in",
            CycleYear = 2026, Status = ExamStatus.UpcomingNotification, Confidence = DataConfidence.NotAnnounced, VerificationStatus = VerificationStatus.TentativeCalendar, VacancyType = VacancyType.NotAnnounced,
            MinAge = 21, MaxAge = 37, BiharDomicileForReservation = true, AcceptsAnyGraduate = false, RequiresEngineering = false,
            QualificationSummary = "Bachelor's degree in Commerce, Economics, Statistics, or Mathematics (OR MBA Finance / CA / ICWA / CS)",
            VerificationNotes = "B.Tech IT graduates must verify if Mathematics/Statistics studied at graduation satisfies BPSC Auditor stream clause — typically restricted to Commerce/Eco/Stats/Maths.",
            SyllabusSubjectCodes = new[] { "QUANT", "ECONOMY", "BIHAR_GK", "POLITY", "CURRENT_AFFAIRS" }
        },
        new ExamSeedSpec
        {
            Code = "BPSC-TECH-AE", Slug = "bpsc-technical-recruitment", Name = "BPSC Assistant Engineer (Technical Recruitment)", ShortName = "BPSC Technical AE",
            CategorySlug = "bihar-government", OrgCode = "BPSC", Department = "Road Construction / Water Resources / PHED / BCD Bihar", Designation = "Assistant Engineer (Civil / Mechanical / Electrical)",
            Group = GroupClassification.GroupBGazetted, IsBihar = true, OfficialWebsite = "https://bpsc.bihar.gov.in",
            CycleYear = 2026, Status = ExamStatus.UpcomingNotification, Confidence = DataConfidence.NotAnnounced, VerificationStatus = VerificationStatus.TentativeCalendar, VacancyType = VacancyType.NotAnnounced,
            MinAge = 21, MaxAge = 37, BiharDomicileForReservation = true, MinQualLevel = "BTechBE", RequiresEngineering = true, AcceptsAnyGraduate = false,
            AcceptedBranches = "Civil Engineering,Mechanical Engineering,Electrical Engineering",
            QualificationSummary = "B.E. / B.Tech specifically in Civil, Mechanical, or Electrical Engineering (branch-restricted)",
            BiharCertConditions = "Bihar domicile mandatory for BC/EBC/EWS/SC/ST reservation.",
            SyllabusSubjectCodes = new[] { "ENGLISH", "GEN_AWARENESS", "BIHAR_GK", "CURRENT_AFFAIRS" }
        },
        new ExamSeedSpec
        {
            Code = "BSSC-CGL", Slug = "bssc-cgl", Name = "BSSC Combined Graduate Level (CGL-4)", ShortName = "BSSC CGL",
            CategorySlug = "bihar-government", OrgCode = "BSSC", Department = "General Administration & Bihar State Departments", Designation = "Assistant Section Officer / Planning Assistant / Malaria Inspector / Auditor",
            Group = GroupClassification.GroupBNonGazetted, IsBihar = true, OfficialWebsite = "https://bssc.bihar.gov.in",
            CycleYear = 2026, Status = ExamStatus.NotificationReleased, Confidence = DataConfidence.OfficialCalendarTentative, VerificationStatus = VerificationStatus.TentativeCalendar,
            TotalVacancies = 1883, VacancyType = VacancyType.Proposed, VacancyNotes = "Approx 1,883 posts referenced across Bihar state departments under BSSC CGL-4.",
            MinAge = 21, MaxAge = 37, BiharDomicileForReservation = true, AcceptsAnyGraduate = true,
            BiharCertConditions = "Bihar domicile & NCL/EWS certificate issued by Circle Officer/RO mandatory for state reservation.",
            QualificationSummary = "Graduation in any discipline from a recognized university",
            SalaryPayLevel = "Level 7 (₹44,900–₹1,42,400)", SelectionProcedure = "Prelims (150 Qs) -> Mains (Hindi + GS/Maths/Reasoning) -> DV",
            SyllabusSubjectCodes = new[] { "QUANT", "REASONING", "GEN_INTEL", "GEN_AWARENESS", "BIHAR_GK", "CURRENT_AFFAIRS", "POLITY", "HISTORY", "GEOGRAPHY" }
        },
        new ExamSeedSpec
        {
            Code = "BSSC-SEC-ASST", Slug = "bssc-secretariat-assistant", Name = "BSSC Secretariat Assistant (Sachivalaya Sahayak)", ShortName = "BSSC Secretariat Asst",
            CategorySlug = "bihar-government", OrgCode = "BSSC", Department = "Bihar State Secretariat Service", Designation = "Secretariat Assistant (ASO)",
            Group = GroupClassification.GroupBNonGazetted, IsBihar = true, OfficialWebsite = "https://bssc.bihar.gov.in",
            CycleYear = 2026, Status = ExamStatus.UpcomingNotification, Confidence = DataConfidence.NotAnnounced, VerificationStatus = VerificationStatus.TentativeCalendar, VacancyType = VacancyType.NotAnnounced,
            MinAge = 21, MaxAge = 37, BiharDomicileForReservation = true, AcceptsAnyGraduate = true,
            QualificationSummary = "Bachelor's degree in any stream",
            SyllabusSubjectCodes = new[] { "QUANT", "REASONING", "GEN_AWARENESS", "BIHAR_GK", "CURRENT_AFFAIRS" }
        },
        new ExamSeedSpec
        {
            Code = "BTSC-TECH", Slug = "btsc-technical-recruitment", Name = "BTSC Technical Recruitment (JE / ITI Instructor / IT)", ShortName = "BTSC Technical",
            CategorySlug = "bihar-government", OrgCode = "BTSC", Department = "Bihar Technical Service Commission", Designation = "Junior Engineer / Technical Officer",
            Group = GroupClassification.GroupBNonGazetted, IsBihar = true, OfficialWebsite = "https://btsc.bihar.gov.in",
            CycleYear = 2026, Status = ExamStatus.UpcomingNotification, Confidence = DataConfidence.NotAnnounced, VerificationStatus = VerificationStatus.TentativeCalendar, VacancyType = VacancyType.NotAnnounced,
            MinAge = 18, MaxAge = 37, BiharDomicileForReservation = true, MinQualLevel = "Diploma", RequiresEngineering = true, AcceptsAnyGraduate = false,
            AcceptedBranches = "Civil Engineering,Mechanical Engineering,Electrical Engineering",
            QualificationSummary = "AICTE-approved Diploma / Degree in Civil, Mechanical, or Electrical (trade-specific per advt)",
            SyllabusSubjectCodes = new[] { "GEN_AWARENESS", "BIHAR_GK", "CURRENT_AFFAIRS" }
        },
        new ExamSeedSpec
        {
            Code = "BIHAR-POLICE-SI", Slug = "bihar-police-si", Name = "BPSSC Bihar Police Sub-Inspector (Daroga)", ShortName = "Bihar Police SI",
            CategorySlug = "bihar-government", OrgCode = "BIHAR_POLICE", Department = "Home Department (Police), Govt of Bihar", Designation = "Police Sub-Inspector",
            Group = GroupClassification.GroupBNonGazetted, IsBihar = true, OfficialWebsite = "https://bpssc.bihar.gov.in",
            CycleYear = 2026, Status = ExamStatus.UpcomingNotification, Confidence = DataConfidence.NotAnnounced, VerificationStatus = VerificationStatus.TentativeCalendar, VacancyType = VacancyType.NotAnnounced,
            MinAge = 20, MaxAge = 37, BiharDomicileForReservation = true, AcceptsAnyGraduate = true,
            QualificationSummary = "Bachelor's degree in any discipline + Physical Measurement & PET (Running, High/Long Jump, Shot Put)",
            VerificationNotes = "Requires clearing Physical Efficiency Test (PET) & height/chest standards.",
            SyllabusSubjectCodes = new[] { "HISTORY", "GEOGRAPHY", "POLITY", "ECONOMY", "BIHAR_GK", "CURRENT_AFFAIRS", "QUANT" }
        },
        new ExamSeedSpec
        {
            Code = "BIHAR-POLICE-CONST", Slug = "bihar-police-constable", Name = "CSBC Bihar Police Constable Recruitment", ShortName = "Bihar Police Constable",
            CategorySlug = "bihar-government", OrgCode = "BIHAR_POLICE", Department = "Central Selection Board of Constable, Bihar", Designation = "Constable (Sipahi)",
            Group = GroupClassification.GroupC, IsBihar = true, IsGraduate = false, OfficialWebsite = "https://csbc.bihar.gov.in",
            CycleYear = 2026, Status = ExamStatus.UpcomingNotification, Confidence = DataConfidence.NotAnnounced, VerificationStatus = VerificationStatus.TentativeCalendar, VacancyType = VacancyType.NotAnnounced,
            MinAge = 18, MaxAge = 25, MinQualLevel = "12th", BiharDomicileForReservation = true, AcceptsAnyGraduate = true,
            QualificationSummary = "Intermediate (10+2) pass + PET/PST physical standards",
            SyllabusSubjectCodes = new[] { "BIHAR_GK", "HISTORY", "GEOGRAPHY", "POLITY", "GEN_AWARENESS", "CURRENT_AFFAIRS", "QUANT" }
        },
        new ExamSeedSpec
        {
            Code = "BIHAR-VIDHAN-SABHA", Slug = "bihar-vidhan-sabha", Name = "Bihar Vidhan Sabha Secretariat Recruitment (ASO / DEO / Reporter)", ShortName = "Bihar Vidhan Sabha",
            CategorySlug = "bihar-government", OrgCode = "BIHAR_VS", Department = "Bihar Legislative Assembly Secretariat", Designation = "Assistant Section Officer / Data Entry Operator / Security Guard",
            Group = GroupClassification.GroupBNonGazetted, IsBihar = true, IsIt = true, OfficialWebsite = "https://vidhansabha.bih.nic.in",
            CycleYear = 2026, Status = ExamStatus.UpcomingNotification, Confidence = DataConfidence.NotAnnounced, VerificationStatus = VerificationStatus.TentativeCalendar, VacancyType = VacancyType.NotAnnounced,
            MinAge = 21, MaxAge = 37, BiharDomicileForReservation = true, AcceptsAnyGraduate = true,
            QualificationSummary = "Graduation (for ASO/Reporter); 12th + Computer Typing/DCA (for DEO/Office Attendant)",
            SyllabusSubjectCodes = new[] { "BIHAR_GK", "QUANT", "REASONING", "COMPUTER_AWARENESS", "POLITY", "CURRENT_AFFAIRS" }
        },
        new ExamSeedSpec
        {
            Code = "BIHAR-SECRETARIAT", Slug = "bihar-secretariat", Name = "Bihar State Secretariat & Legislative Council Recruitment", ShortName = "Bihar Secretariat",
            CategorySlug = "bihar-government", OrgCode = "BIHAR_VS", Department = "Bihar Vidhan Parishad / Secretariat", Designation = "ASO / Lower Division Clerk / DEO",
            Group = GroupClassification.GroupBNonGazetted, IsBihar = true, OfficialWebsite = "https://vidhansabha.bih.nic.in",
            CycleYear = 2026, Status = ExamStatus.UpcomingNotification, Confidence = DataConfidence.NotAnnounced, VerificationStatus = VerificationStatus.TentativeCalendar, VacancyType = VacancyType.NotAnnounced,
            MinAge = 21, MaxAge = 37, BiharDomicileForReservation = true, AcceptsAnyGraduate = true,
            QualificationSummary = "Bachelor's degree + Computer efficiency (ADCA/Typing for DEO/LDC)",
            SyllabusSubjectCodes = new[] { "BIHAR_GK", "POLITY", "GEN_AWARENESS", "COMPUTER_AWARENESS", "QUANT", "REASONING" }
        },
        new ExamSeedSpec
        {
            Code = "BIHAR-IT-PROGRAMMER", Slug = "bihar-govt-it-programmer", Name = "Bihar Government IT Manager, Programmer & Beltron DEO/IT Cadre", ShortName = "Bihar IT & Programmer",
            CategorySlug = "bihar-government", OrgCode = "BPSC", Department = "Bihar IT Dept / BELTRON / State e-Governance Societies", Designation = "IT Manager / Programmer / System Analyst",
            Group = GroupClassification.GroupB, IsBihar = true, IsIt = true, OfficialWebsite = "https://state.bihar.gov.in",
            CycleYear = 2026, Status = ExamStatus.UpcomingNotification, Confidence = DataConfidence.NotAnnounced, VerificationStatus = VerificationStatus.TentativeCalendar, VacancyType = VacancyType.NotAnnounced,
            MinAge = 21, MaxAge = 37, BiharDomicileForReservation = true, MinQualLevel = "BTechBE", RequiresEngineering = true, AcceptsAnyGraduate = false,
            AcceptedBranches = "IT,CS,ECE",
            QualificationSummary = "B.Tech/B.E. in IT/CS/ECE or MCA (often with 1-3 yrs software/e-governance experience for Programmer/IT Manager)",
            VerificationNotes = "Check post-specific experience requirement (some Bihar IT Manager/System Analyst contracts require 1-3 years experience).",
            SyllabusSubjectCodes = new[] { "DBMS", "WEB_TECH", "CN", "OS", "CYBERSECURITY", "PROG_LANG", "BIHAR_GK" }
        },

        // ==================== G. TECHNICAL & IT RECRUITMENT (12) ====================
        new ExamSeedSpec
        {
            Code = "GATE-CS", Slug = "gate-cs", Name = "GATE 2027 – Computer Science & Information Technology (CS)", ShortName = "GATE CS",
            CategorySlug = "technical-it", OrgCode = "GATE", Department = "IIT Madras / IISc & 7 IITs (Used by PSUs, DRDO, BARC, M.Tech)", Designation = "GATE Qualified Score (PSU / Scientist / M.Tech Gateway)",
            Group = GroupClassification.GroupA, IsIt = true, Nature = JobNature.FellowshipOrScore, OfficialWebsite = "https://gate2027.iitm.ac.in",
            CycleYear = 2027, NotificationNumber = "GATE 2027 (IIT Madras)", Status = ExamStatus.ExaminationScheduled,
            StatusReason = "Paid Application Data Correction Window open Nov 10–14, 2026; Exam scheduled Feb 2027.",
            NotificationDate = new DateOnly(2026, 8, 25), AppStart = new DateOnly(2026, 9, 2), AppEnd = new DateOnly(2026, 10, 12),
            CorrectionStart = new DateOnly(2026, 11, 10), CorrectionEnd = new DateOnly(2026, 11, 14),
            AdmitCardDate = new DateOnly(2027, 1, 4), PrelimDate = new DateOnly(2027, 2, 6), ResultDate = new DateOnly(2027, 3, 19),
            PdfUrl = "https://gate2027.iitm.ac.in", ApplyUrl = "https://gate2027.iitm.ac.in",
            VacancyType = VacancyType.NotAnnounced, VacancyNotes = "Qualifying national score valid for 3 years across PSU & Scientist recruitments.",
            MinAge = 18, MaxAge = null, SupportRelaxations = false, MinQualLevel = "BTechBE", RequiresEngineering = false, AcceptsAnyGraduate = true,
            FinalYearEligible = true, QualificationSummary = "Studying in 3rd/4th year or completed B.E./B.Tech/B.Sc(4-yr)/M.Sc/MCA (No age limit)",
            FeeSummary = "₹1,800 (Gen/OBC); ₹900 (Female/SC/ST/PwD)",
            SalaryPayLevel = "Gateway to PSU E2/E3 Scales (₹50,000–₹1,80,000) & M.Tech Stipend",
            SelectionProcedure = "Single 3-Hour CBT (General Aptitude 15 marks + CS/IT Core & Engineering Maths 85 marks)",
            NegativeMarking = "1/3rd for 1-mark MCQ; 2/3rd for 2-mark MCQ; No negative for MSQ/NAT",
            SyllabusSubjectCodes = new[] { "DSA", "DBMS", "OS", "CN", "COA", "DIGITAL_LOGIC", "TOC", "PROG_LANG", "QUANT" },
            Patterns = new() { ("GATE CS CBT", "General Aptitude (10 Qs)", 10, 15, 180, "1/3rd or 2/3rd MCQ", false), ("GATE CS CBT", "Engineering Maths & CS Core (55 Qs)", 55, 85, 180, "1/3rd or 2/3rd MCQ", false) }
        },
        new ExamSeedSpec
        {
            Code = "NIC-SCI-B", Slug = "nic-scientist-b", Name = "NIC Scientist 'B' & Scientific/Technical Assistant 'A'", ShortName = "NIC Scientist B",
            CategorySlug = "technical-it", OrgCode = "NIC", Department = "National Informatics Centre (MeitY)", Designation = "Scientist 'B' (Group A Gazetted)",
            Group = GroupClassification.GroupA, IsIt = true, OfficialWebsite = "https://recruitment.nic.in",
            CycleYear = 2026, Status = ExamStatus.UpcomingNotification, Confidence = DataConfidence.NotAnnounced, VerificationStatus = VerificationStatus.TentativeCalendar, VacancyType = VacancyType.NotAnnounced,
            MinAge = 18, MaxAge = 30, MinQualLevel = "BTechBE", RequiresEngineering = true, AcceptsAnyGraduate = false,
            AcceptedBranches = "CS,IT,ECE", MinPctGen = 60m, MinPctRes = 60m,
            QualificationSummary = "Bachelor's in Engineering/Tech in CS/IT/ECE/Electronics OR M.Sc/MCA/DOEACC 'B' level with >=60% marks",
            SalaryPayLevel = "Level 10 (₹56,100–₹1,77,500) + Central Govt Allowances",
            SelectionProcedure = "Written CBT (65% Technical CS + 35% Generic) -> Interview (85:15 ratio for Scientist B)",
            SyllabusSubjectCodes = new[] { "DSA", "DBMS", "OS", "CN", "OOP", "SE", "COA", "DIGITAL_LOGIC", "TOC", "WEB_TECH", "CYBERSECURITY", "QUANT", "REASONING" },
            History = new()
            {
                (2023, HistoricalDataStatus.Verified, 598, 598, new DateOnly(2023, 3, 4), new DateOnly(2023, 12, 12), "NIC Scientist B (71) + STA-A (331) + SA (196)"),
                (2026, HistoricalDataStatus.AwaitingOfficialNotification, null, null, null, null, "Awaiting next MeitY/NIC notification")
            }
        },
        new ExamSeedSpec
        {
            Code = "NIELIT-SCI-B", Slug = "nielit-scientist-b", Name = "NIELIT Scientist 'B' / Scientific Officer Recruitment", ShortName = "NIELIT Scientist B",
            CategorySlug = "technical-it", OrgCode = "NIELIT", Department = "NIELIT (Ministry of Electronics & IT)", Designation = "Scientist 'B'",
            Group = GroupClassification.GroupA, IsIt = true, OfficialWebsite = "https://nielit.gov.in",
            CycleYear = 2026, Status = ExamStatus.UpcomingNotification, Confidence = DataConfidence.NotAnnounced, VerificationStatus = VerificationStatus.TentativeCalendar, VacancyType = VacancyType.NotAnnounced,
            MinAge = 18, MaxAge = 30, MinQualLevel = "BTechBE", RequiresEngineering = true, AcceptsAnyGraduate = false,
            AcceptedBranches = "CS,IT,ECE", MinPctGen = 60m,
            QualificationSummary = "B.E./B.Tech in CS/IT/ECE or M.Sc(CS/IT)/MCA with >=60% marks",
            SalaryPayLevel = "Level 10 (₹56,100–₹1,77,500)",
            SyllabusSubjectCodes = new[] { "DSA", "DBMS", "OS", "CN", "OOP", "SE", "COA", "DIGITAL_LOGIC", "TOC", "CYBERSECURITY", "QUANT", "REASONING" }
        },
        new ExamSeedSpec
        {
            Code = "ISRO-SCI-ENG", Slug = "isro-scientist-engineer-cs", Name = "ISRO ICRB Scientist/Engineer 'SC' (Computer Science)", ShortName = "ISRO Scientist CS",
            CategorySlug = "technical-it", OrgCode = "ISRO", Department = "Department of Space / ISRO Centres", Designation = "Scientist/Engineer 'SC' (Computer Science)",
            Group = GroupClassification.GroupA, IsIt = true, OfficialWebsite = "https://www.isro.gov.in/Careers.html",
            CycleYear = 2026, Status = ExamStatus.UpcomingNotification, Confidence = DataConfidence.NotAnnounced, VerificationStatus = VerificationStatus.TentativeCalendar, VacancyType = VacancyType.NotAnnounced,
            MinAge = 18, MaxAge = 28, MinQualLevel = "BTechBE", RequiresEngineering = true, AcceptsAnyGraduate = false,
            AcceptedBranches = "CS,IT", MinPctGen = 65m, MinPctRes = 65m,
            QualificationSummary = "B.E./B.Tech in Computer Science / Information Technology with First Class (>=65% marks or 6.84/10 CGPA)",
            SalaryPayLevel = "Level 10 (₹56,100 basic + Space Incentives & Central Allowances)",
            SelectionProcedure = "Written Test (Part A Technical + Part B Aptitude) -> Interview (60% qualifying in Interview)",
            SyllabusSubjectCodes = new[] { "DSA", "DBMS", "OS", "CN", "COA", "DIGITAL_LOGIC", "TOC", "PROG_LANG", "SE", "QUANT" }
        },
        new ExamSeedSpec
        {
            Code = "DRDO-SCI-B", Slug = "drdo-rac-scientist-b", Name = "DRDO RAC Scientist 'B' (Computer Science & Engg)", ShortName = "DRDO Scientist B",
            CategorySlug = "technical-it", OrgCode = "DRDO", Department = "Defence Research & Development Organisation", Designation = "Scientist 'B' (Group A Gazetted)",
            Group = GroupClassification.GroupA, IsIt = true, OfficialWebsite = "https://rac.gov.in",
            CycleYear = 2026, Status = ExamStatus.UpcomingNotification, Confidence = DataConfidence.NotAnnounced, VerificationStatus = VerificationStatus.TentativeCalendar, VacancyType = VacancyType.NotAnnounced,
            MinAge = 18, MaxAge = 28, MinQualLevel = "BTechBE", RequiresEngineering = true, AcceptsAnyGraduate = false,
            AcceptedBranches = "CS,IT", MinPctGen = 60m, RequiresGate = true, AcceptedGatePapers = "CS",
            QualificationSummary = "First Class B.E./B.Tech in Computer Science/IT + Valid GATE Score in CS (Check active RAC cutoff/written requirement)",
            VerificationNotes = "DRDO RAC requires First Class degree AND valid GATE CS score (for shortlisting/weightage).",
            SalaryPayLevel = "Level 10 (₹56,100 basic + DRDO Allowances)",
            SyllabusSubjectCodes = new[] { "DSA", "DBMS", "OS", "CN", "COA", "DIGITAL_LOGIC", "TOC", "PROG_LANG" }
        },
        new ExamSeedSpec
        {
            Code = "CDAC-PE", Slug = "cdac-project-engineer", Name = "C-DAC Project Engineer & Project Officer (IT)", ShortName = "C-DAC Project Engineer",
            CategorySlug = "technical-it", OrgCode = "CDAC", Department = "Centre for Development of Advanced Computing", Designation = "Project Engineer (PE)",
            Group = GroupClassification.GroupA, Nature = JobNature.FixedTermRenewable, IsIt = true, OfficialWebsite = "https://careers.cdac.in",
            CycleYear = 2026, Status = ExamStatus.ApplicationsOpen,
            AppStart = new DateOnly(2026, 10, 20), AppEnd = new DateOnly(2026, 11, 20), AgeCutoffDate = new DateOnly(2026, 11, 20),
            Confidence = DataConfidence.OfficialCalendarTentative, VerificationStatus = VerificationStatus.TentativeCalendar, VacancyType = VacancyType.NotAnnounced,
            VacancyNotes = "Rolling centre-wise consolidated openings on careers.cdac.in; check specific centre requisition.",
            MinAge = 18, MaxAge = 37, MinQualLevel = "BTechBE", RequiresEngineering = true, AcceptsAnyGraduate = false,
            AcceptedBranches = "CS,IT,ECE", MinPctGen = 60m, RequiresExp = true, MinExpMonths = 12,
            ExpNotes = "Project Engineer positions typically require 1–4 years of relevant software/IT/cybersecurity experience.",
            QualificationSummary = "First Class B.E./B.Tech in CS/IT/ECE or MCA + relevant post-qualification experience",
            SalaryPayLevel = "Consolidated ₹4.49 LPA – ₹8.5 LPA (based on experience slab)",
            SyllabusSubjectCodes = new[] { "DSA", "OOP", "WEB_TECH", "DBMS", "CYBERSECURITY", "PROG_LANG" }
        },
        new ExamSeedSpec
        {
            Code = "BEL-PE-TE", Slug = "bel-recruitment", Name = "BEL Probationary Engineer / Project Engineer (CS & IT)", ShortName = "BEL Engineer",
            CategorySlug = "technical-it", OrgCode = "BEL", Department = "Bharat Electronics Limited (Navratna PSU)", Designation = "Probationary Engineer / Project Engineer",
            Group = GroupClassification.GroupA, IsIt = true, OfficialWebsite = "https://bel-india.in/careers/",
            CycleYear = 2026, Status = ExamStatus.UpcomingNotification, Confidence = DataConfidence.NotAnnounced, VerificationStatus = VerificationStatus.TentativeCalendar, VacancyType = VacancyType.NotAnnounced,
            MinAge = 18, MaxAge = 28, MinQualLevel = "BTechBE", RequiresEngineering = true, AcceptsAnyGraduate = false,
            AcceptedBranches = "CS,IT,ECE,ME,EE", MinPctGen = 60m, MinPctRes = 50m,
            QualificationSummary = "First Class B.E./B.Tech in CS/IT/ECE/Mech/Elec (Project Engineer requires 2 yrs experience; PE is fresher-eligible)",
            SyllabusSubjectCodes = new[] { "DSA", "CN", "OS", "DBMS", "CYBERSECURITY", "QUANT", "REASONING" }
        },
        new ExamSeedSpec
        {
            Code = "ECIL-TECH", Slug = "ecil-recruitment", Name = "ECIL Graduate Engineer Trainee / Technical Officer", ShortName = "ECIL Technical",
            CategorySlug = "technical-it", OrgCode = "ECIL", Department = "Electronics Corporation of India Limited", Designation = "GET / Technical Officer (CSE / IT / ECE)",
            Group = GroupClassification.GroupA, IsIt = true, OfficialWebsite = "https://www.ecil.co.in/jobs.html",
            CycleYear = 2026, Status = ExamStatus.UpcomingNotification, Confidence = DataConfidence.NotAnnounced, VerificationStatus = VerificationStatus.TentativeCalendar, VacancyType = VacancyType.NotAnnounced,
            MinAge = 18, MaxAge = 30, MinQualLevel = "BTechBE", RequiresEngineering = true, AcceptsAnyGraduate = false,
            AcceptedBranches = "CS,IT,ECE,EE", MinPctGen = 60m,
            QualificationSummary = "First Class Engineering Degree in CSE/IT/ECE/EEE (Technical Officer often requires 1 yr experience)",
            SyllabusSubjectCodes = new[] { "DSA", "DBMS", "OS", "CN", "DIGITAL_LOGIC", "PROG_LANG" }
        },
        new ExamSeedSpec
        {
            Code = "RAILTEL-ENG", Slug = "railtel-recruitment", Name = "RailTel Assistant Manager / Technical Executive (IT/Telecom)", ShortName = "RailTel Tech",
            CategorySlug = "technical-it", OrgCode = "RAILTEL", Department = "RailTel Corporation of India Ltd.", Designation = "Assistant Manager / Executive (IT & Networks)",
            Group = GroupClassification.GroupA, IsIt = true, OfficialWebsite = "https://www.railtel.in/careers.html",
            CycleYear = 2026, Status = ExamStatus.UpcomingNotification, Confidence = DataConfidence.NotAnnounced, VerificationStatus = VerificationStatus.TentativeCalendar, VacancyType = VacancyType.NotAnnounced,
            MinAge = 21, MaxAge = 30, MinQualLevel = "BTechBE", RequiresEngineering = true, AcceptsAnyGraduate = false,
            AcceptedBranches = "CS,IT,ECE", MinPctGen = 60m,
            QualificationSummary = "B.E./B.Tech in CS, IT, or Electronics & Telecommunication with >=60% marks",
            SyllabusSubjectCodes = new[] { "CN", "CYBERSECURITY", "DBMS", "OS", "WEB_TECH", "QUANT" }
        },
        new ExamSeedSpec
        {
            Code = "CDOT-ENG", Slug = "cdot-recruitment", Name = "C-DOT Research Engineer (Telecom & IT R&D)", ShortName = "C-DOT Engineer",
            CategorySlug = "technical-it", OrgCode = "CDOT", Department = "Centre for Development of Telematics (DoT)", Designation = "Research Engineer",
            Group = GroupClassification.GroupA, IsIt = true, OfficialWebsite = "https://www.cdot.in",
            CycleYear = 2026, Status = ExamStatus.UpcomingNotification, Confidence = DataConfidence.NotAnnounced, VerificationStatus = VerificationStatus.TentativeCalendar, VacancyType = VacancyType.NotAnnounced,
            MinAge = 18, MaxAge = 30, MinQualLevel = "BTechBE", RequiresEngineering = true, AcceptsAnyGraduate = false,
            AcceptedBranches = "CS,IT,ECE", MinPctGen = 65m,
            QualificationSummary = "B.E./B.Tech in CS/IT/ECE with >=65% aggregate (GATE/Campus/Written test)",
            SyllabusSubjectCodes = new[] { "CN", "OS", "DSA", "PROG_LANG", "CYBERSECURITY", "COA" }
        },
        new ExamSeedSpec
        {
            Code = "PSU-GATE-CS", Slug = "psu-recruitment-through-gate", Name = "PSU Executive Trainee / Officer through GATE (IOCL, NTPC, ONGC, BHEL, PGCIL, CIL)", ShortName = "PSU via GATE (CS/IT)",
            CategorySlug = "technical-it", OrgCode = "GATE", Department = "Maharatna & Navratna Central PSUs (CS/IT Cadres)", Designation = "Executive Trainee / Officer (Information Systems / IT)",
            Group = GroupClassification.GroupA, IsIt = true, OfficialWebsite = "https://gate2027.iitm.ac.in",
            CycleYear = 2027, Status = ExamStatus.UpcomingNotification, Confidence = DataConfidence.OfficialCalendarTentative, VerificationStatus = VerificationStatus.TentativeCalendar, VacancyType = VacancyType.NotAnnounced,
            MinAge = 18, MaxAge = 28, MinQualLevel = "BTechBE", RequiresEngineering = true, AcceptsAnyGraduate = false,
            AcceptedBranches = "CS,IT", MinPctGen = 65m, MinPctRes = 55m, RequiresGate = true, AcceptedGatePapers = "CS",
            QualificationSummary = "B.E./B.Tech in Computer Science / Information Technology with >=60–65% AND valid GATE CS score",
            VerificationNotes = "Individual PSUs (IOCL, NTPC, ONGC, CIL, GAIL, NPCIL) release separate recruitment notices after GATE results; upper age varies 26–30.",
            SalaryPayLevel = "IDA E2/E3 Scale (₹50,000–₹1,80,000, CTC ₹16–22 LPA)",
            SyllabusSubjectCodes = new[] { "DSA", "DBMS", "OS", "CN", "COA", "DIGITAL_LOGIC", "TOC", "PROG_LANG" }
        },
        new ExamSeedSpec
        {
            Code = "OTHER-GOVT-IT", Slug = "central-state-it-recruitments", Name = "Central & State IT Officer / System Analyst Recruitments (SSC/State/Autonomous)", ShortName = "Central/State IT Cadre",
            CategorySlug = "technical-it", OrgCode = "NIELIT", Department = "Central Ministries, State IT Societies & Autonomous Bodies", Designation = "Systems Analyst / IT Officer / Programmer",
            Group = GroupClassification.GroupB, IsIt = true, OfficialWebsite = "https://nielit.gov.in",
            CycleYear = 2026, Status = ExamStatus.UpcomingNotification, Confidence = DataConfidence.NotAnnounced, VerificationStatus = VerificationStatus.TentativeCalendar, VacancyType = VacancyType.NotAnnounced,
            MinAge = 21, MaxAge = 35, MinQualLevel = "BTechBE", RequiresEngineering = true, AcceptsAnyGraduate = false,
            AcceptedBranches = "CS,IT,ECE", MinPctGen = 60m,
            QualificationSummary = "B.E./B.Tech in CS/IT/ECE or MCA / M.Sc IT",
            SyllabusSubjectCodes = new[] { "DSA", "DBMS", "OS", "CN", "OOP", "WEB_TECH", "CYBERSECURITY", "SE" }
        }
    };
}
