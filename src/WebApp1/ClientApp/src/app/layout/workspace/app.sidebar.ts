import { Component } from '@angular/core';
import { RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { StyleClassModule } from 'primeng/styleclass';
import { LayoutService } from '../service/layout.service';
import {Tooltip} from 'primeng/tooltip';

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [RouterModule, CommonModule, StyleClassModule, Tooltip],
  templateUrl: './app.sidebar.html',
})
export class AppSidebar {
  isSlimMenu = true;

  sampleAppsSidebarNavs = [
    { icon: 'pi pi-home', title: 'Home', routerLink: '/w' },
    { icon: 'pi pi-briefcase', title: 'Jobs', routerLink: '/w/jobs' },
    // { icon: 'pi pi-inbox', title: 'Inbox', routerLink: '/w' },
    // { icon: 'pi pi-th-large', title: 'Cards', routerLink: '/w' },
    // { icon: 'pi pi-user', title: 'Customers', routerLink: '/w' },
    // { icon: 'pi pi-video', title: 'Movies', routerLink: '/w' }
  ];

  constructor(public layoutService: LayoutService) {}
}
