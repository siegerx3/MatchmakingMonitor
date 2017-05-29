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
    this.activatedRoute.params.filter(p => p['version']).subscribe(p => this.selectVersion(p['version']));

    this.api.getAllVersions().subscribe(v => this.allVersions = v);
  }

  private selectVersion(version: string) {
    if (version == 'latest') {
      this.link = '/api/download/latest';
    } else {
      this.link = '/api/download/specific/' + version;
    }
    this.startDownloading();
  }

  private startDownloading() {
    this.downloading = true;
    this.remaining = 5;
    setTimeout(() => this.downloadLink(), 5000);
    let int = setInterval(() => {
      if (this.remaining > 0) {
        this.remaining--;
      } else {
        clearInterval(int);
      }
    }, 1000)
  }

  private downloadLink() {
    window.location.href = this.link;
  }
}
