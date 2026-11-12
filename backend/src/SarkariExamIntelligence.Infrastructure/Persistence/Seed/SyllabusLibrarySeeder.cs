using Microsoft.EntityFrameworkCore;
using SarkariExamIntelligence.Domain.Entities;
using SarkariExamIntelligence.Domain.Enums;

namespace SarkariExamIntelligence.Infrastructure.Persistence.Seed;

public static class SyllabusLibrarySeeder
{
    public static async Task<Dictionary<string, SyllabusSubject>> SeedAsync(AppDbContext db, CancellationToken ct = default)
    {
        var existing = await db.SyllabusSubjects.Include(s => s.Topics).ToDictionaryAsync(s => s.Code, ct);
        if (existing.Count >= 26)
            return existing;

        var definitions = new (string Code, string Name, SubjectGroup Group, string Desc, string[] Topics)[]
        {
            // 14 Common Subjects
            ("QUANT", "Quantitative Aptitude", SubjectGroup.Common, "Arithmetic, algebra, geometry, data interpretation, and speed mathematics for competitive exams.",
                new[] { "Number System & Simplification", "Percentage, Profit & Loss", "Ratio, Proportion & Partnership", "Time, Work & Pipes", "Time, Speed, Distance & Trains", "Simple & Compound Interest", "Permutation, Combination & Probability", "Data Interpretation (Tables, Bar, Pie, Caselets)", "Mensuration & Geometry", "Quadratic Equations & Number Series" }),
            ("REASONING", "Logical Reasoning", SubjectGroup.Common, "Analytical puzzles, seating arrangements, syllogisms, and deductive reasoning.",
                new[] { "Syllogism & Inequalities", "Linear & Circular Seating Arrangement", "Floor, Box & Scheduling Puzzles", "Blood Relations & Direction Sense", "Coding-Decoding & Alphanumeric Series", "Input-Output & Data Sufficiency", "Statement-Assumption & Course of Action", "Critical & Verbal Reasoning" }),
            ("GEN_INTEL", "General Intelligence", SubjectGroup.Common, "Non-verbal patterns, analogies, classification, spatial and mirror reasoning.",
                new[] { "Analogies & Classification", "Mirror & Water Images", "Paper Folding & Cutting", "Embedded Figures & Pattern Completion", "Matrix & Venn Diagrams", "Dice, Cubes & Counting Figures" }),
            ("ENGLISH", "English Language", SubjectGroup.Common, "Reading comprehension, grammar, vocabulary, cloze tests, and descriptive essay/precis writing.",
                new[] { "Reading Comprehension & Inference", "Cloze Test & Para Jumbles", "Error Spotting & Sentence Improvement", "Idioms, Phrases & One-Word Substitution", "Active/Passive Voice & Narration", "Descriptive Essay, Precis & Formal Letter Writing" }),
            ("GEN_AWARENESS", "General Awareness", SubjectGroup.Common, "Static GK, awards, national institutions, science in everyday life, and government schemes.",
                new[] { "Union Budget & Central Schemes", "National & International Organizations", "Science & Technology Milestones (ISRO, DRDO)", "Books, Authors, Awards & Sports", "Art, Culture, Classical Dances & Heritage", "Important Days, Summits & Committees" }),
            ("CURRENT_AFFAIRS", "Current Affairs", SubjectGroup.Common, "National, international, economic, Bihar-specific and technology current developments (last 12 months).",
                new[] { "National Policies & Bills", "International Relations & Summits", "RBI, SEBI & Economic Updates", "Science, Space & Digital India Initiatives", "Bihar State Schemes & Current Events", "Indices, Reports & Appointments" }),
            ("HISTORY", "Indian History", SubjectGroup.Common, "Ancient, Medieval, Modern Indian History, Indian National Movement and Bihar's role.",
                new[] { "Indus Valley, Vedic Age, Buddhism & Jainism", "Mauryan & Gupta Empires (Magadha Heritage)", "Delhi Sultanate & Mughal Administration", "1857 Revolt & Socio-Religious Reform Movements", "Indian National Congress & Gandhian Era (1915–1947)", "Constitutional Developments & Post-Independence Consolidation" }),
            ("GEOGRAPHY", "Indian Geography", SubjectGroup.Common, "Physical geography, Indian drainage systems, monsoon, soils, agriculture, and mineral resources.",
                new[] { "Physiographic Divisions of India", "Himalayan & Peninsular River Systems", "Indian Monsoon & Climatic Regions", "Soils, Natural Vegetation & Biosphere Reserves", "Agriculture, Cropping Patterns & Irrigation", "Minerals, Energy Resources & Transport Corridors" }),
            ("POLITY", "Indian Polity", SubjectGroup.Common, "Constitution of India, Parliament, Judiciary, Federalism, Panchayati Raj, and Constitutional Bodies.",
                new[] { "Preamble, Fundamental Rights & DPSPs", "President, Parliament & Legislative Process", "State Executive, Governor & State Legislature", "Supreme Court, High Courts & Judicial Review", "Centre-State Relations & Emergency Provisions", "Panchayati Raj (73rd/74th Amendment) & Constitutional Bodies" }),
            ("ECONOMY", "Indian Economy", SubjectGroup.Common, "Macroeconomics, NITI Aayog, Fiscal Policy, Monetary Policy, Banking Reforms, and External Sector.",
                new[] { "National Income (GDP/GNP) & Inclusive Growth", "Fiscal Policy, FRBM & Union Budget Structure", "Inflation, Monetary Policy & RBI Tools", "Agriculture, MSP, Industry & Infrastructure", "Balance of Payments, Forex & WTO", "Five-Year Plans, NITI Aayog & Poverty Alleviation" }),
            ("COMPUTER_AWARENESS", "Computer Awareness", SubjectGroup.Common, "Computer fundamentals, MS Office, internet protocols, DBMS basics, and cyber hygiene for clerical/PO exams.",
                new[] { "Computer Architecture & Memory Hierarchy", "Operating System Basics & Shortcuts", "MS Word, Excel & PowerPoint Functions", "Networking Devices, OSI & Internet Protocols", "Number Systems & Logic Gates Basics", "Malware, Phishing & Cyber Security Hygiene" }),
            ("BANKING_AWARENESS", "Banking Awareness", SubjectGroup.Common, "Indian banking structure, RBI functions, NPAs, Basel norms, digital payments, and Negotiable Instruments.",
                new[] { "History of Indian Banking & RBI Functions", "Monetary Policy Instruments (Repo, CRR, SLR)", "Priority Sector Lending & Financial Inclusion", "NPAs, SARFAESI, IBC & Basel III Norms", "UPI, NEFT, RTGS, IMPS & Digital Banking", "Negotiable Instruments Act & Banking Ombudsman" }),
            ("FINANCIAL_AWARENESS", "Financial Awareness", SubjectGroup.Common, "Capital markets, SEBI regulations, insurance principles, pension funds, and corporate finance.",
                new[] { "Money Market & Capital Market Instruments", "SEBI Regulations, IPOs, Mutual Funds & Derivatives", "NABARD, Rural Credit & Cooperative Banking", "Insurance Principles, IRDAI & Risk Underwriting", "PFRDA, NPS & IFSCA GIFT City Framework", "Financial Statements, Ratios & Costing Basics" }),
            ("BIHAR_GK", "Bihar General Knowledge", SubjectGroup.Common, "Bihar history, geography, river systems, economy, caste/socio-economic surveys, budget and governance.",
                new[] { "Ancient Magadha, Maurya, Gupta & Nalanda/Vikramshila", "Bihar in 1857 (Kunwar Singh) & Champaran Satyagraha", "Physiography, Ganga/Kosi/Gandak Rivers & Flood Plains", "Bihar Economic Survey & State Budget Highlights", "Agriculture Road Map, Industries & Saat Nishchay", "Bihar Administrative Divisions, Panchayati Raj & Art/Culture" }),

            // 12 Technical Subjects
            ("DSA", "Data Structures and Algorithms", SubjectGroup.Technical, "Arrays, trees, graphs, dynamic programming, greedy algorithms, and asymptotic complexity.",
                new[] { "Asymptotic Analysis & Recurrence Relations", "Arrays, Linked Lists, Stacks & Queues", "Trees, BST, AVL, Heaps & Tries", "Hashing & Collision Resolution", "Graph Algorithms (BFS, DFS, Dijkstra, MST)", "Sorting, Searching, Divide & Conquer", "Greedy Method & Dynamic Programming", "NP-Completeness Basics" }),
            ("DBMS", "Database Management Systems", SubjectGroup.Technical, "ER modeling, relational algebra, SQL, normalization, transactions, indexing and NoSQL.",
                new[] { "ER Model & Relational Algebra / Calculus", "SQL Queries, Joins, Views & Triggers", "Functional Dependencies & Normalization (1NF–BCNF)", "Transactions, ACID & Concurrency Control (2PL/Timestamp)", "Recovery Techniques & Write-Ahead Logging", "B-Trees, B+ Trees & File Organization" }),
            ("OS", "Operating Systems", SubjectGroup.Technical, "Processes, threads, CPU scheduling, synchronization, deadlocks, memory management and file systems.",
                new[] { "Process Lifecycle & CPU Scheduling Algorithms", "Inter-Process Communication & Threads", "Process Synchronization, Semaphores & Monitors", "Deadlocks (Detection, Prevention, Banker's Algorithm)", "Virtual Memory, Paging, Segmentation & Page Replacement", "Disk Scheduling & File System Internals" }),
            ("CN", "Computer Networks", SubjectGroup.Technical, "OSI/TCP-IP layers, routing, congestion control, IPv4/IPv6, DNS, HTTP and network security.",
                new[] { "OSI & TCP/IP Reference Models", "Data Link Layer, Framing, Error Detection & MAC (CSMA/CD)", "IPv4/IPv6 Addressing, Subnetting & CIDR", "Routing Algorithms (Distance Vector, Link State, BGP)", "TCP/UDP Flow & Congestion Control", "Application Protocols (DNS, HTTP/HTTPS, SMTP, FTP, DHCP)" }),
            ("OOP", "Object-Oriented Programming", SubjectGroup.Technical, "Encapsulation, inheritance, polymorphism, SOLID principles, design patterns and UML.",
                new[] { "Classes, Objects, Encapsulation & Abstraction", "Inheritance, Polymorphism & Virtual Dispatch", "Exception Handling, Generics & Memory Management", "SOLID Design Principles & UML Diagrams", "Core GoF Design Patterns (Singleton, Factory, Strategy, Observer)" }),
            ("SE", "Software Engineering", SubjectGroup.Technical, "SDLC models, Agile/Scrum, requirements engineering, software testing, DevOps and quality metrics.",
                new[] { "SDLC Models (Waterfall, Iterative, Agile, Scrum)", "Requirements Elicitation, SRS & Feasibility", "Software Architecture & Modular Coupling/Cohesion", "Testing Strategies (Unit, Integration, Black/White Box)", "COCOMO Cost Estimation & Cyclomatic Complexity", "CI/CD, Version Control & Software Maintenance" }),
            ("COA", "Computer Architecture", SubjectGroup.Technical, "Instruction sets, pipelining, ALU design, cache memory organization and I/O interfaces.",
                new[] { "Machine Instructions & Addressing Modes", "ALU Design & IEEE 754 Floating-Point Arithmetic", "Control Unit (Hardwired & Microprogrammed)", "Instruction Pipelining & Data/Control Hazards", "Cache Memory Mapping, Replacement & Coherence", "DMA, Interrupts & I/O Organization" }),
            ("CYBERSECURITY", "Cybersecurity", SubjectGroup.Technical, "Cryptography, PKI, authentication, OWASP vulnerabilities, firewalls, IDS/IPS and IT Act compliance.",
                new[] { "Symmetric & Asymmetric Cryptography (AES, RSA, ECC)", "Hashing, Digital Signatures & PKI Certificates", "Authentication Protocols (Kerberos, OAuth2, JWT, TLS)", "Web Security & OWASP Top 10 (SQLi, XSS, CSRF)", "Network Defense (Firewalls, VPN, IDS/IPS)", "Information Technology Act 2000 & CERT-In Guidelines" }),
            ("WEB_TECH", "Web Technologies", SubjectGroup.Technical, "HTML5, CSS3, JavaScript/TypeScript, REST APIs, HTTP semantics, SPA architecture and XML/JSON.",
                new[] { "HTML5 Semantic Markup & Accessibility", "CSS3 Flexbox, Grid & Responsive Design", "JavaScript ES6+, DOM & Asynchronous Promises", "RESTful Web Services, HTTP Methods & Status Codes", "JSON, XML, XSD & Web Storage/CORS", "Modern SPA & Server-Side Rendering Concepts" }),
            ("PROG_LANG", "Programming Languages", SubjectGroup.Technical, "C, C++, Java, Python, C#, pointers, recursion, compilation pipeline and runtime environments.",
                new[] { "C Programming, Pointers, Structs & Dynamic Memory", "C++ STL, Templates & References", "Java JVM, Collections & Multithreading", "Python Data Model, Comprehensions & Scripting", "Lexical Analysis, Parsing & Runtime Activation Records" }),
            ("DIGITAL_LOGIC", "Digital Logic", SubjectGroup.Technical, "Boolean algebra, K-maps, combinational/sequential circuits, flip-flops and counters.",
                new[] { "Number Systems & Signed Representations", "Boolean Algebra & Karnaugh Map Minimization", "Combinational Circuits (Mux, Decoder, Adder)", "Sequential Circuits (SR, JK, D, T Flip-Flops)", "Registers, Counters & Finite State Machines" }),
            ("TOC", "Theory of Computation", SubjectGroup.Technical, "Finite automata, regular languages, context-free grammars, Turing machines and decidability.",
                new[] { "DFA, NFA & Regular Expressions", "Pumping Lemma & Closure Properties of Regular Languages", "Context-Free Grammars & Pushdown Automata", "Turing Machines & Church-Turing Thesis", "Decidability, Halting Problem & Reducibility" })
        };

        var order = 1;
        foreach (var (code, name, grp, desc, topics) in definitions)
        {
            if (existing.ContainsKey(code)) continue;

            var subject = new SyllabusSubject
            {
                Code = code,
                Name = name,
                SubjectGroup = grp,
                Description = desc,
                DisplayOrder = order++
            };

            var tOrder = 1;
            foreach (var tName in topics)
            {
                subject.Topics.Add(new SyllabusTopic
                {
                    Name = tName,
                    Description = $"Core competitive exam syllabus module covering {tName} under {name}.",
                    DifficultyLevel = tOrder <= 3 ? "Foundation" : tOrder <= 7 ? "Moderate" : "Advanced",
                    EstimatedHours = grp == SubjectGroup.Technical ? 6 : 4,
                    DisplayOrder = tOrder++
                });
            }

            db.SyllabusSubjects.Add(subject);
            existing[code] = subject;
        }

        await db.SaveChangesAsync(ct);
        return existing;
    }
}
