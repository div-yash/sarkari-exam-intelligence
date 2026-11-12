import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../../core/services/api.service';
import { AuthService } from '../../core/services/auth.service';
import { ExamListItem, StudyProgressStatus, SyllabusSubject, SyllabusTopicProgress } from '../../core/models/api.models';

@Component({
  selector: 'app-syllabus',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="space-y-5">
      <div class="flex flex-col sm:flex-row sm:items-center justify-between gap-3">
        <div>
          <h1 class="text-2xl font-bold text-slate-900">Syllabus & Topic Checklist Library</h1>
          <p class="text-xs text-slate-500 mt-0.5">14 Common Subjects + 12 Core IT/CS Technical Subjects mapped across all government exams.</p>
        </div>
        <div class="flex items-center gap-2 text-xs">
          <select [(ngModel)]="selectedExamId" (ngModelChange)="loadLibrary()" class="px-3 py-2 border border-slate-200 rounded-lg bg-white">
            <option value="">All 26 Library Subjects</option>
            @for (ex of exams(); track ex.id) {
              <option [value]="ex.id">{{ ex.shortName }}</option>
            }
          </select>
          <select [(ngModel)]="groupFilter" class="px-3 py-2 border border-slate-200 rounded-lg bg-white">
            <option value="ALL">All Groups</option>
            <option value="Common">Common Subjects (14)</option>
            <option value="Technical">Technical IT/CS (12)</option>
          </select>
        </div>
      </div>

      <div class="grid grid-cols-1 lg:grid-cols-2 gap-5">
        @for (sub of filteredSubjects(); track sub.id) {
          <div class="card-surface p-5 space-y-4">
            <div class="flex items-start justify-between gap-3">
              <div>
                <span class="px-2 py-0.5 rounded text-[10px] font-bold uppercase"
                  [class]="sub.subjectGroup === 'Technical' ? 'bg-blue-100 text-blue-800' : 'bg-slate-200 text-slate-700'">
                  {{ sub.subjectGroup }}
                </span>
                <h2 class="text-base font-bold text-slate-900 mt-1">{{ sub.name }}</h2>
                <p class="text-xs text-slate-500">{{ sub.description }}</p>
              </div>
              <div class="text-right shrink-0">
                <div class="text-sm font-extrabold text-blue-700 tabular-nums">{{ sub.completedTopics }}/{{ sub.totalTopics }}</div>
                <div class="text-[10px] text-slate-500">Completed</div>
              </div>
            </div>

            <div class="w-full h-2 bg-slate-100 rounded-full overflow-hidden">
              <div class="h-full bg-green-600 rounded-full transition-all"
                [style.width.%]="sub.totalTopics > 0 ? (sub.completedTopics * 100 / sub.totalTopics) : 0"></div>
            </div>

            <div class="divide-y divide-slate-100 max-h-80 overflow-y-auto pr-1">
              @for (topic of sub.topics; track topic.topicId) {
                <div class="py-2.5 flex flex-col sm:flex-row sm:items-center justify-between gap-2 text-xs">
                  <div class="min-w-0">
                    <div class="font-semibold text-slate-800">{{ topic.name }}</div>
                    <div class="text-[11px] text-slate-500">
                      {{ topic.difficultyLevel }} · ~{{ topic.estimatedHours }}h · Revisions: {{ topic.revisionCount }}
                    </div>
                  </div>
                  @if (auth.isAuthenticated()) {
                    <div class="flex items-center gap-1.5 shrink-0">
                      <select [ngModel]="topic.status" (ngModelChange)="updateStatus(topic, $event)"
                        class="px-2 py-1 rounded border border-slate-200 bg-white text-[11px] font-semibold">
                        <option value="NotStarted">Not Started</option>
                        <option value="InProgress">In Progress</option>
                        <option value="Completed">Completed</option>
                      </select>
                      <button (click)="incrementRevision(topic)"
                        class="px-2 py-1 rounded bg-blue-50 hover:bg-blue-100 text-blue-700 font-semibold text-[11px]">
                        +Revise
                      </button>
                    </div>
                  }
                </div>
              }
            </div>
          </div>
        }
      </div>
    </div>
  `
})
export class SyllabusComponent implements OnInit {
  private readonly api = inject(ApiService);
  readonly auth = inject(AuthService);

  subjects = signal<SyllabusSubject[]>([]);
  exams = signal<ExamListItem[]>([]);
  selectedExamId = '';
  groupFilter = 'ALL';

  filteredSubjects = computed(() => {
    const list = this.subjects();
    if (this.groupFilter === 'ALL') return list;
    return list.filter((s) => s.subjectGroup === this.groupFilter);
  });

  ngOnInit(): void {
    this.api.searchExams({ pageSize: 100 }).subscribe((res) => this.exams.set(res.items));
    this.loadLibrary();
  }

  loadLibrary(): void {
    this.api.getSyllabusLibrary(this.selectedExamId || undefined).subscribe((data) => this.subjects.set(data));
  }

  updateStatus(topic: SyllabusTopicProgress, status: StudyProgressStatus): void {
    this.api.updateTopicProgress(topic.topicId, {
      examId: this.selectedExamId || undefined,
      status,
      incrementRevision: false,
      preparationNotes: topic.preparationNotes
    }).subscribe((updated) => {
      topic.status = updated.status;
      this.loadLibrary();
    });
  }

  incrementRevision(topic: SyllabusTopicProgress): void {
    this.api.updateTopicProgress(topic.topicId, {
      examId: this.selectedExamId || undefined,
      status: topic.status === 'NotStarted' ? 'InProgress' : topic.status,
      incrementRevision: true,
      preparationNotes: topic.preparationNotes
    }).subscribe((updated) => {
      topic.revisionCount = updated.revisionCount;
      topic.status = updated.status;
    });
  }
}
