import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { Subject } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class SignalRService {
  private hubConnection: signalR.HubConnection | null = null;

  storeItemCreated$ = new Subject<any>();
  storeItemUpdated$ = new Subject<any>();
  storeItemDeleted$ = new Subject<number>();

  startConnection(): void {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('http://localhost:8080/hubs/procurement')
      .withAutomaticReconnect()
      .build();

    this.hubConnection.on('StoreItemCreated', (item) => this.storeItemCreated$.next(item));
    this.hubConnection.on('StoreItemUpdated', (item) => this.storeItemUpdated$.next(item));
    this.hubConnection.on('StoreItemDeleted', (id) => this.storeItemDeleted$.next(id));

    this.hubConnection.start().catch(err => console.error('SignalR connection error:', err));
  }

  stopConnection(): void {
    this.hubConnection?.stop();
  }
}
