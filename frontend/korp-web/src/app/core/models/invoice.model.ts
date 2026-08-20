export type InvoiceStatus = 'Open' | 'Closed';

export interface InvoiceSummary {
  id: string;
  number: number;
  status: InvoiceStatus;
  createdAt: string;
  closedAt: string | null;
}

export interface InvoiceItem {
  productId: string;
  productCode: string;
  productDescription: string;
  quantity: number;
}

export interface Invoice extends InvoiceSummary {
  items: InvoiceItem[];
}

export interface CreateInvoiceItemRequest {
  productId: string;
  quantity: number;
}

export interface CreateInvoiceRequest {
  items: CreateInvoiceItemRequest[];
}

export interface PrintInvoiceResponse {
  id: string;
  number: number;
  status: InvoiceStatus;
  closedAt: string | null;
}
