import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { Movie } from '../../models/movie.model';
import { MovieService } from '../../services/movie.service';

@Component({
  selector: 'app-movie-list',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './movie-list.component.html',
  styleUrl: './movie-list.component.scss'
})
export class MovieListComponent implements OnInit {
  searchTerm = '';
  page = 1;
  pageSize = 3;
  totalCount = 0;
  totalPages = 1;

  movies: Movie[] = [];

  isLoading = false;
  errorMessage = '';

  constructor(private readonly movieService: MovieService) {}

  ngOnInit(): void {
    this.loadMovies();
  }

  loadMovies(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.movieService.getMovies(this.page, this.pageSize, this.searchTerm).subscribe({
      next: result => {
        this.movies = result.items;
        this.page = result.page;
        this.pageSize = result.pageSize;
        this.totalCount = result.totalCount;
        this.totalPages = Math.max(1, result.totalPages);
        this.isLoading = false;
      },
      error: error => {
        console.error('Failed to load movies', error);
        this.errorMessage = 'Nem sikerült betölteni a filmeket. Ellenőrizd, hogy fut-e a Catalog API.';
        this.isLoading = false;
      }
    });
  }

  onSearchChanged(value: string): void {
    this.searchTerm = value;
    this.page = 1;
    this.loadMovies();
  }

  previousPage(): void {
    if (this.page > 1) {
      this.page--;
      this.loadMovies();
    }
  }

  nextPage(): void {
    if (this.page < this.totalPages) {
      this.page++;
      this.loadMovies();
    }
  }
}
