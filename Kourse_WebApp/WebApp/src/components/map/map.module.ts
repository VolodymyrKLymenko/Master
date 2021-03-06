import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';

import { SidebarModule } from 'primeng/sidebar';
import { ButtonModule } from 'primeng/button';
import { TableModule } from 'primeng/table';
import { GMapModule } from 'primeng/gmap';
import { AccordionModule } from 'primeng/accordion';

import { MapComponent } from './map.component';
import { MapRoutingModule } from './map-routing.module';

@NgModule({
  declarations: [
    MapComponent
  ],
  imports: [
    CommonModule,
    MapRoutingModule,
    FormsModule,
    ReactiveFormsModule,
    AccordionModule,
    GMapModule,
    TableModule,
    SidebarModule,
    ButtonModule
  ],
  providers: []
})
export class MapModule { }
