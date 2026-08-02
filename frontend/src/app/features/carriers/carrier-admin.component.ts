import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CarrierService } from '../../core/services/carrier.service';
import { CarrierModel } from '../../core/models/carrier.model';

/** Carrier admin console (E10.S4) — onboarding a new carrier is configuration, not a code change (BR-013). */
@Component({
  selector: 'gsp-carrier-admin',
  standalone: true,
  imports: [CommonModule],
  template: `
    <h1>Carrier Administration</h1>

    <div class="gsp-card">
      <table class="gsp-table">
        <thead>
          <tr>
            <th>Code</th>
            <th>Name</th>
            <th>Adapter Key</th>
            <th>Active</th>
            <th>EDI</th>
            <th>Priority</th>
            <th>Services</th>
          </tr>
        </thead>
        <tbody>
          <tr *ngFor="let carrier of carriers">
            <td>{{ carrier.code }}</td>
            <td>{{ carrier.name }}</td>
            <td><code>{{ carrier.adapterKey }}</code></td>
            <td>{{ carrier.isActive ? 'Yes' : 'No' }}</td>
            <td>{{ carrier.supportsEdi ? 'Yes' : 'No' }}</td>
            <td>{{ carrier.priority }}</td>
            <td>{{ carrier.services.length }} service(s)</td>
          </tr>
        </tbody>
      </table>
      <p class="gsp-hint">
        New regional carriers are onboarded by registering an <code>ICarrierGateway</code> adapter and a matching
        carrier row — the shipment engine and UI require no changes (BR-011–013).
      </p>
    </div>
  `,
  styles: [
    `
      .gsp-hint {
        color: #566;
        font-size: 0.85rem;
        margin-top: 0.75rem;
      }
    `
  ]
})
export class CarrierAdminComponent implements OnInit {
  private readonly carrierService = inject(CarrierService);
  carriers: CarrierModel[] = [];

  ngOnInit(): void {
    this.carrierService.list().subscribe((carriers) => (this.carriers = carriers));
  }
}
