import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { ApiService } from '../../core/services/api.service';
import { NotificationItem } from '../../core/models/api.models';

@Component({
  selector: 'app-notifications',
  standalone: true,
  imports: [CommonModule, RouterLink],
  template: `
    <div class="space-y-5">
      <div class="flex flex-col sm:flex-row sm:items-center justify-between gap-3">
        <div>
          <h1 class="text-2xl font-bold text-slate-900">Notifications & Deadline Alerts</h1>
          <p class="text-xs text-slate-500 mt-0.5">Deduplicated alerts for new notifications, 30d/7d/3d/1d deadlines, date changes, admit cards & results.</p>
        </div>
        <div class="flex gap-2 text-xs">
          <button (click)="runCheck()" class="px-3.5 py-2 rounded-lg bg-blue-600 hover:bg-blue-700 text-white font-semibold">
            Run Alert Scan Now
          </button>
          <button (click)="markAll()" class="px-3.5 py-2 rounded-lg border border-slate-200 bg-white hover:bg-slate-50 font-semibold text-slate-700">
            Mark All Read
          </button>
          <a routerLink="/settings" class="px-3.5 py-2 rounded-lg bg-[#0b192c] text-white font-semibold">
            Alert Preferences
          </a>
        </div>
      </div>

      @if (statusMsg()) {
        <div class="p-3 rounded-lg bg-blue-50 border border-blue-200 text-blue-800 text-xs font-medium">{{ statusMsg() }}</div>
      }

      <div class="card-surface divide-y divide-slate-100">
        @if (items().length === 0) {
          <div class="p-8 text-center text-xs text-slate-500">No notifications yet. Save exams or complete your profile and click "Run Alert Scan Now".</div>
        }
        @for (n of items(); track n.id) {
          <div class="p-4 flex flex-col sm:flex-row sm:items-center justify-between gap-3" [class.bg-blue-50/40]="!n.isRead">
            <div class="space-y-1">
              <div class="flex items-center gap-2">
                @if (!n.isRead) { <span class="w-2 h-2 rounded-full bg-blue-600"></span> }
                <span class="text-xs font-bold uppercase tracking-wider text-slate-500">{{ n.notificationType }}</span>
                <span class="text-[11px] text-slate-400">· {{ n.scheduledForUtc | date:'medium' }}</span>
              </div>
              <div class="text-sm font-bold text-slate-900">{{ n.title }}</div>
              <div class="text-xs text-slate-600">{{ n.message }}</div>
            </div>
            <div class="flex items-center gap-2 shrink-0 text-xs">
              @if (n.examSlug) {
                <a [routerLink]="['/exams', n.examSlug]" class="px-3 py-1.5 rounded-lg bg-slate-100 hover:bg-slate-200 font-semibold text-slate-700">View Exam</a>
              }
              @if (!n.isRead) {
                <button (click)="markOne(n)" class="px-3 py-1.5 rounded-lg bg-blue-600 text-white font-semibold">Mark Read</button>
              }
            </div>
          </div>
        }
      </div>
    </div>
  `
})
export class NotificationsComponent implements OnInit {
  private readonly api = inject(ApiService);

  items = signal<NotificationItem[]>([]);
  statusMsg = signal<string | null>(null);

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.api.getNotifications(false).subscribe((d) => this.items.set(d));
  }

  runCheck(): void {
    this.api.runNotificationCheck().subscribe((count) => {
      this.statusMsg.set(`Alert scan completed: ${count} new deduplicated alert(s) created.`);
      this.load();
    });
  }

  markOne(n: NotificationItem): void {
    this.api.markNotificationRead(n.id).subscribe(() => { n.isRead = true; });
  }

  markAll(): void {
    this.api.markAllNotificationsRead().subscribe(() => {
      this.items.set(this.items().map((x) => ({ ...x, isRead: true })));
    });
  }
}
