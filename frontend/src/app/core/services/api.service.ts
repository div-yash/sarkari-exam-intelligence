import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import {
  AdminDashboardOverview,
  ApiResponse,
  CalendarEvent,
  CategorySummary,
  DashboardSummary,
  EligibilityCheckResult,
  EligibilitySummary,
  ExamDetail,
  ExamListItem,
  MockTestResult,
  NotificationItem,
  NotificationPreference,
  PagedResult,
  PreparationOverview,
  SavedExam,
  StudyPlan,
  SyllabusSubject,
  SyllabusTopicProgress,
  UserApplication,
  VacancyAnalytics
} from '../models/api.models';

@Injectable({ providedIn: 'root' })
export class ApiService {
  constructor(private readonly http: HttpClient) {}

  // Dashboard
  getDashboardSummary(): Observable<DashboardSummary> {
    return this.http.get<ApiResponse<DashboardSummary>>('/api/dashboard/summary').pipe(map((r) => r.data));
  }

  // Exams
  getCategories(): Observable<CategorySummary[]> {
    return this.http.get<ApiResponse<CategorySummary[]>>('/api/exams/categories').pipe(map((r) => r.data));
  }

  searchExams(filters: Record<string, string | number | boolean | undefined | null>): Observable<PagedResult<ExamListItem>> {
    let params = new HttpParams();
    for (const [key, val] of Object.entries(filters)) {
      if (val !== undefined && val !== null && val !== '') {
        params = params.set(key, String(val));
      }
    }
    return this.http.get<ApiResponse<PagedResult<ExamListItem>>>('/api/exams', { params }).pipe(map((r) => r.data));
  }

  getExamDetail(slug: string): Observable<ExamDetail> {
    return this.http.get<ApiResponse<ExamDetail>>(`/api/exams/${encodeURIComponent(slug)}`).pipe(map((r) => r.data));
  }

  // Eligibility
  getMyEligibility(): Observable<EligibilitySummary> {
    return this.http.get<ApiResponse<EligibilitySummary>>('/api/eligibility/my').pipe(map((r) => r.data));
  }

  getMyExamEligibility(examId: string): Observable<EligibilityCheckResult> {
    return this.http.get<ApiResponse<EligibilityCheckResult>>(`/api/eligibility/my/${examId}`).pipe(map((r) => r.data));
  }

  evaluateAdHocEligibility(profile: Record<string, unknown>): Observable<EligibilitySummary> {
    return this.http.post<ApiResponse<EligibilitySummary>>('/api/eligibility/evaluate', profile).pipe(map((r) => r.data));
  }

  // Calendar
  getCalendarEvents(filters: Record<string, string | boolean | undefined | null>): Observable<CalendarEvent[]> {
    let params = new HttpParams();
    for (const [k, v] of Object.entries(filters)) {
      if (v !== undefined && v !== null && v !== '') params = params.set(k, String(v));
    }
    return this.http.get<ApiResponse<CalendarEvent[]>>('/api/calendar/events', { params }).pipe(map((r) => r.data));
  }

  // Watchlist
  getSavedExams(): Observable<SavedExam[]> {
    return this.http.get<ApiResponse<SavedExam[]>>('/api/watchlist').pipe(map((r) => r.data));
  }

  saveExam(examId: string, priorityLevel = 'High', personalNotes?: string): Observable<SavedExam> {
    return this.http.post<ApiResponse<SavedExam>>('/api/watchlist', { examId, priorityLevel, personalNotes }).pipe(map((r) => r.data));
  }

  removeSavedExam(examId: string): Observable<void> {
    return this.http.delete<ApiResponse<unknown>>(`/api/watchlist/${examId}`).pipe(map(() => undefined));
  }

  // Syllabus & Preparation
  getSyllabusLibrary(examId?: string): Observable<SyllabusSubject[]> {
    let params = new HttpParams();
    if (examId) params = params.set('examId', examId);
    return this.http.get<ApiResponse<SyllabusSubject[]>>('/api/syllabus', { params }).pipe(map((r) => r.data));
  }

  updateTopicProgress(topicId: string, payload: { examId?: string; status: string; incrementRevision: boolean; preparationNotes?: string }): Observable<SyllabusTopicProgress> {
    return this.http.put<ApiResponse<SyllabusTopicProgress>>(`/api/syllabus/topics/${topicId}/progress`, payload).pipe(map((r) => r.data));
  }

  getPreparationOverview(): Observable<PreparationOverview> {
    return this.http.get<ApiResponse<PreparationOverview>>('/api/preparation/overview').pipe(map((r) => r.data));
  }

  upsertStudyPlan(payload: Record<string, unknown>): Observable<StudyPlan> {
    return this.http.put<ApiResponse<StudyPlan>>('/api/preparation/study-plan', payload).pipe(map((r) => r.data));
  }

  addMockTest(payload: Record<string, unknown>): Observable<MockTestResult> {
    return this.http.post<ApiResponse<MockTestResult>>('/api/preparation/mock-tests', payload).pipe(map((r) => r.data));
  }

  deleteMockTest(id: string): Observable<void> {
    return this.http.delete<ApiResponse<unknown>>(`/api/preparation/mock-tests/${id}`).pipe(map(() => undefined));
  }

  upsertUserApplication(payload: Record<string, unknown>): Observable<UserApplication> {
    return this.http.put<ApiResponse<UserApplication>>('/api/preparation/applications', payload).pipe(map((r) => r.data));
  }

  // Analytics
  getVacancyAnalytics(categorySlug?: string): Observable<VacancyAnalytics> {
    let params = new HttpParams();
    if (categorySlug) params = params.set('categorySlug', categorySlug);
    return this.http.get<ApiResponse<VacancyAnalytics>>('/api/analytics/vacancies', { params }).pipe(map((r) => r.data));
  }

  // Notifications
  getNotifications(unreadOnly = false): Observable<NotificationItem[]> {
    return this.http.get<ApiResponse<NotificationItem[]>>(`/api/notifications?unreadOnly=${unreadOnly}`).pipe(map((r) => r.data));
  }

  getUnreadCount(): Observable<number> {
    return this.http.get<ApiResponse<number>>('/api/notifications/unread-count').pipe(map((r) => r.data));
  }

  markNotificationRead(id: string): Observable<void> {
    return this.http.post<ApiResponse<unknown>>(`/api/notifications/${id}/read`, {}).pipe(map(() => undefined));
  }

  markAllNotificationsRead(): Observable<void> {
    return this.http.post<ApiResponse<unknown>>('/api/notifications/read-all', {}).pipe(map(() => undefined));
  }

  getNotificationPreferences(): Observable<NotificationPreference> {
    return this.http.get<ApiResponse<NotificationPreference>>('/api/notifications/preferences').pipe(map((r) => r.data));
  }

  updateNotificationPreferences(prefs: NotificationPreference): Observable<NotificationPreference> {
    return this.http.put<ApiResponse<NotificationPreference>>('/api/notifications/preferences', prefs).pipe(map((r) => r.data));
  }

  runNotificationCheck(): Observable<number> {
    return this.http.post<ApiResponse<number>>('/api/notifications/run-check', {}).pipe(map((r) => r.data));
  }

  // Admin
  getAdminOverview(): Observable<AdminDashboardOverview> {
    return this.http.get<ApiResponse<AdminDashboardOverview>>('/api/admin/overview').pipe(map((r) => r.data));
  }

  getAdminLookups(): Observable<{ categories: any[]; orgs: any[]; sources: any[]; subjects: any[] }> {
    return this.http.get<ApiResponse<any>>('/api/admin/lookups').pipe(map((r) => r.data));
  }

  createExam(payload: Record<string, unknown>): Observable<string> {
    return this.http.post<ApiResponse<string>>('/api/admin/exams', payload).pipe(map((r) => r.data));
  }

  updateExamDates(examId: string, payload: Record<string, unknown>): Observable<void> {
    return this.http.put<ApiResponse<unknown>>(`/api/admin/exams/${examId}/dates`, payload).pipe(map(() => undefined));
  }

  updateExamVacancies(examId: string, payload: Record<string, unknown>): Observable<void> {
    return this.http.put<ApiResponse<unknown>>(`/api/admin/exams/${examId}/vacancies`, payload).pipe(map(() => undefined));
  }

  addHistoricalRecord(examId: string, payload: Record<string, unknown>): Observable<void> {
    return this.http.post<ApiResponse<unknown>>(`/api/admin/exams/${examId}/history`, payload).pipe(map(() => undefined));
  }

  changeExamStatus(examId: string, status: string, reason: string): Observable<void> {
    return this.http.post<ApiResponse<unknown>>(`/api/admin/exams/${examId}/status`, { status, reason }).pipe(map(() => undefined));
  }

  approveExamRecord(examId: string, approve: boolean, notes?: string): Observable<void> {
    return this.http.post<ApiResponse<unknown>>(`/api/admin/exams/${examId}/approve`, {
      approve,
      verificationStatus: approve ? 'VerifiedOfficial' : 'Rejected',
      dataConfidence: approve ? 'ConfirmedOfficial' : 'UnverifiedDraft',
      notes
    }).pipe(map(() => undefined));
  }

  uploadNotificationPdf(examId: string, file: File): Observable<string> {
    const fd = new FormData();
    fd.append('file', file);
    return this.http.post<ApiResponse<string>>(`/api/admin/exams/${examId}/upload-pdf`, fd).pipe(map((r) => r.data));
  }

  verifyOfficialSource(sourceId: string): Observable<any> {
    return this.http.post<ApiResponse<any>>(`/api/admin/sources/${sourceId}/verify`, {}).pipe(map((r) => r.data));
  }

  getAdminUsers(): Observable<any[]> {
    return this.http.get<ApiResponse<any[]>>('/api/admin/users').pipe(map((r) => r.data));
  }

  toggleUserActive(userId: string, isActive: boolean): Observable<void> {
    return this.http.post<ApiResponse<unknown>>(`/api/admin/users/${userId}/toggle-active?isActive=${isActive}`, {}).pipe(map(() => undefined));
  }
}
