import { Component } from '@angular/core';

import { Subscription } from 'rxjs';

import { LoaderService, LoaderState } from 'src/core/loader/loader.service';
import { TokenService } from 'src/core/services/auth/token.service';
import { AccountService } from 'src/core/services/auth/account.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.styl']
})
export class AppComponent {
  public hasLoadedResources = false;
  private subscription: Subscription;

  constructor(
    private router: Router,
    private loaderService: LoaderService,
    private tokenService: TokenService,
    private accountService: AccountService) { }

  public ngOnInit(): void {
    this.setLoaderState();
  }

  public logOutUser(): void {
    this.accountService.logOut();
    this.router.navigate(['/login']);
  }

  public get isAuthenticated(): boolean {
    return this.tokenService.getAccessToken() !== null;
  }

  private setLoaderState(): void {
    this.subscription = this.loaderService.loaderState
      .subscribe((state: LoaderState) => {
        this.hasLoadedResources = state.show;
      });
  }
}
