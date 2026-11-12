import { Injectable, computed, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { tap } from 'rxjs';
import { ApiResponse, AuthResponse, UserProfile } from '../models/api.models';

const TOKEN_KEY = 'sei_jwt_token';
const USER_KEY = 'sei_user_state';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly authState = signal<AuthResponse | null>(this.loadInitialState());

  readonly currentUser = this.authState.asReadonly();
  readonly isAuthenticated = computed(() => !!this.authState()?.token);
  readonly isAdmin = computed(() => this.authState()?.roles?.includes('Admin') ?? false);
  readonly userProfile = computed(() => this.authState()?.profile ?? null);

  constructor(private readonly http: HttpClient, private readonly router: Router) {}

  getToken(): string | null {
    return localStorage.getItem(TOKEN_KEY);
  }

  login(email: string, password: string) {
    return this.http.post<ApiResponse<AuthResponse>>('/api/auth/login', { email, password }).pipe(
      tap((res) => {
        if (res.success && res.data) {
          this.persistAuth(res.data);
        }
      })
    );
  }

  register(payload: Record<string, unknown>) {
    return this.http.post<ApiResponse<AuthResponse>>('/api/auth/register', payload).pipe(
      tap((res) => {
        if (res.success && res.data) {
          this.persistAuth(res.data);
        }
      })
    );
  }

  refreshProfile() {
    return this.http.get<ApiResponse<UserProfile>>('/api/profile').pipe(
      tap((res) => {
        const cur = this.authState();
        if (res.success && res.data && cur) {
          const updated: AuthResponse = { ...cur, fullName: res.data.fullName, profile: res.data };
          this.persistAuth(updated);
        }
      })
    );
  }

  updateProfile(payload: Record<string, unknown>) {
    return this.http.put<ApiResponse<UserProfile>>('/api/profile', payload).pipe(
      tap((res) => {
        const cur = this.authState();
        if (res.success && res.data && cur) {
          const updated: AuthResponse = { ...cur, fullName: res.data.fullName, profile: res.data };
          this.persistAuth(updated);
        }
      })
    );
  }

  logout(): void {
    localStorage.removeItem(TOKEN_KEY);
    localStorage.removeItem(USER_KEY);
    this.authState.set(null);
    this.router.navigate(['/auth/login']);
  }

  private persistAuth(auth: AuthResponse): void {
    localStorage.setItem(TOKEN_KEY, auth.token);
    localStorage.setItem(USER_KEY, JSON.stringify(auth));
    this.authState.set(auth);
  }

  private loadInitialState(): AuthResponse | null {
    try {
      const token = localStorage.getItem(TOKEN_KEY);
      const raw = localStorage.getItem(USER_KEY);
      if (!token || !raw) return null;
      const parsed = JSON.parse(raw) as AuthResponse;
      if (parsed.expiresAtUtc && new Date(parsed.expiresAtUtc).getTime() <= Date.now()) {
        localStorage.removeItem(TOKEN_KEY);
        localStorage.removeItem(USER_KEY);
        return null;
      }
      return parsed;
    } catch {
      return null;
    }
  }
}
