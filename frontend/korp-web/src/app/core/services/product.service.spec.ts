import { TestBed } from '@angular/core/testing';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { provideHttpClient } from '@angular/common/http';

import { ProductService } from './product.service';
import { Product } from '../models/product.model';

describe('ProductService', () => {
  let service: ProductService;
  let httpController: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [ProductService, provideHttpClient(), provideHttpClientTesting()],
    });

    service = TestBed.inject(ProductService);
    httpController = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpController.verify();
  });

  it('should get all products', () => {
    const mockProducts: Product[] = [
      {
        id: '11111111-1111-1111-1111-111111111111',
        code: 'PROD-001',
        description: 'Notebook Dell',
        stockQuantity: 10,
      },
    ];

    service.getAll().subscribe((products) => {
      expect(products).toEqual(mockProducts);
    });

    const request = httpController.expectOne('https://localhost:7200/api/products');

    expect(request.request.method).toBe('GET');

    request.flush(mockProducts);
  });

  it('should create a product', () => {
    const requestBody = {
      code: 'PROD-002',
      description: 'Mouse Logitech',
      stockQuantity: 20,
    };

    const mockResponse: Product = {
      id: '22222222-2222-2222-2222-222222222222',
      ...requestBody,
    };

    service.create(requestBody).subscribe((product) => {
      expect(product).toEqual(mockResponse);
    });

    const request = httpController.expectOne('https://localhost:7200/api/products');

    expect(request.request.method).toBe('POST');
    expect(request.request.body).toEqual(requestBody);

    request.flush(mockResponse);
  });
});
