import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { InboxComponent } from './inbox/inbox.component';

const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'inbox', component: InboxComponent },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
