import {Routes} from '@angular/router';
import { HomeComponent } from './home/home.component';
import { MemberListComponent } from './members/member-list/member-list.component';
import { ListsComponent } from './lists/lists.component';
import { MessagesComponent } from './messages/messages.component';
import { AuthGuard } from './_guards/auth.guard';
import { MemberDetailsComponent } from './members/member-details/member-details.component';
import { MemberDetailResolver } from './_resolvers/member-details.resolver';
import { MemberEditComponent } from './members/member-edit/member-edit.component';
import { MemberEditResolver } from './_resolvers/member-edit.resolver';
import { PreventUnsavedChangesGuard } from './_guards/prevent-unsaved-changes.guard';
import { MemberListResolver } from './_resolvers/member-list-resolver';
import { ListsResolver } from './_resolvers/list.resolver';
import { MessageResolver } from './_resolvers/messages.resolver';

export const appRoute: Routes = [
    {path : '', component: HomeComponent},
    {path: '',
    runGuardsAndResolvers: 'always',
    canActivate: [AuthGuard],
    children: [

        {path : 'members', component: MemberListComponent,  resolve: {users: MemberListResolver}},
        {path : 'members/:id', component: MemberDetailsComponent, resolve: {user: MemberDetailResolver}},
        {path : 'lists', component: ListsComponent, resolve: {users: ListsResolver}},
        {path: 'member/edit', component: MemberEditComponent,
                              resolve: {user: MemberEditResolver},
                              canDeactivate: [PreventUnsavedChangesGuard]},
        {path : 'message', component: MessagesComponent, resolve: {messages: MessageResolver}},

    ]

    },
    {path : '**', redirectTo: '', pathMatch: 'full'},
];
