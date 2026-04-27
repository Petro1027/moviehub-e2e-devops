# MovieHub E2E DevOps Project - User Guide

## 1. Overview

MovieHub is a simple movie catalog application.

The application allows users to:

- view movies
- search movies
- use pagination
- open movie details
- view reviews for a movie
- create a new review
- delete an existing review

---

## 2. Opening the application

When the system is running with Docker Compose, open:

```text
http://localhost:8080
```

When the system is running in Kubernetes with port-forwarding, open:

```text
http://localhost:8080
```

The homepage shows the movie catalog.

---

## 3. Movie list page

The movie list page displays the available movies.

Each movie card shows:

- title
- director
- genre
- year
- rating
- description
- details button

The movie data is loaded from the Catalog API.

Catalog API endpoint used by the frontend:

```text
GET http://localhost:5052/api/movies
```

---

## 4. Searching movies

The search field can be used to filter movies.

Search works by:

- title
- director
- genre

Example search terms:

```text
nolan
sci-fi
matrix
```

Example API request:

```text
GET http://localhost:5052/api/movies?search=nolan
```

---

## 5. Pagination

The movie list supports pagination.

The user can navigate between pages with:

- Previous button
- Next button

Example API request:

```text
GET http://localhost:5052/api/movies?page=1&pageSize=3
```

---

## 6. Movie details page

Each movie card has a `Részletek` button.

Clicking this button opens the movie details page.

The details page shows:

- movie title
- movie director
- movie year
- movie genre
- movie rating
- movie description
- reviews connected to the movie

Example URL:

```text
http://localhost:8080/movies/{movieId}
```

The movie details are loaded from the Catalog API:

```text
GET http://localhost:5052/api/movies/{movieId}
```

---

## 7. Reviews section

The reviews section is displayed on the movie details page.

The reviews are loaded from the Reviews API:

```text
GET http://localhost:5053/api/reviews/movie/{movieId}
```

Each review shows:

- reviewer name
- score
- comment
- delete button

---

## 8. Creating a review

To create a review:

1. Open a movie details page.
2. Enter a reviewer name.
3. Select a score between 1 and 5.
4. Enter a comment.
5. Click `Értékelés mentése`.

Example:

```text
Name: Gergo
Score: 5
Comment: Kubernetes alatt is mukodik.
```

The frontend sends the review to the Reviews API:

```text
POST http://localhost:5053/api/reviews
```

Example request body:

```json
{
  "movieId": "example-movie-id",
  "reviewerName": "Gergo",
  "score": 5,
  "comment": "Nagyon jo film."
}
```

After successful creation, the new review appears in the review list.

---

## 9. Review validation

The application validates the review form.

Required fields:

- reviewer name
- score
- comment

The score must be between 1 and 5.

Valid score values:

```text
1
2
3
4
5
```

---

## 10. Deleting a review

To delete a review:

1. Open a movie details page.
2. Find the review.
3. Click the `Törlés` button.

The frontend sends a delete request to the Reviews API:

```text
DELETE http://localhost:5053/api/reviews/{reviewId}
```

After successful deletion, the review disappears from the list.

---

## 11. Main user flow

A typical user flow:

1. Open the application.
2. Browse the movie list.
3. Search for a movie.
4. Open the movie details page.
5. Read existing reviews.
6. Create a new review.
7. Delete the review if needed.

---

## 12. Demo scenario

This scenario can be used during presentation.

### Step 1: Start the system

Docker Compose:

```powershell
docker compose up -d --build
```

Or Kubernetes with port-forwarding:

```powershell
kubectl port-forward svc/catalog-api 5052:8080 -n moviehub
kubectl port-forward svc/reviews-api 5053:8080 -n moviehub
kubectl port-forward svc/frontend 8080:80 -n moviehub
```

### Step 2: Open frontend

```text
http://localhost:8080
```

### Step 3: Show movie list

Explain that the movie list is loaded from the Catalog API and stored in MongoDB.

### Step 4: Search movies

Search for:

```text
nolan
```

### Step 5: Open movie details

Click the `Részletek` button.

Explain that this page uses two backend services:

- Catalog API for movie details
- Reviews API for reviews

### Step 6: Create review

Create a review:

```text
Name: Gergo
Score: 5
Comment: ArgoCD-val telepitett Kubernetes rendszer mukodik.
```

### Step 7: Delete review

Click `Törlés`.

---

## 13. Technical explanation

The application demonstrates a complete end-to-end workflow.

Frontend:

```text
Angular application served by Nginx
```

Backend:

```text
ASP.NET Catalog API
ASP.NET Reviews API
```

Database:

```text
MongoDB 8
```

Local orchestration:

```text
Docker Compose
```

CI:

```text
GitHub Actions builds Docker images
```

Container registry:

```text
GitHub Container Registry
```

Kubernetes:

```text
Kubernetes manifests deploy all components
```

CD:

```text
ArgoCD automatically syncs Kubernetes manifests from GitHub
```

---

## 14. Required local ports

When running the system through Docker Compose or Kubernetes port-forwarding, these ports are used:

```text
Frontend:    localhost:8080
Catalog API: localhost:5052
Reviews API: localhost:5053
```

If the frontend cannot load movies, check that Catalog API is available:

```powershell
kubectl port-forward svc/catalog-api 5052:8080 -n moviehub
```

If the frontend cannot load reviews, check that Reviews API is available:

```powershell
kubectl port-forward svc/reviews-api 5053:8080 -n moviehub
```

If the frontend is not available, check:

```powershell
kubectl port-forward svc/frontend 8080:80 -n moviehub
```

---

## 15. Summary

Implemented user-facing features:

- movie list
- search
- pagination
- movie details
- review list
- review creation
- review deletion

The application is suitable for demonstrating full-stack development and DevOps deployment workflows.