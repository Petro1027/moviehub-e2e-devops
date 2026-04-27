# MovieHub E2E DevOps Project - Deployment Guide

## 1. Overview

This document describes how to run and deploy the MovieHub system.

The system contains:

- Angular frontend
- Catalog API
- Reviews API
- MCP Server
- MongoDB
- Docker Compose
- Kubernetes manifests
- ArgoCD GitOps deployment

---

## 2. Prerequisites

Required tools:

- Git
- Docker Desktop
- Docker Compose
- Node.js and npm
- .NET SDK
- kubectl
- Docker Desktop Kubernetes enabled
- GitHub account

Recommended tools:

- JetBrains Rider
- PowerShell
- Browser

---

## 3. Clone repository

```powershell
git clone https://github.com/Petro1027/moviehub-e2e-devops.git
cd moviehub-e2e-devops
```

Check status:

```powershell
git status
```

Expected result:

```text
nothing to commit, working tree clean
```

---

## 4. Run with Docker Compose

Start the full system:

```powershell
docker compose up -d --build
```

Check containers:

```powershell
docker ps
```

Expected containers:

```text
moviehub-mongodb
moviehub-catalog-api
moviehub-reviews-api
moviehub-mcp-server
moviehub-frontend
```

Application URLs:

```text
Frontend:    http://localhost:8080
Catalog API: http://localhost:5052/api/movies
Reviews API: http://localhost:5053/api/reviews/movie/{movieId}
MCP Server:  http://localhost:5054/health
MongoDB:     localhost:27017
```

Stop services:

```powershell
docker compose down
```

Stop services and remove MongoDB volume:

```powershell
docker compose down -v
```

---

## 5. Test Docker Compose deployment

Test Catalog API:

```powershell
Invoke-RestMethod http://localhost:5052/api/movies | ConvertTo-Json -Depth 5
```

Get a movie id:

```powershell
$movies = Invoke-RestMethod "http://localhost:5052/api/movies"
$movieId = @($movies.items)[0].id
$movieId
```

Create a review:

```powershell
$body = @{
    movieId = $movieId
    reviewerName = "Gergo"
    score = 5
    comment = "Docker Compose alatt mukodik."
} | ConvertTo-Json

$createdReview = Invoke-RestMethod `
    -Uri "http://localhost:5053/api/reviews" `
    -Method Post `
    -Body $body `
    -ContentType "application/json"

$createdReview | ConvertTo-Json -Depth 5
```

Test MCP Server health endpoint:

```powershell
Invoke-RestMethod http://localhost:5054/health | ConvertTo-Json -Depth 5
```

Test MCP tools list:

```powershell
$body = @{
    jsonrpc = "2.0"
    id = 1
    method = "tools/list"
    params = @{}
} | ConvertTo-Json -Depth 5

Invoke-RestMethod `
    -Uri "http://localhost:5054/mcp" `
    -Method Post `
    -Body $body `
    -ContentType "application/json" | ConvertTo-Json -Depth 10
```

Open frontend:

```text
http://localhost:8080
```

---

## 6. CI workflow

The CI workflow is located here:

```text
.github/workflows/ci-build-push.yml
```

The workflow builds and pushes these Docker images:

```text
ghcr.io/petro1027/moviehub-frontend:latest
ghcr.io/petro1027/moviehub-catalog-api:latest
ghcr.io/petro1027/moviehub-reviews-api:latest
ghcr.io/petro1027/moviehub-mcp-server:latest
```

To check CI:

1. Open the GitHub repository.
2. Go to the `Actions` tab.
3. Open `Build and Push Docker Images`.
4. Check that all jobs are green.

To check images:

1. Open the GitHub repository.
2. Check the `Packages` section.
3. Confirm that the four custom packages are visible.

Expected packages:

```text
moviehub-frontend
moviehub-catalog-api
moviehub-reviews-api
moviehub-mcp-server
```

---

## 7. Enable Kubernetes

Docker Desktop Kubernetes must be enabled.

Path:

```text
Docker Desktop -> Settings -> Kubernetes -> Enable Kubernetes
```

Check cluster:

```powershell
kubectl get nodes
```

Expected result:

```text
docker-desktop   Ready
```

---

## 8. Manual Kubernetes deployment

Apply namespace:

```powershell
kubectl apply -f k8s/namespace.yaml
```

Apply MongoDB:

```powershell
kubectl apply -f k8s/mongo/
```

Apply Catalog API:

```powershell
kubectl apply -f k8s/catalog-api/
```

Apply Reviews API:

```powershell
kubectl apply -f k8s/reviews-api/
```

Apply MCP Server:

```powershell
kubectl apply -f k8s/mcp-server/
```

Apply Frontend:

```powershell
kubectl apply -f k8s/frontend/
```

Check resources:

```powershell
kubectl get all -n moviehub
```

Expected pods:

```text
mongodb       Running
catalog-api   Running
reviews-api   Running
mcp-server    Running
frontend      Running
```

Expected services:

```powershell
kubectl get svc -n moviehub
```

Expected services:

```text
mongodb
catalog-api
reviews-api
mcp-server
frontend
```

---

## 9. Test Kubernetes deployment with port-forward

Open four separate terminals.

Catalog API:

```powershell
kubectl port-forward svc/catalog-api 5052:8080 -n moviehub
```

Reviews API:

```powershell
kubectl port-forward svc/reviews-api 5053:8080 -n moviehub
```

MCP Server:

```powershell
kubectl port-forward svc/mcp-server 5054:8080 -n moviehub
```

Frontend:

```powershell
kubectl port-forward svc/frontend 8080:80 -n moviehub
```

Open frontend:

```text
http://localhost:8080
```

Test:

- movie list loads
- search works
- pagination works
- movie details page opens
- reviews are displayed
- review creation works
- review deletion works
- MCP Server health endpoint works
- MCP Server tools/list works

---

## 10. MCP Server test

Health endpoint:

```powershell
Invoke-RestMethod http://localhost:5054/health | ConvertTo-Json -Depth 5
```

Expected result:

```text
service: MovieHub.McpServer
status: Healthy
```

Tools list:

```powershell
$body = @{
    jsonrpc = "2.0"
    id = 1
    method = "tools/list"
    params = @{}
} | ConvertTo-Json -Depth 5

Invoke-RestMethod `
    -Uri "http://localhost:5054/mcp" `
    -Method Post `
    -Body $body `
    -ContentType "application/json" | ConvertTo-Json -Depth 10
```

Expected tools:

```text
get_project_info
get_architecture_summary
get_deployment_checklist
```

Tool call:

```powershell
$body = @{
    jsonrpc = "2.0"
    id = 2
    method = "tools/call"
    params = @{
        name = "get_deployment_checklist"
        arguments = @{}
    }
} | ConvertTo-Json -Depth 10

Invoke-RestMethod `
    -Uri "http://localhost:5054/mcp" `
    -Method Post `
    -Body $body `
    -ContentType "application/json" | ConvertTo-Json -Depth 10
```

Expected result:

```text
Deployment checklist
```

---

## 11. ArgoCD installation

Create namespace:

```powershell
kubectl create namespace argocd
```

Install ArgoCD:

```powershell
kubectl apply -n argocd --server-side --force-conflicts -f https://raw.githubusercontent.com/argoproj/argo-cd/stable/manifests/install.yaml
```

Check ArgoCD pods:

```powershell
kubectl get pods -n argocd
```

Expected pods:

```text
argocd-application-controller   Running
argocd-server                   Running
argocd-repo-server              Running
argocd-redis                    Running
```

---

## 12. Access ArgoCD UI

Start port-forward:

```powershell
kubectl port-forward svc/argocd-server -n argocd 8081:443
```

Open:

```text
https://localhost:8081
```

Username:

```text
admin
```

Get initial password:

```powershell
$encodedPassword = kubectl -n argocd get secret argocd-initial-admin-secret -o jsonpath="{.data.password}"
[System.Text.Encoding]::UTF8.GetString([System.Convert]::FromBase64String($encodedPassword))
```

---

## 13. ArgoCD Application

The ArgoCD Application manifest is located here:

```text
argocd/moviehub-application.yaml
```

Apply it:

```powershell
kubectl apply -f argocd/moviehub-application.yaml
```

Check application:

```powershell
kubectl get applications -n argocd
```

Expected result:

```text
moviehub   Synced   Healthy
```

In the ArgoCD UI, the `moviehub` application should show:

```text
Healthy
Synced
Sync OK
```

The application should include:

```text
namespace/moviehub
deployment/mongodb
service/mongodb
deployment/catalog-api
service/catalog-api
deployment/reviews-api
service/reviews-api
deployment/mcp-server
service/mcp-server
deployment/frontend
service/frontend
```

---

## 14. Test ArgoCD-managed deployment

Check resources:

```powershell
kubectl get all -n moviehub
```

Start port-forward commands in separate terminals:

```powershell
kubectl port-forward svc/catalog-api 5052:8080 -n moviehub
```

```powershell
kubectl port-forward svc/reviews-api 5053:8080 -n moviehub
```

```powershell
kubectl port-forward svc/mcp-server 5054:8080 -n moviehub
```

```powershell
kubectl port-forward svc/frontend 8080:80 -n moviehub
```

Open frontend:

```text
http://localhost:8080
```

Verify:

- movie list loads
- movie details page works
- review creation works
- review deletion works
- MCP Server health endpoint works
- MCP Server tools/list works
- ArgoCD application is Healthy and Synced

---

## 15. Troubleshooting

### Frontend cannot load movies

Check that Catalog API port-forward is running:

```powershell
kubectl port-forward svc/catalog-api 5052:8080 -n moviehub
```

Check API directly:

```powershell
Invoke-RestMethod http://localhost:5052/api/movies | ConvertTo-Json -Depth 5
```

### Frontend cannot load reviews

Check that Reviews API port-forward is running:

```powershell
kubectl port-forward svc/reviews-api 5053:8080 -n moviehub
```

### MCP Server is not available

Check that MCP Server port-forward is running:

```powershell
kubectl port-forward svc/mcp-server 5054:8080 -n moviehub
```

Check health endpoint:

```powershell
Invoke-RestMethod http://localhost:5054/health | ConvertTo-Json -Depth 5
```

### Pod cannot pull GHCR image

Check pod status:

```powershell
kubectl get pods -n moviehub
```

Describe pod:

```powershell
kubectl describe pod -l app=mcp-server -n moviehub
```

Possible causes:

- GHCR package is private
- image name is wrong
- image tag does not exist
- GitHub Actions workflow has not pushed the image yet

### ArgoCD application is OutOfSync

Open ArgoCD UI and click `SYNC`, or wait for auto sync.

---

## 16. Cleanup

Delete MovieHub namespace:

```powershell
kubectl delete namespace moviehub
```

Delete ArgoCD namespace:

```powershell
kubectl delete namespace argocd
```

Stop Docker Compose:

```powershell
docker compose down
```