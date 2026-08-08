import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { CodeReviewResponse } from '../models/code-review-response';
import {
  GitBranchReviewRequest,
  GitBranchReviewResponse
} from '../models/git-branch-review';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ReviewService {

  private readonly apiUrl = environment.apiUrl;

  constructor(private readonly http: HttpClient) {}

  reviewCode(code: string): Observable<CodeReviewResponse> {
    const url = `${this.apiUrl}/api/CodeReview/manual-review`;

    console.log('[AI Code Review] Manual POST:', url);
    console.log('[AI Code Review] Manual payload:', { code });

    return this.http.post<CodeReviewResponse>(url, { code });
  }

  reviewGitBranch(
    request: GitBranchReviewRequest
  ): Observable<GitBranchReviewResponse> {
    const url = `${this.apiUrl}/git-branch-review`;

    console.log('[AI Code Review] Git Compare POST:', url);
    console.log('[AI Code Review] Git Compare payload:', request);

    return this.http.post<GitBranchReviewResponse>(url, request);
  }
}
