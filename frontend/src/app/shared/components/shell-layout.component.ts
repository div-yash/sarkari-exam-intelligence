import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../core/services/auth.service';
import { ApiService } from '../../core/services/api.service';

interface NavItem {
  label: string;
  route: string;
  icon: string;
  badge?: () => number | null;
}

@Component({
  selector: 'app-shell-layout',
  standalone: true,
  imports: [CommonModule, RouterOutlet, RouterLink, RouterLinkActive, FormsModule],
  template: `
    <div class="min-h-screen flex bg-slate-50">
      <!-- Mobile backdrop -->
      @if (mobileMenuOpen()) {
        <div (click)="mobileMenuOpen.set(false)" class="fixed inset-0 bg-slate-900/60 z-30 lg:hidden"></div>
      }

      <!-- Dark Navy Sidebar -->
      <aside
        [class.translate-x-0]="mobileMenuOpen()"
        [class.-translate-x-full]="!mobileMenuOpen()"
        class="fixed lg:static inset-y-0 left-0 z-40 w-64 bg-[#0b192c] text-slate-100 flex flex-col transition-transform duration-200 lg:translate-x-0 shrink-0 shadow-xl">

        <!-- Brand -->
        <div class="px-4 py-3.5 border-b border-white/10 flex items-center justify-between gap-2">
          <a routerLink="/dashboard" class="flex-1 bg-white rounded-xl px-2.5 py-2 shadow-sm hover:shadow transition flex items-center justify-center">
            <img src="/logo.png" alt="Sarkari Exam Intelligence" class="w-full max-h-20 object-contain" />
          </a>
          <button (click)="mobileMenuOpen.set(false)" class="lg:hidden text-slate-300 hover:text-white text-xl leading-none px-1">
            &times;
          </button>
        </div>

        <!-- 12 Required Sidebar Navigation Items -->
        <nav class="flex-1 overflow-y-auto px-3 py-4 space-y-1 text-sm">
          @for (item of navItems; track item.route) {
            <a
              [routerLink]="item.route"
              routerLinkActive="bg-blue-600/20 text-white border-l-4 border-blue-400"
              (click)="mobileMenuOpen.set(false)"
              class="flex items-center justify-between px-3 py-2.5 rounded-lg text-slate-300 hover:bg-white/10 hover:text-white transition">
              <span class="flex items-center gap-3">
                <span class="text-base opacity-90">{{ item.icon }}</span>
                <span class="font-medium">{{ item.label }}</span>
              </span>
              @if (item.badge && item.badge()) {
                <span class="px-2 py-0.5 text-[11px] font-bold rounded-full bg-red-600 text-white tabular-nums">
                  {{ item.badge() }}
                </span>
              }
            </a>
          }

          @if (auth.isAdmin()) {
            <div class="pt-4 mt-4 border-t border-white/10">
              <div class="px-3 pb-1 text-[10px] font-semibold uppercase tracking-wider text-slate-400">Administration</div>
              <a
                routerLink="/admin"
                routerLinkActive="bg-blue-600/20 text-white border-l-4 border-blue-400"
                (click)="mobileMenuOpen.set(false)"
                class="flex items-center gap-3 px-3 py-2.5 rounded-lg text-amber-200 hover:bg-white/10 hover:text-white transition">
                <span>🛡️</span>
                <span class="font-medium">Admin Console</span>
              </a>
            </div>
          }
        </nav>

        <!-- Candidate Profile Card -->
        <div class="p-4 border-t border-white/10 bg-[#071222]">
          @if (auth.isAuthenticated()) {
            <div class="text-xs font-semibold text-white truncate">{{ auth.currentUser()?.fullName }}</div>
            <div class="text-[11px] text-slate-400 truncate">
              {{ auth.userProfile()?.graduationQualification || 'Graduate' }}
              @if (auth.userProfile()?.engineeringBranch) { · {{ auth.userProfile()?.engineeringBranch }} }
              @if (auth.userProfile()?.domicileState) { · {{ auth.userProfile()?.domicileState }} }
            </div>
            <div class="mt-2.5 flex items-center gap-2">
              <a routerLink="/settings" class="flex-1 text-center text-xs py-1.5 rounded bg-white/10 hover:bg-white/20 text-slate-200 font-medium">
                Profile
              </a>
              <button (click)="auth.logout()" class="flex-1 text-xs py-1.5 rounded bg-red-600/20 hover:bg-red-600/40 text-red-200 font-medium">
                Sign Out
              </button>
            </div>
          } @else {
            <div class="text-xs text-slate-300 mb-2">Sign in to evaluate personal eligibility & deadlines</div>
            <div class="flex gap-2">
              <a routerLink="/auth/login" class="flex-1 text-center text-xs py-1.5 rounded bg-blue-600 hover:bg-blue-500 text-white font-semibold">
                Sign In
              </a>
              <a routerLink="/auth/register" class="flex-1 text-center text-xs py-1.5 rounded bg-white/10 hover:bg-white/20 text-white font-medium">
                Register
              </a>
            </div>
          }
        </div>
      </aside>

      <!-- Main Workspace -->
      <div class="flex-1 flex flex-col min-w-0">
        <!-- Top Bar with Global Search -->
        <header class="sticky top-0 z-20 bg-white border-b border-slate-200 px-4 lg:px-6 py-3 flex items-center justify-between gap-4">
          <div class="flex items-center gap-3 flex-1 max-w-2xl">
            <button (click)="mobileMenuOpen.set(true)" class="lg:hidden p-2 rounded-lg border border-slate-200 text-slate-700">
              ☰
            </button>
            <form (ngSubmit)="onGlobalSearch()" class="flex-1 flex items-center gap-2">
              <div class="relative flex-1">
                <input
                  type="text"
                  [(ngModel)]="globalQuery"
                  name="globalQuery"
                  placeholder="Search UPSC, BPSC, SEBI IT, NIC, branch, department or state..."
                  class="w-full pl-9 pr-4 py-2 text-sm bg-slate-50 border border-slate-200 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 focus:bg-white" />
                <span class="absolute left-3 top-2.5 text-slate-400 text-sm">🔍</span>
              </div>
              <button type="submit" class="px-3.5 py-2 text-xs font-semibold rounded-lg bg-[#0b192c] text-white hover:bg-[#17345c]">
                Search
              </button>
            </form>
          </div>

          <div class="flex items-center gap-3">
            <a routerLink="/open-applications" class="hidden sm:inline-flex items-center gap-1.5 px-3 py-1.5 rounded-full bg-green-50 text-green-700 border border-green-200 text-xs font-semibold">
              <span class="w-2 h-2 rounded-full bg-green-600"></span>
              Open Forms
            </a>
            <a routerLink="/notifications" class="relative p-2 rounded-lg border border-slate-200 hover:bg-slate-50 text-slate-700">
              <span>🔔</span>
              @if (unreadCount() > 0) {
                <span class="absolute -top-1 -right-1 w-5 h-5 rounded-full bg-red-600 text-white text-[10px] font-bold flex items-center justify-center tabular-nums">
                  {{ unreadCount() > 99 ? '99+' : unreadCount() }}
                </span>
              }
            </a>
          </div>
        </header>

        <!-- Page Content -->
        <main class="flex-1 p-4 lg:p-6 overflow-x-hidden">
          <router-outlet></router-outlet>
        </main>
      </div>
    </div>
  `
})
export class ShellLayoutComponent implements OnInit {
  readonly auth = inject(AuthService);
  private readonly api = inject(ApiService);
  private readonly router = inject(Router);

  mobileMenuOpen = signal(false);
  unreadCount = signal(0);
  globalQuery = '';

  readonly navItems: NavItem[] = [
    { label: 'Dashboard', route: '/dashboard', icon: '📊' },
    { label: 'All Exams', route: '/exams', icon: '📚' },
    { label: 'Upcoming Notifications', route: '/upcoming-notifications', icon: '🟠' },
    { label: 'Open Applications', route: '/open-applications', icon: '🟢' },
    { label: 'Exam Calendar', route: '/calendar', icon: '📅' },
    { label: 'My Eligibility', route: '/eligibility', icon: '✅' },
    { label: 'Syllabus', route: '/syllabus', icon: '📖' },
    { label: 'Preparation Tracker', route: '/preparation', icon: '🎯' },
    { label: 'Vacancy Analytics', route: '/analytics', icon: '📈' },
    { label: 'Saved Exams', route: '/saved-exams', icon: '⭐' },
    { label: 'Notifications', route: '/notifications', icon: '🔔', badge: () => this.unreadCount() || null },
    { label: 'Settings', route: '/settings', icon: '⚙️' }
  ];

  ngOnInit(): void {
    if (this.auth.isAuthenticated()) {
      this.api.getUnreadCount().subscribe({
        next: (c) => this.unreadCount.set(c),
        error: () => {}
      });
    }
  }

  onGlobalSearch(): void {
    this.router.navigate(['/exams'], {
      queryParams: { search: this.globalQuery.trim() || undefined }
    });
  }
}
