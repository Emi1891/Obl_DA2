export interface ClientSalesLine {
  clientName: string;
  total: number;
}

export interface MonthlySales {
  year: number;
  month: number;
  lines: ClientSalesLine[];
  total: number;
}

export interface SalesReport {
  months: MonthlySales[];
  total: number;
}
