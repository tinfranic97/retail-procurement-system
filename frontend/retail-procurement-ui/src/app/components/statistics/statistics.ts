import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ApiService, BestOffer, QuarterlyPlan } from '../../services/api';
import { finalize } from 'rxjs/internal/operators/finalize';

@Component({
  selector: 'app-statistics',
  imports: [CommonModule, FormsModule],
  templateUrl: './statistics.html',
  styleUrl: './statistics.scss',
})
export class Statistics implements OnInit {
  quarterlyPlans: QuarterlyPlan[] = [];
  bestOffer: BestOffer | null = null;
  productIdInput = '';
  loading = false;
  error = '';

  constructor(private api: ApiService, private cdr: ChangeDetectorRef) {}

  ngOnInit(): void {
    this.loadQuarterlyPlan();
  }

  loadQuarterlyPlan(): void {
    this.loading = true;
    this.api.getCurrentQuarterlyPlan()
    .pipe(finalize(() => {
      this.loading = false;
      this.cdr.detectChanges();
    }))
    .subscribe({
      next: plans => { this.quarterlyPlans = plans; this.loading = false; },
      error: () => { this.error = 'Failed to load quarterly plan.'; this.loading = false; }
    });
  }

  lookupBestOffer(): void {
    const id = parseInt(this.productIdInput);
    if (!id) return;
    this.api.getBestOffer(id)
    .pipe(finalize(() => {
      this.cdr.detectChanges();
    })).subscribe({
      next: offer => { this.bestOffer = offer; this.error = ''; },
      error: () => { this.error = 'No offer found for this product.'; this.bestOffer = null; }
    });
  }

  currentQuarter(): number {
    return Math.floor(new Date().getMonth() / 3) + 1;
  }

  currentYear(): number {
    return new Date().getFullYear();
  }
}
