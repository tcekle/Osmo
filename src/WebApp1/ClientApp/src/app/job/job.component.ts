import {Component, OnInit, ViewChild} from '@angular/core';
import {ActivatedRoute, RouterLink} from '@angular/router';
import {Apollo, gql} from 'apollo-angular';
import {Card} from 'primeng/card';
import { PrimeTemplate} from 'primeng/api';
import {ChartModule, UIChart} from 'primeng/chart';
import {Button } from 'primeng/button';
import {BreadcrumbService} from '../layout/service/breadcrum.service';
import {Select, SelectChangeEvent} from 'primeng/select';
import {NgIf} from '@angular/common';
import {TimeSeriesChartComponent} from '../components/charts/time-series-chart/time-series-chart.component';
import {JobDetails} from '@models/job/JobDetails';
import {ProgrammingResult} from '@models/job/ProgrammingResult';
import {TimeIntervalOption} from '@models/charts/TimeIntervalOption';
import {EventViewerChartComponent} from '../components/charts/event-viewer-chart/event-viewer-chart.component';
import {JobEvent} from '@models/job/JobEvent';

const JOB_QUERY = gql`
  query GetJobById($jobId: UUID!, $interval: String) {
    jobStatistics(jobId: $jobId) {
      total_jobs
      total_success
      total_failures
    }
    jobById(id: $jobId) {
      jobName
      jobDescription
      jobChecksum
      settingChecksum
      createdAt
    }
    jobProgrammingResults(jobId: $jobId, interval: $interval)
    {
      code
      code_name
      occurrences
    }
    jobProgrammingTimes (jobId: $jobId, interval: $interval) {
      bucket
      avg_program_duration
      avg_verify_duration
      avg_blank_check_duration
      avg_erase_duration
      avg_overhead
    }
    jobEvents(jobId: $jobId, interval: $interval) {
      type
      title
      message
      timestamp
    }
  }`


type ProgrammingTime = {
  bucket: string;
  avg_program_duration: number;
};

@Component({
  selector: 'app-job',
  imports: [
    Card,
    PrimeTemplate,
    ChartModule,
    Button,
    RouterLink,
    Select,
    NgIf,
    TimeSeriesChartComponent,
    EventViewerChartComponent
  ],
  templateUrl: './job.component.html',
  styleUrl: './job.component.scss'
})

export class JobComponent implements OnInit {
  @ViewChild('programmingResultsChart') chartComponent!: UIChart;
  jobId: string | null = null;
  job: JobDetails | null = null;
  programmingChartData: any = null;
  programmingResultsChartOptions = {
    responsive: true,
    plugins: {
      legend: {
        position: 'bottom'
      }
    }
  };
  intervals: TimeIntervalOption[] = [
    { name: '5 minutes', code: '5 minutes', offsetSeconds: 5 * 60 },
    { name: '10 minutes', code: '10 minutes', offsetSeconds: 10 * 60 },
    { name: '1 hour', code: '1 hour', offsetSeconds: 1 * 60 * 60 },
    { name: '1 day', code: '1 day', offsetSeconds: 1 * 24 * 60 * 60 },
    { name: '1 week', code: '1 week', offsetSeconds: 7 * 24 * 60 * 60 },
    { name: '1 month', code: '1 month', offsetSeconds: 30 * 24 * 60 * 60 },
    { name: '3 months', code: '3 months', offsetSeconds: 90 * 24 * 60 * 60 },
    { name: '6 months', code: '6 months', offsetSeconds: 180 * 24 * 60 * 60 },
    { name: '1 year', code: '1 year', offsetSeconds: 365 * 24 * 60 * 60 },
    { name: '5 years', code: '5 years', offsetSeconds: 5 * 365 * 24 * 60 * 60 }
  ];

  programmingTimesTrace: any;// ProgrammingTime[] = [];
  jobEventsTrace: any;
  startDate: Date = new Date();
  endDate: Date = new Date();

  constructor(private readonly apollo: Apollo,
              private readonly route: ActivatedRoute,
              private readonly breadcrumbService: BreadcrumbService) {
  }

  ngOnInit() {
    this.jobId = this.route.snapshot.paramMap.get('id');

    this.apollo
      .watchQuery({
        query: JOB_QUERY,
        variables: {
          jobId: this.jobId,
          interval: this.intervals[0].name
        },
      })
      .valueChanges.subscribe((result: any) => {
        this.updateChartData(result, this.intervals[0]);
        if (this.job) {
          this.breadcrumbService.updateLastLabel(this.job.jobById.jobName);
        }
    });
  }

  public timeIntervalSelectionChanged(event: SelectChangeEvent){
    const newValue: TimeIntervalOption = event.value as TimeIntervalOption;

    this.apollo
      .watchQuery({
        query: JOB_QUERY,
        variables: {
          jobId: this.jobId,
          interval: newValue.name
        },
      })
      .valueChanges.subscribe((result: any) => {
        this.updateChartData(result, newValue);
      });
  }

  private updateChartData(result: any, interval: TimeIntervalOption) {
    this.job = result.data;

    this.programmingChartData = {
      labels: this.job?.jobProgrammingResults.map((d: ProgrammingResult) => `${d.code} - ${d.code_name}`),
      datasets: [
        {
          data: this.job?.jobProgrammingResults.map((d: ProgrammingResult) => d.occurrences),
          backgroundColor: ['#42A5F5', '#66BB6A', '#FFA726', '#AB47BC', '#FF6384'], // customize
          hoverBackgroundColor: ['#64B5F6', '#81C784', '#FFB74D', '#BA68C8', '#FF80AB']
        }
      ]
    };

    this.chartComponent?.chart.update();

    const averageProgrammingTimesTrace = {
      name: "Average Programming Duration",
      type: "scatter",
      mode: "lines",
      connectgaps: false,
      x: this.job?.jobProgrammingTimes.map((pt: { bucket: any; }) => pt.bucket),
      y: this.job?.jobProgrammingTimes.map((pt: { avg_program_duration: any; }) => pt.avg_program_duration),
      line: {
        color: "#17BECF" // optional, depending on your chart needs
      }
    };

    this.startDate = new Date(Date.now() - interval.offsetSeconds * 1000);
    this.endDate = new Date();
    this.programmingTimesTrace = [ averageProgrammingTimesTrace ];

    this.jobEventsTrace = this.transformJobEventsToPlotlyData(this.job?.jobEvents ?? []);
  }

  private transformJobEventsToPlotlyData(jobEvents: JobEvent[]): any {
    const symbolMap: Record<string, string> = {
      SystemStartup: 'triangle-up',
      SystemShutdown: 'triangle-down',
    };

    const data = [];

    // Group events by type
    const grouped: Record<string, JobEvent[]> = {};
    for (const evt of jobEvents) {
      if (!grouped[evt.type]) grouped[evt.type] = [];
      grouped[evt.type].push(evt);
    }

    // Add one scattergl trace per event type
    for (const type in grouped) {
      const events = grouped[type];

      data.push({
        x: events.map(e => e.timestamp),
        y: events.map(() => type),
        visible: events.length <= 1000 ? 'legend' : 'legendonly',
        mode: 'markers',
        type: 'scattergl',
        name: `${type} (${events.length})`,
        text: events.map(e => e.title),
        marker: {
          size: 12,
          symbol: symbolMap.hasOwnProperty(type) ? symbolMap[type] : 'diamond',
        },
        hovertemplate:
          "<b>%{x|%Y-%m-%d %H:%M:%S} - %{text}</b><br><br>" +
          "%{customdata.message}<extra></extra>",
        customdata: events.map(e => ({
          message: e.message ? e.message.replace(/(?:\r\n|\r|\n)/g, '<br>') : '<none>',
        })),
      });
    }

    // Build frequency bars (event count per time bucket — optional enhancement)
    const frequencyMap = new Map<string, number>();
    for (const evt of jobEvents) {
      const time = evt.timestamp.slice(0, 10); // YYYY-MM-DD
      frequencyMap.set(time, (frequencyMap.get(time) || 0) + 1);
    }

    const frequencyData = Array.from(frequencyMap.entries()).map(([time, value]) => ({ time, value }));

    data.push({
      x: frequencyData.map(d => d.time),
      y: frequencyData.map(d => d.value),
      type: 'bar',
      yaxis: 'y2',
      name: 'Event Frequency',
      marker: {
        color: 'rgba(0, 0, 0, 0.1)',
      },
      hoverinfo: 'none',
    });

    return data;
  }
}
