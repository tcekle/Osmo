import {Component, OnInit} from '@angular/core';
import {ActivatedRoute, RouterLink} from '@angular/router';
import {Apollo, gql} from 'apollo-angular';
import {Card} from 'primeng/card';
import {PrimeTemplate} from 'primeng/api';
import {BreadcrumbService} from '../layout/service/breadcrum.service';
import {Button} from 'primeng/button';
import {SystemDetails} from '@models/system/SystemDetails';
import {NgIf} from '@angular/common';

const SYSTEM_QUERY = gql`
  query GetSystem($handlerId: Int!) {
    system(databaseId: $handlerId) {
      ipAddress
      hostName
      machineFactory
      handlerType
      entity {
        entityName
      }
    }
  }
`

@Component({
  selector: 'app-system',
  imports: [
    Card,
    PrimeTemplate,
    Button,
    RouterLink,
    NgIf
  ],
  templateUrl: './system.component.html',
  styleUrl: './system.component.scss'
})
export class SystemComponent implements OnInit {

  systemId: number = 0;
  system: SystemDetails | null = null;

  constructor(private readonly apollo: Apollo,
              private readonly route: ActivatedRoute,
              private readonly breadcrumbService: BreadcrumbService) {
  }

  ngOnInit() {
    this.systemId = Number(this.route.snapshot.paramMap.get('id'));

    this.apollo
    .use('connexClient')
    .watchQuery({
      query: SYSTEM_QUERY,
      variables: {
        handlerId: this.systemId, // Replace with the actual handlerId you want to query
      },
    })
    .valueChanges.subscribe((result: any) => {
      this.system = result.data?.system;

      this.breadcrumbService.updateLastLabel(this.system?.entity.entityName ?? '');
    });
  }
}
