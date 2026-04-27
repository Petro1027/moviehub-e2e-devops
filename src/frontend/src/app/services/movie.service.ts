import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Movie, PagedResult } from '../models/movie.model';

@Injectable({
  providedIn: 'root'
})
export class MovieService {
  private readonly apiUrl = 'http://localhost:5052/api/movies';

  constructor(private readonly httpClient: HttpClient) {}

  getMovies(page: number, pageSize: number, searchTerm: string): Observable<PagedResult<Movie>> {
    let params = new HttpParams()
      .set('page', page)
      .set('pageSize', pageSize);

    if (searchTerm.trim()) {
      params = params.set('search', searchTerm.trim());
    }

    return this.httpClient.get<PagedResult<Movie>>(this.apiUrl, { params });
  }
}
