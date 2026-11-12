using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SarkariExamIntelligence.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EntityName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EntityId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Action = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    OldValuesJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NewValuesJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TimestampUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(600)", maxLength: 600, nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ExamOrganizations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ShortCode = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    OrganizationType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    StateName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    OfficialWebsiteUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    RecruitmentPortalUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamOrganizations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NotificationPreferences",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EnableInApp = table.Column<bool>(type: "bit", nullable: false),
                    EnableEmail = table.Column<bool>(type: "bit", nullable: false),
                    AlertOnNewNotification = table.Column<bool>(type: "bit", nullable: false),
                    AlertOnApplicationOpen = table.Column<bool>(type: "bit", nullable: false),
                    Alert30DaysBefore = table.Column<bool>(type: "bit", nullable: false),
                    Alert7DaysBefore = table.Column<bool>(type: "bit", nullable: false),
                    Alert3DaysBefore = table.Column<bool>(type: "bit", nullable: false),
                    Alert1DayBefore = table.Column<bool>(type: "bit", nullable: false),
                    AlertOnExamDateChange = table.Column<bool>(type: "bit", nullable: false),
                    AlertOnAdmitCard = table.Column<bool>(type: "bit", nullable: false),
                    AlertOnResult = table.Column<bool>(type: "bit", nullable: false),
                    OverrideEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationPreferences", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SyllabusSubjects",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    SubjectGroup = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(800)", maxLength: 800, nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SyllabusSubjects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserProfiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DateOfBirth = table.Column<DateOnly>(type: "date", nullable: true),
                    Gender = table.Column<int>(type: "int", nullable: true),
                    Category = table.Column<int>(type: "int", nullable: false),
                    IsPwBD = table.Column<bool>(type: "bit", nullable: false),
                    IsExServiceman = table.Column<bool>(type: "bit", nullable: false),
                    DomicileState = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    GraduationQualification = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    EngineeringBranch = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    University = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    GraduationPercentage = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: true),
                    GraduationCgpa = table.Column<decimal>(type: "decimal(4,2)", precision: 4, scale: 2, nullable: true),
                    PassingYear = table.Column<int>(type: "int", nullable: true),
                    WorkExperienceMonths = table.Column<int>(type: "int", nullable: false),
                    IsWorkingProfessional = table.Column<bool>(type: "bit", nullable: false),
                    CurrentOrganizationType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    AdditionalQualifications = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    HasValidGateScore = table.Column<bool>(type: "bit", nullable: false),
                    GateSubject = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    GateScore = table.Column<int>(type: "int", nullable: true),
                    GateYear = table.Column<int>(type: "int", nullable: true),
                    HasBiharDomicileCertificate = table.Column<bool>(type: "bit", nullable: false),
                    HasValidNclOrEwsCertificate = table.Column<bool>(type: "bit", nullable: false),
                    Nationality = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProfiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OfficialSources",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SourceName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    OfficialSourceUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CalendarOrFeedUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    LastNotificationPdfUrl = table.Column<string>(type: "nvarchar(600)", maxLength: 600, nullable: true),
                    DateLastCheckedUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateLastUpdatedUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SourcePublicationDate = table.Column<DateOnly>(type: "date", nullable: true),
                    VerificationStatus = table.Column<int>(type: "int", nullable: false),
                    DataConfidence = table.Column<int>(type: "int", nullable: false),
                    LastHttpStatusCode = table.Column<int>(type: "int", nullable: true),
                    LastContentHash = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    RateLimitSeconds = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OfficialSources", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OfficialSources_ExamOrganizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "ExamOrganizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SyllabusTopics",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SyllabusSubjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(800)", maxLength: 800, nullable: false),
                    DifficultyLevel = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    EstimatedHours = table.Column<int>(type: "int", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SyllabusTopics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SyllabusTopics_SyllabusSubjects_SyllabusSubjectId",
                        column: x => x.SyllabusSubjectId,
                        principalTable: "SyllabusSubjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Exams",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OfficialSourceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    ShortName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Department = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    JobDesignation = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    GroupClassification = table.Column<int>(type: "int", nullable: false),
                    JobNature = table.Column<int>(type: "int", nullable: false),
                    OfficialRecruitmentWebsite = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    IsBiharSpecific = table.Column<bool>(type: "bit", nullable: false),
                    IsItSpecific = table.Column<bool>(type: "bit", nullable: false),
                    IsGraduateLevel = table.Column<bool>(type: "bit", nullable: false),
                    VerificationStatus = table.Column<int>(type: "int", nullable: false),
                    DataConfidence = table.Column<int>(type: "int", nullable: false),
                    IsApprovedForPublish = table.Column<bool>(type: "bit", nullable: false),
                    Summary = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    DefaultSalaryPayLevel = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    DefaultSelectionProcedure = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    DefaultExamMode = table.Column<int>(type: "int", nullable: false),
                    DefaultNegativeMarking = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Exams_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Exams_ExamOrganizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "ExamOrganizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Exams_OfficialSources_OfficialSourceId",
                        column: x => x.OfficialSourceId,
                        principalTable: "OfficialSources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AgeRelaxations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExamId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Category = table.Column<int>(type: "int", nullable: false),
                    SubCategory = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    RelaxationYears = table.Column<int>(type: "int", nullable: false),
                    MaxUpperAgeOverride = table.Column<int>(type: "int", nullable: true),
                    RequiresDomicileForStateQuota = table.Column<bool>(type: "bit", nullable: false),
                    IsSupportedByNotification = table.Column<bool>(type: "bit", nullable: false),
                    Conditions = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    SourceReference = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgeRelaxations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AgeRelaxations_Exams_ExamId",
                        column: x => x.ExamId,
                        principalTable: "Exams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EducationalQualifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExamId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DegreeLevel = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    DegreeName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    AcceptedBranchesCsv = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    MinimumPercentage = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: true),
                    IsMandatory = table.Column<bool>(type: "bit", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EducationalQualifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EducationalQualifications_Exams_ExamId",
                        column: x => x.ExamId,
                        principalTable: "Exams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExamNotifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExamId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CycleYear = table.Column<int>(type: "int", nullable: false),
                    NotificationNumber = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Title = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    StatusReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsCurrentCycle = table.Column<bool>(type: "bit", nullable: false),
                    NotificationDate = table.Column<DateOnly>(type: "date", nullable: true),
                    ExpectedNotificationDate = table.Column<DateOnly>(type: "date", nullable: true),
                    IsNotificationDateExpected = table.Column<bool>(type: "bit", nullable: false),
                    ApplicationStartDate = table.Column<DateOnly>(type: "date", nullable: true),
                    ExpectedApplicationStartDate = table.Column<DateOnly>(type: "date", nullable: true),
                    IsApplicationStartExpected = table.Column<bool>(type: "bit", nullable: false),
                    ApplicationEndDate = table.Column<DateOnly>(type: "date", nullable: true),
                    ExpectedApplicationEndDate = table.Column<DateOnly>(type: "date", nullable: true),
                    IsApplicationEndExpected = table.Column<bool>(type: "bit", nullable: false),
                    CorrectionWindowStart = table.Column<DateOnly>(type: "date", nullable: true),
                    CorrectionWindowEnd = table.Column<DateOnly>(type: "date", nullable: true),
                    AdmitCardDate = table.Column<DateOnly>(type: "date", nullable: true),
                    ExpectedAdmitCardDate = table.Column<DateOnly>(type: "date", nullable: true),
                    IsAdmitCardExpected = table.Column<bool>(type: "bit", nullable: false),
                    ExamDate = table.Column<DateOnly>(type: "date", nullable: true),
                    ExpectedExamDate = table.Column<DateOnly>(type: "date", nullable: true),
                    IsExamDateExpected = table.Column<bool>(type: "bit", nullable: false),
                    PrelimExamDate = table.Column<DateOnly>(type: "date", nullable: true),
                    ExpectedPrelimExamDate = table.Column<DateOnly>(type: "date", nullable: true),
                    IsPrelimDateExpected = table.Column<bool>(type: "bit", nullable: false),
                    MainsExamDate = table.Column<DateOnly>(type: "date", nullable: true),
                    ExpectedMainsExamDate = table.Column<DateOnly>(type: "date", nullable: true),
                    IsMainsDateExpected = table.Column<bool>(type: "bit", nullable: false),
                    InterviewDate = table.Column<DateOnly>(type: "date", nullable: true),
                    ExpectedInterviewDate = table.Column<DateOnly>(type: "date", nullable: true),
                    IsInterviewDateExpected = table.Column<bool>(type: "bit", nullable: false),
                    ResultDate = table.Column<DateOnly>(type: "date", nullable: true),
                    ExpectedResultDate = table.Column<DateOnly>(type: "date", nullable: true),
                    IsResultDateExpected = table.Column<bool>(type: "bit", nullable: false),
                    AgeCutoffDate = table.Column<DateOnly>(type: "date", nullable: true),
                    AgeCutoffReferenceText = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    ApplicationFeeSummary = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    SalaryPayLevel = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    SelectionProcedure = table.Column<string>(type: "nvarchar(1200)", maxLength: 1200, nullable: false),
                    ExamMode = table.Column<int>(type: "int", nullable: false),
                    NegativeMarkingDetails = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: false),
                    OfficialNotificationPdfUrl = table.Column<string>(type: "nvarchar(600)", maxLength: 600, nullable: true),
                    OfficialApplicationUrl = table.Column<string>(type: "nvarchar(600)", maxLength: 600, nullable: true),
                    SourcePublicationDate = table.Column<DateOnly>(type: "date", nullable: true),
                    LastVerifiedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    VerificationStatus = table.Column<int>(type: "int", nullable: false),
                    DataConfidence = table.Column<int>(type: "int", nullable: false),
                    IsPendingApproval = table.Column<bool>(type: "bit", nullable: false),
                    ContentHash = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    LastChangeSummary = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    LastChangedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamNotifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExamNotifications_Exams_ExamId",
                        column: x => x.ExamId,
                        principalTable: "Exams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExamPatterns",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExamId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StageName = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    SectionName = table.Column<string>(type: "nvarchar(180)", maxLength: 180, nullable: false),
                    QuestionsCount = table.Column<int>(type: "int", nullable: true),
                    MaxMarks = table.Column<decimal>(type: "decimal(7,2)", precision: 7, scale: 2, nullable: true),
                    DurationMinutes = table.Column<int>(type: "int", nullable: true),
                    NegativeMarkingDetails = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    IsQualifyingOnly = table.Column<bool>(type: "bit", nullable: false),
                    OrderIndex = table.Column<int>(type: "int", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamPatterns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExamPatterns_Exams_ExamId",
                        column: x => x.ExamId,
                        principalTable: "Exams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExamSyllabi",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExamId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SyllabusSubjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StageName = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    WeightageNotes = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: true),
                    ExamSpecificTopicsNotes = table.Column<string>(type: "nvarchar(1500)", maxLength: 1500, nullable: true),
                    OfficialSyllabusReferenceUrl = table.Column<string>(type: "nvarchar(600)", maxLength: 600, nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamSyllabi", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExamSyllabi_Exams_ExamId",
                        column: x => x.ExamId,
                        principalTable: "Exams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExamSyllabi_SyllabusSubjects_SyllabusSubjectId",
                        column: x => x.SyllabusSubjectId,
                        principalTable: "SyllabusSubjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HistoricalRecruitments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExamId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CycleYear = table.Column<int>(type: "int", nullable: false),
                    DataStatus = table.Column<int>(type: "int", nullable: false),
                    NotificationDate = table.Column<DateOnly>(type: "date", nullable: true),
                    ApplicationStartDate = table.Column<DateOnly>(type: "date", nullable: true),
                    ApplicationEndDate = table.Column<DateOnly>(type: "date", nullable: true),
                    PrelimExamDate = table.Column<DateOnly>(type: "date", nullable: true),
                    MainsExamDate = table.Column<DateOnly>(type: "date", nullable: true),
                    ResultDate = table.Column<DateOnly>(type: "date", nullable: true),
                    TotalVacancies = table.Column<int>(type: "int", nullable: true),
                    ItVacancies = table.Column<int>(type: "int", nullable: true),
                    GeneralVacancies = table.Column<int>(type: "int", nullable: true),
                    SelectionPatternSummary = table.Column<string>(type: "nvarchar(800)", maxLength: 800, nullable: true),
                    OfficialSourceUrl = table.Column<string>(type: "nvarchar(600)", maxLength: 600, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistoricalRecruitments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HistoricalRecruitments_Exams_ExamId",
                        column: x => x.ExamId,
                        principalTable: "Exams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MockTestResults",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExamId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TestTitle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TestDate = table.Column<DateOnly>(type: "date", nullable: false),
                    ScoreObtained = table.Column<decimal>(type: "decimal(7,2)", precision: 7, scale: 2, nullable: false),
                    MaxScore = table.Column<decimal>(type: "decimal(7,2)", precision: 7, scale: 2, nullable: false),
                    AccuracyPercent = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    TimeTakenMinutes = table.Column<int>(type: "int", nullable: false),
                    WeakAreasNotes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MockTestResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MockTestResults_Exams_ExamId",
                        column: x => x.ExamId,
                        principalTable: "Exams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PreviousYearPapers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExamId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CycleYear = table.Column<int>(type: "int", nullable: false),
                    StageName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    PaperUrl = table.Column<string>(type: "nvarchar(600)", maxLength: 600, nullable: false),
                    SourceLabel = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    IsOfficial = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PreviousYearPapers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PreviousYearPapers_Exams_ExamId",
                        column: x => x.ExamId,
                        principalTable: "Exams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SavedExams",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExamId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PriorityLevel = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    PersonalNotes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SavedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SavedExams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SavedExams_Exams_ExamId",
                        column: x => x.ExamId,
                        principalTable: "Exams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserStudyPlans",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExamId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlanTitle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DailyTargetMinutes = table.Column<int>(type: "int", nullable: false),
                    WeeklyTargetHours = table.Column<int>(type: "int", nullable: false),
                    TargetExamDate = table.Column<DateOnly>(type: "date", nullable: true),
                    TimetableJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserStudyPlans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserStudyPlans_Exams_ExamId",
                        column: x => x.ExamId,
                        principalTable: "Exams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserStudyProgresses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExamId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SyllabusTopicId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    RevisionCount = table.Column<int>(type: "int", nullable: false),
                    LastRevisedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PreparationNotes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserStudyProgresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserStudyProgresses_Exams_ExamId",
                        column: x => x.ExamId,
                        principalTable: "Exams",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserStudyProgresses_SyllabusTopics_SyllabusTopicId",
                        column: x => x.SyllabusTopicId,
                        principalTable: "SyllabusTopics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EligibilityRules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExamId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExamNotificationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    MinAgeYears = table.Column<int>(type: "int", nullable: false),
                    MaxAgeYears = table.Column<int>(type: "int", nullable: true),
                    AgeCutoffDate = table.Column<DateOnly>(type: "date", nullable: true),
                    AgeCutoffBasisDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MinimumQualificationLevel = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    RequiresEngineeringOrTechDegree = table.Column<bool>(type: "bit", nullable: false),
                    AcceptsAnyGraduateDegree = table.Column<bool>(type: "bit", nullable: false),
                    AcceptedEngineeringBranchesCsv = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    QualificationSummary = table.Column<string>(type: "nvarchar(1500)", maxLength: 1500, nullable: false),
                    MinPercentageGeneral = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: true),
                    MinPercentageReserved = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: true),
                    MinCgpa = table.Column<decimal>(type: "decimal(4,2)", precision: 4, scale: 2, nullable: true),
                    FinalYearAppearingEligible = table.Column<bool>(type: "bit", nullable: false),
                    RequiresWorkExperience = table.Column<bool>(type: "bit", nullable: false),
                    MinWorkExperienceMonths = table.Column<int>(type: "int", nullable: false),
                    ExperienceRequirementNotes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NationalityRequirement = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    DomicileRequirement = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    BiharDomicileRequiredForReservation = table.Column<bool>(type: "bit", nullable: false),
                    BiharCertificateConditions = table.Column<string>(type: "nvarchar(800)", maxLength: 800, nullable: true),
                    GenderRequirement = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    RequiresValidGateScore = table.Column<bool>(type: "bit", nullable: false),
                    AcceptedGatePapersCsv = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    MinGateScore = table.Column<int>(type: "int", nullable: true),
                    AdditionalConditions = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VerificationNotes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SourceReference = table.Column<string>(type: "nvarchar(600)", maxLength: 600, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EligibilityRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EligibilityRules_ExamNotifications_ExamNotificationId",
                        column: x => x.ExamNotificationId,
                        principalTable: "ExamNotifications",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EligibilityRules_Exams_ExamId",
                        column: x => x.ExamId,
                        principalTable: "Exams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExamDates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExamNotificationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EventType = table.Column<int>(type: "int", nullable: false),
                    Label = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ConfirmedDate = table.Column<DateOnly>(type: "date", nullable: true),
                    ExpectedDate = table.Column<DateOnly>(type: "date", nullable: true),
                    IsExpected = table.Column<bool>(type: "bit", nullable: false),
                    DisplayStatus = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamDates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExamDates_ExamNotifications_ExamNotificationId",
                        column: x => x.ExamNotificationId,
                        principalTable: "ExamNotifications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExamVacancies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExamNotificationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TotalVacancies = table.Column<int>(type: "int", nullable: true),
                    ItSpecificVacancies = table.Column<int>(type: "int", nullable: true),
                    GeneralVacancies = table.Column<int>(type: "int", nullable: true),
                    VacancyType = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(600)", maxLength: 600, nullable: true),
                    SourceReference = table.Column<string>(type: "nvarchar(600)", maxLength: 600, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamVacancies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExamVacancies_ExamNotifications_ExamNotificationId",
                        column: x => x.ExamNotificationId,
                        principalTable: "ExamNotifications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExamId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ExamNotificationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    NotificationType = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Message = table.Column<string>(type: "nvarchar(1500)", maxLength: 1500, nullable: false),
                    DeduplicationKey = table.Column<string>(type: "nvarchar(220)", maxLength: 220, nullable: false),
                    Channel = table.Column<int>(type: "int", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    ReadAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EmailSent = table.Column<bool>(type: "bit", nullable: false),
                    EmailSentAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ScheduledForUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notifications_ExamNotifications_ExamNotificationId",
                        column: x => x.ExamNotificationId,
                        principalTable: "ExamNotifications",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Notifications_Exams_ExamId",
                        column: x => x.ExamId,
                        principalTable: "Exams",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SourceVerificationLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OfficialSourceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExamNotificationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CheckedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CheckMethod = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    HttpStatusCode = table.Column<int>(type: "int", nullable: true),
                    ContentHash = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ChangesDetected = table.Column<bool>(type: "bit", nullable: false),
                    Outcome = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VerifiedByUserId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SourceVerificationLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SourceVerificationLogs_ExamNotifications_ExamNotificationId",
                        column: x => x.ExamNotificationId,
                        principalTable: "ExamNotifications",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SourceVerificationLogs_OfficialSources_OfficialSourceId",
                        column: x => x.OfficialSourceId,
                        principalTable: "OfficialSources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserApplications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExamId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExamNotificationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TrackingStatus = table.Column<int>(type: "int", nullable: false),
                    AppliedDate = table.Column<DateOnly>(type: "date", nullable: true),
                    ApplicationReferenceNumber = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    DocumentsChecklistJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserApplications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserApplications_ExamNotifications_ExamNotificationId",
                        column: x => x.ExamNotificationId,
                        principalTable: "ExamNotifications",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserApplications_Exams_ExamId",
                        column: x => x.ExamId,
                        principalTable: "Exams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HistoricalCategoryVacancies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HistoricalRecruitmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Category = table.Column<int>(type: "int", nullable: false),
                    VacancyCount = table.Column<int>(type: "int", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistoricalCategoryVacancies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HistoricalCategoryVacancies_HistoricalRecruitments_HistoricalRecruitmentId",
                        column: x => x.HistoricalRecruitmentId,
                        principalTable: "HistoricalRecruitments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PreviousYearCutoffs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExamId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HistoricalRecruitmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CycleYear = table.Column<int>(type: "int", nullable: false),
                    StageName = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Category = table.Column<int>(type: "int", nullable: false),
                    CutoffMarks = table.Column<decimal>(type: "decimal(7,2)", precision: 7, scale: 2, nullable: true),
                    TotalMaxMarks = table.Column<decimal>(type: "decimal(7,2)", precision: 7, scale: 2, nullable: true),
                    SourceUrl = table.Column<string>(type: "nvarchar(600)", maxLength: 600, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PreviousYearCutoffs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PreviousYearCutoffs_Exams_ExamId",
                        column: x => x.ExamId,
                        principalTable: "Exams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PreviousYearCutoffs_HistoricalRecruitments_HistoricalRecruitmentId",
                        column: x => x.HistoricalRecruitmentId,
                        principalTable: "HistoricalRecruitments",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CategoryWiseVacancies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExamVacancyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Category = table.Column<int>(type: "int", nullable: false),
                    SubCategory = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    VacancyCount = table.Column<int>(type: "int", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryWiseVacancies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CategoryWiseVacancies_ExamVacancies_ExamVacancyId",
                        column: x => x.ExamVacancyId,
                        principalTable: "ExamVacancies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AgeRelaxations_ExamId_Category",
                table: "AgeRelaxations",
                columns: new[] { "ExamId", "Category" });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_EntityName_EntityId",
                table: "AuditLogs",
                columns: new[] { "EntityName", "EntityId" });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_TimestampUtc",
                table: "AuditLogs",
                column: "TimestampUtc");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_Code",
                table: "Categories",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Categories_Slug",
                table: "Categories",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CategoryWiseVacancies_ExamVacancyId",
                table: "CategoryWiseVacancies",
                column: "ExamVacancyId");

            migrationBuilder.CreateIndex(
                name: "IX_EducationalQualifications_ExamId",
                table: "EducationalQualifications",
                column: "ExamId");

            migrationBuilder.CreateIndex(
                name: "IX_EligibilityRules_ExamId",
                table: "EligibilityRules",
                column: "ExamId");

            migrationBuilder.CreateIndex(
                name: "IX_EligibilityRules_ExamNotificationId",
                table: "EligibilityRules",
                column: "ExamNotificationId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamDates_ConfirmedDate",
                table: "ExamDates",
                column: "ConfirmedDate");

            migrationBuilder.CreateIndex(
                name: "IX_ExamDates_ExamNotificationId_EventType",
                table: "ExamDates",
                columns: new[] { "ExamNotificationId", "EventType" });

            migrationBuilder.CreateIndex(
                name: "IX_ExamDates_ExpectedDate",
                table: "ExamDates",
                column: "ExpectedDate");

            migrationBuilder.CreateIndex(
                name: "IX_ExamNotifications_ExamId_CycleYear_IsCurrentCycle",
                table: "ExamNotifications",
                columns: new[] { "ExamId", "CycleYear", "IsCurrentCycle" });

            migrationBuilder.CreateIndex(
                name: "IX_ExamNotifications_NotificationDate",
                table: "ExamNotifications",
                column: "NotificationDate");

            migrationBuilder.CreateIndex(
                name: "IX_ExamNotifications_Status_ApplicationEndDate",
                table: "ExamNotifications",
                columns: new[] { "Status", "ApplicationEndDate" });

            migrationBuilder.CreateIndex(
                name: "IX_ExamOrganizations_ShortCode",
                table: "ExamOrganizations",
                column: "ShortCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExamPatterns_ExamId_OrderIndex",
                table: "ExamPatterns",
                columns: new[] { "ExamId", "OrderIndex" });

            migrationBuilder.CreateIndex(
                name: "IX_Exams_CategoryId_IsApprovedForPublish",
                table: "Exams",
                columns: new[] { "CategoryId", "IsApprovedForPublish" });

            migrationBuilder.CreateIndex(
                name: "IX_Exams_Code",
                table: "Exams",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Exams_IsBiharSpecific_IsItSpecific_IsGraduateLevel",
                table: "Exams",
                columns: new[] { "IsBiharSpecific", "IsItSpecific", "IsGraduateLevel" });

            migrationBuilder.CreateIndex(
                name: "IX_Exams_OfficialSourceId",
                table: "Exams",
                column: "OfficialSourceId");

            migrationBuilder.CreateIndex(
                name: "IX_Exams_OrganizationId",
                table: "Exams",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Exams_Slug",
                table: "Exams",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExamSyllabi_ExamId_SyllabusSubjectId_StageName",
                table: "ExamSyllabi",
                columns: new[] { "ExamId", "SyllabusSubjectId", "StageName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExamSyllabi_SyllabusSubjectId",
                table: "ExamSyllabi",
                column: "SyllabusSubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamVacancies_ExamNotificationId",
                table: "ExamVacancies",
                column: "ExamNotificationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HistoricalCategoryVacancies_HistoricalRecruitmentId",
                table: "HistoricalCategoryVacancies",
                column: "HistoricalRecruitmentId");

            migrationBuilder.CreateIndex(
                name: "IX_HistoricalRecruitments_ExamId_CycleYear",
                table: "HistoricalRecruitments",
                columns: new[] { "ExamId", "CycleYear" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MockTestResults_ExamId",
                table: "MockTestResults",
                column: "ExamId");

            migrationBuilder.CreateIndex(
                name: "IX_MockTestResults_UserId_ExamId_TestDate",
                table: "MockTestResults",
                columns: new[] { "UserId", "ExamId", "TestDate" });

            migrationBuilder.CreateIndex(
                name: "IX_NotificationPreferences_UserId",
                table: "NotificationPreferences",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_ExamId",
                table: "Notifications",
                column: "ExamId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_ExamNotificationId",
                table: "Notifications",
                column: "ExamNotificationId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId_DeduplicationKey",
                table: "Notifications",
                columns: new[] { "UserId", "DeduplicationKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId_IsRead_ScheduledForUtc",
                table: "Notifications",
                columns: new[] { "UserId", "IsRead", "ScheduledForUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_OfficialSources_OfficialSourceUrl",
                table: "OfficialSources",
                column: "OfficialSourceUrl");

            migrationBuilder.CreateIndex(
                name: "IX_OfficialSources_OrganizationId",
                table: "OfficialSources",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_PreviousYearCutoffs_ExamId_CycleYear",
                table: "PreviousYearCutoffs",
                columns: new[] { "ExamId", "CycleYear" });

            migrationBuilder.CreateIndex(
                name: "IX_PreviousYearCutoffs_HistoricalRecruitmentId",
                table: "PreviousYearCutoffs",
                column: "HistoricalRecruitmentId");

            migrationBuilder.CreateIndex(
                name: "IX_PreviousYearPapers_ExamId",
                table: "PreviousYearPapers",
                column: "ExamId");

            migrationBuilder.CreateIndex(
                name: "IX_SavedExams_ExamId",
                table: "SavedExams",
                column: "ExamId");

            migrationBuilder.CreateIndex(
                name: "IX_SavedExams_UserId_ExamId",
                table: "SavedExams",
                columns: new[] { "UserId", "ExamId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SourceVerificationLogs_CheckedAtUtc",
                table: "SourceVerificationLogs",
                column: "CheckedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_SourceVerificationLogs_ExamNotificationId",
                table: "SourceVerificationLogs",
                column: "ExamNotificationId");

            migrationBuilder.CreateIndex(
                name: "IX_SourceVerificationLogs_OfficialSourceId",
                table: "SourceVerificationLogs",
                column: "OfficialSourceId");

            migrationBuilder.CreateIndex(
                name: "IX_SyllabusSubjects_Code",
                table: "SyllabusSubjects",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SyllabusTopics_SyllabusSubjectId_DisplayOrder",
                table: "SyllabusTopics",
                columns: new[] { "SyllabusSubjectId", "DisplayOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_UserApplications_ExamId",
                table: "UserApplications",
                column: "ExamId");

            migrationBuilder.CreateIndex(
                name: "IX_UserApplications_ExamNotificationId",
                table: "UserApplications",
                column: "ExamNotificationId");

            migrationBuilder.CreateIndex(
                name: "IX_UserApplications_UserId_ExamId",
                table: "UserApplications",
                columns: new[] { "UserId", "ExamId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserProfiles_UserId",
                table: "UserProfiles",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserStudyPlans_ExamId",
                table: "UserStudyPlans",
                column: "ExamId");

            migrationBuilder.CreateIndex(
                name: "IX_UserStudyPlans_UserId_ExamId",
                table: "UserStudyPlans",
                columns: new[] { "UserId", "ExamId" });

            migrationBuilder.CreateIndex(
                name: "IX_UserStudyProgresses_ExamId",
                table: "UserStudyProgresses",
                column: "ExamId");

            migrationBuilder.CreateIndex(
                name: "IX_UserStudyProgresses_SyllabusTopicId",
                table: "UserStudyProgresses",
                column: "SyllabusTopicId");

            migrationBuilder.CreateIndex(
                name: "IX_UserStudyProgresses_UserId_SyllabusTopicId_ExamId",
                table: "UserStudyProgresses",
                columns: new[] { "UserId", "SyllabusTopicId", "ExamId" },
                unique: true,
                filter: "[ExamId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AgeRelaxations");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "CategoryWiseVacancies");

            migrationBuilder.DropTable(
                name: "EducationalQualifications");

            migrationBuilder.DropTable(
                name: "EligibilityRules");

            migrationBuilder.DropTable(
                name: "ExamDates");

            migrationBuilder.DropTable(
                name: "ExamPatterns");

            migrationBuilder.DropTable(
                name: "ExamSyllabi");

            migrationBuilder.DropTable(
                name: "HistoricalCategoryVacancies");

            migrationBuilder.DropTable(
                name: "MockTestResults");

            migrationBuilder.DropTable(
                name: "NotificationPreferences");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "PreviousYearCutoffs");

            migrationBuilder.DropTable(
                name: "PreviousYearPapers");

            migrationBuilder.DropTable(
                name: "SavedExams");

            migrationBuilder.DropTable(
                name: "SourceVerificationLogs");

            migrationBuilder.DropTable(
                name: "UserApplications");

            migrationBuilder.DropTable(
                name: "UserProfiles");

            migrationBuilder.DropTable(
                name: "UserStudyPlans");

            migrationBuilder.DropTable(
                name: "UserStudyProgresses");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "ExamVacancies");

            migrationBuilder.DropTable(
                name: "HistoricalRecruitments");

            migrationBuilder.DropTable(
                name: "SyllabusTopics");

            migrationBuilder.DropTable(
                name: "ExamNotifications");

            migrationBuilder.DropTable(
                name: "SyllabusSubjects");

            migrationBuilder.DropTable(
                name: "Exams");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "OfficialSources");

            migrationBuilder.DropTable(
                name: "ExamOrganizations");
        }
    }
}
