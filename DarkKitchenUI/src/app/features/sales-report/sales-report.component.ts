import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../core/services/auth.service';
import { OrderService } from '../../core/services/order.service';
import { SalesReport } from '../../core/models/sales-report.model';

@Component({
  selector: 'app-sales-report',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './sales-report.component.html',
  styleUrl: './sales-report.component.css'
})
export class SalesReportComponent implements OnInit {
  private readonly auth = inject(AuthService);
  private readonly orderService = inject(OrderService);

  private readonly monthNames = [
    'January', 'February', 'March', 'April', 'May', 'June',
    'July', 'August', 'September', 'October', 'November', 'December'
  ];

  report: SalesReport | null = null;
  isLoading = false;
  errorMessage = '';

  ngOnInit(): void {
    if (this.auth.isTokenExpired()) {
      this.auth.logout(true);
      return;
    }
    this.load();
  }

  load(): void {
    this.isLoading = true;
    this.errorMessage = '';
    this.orderService.getSalesReport().subscribe({
      next: report => {
        this.report = report;
        this.isLoading = false;
      },
      error: () => {
        this.errorMessage = 'Could not load report.';
        this.isLoading = false;
      }
    });
  }

  monthLabel(year: number, month: number): string {
    return `${this.monthNames[month - 1]} ${year}`;
  }
}
