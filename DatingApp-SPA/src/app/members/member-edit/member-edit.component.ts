import { Component, OnInit, ViewChild } from '@angular/core';
import { User } from 'src/app/_models/user';
import {  ActivatedRoute } from '@angular/router';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { UserService } from 'src/app/_services/user.service';
import { NgForm } from '@angular/forms';
import { AuthService } from 'src/app/_services/auth.service';

@Component({
  selector: 'app-member-edit',
  templateUrl: './member-edit.component.html',
  styleUrls: ['./member-edit.component.css']
})
export class MemberEditComponent implements OnInit {

  @ViewChild('editForm', {static: true}) editForm: NgForm;
  user: User;

  constructor(private route: ActivatedRoute, private alertify: AlertifyService,
              private userService: UserService, private authService: AuthService) { }

  ngOnInit(): void {
    this.route.data.subscribe((data) => {
      this.user = data.user;
    });
  }
 updateUser(){

  this.userService.updateUser(this.authService.decodeedToken.nameid, this.user).subscribe((next) => {
    this.alertify.success('Profile updated successfully');
  }, (error) => {
    this.alertify.error(error);
  });
  this.editForm.reset(this.user);
 }
}
