using Microsoft.EntityFrameworkCore;
using SarkariExamIntelligence.Domain.Entities;
using SarkariExamIntelligence.Domain.Enums;

namespace SarkariExamIntelligence.Infrastructure.Persistence.Seed;

public record OrgSourceSeedResult(
    Dictionary<string, Category> Categories,
    Dictionary<string, ExamOrganization> Organizations,
    Dictionary<string, OfficialSource> Sources);

public static class OfficialSourceSeeder
{
    public static async Task<OrgSourceSeedResult> SeedAsync(AppDbContext db, CancellationToken ct = default)
    {
        var cats = await db.Categories.ToDictionaryAsync(c => c.Slug, ct);
        if (cats.Count == 0)
        {
            var catDefs = new (string Code, string Slug, string Name, string Desc, int Order)[]
            {
                ("UPSC", "upsc", "UPSC Examinations", "Union Public Service Commission — All India Services, Central Group A/B Civil, Defence, Forest & EPFO recruitments.", 1),
                ("SSC", "ssc", "SSC Examinations", "Staff Selection Commission — Central Government Group B Non-Gazetted & Group C ministerial, uniformed & selection posts.", 2),
                ("BANKING", "banking", "Banking & RBI", "IBPS, SBI and Reserve Bank of India officer, specialist IT, regional rural bank & clerical examinations.", 3),
                ("REGULATORS", "financial-regulators-insurance", "Financial Regulators & Insurance", "SEBI, NABARD, PFRDA, IRDAI, IFSCA, SIDBI, LIC, NIACL & GIC Re Grade A/B & AO recruitments.", 4),
                ("RAILWAYS", "railways", "Indian Railways (RRB)", "Railway Recruitment Boards — NTPC, Junior Engineer, ALP, Technician & Level-1 Group D exams.", 5),
                ("BIHAR", "bihar-government", "Bihar Government", "BPSC, BSSC, BTSC, Bihar Police (BPSSC/CSBC), Bihar Vidhan Sabha & State IT/Secretariat recruitments.", 6),
                ("TECH_IT", "technical-it", "Technical & IT Recruitment", "GATE CS, NIC, NIELIT, ISRO, DRDO RAC, C-DAC, BEL, ECIL, RailTel, C-DOT & GATE PSUs.", 7)
            };

            foreach (var c in catDefs)
            {
                var cat = new Category { Code = c.Code, Slug = c.Slug, Name = c.Name, Description = c.Desc, DisplayOrder = c.Order };
                db.Categories.Add(cat);
                cats[c.Slug] = cat;
            }
            await db.SaveChangesAsync(ct);
        }

        var orgs = await db.ExamOrganizations.ToDictionaryAsync(o => o.ShortCode, ct);
        var sources = await db.OfficialSources.Include(s => s.Organization).ToDictionaryAsync(s => s.Organization.ShortCode, ct);

        if (orgs.Count == 0)
        {
            var orgDefs = new (string Code, string Name, string Type, string? State, string Web, string Portal, string CalendarUrl, string Desc)[]
            {
                ("UPSC", "Union Public Service Commission", "Central", null, "https://upsc.gov.in", "https://upsconline.nic.in", "https://upsc.gov.in/examinations/exam-calendar", "Constitutional body recruiting for All India & Central Services."),
                ("SSC", "Staff Selection Commission", "Central", null, "https://ssc.gov.in", "https://ssc.gov.in", "https://ssc.gov.in/for-candidates/tentative-calendar", "Central agency recruiting Group B (Non-Gazetted) & Group C posts."),
                ("IBPS", "Institute of Banking Personnel Selection", "Autonomous", null, "https://www.ibps.in", "https://www.ibps.in", "https://www.ibps.in/index.php/tentative-calendar/", "Centralized recruitment agency for Public Sector Banks & RRBs."),
                ("SBI", "State Bank of India Careers", "PSU", null, "https://sbi.co.in", "https://sbi.co.in/web/careers", "https://sbi.co.in/web/careers/current-openings", "Central Recruitment & Promotion Department, SBI."),
                ("RBI", "Reserve Bank of India", "Regulatory", null, "https://www.rbi.org.in", "https://opportunities.rbi.org.in", "https://opportunities.rbi.org.in/Scripts/vacancies.aspx", "Reserve Bank of India Services Board."),
                ("SEBI", "Securities and Exchange Board of India", "Regulatory", null, "https://www.sebi.gov.in", "https://www.sebi.gov.in/sebiweb/other/career.jsp", "https://www.sebi.gov.in/sebiweb/other/career.jsp", "Capital markets regulator recruiting Grade A officers across streams."),
                ("NABARD", "National Bank for Agriculture and Rural Development", "Regulatory", null, "https://www.nabard.org", "https://www.nabard.org/careers-notices1.aspx", "https://www.nabard.org/careers-notices1.aspx", "Apex rural development & agriculture refinancing institution."),
                ("PFRDA", "Pension Fund Regulatory and Development Authority", "Regulatory", null, "https://www.pfrda.org.in", "https://www.pfrda.org.in/index1.cshtml?lsid=126", "https://www.pfrda.org.in/index1.cshtml?lsid=126", "Statutory pension sector regulator in India."),
                ("IRDAI", "Insurance Regulatory and Development Authority of India", "Regulatory", null, "https://irdai.gov.in", "https://irdai.gov.in/careers", "https://irdai.gov.in/careers", "Statutory insurance regulator in India."),
                ("IFSCA", "International Financial Services Centres Authority", "Regulatory", null, "https://ifsca.gov.in", "https://ifsca.gov.in/Career", "https://ifsca.gov.in/Career", "Unified regulator at GIFT City IFSC."),
                ("SIDBI", "Small Industries Development Bank of India", "Regulatory", null, "https://www.sidbi.in", "https://www.sidbi.in/en/careers", "https://www.sidbi.in/en/careers", "Principal development financial institution for MSMEs."),
                ("LIC", "Life Insurance Corporation of India", "PSU", null, "https://licindia.in", "https://licindia.in/web/guest/careers", "https://licindia.in/web/guest/careers", "India's largest public sector life insurer."),
                ("NIACL", "The New India Assurance Co. Ltd.", "PSU", null, "https://www.newindia.co.in", "https://www.newindia.co.in/portal/readMore/Recruitment", "https://www.newindia.co.in/portal/readMore/Recruitment", "Public sector general insurance company."),
                ("GIC", "General Insurance Corporation of India", "PSU", null, "https://www.gicre.in", "https://www.gicre.in/en/career-en", "https://www.gicre.in/en/career-en", "National reinsurer of India."),
                ("RRB", "Railway Recruitment Boards", "Central", null, "https://indianrailways.gov.in", "https://www.rrbapply.gov.in", "https://www.rrbapply.gov.in", "Ministry of Railways recruitment boards across 21 zones."),
                ("BPSC", "Bihar Public Service Commission", "State", "Bihar", "https://bpsc.bihar.gov.in", "https://onlinebpsc.bihar.gov.in", "https://bpsc.bihar.gov.in", "State constitutional commission for Bihar civil & technical services."),
                ("BSSC", "Bihar Staff Selection Commission", "State", "Bihar", "https://bssc.bihar.gov.in", "https://bssc.bihar.gov.in", "https://bssc.bihar.gov.in", "Bihar state agency for Group B/C non-gazetted & graduate posts."),
                ("BTSC", "Bihar Technical Service Commission", "State", "Bihar", "https://btsc.bihar.gov.in", "https://btsc.bihar.gov.in", "https://btsc.bihar.gov.in", "Bihar state agency for technical, medical & engineering cadres."),
                ("BIHAR_POLICE", "Bihar Police (BPSSC & CSBC)", "State", "Bihar", "https://bpssc.bihar.gov.in", "https://bpssc.bihar.gov.in", "https://csbc.bihar.gov.in", "BPSSC (SI) & CSBC (Constable) recruitment boards of Bihar Police."),
                ("BIHAR_VS", "Bihar Vidhan Sabha & Secretariat", "State", "Bihar", "https://vidhansabha.bih.nic.in", "https://vidhansabha.bih.nic.in", "https://state.bihar.gov.in", "Bihar Legislative Assembly & State Secretariat recruitment portals."),
                ("NIC", "National Informatics Centre", "Central", null, "https://www.nic.in", "https://recruitment.nic.in", "https://www.calicut.nielit.in/nic/", "MeitY premier e-governance IT organization."),
                ("NIELIT", "National Institute of Electronics & IT", "Autonomous", null, "https://nielit.gov.in", "https://recruitment-delhi.nielit.gov.in", "https://nielit.gov.in/recruitments", "MeitY autonomous scientific society conducting Scientist-B & technical drives."),
                ("ISRO", "Indian Space Research Organisation (ICRB)", "Central", null, "https://www.isro.gov.in", "https://www.isro.gov.in/Careers.html", "https://www.isro.gov.in/Careers.html", "Department of Space ICRB centralized Scientist/Engineer recruitment."),
                ("DRDO", "DRDO Recruitment & Assessment Centre (RAC)", "Central", null, "https://rac.gov.in", "https://rac.gov.in", "https://rac.gov.in", "Defence Research & Development Organisation Scientist-B recruitment."),
                ("CDAC", "Centre for Development of Advanced Computing", "Autonomous", null, "https://www.cdac.in", "https://careers.cdac.in", "https://www.cdac.in/index.aspx?id=ca_current_job_opportunities", "MeitY R&D institution recruiting Project Engineers & Technical Officers."),
                ("BEL", "Bharat Electronics Limited", "PSU", null, "https://bel-india.in", "https://bel-india.in/careers/", "https://bel-india.in/careers/", "Navratna Defence PSU recruiting Probationary & Project Engineers."),
                ("ECIL", "Electronics Corporation of India Limited", "PSU", null, "https://www.ecil.co.in", "https://www.ecil.co.in/jobs.html", "https://www.ecil.co.in/jobs.html", "Department of Atomic Energy PSU for Technical Officers & GETs."),
                ("RAILTEL", "RailTel Corporation of India Ltd.", "PSU", null, "https://www.railtel.in", "https://www.railtel.in/careers.html", "https://www.railtel.in/careers.html", "Navratna telecom & IT PSU under Ministry of Railways."),
                ("CDOT", "Centre for Development of Telematics", "Autonomous", null, "https://www.cdot.in", "https://www.cdot.in/cdotweb/web/careers.php", "https://www.cdot.in/cdotweb/web/careers.php", "Department of Telecommunications R&D centre."),
                ("GATE", "GATE Organizing Institute (IIT Madras 2027)", "Central", null, "https://gate2027.iitm.ac.in", "https://gate2027.iitm.ac.in", "https://gate2027.iitm.ac.in", "National GATE CS/IT qualifying exam used by PSUs, DRDO, BARC & M.Tech.")
            };

            foreach (var o in orgDefs)
            {
                var org = new ExamOrganization
                {
                    ShortCode = o.Code,
                    Name = o.Name,
                    OrganizationType = o.Type,
                    StateName = o.State,
                    OfficialWebsiteUrl = o.Web,
                    RecruitmentPortalUrl = o.Portal,
                    Description = o.Desc
                };
                db.ExamOrganizations.Add(org);
                orgs[o.Code] = org;
            }
            await db.SaveChangesAsync(ct);

            var now = DateTime.UtcNow;
            foreach (var o in orgDefs)
            {
                var src = new OfficialSource
                {
                    OrganizationId = orgs[o.Code].Id,
                    SourceName = $"{o.Name} Official Portal",
                    OfficialSourceUrl = o.Portal,
                    CalendarOrFeedUrl = o.CalendarUrl,
                    DateLastCheckedUtc = now,
                    DateLastUpdatedUtc = now,
                    VerificationStatus = VerificationStatus.VerifiedOfficial,
                    DataConfidence = DataConfidence.ConfirmedOfficial,
                    RateLimitSeconds = 120,
                    IsActive = true,
                    Notes = "Primary official government recruitment source registered in Sarkari Exam Intelligence."
                };
                db.OfficialSources.Add(src);
                sources[o.Code] = src;
            }
            await db.SaveChangesAsync(ct);
        }

        return new OrgSourceSeedResult(cats, orgs, sources);
    }
}
