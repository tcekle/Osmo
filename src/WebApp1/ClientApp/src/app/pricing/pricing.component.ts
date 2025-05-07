import { Component } from '@angular/core';
import {NgForOf} from '@angular/common';
import {ButtonDirective} from 'primeng/button';
import {Ripple} from 'primeng/ripple';

@Component({
  selector: 'app-pricing',
  imports: [
    NgForOf,
    ButtonDirective,
    Ripple
  ],
  templateUrl: './pricing.component.html',
  styleUrl: './pricing.component.scss'
})
export class PricingComponent {
  plans = [
    {
      name: 'Free Tier',
      price: 0,
      description: 'Includes basic access and limited features.',
      features: [
        '1GB of included job storage',
        'View Systems',
        'Limited Real-time Monitoring',
        'Manage system licenses'
      ],
      outlined: true,
      buyButtonLabel: 'Get Started'
    },
    {
      name: 'Base Subscription',
      price: 199,
      description: 'Professional package with full access and storage.',
      features: [
        'Everything in Free Tier',
        '100GB of included storage',
        'Job Data Metrics/Collection',
        'Historical Metrics',
        'Expandable Storage: +$99 per additional 100GB',
        'Per-User Access Control: $5/month per active user seat',
        'Simple and Predictable: No hidden fees, no complicated bandwidth charges'
      ],
      outlined: false,
      buyButtonLabel: 'Buy Now'
    }
  ];
}
