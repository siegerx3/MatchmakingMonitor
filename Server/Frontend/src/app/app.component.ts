import { Component } from '@angular/core';
import { Router, NavigationEnd } from '@angular/router';

import { ApiService } from './services/api.service';

@Component({
	selector: 'app-root',
	templateUrl: './app.component.html'
})
export class AppComponent {
	constructor(public api: ApiService, public router: Router) {
		this.router.events.subscribe(event => {
			if (event instanceof NavigationEnd && window.location.hostname !== 'localhost' && window.location.hostname !== '127.0.0.1') {
				ga('set', 'page', event.urlAfterRedirects);
				ga('send', 'pageview');
			}
		});
	}
}