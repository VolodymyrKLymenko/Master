import { environment } from 'src/environments/environment';

export const ApiRoutes = {
    authenticate: `${environment.baseUrl}/accounts/login`,
    refreshToken: ``,
    resetPassword: ``,
    userProfile: `${environment.baseUrl}/accounts/users`
};
