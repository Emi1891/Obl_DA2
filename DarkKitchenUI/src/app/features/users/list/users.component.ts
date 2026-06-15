import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { UserManagementService } from '../../../core/services/user-management.service';
import { UserFilters, UserListItem } from '../../../core/models/user-management.model';
import { PhoneFormatPipe } from '../../../shared/pipes/phone-format.pipe';

@Component({
  selector: 'app-users',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink, PhoneFormatPipe],
  templateUrl: './users.component.html',
  styleUrl: './users.component.css'
})
export class UsersComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly auth = inject(AuthService);
  private readonly users = inject(UserManagementService);
  private readonly router = inject(Router);

  list: UserListItem[] = [];
  isLoading = false;
  errorMessage = '';
  feedback = '';

  filtersForm = this.fb.group({
    name: [''],
    surname: ['']
  });

  private readonly perms = new Set(this.auth.getPermissions());
  readonly canView = this.perms.has('GetUsers');
  readonly canCreate = this.perms.has('CreateUser');
  readonly canEdit = this.perms.has('UpdateUser');
  readonly canDelete = this.perms.has('DeleteUser');

  ngOnInit(): void {
    if (this.canView) this.load();
  }

  load(): void {
    const raw = this.filtersForm.value;
    const filters: UserFilters = {
      name: raw.name?.trim() || undefined,
      surname: raw.surname?.trim() || undefined
    };

    this.isLoading = true;
    this.errorMessage = '';
    this.users.getUsers(filters).subscribe({
      next: list => {
        this.list = list;
        this.isLoading = false;
      },
      error: () => {
        this.errorMessage = 'Could not load users.';
        this.isLoading = false;
      }
    });
  }

  clearFilters(): void {
    this.filtersForm.reset({ name: '', surname: '' });
    this.load();
  }

  edit(user: UserListItem): void {
    this.router.navigate(['/users', user.email, 'edit'], { state: { user } });
  }

  remove(user: UserListItem): void {
    if (!this.canDelete) return;
    const ok = confirm(`Delete user "${user.name} ${user.surname}" (${user.email})? This cannot be undone.`);
    if (!ok) return;

    this.users.deleteUser(user.email).subscribe({
      next: () => {
        this.list = this.list.filter(u => u.email !== user.email);
        this.feedback = `User "${user.email}" deleted.`;
      },
      error: err => {
        this.errorMessage = err?.error?.message ?? err?.error ?? `Could not delete "${user.email}".`;
      }
    });
  }

  roleClass(role: string): string {
    return `role role-${role.toLowerCase()}`;
  }

  trackByEmail(_idx: number, u: UserListItem) {
    return u.email;
  }
}
