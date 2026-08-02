import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { D365SyncService } from '../../core/services/d365-sync.service';
import { D365SyncRecordModel } from '../../core/models/carrier.model';

/** D365 sync reconciliation report (BR-051/E7.S6, UR-051) — synced vs. pending vs. failed. */
@Component({
  selector: 'gsp-d365-sync-status',
  standalone: true,
  imports: [CommonModule],
  template: `
    <h1>D365 Sync Reconciliation</h1>
    <p class="gsp-hint">
      Shipment/kit operations are never blocked by D365 availability — sync operations are queued in an outbox and
      retried automatically (BR-063). This report shows what is still pending or has failed after the configured
      retry limit.
    </p>

    <div class="gsp-card">
      <table class="gsp-table">
        <thead>
          <tr>
            <th>Direction</th>
            <th>Entity Type</th>
            <th>Status</th>
            <th>Attempts</th>
            <th>Last Error</th>
            <th>Created</th>
          </tr>
        </thead>
        <tbody>
          <tr *ngFor="let record of records">
            <td>{{ record.direction }}</td>
            <td>{{ record.entityType }}</td>
            <td><span class="gsp-badge">{{ record.status }}</span></td>
            <td>{{ record.attemptCount }}</td>
            <td>{{ record.lastError || '—' }}</td>
            <td>{{ record.createdAtUtc | date: 'short' }}</td>
          </tr>
        </tbody>
      </table>
      <p *ngIf="!records.length">No pending or failed D365 sync records.</p>
    </div>
  `,
  styles: [
    `
      .gsp-hint {
        color: #566;
        font-size: 0.9rem;
        max-width: 640px;
      }
    `
  ]
})
export class D365SyncStatusComponent implements OnInit {
  private readonly d365SyncService = inject(D365SyncService);
  records: D365SyncRecordModel[] = [];

  ngOnInit(): void {
    this.d365SyncService.getPending().subscribe((records) => (this.records = records));
  }
}
