import {Component, Input, OnChanges, SimpleChanges} from '@angular/core';
import {NgClass, NgIf} from '@angular/common';

import * as PlotlyJS from 'plotly.js-dist-min';
import {PlotlyModule} from 'angular-plotly.js';

PlotlyModule.plotlyjs = PlotlyJS;

@Component({
  standalone: true,
  selector: 'app-time-series-chart',
  templateUrl: './time-series-chart.component.html',
  styleUrls: ['./time-series-chart.component.scss'],
  imports: [
    NgClass,
    PlotlyModule,
    NgIf
  ]
})
export class TimeSeriesChartComponent implements OnChanges {
  @Input() trace: any;
  @Input() chartTitle: string = 'Time Series';
  @Input() class: string = '';
  @Input() startDate: Date = new Date();
  @Input() endDate: Date = new Date();

  layout: any = null;
  public ready: boolean = false;

  ngOnChanges(changes: SimpleChanges) {
    if (changes['trace'] && this.trace?.length) {
      const allY = this.trace.flatMap((t: { y: any; }) => t.y ?? []);

      this.layout = {
        title: { text: this.chartTitle },
        xaxis: {
          type: 'date',
          range: [this.startDate.toISOString(), this.endDate.toISOString()],
          rangeslider: { range: [this.startDate.toISOString(), this.endDate.toISOString()] }
        },
        yaxis: {
          type: 'linear',
          range: [Math.min(...allY), Math.max(...allY)]
        },
        autosize: true
      };

      this.ready = true;
    }
  }
}
