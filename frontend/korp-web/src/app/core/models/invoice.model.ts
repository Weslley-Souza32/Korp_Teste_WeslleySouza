export interface InvoiceItem {
  productId: string;
  productCode: string;
  productDescription: string;
  quantity: number;
}

export interface Invoice {
  id: string;
  number: number;
  status: 'Open' | 'Closed';
  createdAt: string;
  closedAt: string | null;
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
  status: 'Open' | 'Closed';
  closedAt: string | null;
}
