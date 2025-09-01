# Developer Store — Local Execution & Authorization Guide (EN)

This repository contains the Developer Store technical challenge: an ASP.NET Core Web API (NET 8) for user registration, authentication and management.

This document explains, in detail, how to run the API locally (Visual Studio or CLI), how to run only the dependencies using Docker, and how to authenticate and use the Swagger UI with JWT (Authorize).

---


## Quick summary

- Project root: `backend/`
- API project: `backend/src/Ambev.DeveloperEvaluation.WebApi`
- New Docker file with only dependencies: `backend/docker-compose.deps.yml`
- The API is intended to be run locally (Visual Studio or dotnet run) while Postgres/Mongo/Redis can run as Docker containers.

---

## Prerequisites

- .NET 8 SDK installed
- Docker and Docker Compose installed (only for dependencies)
- Visual Studio 2022/2023 or VS Code (if you use Visual Studio, recommended: open the solution file `backend/Ambev.DeveloperEvaluation.sln`)

---

## 1) Start only the dependencies with Docker

We provide a lightweight compose file containing only the database/cache services.

From repository root:

cd backend
docker-compose -f docker-compose.deps.yml up -d

This will start:
- PostgreSQL (container name: `ambev_developer_evaluation_database`) → port 5432
- MongoDB (container name: `ambev_developer_evaluation_nosql`) → port 27017
- Redis (container name: `ambev_developer_evaluation_cache`) → port 6379 (requires password)

Verify containers with:
docker ps

If you prefer, the original `docker-compose.yml` was adjusted to only include dependencies as well. Use `docker-compose up -d` from `backend` if you want to use that file.

---

## 2) Run the API locally (Visual Studio)

Recommended: run the API locally and use Docker only for dependencies. This is what the reviewers will likely expect.

1. Open Visual Studio.
2. File → Open → Project/Solution → select `backend/Ambev.DeveloperEvaluation.sln`.
3. In Solution Explorer, right-click `Ambev.DeveloperEvaluation.WebApi` → Set as Startup Project.
4. Select the run profile (top toolbar): choose `https` or `http` (profiles are pre-configured).
   - Profiles already include a `ConnectionStrings__DefaultConnection` environment variable pointing to the local Docker Postgres:
     Host=localhost;Port=5432;Database=developer_evaluation;Username=developer;Password=ev@luAt10n
5. Press F5 (Debug) or Ctrl+F5 (Run).
6. When the app starts, Swagger UI will open automatically:
   - http://localhost:5119/swagger or https://localhost:7181/swagger

Notes:
- The app applies EF Core migrations automatically on startup by default. If you want to skip migrations and apply them manually, set the environment variable `SKIP_MIGRATIONS=true` for the run profile or in your environment, and then run:
  cd backend/src/Ambev.DeveloperEvaluation.WebApi
  dotnet ef database update

---

## 3) Run the API locally (CLI)

If you prefer to run the API from a terminal:

Open a terminal and run:

cd backend\src\Ambev.DeveloperEvaluation.WebApi
set "ConnectionStrings__DefaultConnection=Host=localhost;Port=5432;Database=developer_evaluation;Username=developer;Password=ev@luAt10n"
set "ASPNETCORE_ENVIRONMENT=Development"
dotnet run --urls "http://localhost:5119;https://localhost:7181"

After startup Swagger will be available at:
http://localhost:5119/swagger

---

## 4) How authentication works (short)

- The API uses JWT tokens.
- The login endpoint (POST `/api/Auth`) returns a JSON response with authentication data. The application layer returns a result object that contains a `Token` property (the JWT).
- The WebApi wraps responses using `ApiResponseWithData<T>` — so the token is available at `response.data.token` (or `response.data.Token` depending on your client).
- All protected endpoints require the header:
  Authorization: Bearer {token}

---

## 5) Step-by-step: create a user, authenticate, use Authorization in Swagger

1) Create a user (example JSON)
- Endpoint: POST /api/Users
- Example body:
{
  "username": "testuser",
  "password": "Password@123",
  "phone": "(11) 98765-4321",
  "email": "test.user@example.com",
  "status": 1,
  "role": 1
}

You can create the user using Swagger `POST /api/Users` (Try it out) or using curl:

curl -X POST "http://localhost:5119/api/Users" -H "Content-Type: application/json" -d ^
"{\"username\":\"testuser\",\"password\":\"Password@123\",\"phone\":\"(11) 98765-4321\",\"email\":\"test.user@example.com\",\"status\":1,\"role\":1}"

Response: 201 Created on success.

2) Authenticate (get token)
- Endpoint: POST /api/Auth
- Example body:
{
  "email": "test.user@example.com",
  "password": "Password@123"
}

Using curl:

curl -X POST "http://localhost:5119/api/Auth" -H "Content-Type: application/json" -d ^
"{\"email\":\"test.user@example.com\",\"password\":\"Password@123\"}"

Typical response body (wrapped):
{
  "success": true,
  "message": "User authenticated successfully",
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "id": "GUID",
    "name": "testuser",
    "email": "test.user@example.com",
    "phone": "...",
    "role": "..."
  }
}

Take the value of `data.token` (the JWT).

3) Use "Authorize" in Swagger
- Open http://localhost:5119/swagger
- Click the "Authorize" button (lock icon).
- In the input labeled `Bearer` paste the token value, prefixed with "Bearer ":
  Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6...
- Click "Authorize" → Close.
- Swagger will now attach the Authorization header for requests made in the UI.

4) Example curl call to protected endpoint:

curl -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6..." http://localhost:5119/api/Users/me

---

## 6) Troubleshooting

- If the API fails to connect to Postgres:
  - Ensure the dependencies are started: `cd backend && docker-compose -f docker-compose.deps.yml up -d`
  - Verify container state: `docker ps`
  - Check Postgres logs: `cd backend && docker-compose logs --tail=200 ambev.developerevaluation.database`
  - Check port availability (Windows): `netstat -a -n | findstr 5119`

- If Swagger shows "Failed to fetch" when calling an endpoint (common during Try it out / POST):
  - Cause: the application is configured with HTTPS redirection (the server redirects HTTP requests to HTTPS). Swagger UI loaded over HTTP may trigger a redirect (307) which the browser's fetch can block or fail. The browser will also block mixed-content if the UI is loaded via HTTPS and the request targets HTTP.
  - Quick fix:
    1. Open the HTTPS Swagger endpoint directly in your browser:
       https://localhost:7181/swagger
    2. If the browser warns about an invalid/self-signed certificate, click "Advanced" → proceed to the site (or add a security exception). This allows the Swagger UI to make requests to the same scheme without redirection issues.
    3. Retry the request (Try it out) or use the "Authorize" modal and then call protected endpoints.
  - Alternative:
    - Use a terminal client (curl) that ignores cert validation for quick testing:
      curl -k -X POST "https://localhost:7181/api/Auth" -H "Content-Type: application/json" -d '{"email":"test.user@example.com","password":"Password@123"}'
    - Or ensure you use the correct scheme that the server expects (HTTPS) and match the port.

- Migrations failing:
  - Option: set SKIP_MIGRATIONS=true environment variable for the run profile and apply migrations manually:
    cd backend/src/Ambev.DeveloperEvaluation.WebApi
    dotnet ef database update

---

## 7) Running tests

From repository root:

cd backend
dotnet test

Note: Integration/functional tests might also expect dependencies to be running in Docker.

---

## 8) For evaluators

- Start dependencies
- Run the WebAPI locally (Visual Studio preferred)
- Use Swagger to create a user → authenticate → press Authorize with the token → test protected endpoints
- The repository applies migrations automatically on startup; the flow should work out-of-the-box if Docker is running and ports are available.

---

If you want, I will:
- Add screenshots showing the exact flow inside Swagger.
- Or run a quick curl auth here and paste the token (requires your confirmation).
