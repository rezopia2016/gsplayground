import { Routes } from '@angular/router';

export const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'dashboard' },
  {
    path: 'dashboard',
    loadComponent: () => import('./features/dashboard/dashboard.component').then((m) => m.DashboardComponent)
  },
  {
    path: 'shipments',
    loadComponent: () => import('./features/shipments/shipment-list/shipment-list.component').then((m) => m.ShipmentListComponent)
  },
  {
    path: 'shipments/new',
    loadComponent: () =>
      import('./features/shipments/shipment-create/shipment-create.component').then((m) => m.ShipmentCreateComponent)
  },
  {
    path: 'kits',
    loadComponent: () => import('./features/kits/kit-logistics.component').then((m) => m.KitLogisticsComponent)
  },
  {
    path: 'labels',
    loadComponent: () => import('./features/labels/label-management.component').then((m) => m.LabelManagementComponent)
  },
  {
    path: 'tracking',
    loadComponent: () => import('./features/tracking/tracking.component').then((m) => m.TrackingComponent)
  },
  {
    path: 'carriers',
    loadComponent: () => import('./features/carriers/carrier-admin.component').then((m) => m.CarrierAdminComponent)
  },
  {
    path: 'd365-sync',
    loadComponent: () => import('./features/d365-sync/d365-sync-status.component').then((m) => m.D365SyncStatusComponent)
  },
  { path: '**', redirectTo: 'dashboard' }
];
