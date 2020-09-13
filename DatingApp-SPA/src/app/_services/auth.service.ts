import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import {map} from 'rxjs/operators';
import { JwtHelperService } from '@auth0/angular-jwt';
import { environment } from 'src/environments/environment';
import { User } from '../_models/user';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

baseUrl = environment.apiUrl;
user: User;
helper = new JwtHelperService();
decodeedToken: any;


constructor(private http: HttpClient) { }

login(model: any){
  return this.http.post(this.baseUrl + 'auth/login', model)
  .pipe(
    map((response: any) => {
      const user = response;
      if (user){
        localStorage.setItem('token', user.token);
        this.decodeedToken = this.helper.decodeToken(user.token);
        console.log(this.decodeedToken);
      }

    })
  );
}
register(user: User){
  return this.http.post(this.baseUrl + 'auth/register', user);
}

loggedIn(){
  const token = localStorage.getItem('token');
  return !this.helper.isTokenExpired(token);
}
}
