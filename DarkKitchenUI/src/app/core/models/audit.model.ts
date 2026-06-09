export interface AuditRecord {
  id: number;
  dateTime: string;
  entityName: string;
  entityId: number;
  changeDescription: string;
  responsibleUser: string;
}

export interface AuditFilter {
  entityName: string;
  entityId?: number;
  dateFrom: string;
  dateTo: string;
}
