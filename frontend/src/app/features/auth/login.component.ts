import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  template: `
    <div class="min-h-screen bg-[#0b192c] flex items-center justify-center p-4">
      <div class="w-full max-w-md bg-white rounded-2xl shadow-2xl overflow-hidden">
        <div class="p-6 bg-[#102542] text-white border-b border-white/10">
          <div class="flex items-center gap-3">
            <div class="w-10 h-10 rounded-lg bg-blue-600 flex items-center justify-center font-extrabold">SEI</div>
            <div>
              <h1 class="text-lg font-bold">Sarkari Exam Intelligence</h1>
              <p class="text-xs text-blue-200">Verified Government Exam & Eligibility Tracker</p>
            </div>
          </div>
        </div>

        <form (ngSubmit)="submit()" class="p-6 space-y-4">
          <h2 class="text-xl font-bold text-slate-900">Sign in to your account</h2>

          @if (error()) {
            <div class="p-3 rounded-lg bg-red-50 border border-red-200 text-red-700 text-xs font-medium">
              {{ error() }}
            </div>
          }

          <div>
            <label class="block text-xs font-semibold text-slate-700 mb-1">Email Address</label>
            <input type="email" [(ngModel)]="email" name="email" required
              class="w-full px-3.5 py-2.5 text-sm border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:outline-none" />
          </div>

          <div>
            <label class="block text-xs font-semibold text-slate-700 mb-1">Password</label>
            <input type="password" [(ngModel)]="password" name="password" required
              class="w-full px-3.5 py-2.5 text-sm border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:outline-none" />
          </div>

          <button type="submit" [disabled]="loading()"
            class="w-full py-2.5 rounded-lg bg-blue-600 hover:bg-blue-700 text-white font-semibold text-sm disabled:opacity-60">
            {{ loading() ? 'Signing in...' : 'Sign In' }}
          </button>

          <!-- Quick Credential Helpers for Demo Evaluation -->
          <div class="pt-3 border-t border-slate-200">
            <div class="text-[11px] font-semibold text-slate-500 uppercase tracking-wider mb-2">Seeded Evaluation Accounts</div>
            <div class="grid grid-cols-2 gap-2">
              <button type="button" (click)="fillDemo('candidate@sarkariexam.in', 'Candidate@12345')"
                class="p-2 text-left rounded-lg border border-slate-200 hover:border-blue-400 hover:bg-blue-50/50 transition">
                <div class="text-xs font-bold text-slate-800">B.Tech IT (Bihar)</div>
                <div class="text-[11px] text-slate-500">Candidate Profile</div>
              </button>
              <button type="button" (click)="fillDemo('admin@sarkariexam.in', 'Admin@12345')"
                class="p-2 text-left rounded-lg border border-slate-200 hover:border-amber-400 hover:bg-amber-50/50 transition">
                <div class="text-xs font-bold text-slate-800">System Admin</div>
                <div class="text-[11px] text-slate-500">Verification Console</div>
              </button>
            </div>
          </div>

          <div class="text-center text-xs text-slate-600 pt-2">
            New candidate?
            <a routerLink="/auth/register" class="text-blue-600 font-semibold hover:underline">Create your eligibility profile</a>
            · <a routerLink="/dashboard" class="text-slate-500 hover:underline">Browse as Guest</a>
          </div>
        </form>
      </div>
    </div>
  `
})
export class LoginComponent {
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);

  email = '';
  password = '';
  loading = signal(false);
  error = signal<string | null>(null);

  fillDemo(email: string, password: string): void {
    this.email = email;
    this.password = password;
    this.submit();
  }

  submit(): void {
    this.error.set(null);
    this.loading.set(true);
    this.auth.login(this.email, this.password).subscribe({
      next: () => {
        this.loading.set(false);
        this.router.navigate(['/dashboard']);
      },
      error: (err) => {
        this.loading.set(false);
        this.error.set(err?.error?.message || 'Invalid credentials or server unreachable.');
      }
    });
  }
}
