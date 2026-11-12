import { AfterViewInit, Component, ElementRef, OnDestroy, OnInit, ViewChild, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { Chart, registerables } from 'chart.js';
import { ApiService } from '../../core/services/api.service';
import { ExamListItem, PreparationOverview } from '../../core/models/api.models';

Chart.register(...registerables);

@Component({
  selector: 'app-preparation',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  template: `
    <div class="space-y-6">
      <div class="flex flex-col sm:flex-row sm:items-center justify-between gap-3">
        <div>
          <h1 class="text-2xl font-bold text-slate-900">My Preparation & Application Tracker</h1>
          <p class="text-xs text-slate-500 mt-0.5">Track study targets, timetable, syllabus mastery, mock test performance, applications & documents.</p>
        </div>
        <a routerLink="/syllabus" class="px-4 py-2 rounded-lg bg-[#0b192c] text-white text-xs font-semibold">Open Syllabus Checklist →</a>
      </div>

      @if (overview(); as ov) {
        <!-- Summary Metrics -->
        <div class="grid grid-cols-2 lg:grid-cols-4 gap-4">
          <div class="card-surface p-4">
            <div class="text-xs text-slate-500">Topic Completion</div>
            <div class="text-2xl font-extrabold text-blue-700 mt-1 tabular-nums">{{ ov.overallCompletionPercent }}%</div>
            <div class="text-[11px] text-slate-500">{{ ov.completedTopicsCount }}/{{ ov.totalTopicsTracked }} topics done</div>
          </div>
          <div class="card-surface p-4">
            <div class="text-xs text-slate-500">Total Revisions</div>
            <div class="text-2xl font-extrabold text-slate-900 mt-1 tabular-nums">{{ ov.totalRevisions }}</div>
          </div>
          <div class="card-surface p-4">
            <div class="text-xs text-slate-500">Mock Tests Logged</div>
            <div class="text-2xl font-extrabold text-slate-900 mt-1 tabular-nums">{{ ov.mockTestsTaken }}</div>
          </div>
          <div class="card-surface p-4">
            <div class="text-xs text-slate-500">Avg Mock Score</div>
            <div class="text-2xl font-extrabold text-green-700 mt-1 tabular-nums">{{ ov.averageMockScorePercent }}%</div>
          </div>
        </div>

        <!-- Study Plans & Timetable + Mock Test Logger & Chart -->
        <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
          <div class="card-surface p-5 space-y-4">
            <h2 class="text-base font-bold text-slate-900">Study Plans & Daily / Weekly Targets</h2>
            @for (plan of ov.studyPlans; track plan.id) {
              <div class="p-3.5 rounded-xl bg-slate-50 border border-slate-200 text-xs space-y-1.5">
                <div class="flex items-center justify-between">
                  <span class="font-bold text-slate-900 text-sm">{{ plan.planTitle }}</span>
                  <span class="px-2 py-0.5 rounded bg-blue-100 text-blue-800 font-semibold">{{ plan.examName }}</span>
                </div>
                <div class="text-slate-600">
                  Daily Target: <strong>{{ plan.dailyTargetMinutes }} mins</strong> · Weekly: <strong>{{ plan.weeklyTargetHours }} hrs</strong>
                  @if (plan.targetExamDate) { · Target Date: <strong>{{ plan.targetExamDate | date:'dd MMM yyyy' }}</strong> }
                </div>
              </div>
            }

            <form (ngSubmit)="savePlan()" class="pt-3 border-t border-slate-100 space-y-3 text-xs">
              <div class="font-bold text-slate-800">Add / Update Study Target</div>
              <div class="grid grid-cols-1 sm:grid-cols-2 gap-2.5">
                <select [(ngModel)]="planForm.examId" name="planExamId" required class="px-3 py-2 border border-slate-200 rounded-lg bg-white">
                  <option value="">Select Exam</option>
                  @for (ex of exams(); track ex.id) { <option [value]="ex.id">{{ ex.shortName }}</option> }
                </select>
                <input type="text" [(ngModel)]="planForm.planTitle" name="planTitle" placeholder="Plan Title" required
                  class="px-3 py-2 border border-slate-200 rounded-lg" />
                <input type="number" [(ngModel)]="planForm.dailyTargetMinutes" name="dailyMins" placeholder="Daily mins"
                  class="px-3 py-2 border border-slate-200 rounded-lg" />
                <input type="number" [(ngModel)]="planForm.weeklyTargetHours" name="weeklyHrs" placeholder="Weekly hrs"
                  class="px-3 py-2 border border-slate-200 rounded-lg" />
              </div>
              <button type="submit" class="px-4 py-2 rounded-lg bg-blue-600 hover:bg-blue-700 text-white font-semibold">Save Study Plan</button>
            </form>
          </div>

          <div class="card-surface p-5 space-y-4">
            <h2 class="text-base font-bold text-slate-900">Mock Test Performance</h2>
            @if (ov.recentMockTests.length > 0) {
              <div class="h-44"><canvas #mockChart></canvas></div>
            }
            <form (ngSubmit)="addMock()" class="pt-2 border-t border-slate-100 space-y-2.5 text-xs">
              <div class="font-bold text-slate-800">Record Mock Test Score</div>
              <div class="grid grid-cols-2 sm:grid-cols-3 gap-2">
                <select [(ngModel)]="mockForm.examId" name="mockExam" required class="px-2.5 py-2 border border-slate-200 rounded-lg bg-white">
                  <option value="">Exam</option>
                  @for (ex of exams(); track ex.id) { <option [value]="ex.id">{{ ex.shortName }}</option> }
                </select>
                <input type="text" [(ngModel)]="mockForm.testTitle" name="testTitle" placeholder="Mock Title" required class="px-2.5 py-2 border border-slate-200 rounded-lg" />
                <input type="date" [(ngModel)]="mockForm.testDate" name="testDate" required class="px-2.5 py-2 border border-slate-200 rounded-lg" />
                <input type="number" step="0.5" [(ngModel)]="mockForm.scoreObtained" name="scoreObtained" placeholder="Score" required class="px-2.5 py-2 border border-slate-200 rounded-lg" />
                <input type="number" [(ngModel)]="mockForm.maxScore" name="maxScore" placeholder="Max Marks" required class="px-2.5 py-2 border border-slate-200 rounded-lg" />
                <input type="number" step="0.5" [(ngModel)]="mockForm.accuracyPercent" name="accuracyPercent" placeholder="Accuracy %" required class="px-2.5 py-2 border border-slate-200 rounded-lg" />
              </div>
              <button type="submit" class="px-4 py-2 rounded-lg bg-[#0b192c] text-white font-semibold">Add Mock Score</button>
            </form>
          </div>
        </div>

        <!-- Application Tracking & Documents Checklist -->
        <div class="card-surface p-5 space-y-4">
          <div class="flex items-center justify-between">
            <h2 class="text-base font-bold text-slate-900">Application Status & Documents Checklist</h2>
          </div>

          <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
            @for (app of ov.applications; track app.id) {
              <div class="p-4 rounded-xl border border-slate-200 bg-slate-50 text-xs space-y-2">
                <div class="flex items-center justify-between">
                  <a [routerLink]="['/exams', app.examSlug]" class="font-bold text-sm text-slate-900 hover:text-blue-600">{{ app.examName }}</a>
                  <span class="px-2 py-0.5 rounded bg-green-100 text-green-800 font-bold">{{ app.trackingStatus }}</span>
                </div>
                @if (app.daysToExam !== null && app.daysToExam !== undefined) {
                  <div class="text-blue-700 font-semibold">Countdown: {{ app.daysToExam }} days to exam</div>
                }
                <div class="text-slate-600">{{ app.notes }}</div>
              </div>
            }
          </div>

          <form (ngSubmit)="saveApplication()" class="pt-3 border-t border-slate-100 grid grid-cols-1 sm:grid-cols-5 gap-2.5 text-xs">
            <select [(ngModel)]="appForm.examId" name="appExam" required class="px-3 py-2 border border-slate-200 rounded-lg bg-white">
              <option value="">Select Exam to Track</option>
              @for (ex of exams(); track ex.id) { <option [value]="ex.id">{{ ex.shortName }}</option> }
            </select>
            <select [(ngModel)]="appForm.trackingStatus" name="appStatus" class="px-3 py-2 border border-slate-200 rounded-lg bg-white">
              <option value="Interested">Interested</option>
              <option value="Preparing">Preparing</option>
              <option value="Applied">Applied</option>
              <option value="AdmitCardDownloaded">Admit Card Downloaded</option>
              <option value="Appeared">Appeared</option>
              <option value="QualifiedStage">Qualified Stage</option>
            </select>
            <input type="text" [(ngModel)]="appForm.applicationReferenceNumber" name="refNo" placeholder="Application Ref No." class="px-3 py-2 border border-slate-200 rounded-lg" />
            <input type="text" [(ngModel)]="appForm.notes" name="appNotes" placeholder="Notes / Document checklist" class="px-3 py-2 border border-slate-200 rounded-lg" />
            <button type="submit" class="px-4 py-2 rounded-lg bg-green-600 hover:bg-green-700 text-white font-semibold">Update Application</button>
          </form>
        </div>
      }
    </div>
  `
})
export class PreparationComponent implements OnInit, AfterViewInit, OnDestroy {
  private readonly api = inject(ApiService);

  @ViewChild('mockChart') mockCanvas?: ElementRef<HTMLCanvasElement>;

  overview = signal<PreparationOverview | null>(null);
  exams = signal<ExamListItem[]>([]);

  planForm = { examId: '', planTitle: '', dailyTargetMinutes: 120, weeklyTargetHours: 16, isActive: true };
  mockForm = { examId: '', testTitle: '', testDate: '2026-11-11', scoreObtained: 120, maxScore: 200, accuracyPercent: 80, timeTakenMinutes: 90 };
  appForm = { examId: '', trackingStatus: 'Applied', applicationReferenceNumber: '', notes: '', documentsChecklistJson: '[]' };

  private chart?: Chart;
  private viewReady = false;

  ngOnInit(): void {
    this.api.searchExams({ pageSize: 100 }).subscribe((res) => this.exams.set(res.items));
    this.loadOverview();
  }

  ngAfterViewInit(): void {
    this.viewReady = true;
    this.renderChart();
  }

  ngOnDestroy(): void {
    this.chart?.destroy();
  }

  loadOverview(): void {
    this.api.getPreparationOverview().subscribe((ov) => {
      this.overview.set(ov);
      setTimeout(() => this.renderChart(), 0);
    });
  }

  savePlan(): void {
    if (!this.planForm.examId) return;
    this.api.upsertStudyPlan(this.planForm).subscribe(() => this.loadOverview());
  }

  addMock(): void {
    if (!this.mockForm.examId) return;
    this.api.addMockTest(this.mockForm).subscribe(() => this.loadOverview());
  }

  saveApplication(): void {
    if (!this.appForm.examId) return;
    this.api.upsertUserApplication(this.appForm).subscribe(() => this.loadOverview());
  }

  private renderChart(): void {
    const ov = this.overview();
    if (!this.viewReady || !ov || !this.mockCanvas || ov.recentMockTests.length === 0) return;

    this.chart?.destroy();
    const chronological = [...ov.recentMockTests].reverse();
    this.chart = new Chart(this.mockCanvas.nativeElement, {
      type: 'line',
      data: {
        labels: chronological.map((m) => m.testTitle),
        datasets: [
          { label: 'Score %', data: chronological.map((m) => m.percentageScore), borderColor: '#2563eb', tension: 0.3 },
          { label: 'Accuracy %', data: chronological.map((m) => m.accuracyPercent), borderColor: '#16a34a', tension: 0.3 }
        ]
      },
      options: { responsive: true, maintainAspectRatio: false }
    });
  }
}
