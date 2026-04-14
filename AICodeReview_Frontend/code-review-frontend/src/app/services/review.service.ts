import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CodeReviewResponse } from '../models/code-review-response';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})

export class ReviewService {

  constructor(private http: HttpClient) {}

  reviewCode(code: string): Observable<CodeReviewResponse> {
    return this.http.post<CodeReviewResponse>(
      `${environment.apiUrl}/CodeReview/manual-review`,
      { code }
    );
  }
}
