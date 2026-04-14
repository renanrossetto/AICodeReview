import { Routes } from '@angular/router';
import { CodeReview } from './pages/code-review/code-review';

export const routes: Routes = [
  {
    path: 'review',
    component: CodeReview
  },
  {
    path: '',
    redirectTo: 'review',
    pathMatch: 'full'
  }
];

