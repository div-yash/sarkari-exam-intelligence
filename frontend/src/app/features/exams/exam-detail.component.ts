import { AfterViewInit, Component, ElementRef, OnDestroy, OnInit, ViewChild, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { Chart, registerables } from 'chart.js';
import { ApiService } from '../../core/services/api.service';
import { AuthService } from '../../core/services/auth.service';
import { environment } from '../../../environments/environment';
import { DateField, EligibilityCheckResult, ExamDetail } from '../../core/models/api.models';
import { EligibilityBadgeComponent, StatusBadgeComponent } from '../../shared/components/status-badge.component';

Chart.register(...registerables);

@Component({
  selector: 'app-exam-detail',
  standalone: true,
  imports: [CommonModule, RouterLink, StatusBadgeComponent, EligibilityBadgeComponent],
  template: `
    @if (loading()) {
      <div class="card-surface p-8 animate-pulse text-sm text-slate-500">Loading verified examination dossier...</div>
    } @else if (exam()) {
      <div class="space-y-6">
        <!-- Breadcrumb & Top Hero (Fields 1-6, 35-37) -->
        <div class="navy-header rounded-2xl p-6 shadow-md">
          <div class="flex flex-wrap items-center justify-between gap-3 mb-3">
            <div class="flex items-center gap-2 text-xs text-blue-200">
              <a routerLink="/exams" class="hover:underline">All Exams</a>
              <span>/</span>
              <span>{{ exam()!.categoryName }}</span>
              <span>/</span>
              <span class="text-white font-semibold">{{ exam()!.shortName }}</span>
            </div>
            <div class="flex items-center gap-2">
              <app-status-badge [status]="exam()!.status"></app-status-badge>
              @if (auth.isAuthenticated()) {
                <button (click)="toggleWatchlist()" class="px-3 py-1 rounded-lg text-xs font-semibold bg-white/15 hover:bg-white/25 text-white">
                  {{ exam()!.isSaved ? '★ Saved in Watchlist' : '☆ Add to Watchlist' }}
                </button>
              }
            </div>
          </div>

          <h1 class="text-2xl lg:text-3xl font-extrabold text-white">{{ exam()!.name }}</h1>
          <p class="text-sm text-blue-100 mt-1">{{ exam()!.summary }}</p>

          <div class="grid grid-cols-2 sm:grid-cols-4 lg:grid-cols-6 gap-4 mt-5 pt-4 border-t border-white/15 text-xs">
            <div>
              <div class="text-blue-200/80">Conducting Body</div>
              <div class="font-semibold text-white mt-0.5">{{ exam()!.conductingOrganization }}</div>
            </div>
            <div>
              <div class="text-blue-200/80">Department</div>
              <div class="font-semibold text-white mt-0.5">{{ exam()!.department }}</div>
            </div>
            <div>
              <div class="text-blue-200/80">Job Designation</div>
              <div class="font-semibold text-white mt-0.5">{{ exam()!.jobDesignation }}</div>
            </div>
            <div>
              <div class="text-blue-200/80">Classification</div>
              <div class="font-semibold text-white mt-0.5">{{ exam()!.groupClassification }}</div>
            </div>
            <div>
              <div class="text-blue-200/80">Job Nature</div>
              <div class="font-semibold text-white mt-0.5">{{ exam()!.jobNature }}</div>
            </div>
            <div>
              <div class="text-blue-200/80">Pay Level / Salary</div>
              <div class="font-semibold text-white mt-0.5">{{ exam()!.salaryPayLevel }}</div>
            </div>
          </div>

          <!-- Official PDF, Apply & Website Links (Fields 35-37) -->
          <div class="flex flex-wrap gap-3 mt-5">
            @if (exam()!.officialNotificationPdfUrl) {
              <a [href]="resolvePdfUrl(exam()!.officialNotificationPdfUrl!)" target="_blank" rel="noopener noreferrer"
                class="px-4 py-2 rounded-lg bg-white text-[#0b192c] text-xs font-bold hover:bg-blue-50">
                📄 Official Notification PDF / Portal ↗
              </a>
            }
            @if (exam()!.officialApplicationUrl) {
              <a [href]="exam()!.officialApplicationUrl" target="_blank" rel="noopener noreferrer"
                class="px-4 py-2 rounded-lg bg-green-600 hover:bg-green-500 text-white text-xs font-bold">
                ✅ Official Application Link ↗
              </a>
            }
            <a [href]="exam()!.officialRecruitmentWebsite" target="_blank" rel="noopener noreferrer"
              class="px-4 py-2 rounded-lg bg-white/10 hover:bg-white/20 text-white text-xs font-semibold border border-white/20">
              🌐 Official Recruitment Website ↗
            </a>
          </div>
        </div>

        <!-- Personalized Eligibility Verdict -->
        @if (eligibility()) {
          <div class="card-surface p-5 border-l-4"
            [class.border-l-green-600]="eligibility()!.status === 'Eligible'"
            [class.border-l-amber-500]="eligibility()!.status === 'PotentiallyEligibleVerificationRequired'"
            [class.border-l-red-600]="eligibility()!.status === 'NotEligible'">
            <div class="flex flex-wrap items-center justify-between gap-3">
              <div>
                <div class="text-xs font-bold uppercase tracking-wider text-slate-500">Your Personal Eligibility Evaluation</div>
                <div class="text-base font-bold text-slate-900 mt-0.5">{{ eligibility()!.summary }}</div>
              </div>
              <app-eligibility-badge [status]="eligibility()!.status"></app-eligibility-badge>
            </div>
            <div class="grid grid-cols-1 md:grid-cols-2 gap-4 mt-4 text-xs">
              <div class="bg-slate-50 rounded-lg p-3 border border-slate-200">
                <div class="font-bold text-slate-800 mb-1">Age Calculation on Notification Cutoff Date</div>
                <div>Age: <span class="font-semibold">{{ eligibility()!.ageEvaluation.calculatedAgeText }}</span></div>
                <div>Cutoff Basis: {{ eligibility()!.ageEvaluation.cutoffDateBasis }}</div>
                <div>Relaxation: {{ eligibility()!.ageEvaluation.relaxationExplanation }}</div>
              </div>
              <div class="bg-slate-50 rounded-lg p-3 border border-slate-200 space-y-1">
                <div class="font-bold text-slate-800">Criteria Verification Breakdown</div>
                @for (c of eligibility()!.criteriaChecks; track c.criterion) {
                  <div class="flex items-start gap-1.5">
                    <span>{{ c.passed ? (c.requiresVerification ? '⚠️' : '✅') : '❌' }}</span>
                    <span><strong>{{ c.criterion }}:</strong> {{ c.detail }}</span>
                  </div>
                }
              </div>
            </div>
          </div>
        }

        <!-- Confirmed vs Expected Dates Timeline (Fields 14-22) -->
        <div class="card-surface p-5">
          <div class="flex items-center justify-between mb-4">
            <h2 class="text-base font-bold text-slate-900">Important Dates (Confirmed vs Expected)</h2>
            <span class="text-xs text-slate-500">Cycle {{ exam()!.cycleYear }} {{ exam()!.notificationNumber ? '· ' + exam()!.notificationNumber : '' }}</span>
          </div>
          <div class="grid grid-cols-2 sm:grid-cols-3 lg:grid-cols-5 gap-3 text-xs">
            @for (d of dateCards(); track d.label) {
              <div class="p-3 rounded-xl border border-slate-200 bg-slate-50/70">
                <div class="text-slate-500 font-medium">{{ d.label }}</div>
                <div class="font-bold text-sm mt-1 tabular-nums" [class]="dateClass(d.field)">
                  {{ d.field.displayText }}
                </div>
                <span class="inline-block mt-1 px-1.5 py-0.5 rounded text-[10px] font-semibold"
                  [class]="d.field.dateLabelType === 'Confirmed' ? 'bg-green-100 text-green-800' : d.field.dateLabelType === 'Expected' ? 'bg-orange-100 text-orange-800' : 'bg-slate-200 text-slate-600'">
                  {{ d.field.dateLabelType }}
                </span>
              </div>
            }
            <div class="p-3 rounded-xl border border-slate-200 bg-slate-50/70">
              <div class="text-slate-500 font-medium">Correction Window</div>
              <div class="font-bold text-sm mt-1 text-slate-800 tabular-nums">
                @if (exam()!.correctionWindowStart) {
                  {{ exam()!.correctionWindowStart | date:'dd MMM' }} – {{ exam()!.correctionWindowEnd | date:'dd MMM yyyy' }}
                } @else {
                  Not Announced
                }
              </div>
            </div>
          </div>
        </div>

        <!-- Eligibility Rules (7-13) & Vacancies (23-25) & Exam Fee/Pattern (26-31) -->
        <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
          <div class="card-surface p-5 space-y-3 text-xs">
            <h2 class="text-base font-bold text-slate-900">Official Eligibility Conditions & Relaxations</h2>
            @if (exam()!.eligibilityRule; as er) {
              <div class="space-y-2 text-slate-700">
                <div><strong>Qualification Requirements:</strong> {{ er.qualificationSummary }}</div>
                <div><strong>Accepted Engineering Branches:</strong> {{ er.acceptedEngineeringBranches || (er.acceptsAnyGraduateDegree ? 'All recognized graduate streams' : 'Check notification') }}</div>
                <div><strong>Minimum % / CGPA:</strong> {{ er.minPercentageGeneral ? er.minPercentageGeneral + '% Gen' : 'No minimum % cutoff' }} {{ er.minPercentageReserved ? ' / ' + er.minPercentageReserved + '% Reserved' : '' }}</div>
                <div><strong>Experience Requirements:</strong> {{ er.requiresWorkExperience ? er.minWorkExperienceMonths + ' months (' + er.experienceRequirementNotes + ')' : 'No mandatory work experience' }}</div>
                <div><strong>Age Limit:</strong> {{ er.minAgeYears }} – {{ er.maxAgeYears ? er.maxAgeYears + ' yrs' : 'No Upper Age Limit' }} ({{ er.ageCutoffBasisDescription }})</div>
                <div><strong>Nationality & Domicile:</strong> {{ er.nationalityRequirement }} · {{ er.domicileRequirement || 'All India' }}</div>
                @if (er.biharDomicileRequiredForReservation) {
                  <div class="p-2.5 rounded-lg bg-amber-50 border border-amber-200 text-amber-900">
                    <strong>Bihar Reservation Rule:</strong> {{ er.biharCertificateConditions }}
                  </div>
                }
              </div>
            }
            @if (exam()!.ageRelaxations.length > 0) {
              <div class="pt-2 border-t border-slate-100">
                <div class="font-bold text-slate-800 mb-1.5">Category-Wise Age Relaxation</div>
                <div class="grid grid-cols-2 gap-2">
                  @for (ar of exam()!.ageRelaxations; track $index) {
                    <div class="p-2 rounded bg-slate-50 border border-slate-200">
                      <span class="font-bold">{{ ar.category }}:</span> +{{ ar.relaxationYears }} yrs
                      <div class="text-[11px] text-slate-500">{{ ar.conditions }}</div>
                    </div>
                  }
                </div>
              </div>
            }
          </div>

          <div class="card-surface p-5 space-y-4 text-xs">
            <h2 class="text-base font-bold text-slate-900">Vacancies, Fees & Selection Scheme</h2>
            @if (exam()!.vacancy; as v) {
              <div class="grid grid-cols-3 gap-3">
                <div class="p-3 rounded-xl bg-blue-50 border border-blue-200">
                  <div class="text-blue-700 font-medium">Total Vacancies</div>
                  <div class="text-lg font-extrabold text-blue-950 tabular-nums">{{ v.totalVacancies ?? 'Not Announced' }}</div>
                  <div class="text-[10px] text-blue-700">{{ v.vacancyType }}</div>
                </div>
                <div class="p-3 rounded-xl bg-slate-50 border border-slate-200">
                  <div class="text-slate-600 font-medium">IT-Specific</div>
                  <div class="text-lg font-extrabold text-slate-900 tabular-nums">{{ v.itSpecificVacancies ?? 'N/A' }}</div>
                </div>
                <div class="p-3 rounded-xl bg-slate-50 border border-slate-200">
                  <div class="text-slate-600 font-medium">General / Other</div>
                  <div class="text-lg font-extrabold text-slate-900 tabular-nums">{{ v.generalVacancies ?? 'N/A' }}</div>
                </div>
              </div>
              @if (v.categoryWiseVacancies.length > 0) {
                <div class="flex flex-wrap gap-2">
                  @for (cv of v.categoryWiseVacancies; track cv.category) {
                    <span class="px-2.5 py-1 rounded bg-slate-100 font-semibold">{{ cv.category }}: {{ cv.vacancyCount }}</span>
                  }
                </div>
              }
            }
            <div class="space-y-1.5 text-slate-700 pt-2 border-t border-slate-100">
              <div><strong>Application Fee:</strong> {{ exam()!.applicationFee }}</div>
              <div><strong>Exam Mode:</strong> {{ exam()!.examMode }}</div>
              <div><strong>Selection Procedure:</strong> {{ exam()!.selectionProcedure }}</div>
              <div><strong>Negative Marking:</strong> {{ exam()!.negativeMarking }}</div>
            </div>
          </div>
        </div>

        <!-- Section-Wise Exam Pattern & Detailed Syllabus (Fields 31-32) -->
        <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
          <div class="card-surface p-5">
            <h2 class="text-base font-bold text-slate-900 mb-3">Section-Wise Examination Pattern</h2>
            @if (exam()!.examPatterns.length === 0) {
              <p class="text-xs text-slate-500">Detailed sectional breakdown follows the official notification scheme ({{ exam()!.selectionProcedure }}).</p>
            } @else {
              <div class="overflow-x-auto">
                <table class="w-full text-xs text-left">
                  <thead class="bg-slate-50 text-slate-600 border-b border-slate-200">
                    <tr>
                      <th class="py-2 px-2">Stage</th>
                      <th class="py-2 px-2">Section</th>
                      <th class="py-2 px-2">Qs</th>
                      <th class="py-2 px-2">Marks</th>
                      <th class="py-2 px-2">Mins</th>
                    </tr>
                  </thead>
                  <tbody class="divide-y divide-slate-100">
                    @for (p of exam()!.examPatterns; track $index) {
                      <tr>
                        <td class="py-2 px-2 font-semibold">{{ p.stageName }}</td>
                        <td class="py-2 px-2">{{ p.sectionName }}</td>
                        <td class="py-2 px-2 tabular-nums">{{ p.questionsCount ?? '—' }}</td>
                        <td class="py-2 px-2 tabular-nums">{{ p.maxMarks ?? '—' }}</td>
                        <td class="py-2 px-2 tabular-nums">{{ p.durationMinutes ?? '—' }}</td>
                      </tr>
                    }
                  </tbody>
                </table>
              </div>
            }
          </div>

          <div class="card-surface p-5">
            <div class="flex items-center justify-between mb-3">
              <h2 class="text-base font-bold text-slate-900">Mapped Official Syllabus Library</h2>
              <a routerLink="/syllabus" class="text-xs text-blue-600 font-semibold hover:underline">Track Study Progress →</a>
            </div>
            <div class="space-y-3 max-h-72 overflow-y-auto pr-1">
              @for (s of exam()!.syllabi; track s.subjectId) {
                <div class="p-3 rounded-lg bg-slate-50 border border-slate-200 text-xs">
                  <div class="flex items-center justify-between">
                    <span class="font-bold text-slate-900">{{ s.subjectName }}</span>
                    <span class="px-2 py-0.5 rounded text-[10px] font-semibold"
                      [class]="s.subjectGroup === 'Technical' ? 'bg-blue-100 text-blue-800' : 'bg-slate-200 text-slate-700'">
                      {{ s.subjectGroup }}
                    </span>
                  </div>
                  <div class="text-slate-600 mt-1">{{ s.topics.slice(0, 5).join(' · ') }}</div>
                </div>
              }
            </div>
          </div>
        </div>

        <!-- 5-Year Historical Comparison Chart & Table + Cutoffs & Papers (Fields 33-34) -->
        <div class="card-surface p-5 space-y-5">
          <div class="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-2">
            <div>
              <h2 class="text-base font-bold text-slate-900">5-Year Historical Recruitment Comparison</h2>
              <p class="text-xs text-slate-500">Cycles without official data are explicitly marked Awaiting Official Notification — never invented.</p>
            </div>
          </div>

          @if (exam()!.historicalRecruitments.length > 0) {
            <div class="h-56"><canvas #historyChart></canvas></div>
            <div class="overflow-x-auto">
              <table class="w-full text-xs text-left">
                <thead class="bg-slate-50 text-slate-600 border-b border-slate-200">
                  <tr>
                    <th class="py-2 px-3">Cycle Year</th>
                    <th class="py-2 px-3">Data Status</th>
                    <th class="py-2 px-3">Total Vacancies</th>
                    <th class="py-2 px-3">IT Vacancies</th>
                    <th class="py-2 px-3">Notification Date</th>
                    <th class="py-2 px-3">Notes</th>
                  </tr>
                </thead>
                <tbody class="divide-y divide-slate-100">
                  @for (h of exam()!.historicalRecruitments; track h.cycleYear) {
                    <tr>
                      <td class="py-2 px-3 font-bold tabular-nums">{{ h.cycleYear }}</td>
                      <td class="py-2 px-3">{{ h.dataStatus }}</td>
                      <td class="py-2 px-3 font-semibold tabular-nums">{{ h.totalVacancies ?? 'Awaiting Notification' }}</td>
                      <td class="py-2 px-3 tabular-nums">{{ h.itVacancies ?? '—' }}</td>
                      <td class="py-2 px-3 tabular-nums">{{ h.notificationDate ? (h.notificationDate | date:'dd MMM yyyy') : '—' }}</td>
                      <td class="py-2 px-3 text-slate-600">{{ h.notes || '—' }}</td>
                    </tr>
                  }
                </tbody>
              </table>
            </div>
          } @else {
            <p class="text-xs text-slate-500">Historical cycle archive awaiting verification via official recruitment logs.</p>
          }

          <div class="grid grid-cols-1 md:grid-cols-2 gap-4 pt-3 border-t border-slate-100 text-xs">
            <div>
              <h3 class="font-bold text-slate-800 mb-2">Previous-Year Cutoffs</h3>
              @if (exam()!.previousYearCutoffs.length === 0) {
                <p class="text-slate-500">Official cutoff marks not yet archived for this exam.</p>
              } @else {
                <div class="space-y-1.5">
                  @for (c of exam()!.previousYearCutoffs; track $index) {
                    <div class="p-2 rounded bg-slate-50 border border-slate-200 flex justify-between">
                      <span>{{ c.cycleYear }} · {{ c.stageName }} ({{ c.category }})</span>
                      <span class="font-bold tabular-nums">{{ c.cutoffMarks }} / {{ c.totalMaxMarks }}</span>
                    </div>
                  }
                </div>
              }
            </div>
            <div>
              <h3 class="font-bold text-slate-800 mb-2">Official Previous Papers & Source Traceability</h3>
              <div class="space-y-2">
                @for (p of exam()!.previousYearPapers; track $index) {
                  <a [href]="p.paperUrl" target="_blank" rel="noopener noreferrer"
                    class="block p-2 rounded bg-slate-50 border border-slate-200 hover:border-blue-300 text-blue-700 font-medium">
                    {{ p.title }} ({{ p.sourceLabel }}) ↗
                  </a>
                }
                <div class="p-3 rounded-lg bg-[#0b192c] text-slate-200 text-[11px] space-y-1">
                  <div><strong>Official Source:</strong> {{ exam()!.officialSource.sourceOrganization }}</div>
                  <div><strong>Verification Status:</strong> {{ exam()!.officialSource.verificationStatus }} · {{ exam()!.officialSource.dataConfidence }}</div>
                  <div><strong>Last Checked:</strong> {{ exam()!.officialSource.dateLastCheckedUtc | date:'medium' }}</div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    }
  `
})
export class ExamDetailComponent implements OnInit, AfterViewInit, OnDestroy {
  private readonly route = inject(ActivatedRoute);
  private readonly api = inject(ApiService);
  readonly auth = inject(AuthService);

  @ViewChild('historyChart') historyCanvas?: ElementRef<HTMLCanvasElement>;

  exam = signal<ExamDetail | null>(null);
  eligibility = signal<EligibilityCheckResult | null>(null);
  loading = signal(true);

  private chart?: Chart;
  private viewReady = false;

  ngOnInit(): void {
    this.route.paramMap.subscribe((params) => {
      const slug = params.get('slug');
      if (!slug) return;
      this.loading.set(true);
      this.api.getExamDetail(slug).subscribe({
        next: (detail) => {
          this.exam.set(detail);
          this.loading.set(false);
          if (this.auth.isAuthenticated()) {
            this.api.getMyExamEligibility(detail.id).subscribe({ next: (el) => this.eligibility.set(el) });
          }
          setTimeout(() => this.renderChart(), 0);
        },
        error: () => this.loading.set(false)
      });
    });
  }

  ngAfterViewInit(): void {
    this.viewReady = true;
    this.renderChart();
  }

  ngOnDestroy(): void {
    this.chart?.destroy();
  }

  dateCards(): Array<{ label: string; field: DateField }> {
    const e = this.exam();
    if (!e) return [];
    return [
      { label: 'Notification Date', field: e.notificationPublicationDate },
      { label: 'Application Opens', field: e.applicationOpeningDate },
      { label: 'Application Closes', field: e.applicationClosingDate },
      { label: 'Admit Card Date', field: e.admitCardDate },
      { label: 'Prelims / CBT-1 Date', field: e.preliminaryExamDate },
      { label: 'Mains / CBT-2 Date', field: e.mainsExamDate },
      { label: 'Interview Date', field: e.interviewDate },
      { label: 'Result Date', field: e.resultDate }
    ];
  }

  dateClass(f: DateField): string {
    if (f.dateLabelType === 'Confirmed') return 'text-slate-900';
    if (f.dateLabelType === 'Expected') return 'text-orange-700';
    return 'text-slate-400';
  }

  resolvePdfUrl(url: string): string {
    if (url.startsWith('/')) {
      const base = environment.apiBaseUrl?.replace(/\/+$/, '') ?? '';
      return `${base}${url}`;
    }
    return url;
  }

  toggleWatchlist(): void {
    const e = this.exam();
    if (!e) return;
    if (e.isSaved) {
      this.api.removeSavedExam(e.id).subscribe(() => { e.isSaved = false; });
    } else {
      this.api.saveExam(e.id).subscribe(() => { e.isSaved = true; });
    }
  }

  private renderChart(): void {
    const e = this.exam();
    if (!this.viewReady || !e || !this.historyCanvas || e.historicalRecruitments.length === 0) return;

    this.chart?.destroy();
    this.chart = new Chart(this.historyCanvas.nativeElement, {
      type: 'bar',
      data: {
        labels: e.historicalRecruitments.map((h) => String(h.cycleYear)),
        datasets: [
          {
            label: 'Total Official Vacancies',
            data: e.historicalRecruitments.map((h) => h.totalVacancies ?? 0),
            backgroundColor: '#0b192c',
            borderRadius: 6
          },
          {
            label: 'IT Stream Vacancies',
            data: e.historicalRecruitments.map((h) => h.itVacancies ?? 0),
            backgroundColor: '#2563eb',
            borderRadius: 6
          }
        ]
      },
      options: { responsive: true, maintainAspectRatio: false }
    });
  }
}
