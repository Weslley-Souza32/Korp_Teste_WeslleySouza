import { Component, inject, OnInit, signal } from '@angular/core';
import { RouterLink } from '@angular/router';

import { MatButtonModule } from '@angular/material/button';
import { MatChipsModule } from '@angular/material/chips';
import { MatTableModule } from '@angular/material/table';
import { DatePipe } from '@angular/common';

import { Invoice } from '../../../../core/models/invoice.model';
import { InvoiceService } from '../../../../core/services/invoice.service';

@Component({
  selector: 'app-invoice-list',
  imports: [RouterLink, MatButtonModule, MatChipsModule, MatTableModule, DatePipe],
  templateUrl: './invoice-list.html',
  styleUrl: './invoice-list.scss',
})
export class InvoiceList implements OnInit {
  private readonly invoiceService = inject(InvoiceService);

  invoices = signal<Invoice[]>([]);

  displayedColumns: string[] = ['number', 'status', 'createdAt', 'closedAt', 'actions'];

  ngOnInit(): void {
    this.loadInvoices();
  }

  private loadInvoices(): void {
    this.invoiceService.getAll().subscribe({
      next: (invoices) => {
        this.invoices.set(invoices);
      },
      error: (error) => {
        console.error('Error loading invoices:', error);
      },
    });
  }
}
