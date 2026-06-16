import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { OrderService } from '../../core/services/order.service';
import { SalesReport } from '../../core/models/sales-report.model';
import { MONTH_NAMES } from '../../shared/utils/date';

@Component({
  selector: 'app-sales-report',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './sales-report.component.html',
  styleUrl: './sales-report.component.css'
})
export class SalesReportComponent implements OnInit {
  private readonly orderService = inject(OrderService);

  report: SalesReport | null = null;
  isLoading = false;
  errorMessage = '';

  ngOnInit(): void {
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
    return `${MONTH_NAMES[month - 1]} ${year}`;
  }
}
