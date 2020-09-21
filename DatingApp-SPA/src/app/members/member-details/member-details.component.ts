import { Component, OnInit, ViewChild } from '@angular/core';
import { User } from 'src/app/_models/user';

import { ActivatedRoute } from '@angular/router';
import { TabsetComponent } from 'ngx-bootstrap/tabs';
import { UserService } from 'src/app/_services/user.service';
import { AuthService } from 'src/app/_services/auth.service';
import { AlertifyService } from 'src/app/_services/alertify.service';

@Component({
  selector: 'app-member-details',
  templateUrl: './member-details.component.html',
  styleUrls: ['./member-details.component.css']
})
export class MemberDetailsComponent implements OnInit {

  user: User;
  @ViewChild('staticTabs', { static: true }) staticTabs: TabsetComponent;
  constructor(private route: ActivatedRoute, private userService: UserService, private authService: AuthService, private alertify: AlertifyService) { }
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

  sendLike(recipientId: number){
    this.userService.sendLike(this.authService.decodeedToken.nameid, recipientId).subscribe((response) => {
      this.alertify.success('you have liked: ' + this.user.knownAs);
    }, (error) => {
      this.alertify.error(error);
    });
  }

}

