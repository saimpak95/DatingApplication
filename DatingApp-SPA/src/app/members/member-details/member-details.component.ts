import { Component, OnInit, ViewChild } from '@angular/core';
import { User } from 'src/app/_models/user';

import { ActivatedRoute } from '@angular/router';
import { TabsetComponent } from 'ngx-bootstrap/tabs';

@Component({
  selector: 'app-member-details',
  templateUrl: './member-details.component.html',
  styleUrls: ['./member-details.component.css']
})
export class MemberDetailsComponent implements OnInit {

  user: User;
  @ViewChild('staticTabs', { static: true }) staticTabs: TabsetComponent;
  constructor(private route: ActivatedRoute) { }
  ngOnInit(): void {
    this.route.data.subscribe((data) => {
      this.user = data.user;
    });
    this.route.queryParams.subscribe(params => {
      console.log(params);
      const selectedTab = params.tab;
      this.staticTabs.tabs[selectedTab > 0 ? selectedTab : 0].active = true;
      console.log(selectedTab);
    });
  }


  selectTab(tabId: number) {
    this.staticTabs.tabs[tabId].active = true;
  }

  //  members/4
 /* loadUser(){
    this.userService.getUserByID(+this.route.snapshot.params.id).subscribe((user: User) => {
      this.user = user;
    }, (error) => {
      this.alertify.error(error);
    });
    */

}

