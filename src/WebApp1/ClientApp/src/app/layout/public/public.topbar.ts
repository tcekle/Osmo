import { Component } from '@angular/core';
import { MenuItem } from 'primeng/api';
import { RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { StyleClassModule } from 'primeng/styleclass';
import { LayoutService } from '../service/layout.service';
import {Ripple} from 'primeng/ripple';
import {Button} from "primeng/button";

@Component({
  selector: 'public-topbar',
  standalone: true,
    imports: [RouterModule, CommonModule, StyleClassModule, Ripple, Button],
  templateUrl: './public.topbar.html',
  styleUrls: ['./public.topbar.scss'],
})
export class PublicTopbar {
  items!: MenuItem[];

  constructor(public layoutService: LayoutService) {}

  toggleDarkMode() {
    this.layoutService.layoutConfig.update((state) => ({ ...state, darkTheme: !state.darkTheme }));
  }
}
