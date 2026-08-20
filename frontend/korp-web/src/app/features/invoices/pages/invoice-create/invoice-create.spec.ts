import { ComponentFixture, TestBed } from '@angular/core/testing';
import { InvoiceCreate } from './invoice-create';
import { provideRouter } from '@angular/router';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';

describe('InvoiceCreate', () => {
  let component: InvoiceCreate;
  let fixture: ComponentFixture<InvoiceCreate>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [InvoiceCreate],
      providers: [provideRouter([]), provideHttpClient(), provideHttpClientTesting()],
    }).compileComponents();

    fixture = TestBed.createComponent(InvoiceCreate);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should add a new invoice item', () => {
    const initialLength = component.items.length;

    component.addItem();

    expect(component.items.length).toBe(initialLength + 1);
  });

  it('should remove an invoice item when more than one exists', () => {
    component.addItem();

    expect(component.items.length).toBe(2);

    component.removeItem(1);

    expect(component.items.length).toBe(1);
  });

  it('should keep at least one invoice item', () => {
    expect(component.items.length).toBe(1);

    component.removeItem(0);

    expect(component.items.length).toBe(1);
  });
});
