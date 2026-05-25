import { ChangeDetectorRef, Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { AuthService } from '../../services/auth';
import { finalize } from 'rxjs/internal/operators/finalize';

@Component({
  selector: 'app-login',
  imports: [CommonModule, FormsModule],
  templateUrl: './login.html',
  styleUrl: './login.scss',
})
export class Login {
  mode: 'login' | 'register' = 'login';
  username = '';
  email = '';
  password = '';
  error = '';
  loading = false;

  constructor(private auth: AuthService, private router: Router, private cdr: ChangeDetectorRef) {}

  submit(): void {
    this.loading = true;
    this.error = '';

    const obs = this.mode === 'login'
      ? this.auth.login(this.username, this.password)
      : this.auth.register(this.username, this.email, this.password);

    obs.pipe(
      finalize(() => {
        this.cdr.detectChanges();
      })
    ).subscribe({
      next: () => { this.loading = false; this.router.navigate(['/store-items']); },
      error: (err: HttpErrorResponse) => {
        this.loading = false;
        this.error = err.error?.message ?? 'Authentication failed. Please try again.';
      }
    });
  }
}
