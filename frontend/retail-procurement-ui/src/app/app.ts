import { Component, OnInit } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Navbar } from './components/navbar/navbar';
import { SignalRService } from './services/signalr';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, Navbar],
  template: `
    <app-navbar></app-navbar>
    <main>
      <router-outlet></router-outlet>
    </main>
  `,
  styleUrl: './app.scss'
})
export class App implements OnInit {
  constructor(private signalR: SignalRService) {}

  ngOnInit(): void {
    this.signalR.startConnection();
  }
}
