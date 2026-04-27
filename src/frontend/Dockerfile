FROM node:24-alpine AS build
WORKDIR /app

COPY package*.json ./
RUN npm ci

COPY . .
RUN npm run build

FROM nginx:alpine AS runtime

COPY --from=build /app/dist/frontend /tmp/frontend-dist

RUN if [ -f /tmp/frontend-dist/browser/index.html ]; then \
      cp -r /tmp/frontend-dist/browser/* /usr/share/nginx/html/; \
    else \
      cp -r /tmp/frontend-dist/* /usr/share/nginx/html/; \
    fi

COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80
