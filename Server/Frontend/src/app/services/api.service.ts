import { Injectable } from '@angular/core';
import { Http } from '@angular/http';

import { Observable } from 'rxjs/Observable';


@Injectable()
export class ApiService {
  constructor(private http: Http) {
    this.getLatestVersion().subscribe();
  }

  private _latestVersion: string;
  public get latestVersion(): string {
    return this._latestVersion;
  }

  public getLatestVersion(): Observable<string> {
    return this.http.get('/api/version/latest')
      .map(response => response.json(), err => console.log(err))
      .do(lv => this._latestVersion = lv);
  }

  public getAllVersions(): Observable<string[]> {
    return this.http.get('/api/version/all')
      .map(response => response.json(), err => console.log(err));
  }
}