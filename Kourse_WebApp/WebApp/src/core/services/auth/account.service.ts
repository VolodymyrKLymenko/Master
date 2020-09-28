import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';
import { Observable } from 'rxjs';
import decode from 'jwt-decode';

import { TokenService } from './token.service';
import { UserService } from './user.service';
import { ApiRoutes } from 'src/utilities/api-routes';
import { User } from 'src/core/models/auth/user.model';
import { LoginModel } from 'src/core/models/auth/login.model';
import { LoginResultModel } from 'src/core/models/auth/login-result.model';
import { RefreshTokenModel } from 'src/core/models/auth/refresh-token.model';
import { ResetPasswordModel } from 'src/core/models/auth/reset-password.model';
import { RegistrationModel } from 'src/core/models/auth/registration.model';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  public user: User;

  constructor(
    private http: HttpClient,
    private tokenService: TokenService,
    private userService: UserService
  ) { }

  public authenticate(model: LoginModel): Observable<LoginResultModel> {
    return this.http.post<LoginResultModel>(ApiRoutes.authenticate, model)
      .pipe(map(result => {
        this.updateLoginationData(result);
        return result;
      }));
  }

  public logOut(): void {
    this.user = null;
    this.tokenService.removeTokens();
  }

  public refreshToken(model: RefreshTokenModel): Observable<LoginResultModel> {
    return this.http.post<LoginResultModel>(`${ApiRoutes.refreshToken}`, model);
  }

  public resetPassword(model: ResetPasswordModel): Observable<LoginResultModel> {
    return this.http.post<LoginResultModel>(ApiRoutes.resetPassword, model)
      .pipe(map(result => {
        this.updateLoginationData(result);
        return result;
      }));
  }

  private updateLoginationData(loginResult: LoginResultModel): void {
    if (loginResult !== null) {
      const token = loginResult.accessToken;
      this.tokenService.setAccessToken(token);
      this.tokenService.setRefreshToken(loginResult.refreshToken);

      const user = {
        id: loginResult.userId,
        login: loginResult.userName
      } as User;
      this.user = user;
      this.userService.setUserInLocalStorage(user);
    }
  }

  public registerUser(model: RegistrationModel): Observable<any> {
    return this.http.post<any>(`${ApiRoutes.userProfile}`, model);
  }

  public getUser(): Observable<User> {
    return this.http.get<User>(`${ApiRoutes.userProfile}`);
  }
}
