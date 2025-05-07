import { ApplicationConfig, provideZoneChangeDetection, inject } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import {provideHttpClient, withInterceptorsFromDi} from '@angular/common/http';


import { providePrimeNG } from 'primeng/config';
import Aura from '@primeng/themes/aura';

import { routes } from './app.routes';
import {APOLLO_NAMED_OPTIONS, provideApollo} from 'apollo-angular';
import { HttpLink } from 'apollo-angular/http';
import { InMemoryCache } from '@apollo/client/core';
import { DataioColors } from './themes/dataioTheme';

export function getBaseUrl() {
  return document.getElementsByTagName('base')[0].href;
}

export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    provideAnimationsAsync(),
    providePrimeNG({
      theme: {
        preset: DataioColors,
        options: {
          darkModeSelector: '.my-app-dark',
          cssLayer: {
            name: 'primeng',
          },
        }
      }
    }),
    provideHttpClient(withInterceptorsFromDi()),
    { provide: 'BASE_URL', useFactory: getBaseUrl, deps: [] },
    provideApollo(() => {
      const httpLink = inject(HttpLink);

      return {
        link: httpLink.create({
          uri: 'api/graphql',
        }),
        cache: new InMemoryCache(),
      };
    }),
    {
      provide: APOLLO_NAMED_OPTIONS,
      useFactory: () => {
        const httpLink = inject(HttpLink);

        return {
          connexClient: {
            cache: new InMemoryCache(),
            link: httpLink.create({ uri: 'graphql' }),
          }
        };
      },
      deps: [HttpLink],
    }]
};
