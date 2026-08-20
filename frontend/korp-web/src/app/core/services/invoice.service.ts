import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { CreateInvoiceRequest, Invoice, PrintInvoiceResponse } from '../models/invoice.model';

@Injectable({
  providedIn: 'root',
})
export class InvoiceService {
  private readonly http = inject(HttpClient);

  private readonly baseUrl = 'https://localhost:7066/api/invoices';

  getAll(): Observable<Invoice[]> {
    return this.http.get<Invoice[]>(this.baseUrl);
  }

  getById(id: string): Observable<Invoice> {
    return this.http.get<Invoice>(`${this.baseUrl}/${id}`);
  }

  create(request: CreateInvoiceRequest): Observable<Invoice> {
    return this.http.post<Invoice>(this.baseUrl, request);
  }

  print(id: string): Observable<PrintInvoiceResponse> {
    return this.http.post<PrintInvoiceResponse>(`${this.baseUrl}/${id}/print`, {});
  }
}
