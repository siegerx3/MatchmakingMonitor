import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

import { ApiService } from '../services/api.service';

@Component({
	templateUrl: './download.component.html'
})
export class DownloadComponent {

	public link: string;
	public downloading: boolean;
	public remaining: number;

	public selectedVersion: string;
	public allVersions: string[];

	constructor(private api: ApiService, private activatedRoute: ActivatedRoute) {
		this.activatedRoute.params.filter(p => p['version']).subscribe(p => {
			this.selectVersion(p['version']);
			this.api.getLatestVersion().subscribe(v => this.selectedVersion = v);
			this.startDownloading();
		});

		this.api.getAllVersions().subscribe(v => this.allVersions = v);
	}

	public selectVersion(version: string) {
		if (version == 'latest') {
			this.link = '/api/download/latest';
		} else {
			this.link = '/api/download/specific/' + version;
		}
	}

	timeout: any;
	interval: any;

	public startDownloading() {
		clearTimeout(this.timeout);
		clearInterval(this.interval);

		const time = 5;
		this.downloading = true;
		this.remaining = time;
		this.timeout = setTimeout(() => this.downloadLink(), time * 1000);
		this.interval = setInterval(() => {
			if (this.remaining > 0) {
				this.remaining--;
			} else {
				clearInterval(this.interval);
			}
		},
			1000);
	}

	private downloadLink() {
		window.location.href = this.link;
	}
}
