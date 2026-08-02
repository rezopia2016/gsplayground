import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { KitService } from '../../core/services/kit.service';
import { KitResponse } from '../../core/models/kit.model';

/** Kit lifecycle view for Lab Operations Coordinators (E4.S6, UR-030/033). */
@Component({
  selector: 'gsp-kit-logistics',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <h1>Kit &amp; Collection Logistics</h1>

    <div class="gsp-card">
      <label>
        Kit ID
        <input type="text" [(ngModel)]="kitId" placeholder="Paste a kit GUID to look it up" />
      </label>
      <button (click)="loadKit()">Load Kit</button>
    </div>

    <div class="gsp-card" *ngIf="kit as k">
      <h2>{{ k.kitNumber }} — <span class="gsp-badge">{{ k.status }}</span></h2>
      <p>Kit Type: {{ k.kitTypeCode }} · Complete: {{ k.isComplete ? 'Yes' : 'No' }}</p>

      <table class="gsp-table">
        <thead>
          <tr>
            <th>Component</th>
            <th>Name</th>
            <th>Qty</th>
            <th>Required</th>
            <th>Fulfilled</th>
            <th></th>
          </tr>
        </thead>
        <tbody>
          <tr *ngFor="let c of k.components">
            <td>{{ c.componentCode }}</td>
            <td>{{ c.componentName }}</td>
            <td>{{ c.quantity }}</td>
            <td>{{ c.isRequired ? 'Yes' : 'No' }}</td>
            <td>{{ c.isFulfilled ? 'Yes' : 'No' }}</td>
            <td>
              <button *ngIf="!c.isFulfilled" (click)="fulfill(c.componentCode)">Mark fulfilled</button>
            </td>
          </tr>
        </tbody>
      </table>

      <button (click)="dispatchKit()" [disabled]="!k.isComplete">Dispatch Kit</button>
      <p *ngIf="!k.isComplete" class="gsp-hint">Dispatch is blocked until all required components are fulfilled (BR-032).</p>
    </div>
  `,
  styles: [
    `
      .gsp-card {
        margin-bottom: 1rem;
      }
      .gsp-hint {
        color: #b45309;
        font-size: 0.85rem;
      }
      label {
        display: flex;
        flex-direction: column;
        gap: 0.3rem;
        margin-bottom: 0.5rem;
        max-width: 420px;
      }
      input {
        padding: 0.4rem 0.6rem;
        border: 1px solid #d0d5d8;
        border-radius: 4px;
      }
    `
  ]
})
export class KitLogisticsComponent {
  private readonly kitService = inject(KitService);

  kitId = '';
  kit: KitResponse | null = null;

  loadKit(): void {
    if (!this.kitId) {
      return;
    }
    this.kitService.get(this.kitId).subscribe((kit) => (this.kit = kit));
  }

  fulfill(componentCode: string): void {
    if (!this.kit) {
      return;
    }
    this.kitService.fulfillComponent(this.kit.id, componentCode).subscribe((kit) => (this.kit = kit));
  }

  dispatchKit(): void {
    if (!this.kit) {
      return;
    }
    this.kitService.dispatch(this.kit.id).subscribe((kit) => (this.kit = kit));
  }
}
