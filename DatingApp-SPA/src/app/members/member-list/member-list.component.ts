import { Component, OnInit } from '@angular/core';
import { User } from '../../_models/user';
import { UserService } from '../../_services/user.service';
import { AlertifyService } from '../../_services/alertify.service';
import { Pagination, Paginationresult } from 'src/app/_models/pagination';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css']
})
export class MemberListComponent implements OnInit {

  pageNumber = 1;
  pageSize = 30;
  users: User[];
  user: User = JSON.parse(localStorage.getItem('user'));
  genderList = [{value: 'Male', display: 'Male'}, {value: 'Female', display: 'Female'}];
  userParams: any = {};
  pagination: Pagination;
  constructor(private userService: UserService, private alertify: AlertifyService, private route: ActivatedRoute) { }

  ngOnInit(): void {
    this.route.data.subscribe(data => {
      this.userParams.gender = this.user.gender === 'Female' ? 'Male' : 'Female';
      this.userParams.minAge = 18;
      this.userParams.maxAge = 90;
      this.userParams.orderBy = 'lastActive';
      this.loadUsers();
    });
  }

  resetFilters(){
    this.userParams.gender = this.user.gender === 'Female' ? 'Male' : 'Female';
    this.userParams.minAge = 18;
    this.userParams.maxAge = 90;
    this.route.data.subscribe(data => {
    this.users = data.users.result; });
  }

loadUsers(){
this.userService.getUsers(this.pageNumber, this.pageSize, this.userParams).subscribe((res: Paginationresult<User[]>) => {
  this.users = res.result;
},
 (error) => {
   this.alertify.error(error);
 });
}
}
