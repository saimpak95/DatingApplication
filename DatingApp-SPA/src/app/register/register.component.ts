import { AlertifyService } from './../_services/alertify.service';
import { AuthService } from './../_services/auth.service';
import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';
import { User } from '../_models/user';
import { Router } from '@angular/router';


@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  constructor(private router: Router,
              private authService: AuthService, private alertify: AlertifyService, private formBuilder: FormBuilder) { }

  @Input() valuesFromHome: any;
  @Output() cancelRegister = new EventEmitter();
  registerForm: FormGroup;
  user: User;
  colorTheme = 'theme-red';

   bsConfig: Partial<BsDatepickerConfig>;

  ngOnInit(): void {
   this.createRegisterForm();
   this.bsConfig = {
     containerClass: this.colorTheme
   };
  }

  applyTheme(pop: any) {
    // create new object on each property change
    // so Angular can catch object reference change
    this.bsConfig = Object.assign({}, { containerClass: this.colorTheme });
    setTimeout(() => {
      pop.show();
    });
  }
  createRegisterForm(){
    this.registerForm = this.formBuilder.group({
      gender: ['Male'],
      username: ['', Validators.required],
      knownAs: ['', Validators.required],
      dateOfBirth: [null, Validators.required],
      city: ['', Validators.required],
      country: ['', Validators.required],
      password: ['', [Validators.required, Validators.minLength(4), Validators.maxLength(8)]],
      confirmPassword: ['', Validators.required],

    }, {validator: this.passswordMatchValidator});
  }

  passswordMatchValidator(g: FormGroup){
   return g.get('password').value === g.get('confirmPassword').value ? null : {mismatch: true};
  }
  register(){
    if (this.registerForm.valid){
      this.user = Object.assign({}, this.registerForm.value);
      this.authService.register(this.user).subscribe((success) => {
      this.alertify.success('Registered Successfully');
    },
    (error) => {
      this.alertify.error(error);
    }, () => {
      this.authService.login(this.user).subscribe(() => {
        this.router.navigate(['/members']);
      });
    });
    }
  }
  cancel(){
    this.cancelRegister.emit(false);
    console.log('Cancelled');
  }

}
