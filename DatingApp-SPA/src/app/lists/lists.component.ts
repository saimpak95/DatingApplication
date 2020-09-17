import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Pagination, Paginationresult } from '../_models/pagination';
import { User } from '../_models/user';
import { AlertifyService } from '../_services/alertify.service';
import { AuthService } from '../_services/auth.service';
import { UserService } from '../_services/user.service';

@Component({
  selector: 'app-lists',
  templateUrl: './lists.component.html',
  styleUrls: ['./lists.component.css']
})
export class ListsComponent implements OnInit {

  pageNumber = 1;
  pageSize = 30;
  users: User[];
  pagination: Pagination;
  likeParams: string;

  constructor(private authService: AuthService, private userService: UserService,
              private route: ActivatedRoute, private alertify: AlertifyService) { }

  ngOnInit(): void {

   this.likeParams = 'Likers';
   this.loadUsers();

  }
  loadUsers(){
    this.userService.getUsers(this.pageNumber, this.pageSize, null, this.likeParams).subscribe((res: Paginationresult<User[]>) => {
      this.users = res.result;
      console.log(this.users);
    },
     (error) => {
       this.alertify.error(error);
     });
    }
}
