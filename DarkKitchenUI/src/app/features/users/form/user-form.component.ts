import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { UserManagementService } from '../../../core/services/user-management.service';
import {
  CreateUserPayload,
  UpdateUserPayload,
  UserListItem,
  UserRole
} from '../../../core/models/user-management.model';
import { formatPhoneDigits, toLocalPhone } from '../../../shared/phone';

@Component({
  selector: 'app-user-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './user-form.component.html',
  styleUrl: './user-form.component.css'
})
export class UserFormComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly auth = inject(AuthService);
  private readonly users = inject(UserManagementService);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);

  isEdit = false;
  originalEmail: string | null = null;
  errorMessage = '';
  isSubmitting = false;
  showPassword = false;

  readonly roles: UserRole[] = ['Admin', 'Client', 'Dispatcher'];

  form = this.fb.group({
    name: ['', Validators.required],
    surname: ['', Validators.required],
    email: ['', [Validators.required, Validators.email]],
    phone: ['', Validators.required],
    password: [''],
    role: ['Client' as UserRole, Validators.required]
  });

  ngOnInit(): void {
    if (this.auth.isTokenExpired()) {
      this.auth.logout(true);
      return;
    }

    const emailParam = this.route.snapshot.paramMap.get('email');
    if (emailParam) {
      this.isEdit = true;
      this.originalEmail = emailParam;
      this.form.controls.email.disable();
      this.form.controls.role.disable();

      const navState = history.state as { user?: UserListItem };
      if (navState?.user && navState.user.email === emailParam) {
        this.form.patchValue({
          name: navState.user.name,
          surname: navState.user.surname,
          email: navState.user.email,
          phone: toLocalPhone(navState.user.phone),
          role: navState.user.role
        });
      } else {
        this.form.patchValue({ email: emailParam });
      }
    } else {
      this.form.controls.password.addValidators([Validators.required]);
      this.form.controls.password.updateValueAndValidity();
    }
  }

  togglePassword(): void {
    this.showPassword = !this.showPassword;
  }

  onPhoneInput(event: Event): void {
    const digits = (event.target as HTMLInputElement).value.replace(/\D/g, '').slice(0, 9);
    this.form.get('phone')?.setValue(formatPhoneDigits(digits), { emitEvent: false });
  }

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.isSubmitting = true;
    this.errorMessage = '';
    const raw = this.form.getRawValue();

    if (this.isEdit && this.originalEmail) {
      const payload: UpdateUserPayload = {
        email: this.originalEmail,
        name: raw.name || undefined,
        surname: raw.surname || undefined,
        phone: raw.phone ? `+598${raw.phone.replace(/\s/g, '')}` : undefined,
        password: raw.password ? raw.password : undefined
      };
      this.users.updateUser(payload).subscribe({
        next: () => this.router.navigate(['/users']),
        error: err => {
          this.errorMessage = this.extractError(err) ?? 'Could not update user.';
          this.isSubmitting = false;
        }
      });
    } else {
      const payload: CreateUserPayload = {
        name: raw.name!,
        surname: raw.surname!,
        email: raw.email!,
        phone: `+598${raw.phone!.replace(/\s/g, '')}`,
        password: raw.password!,
        role: raw.role!
      };
      this.users.createUser(payload).subscribe({
        next: () => this.router.navigate(['/users']),
        error: err => {
          this.errorMessage = this.extractError(err) ?? 'Could not create user.';
          this.isSubmitting = false;
        }
      });
    }
  }

  private extractError(err: unknown): string | null {
    const e = err as { error?: { message?: string } | string };
    if (typeof e.error === 'string') return e.error.replace(/^[^:]+:\s*/, '');
    return e.error?.message?.replace(/^[^:]+:\s*/, '') ?? null;
  }

  cancel(): void {
    this.router.navigate(['/users']);
  }
}
