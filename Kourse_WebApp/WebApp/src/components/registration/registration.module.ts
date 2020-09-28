import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';

import { RegistrationComponent } from './registration.component';
import { RegistrationRoutingModule } from './registration-routing.module';

@NgModule({
  declarations: [
    RegistrationComponent
  ],
  imports: [
    CommonModule,
    RegistrationRoutingModule,
    FormsModule,
    ReactiveFormsModule
  ],
  providers: []
})
export class RegistrationModule { }
