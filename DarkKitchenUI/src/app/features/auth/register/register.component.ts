import { Component, inject } from '@angular/core';
import { ReactiveFormsModule, FormBuilder } from '@angular/forms';
import { Router } from '@angular/router';
import { UserService } from '../../../core/services/user.service';
import { formatPhoneDigits } from '../../../shared/phone';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})
export class RegisterComponent {
  private readonly fb = inject(FormBuilder);
  private readonly userService = inject(UserService);
  private readonly router = inject(Router);

  form = this.fb.group({
    name: [''],
    surname: [''],
    email: [''],
    phone: [''],
    password: [''],
    confirmPassword: ['']
  });

  errorMessage = '';
  showPassword = false;
  showConfirmPassword = false;

  togglePassword() {
    this.showPassword = !this.showPassword;
  }

  toggleConfirmPassword() {
    this.showConfirmPassword = !this.showConfirmPassword;
  }

  onPhoneInput(event: Event) {
    const digits = (event.target as HTMLInputElement).value.replace(/\D/g, '').slice(0, 9);
    this.form.get('phone')?.setValue(formatPhoneDigits(digits), { emitEvent: false });
  }

  onSubmit() {
    const { name, surname, email, phone, password, confirmPassword } = this.form.value;

    if (!name || !surname || !email || !phone || !password || !confirmPassword) {
      this.errorMessage = 'Please fill in all required fields.';
      return;
    }

    if (password !== confirmPassword) {
      this.errorMessage = 'Passwords do not match.';
      return;
    }

    this.errorMessage = '';
    this.userService.register({
      name,
      surname,
      email,
      phone: phone ? `+598${phone.replace(/\s/g, '')}` : undefined,
      password
    }).subscribe({
      next: () => this.router.navigate(['/login']),
      error: (err) => this.errorMessage = err.error?.message?.replace(/^[^:]+:\s*/, '') ?? ''
    });
  }

  goToLogin() {
    this.router.navigate(['/login']);
  }
}
