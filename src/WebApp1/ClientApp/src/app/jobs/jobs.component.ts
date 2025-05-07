import {Component, OnInit} from '@angular/core';
import {TableLazyLoadEvent, TableModule} from 'primeng/table';
import {Apollo, gql} from 'apollo-angular';
import {Router} from '@angular/router';
import {IconField} from 'primeng/iconfield';
import {InputIcon} from 'primeng/inputicon';
import {InputText} from 'primeng/inputtext';
import {JobListDetails} from '@models/job/JobListDetails';
import {JobQueryResult} from '@models/graphqlQueries/JobQueryResult';
import {debounceTime, Subject} from 'rxjs';
import {FormsModule} from '@angular/forms';

const GET_JOBS = gql`
  query GetJobs($skip: Int!, $take: Int!, $searchQuery: String!) {
    jobs(skip: $skip,
      take: $take,
      where:  {
        jobName:  {
          contains: $searchQuery
        }
      }
      order: [ { jobName: ASC }])
    {
      items {
        givenJobId
        jobName
        jobDescription
        jobChecksum
        settingChecksum
        id
        name
        createdAt
      }
      totalCount
    }
  }`

@Component({
  selector: 'app-jobs',
  templateUrl: './jobs.component.html',
  styleUrls: ['./jobs.component.scss'],
  imports: [
    TableModule,
    IconField,
    InputIcon,
    InputText,
    FormsModule
  ]
})
export class JobsComponent implements OnInit {
  jobs: JobListDetails[] = [];
  totalRecords = 0;
  loading = false;
  searchQuery: String = '';

  rowsPerPage: number = 10;
  currentPage: number = 0;

  searchInput$ = new Subject<string>();

  constructor(private readonly apollo: Apollo,
              private readonly router: Router) {}

  ngOnInit() {
    this.searchInput$
      .pipe(debounceTime(300)) // adjust delay here
      .subscribe((value) => {
        this.searchQuery = value;
        this.currentPage = 0; // reset to first page on search
        this.loadJobs();
      });
  }

  onRowClick(event: JobListDetails) {
    this.router.navigate(['w/jobs', event.id]);
  }

  loadJobsLazy(event: TableLazyLoadEvent) {
    this.rowsPerPage = event.rows ?? 10;
    const first = event.first ?? 0;
    this.currentPage = Math.floor(first / this.rowsPerPage);

    this.loadJobs();
  }

  onSearch(event: any) {
    this.searchInput$.next(event.target.value);
    // this.searchQuery = event.target.value;
    // this.currentPage = 0;
    // this.loadJobs();
  }

  loadJobs() {
    this.loading = true;

    const rowsPerPage = this.rowsPerPage ?? 10;
    const firstRowIndex = this.currentPage ?? 0;

    // const currentPage = Math.floor(firstRowIndex / rowsPerPage);

    this.apollo
      .watchQuery<JobQueryResult>({
        query: GET_JOBS,
        variables: {
          skip: this.currentPage * rowsPerPage,
          take: rowsPerPage,
          searchQuery: this.searchQuery
        },
        fetchPolicy: 'network-only'
      })
      .valueChanges.subscribe({
      next: (result) => {
        this.jobs = result.data.jobs.items;
        this.totalRecords = result.data.jobs.totalCount;
        this.loading = false;
      },
      error: (error) => {
        this.loading = false;
        console.error('Error loading jobs:', error);
      }
    });
  }
}
