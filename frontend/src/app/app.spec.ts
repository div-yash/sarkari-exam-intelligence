import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { App } from './app';
import { EligibilityBadgeComponent, StatusBadgeComponent } from './shared/components/status-badge.component';

describe('App & Shared Badges', () => {
  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [App, StatusBadgeComponent, EligibilityBadgeComponent],
      providers: [provideRouter([])]
    }).compileComponents();
  });

  it('should create the root application', () => {
    const fixture = TestBed.createComponent(App);
    expect(fixture.componentInstance).toBeTruthy();
  });

  it('should render red Closing Soon badge for urgent deadlines', () => {
    const fixture = TestBed.createComponent(StatusBadgeComponent);
    fixture.componentInstance.status = 'ClosingSoon';
    fixture.detectChanges();
    const el = fixture.nativeElement as HTMLElement;
    expect(el.textContent).toContain('Closing Soon');
    expect(el.innerHTML).toContain('bg-red-50');
  });

  it('should render explicit Potentially Eligible label', () => {
    const fixture = TestBed.createComponent(EligibilityBadgeComponent);
    fixture.componentInstance.status = 'PotentiallyEligibleVerificationRequired';
    fixture.detectChanges();
    const el = fixture.nativeElement as HTMLElement;
    expect(el.textContent).toContain('Potentially Eligible – Verify');
  });
});
