import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { Movie } from '../../models/movie.model';

@Component({
  selector: 'app-movie-list',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './movie-list.component.html',
  styleUrl: './movie-list.component.scss'
})
export class MovieListComponent {
  searchTerm = '';
  page = 1;
  pageSize = 3;

  movies: Movie[] = [
    {
      id: '1',
      title: 'The Matrix',
      director: 'The Wachowskis',
      genre: 'Sci-Fi',
      year: 1999,
      rating: 9.1,
      description: 'A hacker discovers that reality is a simulation.',
      createdAt: new Date().toISOString()
    },
    {
      id: '2',
      title: 'Inception',
      director: 'Christopher Nolan',
      genre: 'Sci-Fi',
      year: 2010,
      rating: 8.8,
      description: "A thief enters people's dreams to steal secrets.",
      createdAt: new Date().toISOString()
    },
    {
      id: '3',
      title: 'The Lord of the Rings: The Fellowship of the Ring',
      director: 'Peter Jackson',
      genre: 'Fantasy',
      year: 2001,
      rating: 8.9,
      description: 'A young hobbit begins a journey to destroy a powerful ring.',
      createdAt: new Date().toISOString()
    },
    {
      id: '4',
      title: 'Interstellar',
      director: 'Christopher Nolan',
      genre: 'Sci-Fi',
      year: 2014,
      rating: 8.7,
      description: 'A team travels through a wormhole in search of a new home for humanity.',
      createdAt: new Date().toISOString()
    },
    {
      id: '5',
      title: 'The Dark Knight',
      director: 'Christopher Nolan',
      genre: 'Action',
      year: 2008,
      rating: 9.0,
      description: 'Batman faces the Joker in Gotham City.',
      createdAt: new Date().toISOString()
    }
  ];

  get filteredMovies(): Movie[] {
    const normalizedSearchTerm = this.searchTerm.trim().toLowerCase();

    if (!normalizedSearchTerm) {
      return this.movies;
    }

    return this.movies.filter(movie =>
      movie.title.toLowerCase().includes(normalizedSearchTerm) ||
      movie.director.toLowerCase().includes(normalizedSearchTerm) ||
      movie.genre.toLowerCase().includes(normalizedSearchTerm)
    );
  }

  get totalPages(): number {
    return Math.max(1, Math.ceil(this.filteredMovies.length / this.pageSize));
  }

  get pagedMovies(): Movie[] {
    const startIndex = (this.page - 1) * this.pageSize;

    return this.filteredMovies.slice(startIndex, startIndex + this.pageSize);
  }

  onSearchChanged(value: string): void {
    this.searchTerm = value;
    this.page = 1;
  }

  previousPage(): void {
    if (this.page > 1) {
      this.page--;
    }
  }

  nextPage(): void {
    if (this.page < this.totalPages) {
      this.page++;
    }
  }
}
