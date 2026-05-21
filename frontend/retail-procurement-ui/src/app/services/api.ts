import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuthService } from './auth';

export interface StoreItem {
  id: number;
  name: string;
  description: string;
  price: number;
  category: string;
  stockQuantity: number;
  createdAt: string;
  updatedAt: string;
}

export interface Supplier {
  id: number;
  name: string;
  email: string;
  phone: string;
  address: string;
  contactPerson: string;
  createdAt: string;
  updatedAt: string;
}

export interface SupplierStatistics {
  supplierId: number;
  supplierName: string;
  totalItemsSold: number;
  totalRevenue: number;
  itemBreakdown: { storeItemId: number; itemName: string; quantitySold: number; revenue: number }[];
}

export interface BestOffer {
  storeItemId: number;
  storeItemName: string;
  supplierId: number;
  supplierName: string;
  supplierPrice: number;
  leadTimeDays: number;
  selectionCriteria: string;
}

export interface QuarterlyPlan {
  id: number;
  supplierId: number;
  supplierName: string;
  quarter: number;
  year: number;
  plannedBudget: number;
  notes: string;
}

@Injectable({ providedIn: 'root' })
export class ApiService {
  private readonly baseUrl = '/api';

  constructor(private http: HttpClient, private auth: AuthService) {}

  private get headers(): HttpHeaders {
    const token = this.auth.token;
    return token ? new HttpHeaders({ Authorization: `Bearer ${token}` }) : new HttpHeaders();
  }

  getStoreItems(): Observable<StoreItem[]> {
    return this.http.get<StoreItem[]>(`${this.baseUrl}/store-items`);
  }

  createStoreItem(item: Partial<StoreItem>): Observable<StoreItem> {
    return this.http.post<StoreItem>(`${this.baseUrl}/store-items`, item, { headers: this.headers });
  }

  updateStoreItem(id: number, item: Partial<StoreItem>): Observable<StoreItem> {
    return this.http.put<StoreItem>(`${this.baseUrl}/store-items/${id}`, item, { headers: this.headers });
  }

  deleteStoreItem(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/store-items/${id}`, { headers: this.headers });
  }

  getSuppliers(): Observable<Supplier[]> {
    return this.http.get<Supplier[]>(`${this.baseUrl}/suppliers`);
  }

  createSupplier(supplier: Partial<Supplier>): Observable<Supplier> {
    return this.http.post<Supplier>(`${this.baseUrl}/suppliers`, supplier, { headers: this.headers });
  }

  getSupplierStatistics(id: number): Observable<SupplierStatistics> {
    return this.http.get<SupplierStatistics>(`${this.baseUrl}/statistics/supplier/${id}`);
  }

  getBestOffer(productId: number): Observable<BestOffer> {
    return this.http.get<BestOffer>(`${this.baseUrl}/statistics/best-offer/${productId}`);
  }

  getCurrentQuarterlyPlan(): Observable<QuarterlyPlan[]> {
    return this.http.get<QuarterlyPlan[]>(`${this.baseUrl}/statistics/quarterly-plan`);
  }
}
