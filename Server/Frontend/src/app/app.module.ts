import { NgModule } from '@angular/core';
import { HttpModule } from '@angular/http';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule, NoopAnimationsModule } from '@angular/platform-browser/animations';
import { MaterialModule, MdProgressSpinnerModule, MdSelectModule, MdCardModule } from '@angular/material';

import { AppComponent } from './app.component';
import { AppRoutingModule } from './app-routing.module';

import { HomeComponent } from './home/home.component';
import { DownloadComponent } from './download/download.component';

import { ApiService } from './services/api.service';

@NgModule({
	declarations: [
		AppComponent,
		HomeComponent,
		DownloadComponent
	],
	imports: [
		BrowserModule,
		HttpModule,
		FormsModule,
		ReactiveFormsModule,
		BrowserAnimationsModule,
		NoopAnimationsModule,
		AppRoutingModule,
		MaterialModule,
		MdProgressSpinnerModule,
		MdSelectModule,
		MdCardModule
	],
	providers: [ApiService],
	bootstrap: [AppComponent]
})
export class AppModule { }
