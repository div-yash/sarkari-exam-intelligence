import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  template: `
    <div class="min-h-screen bg-[#0b192c] py-8 px-4 flex items-center justify-center">
      <div class="w-full max-w-3xl bg-white rounded-2xl shadow-2xl overflow-hidden">
        <div class="p-6 bg-[#102542] text-white border-b border-white/10 flex flex-col sm:flex-row sm:items-center gap-4">
          <div class="bg-white rounded-xl px-3 py-1.5 shadow-sm shrink-0 self-start sm:self-auto">
            <img src="/logo.png" alt="Sarkari Exam Intelligence" class="h-14 w-auto object-contain" />
          </div>
          <div>
            <h1 class="text-xl font-bold">Candidate Registration & Eligibility Profile</h1>
            <p class="text-xs text-blue-200 mt-1">
              Supports all Indian graduates across states, categories, branches & experience levels.
            </p>
          </div>
        </div>

        <form (ngSubmit)="submit()" class="p-6 space-y-6">
          @if (error()) {
            <div class="p-3 rounded-lg bg-red-50 border border-red-200 text-red-700 text-xs font-medium">
              {{ error() }}
            </div>
          }

          <!-- Account & Personal Details -->
          <div>
            <h3 class="text-xs font-bold uppercase tracking-wider text-slate-500 mb-3">1. Account & Personal Details</h3>
            <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
              <div>
                <label class="block text-xs font-semibold text-slate-700 mb-1">Full Name *</label>
                <input type="text" [(ngModel)]="form.fullName" name="fullName" required
                  class="w-full px-3 py-2 text-sm border border-slate-300 rounded-lg" />
              </div>
              <div>
                <label class="block text-xs font-semibold text-slate-700 mb-1">Email Address *</label>
                <input type="email" [(ngModel)]="form.email" name="email" required
                  class="w-full px-3 py-2 text-sm border border-slate-300 rounded-lg" />
              </div>
              <div>
                <label class="block text-xs font-semibold text-slate-700 mb-1">Password (min 8 chars) *</label>
                <input type="password" [(ngModel)]="form.password" name="password" required minlength="8"
                  class="w-full px-3 py-2 text-sm border border-slate-300 rounded-lg" />
              </div>
              <div>
                <label class="block text-xs font-semibold text-slate-700 mb-1">Date of Birth *</label>
                <input type="date" [(ngModel)]="form.dateOfBirth" name="dateOfBirth" required
                  class="w-full px-3 py-2 text-sm border border-slate-300 rounded-lg" />
              </div>
              <div>
                <label class="block text-xs font-semibold text-slate-700 mb-1">Gender *</label>
                <select [(ngModel)]="form.gender" name="gender" class="w-full px-3 py-2 text-sm border border-slate-300 rounded-lg">
                  <option value="Male">Male</option>
                  <option value="Female">Female</option>
                  <option value="Other">Other</option>
                </select>
              </div>
              <div>
                <label class="block text-xs font-semibold text-slate-700 mb-1">Category *</label>
                <select [(ngModel)]="form.category" name="category" class="w-full px-3 py-2 text-sm border border-slate-300 rounded-lg">
                  <option value="General">General (Unreserved)</option>
                  <option value="EWS">EWS</option>
                  <option value="OBCNCL">OBC-NCL (Central / Bihar BC-EBC)</option>
                  <option value="SC">SC</option>
                  <option value="ST">ST</option>
                </select>
              </div>
            </div>
          </div>

          <!-- Domicile & Certificates -->
          <div>
            <h3 class="text-xs font-bold uppercase tracking-wider text-slate-500 mb-3">2. Domicile & Reservation Verification</h3>
            <div class="grid grid-cols-1 md:grid-cols-3 gap-4 items-center">
              <div>
                <label class="block text-xs font-semibold text-slate-700 mb-1">Domicile State *</label>
                <select [(ngModel)]="form.domicileState" name="domicileState" class="w-full px-3 py-2 text-sm border border-slate-300 rounded-lg">
                  @for (st of indianStates; track st) {
                    <option [value]="st">{{ st }}</option>
                  }
                </select>
              </div>
              <label class="flex items-center gap-2 text-xs font-medium text-slate-700 pt-5">
                <input type="checkbox" [(ngModel)]="form.hasBiharDomicileCertificate" name="hasBiharDomicileCertificate" class="rounded" />
                Valid Domicile Certificate
              </label>
              <label class="flex items-center gap-2 text-xs font-medium text-slate-700 pt-5">
                <input type="checkbox" [(ngModel)]="form.hasValidNclOrEwsCertificate" name="hasValidNclOrEwsCertificate" class="rounded" />
                Valid NCL / EWS / Caste Certificate
              </label>
            </div>
          </div>

          <!-- Academic Qualification -->
          <div>
            <h3 class="text-xs font-bold uppercase tracking-wider text-slate-500 mb-3">3. Education & Branch</h3>
            <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
              <div>
                <label class="block text-xs font-semibold text-slate-700 mb-1">Graduation Qualification *</label>
                <select [(ngModel)]="form.graduationQualification" name="graduationQualification" class="w-full px-3 py-2 text-sm border border-slate-300 rounded-lg">
                  <option value="B.Tech">B.Tech</option>
                  <option value="B.E.">B.E.</option>
                  <option value="B.Sc">B.Sc</option>
                  <option value="B.Com">B.Com</option>
                  <option value="B.A.">B.A.</option>
                  <option value="BCA">BCA</option>
                  <option value="MCA">MCA</option>
                  <option value="M.Tech">M.Tech</option>
                  <option value="M.Sc">M.Sc</option>
                  <option value="Diploma">Diploma</option>
                  <option value="12th">12th (Intermediate)</option>
                </select>
              </div>
              <div>
                <label class="block text-xs font-semibold text-slate-700 mb-1">Engineering Branch / Stream</label>
                <input type="text" [(ngModel)]="form.engineeringBranch" name="engineeringBranch" placeholder="e.g. Information Technology"
                  class="w-full px-3 py-2 text-sm border border-slate-300 rounded-lg" />
              </div>
              <div>
                <label class="block text-xs font-semibold text-slate-700 mb-1">University</label>
                <input type="text" [(ngModel)]="form.university" name="university"
                  class="w-full px-3 py-2 text-sm border border-slate-300 rounded-lg" />
              </div>
              <div>
                <label class="block text-xs font-semibold text-slate-700 mb-1">Graduation Percentage (%)</label>
                <input type="number" step="0.01" [(ngModel)]="form.graduationPercentage" name="graduationPercentage"
                  class="w-full px-3 py-2 text-sm border border-slate-300 rounded-lg" />
              </div>
              <div>
                <label class="block text-xs font-semibold text-slate-700 mb-1">CGPA (out of 10)</label>
                <input type="number" step="0.01" [(ngModel)]="form.graduationCgpa" name="graduationCgpa"
                  class="w-full px-3 py-2 text-sm border border-slate-300 rounded-lg" />
              </div>
              <div>
                <label class="block text-xs font-semibold text-slate-700 mb-1">Passing Year</label>
                <input type="number" [(ngModel)]="form.passingYear" name="passingYear"
                  class="w-full px-3 py-2 text-sm border border-slate-300 rounded-lg" />
              </div>
            </div>
          </div>

          <!-- Work Experience & GATE -->
          <div>
            <h3 class="text-xs font-bold uppercase tracking-wider text-slate-500 mb-3">4. Experience, Additional Qualifications & GATE</h3>
            <div class="grid grid-cols-1 md:grid-cols-4 gap-4">
              <div>
                <label class="block text-xs font-semibold text-slate-700 mb-1">Work Experience (Months)</label>
                <input type="number" [(ngModel)]="form.workExperienceMonths" name="workExperienceMonths"
                  class="w-full px-3 py-2 text-sm border border-slate-300 rounded-lg" />
              </div>
              <div>
                <label class="block text-xs font-semibold text-slate-700 mb-1">Additional Qualifications</label>
                <input type="text" [(ngModel)]="form.additionalQualifications" name="additionalQualifications" placeholder="e.g. M.Tech, DOEACC B"
                  class="w-full px-3 py-2 text-sm border border-slate-300 rounded-lg" />
              </div>
              <label class="flex items-center gap-2 text-xs font-medium text-slate-700 pt-5">
                <input type="checkbox" [(ngModel)]="form.isWorkingProfessional" name="isWorkingProfessional" class="rounded" />
                Working Professional
              </label>
              <label class="flex items-center gap-2 text-xs font-medium text-slate-700 pt-5">
                <input type="checkbox" [(ngModel)]="form.hasValidGateScore" name="hasValidGateScore" class="rounded" />
                Valid GATE Score
              </label>
            </div>
            @if (form.hasValidGateScore) {
              <div class="grid grid-cols-1 md:grid-cols-3 gap-4 mt-3">
                <div>
                  <label class="block text-xs font-semibold text-slate-700 mb-1">GATE Paper Code</label>
                  <input type="text" [(ngModel)]="form.gateSubject" name="gateSubject" placeholder="CS"
                    class="w-full px-3 py-2 text-sm border border-slate-300 rounded-lg" />
                </div>
                <div>
                  <label class="block text-xs font-semibold text-slate-700 mb-1">GATE Score (0–1000)</label>
                  <input type="number" [(ngModel)]="form.gateScore" name="gateScore"
                    class="w-full px-3 py-2 text-sm border border-slate-300 rounded-lg" />
                </div>
                <div>
                  <label class="block text-xs font-semibold text-slate-700 mb-1">GATE Year</label>
                  <input type="number" [(ngModel)]="form.gateYear" name="gateYear"
                    class="w-full px-3 py-2 text-sm border border-slate-300 rounded-lg" />
                </div>
              </div>
            }
          </div>

          <div class="flex items-center justify-between pt-2 border-t border-slate-200">
            <a routerLink="/auth/login" class="text-xs text-blue-600 font-semibold hover:underline">Already registered? Sign In</a>
            <button type="submit" [disabled]="loading()"
              class="px-6 py-2.5 rounded-lg bg-blue-600 hover:bg-blue-700 text-white font-semibold text-sm disabled:opacity-60">
              {{ loading() ? 'Creating Profile...' : 'Register & Evaluate Eligibility' }}
            </button>
          </div>
        </form>
      </div>
    </div>
  `
})
export class RegisterComponent {
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);

  loading = signal(false);
  error = signal<string | null>(null);

  readonly indianStates = [
    'Bihar', 'Uttar Pradesh', 'Jharkhand', 'Delhi', 'Maharashtra', 'Karnataka',
    'West Bengal', 'Rajasthan', 'Madhya Pradesh', 'Tamil Nadu', 'Telangana',
    'Gujarat', 'Haryana', 'Punjab', 'Odisha', 'Assam', 'Kerala', 'Uttarakhand', 'Other'
  ];

  form = {
    fullName: '',
    email: '',
    password: '',
    dateOfBirth: '1999-06-15',
    gender: 'Male',
    category: 'General',
    isPwBD: false,
    isExServiceman: false,
    domicileState: 'Bihar',
    graduationQualification: 'B.Tech',
    engineeringBranch: 'Information Technology',
    university: '',
    graduationPercentage: 75 as number | null,
    graduationCgpa: null as number | null,
    passingYear: 2022 as number | null,
    workExperienceMonths: 0,
    isWorkingProfessional: false,
    additionalQualifications: '',
    hasValidGateScore: false,
    gateSubject: 'CS',
    gateScore: null as number | null,
    gateYear: 2026 as number | null,
    hasBiharDomicileCertificate: false,
    hasValidNclOrEwsCertificate: false,
    nationality: 'Indian'
  };

  submit(): void {
    this.error.set(null);
    this.loading.set(true);
    this.auth.register(this.form).subscribe({
      next: () => {
        this.loading.set(false);
        this.router.navigate(['/eligibility']);
      },
      error: (err) => {
        this.loading.set(false);
        const list = err?.error?.errors as string[] | undefined;
        this.error.set(list?.length ? list.join(' ') : (err?.error?.message || 'Registration failed.'));
      }
    });
  }
}
