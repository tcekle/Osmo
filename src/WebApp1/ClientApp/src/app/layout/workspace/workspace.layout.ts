import { Component } from '@angular/core';
import { RouterOutlet} from '@angular/router';
import { MenubarModule } from 'primeng/menubar';
import { ToolbarModule } from 'primeng/toolbar';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { MenuModule } from 'primeng/menu';
import {AppSidebar} from './app.sidebar';
import {BreadcrumbComponent} from './breadcrumb/breadcrumb.component';
import {AppTopbar} from './app.topbar';

@Component({
  selector: 'app-layout',
  standalone: true,
  imports: [
    RouterOutlet,
    MenubarModule,
    ToolbarModule,
    ButtonModule,
    InputTextModule,
    MenuModule,
    AppSidebar,
    BreadcrumbComponent,
    AppTopbar
  ],
  templateUrl: 'workspace.layout.html',
  styleUrl: './workspace.layout.scss'
})
export class WorkspaceLayout {
  collapsed = false;

  title = 'OsmoUi';
  menuItems = [
    { label: 'Home', icon: 'pi pi-fw pi-home', routerLink: '/' },
    { label: 'Jobs', icon: 'pi pi-fw pi-info-circle', routerLink: '/about' },
    { label: 'Handlers', icon: 'pi pi-fw pi-envelope', routerLink: '/contact' },
    { label: 'Weather forecast', icon: 'pi pi-fw pi-envelope', routerLink: '/fetch-data' }
  ];

  toggleMenu() {
    this.collapsed = !this.collapsed;
  }
}
