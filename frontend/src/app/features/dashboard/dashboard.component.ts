import { AfterViewInit, Component, ElementRef, OnDestroy, OnInit, ViewChild, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { Chart, registerables } from 'chart.js';
import { ApiService } from '../../core/services/api.service';
import { AuthService } from '../../core/services/auth.service';
import { DashboardSummary } from '../../core/models/api.models';
import { EligibilityBadgeComponent, StatusBadgeComponent } from '../../shared/components/status-badge.component';

Chart.register(...registerables);

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RouterLink, StatusBadgeComponent, EligibilityBadgeComponent],
  template: `
    <div class="space-y-6">
      <!-- Header Banner -->
      <div class="navy-header rounded-2xl p-6 shadow-sm flex flex-col md:flex-row md:items-center md:justify-between gap-4">
        <div>
          <span class="inline-block px-2.5 py-0.5 rounded-full bg-blue-500/20 border border-blue-400/30 text-blue-200 text-xs font-semibold mb-2">
            Verified Indian Government Recruitment Intelligence
          </span>
          <h1 class="text-2xl font-bold tracking-tight">
            @if (auth.isAuthenticated()) {
              Welcome back, {{ auth.currentUser()?.fullName }}
            } @else {
              Sarkari Exam Intelligence Dashboard
            }
          </h1>
          <p class="text-sm text-blue-100/80 mt-1">
            Official sources tracked across UPSC, SSC, Banking, Financial Regulators, Railways, Bihar Govt & Technical IT cadres.
          </p>
        </div>
        <div class="flex flex-wrap gap-2">
          <a routerLink="/eligibility" class="px-4 py-2 rounded-lg bg-blue-600 hover:bg-blue-500 text-white text-xs font-semibold shadow">
            Check My Eligibility
          </a>
          <a routerLink="/calendar" class="px-4 py-2 rounded-lg bg-white/10 hover:bg-white/20 text-white text-xs font-semibold border border-white/15">
            Exam Calendar
          </a>
        </div>
      </div>

      @if (loading()) {
        <div class="grid grid-cols-2 md:grid-cols-4 gap-4">
          @for (_ of [1,2,3,4,5,6,7,8]; track $index) {
            <div class="card-surface p-4 h-24 animate-pulse bg-slate-100"></div>
          }
        </div>
      } @else if (summary()) {
        <!-- Prominent Urgent Deadlines Notification Area -->
        @if (summary()!.urgentDeadlines.length > 0) {
          <div class="rounded-2xl border-2 border-red-200 bg-red-50/70 p-4 lg:p-5">
            <div class="flex items-center justify-between mb-3">
              <div class="flex items-center gap-2">
                <span class="px-2 py-0.5 rounded bg-red-600 text-white text-xs font-bold uppercase">Urgent Deadlines</span>
                <h2 class="text-sm font-bold text-red-950">Applications Closing Within 30 Days</h2>
              </div>
              <a routerLink="/open-applications" class="text-xs font-semibold text-red-700 hover:underline">View all open forms →</a>
            </div>
            <div class="grid grid-cols-1 md:grid-cols-2 xl:grid-cols-3 gap-3">
              @for (item of summary()!.urgentDeadlines; track item.examId) {
                <a [routerLink]="['/exams', item.slug]" class="bg-white rounded-xl p-3.5 border border-red-200 hover:border-red-400 transition flex items-start justify-between gap-3">
                  <div class="min-w-0">
                    <div class="text-xs font-semibold text-slate-500">{{ item.organizationCode }}</div>
                    <div class="text-sm font-bold text-slate-900 truncate">{{ item.examName }}</div>
                    <div class="text-xs text-slate-600 mt-1 tabular-nums">Closes: {{ item.applicationEndDate | date:'dd MMM yyyy' }}</div>
                  </div>
                  <span [class]="item.daysRemaining <= 3 ? 'bg-red-600 text-white' : item.daysRemaining <= 7 ? 'bg-red-100 text-red-800' : 'bg-orange-100 text-orange-800'"
                    class="px-2.5 py-1 rounded-lg text-xs font-bold whitespace-nowrap tabular-nums">
                    {{ item.daysRemaining === 0 ? 'TODAY' : item.daysRemaining + 'd left' }}
                  </span>
                </a>
              }
            </div>
          </div>
        }

        <!-- 11 Required Dashboard KPI Widgets -->
        <div class="grid grid-cols-2 sm:grid-cols-3 lg:grid-cols-4 xl:grid-cols-6 gap-4">
          <a routerLink="/exams" class="card-surface p-4 hover:border-blue-300 transition">
            <div class="text-xs font-medium text-slate-500">Total Exams Tracked</div>
            <div class="text-2xl font-extrabold text-[#0b192c] mt-1 tabular-nums">{{ summary()!.totalExamsTracked }}</div>
          </a>
          <a routerLink="/open-applications" class="card-surface p-4 border-l-4 border-l-green-600 hover:border-green-400 transition">
            <div class="text-xs font-medium text-green-700">Open Applications</div>
            <div class="text-2xl font-extrabold text-green-700 mt-1 tabular-nums">{{ summary()!.currentlyOpenApplications }}</div>
          </a>
          <a routerLink="/upcoming-notifications" class="card-surface p-4 border-l-4 border-l-orange-500 hover:border-orange-400 transition">
            <div class="text-xs font-medium text-orange-700">Upcoming Notifications</div>
            <div class="text-2xl font-extrabold text-orange-600 mt-1 tabular-nums">{{ summary()!.upcomingNotificationsCount }}</div>
          </a>
          <a routerLink="/open-applications" class="card-surface p-4 border-l-4 border-l-red-600 hover:border-red-400 transition">
            <div class="text-xs font-medium text-red-700">Closing Within 7 Days</div>
            <div class="text-2xl font-extrabold text-red-600 mt-1 tabular-nums">{{ summary()!.applicationsClosingIn7Days }}</div>
          </a>
          <a routerLink="/open-applications" class="card-surface p-4">
            <div class="text-xs font-medium text-slate-500">Closing Within 30 Days</div>
            <div class="text-2xl font-extrabold text-amber-700 mt-1 tabular-nums">{{ summary()!.applicationsClosingIn30Days }}</div>
          </a>
          <a routerLink="/calendar" class="card-surface p-4">
            <div class="text-xs font-medium text-slate-500">Upcoming Exam Dates</div>
            <div class="text-2xl font-extrabold text-blue-700 mt-1 tabular-nums">{{ summary()!.upcomingExamDatesCount }}</div>
          </a>
          <a routerLink="/eligibility" class="card-surface p-4">
            <div class="text-xs font-medium text-slate-500">Matching Qualification</div>
            <div class="text-2xl font-extrabold text-blue-700 mt-1 tabular-nums">{{ summary()!.examsMatchingMyQualification }}</div>
          </a>
          <a routerLink="/saved-exams" class="card-surface p-4">
            <div class="text-xs font-medium text-slate-500">Saved Examinations</div>
            <div class="text-2xl font-extrabold text-slate-800 mt-1 tabular-nums">{{ summary()!.savedExamsCount }}</div>
          </a>
          <a routerLink="/exams" class="card-surface p-4">
            <div class="text-xs font-medium text-slate-500">Newly Released</div>
            <div class="text-2xl font-extrabold text-sky-700 mt-1 tabular-nums">{{ summary()!.newlyReleasedNotificationsCount }}</div>
          </a>
          <a routerLink="/analytics" class="card-surface p-4 col-span-1 sm:col-span-2 lg:col-span-1">
            <div class="text-xs font-medium text-slate-500">Official Vacancies</div>
            <div class="text-2xl font-extrabold text-[#0b192c] mt-1 tabular-nums">{{ summary()!.totalVacanciesAnnounced | number }}</div>
          </a>
          <a routerLink="/preparation" class="card-surface p-4 col-span-2">
            <div class="flex items-center justify-between text-xs font-medium text-slate-500">
              <span>My Preparation Progress</span>
              <span class="font-bold text-blue-700 tabular-nums">{{ summary()!.myPreparationProgressPercent }}%</span>
            </div>
            <div class="w-full h-2.5 bg-slate-100 rounded-full mt-3 overflow-hidden">
              <div class="h-full bg-blue-600 rounded-full transition-all" [style.width.%]="summary()!.myPreparationProgressPercent"></div>
            </div>
          </a>
        </div>

        <!-- Interactive Charts -->
        <div class="grid grid-cols-1 lg:grid-cols-3 gap-6">
          <div class="card-surface p-5 lg:col-span-2">
            <h3 class="text-sm font-bold text-slate-900 mb-3">Exams Tracked by Category</h3>
            <div class="h-64"><canvas #categoryChart></canvas></div>
          </div>
          <div class="card-surface p-5">
            <h3 class="text-sm font-bold text-slate-900 mb-3">Lifecycle Status Distribution</h3>
            <div class="h-64"><canvas #statusChart></canvas></div>
          </div>
        </div>

        <!-- Open Applications & Upcoming Exam Dates Lists -->
        <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
          <div class="card-surface p-5">
            <div class="flex items-center justify-between mb-4">
              <h3 class="text-sm font-bold text-slate-900">Currently Open Applications</h3>
              <a routerLink="/open-applications" class="text-xs font-semibold text-blue-600 hover:underline">See all →</a>
            </div>
            @if (summary()!.openApplicationsList.length === 0) {
              <p class="text-xs text-slate-500 py-4">No currently open application windows in verified records.</p>
            } @else {
              <div class="divide-y divide-slate-100">
                @for (item of summary()!.openApplicationsList; track item.examId) {
                  <div class="py-3 flex items-center justify-between gap-3">
                    <div class="min-w-0">
                      <a [routerLink]="['/exams', item.slug]" class="text-sm font-semibold text-slate-900 hover:text-blue-600 truncate block">{{ item.name }}</a>
                      <div class="text-xs text-slate-500 mt-0.5 flex items-center gap-2">
                        <span>{{ item.organizationCode }}</span>
                        <span>·</span>
                        <span class="tabular-nums">Deadline: {{ item.relevantDate ? (item.relevantDate | date:'dd MMM yyyy') : 'Not Announced' }}</span>
                      </div>
                    </div>
                    <div class="flex items-center gap-2 shrink-0">
                      <app-eligibility-badge [status]="item.eligibilityStatus"></app-eligibility-badge>
                      <app-status-badge [status]="item.status"></app-status-badge>
                    </div>
                  </div>
                }
              </div>
            }
          </div>

          <div class="card-surface p-5">
            <div class="flex items-center justify-between mb-4">
              <h3 class="text-sm font-bold text-slate-900">Upcoming Examination Dates</h3>
              <a routerLink="/calendar" class="text-xs font-semibold text-blue-600 hover:underline">Open Calendar →</a>
            </div>
            @if (summary()!.upcomingExamDatesList.length === 0) {
              <p class="text-xs text-slate-500 py-4">No confirmed or expected upcoming examination dates right now.</p>
            } @else {
              <div class="divide-y divide-slate-100">
                @for (item of summary()!.upcomingExamDatesList; track item.examId) {
                  <div class="py-3 flex items-center justify-between gap-3">
                    <div class="min-w-0">
                      <a [routerLink]="['/exams', item.slug]" class="text-sm font-semibold text-slate-900 hover:text-blue-600 truncate block">{{ item.name }}</a>
                      <div class="text-xs text-slate-500 mt-0.5">
                        {{ item.categoryName }} ·
                        <span class="font-medium" [class.text-orange-600]="item.isExpected">
                          {{ item.isExpected ? 'Expected: ' : 'Confirmed: ' }}{{ item.relevantDate | date:'dd MMM yyyy' }}
                        </span>
                      </div>
                    </div>
                    <app-status-badge [status]="item.status"></app-status-badge>
                  </div>
                }
              </div>
            }
          </div>
        </div>
      }
    </div>
  `
})
export class DashboardComponent implements OnInit, AfterViewInit, OnDestroy {
  private readonly api = inject(ApiService);
  readonly auth = inject(AuthService);

  @ViewChild('categoryChart') categoryCanvas?: ElementRef<HTMLCanvasElement>;
  @ViewChild('statusChart') statusCanvas?: ElementRef<HTMLCanvasElement>;

  summary = signal<DashboardSummary | null>(null);
  loading = signal(true);

  private charts: Chart[] = [];
  private viewReady = false;

  ngOnInit(): void {
    this.api.getDashboardSummary().subscribe({
      next: (data) => {
        this.summary.set(data);
        this.loading.set(false);
        setTimeout(() => this.renderCharts(), 0);
      },
      error: () => this.loading.set(false)
    });
  }

  ngAfterViewInit(): void {
    this.viewReady = true;
    this.renderCharts();
  }

  ngOnDestroy(): void {
    this.charts.forEach((c) => c.destroy());
  }

  private renderCharts(): void {
    const data = this.summary();
    if (!this.viewReady || !data || !this.categoryCanvas || !this.statusCanvas) return;

    this.charts.forEach((c) => c.destroy());
    this.charts = [];

    this.charts.push(new Chart(this.categoryCanvas.nativeElement, {
      type: 'bar',
      data: {
        labels: data.examsByCategory.map((c) => c.categoryName),
        datasets: [
          {
            label: 'Total Exams Tracked',
            data: data.examsByCategory.map((c) => c.examCount),
            backgroundColor: '#0b192c',
            borderRadius: 6
          },
          {
            label: 'Open Applications',
            data: data.examsByCategory.map((c) => c.openCount),
            backgroundColor: '#16a34a',
            borderRadius: 6
          }
        ]
      },
      options: { responsive: true, maintainAspectRatio: false }
    }));

    this.charts.push(new Chart(this.statusCanvas.nativeElement, {
      type: 'doughnut',
      data: {
        labels: data.statusDistribution.map((s) => s.statusName),
        datasets: [{
          data: data.statusDistribution.map((s) => s.count),
          backgroundColor: ['#ea580c', '#16a34a', '#dc2626', '#2563eb', '#64748b', '#7c3aed', '#059669', '#d97706']
        }]
      },
      options: { responsive: true, maintainAspectRatio: false, plugins: { legend: { position: 'bottom' } } }
    }));
  }
}
