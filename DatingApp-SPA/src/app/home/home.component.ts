import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {

  registerMode = false;
  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }
  value: any;
  ngOnInit(): void {
    this.getValues();
  }

  registerToggler(){
    this.registerMode = !this.registerMode;
  }

  getValues(){
    this.http.get(this.baseUrl + 'Values').subscribe((response) => {
      this.value = response;
    }, (error) => {
      console.log(error);
    });
  }
  cancelRegisterMode(registerMode: boolean){
    this.registerMode = registerMode;
  }
}
