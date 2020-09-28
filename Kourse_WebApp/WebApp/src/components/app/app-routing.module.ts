import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AuthGuard } from 'src/core/guards/auth.guard';
import { AuthModule } from '../auth/auth.module';
import { MapModule } from '../map/map.module';
import { RegistrationModule } from '../registration/registration.module';

const routes: Routes = [
  {
    path: '',
    pathMatch: 'full',
    redirectTo: '/map'
  },
  {
    path: 'map',
    loadChildren: () => MapModule,
    canActivate: [AuthGuard]
  },
  {
    path: 'login',
    loadChildren: () => AuthModule
  },
  {
    path: 'registration',
    loadChildren: () => RegistrationModule
  },
  {
    path: 'profile',
    loadChildren: '../profile/profile.module#ProfileModule'
  },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
