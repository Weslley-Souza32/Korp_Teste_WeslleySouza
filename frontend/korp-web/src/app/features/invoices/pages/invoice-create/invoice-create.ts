import { Component, inject, OnInit, signal } from '@angular/core';
import { FormArray, FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';

import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSelectModule } from '@angular/material/select';

import { Product } from '../../../../core/models/product.model';
import { InvoiceService } from '../../../../core/services/invoice.service';
import { ProductService } from '../../../../core/services/product.service';

@Component({
  selector: 'app-invoice-create',
  imports: [
    ReactiveFormsModule,
    RouterLink,
    MatButtonModule,
    MatCardModule,
    MatFormFieldModule,
    MatIconModule,
    MatInputModule,
    MatProgressSpinnerModule,
    MatSelectModule,
  ],
  templateUrl: './invoice-create.html',
  styleUrl: './invoice-create.scss',
})
export class InvoiceCreate implements OnInit {
  private readonly formBuilder = inject(FormBuilder);
  private readonly invoiceService = inject(InvoiceService);
  private readonly productService = inject(ProductService);
  private readonly router = inject(Router);

  products = signal<Product[]>([]);
  isSubmitting = signal(false);
  errorMessage = signal<string | null>(null);

  form = this.formBuilder.nonNullable.group({
    items: this.formBuilder.array([this.createItemGroup()]),
  });

  get items(): FormArray {
    return this.form.controls.items;
  }

  ngOnInit(): void {
    this.loadProducts();
  }

  addItem(): void {
    this.items.push(this.createItemGroup());
  }

  removeItem(index: number): void {
    if (this.items.length === 1) {
      return;
    }

    this.items.removeAt(index);
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.isSubmitting.set(true);
    this.errorMessage.set(null);

    this.invoiceService.create(this.form.getRawValue()).subscribe({
      next: (invoice) => {
        this.router.navigate(['/invoices', invoice.id]);
      },
      error: (error) => {
        console.error('Error creating invoice:', error);

        const validationErrors = error?.error?.errors;

        if (validationErrors) {
          const messages = Object.values(validationErrors).flat().join(' ');

          this.errorMessage.set(messages);
        } else {
          this.errorMessage.set(error?.error?.detail ?? 'Não foi possível criar a nota fiscal.');
        }

        this.isSubmitting.set(false);
      },
    });
  }

  private createItemGroup() {
    return this.formBuilder.nonNullable.group({
      productId: ['', Validators.required],
      quantity: [1, [Validators.required, Validators.min(1)]],
    });
  }

  private loadProducts(): void {
    this.productService.getAll().subscribe({
      next: (products) => {
        this.products.set(products);
      },
      error: (error) => {
        console.error('Error loading products:', error);

        this.errorMessage.set('Não foi possível carregar os produtos.');
      },
    });
  }
}
