import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';

import { InvoiceService } from './invoice.service';
import { CreateInvoiceRequest, Invoice, PrintInvoiceResponse } from '../models/invoice.model';

describe('InvoiceService', () => {
  let service: InvoiceService;
  let httpController: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [InvoiceService, provideHttpClient(), provideHttpClientTesting()],
    });

    service = TestBed.inject(InvoiceService);
    httpController = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpController.verify();
  });

  it('should get all invoices', () => {
    const mockInvoices: Invoice[] = [
      {
        id: '11111111-1111-1111-1111-111111111111',
        number: 1,
        status: 'Open',
        createdAt: '2026-08-20T12:00:00Z',
        closedAt: null,
        items: [],
      },
    ];

    service.getAll().subscribe((invoices) => {
      expect(invoices).toEqual(mockInvoices);
    });

    const request = httpController.expectOne('https://localhost:7066/api/invoices');

    expect(request.request.method).toBe('GET');

    request.flush(mockInvoices);
  });

  it('should get invoice by id', () => {
    const invoiceId = '11111111-1111-1111-1111-111111111111';

    const mockInvoice: Invoice = {
      id: invoiceId,
      number: 1,
      status: 'Open',
      createdAt: '2026-08-20T12:00:00Z',
      closedAt: null,
      items: [
        {
          productId: '22222222-2222-2222-2222-222222222222',
          productCode: 'PROD-001',
          productDescription: 'Notebook Dell',
          quantity: 2,
        },
      ],
    };

    service.getById(invoiceId).subscribe((invoice) => {
      expect(invoice).toEqual(mockInvoice);
    });

    const request = httpController.expectOne(`https://localhost:7066/api/invoices/${invoiceId}`);

    expect(request.request.method).toBe('GET');

    request.flush(mockInvoice);
  });

  it('should create an invoice', () => {
    const requestBody: CreateInvoiceRequest = {
      items: [
        {
          productId: '22222222-2222-2222-2222-222222222222',
          quantity: 2,
        },
      ],
    };

    const mockResponse: Invoice = {
      id: '11111111-1111-1111-1111-111111111111',
      number: 1,
      status: 'Open',
      createdAt: '2026-08-20T12:00:00Z',
      closedAt: null,
      items: [
        {
          productId: requestBody.items[0].productId,
          productCode: 'PROD-001',
          productDescription: 'Notebook Dell',
          quantity: 2,
        },
      ],
    };

    service.create(requestBody).subscribe((invoice) => {
      expect(invoice).toEqual(mockResponse);
    });

    const request = httpController.expectOne('https://localhost:7066/api/invoices');

    expect(request.request.method).toBe('POST');
    expect(request.request.body).toEqual(requestBody);

    request.flush(mockResponse);
  });

  it('should print an invoice', () => {
    const invoiceId = '11111111-1111-1111-1111-111111111111';

    const mockResponse: PrintInvoiceResponse = {
      id: invoiceId,
      number: 1,
      status: 'Closed',
      closedAt: '2026-08-20T12:30:00Z',
    };

    service.print(invoiceId).subscribe((response) => {
      expect(response).toEqual(mockResponse);
    });

    const request = httpController.expectOne(
      `https://localhost:7066/api/invoices/${invoiceId}/print`,
    );

    expect(request.request.method).toBe('POST');
    expect(request.request.body).toEqual({});

    request.flush(mockResponse);
  });
});
