import { Component, inject, OnInit, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';

import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatChipsModule } from '@angular/material/chips';
import { MatTableModule } from '@angular/material/table';

import { Invoice } from '../../../../core/models/invoice.model';
import { InvoiceService } from '../../../../core/services/invoice.service';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

@Component({
  selector: 'app-invoice-details',
  imports: [
    DatePipe,
    RouterLink,
    MatButtonModule,
    MatCardModule,
    MatChipsModule,
    MatTableModule,
    MatProgressSpinnerModule,
  ],
  templateUrl: './invoice-details.html',
  styleUrl: './invoice-details.scss',
})
export class InvoiceDetails implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly invoiceService = inject(InvoiceService);

  invoice = signal<Invoice | null>(null);
  isLoading = signal(true);
  isPrinting = signal(false);
  printErrorMessage = signal<string | null>(null);
  successMessage = signal<string | null>(null);
  errorMessage = signal<string | null>(null);

  displayedColumns: string[] = ['code', 'description', 'quantity'];

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');

    if (!id) {
      this.errorMessage.set('Identificador da nota fiscal não informado.');
      this.isLoading.set(false);
      return;
    }

    this.loadInvoice(id);
  }

  private loadInvoice(id: string): void {
    this.invoiceService.getById(id).subscribe({
      next: (invoice) => {
        this.invoice.set(invoice);
        this.isLoading.set(false);
      },
      error: (error) => {
        console.error('Error loading invoice:', error);

        this.errorMessage.set(error?.error?.detail ?? 'Não foi possível carregar a nota fiscal.');

        this.isLoading.set(false);
      },
    });
  }

  printInvoice(): void {
    const currentInvoice = this.invoice();

    if (!currentInvoice || currentInvoice.status !== 'Open') {
      return;
    }

    this.isPrinting.set(true);
    this.printErrorMessage.set(null);
    this.successMessage.set(null);

    this.invoiceService.print(currentInvoice.id).subscribe({
      next: (response) => {
        this.invoice.update((invoice) => {
          if (!invoice) {
            return invoice;
          }

          return {
            ...invoice,
            status: response.status,
            closedAt: response.closedAt,
          };
        });

        this.successMessage.set('Nota fiscal impressa e fechada com sucesso.');

        this.isPrinting.set(false);
      },

      error: (error) => {
        console.error('Error printing invoice:', error);

        if (error.status === 409) {
          this.printErrorMessage.set(
            error?.error?.detail ?? 'Não foi possível imprimir a nota fiscal.',
          );
        } else if (error.status === 503) {
          this.printErrorMessage.set(
            'O serviço de estoque está indisponível no momento. Tente novamente em instantes.',
          );
        } else {
          this.printErrorMessage.set(
            error?.error?.detail ?? 'Não foi possível imprimir a nota fiscal.',
          );
        }

        this.isPrinting.set(false);
      },
    });
  }
}
