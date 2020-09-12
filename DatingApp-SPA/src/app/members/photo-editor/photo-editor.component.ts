import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import {Photo} from '../../_models/photo';
import { FileUploader } from 'ng2-file-upload';
import { UserService } from 'src/app/_services/user.service';
import { AuthService } from 'src/app/_services/auth.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { environment } from 'src/environments/environment';



@Component({
  selector: 'app-photo-editor',
  templateUrl: './photo-editor.component.html',
  styleUrls: ['./photo-editor.component.css']
})
export class PhotoEditorComponent implements OnInit {

  @Input() photos: Photo[];
  @Output() getMemberPhotoChange = new EventEmitter<string>();
  uploader: FileUploader;
  hasBaseDropZoneOver: boolean;
  response: string;
  currentPhoto: Photo;

  constructor(private authService: AuthService, private userService: UserService, private alertify: AlertifyService) {


  }

  ngOnInit(): void {
    this.initializeUploader();
  }
  public fileOverBase(e: any): void {
    this.hasBaseDropZoneOver = e;
  }
  initializeUploader(){
    this.uploader = new FileUploader({
      url: environment.apiUrl + 'user/' + this.authService.decodeedToken.nameid + '/photos',
      authToken: 'Bearer ' + localStorage.getItem('token'),
      isHTML5: true,
      allowedFileType: ['image'],
      removeAfterUpload: true,
      autoUpload: false,
      maxFileSize: 10 * 1024 * 1024
    });
    this.uploader.onAfterAddingFile = (file) => {
      file.withCredentials = false;
    };
    this.uploader.onSuccessItem = (item, response, status, headers) => {
      if (response){
        const res: Photo = JSON.parse(response);
        const photo = {
          id: res.id,
          url: res.url,
          dateAdded: res.dateAdded,
          description: res.description,
          isMain: res.isMain
        };
        this.photos.push(photo);
        // if (photo.isMain){
          // this.authService.user.photoUrl = photo.url;
          // localStorage.setItem('user', JSON.stringify(this.authService.user));
        // }
      }
    };
}
setMainPhoto(photo: Photo){
  this.userService.setMainPhoto(this.authService.decodeedToken.nameid, photo.id).subscribe(() => {
   this.currentPhoto = this.photos.filter(p => p.isMain === true)[0];
   this.currentPhoto.isMain = false;
   photo.isMain = true;
   this.getMemberPhotoChange.emit(photo.url);

  }, (error) => {
    this.alertify.error(error);
  });
}
deletePhoto(photoId: number){
  this.alertify.confirm('Are you sure you want to delete this photo?', () => {
   this.userService.deletePhoto(this.authService.decodeedToken.nameid, photoId).subscribe(() => {
         this.photos.splice(this.photos.findIndex(p => p.id === photoId), 1);
         this.alertify.success('Photo Deleted successfully');
   }, (error) => {
     this.alertify.error(error);
   });
  });
}
}
