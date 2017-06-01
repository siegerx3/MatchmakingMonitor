import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { MdDialog, MdDialogRef } from '@angular/material';

import { ApiService } from '../services/api.service';

import { ChangelogDialog } from './changelog.component';

@Component({
	templateUrl: './download.component.html'
})
export class DownloadComponent {

	public link: string;
	public downloading: boolean;
	public remaining: number;

	public selectedVersion: string;
	public allVersions: string[];

	public changelogs: string[];

	constructor(private api: ApiService, private dialog: MdDialog, private activatedRoute: ActivatedRoute) {
		this.activatedRoute.params.filter(p => p['version']).subscribe(p => {
			this.selectVersion(p['version']);
			this.api.getLatestVersion().subscribe(v => this.selectedVersion = v);
			this.startDownloading();
		});

		this.api.getAllVersions().subscribe(v => this.allVersions = v);
		this.api.getChangelogs().subscribe(c => this.changelogs = c);
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


	public openChangelog(changelog: string) {
		this.api.getChangelog(changelog).subscribe(c => {
			let dialogRef = this.dialog.open(ChangelogDialog, {
				data: {
					text: c,
					title: `Changelog for version ${changelog}`
				},
			});
		});
	}
}