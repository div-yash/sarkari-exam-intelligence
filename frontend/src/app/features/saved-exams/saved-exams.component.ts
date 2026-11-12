import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { ApiService } from '../../core/services/api.service';
import { SavedExam } from '../../core/models/api.models';
import { StatusBadgeComponent } from '../../shared/components/status-badge.component';

@Component({
  selector: 'app-saved-exams',
  standalone: true,
  imports: [CommonModule, RouterLink, StatusBadgeComponent],
  template: `
    <div class="space-y-5">
      <div>
        <h1 class="text-2xl font-bold text-slate-900">Saved Examinations Watchlist</h1>
        <p class="text-xs text-slate-500 mt-0.5">Exams saved for deadline tracking, alerts, and preparation focus.</p>
      </div>

      @if (items().length === 0) {
        <div class="card-surface p-8 text-center text-sm text-slate-600">
          No saved examinations yet. Browse <a routerLink="/exams" class="text-blue-600 font-semibold underline">All Exams</a> to add to your watchlist.
        </div>
      } @else {
        <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
          @for (item of items(); track item.id) {
            <div class="card-surface p-4 flex flex-col justify-between gap-3">
              <div>
                <div class="flex items-start justify-between gap-2">
                  <div>
                    <span class="text-xs font-bold text-blue-700">{{ item.organizationCode }} · {{ item.categoryName }}</span>
                    <a [routerLink]="['/exams', item.examSlug]" class="block text-base font-bold text-slate-900 hover:text-blue-600 mt-0.5">{{ item.examName }}</a>
                  </div>
                  <app-status-badge [status]="item.status"></app-status-badge>
                </div>
                <p class="text-xs text-slate-500 mt-2">{{ item.personalNotes || 'Priority: ' + item.priorityLevel }}</p>
              </div>
              <div class="flex items-center justify-between pt-3 border-t border-slate-100 text-xs">
                <span class="text-slate-600 tabular-nums">
                  Deadline: {{ item.applicationEndDate ? (item.applicationEndDate | date:'dd MMM yyyy') : 'Not Announced' }}
                </span>
                <button (click)="remove(item.examId)" class="text-red-600 hover:underline font-semibold">Remove</button>
              </div>
            </div>
          }
        </div>
      }
    </div>
  `
})
export class SavedExamsComponent implements OnInit {
  private readonly api = inject(ApiService);
  items = signal<SavedExam[]>([]);

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.api.getSavedExams().subscribe((d) => this.items.set(d));
  }

  remove(examId: string): void {
    this.api.removeSavedExam(examId).subscribe(() => {
      this.items.set(this.items().filter((x) => x.examId !== examId));
    });
  }
}
