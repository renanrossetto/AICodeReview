import { CommonModule } from '@angular/common';
import { Component, ViewEncapsulation } from '@angular/core';

import { GitCompare } from '../git-compare/git-compare';
import { ManualReview } from '../manual-review/manual-review';

@Component({
  selector: 'app-code-review',
  standalone: true,
  imports: [CommonModule, ManualReview, GitCompare],
  templateUrl: './code-review.html',
  styleUrls: ['./code-review.css'],
  encapsulation: ViewEncapsulation.None
})
export class CodeReview {
  activeMode: 'manual' | 'pull-request' = 'manual';
}
