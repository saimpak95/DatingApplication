import {Routes} from '@angular/router';
import { HomeComponent } from './home/home.component';
import { MemberListComponent } from './members/member-list/member-list.component';
import { ListsComponent } from './lists/lists.component';
import { MessagesComponent } from './messages/messages.component';
import { AuthGuard } from './_guards/auth.guard';
import { MemberDetailsComponent } from './members/member-details/member-details.component';
import { MemberDetailResolver } from './_resolvers/member-details.resolver';

export const appRoute: Routes = [
    {path : '', component: HomeComponent},
    {path: '',
    runGuardsAndResolvers: 'always',
    canActivate: [AuthGuard],
    children: [

        {path : 'members', component: MemberListComponent},
        {path : 'members/:id', component: MemberDetailsComponent, resolve: {user: MemberDetailResolver}},
        {path : 'lists', component: ListsComponent},
        {path : 'message', component: MessagesComponent},

    ]

    },
    {path : '**', redirectTo: '', pathMatch: 'full'},
];
