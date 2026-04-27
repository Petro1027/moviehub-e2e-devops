# MovieHub E2E DevOps Project - Architecture

## 1. Project goal

The goal of this project is to demonstrate a complete end-to-end development and deployment workflow.

The project includes:

- Angular frontend application
- ASP.NET backend services
- MongoDB database
- Docker containerization
- Docker Compose local runtime
- GitHub Actions CI workflow
- GitHub Container Registry image publishing
- Kubernetes manifests
- ArgoCD-based GitOps deployment

The application domain is a simple movie catalog system.

---

## 2. Domain model

### Movie

The `Movie` entity represents a movie in the catalog.

Main fields:

- id
- title
- director
- genre
- year
- rating
- description
- createdAt

The `Movie` entity is managed by the Catalog API.

### Review

The `Review` entity represents a user review connected to a movie.

Main fields:

- id
- movieId
- reviewerName
- score
- comment
- createdAt

The `Review` entity is managed by the Reviews API.

---

## 3. Main components

### Frontend

Technology:

- Angular
- TypeScript
- SCSS
- Nginx

Responsibilities:

- display movies
- search movies
- paginate movies
- show movie details
- display reviews
- create reviews
- delete reviews

Runtime URLs:

```text
Local Angular development: http://localhost:4200
Docker/Kubernetes frontend: http://localhost:8080
```

---

### Catalog API

Technology:

- ASP.NET
- C#
- MongoDB.Driver

Responsibilities:

- manage movie catalog data
- provide movie list
- provide pagination
- provide search
- provide movie details
- seed default movie data when the database is empty

Main endpoints:

```text
GET    /api/movies
GET    /api/movies/{id}
POST   /api/movies
PUT    /api/movies/{id}
DELETE /api/movies/{id}
GET    /api/movies/top-rated
```

Runtime URL:

```text
http://localhost:5052
```

---

### Reviews API

Technology:

- ASP.NET
- C#
- MongoDB.Driver

Responsibilities:

- manage reviews for movies
- list reviews by movie id
- create reviews
- delete reviews
- validate review score

Main endpoints:

```text
GET    /api/reviews/movie/{movieId}
POST   /api/reviews
DELETE /api/reviews/{id}
```

Runtime URL:

```text
http://localhost:5053
```

---

### MongoDB

Technology:

- MongoDB 8

Responsibilities:

- store movie documents
- store review documents

Collections:

```text
movies
reviews
```

Internal service name in Docker Compose and Kubernetes:

```text
mongodb
```

Connection string used inside containers and Kubernetes:

```text
mongodb://moviehub:moviehub-password@mongodb:27017/?authSource=admin
```

---

## 4. Architecture overview

```text
User
 |
 v
Angular Frontend
 |
 |--- GET /api/movies ---------------------> Catalog API
 |                                           |
 |                                           v
 |                                        MongoDB
 |
 |--- GET/POST/DELETE /api/reviews --------> Reviews API
                                             |
                                             v
                                          MongoDB
```

---

## 5. Container architecture

The system uses the following containers:

```text
moviehub-frontend
moviehub-catalog-api
moviehub-reviews-api
mongo:8
```

Custom images are built by GitHub Actions and pushed to GitHub Container Registry:

```text
ghcr.io/petro1027/moviehub-frontend:latest
ghcr.io/petro1027/moviehub-catalog-api:latest
ghcr.io/petro1027/moviehub-reviews-api:latest
```

---

## 6. Docker Compose architecture

Docker Compose runs:

- MongoDB
- Catalog API
- Reviews API
- Angular frontend

Local URLs:

```text
Frontend:    http://localhost:8080
Catalog API: http://localhost:5052/api/movies
Reviews API: http://localhost:5053/api/reviews/movie/{movieId}
MongoDB:     localhost:27017
```

---

## 7. Kubernetes architecture

Kubernetes manifests are stored in the `k8s/` folder.

Main Kubernetes resources:

- namespace
- MongoDB secret
- MongoDB deployment
- MongoDB service
- Catalog API deployment
- Catalog API service
- Reviews API deployment
- Reviews API service
- Frontend deployment
- Frontend service

Namespace:

```text
moviehub
```

Kubernetes services:

```text
mongodb
catalog-api
reviews-api
frontend
```

---

## 8. CI workflow

The GitHub Actions workflow is located here:

```text
.github/workflows/ci-build-push.yml
```

The workflow:

1. runs on push to the `main` branch
2. builds the frontend Docker image
3. builds the Catalog API Docker image
4. builds the Reviews API Docker image
5. pushes all images to GitHub Container Registry

---

## 9. CD workflow with ArgoCD

ArgoCD is installed manually into the local Kubernetes cluster.

The ArgoCD Application manifest is located here:

```text
argocd/moviehub-application.yaml
```

The ArgoCD Application watches:

```text
https://github.com/Petro1027/moviehub-e2e-devops.git
```

Target path:

```text
k8s
```

Target branch:

```text
main
```

Auto sync is enabled:

```text
prune: true
selfHeal: true
```

This means ArgoCD automatically synchronizes the Kubernetes manifests from GitHub into the local Kubernetes cluster.

---

## 10. Summary

This project demonstrates:

- frontend development
- backend development
- REST API design
- MongoDB integration
- microservice-based backend design
- Docker containerization
- Docker Compose orchestration
- CI workflow
- Docker image publishing
- Kubernetes deployment
- ArgoCD GitOps-based CD workflow