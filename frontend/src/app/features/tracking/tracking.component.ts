import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ShipmentService } from '../../core/services/shipment.service';
import { ShipmentResponse } from '../../core/models/shipment.model';

/** Shipment tracking lookup for Customer Service Reps (UR-003/UR-033). */
@Component({
  selector: 'gsp-tracking',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <h1>Shipment Tracking</h1>

    <div class="gsp-card">
      <label>
        Shipment ID
        <input type="text" [(ngModel)]="shipmentId" placeholder="Paste a shipment GUID" />
      </label>
      <button (click)="lookup()">Track</button>
    </div>

    <div class="gsp-card" *ngIf="shipment as s">
      <h2>{{ s.shipmentNumber }} — <span class="gsp-badge">{{ s.status }}</span></h2>
      <dl>
        <dt>Route</dt>
        <dd>{{ s.originCountryCode }} → {{ s.destinationCountryCode }}</dd>
        <dt>Carrier</dt>
        <dd>{{ s.carrierCode || '—' }} ({{ s.carrierServiceCode || '—' }})</dd>
        <dt>Tracking Number</dt>
        <dd>{{ s.carrierTrackingNumber || '—' }}</dd>
        <dt>Dispatched</dt>
        <dd>{{ s.dispatchedAtUtc ? (s.dispatchedAtUtc | date: 'medium') : 'Not yet dispatched' }}</dd>
        <dt>Delivered</dt>
        <dd>{{ s.deliveredAtUtc ? (s.deliveredAtUtc | date: 'medium') : 'In progress' }}</dd>
      </dl>
    </div>
  `,
  styles: [
    `
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
      dl {
        display: grid;
        grid-template-columns: 160px 1fr;
        row-gap: 0.4rem;
      }
      dt {
        font-weight: 600;
        color: #445;
      }
    `
  ]
})
export class TrackingComponent {
  private readonly shipmentService = inject(ShipmentService);

  shipmentId = '';
  shipment: ShipmentResponse | null = null;

  lookup(): void {
    if (!this.shipmentId) {
      return;
    }
    this.shipmentService.get(this.shipmentId).subscribe((shipment) => (this.shipment = shipment));
  }
}
