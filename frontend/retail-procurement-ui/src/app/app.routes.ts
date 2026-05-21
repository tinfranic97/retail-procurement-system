import { Routes } from '@angular/router';
import { StoreItems } from './components/store-items/store-items';
import { Suppliers } from './components/suppliers/suppliers';
import { Statistics } from './components/statistics/statistics';
import { Login } from './components/login/login';

export const routes: Routes = [
  { path: '', redirectTo: '/store-items', pathMatch: 'full' },
  { path: 'store-items', component: StoreItems },
  { path: 'suppliers', component: Suppliers },
  { path: 'statistics', component: Statistics },
  { path: 'login', component: Login },
];
