import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { ApiService } from '../../core/services/api.service';
import { environment } from '../../../environments/environment';
import { CalendarEvent, CategorySummary } from '../../core/models/api.models';
import { StatusBadgeComponent } from '../../shared/components/status-badge.component';

@Component({
  selector: 'app-calendar',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, StatusBadgeComponent],
  template: `
    <div class="space-y-5">
      <div class="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-3">
        <div>
          <h1 class="text-2xl font-bold text-slate-900">Government Exam & Deadline Calendar</h1>
          <p class="text-xs text-slate-500 mt-0.5">Monthly, weekly, and list views of openings, deadlines, admit cards & exam dates.</p>
        </div>
        <a [href]="exportUrl()" target="_blank" class="px-4 py-2 rounded-lg bg-[#0b192c] hover:bg-[#17345c] text-white text-xs font-semibold">
          ⬇ Export .ICS Calendar
        </a>
      </div>

      <!-- Controls -->
      <div class="card-surface p-4 flex flex-wrap items-center justify-between gap-3 text-xs">
        <div class="flex items-center gap-2">
          <button (click)="viewMode.set('month')" [class]="tabClass(viewMode() === 'month')">Monthly</button>
          <button (click)="viewMode.set('week')" [class]="tabClass(viewMode() === 'week')">Weekly</button>
          <button (click)="viewMode.set('list')" [class]="tabClass(viewMode() === 'list')">List View</button>
        </div>

        <div class="flex flex-wrap items-center gap-3">
          <select [(ngModel)]="selectedCategory" (ngModelChange)="loadEvents()" class="px-3 py-1.5 border border-slate-200 rounded-lg bg-white">
            <option value="">All Exam Categories</option>
            @for (c of categories(); track c.id) {
              <option [value]="c.slug">{{ c.name }}</option>
            }
          </select>
          <label class="flex items-center gap-1.5 font-medium text-slate-700">
            <input type="checkbox" [(ngModel)]="onlyConfirmed" (ngModelChange)="loadEvents()" />
            Confirmed Dates Only
          </label>
          <div class="flex items-center gap-1.5">
            <button (click)="shiftPeriod(-1)" class="px-2.5 py-1 border border-slate-200 rounded hover:bg-slate-50">◀</button>
            <span class="font-bold text-slate-800 px-2 min-w-32 text-center">{{ currentMonthLabel() }}</span>
            <button (click)="shiftPeriod(1)" class="px-2.5 py-1 border border-slate-200 rounded hover:bg-slate-50">▶</button>
          </div>
        </div>
      </div>

      @if (viewMode() === 'month') {
        <div class="card-surface overflow-hidden">
          <div class="grid grid-cols-7 bg-[#0b192c] text-white text-xs font-semibold text-center py-2.5">
            <div>Sun</div><div>Mon</div><div>Tue</div><div>Wed</div><div>Thu</div><div>Fri</div><div>Sat</div>
          </div>
          <div class="grid grid-cols-7 divide-x divide-y divide-slate-200">
            @for (cell of monthCells(); track cell.dateStr) {
              <div class="min-h-24 p-2 bg-white" [class.bg-slate-50]="!cell.inCurrentMonth">
                <div class="text-xs font-bold mb-1 tabular-nums" [class.text-blue-600]="cell.isToday" [class.text-slate-400]="!cell.inCurrentMonth">
                  {{ cell.day }}
                </div>
                <div class="space-y-1">
                  @for (ev of cell.events.slice(0, 3); track $index) {
                    <a [routerLink]="['/exams', ev.examSlug]"
                      [class]="eventPillClass(ev)"
                      class="block px-1.5 py-0.5 rounded text-[10px] font-semibold truncate">
                      {{ ev.examName }}
                    </a>
                  }
                  @if (cell.events.length > 3) {
                    <div class="text-[10px] text-slate-500 font-medium">+{{ cell.events.length - 3 }} more</div>
                  }
                </div>
              </div>
            }
          </div>
        </div>
      } @else if (viewMode() === 'week') {
        <div class="grid grid-cols-1 md:grid-cols-7 gap-3">
          @for (day of weekDays(); track day.dateStr) {
            <div class="card-surface p-3 min-h-48">
              <div class="text-xs font-bold pb-2 mb-2 border-b border-slate-100" [class.text-blue-600]="day.isToday">
                {{ day.label }}
              </div>
              <div class="space-y-2">
                @for (ev of day.events; track $index) {
                  <a [routerLink]="['/exams', ev.examSlug]" [class]="eventPillClass(ev)" class="block p-2 rounded-lg text-xs font-semibold">
                    <div>{{ ev.examName }}</div>
                    <div class="text-[10px] opacity-80">{{ ev.eventType }}</div>
                  </a>
                }
                @if (day.events.length === 0) {
                  <div class="text-[11px] text-slate-400">No events</div>
                }
              </div>
            </div>
          }
        </div>
      } @else {
        <div class="card-surface divide-y divide-slate-100">
          @if (events().length === 0) {
            <div class="p-8 text-center text-xs text-slate-500">No scheduled events match your filters.</div>
          }
          @for (ev of events(); track $index) {
            <div class="p-4 flex flex-col sm:flex-row sm:items-center justify-between gap-3">
              <div class="flex items-center gap-4">
                <div class="w-16 text-center p-2 rounded-lg bg-slate-100">
                  <div class="text-xs font-extrabold text-slate-900 tabular-nums">{{ ev.eventDate | date:'dd MMM' }}</div>
                  <div class="text-[10px] text-slate-500 tabular-nums">{{ ev.eventDate | date:'yyyy' }}</div>
                </div>
                <div>
                  <a [routerLink]="['/exams', ev.examSlug]" class="text-sm font-bold text-slate-900 hover:text-blue-600">{{ ev.title }}</a>
                  <div class="text-xs text-slate-500">{{ ev.categoryName }} · {{ ev.organizationCode }} · {{ ev.isExpected ? 'Expected Date' : 'Confirmed Date' }}</div>
                </div>
              </div>
              <app-status-badge [status]="ev.examStatus"></app-status-badge>
            </div>
          }
        </div>
      }
    </div>
  `
})
export class CalendarComponent implements OnInit {
  private readonly api = inject(ApiService);

  viewMode = signal<'month' | 'week' | 'list'>('month');
  categories = signal<CategorySummary[]>([]);
  events = signal<CalendarEvent[]>([]);
  selectedCategory = '';
  onlyConfirmed = false;
  anchorDate = signal(new Date(2026, 10, 11)); // Nov 2026 default

  currentMonthLabel = computed(() =>
    this.anchorDate().toLocaleDateString('en-IN', { month: 'long', year: 'numeric' })
  );

  monthCells = computed(() => {
    const d = this.anchorDate();
    const year = d.getFullYear();
    const month = d.getMonth();
    const first = new Date(year, month, 1);
    const startOffset = first.getDay();
    const start = new Date(year, month, 1 - startOffset);
    const todayStr = this.toIso(new Date());

    const map = new Map<string, CalendarEvent[]>();
    for (const ev of this.events()) {
      const list = map.get(ev.eventDate) ?? [];
      list.push(ev);
      map.set(ev.eventDate, list);
    }

    const cells: Array<{ day: number; dateStr: string; inCurrentMonth: boolean; isToday: boolean; events: CalendarEvent[] }> = [];
    for (let i = 0; i < 42; i++) {
      const cur = new Date(start.getFullYear(), start.getMonth(), start.getDate() + i);
      const iso = this.toIso(cur);
      cells.push({
        day: cur.getDate(),
        dateStr: iso,
        inCurrentMonth: cur.getMonth() === month,
        isToday: iso === todayStr,
        events: map.get(iso) ?? []
      });
    }
    return cells;
  });

  weekDays = computed(() => {
    const d = this.anchorDate();
    const start = new Date(d.getFullYear(), d.getMonth(), d.getDate() - d.getDay());
    const todayStr = this.toIso(new Date());
    const map = new Map<string, CalendarEvent[]>();
    for (const ev of this.events()) {
      const list = map.get(ev.eventDate) ?? [];
      list.push(ev);
      map.set(ev.eventDate, list);
    }
    return Array.from({ length: 7 }, (_, i) => {
      const cur = new Date(start.getFullYear(), start.getMonth(), start.getDate() + i);
      const iso = this.toIso(cur);
      return {
        dateStr: iso,
        label: cur.toLocaleDateString('en-IN', { weekday: 'short', day: '2-digit', month: 'short' }),
        isToday: iso === todayStr,
        events: map.get(iso) ?? []
      };
    });
  });

  ngOnInit(): void {
    this.api.getCategories().subscribe((c) => this.categories.set(c));
    this.loadEvents();
  }

  loadEvents(): void {
    this.api.getCalendarEvents({
      categorySlug: this.selectedCategory || undefined,
      onlyConfirmed: this.onlyConfirmed
    }).subscribe((evs) => this.events.set(evs));
  }

  shiftPeriod(delta: number): void {
    const cur = this.anchorDate();
    if (this.viewMode() === 'week') {
      this.anchorDate.set(new Date(cur.getFullYear(), cur.getMonth(), cur.getDate() + delta * 7));
    } else {
      this.anchorDate.set(new Date(cur.getFullYear(), cur.getMonth() + delta, 1));
    }
  }

  exportUrl(): string {
    const qs = new URLSearchParams();
    if (this.selectedCategory) qs.set('categorySlug', this.selectedCategory);
    if (this.onlyConfirmed) qs.set('onlyConfirmed', 'true');
    const base = environment.apiBaseUrl?.replace(/\/+$/, '') ?? '';
    return `${base}/api/calendar/export.ics?${qs.toString()}`;
  }

  tabClass(active: boolean): string {
    return active
      ? 'px-3 py-1.5 rounded-lg bg-[#0b192c] text-white font-semibold'
      : 'px-3 py-1.5 rounded-lg bg-slate-100 text-slate-700 hover:bg-slate-200 font-medium';
  }

  eventPillClass(ev: CalendarEvent): string {
    if (ev.eventType === 'ApplicationDeadline') return 'bg-red-100 text-red-800';
    if (ev.eventType === 'ApplicationOpen') return 'bg-green-100 text-green-800';
    if (ev.eventType === 'NotificationRelease') return 'bg-orange-100 text-orange-800';
    return 'bg-blue-100 text-blue-800';
  }

  private toIso(d: Date): string {
    const m = String(d.getMonth() + 1).padStart(2, '0');
    const day = String(d.getDate()).padStart(2, '0');
    return `${d.getFullYear()}-${m}-${day}`;
  }
}
