import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ActivatedRoute, provideRouter } from '@angular/router';
import { of, throwError } from 'rxjs';

import { InvoiceDetails } from './invoice-details';
import { InvoiceService } from '../../../../core/services/invoice.service';
import { Invoice } from '../../../../core/models/invoice.model';

describe('InvoiceDetails', () => {
  let component: InvoiceDetails;
  let fixture: ComponentFixture<InvoiceDetails>;

  let invoiceServiceMock: {
    getById: ReturnType<typeof vi.fn>;
    print: ReturnType<typeof vi.fn>;
  };

  const invoiceId = '11111111-1111-1111-1111-111111111111';

  const openInvoice: Invoice = {
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

  beforeEach(async () => {
    invoiceServiceMock = {
      getById: vi.fn(),
      print: vi.fn(),
    };

    invoiceServiceMock.getById.mockReturnValue(of(openInvoice));

    await TestBed.configureTestingModule({
      imports: [InvoiceDetails],
      providers: [
        provideRouter([]),

        {
          provide: ActivatedRoute,
          useValue: {
            snapshot: {
              paramMap: {
                get: () => invoiceId,
              },
            },
          },
        },

        {
          provide: InvoiceService,
          useValue: invoiceServiceMock,
        },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(InvoiceDetails);
    component = fixture.componentInstance;

    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should close invoice when printing succeeds', () => {
    invoiceServiceMock.print.mockReturnValue(
      of({
        id: invoiceId,
        number: 1,
        status: 'Closed',
        closedAt: '2026-08-20T12:30:00Z',
      }),
    );

    component.printInvoice();

    expect(component.invoice()?.status).toBe('Closed');

    expect(component.invoice()?.closedAt).toBe('2026-08-20T12:30:00Z');

    expect(component.successMessage()).toBe('Nota fiscal impressa e fechada com sucesso.');
  });

  it('should show service unavailable message when stock service is unavailable', () => {
    invoiceServiceMock.print.mockReturnValue(
      throwError(() => ({
        status: 503,
        error: {},
      })),
    );

    component.printInvoice();

    expect(component.invoice()?.status).toBe('Open');

    expect(component.printErrorMessage()).toBe(
      'O serviço de estoque está indisponível no momento. Tente novamente em instantes.',
    );
  });
});
