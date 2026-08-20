import { Component, inject, OnInit, signal } from '@angular/core';
import { RouterLink } from '@angular/router';

import { MatButtonModule } from '@angular/material/button';
import { MatTableModule } from '@angular/material/table';

import { Product } from '../../../../core/models/product.model';
import { ProductService } from '../../../../core/services/product.service';

@Component({
  selector: 'app-product-list',
  imports: [MatTableModule, MatButtonModule, RouterLink],
  templateUrl: './product-list.html',
  styleUrl: './product-list.scss',
})
export class ProductList implements OnInit {
  private readonly productService = inject(ProductService);

  products = signal<Product[]>([]);

  displayedColumns: string[] = ['code', 'description', 'stockQuantity'];

  ngOnInit(): void {
    this.loadProducts();
  }

  private loadProducts(): void {
    this.productService.getAll().subscribe({
      next: (products) => {
        this.products.set(products);
      },
      error: (error) => {
        console.error('Error loading products:', error);
      },
    });
  }
}
