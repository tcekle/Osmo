import {Component, OnDestroy, OnInit} from '@angular/core';
import { BreadcrumbService} from '../../service/breadcrum.service';
import {Breadcrumb} from 'primeng/breadcrumb';
import {RouterLink} from '@angular/router';
import {NgClass, NgIf} from '@angular/common';
import { Subscription } from 'rxjs';
import {MenuItem} from 'primeng/api';

@Component({
  selector: 'app-breadcrumb',
  standalone: true,
  imports: [
    Breadcrumb,
    RouterLink,
    NgIf
  ],
  templateUrl: './breadcrumb.component.html'
})

export class BreadcrumbComponent implements OnInit, OnDestroy {
  breadcrumbs: Array<{ label: string, url: string }> = [];
  homeItem: MenuItem = {
    icon: 'pi pi-home',
    url: "/w",
  }
  private breadcrumbSub?: Subscription;

  constructor(private readonly breadcrumbService: BreadcrumbService) {}

  ngOnInit(): void {
    this.breadcrumbSub = this.breadcrumbService.breadcrumbs$.subscribe(allBreadcrumbs => {
      this.breadcrumbs = allBreadcrumbs;
    });
  }

  ngOnDestroy(): void {
    this.breadcrumbSub?.unsubscribe();
  }
}
