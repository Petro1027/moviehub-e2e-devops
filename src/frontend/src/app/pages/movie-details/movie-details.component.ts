import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { Movie } from '../../models/movie.model';
import { Review } from '../../models/review.model';
import { MovieService } from '../../services/movie.service';
import { ReviewService } from '../../services/review.service';

@Component({
  selector: 'app-movie-details',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './movie-details.component.html',
  styleUrl: './movie-details.component.scss'
})
export class MovieDetailsComponent implements OnInit {
  movieId = '';
  movie: Movie | null = null;
  reviews: Review[] = [];

  reviewerName = '';
  score = 5;
  comment = '';

  isLoading = false;
  isSavingReview = false;
  errorMessage = '';
  reviewErrorMessage = '';

  constructor(
    private readonly route: ActivatedRoute,
    private readonly movieService: MovieService,
    private readonly reviewService: ReviewService
  ) {}

  ngOnInit(): void {
    this.movieId = this.route.snapshot.paramMap.get('id') ?? '';

    if (!this.movieId) {
      this.errorMessage = 'Hiányzó film azonosító.';
      return;
    }

    this.loadMovie();
    this.loadReviews();
  }

  loadMovie(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.movieService.getMovieById(this.movieId).subscribe({
      next: movie => {
        this.movie = movie;
        this.isLoading = false;
      },
      error: error => {
        console.error('Failed to load movie', error);
        this.errorMessage = 'Nem sikerült betölteni a film részleteit.';
        this.isLoading = false;
      }
    });
  }

  loadReviews(): void {
    this.reviewService.getReviewsByMovieId(this.movieId).subscribe({
      next: reviews => {
        this.reviews = reviews;
      },
      error: error => {
        console.error('Failed to load reviews', error);
        this.reviewErrorMessage = 'Nem sikerült betölteni az értékeléseket.';
      }
    });
  }

  onReviewerNameChanged(value: string): void {
    this.reviewerName = value;
  }

  onScoreChanged(value: string): void {
    this.score = Number(value);
  }

  onCommentChanged(value: string): void {
    this.comment = value;
  }

  createReview(): void {
    this.reviewErrorMessage = '';

    if (!this.reviewerName.trim()) {
      this.reviewErrorMessage = 'A név megadása kötelező.';
      return;
    }

    if (this.score < 1 || this.score > 5) {
      this.reviewErrorMessage = 'A pontszámnak 1 és 5 között kell lennie.';
      return;
    }

    if (!this.comment.trim()) {
      this.reviewErrorMessage = 'A komment megadása kötelező.';
      return;
    }

    this.isSavingReview = true;

    this.reviewService.createReview({
      movieId: this.movieId,
      reviewerName: this.reviewerName.trim(),
      score: this.score,
      comment: this.comment.trim()
    }).subscribe({
      next: review => {
        this.reviews = [review, ...this.reviews];
        this.reviewerName = '';
        this.score = 5;
        this.comment = '';
        this.isSavingReview = false;
      },
      error: error => {
        console.error('Failed to create review', error);
        this.reviewErrorMessage = 'Nem sikerült menteni az értékelést.';
        this.isSavingReview = false;
      }
    });
  }

  deleteReview(reviewId: string): void {
    this.reviewService.deleteReview(reviewId).subscribe({
      next: () => {
        this.reviews = this.reviews.filter(review => review.id !== reviewId);
      },
      error: error => {
        console.error('Failed to delete review', error);
        this.reviewErrorMessage = 'Nem sikerült törölni az értékelést.';
      }
    });
  }
}
