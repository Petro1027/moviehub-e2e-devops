export interface Review {
  id: string;
  movieId: string;
  reviewerName: string;
  score: number;
  comment: string;
  createdAt: string;
}

export interface CreateReviewRequest {
  movieId: string;
  reviewerName: string;
  score: number;
  comment: string;
}
