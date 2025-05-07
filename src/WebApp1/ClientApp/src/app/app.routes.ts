import { Routes } from '@angular/router';

import { DashboardComponent } from './dashboard/dashboard.component';
import { CounterComponent } from './counter/counter.component';
import { FetchDataComponent } from './fetch-data/fetch-data.component';
import { WorkspaceLayout} from './layout/workspace/workspace.layout';
import {PricingComponent} from './pricing/pricing.component';
import { PublicLayout } from './layout/public/public.layout';
import {HomeComponent} from './home/home.component';
import {JobsComponent} from './jobs/jobs.component';
import {SystemComponent} from './system/system.component';
import {JobComponent} from './job/job.component';
import {AuditRecordComponent} from './audit-record/audit-record.component';

export const routes: Routes = [
  {
    path: 'w',
    component: WorkspaceLayout,
    children: [
      { path: '', component: DashboardComponent },
      { path: 'jobs', component: JobsComponent, data: { breadcrumb: 'Jobs' }, },
      { path: 'jobs/:id', component: JobComponent, data: { breadcrumb: 'Jobs', ignoreParameters: true }, },
      { path: 'system/:id', component: SystemComponent, data: { breadcrumb: 'System', ignoreParameters: true }, },
      { path: 'records/:id', component: AuditRecordComponent, data: { breadcrumb: 'Record', ignoreParameters: true }, },
      {
        path: 'fetch-data',
        title: 'Fetch Data',
        component: FetchDataComponent
      },
    ]
  },
  {
    path: '',
    title: 'Home',
    component: PublicLayout,
    children: [
      { path: '', component: HomeComponent },
      { path: 'pricing', title: 'Pricing', component: PricingComponent },
    ]
  },
  {
    path: 'counter',
    title: 'Counter',
    component: CounterComponent
  }
];
