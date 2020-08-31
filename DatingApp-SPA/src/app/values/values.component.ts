import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-values',
  templateUrl: './values.component.html',
  styleUrls: ['./values.component.css']
})
export class ValuesComponent implements OnInit {
  value: any;
  constructor(private http: HttpClient) { }

  ngOnInit(): void {
    this.getValues();
  }
  getValues(){
    this.http.get('https://localhost:44363/api/Values').subscribe((response) => {
      this.value = response;
    }, (error) => {
      console.log(error);
    });
  }
}
