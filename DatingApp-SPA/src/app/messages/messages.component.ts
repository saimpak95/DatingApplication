import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Message } from '../_models/message';
import { Pagination, Paginationresult } from '../_models/pagination';
import { AlertifyService } from '../_services/alertify.service';
import { AuthService } from '../_services/auth.service';
import { UserService } from '../_services/user.service';

@Component({
  selector: 'app-messages',
  templateUrl: './messages.component.html',
  styleUrls: ['./messages.component.css']
})
export class MessagesComponent implements OnInit {
  pageNumber = 1;
  pageSize = 30;
  messages: Message[];
  pagination: Pagination;
  messageContainer = 'Unread';

  constructor(private userService: UserService, private authService: AuthService,
              private alerify: AlertifyService, private route: ActivatedRoute) { }

  ngOnInit(): void {
    this.route.data.subscribe(data => {
      this.messages = data.messages.result;
      this.pagination = data.messages.pagination;
    });
  }
  loadMessages(){
    this.userService.getMesages(this.authService.decodeedToken.nameid, this.pageNumber,
                    this.pageSize, this.messageContainer).subscribe((res: Paginationresult<Message[]>) => {
                        this.messages = res.result;
                    }, (error) => {
                      this.alerify.error(error);
                    });
  }
deleteMessage(messageId: number){
  this.alerify.confirm('Are you sure you want to delete?', () => {
    this.userService.deleteMessage(this.authService.decodeedToken.nameid, messageId).subscribe(
      (response) => {
        this.messages.splice(this.messages.findIndex(m => m.id === messageId), 1);
        this.alerify.success('Message has been deleted successfully');
      }, (error) => {
        this.alerify.error(error);
      }
    );
  })
}

}
