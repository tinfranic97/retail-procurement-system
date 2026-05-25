import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ApiService, Supplier, SupplierStatistics } from '../../services/api';
import { AuthService } from '../../services/auth';
import { finalize } from 'rxjs/internal/operators/finalize';

@Component({
  selector: 'app-suppliers',
  imports: [CommonModule, FormsModule],
  templateUrl: './suppliers.html',
  styleUrl: './suppliers.scss',
})
export class Suppliers implements OnInit {
  suppliers: Supplier[] = [];
  loading = false;
  error = '';
  notification = '';
  showForm = false;
  isLoggedIn = false;
  selectedStats: SupplierStatistics | null = null;

  form: Partial<Supplier> = { name: '', email: '', phone: '', address: '', contactPerson: '' };

  constructor(private api: ApiService, private auth: AuthService, private cdr: ChangeDetectorRef) {}

  ngOnInit(): void {
    this.loadSuppliers();
    this.auth.isLoggedIn$.subscribe(v => this.isLoggedIn = v);
  }

  loadSuppliers(): void {
    this.loading = true;

    this.api.getSuppliers()
    .pipe(
      finalize(() => {
      this.loading = false;
      this.cdr.detectChanges();
    }))
    .subscribe({
      next: s => {
         this.suppliers = s; this.loading = false; 
      },
      error: () => { 
        this.flashError('Failed to load suppliers.'); this.loading = false;
      }
    });
  }

  submitForm(): void {
    this.api.createSupplier(this.form)
    .pipe(
      finalize(() => {
        this.cdr.detectChanges();
      })
    )
    .subscribe({
      next: s => {
        this.suppliers = [...this.suppliers, s];
        this.showForm = false;
        this.form = { name: '', email: '', phone: '', address: '', contactPerson: '' };
      },
      error: () => this.flashError('Failed to create supplier.')
    });
  }

  private flashError(msg: string): void {
    this.error = msg;
    setTimeout(() => { this.error = ''; this.cdr.detectChanges(); }, 3000);
  }

  viewStats(id: number): void {
    this.api.getSupplierStatistics(id)
    .pipe(
      finalize(() => {
      this.cdr.detectChanges();
    }))
    .subscribe({
      next: stats => this.selectedStats = stats,
      error: () => this.flashError('No statistics available for this supplier.')
    });
  }
}
