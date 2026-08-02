import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ShipmentService } from '../../../core/services/shipment.service';
import { RateQuoteResponse } from '../../../core/models/shipment.model';

/** Create shipment from an order (E1.S1, UR-001) with live multi-carrier rate comparison (UR-010). */
@Component({
  selector: 'gsp-shipment-create',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './shipment-create.component.html',
  styleUrl: './shipment-create.component.scss'
})
export class ShipmentCreateComponent {
  private readonly fb = inject(FormBuilder);
  private readonly shipmentService = inject(ShipmentService);
  private readonly router = inject(Router);

  rates: RateQuoteResponse[] = [];
  submitting = false;

  form = this.fb.nonNullable.group({
    d365OrderReference: [''],
    direction: ['Outbound', Validators.required],
    originCountryCode: ['US', [Validators.required, Validators.maxLength(2)]],
    destinationCountryCode: ['', [Validators.required, Validators.maxLength(2)]],
    originAddress: ['', Validators.required],
    destinationAddress: ['', Validators.required],
    materialClassification: ['Standard', Validators.required],
    requiresTemperatureControl: [false],
    customerType: [''],
    isVipHandling: [false],
    totalWeightKg: [1, [Validators.required, Validators.min(0.01)]]
  });

  getRates(): void {
    const value = this.form.getRawValue();
    this.shipmentService
      .getRates({
        originCountryCode: value.originCountryCode,
        destinationCountryCode: value.destinationCountryCode,
        totalWeightKg: value.totalWeightKg,
        materialClassification: value.materialClassification as any,
        requiresTemperatureControl: value.requiresTemperatureControl
      })
      .subscribe((rates) => (this.rates = rates));
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const value = this.form.getRawValue();
    this.submitting = true;

    this.shipmentService
      .create({
        d365OrderReference: value.d365OrderReference || undefined,
        direction: value.direction as any,
        originCountryCode: value.originCountryCode,
        destinationCountryCode: value.destinationCountryCode,
        originAddress: value.originAddress,
        destinationAddress: value.destinationAddress,
        materialClassification: value.materialClassification as any,
        requiresTemperatureControl: value.requiresTemperatureControl,
        customerType: value.customerType || undefined,
        isVipHandling: value.isVipHandling
      })
      .subscribe({
        next: (shipment) => this.router.navigate(['/shipments'], { queryParams: { created: shipment.shipmentNumber } }),
        error: () => (this.submitting = false)
      });
  }
}
