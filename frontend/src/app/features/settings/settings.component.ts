import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../../core/services/api.service';
import { AuthService } from '../../core/services/auth.service';
import { NotificationPreference } from '../../core/models/api.models';

@Component({
  selector: 'app-settings',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="space-y-6 max-w-4xl">
      <div>
        <h1 class="text-2xl font-bold text-slate-900">Candidate Profile & Notification Preferences</h1>
        <p class="text-xs text-slate-500 mt-0.5">Updates immediately re-evaluate your eligibility across all 65 examination families.</p>
      </div>

      @if (savedBanner()) {
        <div class="p-3 rounded-lg bg-green-50 border border-green-200 text-green-800 text-xs font-semibold">{{ savedBanner() }}</div>
      }

      <!-- Profile Form -->
      <form (ngSubmit)="saveProfile()" class="card-surface p-5 space-y-4 text-xs">
        <h2 class="text-base font-bold text-slate-900">Eligibility & Academic Profile</h2>
        <div class="grid grid-cols-1 sm:grid-cols-3 gap-4">
          <div>
            <label class="block font-semibold text-slate-700 mb-1">Full Name</label>
            <input type="text" [(ngModel)]="profile.fullName" name="fullName" required class="w-full px-3 py-2 border border-slate-300 rounded-lg" />
          </div>
          <div>
            <label class="block font-semibold text-slate-700 mb-1">Date of Birth</label>
            <input type="date" [(ngModel)]="profile.dateOfBirth" name="dateOfBirth" class="w-full px-3 py-2 border border-slate-300 rounded-lg" />
          </div>
          <div>
            <label class="block font-semibold text-slate-700 mb-1">Category</label>
            <select [(ngModel)]="profile.category" name="category" class="w-full px-3 py-2 border border-slate-300 rounded-lg bg-white">
              <option value="General">General</option>
              <option value="EWS">EWS</option>
              <option value="OBCNCL">OBC-NCL</option>
              <option value="SC">SC</option>
              <option value="ST">ST</option>
            </select>
          </div>
          <div>
            <label class="block font-semibold text-slate-700 mb-1">Domicile State</label>
            <input type="text" [(ngModel)]="profile.domicileState" name="domicileState" class="w-full px-3 py-2 border border-slate-300 rounded-lg" />
          </div>
          <div>
            <label class="block font-semibold text-slate-700 mb-1">Qualification</label>
            <input type="text" [(ngModel)]="profile.graduationQualification" name="graduationQualification" class="w-full px-3 py-2 border border-slate-300 rounded-lg" />
          </div>
          <div>
            <label class="block font-semibold text-slate-700 mb-1">Engineering Branch</label>
            <input type="text" [(ngModel)]="profile.engineeringBranch" name="engineeringBranch" class="w-full px-3 py-2 border border-slate-300 rounded-lg" />
          </div>
          <div>
            <label class="block font-semibold text-slate-700 mb-1">Graduation %</label>
            <input type="number" step="0.01" [(ngModel)]="profile.graduationPercentage" name="graduationPercentage" class="w-full px-3 py-2 border border-slate-300 rounded-lg" />
          </div>
          <div>
            <label class="block font-semibold text-slate-700 mb-1">Work Experience (Months)</label>
            <input type="number" [(ngModel)]="profile.workExperienceMonths" name="workExperienceMonths" class="w-full px-3 py-2 border border-slate-300 rounded-lg" />
          </div>
          <div>
            <label class="block font-semibold text-slate-700 mb-1">GATE Score (if any)</label>
            <input type="number" [(ngModel)]="profile.gateScore" name="gateScore" class="w-full px-3 py-2 border border-slate-300 rounded-lg" />
          </div>
        </div>
        <div class="flex flex-wrap gap-5 pt-2">
          <label class="flex items-center gap-2"><input type="checkbox" [(ngModel)]="profile.isWorkingProfessional" name="isWorking" /> Working Professional</label>
          <label class="flex items-center gap-2"><input type="checkbox" [(ngModel)]="profile.hasValidGateScore" name="hasGate" /> Valid GATE Score</label>
          <label class="flex items-center gap-2"><input type="checkbox" [(ngModel)]="profile.hasBiharDomicileCertificate" name="hasDomicile" /> Domicile Certificate</label>
          <label class="flex items-center gap-2"><input type="checkbox" [(ngModel)]="profile.hasValidNclOrEwsCertificate" name="hasCert" /> NCL / EWS / Caste Certificate</label>
        </div>
        <button type="submit" class="px-5 py-2 rounded-lg bg-[#0b192c] text-white font-semibold">Save Profile</button>
      </form>

      <!-- Notification Preferences -->
      <form (ngSubmit)="savePreferences()" class="card-surface p-5 space-y-4 text-xs">
        <h2 class="text-base font-bold text-slate-900">Notification & Email Alert Preferences</h2>
        <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-3">
          <label class="flex items-center gap-2"><input type="checkbox" [(ngModel)]="prefs.enableInApp" name="enableInApp" /> In-App Notifications</label>
          <label class="flex items-center gap-2"><input type="checkbox" [(ngModel)]="prefs.enableEmail" name="enableEmail" /> Email Alerts</label>
          <label class="flex items-center gap-2"><input type="checkbox" [(ngModel)]="prefs.alertOnNewNotification" name="alertNew" /> New Official Notification</label>
          <label class="flex items-center gap-2"><input type="checkbox" [(ngModel)]="prefs.alertOnApplicationOpen" name="alertOpen" /> Applications Open</label>
          <label class="flex items-center gap-2"><input type="checkbox" [(ngModel)]="prefs.alert30DaysBefore" name="alert30" /> 30 Days Before Deadline</label>
          <label class="flex items-center gap-2"><input type="checkbox" [(ngModel)]="prefs.alert7DaysBefore" name="alert7" /> 7 Days Before Deadline</label>
          <label class="flex items-center gap-2"><input type="checkbox" [(ngModel)]="prefs.alert3DaysBefore" name="alert3" /> 3 Days Before Deadline</label>
          <label class="flex items-center gap-2"><input type="checkbox" [(ngModel)]="prefs.alert1DayBefore" name="alert1" /> 1 Day Before Deadline</label>
          <label class="flex items-center gap-2"><input type="checkbox" [(ngModel)]="prefs.alertOnExamDateChange" name="alertChange" /> Exam Date Changes</label>
          <label class="flex items-center gap-2"><input type="checkbox" [(ngModel)]="prefs.alertOnAdmitCard" name="alertAdmit" /> Admit Card Released</label>
          <label class="flex items-center gap-2"><input type="checkbox" [(ngModel)]="prefs.alertOnResult" name="alertResult" /> Result Declared</label>
        </div>
        <div class="max-w-sm">
          <label class="block font-semibold text-slate-700 mb-1">Notification Email Override (Optional)</label>
          <input type="email" [(ngModel)]="prefs.overrideEmail" name="overrideEmail" class="w-full px-3 py-2 border border-slate-300 rounded-lg" />
        </div>
        <button type="submit" class="px-5 py-2 rounded-lg bg-blue-600 hover:bg-blue-700 text-white font-semibold">Save Alert Preferences</button>
      </form>
    </div>
  `
})
export class SettingsComponent implements OnInit {
  private readonly auth = inject(AuthService);
  private readonly api = inject(ApiService);

  savedBanner = signal<string | null>(null);
  profile = {
    fullName: '', dateOfBirth: '', gender: 'Male', category: 'General', domicileState: 'Bihar',
    graduationQualification: 'B.Tech', engineeringBranch: 'Information Technology',
    graduationPercentage: 75 as number | null, workExperienceMonths: 0, isWorkingProfessional: false,
    hasValidGateScore: false, gateSubject: 'CS', gateScore: null as number | null,
    hasBiharDomicileCertificate: false, hasValidNclOrEwsCertificate: false, nationality: 'Indian'
  };

  prefs: NotificationPreference = {
    enableInApp: true, enableEmail: true, alertOnNewNotification: true, alertOnApplicationOpen: true,
    alert30DaysBefore: true, alert7DaysBefore: true, alert3DaysBefore: true, alert1DayBefore: true,
    alertOnExamDateChange: true, alertOnAdmitCard: true, alertOnResult: true, overrideEmail: ''
  };

  ngOnInit(): void {
    this.auth.refreshProfile().subscribe({
      next: (p) => { this.profile = { ...this.profile, ...p }; }
    });
    this.api.getNotificationPreferences().subscribe({
      next: (pr) => { this.prefs = pr; }
    });
  }

  saveProfile(): void {
    this.auth.updateProfile(this.profile).subscribe(() => {
      this.savedBanner.set('Candidate profile updated successfully.');
    });
  }

  savePreferences(): void {
    this.api.updateNotificationPreferences(this.prefs).subscribe(() => {
      this.savedBanner.set('Notification preferences saved.');
    });
  }
}
