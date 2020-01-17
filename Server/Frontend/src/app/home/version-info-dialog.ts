import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
  selector: 'version-info-dialog',
  template:
    `<h1 mat-dialog-title>{{data.title}}</h1>
    <mat-dialog-content>
      <strong>
            Since update 0.9.0.0 (15.1.2020) this app will not work anymore. Wargaming has removed something that it required to run.
            <br />
            However i am currently working on a new version that is almost ready.
            <br />
            At the moment you can only get in on the discord in the #new-version-testing channel.
            <br />
            Once the new version is final this website will redirect to the new one.
            <br />
            Sorry for the inconvenience.
      </strong>
    </mat-dialog-content>`
})
export class VersionInfoDialog {
  constructor(@Inject(MAT_DIALOG_DATA) public data: any) { }
}