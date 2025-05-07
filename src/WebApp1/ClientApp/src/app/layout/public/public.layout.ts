import { Component } from '@angular/core';
import { RouterOutlet} from '@angular/router';

import { MenubarModule } from 'primeng/menubar';
import { ToolbarModule } from 'primeng/toolbar';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { MenuModule } from 'primeng/menu';
import { PublicTopbar } from './public.topbar';

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
    PublicTopbar
  ],
  templateUrl: 'public.layout.html',
  styleUrl: './public.layout.scss'
})
export class PublicLayout {
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

  toggleDarkMode() {
    const element = document.querySelector('html');
    element?.classList.toggle('my-app-dark');
  }
}
