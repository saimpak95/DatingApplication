import { AuthService } from './../_services/auth.service';
import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';


@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  @Input() valuesFromHome: any;
  @Output() cancelRegister = new EventEmitter();
  model: any = {};

  constructor(private authService: AuthService) { }

  ngOnInit(): void {
  }
  register(){
    this.authService.register(this.model).subscribe((success) => {
      console.log('Registered Successfully');
    },
    (error) => {
      console.log(error);
    });

  }
  cancel(){
    this.cancelRegister.emit(false);
    console.log('Cancelled');
  }

}
