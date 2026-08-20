import { Routes } from '@angular/router';
import { MainLayout } from './layout/main-layout/main-layout';
import { ProductList } from './features/products/pages/product-list/product-list';
import { ProductCreate } from './features/products/pages/product-create/product-create';
import { InvoiceList } from './features/invoices/pages/invoice-list/invoice-list';
import { InvoiceCreate } from './features/invoices/pages/invoice-create/invoice-create';
import { InvoiceDetails } from './features/invoices/pages/invoice-details/invoice-details';

export const routes: Routes = [
  {
    path: '',
    component: MainLayout,
    children: [
      {
        path: '',
        redirectTo: 'products',
        pathMatch: 'full',
      },
      {
        path: 'products',
        component: ProductList,
      },
      {
        path: 'products/new',
        component: ProductCreate,
      },
      {
        path: 'invoices',
        component: InvoiceList,
      },
      {
        path: 'invoices/new',
        component: InvoiceCreate,
      },
      {
        path: 'invoices/:id',
        component: InvoiceDetails,
      },
    ],
  },
];
