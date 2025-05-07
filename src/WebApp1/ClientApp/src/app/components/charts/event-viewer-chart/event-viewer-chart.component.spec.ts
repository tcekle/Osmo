import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EventViewerChartComponent } from './event-viewer-chart.component';

describe('EventViewerChartComponent', () => {
  let component: EventViewerChartComponent;
  let fixture: ComponentFixture<EventViewerChartComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EventViewerChartComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(EventViewerChartComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
