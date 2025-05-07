import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

import { ButtonModule } from 'primeng/button';
import { IconFieldModule } from 'primeng/iconfield';
import { InputIconModule } from 'primeng/inputicon';
import { OverlayBadgeModule } from 'primeng/overlaybadge';

import { Apollo, gql } from 'apollo-angular';
import {TableModule} from 'primeng/table';
import {OnlineIndicatorComponent} from '../online-indicator/online-indicator.component';

@Component({
  selector: 'app-test',
  imports: [
    ButtonModule,
    IconFieldModule,
    InputIconModule,
    OverlayBadgeModule,
    TableModule,
    OnlineIndicatorComponent
  ],
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss'],
  standalone: true
})
export class DashboardComponent implements OnInit {

  systems: any[] = [];

  constructor(private readonly apollo: Apollo,
              private readonly router: Router) {}

  onRowClick(event: any) {
    const system = event;
    this.router.navigate(['w/system', system.handlerId]);
  }

  ngOnInit() {
    this.apollo
    .use('connexClient')
    .watchQuery({
      query: gql`
        {
          systems {
            ipAddress
            entity {
              entityIdentifier
            }
            handlerId
            handlerType
            hostName
            entity {
              entityName
              id
            }
          }
        }
      `,
    })
    .valueChanges.subscribe((result: any) => {
      this.systems = result.data?.systems;
    });
  }
}
