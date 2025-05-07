import { Component, OnInit } from '@angular/core';
import {Apollo, gql} from 'apollo-angular';
import {ActivatedRoute} from '@angular/router';

// const JOB_QUERY = gql`
//   query GetRecord($recordId: UUID!) {
//
//   }`

@Component({
  selector: 'app-audit-records',
  templateUrl: './audit-record.component.html',
  styleUrls: ['./audit-record.component.css']
})
export class AuditRecordComponent implements OnInit {
  auditRecords: any[] = [];
  recordId: string | null = null;

  constructor(private readonly apollo: Apollo,
              private readonly route: ActivatedRoute,) {
  }

  ngOnInit() {
    this.recordId = this.route.snapshot.paramMap.get('id');




    // Sample data placeholder (Replace with real data fetching logic)
    this.auditRecords = [
      {
        TimeStamp: "2020-06-22T01:10:38.339372Z",
        Programmer: {
          Class: "LumenX",
          FirmwareVersion: "1.8.0.59",
          SerialNumber: "001-035-136-148-137-224-207-100-238",
          SystemVersion: "1.8.0.59",
          ProgrammerIP: "10.0.0.5"
        },
        Job: {
          JobName: "BIG SE050 with much stuff 2.9.2",
          DeviceName: "SE050C1HQ1"
        },
        PartDetail: {
          Result: {
            CodeName: "Success"
          }
        }
      }
    ];
  }
}
