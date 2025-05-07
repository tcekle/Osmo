import { Component } from '@angular/core';
import { RouterOutlet} from '@angular/router';

import { MenubarModule } from 'primeng/menubar';
import { ToolbarModule } from 'primeng/toolbar';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { MenuModule } from 'primeng/menu';

@Component({
  selector: 'app-root',
  imports: [
    RouterOutlet,
    MenubarModule,
    ToolbarModule,
    ButtonModule,
    InputTextModule,
    MenuModule,
  ],
  template: `<router-outlet></router-outlet>`,
  styleUrl: './app.component.scss',
})

export class AppComponent {
  title = 'OsmoUi';
  menuItems = [
    { label: 'Home', icon: 'pi pi-fw pi-home', routerLink: '/' },
    { label: 'Jobs', icon: 'pi pi-fw pi-info-circle', routerLink: '/about' },
    { label: 'Handlers', icon: 'pi pi-fw pi-envelope', routerLink: '/contact' },
    { label: 'Weather forecast', icon: 'pi pi-fw pi-envelope', routerLink: '/fetch-data' }
  ];

  collapsed = false;

  toggleMenu() {
    this.collapsed = !this.collapsed;
  }
}
