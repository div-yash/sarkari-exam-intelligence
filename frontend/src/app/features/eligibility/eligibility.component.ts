import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { ApiService } from '../../core/services/api.service';
import { AuthService } from '../../core/services/auth.service';
import { EligibilityCheckResult, EligibilityResultStatus, EligibilitySummary } from '../../core/models/api.models';
import { EligibilityBadgeComponent, StatusBadgeComponent } from '../../shared/components/status-badge.component';

@Component({
  selector: 'app-eligibility',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, EligibilityBadgeComponent, StatusBadgeComponent],
  template: `
    <div class="space-y-6">
      <div class="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-3">
        <div>
          <h1 class="text-2xl font-bold text-slate-900">Intelligent Eligibility Checker</h1>
          <p class="text-xs text-slate-500 mt-0.5">
            Validates exact engineering branch, marks, age on notification cutoff date, supported relaxation & Bihar domicile rules.
          </p>
        </div>
        @if (auth.isAuthenticated()) {
          <button (click)="showSimulator.set(!showSimulator())"
            class="px-4 py-2 rounded-lg border border-slate-300 bg-white text-xs font-semibold text-slate-700 hover:bg-slate-50">
            {{ showSimulator() ? 'Use My Saved Profile' : 'Test Custom Candidate Profile' }}
          </button>
        }
      </div>

      <!-- Ad-Hoc / Guest Candidate Profile Evaluator -->
      @if (!auth.isAuthenticated() || showSimulator()) {
        <div class="card-surface p-5 space-y-4">
          <div class="flex items-center justify-between">
            <h2 class="text-sm font-bold text-slate-900">Evaluate Any Indian Candidate Profile</h2>
            <span class="text-xs text-slate-500">Default sample: B.Tech IT Graduate from Bihar</span>
          </div>
          <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-3 text-xs">
            <div>
              <label class="block font-semibold text-slate-700 mb-1">Date of Birth</label>
              <input type="date" [(ngModel)]="adHoc.dateOfBirth" class="w-full px-3 py-2 border border-slate-300 rounded-lg" />
            </div>
            <div>
              <label class="block font-semibold text-slate-700 mb-1">Category</label>
              <select [(ngModel)]="adHoc.category" class="w-full px-3 py-2 border border-slate-300 rounded-lg bg-white">
                <option value="General">General</option>
                <option value="EWS">EWS</option>
                <option value="OBCNCL">OBC-NCL</option>
                <option value="SC">SC</option>
                <option value="ST">ST</option>
              </select>
            </div>
            <div>
              <label class="block font-semibold text-slate-700 mb-1">Domicile State</label>
              <input type="text" [(ngModel)]="adHoc.domicileState" class="w-full px-3 py-2 border border-slate-300 rounded-lg" />
            </div>
            <div>
              <label class="block font-semibold text-slate-700 mb-1">Qualification</label>
              <select [(ngModel)]="adHoc.graduationQualification" class="w-full px-3 py-2 border border-slate-300 rounded-lg bg-white">
                <option value="B.Tech">B.Tech</option>
                <option value="B.E.">B.E.</option>
                <option value="B.Sc">B.Sc</option>
                <option value="B.Com">B.Com</option>
                <option value="B.A.">B.A.</option>
                <option value="MCA">MCA</option>
                <option value="M.Tech">M.Tech</option>
                <option value="12th">12th</option>
              </select>
            </div>
            <div>
              <label class="block font-semibold text-slate-700 mb-1">Engineering Branch</label>
              <input type="text" [(ngModel)]="adHoc.engineeringBranch" class="w-full px-3 py-2 border border-slate-300 rounded-lg" />
            </div>
            <div>
              <label class="block font-semibold text-slate-700 mb-1">Percentage (%)</label>
              <input type="number" step="0.1" [(ngModel)]="adHoc.graduationPercentage" class="w-full px-3 py-2 border border-slate-300 rounded-lg" />
            </div>
            <div>
              <label class="block font-semibold text-slate-700 mb-1">Work Experience (Months)</label>
              <input type="number" [(ngModel)]="adHoc.workExperienceMonths" class="w-full px-3 py-2 border border-slate-300 rounded-lg" />
            </div>
            <div class="flex flex-col justify-end gap-1.5">
              <label class="flex items-center gap-1.5"><input type="checkbox" [(ngModel)]="adHoc.hasValidGateScore" /> Valid GATE CS Score</label>
              <label class="flex items-center gap-1.5"><input type="checkbox" [(ngModel)]="adHoc.hasBiharDomicileCertificate" /> Domicile & NCL/EWS Cert</label>
            </div>
          </div>
          <button (click)="runAdHoc()" class="px-5 py-2 rounded-lg bg-blue-600 hover:bg-blue-700 text-white text-xs font-semibold">
            Run Eligibility Check Across All Exams
          </button>
        </div>
      }

      @if (summary()) {
        <!-- Summary Metrics -->
        <div class="grid grid-cols-2 md:grid-cols-4 gap-4">
          <button (click)="filterStatus.set('ALL')" class="card-surface p-4 text-left hover:border-blue-300">
            <div class="text-xs text-slate-500">Total Evaluated</div>
            <div class="text-2xl font-extrabold text-slate-900 mt-1 tabular-nums">{{ summary()!.totalExamsEvaluated }}</div>
          </button>
          <button (click)="filterStatus.set('Eligible')" class="card-surface p-4 text-left border-l-4 border-l-green-600">
            <div class="text-xs font-medium text-green-700">Eligible</div>
            <div class="text-2xl font-extrabold text-green-700 mt-1 tabular-nums">{{ summary()!.eligibleCount }}</div>
          </button>
          <button (click)="filterStatus.set('PotentiallyEligibleVerificationRequired')" class="card-surface p-4 text-left border-l-4 border-l-amber-500">
            <div class="text-xs font-medium text-amber-700">Potentially Eligible (Verify)</div>
            <div class="text-2xl font-extrabold text-amber-700 mt-1 tabular-nums">{{ summary()!.potentiallyEligibleCount }}</div>
          </button>
          <button (click)="filterStatus.set('NotEligible')" class="card-surface p-4 text-left border-l-4 border-l-red-600">
            <div class="text-xs font-medium text-red-700">Not Eligible</div>
            <div class="text-2xl font-extrabold text-red-700 mt-1 tabular-nums">{{ summary()!.notEligibleCount }}</div>
          </button>
        </div>

        <!-- Results List -->
        <div class="space-y-3">
          @for (item of filteredResults(); track item.examId) {
            <div class="card-surface p-4 space-y-3">
              <div class="flex flex-col sm:flex-row sm:items-center justify-between gap-3">
                <div>
                  <div class="flex items-center gap-2">
                    <span class="px-2 py-0.5 rounded bg-[#0b192c] text-white text-[11px] font-bold">{{ item.organizationCode }}</span>
                    <a [routerLink]="['/exams', item.examSlug]" class="text-base font-bold text-slate-900 hover:text-blue-600">{{ item.examName }}</a>
                  </div>
                  <p class="text-xs text-slate-600 mt-1">{{ item.summary }}</p>
                </div>
                <div class="flex items-center gap-2 shrink-0">
                  <app-eligibility-badge [status]="item.status"></app-eligibility-badge>
                  <app-status-badge [status]="item.currentStatus"></app-status-badge>
                </div>
              </div>

              <div class="grid grid-cols-1 md:grid-cols-2 gap-3 pt-2 border-t border-slate-100 text-xs">
                <div class="bg-slate-50 rounded-lg p-2.5">
                  <div><strong>Cutoff Age:</strong> {{ item.ageEvaluation.calculatedAgeText }}</div>
                  <div class="text-slate-600">{{ item.ageEvaluation.relaxationExplanation }}</div>
                </div>
                <div class="space-y-1">
                  @for (c of item.criteriaChecks; track c.criterion) {
                    <div class="flex items-start gap-1.5">
                      <span>{{ c.passed ? (c.requiresVerification ? '⚠️' : '✅') : '❌' }}</span>
                      <span><strong>{{ c.criterion }}:</strong> {{ c.detail }}</span>
                    </div>
                  }
                </div>
              </div>
            </div>
          }
        </div>
      }
    </div>
  `
})
export class EligibilityComponent implements OnInit {
  private readonly api = inject(ApiService);
  readonly auth = inject(AuthService);

  summary = signal<EligibilitySummary | null>(null);
  showSimulator = signal(false);
  filterStatus = signal<'ALL' | EligibilityResultStatus>('ALL');

  adHoc = {
    dateOfBirth: '1999-06-15',
    gender: 'Male',
    category: 'OBCNCL',
    domicileState: 'Bihar',
    graduationQualification: 'B.Tech',
    engineeringBranch: 'Information Technology',
    graduationPercentage: 78.4 as number | null,
    workExperienceMonths: 36,
    hasValidGateScore: false,
    gateSubject: 'CS',
    gateScore: 550 as number | null,
    hasBiharDomicileCertificate: true,
    hasValidNclOrEwsCertificate: true,
    nationality: 'Indian'
  };

  filteredResults = computed<EligibilityCheckResult[]>(() => {
    const s = this.summary();
    if (!s) return [];
    const f = this.filterStatus();
    if (f === 'ALL') return s.results;
    return s.results.filter((r) => r.status === f);
  });

  ngOnInit(): void {
    if (this.auth.isAuthenticated()) {
      this.api.getMyEligibility().subscribe((s) => this.summary.set(s));
    } else {
      this.runAdHoc();
    }
  }

  runAdHoc(): void {
    const payload = { ...this.adHoc, hasValidNclOrEwsCertificate: this.adHoc.hasBiharDomicileCertificate };
    this.api.evaluateAdHocEligibility(payload).subscribe((s) => this.summary.set(s));
  }
}
