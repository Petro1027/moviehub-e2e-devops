# MovieHub E2E DevOps Project

Ez egy egyetemi projektfeladat, amelynek célja egy teljes end-to-end fejlesztési és telepítési folyamat bemutatása.

## Projekt célja

A MovieHub egy egyszerû filmkatalógus alkalmazás, amelyben filmeket lehet kezelni.

A projekt bemutatja:

- frontend alkalmazás fejlesztését,
- backend REST API fejlesztését,
- MongoDB adatbázis használatát,
- Docker konténerizálást,
- GitHub Actions CI workflow-t,
- Docker image-ek feltöltését registry-be,
- Kubernetes manifestek használatát,
- lokális Kubernetes telepítést,
- ArgoCD alapú CD workflow-t.

## Tervezett technológiák

- Frontend: Angular
- Backend: ASP.NET Core / C#
- Database: MongoDB
- Containerization: Docker
- CI: GitHub Actions
- Registry: GitHub Container Registry
- Deployment: Kubernetes
- CD: ArgoCD

## Tervezett funkciók

- filmek listázása,
- film létrehozása,
- film szerkesztése,
- film törlése,
- film részletek megtekintése,
- pagination,
- értékelések kezelése.

## Projektstruktúra

`	ext
moviehub-e2e-devops/
+¦¦ src/
-   +¦¦ frontend/
-   L¦¦ backend/
+¦¦ k8s/
+¦¦ docs/
+¦¦ .github/
-   L¦¦ workflows/
+¦¦ .gitignore
L¦¦ README.md
