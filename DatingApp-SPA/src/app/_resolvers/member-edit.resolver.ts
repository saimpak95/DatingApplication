import {Injectable} from '@angular/core';
import { Resolve, Router, ActivatedRouteSnapshot } from '@angular/router';
import { User } from '../_models/user';
import { AlertifyService } from '../_services/alertify.service';
import { UserService } from '../_services/user.service';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { AuthService } from '../_services/auth.service';

@Injectable()
export class MemberEditResolver implements Resolve<User>{
    // tslint:disable-next-line: max-line-length
    constructor(private authService: AuthService, private router: Router, private alertify: AlertifyService, private userService: UserService){}
    resolve(route: ActivatedRouteSnapshot): Observable<User>{
        return this.userService.getUserByID(this.authService.decodeedToken.nameid).pipe(
            catchError((error) => {
                this.alertify.error('Problem retrieving your data');
                this.router.navigate(['/members']);
                return of(null);
            })
        );
    }
}
