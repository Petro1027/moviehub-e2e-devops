import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { CreateReviewRequest, Review } from '../models/review.model';

@Injectable({
  providedIn: 'root'
})
export class ReviewService {
  private readonly apiUrl = 'http://localhost:5053/api/reviews';

  constructor(private readonly httpClient: HttpClient) {}

  getReviewsByMovieId(movieId: string): Observable<Review[]> {
    return this.httpClient.get<Review[]>(`${this.apiUrl}/movie/${movieId}`);
  }

  createReview(request: CreateReviewRequest): Observable<Review> {
    return this.httpClient.post<Review>(this.apiUrl, request);
  }

  deleteReview(id: string): Observable<void> {
    return this.httpClient.delete<void>(`${this.apiUrl}/${id}`);
  }
}
