import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import {map} from 'rxjs/operators';
import { JwtHelperService } from '@auth0/angular-jwt';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

helper = new JwtHelperService();
decodeedToken: any;


constructor(private http: HttpClient) { }

login(model: any){
  return this.http.post('https://localhost:44363/api/auth/login', model)
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
register(model: any){
  return this.http.post('https://localhost:44363/api/auth/register', model);
}

loggedIn(){
  const token = localStorage.getItem('token');
  return !this.helper.isTokenExpired(token);
}
}
