import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { AuditService } from '../../core/services/audit.service';
import { AuditRecord } from '../../core/models/audit.model';

@Component({
  selector: 'app-audit',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './audit.component.html',
  styleUrl: './audit.component.css'
})
export class AuditComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly auth = inject(AuthService);
  private readonly auditService = inject(AuditService);
  private readonly router = inject(Router);

  readonly entityNames = ['Product', 'Promotion'];

  private readonly monthNames = [
    'January', 'February', 'March', 'April', 'May', 'June',
    'July', 'August', 'September', 'October', 'November', 'December'
  ];

  records: AuditRecord[] = [];
  hasSearched = false;
  isLoading = false;
  errorMessage = '';

  form = this.fb.group({
    entityName: ['Product', Validators.required],
    entityId: [null as number | null],
    dateFrom: ['', Validators.required],
    dateTo: ['', Validators.required]
  });

  ngOnInit(): void {
    if (this.auth.isTokenExpired()) {
      this.auth.logout(true);
      return;
    }
    const today = new Date();
    const monthAgo = new Date();
    monthAgo.setMonth(today.getMonth() - 1);
    this.form.patchValue({
      dateFrom: monthAgo.toISOString().slice(0, 10),
      dateTo: today.toISOString().slice(0, 10)
    });
    this.load();
  }

  load(): void {
    if (this.form.invalid) return;
    const { entityName, entityId, dateFrom, dateTo } = this.form.getRawValue();

    if (dateFrom! >= dateTo!) {
      this.errorMessage = 'From date must be before To date.';
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';
    this.auditService.getAuditRecords({
      entityName: entityName!,
      entityId: entityId ?? undefined,
      dateFrom: `${dateFrom}T00:00:00`,
      dateTo: `${dateTo}T23:59:59`
    }).subscribe({
      next: list => {
        this.records = list;
        this.hasSearched = true;
        this.isLoading = false;
      },
      error: () => {
        this.errorMessage = 'Could not load audit records.';
        this.isLoading = false;
      }
    });
  }

  formatDate(value: string): string {
    const utcValue = value.endsWith('Z') ? value : `${value}Z`;
    const date = new Date(utcValue);
    const day = date.getDate();
    const month = this.monthNames[date.getMonth()];
    const year = date.getFullYear();
    const hours = date.getHours().toString().padStart(2, '0');
    const minutes = date.getMinutes().toString().padStart(2, '0');
    return `${month} ${day}, ${year}, ${hours}:${minutes}`;
  }

  back(): void {
    this.router.navigate(['/home']);
  }
}
