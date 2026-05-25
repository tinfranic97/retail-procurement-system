import { Component, OnInit, OnDestroy, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { finalize, Subscription } from 'rxjs';
import { ApiService, StoreItem } from '../../services/api';
import { SignalRService } from '../../services/signalr';
import { AuthService } from '../../services/auth';

@Component({
  selector: 'app-store-items',
  imports: [CommonModule, FormsModule],
  templateUrl: './store-items.html',
  styleUrls: ['./store-items.scss'],
})
export class StoreItems implements OnInit, OnDestroy {
  items: StoreItem[] = [];
  loading = false;
  error = '';
  notification = '';
  showForm = false;
  isLoggedIn = false;

  form: Partial<StoreItem> = { name: '', description: '', price: 0, category: '', stockQuantity: 0 };

  private subs: Subscription[] = [];

  constructor(
    private api: ApiService,
    private signalR: SignalRService,
    private auth: AuthService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.loadItems();
    this.subs.push(this.auth.isLoggedIn$.subscribe(v => this.isLoggedIn = v));
    this.subs.push(this.signalR.storeItemCreated$.subscribe(item => {
      this.items = [...this.items, item];
      this.notify('New item added: ' + item.name);
    }));
    this.subs.push(this.signalR.storeItemUpdated$.subscribe(item => {
      this.items = this.items.map(i => i.id === item.id ? item : i);
      this.notify('Item updated: ' + item.name);
    }));
    this.subs.push(this.signalR.storeItemDeleted$.subscribe(id => {
      this.items = this.items.filter(i => i.id !== id);
      this.notify('Item deleted');
    }));
  }

  loadItems(): void {
    this.loading = true;
    this.error = '';

    this.api.getStoreItems()
      .pipe(
        finalize(() => {
          this.loading = false;
          this.cdr.detectChanges();
        })
      )
      .subscribe({
        next: items => {
          this.items = items;
        },
        error: () => this.flashError('Failed to load items.')
      });
  }

  submitForm(): void {
    this.api.createStoreItem(this.form)
    .pipe(
      finalize(() => {
        this.cdr.detectChanges();
      })
    )
    .subscribe({
      next: () => { this.showForm = false; this.resetForm(); },
      error: () => this.flashError('Failed to create item.')
    });
  }

  delete(id: number): void {
    if (!confirm('Delete this item?')) return;
    this.api.deleteStoreItem(id)
    .pipe(
      finalize(() => {
        this.loadItems();
      })
    )
    .subscribe({ error: () => this.flashError('Failed to delete item.') });
  }

  resetForm(): void {
    this.form = { name: '', description: '', price: 0, category: '', stockQuantity: 0 };
  }

  private notify(msg: string): void {
    this.notification = msg;
    setTimeout(() => { this.notification = ''; this.cdr.detectChanges(); }, 3000);
  }

  private flashError(msg: string): void {
    this.error = msg;
    setTimeout(() => { this.error = ''; this.cdr.detectChanges(); }, 3000);
  }

  ngOnDestroy(): void {
    this.subs.forEach(s => s.unsubscribe());
  }
}
