import {ActivatedRoute, NavigationEnd, Router} from '@angular/router';
import {BehaviorSubject, filter} from 'rxjs';
import {Injectable} from '@angular/core';

export interface BreadcrumbEntry {
  label: string;
  url: string;
  icon: string | null;
}

@Injectable({
  providedIn: 'root',
})
export class BreadcrumbService {
  // breadcrumbs: Array<{ label: string, url: string }> = [];
  private breadcrumbsSubject = new BehaviorSubject<Array<BreadcrumbEntry>>([]);
  public readonly breadcrumbs$ = this.breadcrumbsSubject.asObservable();

  constructor(private router: Router, private activatedRoute: ActivatedRoute) {
    this.router.events.pipe(
      filter(event => event instanceof NavigationEnd)
    ).subscribe(() => {
      // this.breadcrumbs = this.createBreadcrumbs(this.activatedRoute.root);
      const newBreadcrumbs = this.createBreadcrumbs(this.activatedRoute.root);
      this.breadcrumbsSubject.next(newBreadcrumbs);
    });
  }

  private createBreadcrumbs(route: ActivatedRoute, url: string = '', breadcrumbs: Array<BreadcrumbEntry> = []): Array<BreadcrumbEntry> {
    const children: ActivatedRoute[] = route.children;

    for (const child of children) {
      const routeSegments = child.snapshot.url.map(segment => segment.path);
      if (routeSegments.length === 0) {
        continue;
      }

      let routeURL;

      if (child.snapshot.data['ignoreParameters'] === true) {
        routeURL = routeSegments[0];
      }
      else {
        routeURL = routeSegments.join('/');
      }

      url += `/${routeURL}`;

      const label = child.snapshot.data['breadcrumb'];
      if (label) {
        breadcrumbs.push({ label, url, icon: null });
      }

      // Recurse into next level
      this.createBreadcrumbs(child, url, breadcrumbs);

      // const routeURL: string = child.snapshot.url.map(segment => segment.path).join('/');
      // if (routeURL) {
      //   if (child.snapshot.data['ignoreParameters'] === true) {
      //
      //   url += `/${routeURL}`;
      // }
      //
      // const label = child.snapshot.data['breadcrumb'];
      // if (label) {
      //   breadcrumbs.push({ label, url, icon: '' });
      // }
      //
      // // 🔁 Keep traversing deeper children
      // this.createBreadcrumbs(child, url, breadcrumbs);
    }

    return breadcrumbs;
  }

  updateLastLabel(newLabel: string) {
    const newBreadcrumbs = this.createBreadcrumbs(this.activatedRoute.root);
    newBreadcrumbs.push({ label: newLabel, url: '', icon: '' });
    this.breadcrumbsSubject.next(newBreadcrumbs);
    // if (this.breadcrumbs.length > 0) {
    //   const updated = [...this.breadcrumbs];
    //   updated[updated.length - 1] = {
    //     ...updated[updated.length - 1],
    //     label: newLabel
    //   };
    //   this.setBreadcrumbs(updated);
    }
}
