import { AfterViewInit, Component, ElementRef, OnDestroy, OnInit, ViewChild, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { Chart, registerables } from 'chart.js';
import { ApiService } from '../../core/services/api.service';
import { CategorySummary, VacancyAnalytics } from '../../core/models/api.models';

Chart.register(...registerables);

@Component({
  selector: 'app-analytics',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  template: `
    <div class="space-y-6">
      <div class="flex flex-col sm:flex-row sm:items-center justify-between gap-3">
        <div>
          <h1 class="text-2xl font-bold text-slate-900">Vacancy & Historical Recruitment Analytics</h1>
          <p class="text-xs text-slate-500 mt-0.5">Verified multi-year trends across departments, IT vs general cadres, and category splits.</p>
        </div>
        <select [(ngModel)]="selectedCategory" (ngModelChange)="loadData()" class="px-3 py-2 text-xs border border-slate-200 rounded-lg bg-white">
          <option value="">All Categories</option>
          @for (c of categories(); track c.id) {
            <option [value]="c.slug">{{ c.name }}</option>
          }
        </select>
      </div>

      @if (analytics(); as an) {
        <!-- Explicit Non-Prediction Disclaimer -->
        <div class="p-4 rounded-xl bg-amber-50 border border-amber-200 text-xs text-amber-900 font-medium">
          <strong>Accuracy Notice:</strong> {{ an.disclaimer }}
        </div>

        <div class="grid grid-cols-1 lg:grid-cols-3 gap-6">
          <div class="card-surface p-5 lg:col-span-2">
            <h2 class="text-sm font-bold text-slate-900 mb-3">Announced Official Vacancies by Year</h2>
            <div class="h-64"><canvas #yearChart></canvas></div>
          </div>
          <div class="card-surface p-5">
            <h2 class="text-sm font-bold text-slate-900 mb-3">IT-Specific vs General Vacancies</h2>
            <div class="h-64"><canvas #itSplitChart></canvas></div>
          </div>
        </div>

        <div class="card-surface p-5">
          <h2 class="text-sm font-bold text-slate-900 mb-3">Vacancies by Exam Category / Department</h2>
          <div class="h-64"><canvas #deptChart></canvas></div>
        </div>

        <!-- Exam Comparison Table -->
        <div class="card-surface p-5">
          <h2 class="text-sm font-bold text-slate-900 mb-3">Exam-Wise Verified Vacancy Comparison</h2>
          <div class="overflow-x-auto">
            <table class="w-full text-xs text-left">
              <thead class="bg-slate-50 text-slate-600 border-b border-slate-200">
                <tr>
                  <th class="py-2.5 px-3">Examination</th>
                  <th class="py-2.5 px-3">Category</th>
                  <th class="py-2.5 px-3">Latest Cycle</th>
                  <th class="py-2.5 px-3">Latest Vacancies</th>
                  <th class="py-2.5 px-3">Multi-Year Sum (Verified)</th>
                  <th class="py-2.5 px-3">Data Status</th>
                </tr>
              </thead>
              <tbody class="divide-y divide-slate-100">
                @for (row of an.vacanciesByExam; track row.examId) {
                  <tr>
                    <td class="py-2.5 px-3 font-bold">
                      <a [routerLink]="['/exams', row.examSlug]" class="text-blue-700 hover:underline">{{ row.examName }}</a>
                    </td>
                    <td class="py-2.5 px-3">{{ row.categoryName }}</td>
                    <td class="py-2.5 px-3 tabular-nums">{{ row.latestCycleYear }}</td>
                    <td class="py-2.5 px-3 font-semibold tabular-nums">{{ row.latestVacancies ?? 'Not Announced' }}</td>
                    <td class="py-2.5 px-3 tabular-nums">{{ row.fiveYearTotalVacancies ?? '—' }}</td>
                    <td class="py-2.5 px-3">{{ row.latestDataStatus }}</td>
                  </tr>
                }
              </tbody>
            </table>
          </div>
        </div>
      }
    </div>
  `
})
export class AnalyticsComponent implements OnInit, AfterViewInit, OnDestroy {
  private readonly api = inject(ApiService);

  @ViewChild('yearChart') yearCanvas?: ElementRef<HTMLCanvasElement>;
  @ViewChild('itSplitChart') itSplitCanvas?: ElementRef<HTMLCanvasElement>;
  @ViewChild('deptChart') deptCanvas?: ElementRef<HTMLCanvasElement>;

  analytics = signal<VacancyAnalytics | null>(null);
  categories = signal<CategorySummary[]>([]);
  selectedCategory = '';

  private charts: Chart[] = [];
  private viewReady = false;

  ngOnInit(): void {
    this.api.getCategories().subscribe((c) => this.categories.set(c));
    this.loadData();
  }

  ngAfterViewInit(): void {
    this.viewReady = true;
    this.renderCharts();
  }

  ngOnDestroy(): void {
    this.charts.forEach((c) => c.destroy());
  }

  loadData(): void {
    this.api.getVacancyAnalytics(this.selectedCategory || undefined).subscribe((data) => {
      this.analytics.set(data);
      setTimeout(() => this.renderCharts(), 0);
    });
  }

  private renderCharts(): void {
    const an = this.analytics();
    if (!this.viewReady || !an || !this.yearCanvas || !this.itSplitCanvas || !this.deptCanvas) return;

    this.charts.forEach((c) => c.destroy());
    this.charts = [];

    this.charts.push(new Chart(this.yearCanvas.nativeElement, {
      type: 'bar',
      data: {
        labels: an.vacanciesByYear.map((y) => String(y.year)),
        datasets: [
          { label: 'General & Other Vacancies', data: an.vacanciesByYear.map((y) => y.generalVacancies), backgroundColor: '#0b192c', borderRadius: 4 },
          { label: 'IT-Specific Vacancies', data: an.vacanciesByYear.map((y) => y.itSpecificVacancies), backgroundColor: '#2563eb', borderRadius: 4 }
        ]
      },
      options: { responsive: true, maintainAspectRatio: false, scales: { x: { stacked: true }, y: { stacked: true } } }
    }));

    this.charts.push(new Chart(this.itSplitCanvas.nativeElement, {
      type: 'doughnut',
      data: {
        labels: ['IT-Specific Posts', 'General / Non-IT Posts'],
        datasets: [{
          data: [an.itVsGeneralSplit.itSpecificVacancies, an.itVsGeneralSplit.generalAndOtherVacancies],
          backgroundColor: ['#2563eb', '#0b192c']
        }]
      },
      options: { responsive: true, maintainAspectRatio: false, plugins: { legend: { position: 'bottom' } } }
    }));

    this.charts.push(new Chart(this.deptCanvas.nativeElement, {
      type: 'bar',
      data: {
        labels: an.vacanciesByDepartment.map((d) => d.categoryName),
        datasets: [
          { label: 'Total Latest/Verified Vacancies', data: an.vacanciesByDepartment.map((d) => d.totalVacancies), backgroundColor: '#16a34a', borderRadius: 6 }
        ]
      },
      options: { responsive: true, maintainAspectRatio: false }
    }));
  }
}
