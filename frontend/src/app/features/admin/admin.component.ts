import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../../core/services/api.service';
import { AdminDashboardOverview, ExamListItem } from '../../core/models/api.models';

@Component({
  selector: 'app-admin',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="space-y-6">
      <div class="navy-header rounded-2xl p-6 flex flex-col sm:flex-row sm:items-center justify-between gap-4">
        <div>
          <h1 class="text-2xl font-extrabold">Admin Verification & Data Governance Console</h1>
          <p class="text-xs text-blue-200 mt-1">
            Approve pending updates, verify official government portals, upload notification PDFs, postpone/cancel exams & inspect audit logs.
          </p>
        </div>
        @if (overview(); as ov) {
          <div class="flex gap-4 text-xs">
            <div class="px-3 py-2 rounded-lg bg-white/10"><span class="block text-blue-200">Exams</span><strong class="text-lg">{{ ov.totalExams }}</strong></div>
            <div class="px-3 py-2 rounded-lg bg-white/10"><span class="block text-amber-200">Pending</span><strong class="text-lg">{{ ov.pendingApprovalsCount }}</strong></div>
            <div class="px-3 py-2 rounded-lg bg-white/10"><span class="block text-blue-200">Users</span><strong class="text-lg">{{ ov.totalUsers }}</strong></div>
          </div>
        }
      </div>

      @if (feedback()) {
        <div class="p-3 rounded-lg bg-blue-50 border border-blue-200 text-blue-900 text-xs font-semibold">{{ feedback() }}</div>
      }

      <!-- Tabs -->
      <div class="flex flex-wrap gap-2 text-xs">
        <button (click)="tab.set('manage')" [class]="tabBtn('manage')">Manage Dates, Vacancies & Status</button>
        <button (click)="tab.set('create')" [class]="tabBtn('create')">Add New Examination</button>
        <button (click)="tab.set('sources')" [class]="tabBtn('sources')">Official Source Registry</button>
        <button (click)="tab.set('approvals')" [class]="tabBtn('approvals')">Pending Approvals</button>
        <button (click)="tab.set('users')" [class]="tabBtn('users')">Manage Users</button>
        <button (click)="tab.set('audit')" [class]="tabBtn('audit')">Audit Logs</button>
      </div>

      <!-- Manage Existing Exam Dates / Vacancies / Status / PDF / History -->
      @if (tab() === 'manage') {
        <div class="grid grid-cols-1 lg:grid-cols-2 gap-6 text-xs">
          <div class="card-surface p-5 space-y-3">
            <h2 class="text-sm font-bold text-slate-900">Update Dates, Status & Official PDF</h2>
            <select [(ngModel)]="selectedExamId" class="w-full px-3 py-2 border border-slate-300 rounded-lg bg-white">
              <option value="">Select Exam</option>
              @for (e of exams(); track e.id) { <option [value]="e.id">{{ e.name }}</option> }
            </select>
            <div class="grid grid-cols-2 gap-2.5">
              <div>
                <label class="block font-semibold mb-1">Cycle Year</label>
                <input type="number" [(ngModel)]="dateForm.cycleYear" class="w-full px-3 py-2 border border-slate-300 rounded-lg" />
              </div>
              <div>
                <label class="block font-semibold mb-1">Status</label>
                <select [(ngModel)]="dateForm.status" class="w-full px-3 py-2 border border-slate-300 rounded-lg bg-white">
                  <option value="UpcomingNotification">UpcomingNotification</option>
                  <option value="NotificationReleased">NotificationReleased</option>
                  <option value="ApplicationsOpen">ApplicationsOpen</option>
                  <option value="ClosingSoon">ClosingSoon</option>
                  <option value="ApplicationsClosed">ApplicationsClosed</option>
                  <option value="AdmitCardReleased">AdmitCardReleased</option>
                  <option value="ExaminationScheduled">ExaminationScheduled</option>
                  <option value="ExaminationCompleted">ExaminationCompleted</option>
                  <option value="ResultDeclared">ResultDeclared</option>
                  <option value="Postponed">Postponed</option>
                  <option value="Cancelled">Cancelled</option>
                </select>
              </div>
              <div>
                <label class="block font-semibold mb-1">Notification Date</label>
                <input type="date" [(ngModel)]="dateForm.notificationDate" class="w-full px-3 py-2 border border-slate-300 rounded-lg" />
              </div>
              <div>
                <label class="block font-semibold mb-1">Application End Date</label>
                <input type="date" [(ngModel)]="dateForm.applicationEndDate" class="w-full px-3 py-2 border border-slate-300 rounded-lg" />
              </div>
              <div>
                <label class="block font-semibold mb-1">Exam Date</label>
                <input type="date" [(ngModel)]="dateForm.examDate" class="w-full px-3 py-2 border border-slate-300 rounded-lg" />
              </div>
              <div>
                <label class="block font-semibold mb-1">Upload Official PDF</label>
                <input type="file" accept=".pdf" (change)="onPdfSelected($event)" class="w-full text-[11px]" />
              </div>
            </div>
            <input type="text" [(ngModel)]="dateForm.changeSummary" placeholder="Change summary / audit reason" class="w-full px-3 py-2 border border-slate-300 rounded-lg" />
            <div class="flex flex-wrap gap-2">
              <button (click)="saveDates()" class="px-4 py-2 rounded-lg bg-blue-600 text-white font-semibold">Save Dates</button>
              <button (click)="quickStatus('Postponed')" class="px-3 py-2 rounded-lg bg-amber-600 text-white font-semibold">Mark Postponed</button>
              <button (click)="quickStatus('Cancelled')" class="px-3 py-2 rounded-lg bg-red-600 text-white font-semibold">Mark Cancelled</button>
            </div>
          </div>

          <div class="card-surface p-5 space-y-3">
            <h2 class="text-sm font-bold text-slate-900">Update Vacancies & Historical Records</h2>
            <div class="grid grid-cols-3 gap-2">
              <input type="number" [(ngModel)]="vacForm.totalVacancies" placeholder="Total Vacancies" class="px-3 py-2 border border-slate-300 rounded-lg" />
              <input type="number" [(ngModel)]="vacForm.itSpecificVacancies" placeholder="IT Vacancies" class="px-3 py-2 border border-slate-300 rounded-lg" />
              <select [(ngModel)]="vacForm.vacancyType" class="px-3 py-2 border border-slate-300 rounded-lg bg-white">
                <option value="Official">Official</option>
                <option value="Proposed">Proposed</option>
                <option value="NotAnnounced">NotAnnounced</option>
              </select>
            </div>
            <button (click)="saveVacancies()" class="px-4 py-2 rounded-lg bg-[#0b192c] text-white font-semibold">Update Vacancies</button>

            <div class="pt-3 border-t border-slate-100 space-y-2">
              <div class="font-bold text-slate-800">Add Historical Recruitment Cycle</div>
              <div class="grid grid-cols-3 gap-2">
                <input type="number" [(ngModel)]="histForm.cycleYear" placeholder="Year" class="px-3 py-2 border border-slate-300 rounded-lg" />
                <input type="number" [(ngModel)]="histForm.totalVacancies" placeholder="Vacancies" class="px-3 py-2 border border-slate-300 rounded-lg" />
                <select [(ngModel)]="histForm.dataStatus" class="px-3 py-2 border border-slate-300 rounded-lg bg-white">
                  <option value="Verified">Verified</option>
                  <option value="AwaitingOfficialNotification">AwaitingOfficialNotification</option>
                  <option value="Unavailable">Unavailable</option>
                </select>
              </div>
              <input type="text" [(ngModel)]="histForm.notes" placeholder="Official reference note" class="w-full px-3 py-2 border border-slate-300 rounded-lg" />
              <button (click)="addHistory()" class="px-4 py-2 rounded-lg bg-green-600 text-white font-semibold">Record Historical Cycle</button>
            </div>
          </div>
        </div>
      }

      <!-- Create New Exam -->
      @if (tab() === 'create') {
        <form (ngSubmit)="createNewExam()" class="card-surface p-5 space-y-3 text-xs max-w-3xl">
          <h2 class="text-sm font-bold text-slate-900">Register New Government Examination</h2>
          <div class="grid grid-cols-1 sm:grid-cols-3 gap-3">
            <select [(ngModel)]="newExam.categoryId" name="cat" required class="px-3 py-2 border border-slate-300 rounded-lg bg-white">
              <option value="">Category *</option>
              @for (c of lookups()?.categories ?? []; track c.id) { <option [value]="c.id">{{ c.name }}</option> }
            </select>
            <select [(ngModel)]="newExam.organizationId" name="org" required class="px-3 py-2 border border-slate-300 rounded-lg bg-white">
              <option value="">Organization *</option>
              @for (o of lookups()?.orgs ?? []; track o.id) { <option [value]="o.id">{{ o.shortCode }} – {{ o.name }}</option> }
            </select>
            <select [(ngModel)]="newExam.officialSourceId" name="src" required class="px-3 py-2 border border-slate-300 rounded-lg bg-white">
              <option value="">Official Source *</option>
              @for (s of lookups()?.sources ?? []; track s.id) { <option [value]="s.id">{{ s.sourceName }}</option> }
            </select>
            <input type="text" [(ngModel)]="newExam.name" name="name" placeholder="Full Exam Name *" required class="px-3 py-2 border border-slate-300 rounded-lg" />
            <input type="text" [(ngModel)]="newExam.shortName" name="shortName" placeholder="Short Name *" required class="px-3 py-2 border border-slate-300 rounded-lg" />
            <input type="text" [(ngModel)]="newExam.slug" name="slug" placeholder="url-slug *" required class="px-3 py-2 border border-slate-300 rounded-lg" />
            <input type="text" [(ngModel)]="newExam.code" name="code" placeholder="EXAM-CODE *" required class="px-3 py-2 border border-slate-300 rounded-lg" />
            <input type="text" [(ngModel)]="newExam.department" name="dept" placeholder="Department *" required class="px-3 py-2 border border-slate-300 rounded-lg" />
            <input type="text" [(ngModel)]="newExam.jobDesignation" name="desig" placeholder="Designation *" required class="px-3 py-2 border border-slate-300 rounded-lg" />
          </div>
          <input type="url" [(ngModel)]="newExam.officialRecruitmentWebsite" name="web" placeholder="Official Website URL *" required class="w-full px-3 py-2 border border-slate-300 rounded-lg" />
          <div class="flex gap-4">
            <label class="flex items-center gap-1.5"><input type="checkbox" [(ngModel)]="newExam.isItSpecific" name="isIt" /> IT Specific</label>
            <label class="flex items-center gap-1.5"><input type="checkbox" [(ngModel)]="newExam.isBiharSpecific" name="isBihar" /> Bihar Specific</label>
            <label class="flex items-center gap-1.5"><input type="checkbox" [(ngModel)]="newExam.requireApprovalBeforePublish" name="reqApp" /> Require Admin Approval Before Publish</label>
          </div>
          <button type="submit" class="px-5 py-2 rounded-lg bg-blue-600 text-white font-semibold">Create Exam</button>
        </form>
      }

      <!-- Official Sources Registry -->
      @if (tab() === 'sources' && overview()) {
        <div class="card-surface overflow-x-auto">
          <table class="w-full text-xs text-left">
            <thead class="bg-slate-50 border-b border-slate-200 text-slate-600">
              <tr>
                <th class="py-2.5 px-3">Org</th>
                <th class="py-2.5 px-3">Official Source URL</th>
                <th class="py-2.5 px-3">Status</th>
                <th class="py-2.5 px-3">Last Checked</th>
                <th class="py-2.5 px-3">Action</th>
              </tr>
            </thead>
            <tbody class="divide-y divide-slate-100">
              @for (s of overview()!.sources; track s.id) {
                <tr>
                  <td class="py-2.5 px-3 font-bold">{{ s.organizationCode }}</td>
                  <td class="py-2.5 px-3"><a [href]="s.officialSourceUrl" target="_blank" class="text-blue-600 hover:underline">{{ s.officialSourceUrl }}</a></td>
                  <td class="py-2.5 px-3">{{ s.verificationStatus }}</td>
                  <td class="py-2.5 px-3">{{ s.dateLastCheckedUtc ? (s.dateLastCheckedUtc | date:'short') : 'Never' }}</td>
                  <td class="py-2.5 px-3">
                    <button (click)="verifySource(s.id)" class="px-2.5 py-1 rounded bg-[#0b192c] text-white font-semibold">Verify Source</button>
                  </td>
                </tr>
              }
            </tbody>
          </table>
        </div>
      }

      <!-- Pending Approvals -->
      @if (tab() === 'approvals' && overview()) {
        <div class="card-surface divide-y divide-slate-100 text-xs">
          @if (overview()!.pendingItems.length === 0) {
            <div class="p-8 text-center text-slate-500">All exam records are verified and approved.</div>
          }
          @for (p of overview()!.pendingItems; track p.examId) {
            <div class="p-4 flex items-center justify-between gap-3">
              <div>
                <div class="font-bold text-slate-900">{{ p.examName }} ({{ p.organizationCode }})</div>
                <div class="text-slate-500">{{ p.verificationStatus }} · {{ p.lastChangeSummary || 'Pending approval' }}</div>
              </div>
              <div class="flex gap-2">
                <button (click)="approve(p.examId, true)" class="px-3 py-1.5 rounded bg-green-600 text-white font-semibold">Approve</button>
                <button (click)="approve(p.examId, false)" class="px-3 py-1.5 rounded bg-red-600 text-white font-semibold">Reject</button>
              </div>
            </div>
          }
        </div>
      }

      <!-- Manage Users -->
      @if (tab() === 'users') {
        <div class="card-surface overflow-x-auto">
          <table class="w-full text-xs text-left">
            <thead class="bg-slate-50 border-b border-slate-200 text-slate-600">
              <tr><th class="py-2.5 px-3">Name</th><th class="py-2.5 px-3">Email</th><th class="py-2.5 px-3">Profile</th><th class="py-2.5 px-3">Roles</th><th class="py-2.5 px-3">Status</th></tr>
            </thead>
            <tbody class="divide-y divide-slate-100">
              @for (u of users(); track u.id) {
                <tr>
                  <td class="py-2.5 px-3 font-bold">{{ u.fullName }}</td>
                  <td class="py-2.5 px-3">{{ u.email }}</td>
                  <td class="py-2.5 px-3">{{ u.graduationQualification }} · {{ u.domicileState }}</td>
                  <td class="py-2.5 px-3">{{ u.roles.join(', ') }}</td>
                  <td class="py-2.5 px-3">
                    <button (click)="toggleUser(u)" class="px-2.5 py-1 rounded font-semibold"
                      [class]="u.isActive ? 'bg-green-100 text-green-800' : 'bg-red-100 text-red-800'">
                      {{ u.isActive ? 'Active (Disable)' : 'Disabled (Enable)' }}
                    </button>
                  </td>
                </tr>
              }
            </tbody>
          </table>
        </div>
      }

      <!-- Audit Logs -->
      @if (tab() === 'audit' && overview()) {
        <div class="card-surface overflow-x-auto">
          <table class="w-full text-xs text-left">
            <thead class="bg-slate-50 border-b border-slate-200 text-slate-600">
              <tr><th class="py-2 px-3">Time (UTC)</th><th class="py-2 px-3">Admin</th><th class="py-2 px-3">Action</th><th class="py-2 px-3">Entity</th><th class="py-2 px-3">Reason</th></tr>
            </thead>
            <tbody class="divide-y divide-slate-100">
              @for (log of overview()!.recentAuditLogs; track log.id) {
                <tr>
                  <td class="py-2 px-3 tabular-nums">{{ log.timestampUtc | date:'short' }}</td>
                  <td class="py-2 px-3">{{ log.userEmail }}</td>
                  <td class="py-2 px-3 font-bold">{{ log.action }}</td>
                  <td class="py-2 px-3">{{ log.entityName }}</td>
                  <td class="py-2 px-3 text-slate-600">{{ log.reason || '—' }}</td>
                </tr>
              }
            </tbody>
          </table>
        </div>
      }
    </div>
  `
})
export class AdminComponent implements OnInit {
  private readonly api = inject(ApiService);

  tab = signal<'manage' | 'create' | 'sources' | 'approvals' | 'users' | 'audit'>('manage');
  overview = signal<AdminDashboardOverview | null>(null);
  exams = signal<ExamListItem[]>([]);
  lookups = signal<any>(null);
  users = signal<any[]>([]);
  feedback = signal<string | null>(null);

  selectedExamId = '';
  dateForm = { cycleYear: 2026, status: 'ApplicationsOpen', notificationDate: '', applicationEndDate: '', examDate: '', changeSummary: '' };
  vacForm = { totalVacancies: null as number | null, itSpecificVacancies: null as number | null, vacancyType: 'Official', notes: '' };
  histForm = { cycleYear: 2025, totalVacancies: null as number | null, dataStatus: 'Verified', notes: '' };
  newExam = { categoryId: '', organizationId: '', officialSourceId: '', name: '', shortName: '', slug: '', code: '', department: '', jobDesignation: '', officialRecruitmentWebsite: '', isItSpecific: false, isBiharSpecific: false, requireApprovalBeforePublish: false };

  ngOnInit(): void {
    this.refreshAll();
    this.api.getAdminLookups().subscribe((l) => this.lookups.set(l));
    this.api.getAdminUsers().subscribe((u) => this.users.set(u));
    this.api.searchExams({ pageSize: 100 }).subscribe((res) => this.exams.set(res.items));
  }

  refreshAll(): void {
    this.api.getAdminOverview().subscribe((ov) => this.overview.set(ov));
  }

  tabBtn(t: string): string {
    return this.tab() === t
      ? 'px-3.5 py-2 rounded-lg bg-[#0b192c] text-white font-semibold'
      : 'px-3.5 py-2 rounded-lg bg-white border border-slate-200 text-slate-700 font-medium';
  }

  saveDates(): void {
    if (!this.selectedExamId) return;
    this.api.updateExamDates(this.selectedExamId, this.dateForm).subscribe(() => {
      this.feedback.set('Dates updated & audit logged.');
      this.refreshAll();
    });
  }

  quickStatus(status: string): void {
    if (!this.selectedExamId) return;
    this.api.changeExamStatus(this.selectedExamId, status, `Admin marked exam ${status}`).subscribe(() => {
      this.feedback.set(`Exam marked as ${status}.`);
      this.refreshAll();
    });
  }

  onPdfSelected(ev: Event): void {
    const input = ev.target as HTMLInputElement;
    const file = input.files?.[0];
    if (!file || !this.selectedExamId) return;
    this.api.uploadNotificationPdf(this.selectedExamId, file).subscribe((url) => {
      this.feedback.set(`Notification PDF uploaded: ${url}`);
      this.refreshAll();
    });
  }

  saveVacancies(): void {
    if (!this.selectedExamId) return;
    this.api.updateExamVacancies(this.selectedExamId, this.vacForm).subscribe(() => {
      this.feedback.set('Vacancies updated.');
      this.refreshAll();
    });
  }

  addHistory(): void {
    if (!this.selectedExamId) return;
    this.api.addHistoricalRecord(this.selectedExamId, this.histForm).subscribe({
      next: () => { this.feedback.set('Historical recruitment cycle recorded.'); this.refreshAll(); },
      error: (err) => { this.feedback.set(err?.error?.message || 'Could not add history.'); }
    });
  }

  createNewExam(): void {
    this.api.createExam(this.newExam).subscribe(() => {
      this.feedback.set('New examination registered.');
      this.refreshAll();
      this.api.searchExams({ pageSize: 100 }).subscribe((res) => this.exams.set(res.items));
    });
  }

  verifySource(sourceId: string): void {
    this.api.verifyOfficialSource(sourceId).subscribe((res) => {
      this.feedback.set(`${res.sourceName}: ${res.notes}`);
      this.refreshAll();
    });
  }

  approve(examId: string, approve: boolean): void {
    this.api.approveExamRecord(examId, approve, approve ? 'Approved by admin' : 'Rejected by admin').subscribe(() => {
      this.feedback.set(approve ? 'Record approved for public display.' : 'Record rejected.');
      this.refreshAll();
    });
  }

  toggleUser(u: any): void {
    this.api.toggleUserActive(u.id, !u.isActive).subscribe(() => {
      u.isActive = !u.isActive;
    });
  }
}
