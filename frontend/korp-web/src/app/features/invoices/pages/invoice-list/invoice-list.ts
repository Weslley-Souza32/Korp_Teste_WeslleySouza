import { Component, inject, OnInit, signal } from '@angular/core';
import { RouterLink } from '@angular/router';

import { MatButtonModule } from '@angular/material/button';
import { MatChipsModule } from '@angular/material/chips';
import { MatTableModule } from '@angular/material/table';
import { DatePipe } from '@angular/common';

import { InvoiceSummary } from '../../../../core/models/invoice.model';
import { InvoiceService } from '../../../../core/services/invoice.service';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

@Component({
  selector: 'app-invoice-list',
  imports: [
    RouterLink,
    MatButtonModule,
    MatChipsModule,
    MatTableModule,
    DatePipe,
    MatProgressSpinnerModule,
  ],
  templateUrl: './invoice-list.html',
  styleUrl: './invoice-list.scss',
})
export class InvoiceList implements OnInit {
  private readonly invoiceService = inject(InvoiceService);

  invoices = signal<InvoiceSummary[]>([]);
  isLoading = signal(true);
  errorMessage = signal<string | null>(null);

  displayedColumns: string[] = ['number', 'status', 'createdAt', 'closedAt', 'actions'];

  ngOnInit(): void {
    this.loadInvoices();
  }

  private loadInvoices(): void {
    this.isLoading.set(true);
    this.errorMessage.set(null);

    this.invoiceService.getAll().subscribe({
      next: (invoices) => {
        this.invoices.set(invoices);
        this.isLoading.set(false);
      },
      error: (error) => {
        console.error('Error loading invoices:', error);

        this.errorMessage.set('Não foi possível carregar as notas fiscais.');

        this.isLoading.set(false);
      },
    });
  }
}
