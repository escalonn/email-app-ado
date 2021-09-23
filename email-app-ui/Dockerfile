# newest lts
FROM node:14 as build

WORKDIR /app

# restore dependencies as separate layer
COPY package.json package-lock.json ./
# deterministic restore using package-lock
RUN npm ci

COPY . ./

RUN npx ng build

# stable
FROM nginx:1.20

WORKDIR /usr/share/nginx/html

# remove default nginx page
RUN rm -rf *
# config server for deep linking
COPY nginx-custom.conf /etc/nginx/conf.d/default.conf

COPY --from=build /app/dist/* ./
