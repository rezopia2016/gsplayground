import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { LabelService } from '../../core/services/label.service';
import { LabelResponse, LabelType, LabelFormat } from '../../core/models/label.model';

const ALL_LABEL_TYPES: LabelType[] = ['Customer', 'Internal', 'Sample', 'CollectionKit', 'Hazard', 'Return'];

/** Label generation & reprint UI covering the six lab label types (BR-020, UR-020/021). */
@Component({
  selector: 'gsp-label-management',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <h1>Label Management</h1>

    <div class="gsp-card">
      <label>
        Package ID
        <input type="text" [(ngModel)]="packageId" placeholder="Paste a package GUID" />
      </label>

      <fieldset>
        <legend>Label types</legend>
        <label class="gsp-checkbox" *ngFor="let type of allLabelTypes">
          <input type="checkbox" [checked]="selectedTypes.has(type)" (change)="toggleType(type)" />
          {{ type }}
        </label>
      </fieldset>

      <label>
        Format
        <select [(ngModel)]="format">
          <option value="Zpl">ZPL</option>
          <option value="Pdf">PDF</option>
          <option value="Qr">QR</option>
        </select>
      </label>

      <button (click)="generate()">Generate Labels</button>
    </div>

    <div class="gsp-card" *ngIf="labels.length">
      <h2>Generated Labels</h2>
      <table class="gsp-table">
        <thead>
          <tr>
            <th>Type</th>
            <th>Format</th>
            <th>Barcode</th>
            <th>Print Count</th>
            <th></th>
          </tr>
        </thead>
        <tbody>
          <tr *ngFor="let label of labels">
            <td>{{ label.type }}</td>
            <td>{{ label.format }}</td>
            <td>{{ label.barcodeValue }}</td>
            <td>{{ label.printCount }}</td>
            <td><button (click)="reprint(label)">Reprint</button></td>
          </tr>
        </tbody>
      </table>
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
      .gsp-checkbox {
        flex-direction: row;
        align-items: center;
        gap: 0.5rem;
      }
      input,
      select {
        padding: 0.4rem 0.6rem;
        border: 1px solid #d0d5d8;
        border-radius: 4px;
      }
      fieldset {
        border: 1px solid #e2e5e7;
        border-radius: 6px;
        margin-bottom: 0.75rem;
      }
    `
  ]
})
export class LabelManagementComponent {
  private readonly labelService = inject(LabelService);

  readonly allLabelTypes = ALL_LABEL_TYPES;
  selectedTypes = new Set<LabelType>(['Customer', 'Sample']);
  format: LabelFormat = 'Zpl';
  packageId = '';
  labels: LabelResponse[] = [];

  toggleType(type: LabelType): void {
    if (this.selectedTypes.has(type)) {
      this.selectedTypes.delete(type);
    } else {
      this.selectedTypes.add(type);
    }
  }

  generate(): void {
    if (!this.packageId || this.selectedTypes.size === 0) {
      return;
    }

    this.labelService
      .generate({ packageId: this.packageId, requestedTypes: Array.from(this.selectedTypes), format: this.format })
      .subscribe((labels) => (this.labels = labels));
  }

  reprint(label: LabelResponse): void {
    const reason = window.prompt('Reason for reprint?') ?? 'Reprint requested';
    this.labelService.reprint(label.id, { reason }).subscribe((updated) => {
      const index = this.labels.findIndex((l) => l.id === updated.id);
      if (index >= 0) {
        this.labels[index] = updated;
      }
    });
  }
}
