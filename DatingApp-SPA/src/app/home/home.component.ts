import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {

  registerMode = false;

  constructor(private http: HttpClient) { }
  value: any;
  ngOnInit(): void {
    this.getValues();
  }

  registerToggler(){
    this.registerMode = !this.registerMode;
  }

  getValues(){
    this.http.get('https://localhost:44363/api/Values').subscribe((response) => {
      this.value = response;
    }, (error) => {
      console.log(error);
    });
  }
  cancelRegisterMode(registerMode: boolean){
    this.registerMode = registerMode;
  }
}
