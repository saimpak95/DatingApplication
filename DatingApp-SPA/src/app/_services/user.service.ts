import { Injectable } from '@angular/core';
import {environment} from '../../environments/environment';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { User } from '../_models/user';
import { Paginationresult } from '../_models/pagination';
import { map } from 'rxjs/operators';
import { Message } from '../_models/message';


@Injectable({
  providedIn: 'root'
})
export class UserService {
  baseUrl = environment.apiUrl;

  constructor(private httpClient: HttpClient) { }

  getUsers(page?, itemsPerPage?, userParams?, likesParam?): Observable<Paginationresult<User[]>> {
    const paginatedResult: Paginationresult<User[]> = new Paginationresult<User[]>();

    let params = new HttpParams();

    if (page != null && itemsPerPage != null) {
      params = params.append('pageNumber', page);
      params = params.append('pageSize', itemsPerPage);
    }
    if (userParams != null) {
      params = params.append('minAge', userParams.minAge);
      params = params.append('maxAge', userParams.maxAge);
      params = params.append('gender', userParams.gender);
      params = params.append('orderBy', userParams.orderBy);
    }

    if (likesParam === 'Likers') {
      params = params.append('Likers', 'true');
    }

    if (likesParam === 'Likees') {
      params = params.append('Likees', 'true');
    }

    return this.httpClient.get<User[]>(this.baseUrl + 'users', { observe: 'response', params})
      .pipe(
        map(response => {
          paginatedResult.result = response.body;
          if (response.headers.get('Pagination') != null) {
            paginatedResult.pagination = JSON.parse(response.headers.get('Pagination'));
          }
          return paginatedResult;
        })
      );
  }
  getUserByID(id: number): Observable<User>{
    return this.httpClient.get<User>(this.baseUrl + 'users/' + id);
  }
  updateUser(id: number, user: User){
    return this.httpClient.put(this.baseUrl + 'users/' + id, user);
  }
  setMainPhoto(userId: number, id: number){
    return this.httpClient.post(this.baseUrl + 'user/' + userId + '/photos/' + id + '/SetMain', {});
  }
  deletePhoto(userId: number, photoId: number){
    return this.httpClient.delete(this.baseUrl + 'user/' + userId + '/photos/' + photoId);
   }
   sendLike(id: number, recipientId: number){
    return this.httpClient.post(this.baseUrl + 'users/' + id + '/like/' + recipientId, {});
   }
   getMesages(id: number, page?, itemsPerPage?, messageContainer?){
    const paginatedResult: Paginationresult<Message[]> = new Paginationresult<Message[]>();
    let params = new HttpParams();
    params = params.append('MessageContainer', messageContainer);
    if (page != null && itemsPerPage != null) {
      params = params.append('pageNumber', page);
      params = params.append('pageSize', itemsPerPage);
    }
    return this.httpClient.get<Message[]>(this.baseUrl + 'user/' + id + '/Message', {observe: 'response', params})
    .pipe(map((response) => {
      paginatedResult.result = response.body;
      if (response.headers.get('Pagination') !== null) {
        paginatedResult.pagination = JSON.parse(response.headers.get('Pagination'));
      }
      return paginatedResult;
    }));
   }
   getMessageThread(userId: number, recipientId: number){
    return this.httpClient.get<Message[]>(this.baseUrl + 'user/' + userId + '/Message/thread/' + recipientId);
   }
   sendMessage(userId, message: Message){
    return this.httpClient.post(this.baseUrl + 'user/' + userId + '/Message', message);

   }
   deleteMessage(userId: number, messageId: number){
     return this.httpClient.post(this.baseUrl + 'user/' + userId + '/Message/' + messageId, {});
   }
   markAsRead(userId: number, messageId: number){
     return this.httpClient.post(this.baseUrl + 'user/' + userId + '/Message/' + messageId + '/read', {}).subscribe();
   }
}
