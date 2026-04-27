export interface Movie {
  id: string;
  title: string;
  director: string;
  genre: string;
  year: number;
  rating: number;
  description: string;
  createdAt: string;
}

export interface PagedResult<T> {
  items: T[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}
