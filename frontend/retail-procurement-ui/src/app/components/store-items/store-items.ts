import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Subscription } from 'rxjs';
import { ApiService, StoreItem } from '../../services/api';
import { SignalRService } from '../../services/signalr';
import { AuthService } from '../../services/auth';

@Component({
  selector: 'app-store-items',
  imports: [CommonModule, FormsModule],
  templateUrl: './store-items.html',
  styleUrl: './store-items.scss',
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
    private auth: AuthService
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
    this.api.getStoreItems().subscribe({
      next: items => { this.items = items; this.loading = false; },
      error: () => { this.error = 'Failed to load items.'; this.loading = false; }
    });
  }

  submitForm(): void {
    this.api.createStoreItem(this.form).subscribe({
      next: () => { this.showForm = false; this.resetForm(); },
      error: () => this.error = 'Failed to create item.'
    });
  }

  delete(id: number): void {
    if (!confirm('Delete this item?')) return;
    this.api.deleteStoreItem(id).subscribe({ error: () => this.error = 'Failed to delete item.' });
  }

  resetForm(): void {
    this.form = { name: '', description: '', price: 0, category: '', stockQuantity: 0 };
  }

  private notify(msg: string): void {
    this.notification = msg;
    setTimeout(() => this.notification = '', 3000);
  }

  ngOnDestroy(): void {
    this.subs.forEach(s => s.unsubscribe());
  }
}
