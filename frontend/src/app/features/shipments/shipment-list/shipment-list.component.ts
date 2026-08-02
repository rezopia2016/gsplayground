import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { ShipmentService } from '../../../core/services/shipment.service';
import { ShipmentResponse, ShipmentStatus } from '../../../core/models/shipment.model';

/** Shipment list/search UI (E1.S6, UR-003). */
@Component({
  selector: 'gsp-shipment-list',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './shipment-list.component.html',
  styleUrl: './shipment-list.component.scss'
})
export class ShipmentListComponent implements OnInit {
  private readonly shipmentService = inject(ShipmentService);

  shipments: ShipmentResponse[] = [];
  statusFilter: ShipmentStatus | 'All' = 'All';
  searchTerm = '';

  ngOnInit(): void {
    this.shipmentService.list().subscribe((shipments) => (this.shipments = shipments));
  }

  get filteredShipments(): ShipmentResponse[] {
    return this.shipments.filter((s) => {
      const matchesStatus = this.statusFilter === 'All' || s.status === this.statusFilter;
      const matchesSearch =
        !this.searchTerm ||
        s.shipmentNumber.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
        (s.carrierTrackingNumber ?? '').toLowerCase().includes(this.searchTerm.toLowerCase());
      return matchesStatus && matchesSearch;
    });
  }

  cancelShipment(shipment: ShipmentResponse): void {
    this.shipmentService.cancel(shipment.id, 'Cancelled from shipment list').subscribe((updated) => {
      const index = this.shipments.findIndex((s) => s.id === updated.id);
      if (index >= 0) {
        this.shipments[index] = updated;
      }
    });
  }
}
