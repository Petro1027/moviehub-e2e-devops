import { Routes } from '@angular/router';
import { MovieListComponent } from './pages/movie-list/movie-list.component';

export const routes: Routes = [
  {
    path: '',
    component: MovieListComponent
  },
  {
    path: '**',
    redirectTo: ''
  }
];
