import { Routes } from '@angular/router';
import { adminGuard, authGuard } from './core/guards/auth.guard';
import { ShellLayoutComponent } from './shared/components/shell-layout.component';

export const routes: Routes = [
  {
    path: 'auth/login',
    loadComponent: () => import('./features/auth/login.component').then((m) => m.LoginComponent)
  },
  {
    path: 'auth/register',
    loadComponent: () => import('./features/auth/register.component').then((m) => m.RegisterComponent)
  },
  {
    path: '',
    component: ShellLayoutComponent,
    children: [
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
      {
        path: 'dashboard',
        loadComponent: () => import('./features/dashboard/dashboard.component').then((m) => m.DashboardComponent)
      },
      {
        path: 'exams',
        data: { mode: 'all' },
        loadComponent: () => import('./features/exams/exam-directory.component').then((m) => m.ExamDirectoryComponent)
      },
      {
        path: 'exams/:slug',
        loadComponent: () => import('./features/exams/exam-detail.component').then((m) => m.ExamDetailComponent)
      },
      {
        path: 'upcoming-notifications',
        data: { mode: 'upcoming' },
        loadComponent: () => import('./features/exams/exam-directory.component').then((m) => m.ExamDirectoryComponent)
      },
      {
        path: 'open-applications',
        data: { mode: 'open' },
        loadComponent: () => import('./features/exams/exam-directory.component').then((m) => m.ExamDirectoryComponent)
      },
      {
        path: 'calendar',
        loadComponent: () => import('./features/calendar/calendar.component').then((m) => m.CalendarComponent)
      },
      {
        path: 'eligibility',
        loadComponent: () => import('./features/eligibility/eligibility.component').then((m) => m.EligibilityComponent)
      },
      {
        path: 'syllabus',
        loadComponent: () => import('./features/syllabus/syllabus.component').then((m) => m.SyllabusComponent)
      },
      {
        path: 'preparation',
        canActivate: [authGuard],
        loadComponent: () => import('./features/preparation/preparation.component').then((m) => m.PreparationComponent)
      },
      {
        path: 'analytics',
        loadComponent: () => import('./features/analytics/analytics.component').then((m) => m.AnalyticsComponent)
      },
      {
        path: 'saved-exams',
        canActivate: [authGuard],
        loadComponent: () => import('./features/saved-exams/saved-exams.component').then((m) => m.SavedExamsComponent)
      },
      {
        path: 'notifications',
        canActivate: [authGuard],
        loadComponent: () => import('./features/notifications/notifications.component').then((m) => m.NotificationsComponent)
      },
      {
        path: 'settings',
        canActivate: [authGuard],
        loadComponent: () => import('./features/settings/settings.component').then((m) => m.SettingsComponent)
      },
      {
        path: 'admin',
        canActivate: [adminGuard],
        loadComponent: () => import('./features/admin/admin.component').then((m) => m.AdminComponent)
      }
    ]
  },
  { path: '**', redirectTo: 'dashboard' }
];
