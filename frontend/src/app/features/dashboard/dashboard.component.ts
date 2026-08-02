import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ShipmentService } from '../../core/services/shipment.service';
import { D365SyncService } from '../../core/services/d365-sync.service';
import { ShipmentResponse } from '../../core/models/shipment.model';
import { D365SyncRecordModel } from '../../core/models/carrier.model';

/** Operational shipping dashboard — volumes, exceptions, D365 sync health (BR-070/E8.S1). */
@Component({
  selector: 'gsp-dashboard',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss'
})
export class DashboardComponent implements OnInit {
  private readonly shipmentService = inject(ShipmentService);
  private readonly d365SyncService = inject(D365SyncService);

  shipments: ShipmentResponse[] = [];
  pendingSyncRecords: D365SyncRecordModel[] = [];
  loading = true;

  ngOnInit(): void {
    this.shipmentService.list().subscribe((shipments) => {
      this.shipments = shipments;
      this.loading = false;
    });
    this.d365SyncService.getPending().subscribe((records) => (this.pendingSyncRecords = records));
  }

  get exceptionCount(): number {
    return this.shipments.filter((s) => s.status === 'Exception').length;
  }

  get inTransitCount(): number {
    return this.shipments.filter((s) => s.status === 'InTransit' || s.status === 'Dispatched').length;
  }

  get failedSyncCount(): number {
    return this.pendingSyncRecords.filter((r) => r.status === 'Failed').length;
  }
}
