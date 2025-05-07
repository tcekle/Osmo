import {Component, Input, OnChanges, SimpleChanges} from '@angular/core';
import {NgClass, NgIf} from '@angular/common';
import {PlotlyModule} from 'angular-plotly.js';

@Component({
  selector: 'app-event-viewer-chart',
  imports: [
    NgClass,
    PlotlyModule,
    NgIf
  ],
  templateUrl: './event-viewer-chart.component.html',
  styleUrl: './event-viewer-chart.component.scss'
})
export class EventViewerChartComponent implements OnChanges {
  @Input() trace: any;
  @Input() chartTitle: string = 'Time Series';
  @Input() class: string = '';
  @Input() startDate: Date = new Date();
  @Input() endDate: Date = new Date();

  layout: any = null;
  public ready: boolean = false;

  ngOnChanges(changes: SimpleChanges) {
    if (changes['trace'] && this.trace?.length) {
      this.layout = {
        height: 400,
        xaxis: {
          showgrid: true,
          showline: true,
          linecolor: "rgb(102, 102, 102)",
          titlefont: { font: { color: "rgb(204, 204, 204)" } },
          title: "Time of the event",
          type: 'date',
          range: [this.startDate.toISOString(), this.endDate.toISOString()],
          rangeslider: { range: [this.startDate.toISOString(), this.endDate.toISOString()] }
        },
        xaxis2: {
          matches: 'x',
          rangeslider: {
            visible: true
          }
        },
        yaxis: {
          showgrid: true,
          showline: true,
          fixedrange: true,
          linecolor: "rgb(102, 102, 102)",
          titlefont: { font: { color: "rgb(204, 204, 204)" } },
          tickfont: { font: { color: "rgb(102, 102, 102)" } },
          title: "ConneX Event Type",
        },
        yaxis2: {
          title: 'frequency',
          overlaying: 'y',
          side: 'right'

        },
        title:'ConneX events',
        margin: { l: 140, r: 40, b: 100, t: 50 },
        hovermode: "closest",
        hoverlabel: { bgcolor: "#FFF" }
      };

      this.ready = true;
    }
  }
}
