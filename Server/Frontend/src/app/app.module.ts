import { NgModule } from '@angular/core';
import { HttpClientModule } from '@angular/common/http';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule, NoopAnimationsModule } from '@angular/platform-browser/animations';

import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatSelectModule } from '@angular/material/select';
import { MatListModule } from '@angular/material/list';
import { MatRadioModule } from '@angular/material/radio';
import { MatDialogModule } from '@angular/material/dialog';
import { MatInputModule } from '@angular/material/input';

import { NgxMdModule } from 'ngx-md';

import { AppComponent } from './app.component';
import { AppRoutingModule } from './app-routing.module';

import { HomeComponent } from './home/home.component';
import { DownloadComponent } from './download/download.component';
import { ChangelogDialog } from './download/changelog.component';

import { ApiService } from './services/api.service';

import { environment } from 'environments/environment.prod';
import { PrivacyComponent } from './pricacy/privacy.component';
import { VersionInfoDialog } from './home/version-info-dialog';

@NgModule({
  declarations: [
    AppComponent,
    HomeComponent,
    DownloadComponent,
    PrivacyComponent,
    ChangelogDialog,
    VersionInfoDialog
  ],
  entryComponents: [ChangelogDialog, VersionInfoDialog],
  imports: [
    BrowserModule,
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,
    BrowserAnimationsModule,
    NoopAnimationsModule,
    AppRoutingModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatProgressSpinnerModule,
    MatSelectModule,
    MatToolbarModule,
    MatCardModule,
    MatListModule,
    MatRadioModule,
    MatDialogModule,
    NgxMdModule.forRoot()
  ],
  providers: [ApiService],
  bootstrap: [AppComponent]
})
export class AppModule {
}