import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { EligibilityResultStatus, ExamStatus } from '../../core/models/api.models';

@Component({
  selector: 'app-status-badge',
  standalone: true,
  imports: [CommonModule],
  template: `
    <span [class]="badgeClasses" class="inline-flex items-center gap-1.5 px-2.5 py-0.5 rounded-full text-xs font-semibold border whitespace-nowrap">
      <span class="w-1.5 h-1.5 rounded-full" [class]="dotClass"></span>
      {{ label }}
    </span>
  `
})
export class StatusBadgeComponent {
  @Input({ required: true }) status!: ExamStatus;

  get label(): string {
    switch (this.status) {
      case 'UpcomingNotification': return 'Upcoming Notification';
      case 'NotificationReleased': return 'Notification Released';
      case 'ApplicationsOpen': return 'Applications Open';
      case 'ClosingSoon': return 'Closing Soon';
      case 'ApplicationsClosed': return 'Applications Closed';
      case 'AdmitCardReleased': return 'Admit Card Released';
      case 'ExaminationScheduled': return 'Exam Scheduled';
      case 'ExaminationCompleted': return 'Exam Completed';
      case 'ResultDeclared': return 'Result Declared';
      case 'Postponed': return 'Postponed';
      case 'Cancelled': return 'Cancelled';
      default: return this.status;
    }
  }

  get badgeClasses(): string {
    switch (this.status) {
      case 'ApplicationsOpen': return 'bg-green-50 text-green-700 border-green-200';
      case 'ClosingSoon': return 'bg-red-50 text-red-700 border-red-300 animate-pulse';
      case 'UpcomingNotification': return 'bg-orange-50 text-orange-700 border-orange-200';
      case 'NotificationReleased': return 'bg-sky-50 text-sky-700 border-sky-200';
      case 'ApplicationsClosed': return 'bg-slate-100 text-slate-600 border-slate-300';
      case 'AdmitCardReleased': return 'bg-indigo-50 text-indigo-700 border-indigo-200';
      case 'ExaminationScheduled': return 'bg-blue-50 text-blue-700 border-blue-200';
      case 'ExaminationCompleted': return 'bg-purple-50 text-purple-700 border-purple-200';
      case 'ResultDeclared': return 'bg-emerald-50 text-emerald-800 border-emerald-300';
      case 'Postponed': return 'bg-amber-50 text-amber-800 border-amber-300';
      case 'Cancelled': return 'bg-rose-100 text-rose-900 border-rose-300';
      default: return 'bg-slate-100 text-slate-700 border-slate-200';
    }
  }

  get dotClass(): string {
    switch (this.status) {
      case 'ApplicationsOpen': return 'bg-green-600';
      case 'ClosingSoon': return 'bg-red-600';
      case 'UpcomingNotification': return 'bg-orange-500';
      case 'NotificationReleased': return 'bg-sky-600';
      case 'ApplicationsClosed': return 'bg-slate-500';
      case 'AdmitCardReleased': return 'bg-indigo-600';
      case 'ExaminationScheduled': return 'bg-blue-600';
      case 'ExaminationCompleted': return 'bg-purple-600';
      case 'ResultDeclared': return 'bg-emerald-600';
      case 'Postponed': return 'bg-amber-600';
      case 'Cancelled': return 'bg-rose-700';
      default: return 'bg-slate-400';
    }
  }
}

@Component({
  selector: 'app-eligibility-badge',
  standalone: true,
  imports: [CommonModule],
  template: `
    <span [class]="classes" class="inline-flex items-center gap-1 px-2.5 py-0.5 rounded-md text-xs font-semibold border whitespace-nowrap">
      {{ text }}
    </span>
  `
})
export class EligibilityBadgeComponent {
  @Input() status?: EligibilityResultStatus;

  get text(): string {
    if (this.status === 'Eligible') return 'Eligible';
    if (this.status === 'PotentiallyEligibleVerificationRequired') return 'Potentially Eligible – Verify';
    if (this.status === 'NotEligible') return 'Not Eligible';
    return 'Sign in to check';
  }

  get classes(): string {
    if (this.status === 'Eligible') return 'bg-green-50 text-green-700 border-green-200';
    if (this.status === 'PotentiallyEligibleVerificationRequired') return 'bg-amber-50 text-amber-800 border-amber-200';
    if (this.status === 'NotEligible') return 'bg-rose-50 text-rose-700 border-rose-200';
    return 'bg-slate-50 text-slate-600 border-slate-200';
  }
}
