import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { ApiService } from '../../core/services/api.service';
import { AuthService } from '../../core/services/auth.service';
import { CategorySummary, ExamListItem } from '../../core/models/api.models';
import { EligibilityBadgeComponent, StatusBadgeComponent } from '../../shared/components/status-badge.component';

@Component({
  selector: 'app-exam-directory',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, StatusBadgeComponent, EligibilityBadgeComponent],
  template: `
    <div class="space-y-5">
      <!-- Page Header -->
      <div class="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-3">
        <div>
          <h1 class="text-2xl font-bold text-slate-900">{{ pageTitle() }}</h1>
          <p class="text-xs text-slate-500 mt-0.5">{{ pageSubtitle() }}</p>
        </div>
        <div class="text-xs font-semibold text-slate-600 bg-white border border-slate-200 px-3 py-1.5 rounded-lg tabular-nums">
          {{ totalCount() }} examinations matched
        </div>
      </div>

      <!-- Filter & Sorting Panel -->
      <div class="card-surface p-4 space-y-4">
        <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-5 gap-3">
          <input type="text" [(ngModel)]="filters.search" (ngModelChange)="onFilterChange()"
            placeholder="Search exam, post, dept, branch..."
            class="px-3 py-2 text-xs border border-slate-200 rounded-lg focus:ring-2 focus:ring-blue-500 focus:outline-none" />

          <select [(ngModel)]="filters.categorySlug" (ngModelChange)="onFilterChange()"
            class="px-3 py-2 text-xs border border-slate-200 rounded-lg bg-white">
            <option value="">All Categories (7 Families)</option>
            @for (cat of categories(); track cat.id) {
              <option [value]="cat.slug">{{ cat.name }} ({{ cat.examCount }})</option>
            }
          </select>

          <select [(ngModel)]="filters.status" (ngModelChange)="onFilterChange()"
            class="px-3 py-2 text-xs border border-slate-200 rounded-lg bg-white">
            <option value="">All Statuses</option>
            <option value="ApplicationsOpen">Applications Open</option>
            <option value="ClosingSoon">Closing Soon</option>
            <option value="UpcomingNotification">Upcoming Notification</option>
            <option value="NotificationReleased">Notification Released</option>
            <option value="ExaminationScheduled">Examination Scheduled</option>
            <option value="ExaminationCompleted">Examination Completed</option>
            <option value="AdmitCardReleased">Admit Card Released</option>
            <option value="ResultDeclared">Result Declared</option>
            <option value="ApplicationsClosed">Applications Closed</option>
            <option value="Postponed">Postponed</option>
            <option value="Cancelled">Cancelled</option>
          </select>

          <input type="text" [(ngModel)]="filters.engineeringBranch" (ngModelChange)="onFilterChange()"
            placeholder="Branch (e.g. IT, CS, ECE)"
            class="px-3 py-2 text-xs border border-slate-200 rounded-lg" />

          <select [(ngModel)]="filters.sortBy" (ngModelChange)="onFilterChange()"
            class="px-3 py-2 text-xs border border-slate-200 rounded-lg bg-white">
            <option value="deadline">Sort: Application Deadline</option>
            <option value="notificationDate">Sort: Notification Date</option>
            <option value="examDate">Sort: Exam Date</option>
            <option value="vacancies">Sort: Total Vacancies (High–Low)</option>
            <option value="eligibility">Sort: Relevance to My Eligibility</option>
          </select>
        </div>

        <!-- Quick Toggle Chips -->
        <div class="flex flex-wrap items-center gap-2 text-xs">
          <button type="button" (click)="toggleFlag('onlyOpenApplications')"
            [class]="chipClass(filters.onlyOpenApplications)">Open Applications</button>
          <button type="button" (click)="toggleFlag('notificationsExpectedThisMonth')"
            [class]="chipClass(filters.notificationsExpectedThisMonth)">Expected This Month</button>
          <button type="button" (click)="toggleFlag('notificationsExpectedIn3Months')"
            [class]="chipClass(filters.notificationsExpectedIn3Months)">Expected in 3 Months</button>
          <button type="button" (click)="toggleFlag('examsIn30Days')"
            [class]="chipClass(filters.examsIn30Days)">Exams in 30 Days</button>
          <button type="button" (click)="toggleFlag('examsIn90Days')"
            [class]="chipClass(filters.examsIn90Days)">Exams in 90 Days</button>
          <button type="button" (click)="toggleFlag('onlyBihar')"
            [class]="chipClass(filters.onlyBihar)">Bihar Vacancies</button>
          <button type="button" (click)="toggleFlag('onlyItSpecific')"
            [class]="chipClass(filters.onlyItSpecific)">IT-Specific</button>
          <button type="button" (click)="toggleFlag('onlyGraduateLevel')"
            [class]="chipClass(filters.onlyGraduateLevel)">Graduate-Level</button>
          @if (auth.isAuthenticated()) {
            <button type="button" (click)="toggleFlag('onlyMatchingMyEligibility')"
              [class]="chipClass(filters.onlyMatchingMyEligibility)">Matching My Eligibility</button>
          }
          <button type="button" (click)="resetFilters()" class="ml-auto text-xs text-slate-500 hover:text-slate-800 underline">
            Reset filters
          </button>
        </div>
      </div>

      <!-- Exam Cards Grid -->
      @if (loading()) {
        <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
          @for (_ of [1,2,3,4,5,6]; track $index) {
            <div class="card-surface p-5 h-44 animate-pulse bg-slate-100"></div>
          }
        </div>
      } @else if (exams().length === 0) {
        <div class="card-surface p-10 text-center">
          <p class="text-sm font-semibold text-slate-700">No examinations match the selected criteria.</p>
          <button (click)="resetFilters()" class="mt-3 px-4 py-2 text-xs rounded-lg bg-blue-600 text-white font-semibold">Clear Filters</button>
        </div>
      } @else {
        <div class="grid grid-cols-1 xl:grid-cols-2 gap-4">
          @for (exam of exams(); track exam.id) {
            <div class="card-surface p-5 hover:border-blue-300 transition flex flex-col justify-between gap-4">
              <div>
                <div class="flex items-start justify-between gap-3">
                  <div>
                    <div class="flex flex-wrap items-center gap-1.5 mb-1">
                      <span class="px-2 py-0.5 rounded bg-[#0b192c] text-white text-[11px] font-bold">{{ exam.organizationCode }}</span>
                      <span class="px-2 py-0.5 rounded bg-slate-100 text-slate-700 text-[11px] font-medium">{{ exam.categoryName }}</span>
                      @if (exam.isItSpecific) {
                        <span class="px-2 py-0.5 rounded bg-blue-50 text-blue-700 border border-blue-200 text-[11px] font-semibold">IT/CS</span>
                      }
                      @if (exam.isBiharSpecific) {
                        <span class="px-2 py-0.5 rounded bg-amber-50 text-amber-800 border border-amber-200 text-[11px] font-semibold">Bihar</span>
                      }
                    </div>
                    <a [routerLink]="['/exams', exam.slug]" class="text-base font-bold text-slate-900 hover:text-blue-600">
                      {{ exam.name }}
                    </a>
                    <div class="text-xs text-slate-500 mt-0.5">{{ exam.department }} · {{ exam.jobDesignation }}</div>
                  </div>
                  <app-status-badge [status]="exam.status"></app-status-badge>
                </div>

                <!-- Key Metrics Grid -->
                <div class="grid grid-cols-2 sm:grid-cols-4 gap-3 mt-4 pt-3 border-t border-slate-100 text-xs">
                  <div>
                    <div class="text-slate-400">Apply Deadline</div>
                    <div class="font-semibold text-slate-800 tabular-nums">
                      @if (exam.applicationEndDate) {
                        {{ exam.applicationEndDate | date:'dd MMM yyyy' }}
                      } @else if (exam.expectedApplicationEndDate) {
                        <span class="text-orange-600">Exp: {{ exam.expectedApplicationEndDate | date:'MMM yyyy' }}</span>
                      } @else {
                        <span class="text-slate-500">Not Announced</span>
                      }
                    </div>
                  </div>
                  <div>
                    <div class="text-slate-400">Exam Date</div>
                    <div class="font-semibold text-slate-800 tabular-nums">
                      @if (exam.examDate) {
                        {{ exam.examDate | date:'dd MMM yyyy' }}
                      } @else if (exam.expectedExamDate) {
                        <span class="text-orange-600">Exp: {{ exam.expectedExamDate | date:'MMM yyyy' }}</span>
                      } @else {
                        <span class="text-slate-500">Not Announced</span>
                      }
                    </div>
                  </div>
                  <div>
                    <div class="text-slate-400">Vacancies</div>
                    <div class="font-semibold text-slate-800 tabular-nums">
                      @if (exam.totalVacancies !== null && exam.totalVacancies !== undefined) {
                        {{ exam.totalVacancies | number }}
                        <span class="text-[10px] text-slate-500">({{ exam.vacancyType }})</span>
                      } @else {
                        <span class="text-slate-500">Not Announced</span>
                      }
                    </div>
                  </div>
                  <div>
                    <div class="text-slate-400">Age Limit</div>
                    <div class="font-semibold text-slate-800 tabular-nums">
                      {{ exam.minAge ?? 18 }}–{{ exam.maxAge ? exam.maxAge + ' yrs' : 'No Cap' }}
                    </div>
                  </div>
                </div>
              </div>

              <!-- Footer Row -->
              <div class="flex flex-wrap items-center justify-between gap-2 pt-3 border-t border-slate-100">
                <div class="flex items-center gap-2">
                  <app-eligibility-badge [status]="exam.userEligibilityStatus"></app-eligibility-badge>
                  <span class="text-[11px] text-slate-500">{{ exam.dataConfidence }}</span>
                </div>
                <div class="flex items-center gap-2">
                  @if (auth.isAuthenticated()) {
                    <button (click)="toggleSave(exam)"
                      class="px-2.5 py-1.5 rounded-lg text-xs font-semibold border transition"
                      [class]="exam.isSaved ? 'bg-amber-50 border-amber-300 text-amber-800' : 'bg-white border-slate-200 text-slate-700 hover:bg-slate-50'">
                      {{ exam.isSaved ? '★ Saved' : '☆ Watchlist' }}
                    </button>
                  }
                  <a [routerLink]="['/exams', exam.slug]"
                    class="px-3 py-1.5 rounded-lg bg-[#0b192c] hover:bg-[#17345c] text-white text-xs font-semibold">
                    Full Details →
                  </a>
                </div>
              </div>
            </div>
          }
        </div>

        <!-- Pagination -->
        @if (totalPages() > 1) {
          <div class="flex items-center justify-between pt-2">
            <div class="text-xs text-slate-500">Page {{ filters.page }} of {{ totalPages() }}</div>
            <div class="flex gap-2">
              <button [disabled]="filters.page <= 1" (click)="changePage(filters.page - 1)"
                class="px-3 py-1.5 text-xs rounded-lg border border-slate-200 bg-white disabled:opacity-50">Previous</button>
              <button [disabled]="filters.page >= totalPages()" (click)="changePage(filters.page + 1)"
                class="px-3 py-1.5 text-xs rounded-lg border border-slate-200 bg-white disabled:opacity-50">Next</button>
            </div>
          </div>
        }
      }
    </div>
  `
})
export class ExamDirectoryComponent implements OnInit {
  private readonly api = inject(ApiService);
  readonly auth = inject(AuthService);
  private readonly route = inject(ActivatedRoute);

  categories = signal<CategorySummary[]>([]);
  exams = signal<ExamListItem[]>([]);
  loading = signal(true);
  totalCount = signal(0);
  totalPages = signal(1);
  mode = signal<'all' | 'upcoming' | 'open'>('all');

  filters = {
    search: '',
    categorySlug: '',
    status: '',
    engineeringBranch: '',
    sortBy: 'deadline',
    sortDescending: false,
    onlyOpenApplications: false,
    notificationsExpectedThisMonth: false,
    notificationsExpectedIn3Months: false,
    examsIn30Days: false,
    examsIn90Days: false,
    onlyBihar: false,
    onlyItSpecific: false,
    onlyGraduateLevel: false,
    onlyMatchingMyEligibility: false,
    page: 1,
    pageSize: 16
  };

  ngOnInit(): void {
    this.api.getCategories().subscribe({ next: (c) => this.categories.set(c) });

    this.route.data.subscribe((d) => {
      const m = d['mode'] as 'all' | 'upcoming' | 'open' | undefined;
      if (m) this.mode.set(m);
      if (m === 'open') {
        this.filters.onlyOpenApplications = true;
      } else if (m === 'upcoming') {
        this.filters.status = 'UpcomingNotification';
      }
    });

    this.route.queryParams.subscribe((qp) => {
      if (qp['search']) this.filters.search = qp['search'];
      if (qp['category']) this.filters.categorySlug = qp['category'];
      this.loadExams();
    });
  }

  pageTitle(): string {
    if (this.mode() === 'open') return 'Currently Open Applications';
    if (this.mode() === 'upcoming') return 'Upcoming Government Notifications';
    return 'All Government Examinations (65 Families)';
  }

  pageSubtitle(): string {
    if (this.mode() === 'open') return 'Active application windows with verified closing dates and official apply links.';
    if (this.mode() === 'upcoming') return 'Tracked upcoming and expected notifications with historical reference cycles.';
    return 'Search across UPSC, SSC, Banking, Financial Regulators, Railways, Bihar Govt & Technical IT recruitments.';
  }

  loadExams(): void {
    this.loading.set(true);
    this.filters.sortDescending = this.filters.sortBy === 'vacancies';

    const query: Record<string, string | number | boolean> = {};
    for (const [k, v] of Object.entries(this.filters)) {
      if (v !== false && v !== '' && v !== null && v !== undefined) {
        query[k] = v;
      }
    }

    this.api.searchExams(query).subscribe({
      next: (res) => {
        this.exams.set(res.items);
        this.totalCount.set(res.totalCount);
        this.totalPages.set(res.totalPages);
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  onFilterChange(): void {
    this.filters.page = 1;
    this.loadExams();
  }

  toggleFlag(key: 'onlyOpenApplications' | 'notificationsExpectedThisMonth' | 'notificationsExpectedIn3Months' | 'examsIn30Days' | 'examsIn90Days' | 'onlyBihar' | 'onlyItSpecific' | 'onlyGraduateLevel' | 'onlyMatchingMyEligibility'): void {
    this.filters[key] = !this.filters[key];
    this.onFilterChange();
  }

  resetFilters(): void {
    this.filters = {
      search: '', categorySlug: '', status: this.mode() === 'upcoming' ? 'UpcomingNotification' : '',
      engineeringBranch: '', sortBy: 'deadline', sortDescending: false,
      onlyOpenApplications: this.mode() === 'open', notificationsExpectedThisMonth: false,
      notificationsExpectedIn3Months: false, examsIn30Days: false, examsIn90Days: false,
      onlyBihar: false, onlyItSpecific: false, onlyGraduateLevel: false,
      onlyMatchingMyEligibility: false, page: 1, pageSize: 16
    };
    this.loadExams();
  }

  changePage(p: number): void {
    this.filters.page = p;
    this.loadExams();
  }

  chipClass(active: boolean): string {
    return active
      ? 'px-3 py-1 rounded-full bg-blue-600 text-white font-semibold border border-blue-600'
      : 'px-3 py-1 rounded-full bg-slate-100 text-slate-700 hover:bg-slate-200 font-medium border border-slate-200';
  }

  toggleSave(exam: ExamListItem): void {
    if (exam.isSaved) {
      this.api.removeSavedExam(exam.id).subscribe(() => {
        exam.isSaved = false;
      });
    } else {
      this.api.saveExam(exam.id).subscribe(() => {
        exam.isSaved = true;
      });
    }
  }
}
