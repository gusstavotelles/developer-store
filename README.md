# Developer Store — Local Execution, Architecture & Guidelines (EN)

This repository contains the Developer Store technical challenge: an ASP.NET Core Web API (.NET 8) for user registration, authentication and sales management. Below you will find runnable instructions, architecture & design rationale, tools used and notes to help understand the project and reproduce the verification flows.

---

## Project overview

- Purpose: implement a small but realistic backend demonstrating domain modeling, validation, authentication (JWT), persistence, tests and documentation (Swagger).
- Scope: Users (register/auth) and Sales (create, list, get, cancel) with business rules (discounts, quantity limits).
- Location: API project is `backend/src/Ambev.DeveloperEvaluation.WebApi` and solution root is `backend/Ambev.DeveloperEvaluation.sln`.

---

## How to run (quick)

1. Start dependencies:
   - cd backend
   - docker-compose -f docker-compose.deps.yml up -d
2. Run API locally:
   - cd backend/src/Ambev.DeveloperEvaluation.WebApi
   - dotnet run
3. Swagger UI:
   - http://localhost:5119/swagger (or https://localhost:7181/swagger)

Detailed run instructions and troubleshooting are in the README sections below.

---

## Architecture & design decisions

This project follows a layered/clean architecture approach with clear separation of responsibilities:

- Domain layer
  - Entities, value objects, domain validation and domain-specific exceptions live here.
  - Contains business rules and domain validators (e.g. phone, password, email).
- Application layer
  - Implements use-cases as commands/handlers (MediatR based).
  - Each feature exposes a Command/Handler/Result/Validator set.
  - Keeps orchestration, mapping and application validation separate from persistence.
- Infrastructure/ORM
  - Entity Framework Core (Npgsql provider) implementation.
  - Repositories (UserRepository, SaleRepository) encapsulate database access.
  - Migrations live under ORM project so schema is versioned.
- WebApi layer
  - Controllers, request/response DTOs, AutoMapper profiles, middleware and Swagger configuration.
  - Handles HTTP concerns and maps DTOs to application commands.
- IoC / Module initializers
  - All DI registrations are centralized into module initializers for each module (Application, Infrastructure, WebApi).
  - This keeps Program.cs focused and makes testing easier.

Why this structure:
- Testability: handlers and services can be unit-tested without HTTP/persistence.
- Maintainability: small, focused classes & projects make changes localized.
- Familiar patterns: MediatR + validators + DTO mapping are common patterns that aid readability.

---

## Patterns, libraries and tools used

- .NET 8 / ASP.NET Core Web API — platform and framework.
- MediatR — decoupled request/handler pattern for use-cases (commands & queries).
- FluentValidation — request and domain validation.
- AutoMapper — map between DTOs / commands / results / entities.
- Entity Framework Core (Npgsql) — relational persistence with migrations.
- Serilog — structured logging.
- xUnit — unit, integration and functional tests.
- Docker + Docker Compose — run PostgreSQL, MongoDB, Redis as dependencies.
- Swagger (Swashbuckle) — interactive API documentation and testing (JWT Authorize integrated).
- BCrypt — password hashing (via IPasswordHasher implementation).
- JWT — token-based authentication (JwtTokenGenerator).

---

## Key implementation details & rationale

- Authentication
  - JWT tokens issued by JwtTokenGenerator. Expiry and signing configured via appsettings or environment.
  - Passwords hashed with BCrypt before storing.
  - Swagger configured with a Bearer security definition so the UI can perform authorized calls.

- Validation
  - FluentValidation used at both request DTO level and domain/application layers.
  - Duplicate email during user creation is surfaced as a validation error (400) with field-level details.

- Database migrations
  - EF Core migrations are applied automatically on startup by default for convenience.
  - To skip automatic migrations (for debugging), set environment variable `SKIP_MIGRATIONS=true`.

- Tests
  - Unit tests cover domain and application logic.
  - Integration tests use WebApplicationFactory with an InMemory provider when appropriate.
  - Functional tests exercise end-to-end flows; they may require Docker dependencies.

- Sales business rules (implemented)
  - Quantity-based discounts applied at the application layer (e.g., 4+ units => 10% off) and enforced during sale creation.

- Separation of concerns
  - Controllers validate requests and forward commands to MediatR handlers.
  - Mapping and response shaping are handled by AutoMapper profiles.
  - Domain layer remains independent from infrastructure concerns.

---

## Security considerations

- Password hashing with BCrypt to avoid storing plain passwords.
- Tokens are signed using a configured secret (check appsettings). Use environment variables or a secret manager in production.
- Input validation is enforced to avoid invalid domain states.
- HTTPS recommended for production; use `dotnet dev-certs https --trust` for local development.

---

## Observability & logging

- Serilog integration for structured logs.
- Domain events are published to a LoggerEventPublisher (replaceable with a real bus).
- Health checks registered at `/health`.

---

## Developer experience

- `backend/docker-compose.deps.yml` provides required database/cache services to run locally while keeping the API running natively with `dotnet run`.
- The project includes a verification script used during development; it is not tracked in the repository.
- Clear run instructions are provided for both Visual Studio and CLI.

---

## Testing & verification

- Run all tests:
  - cd backend
  - dotnet test
- To reproduce a verification flow:
  - Start Docker dependencies
  - Start the API (`dotnet run`)
  - Use Swagger or the provided curl examples to create a user, authenticate and exercise protected endpoints.
- A summary of an automated verification run is available in `docs/EVIDENCE.md`.

---

## Deployment notes

- For production, configure a secure secret key provider for JWT signing (e.g. vault / environment variables).
- Prefer managed databases and caches for production deployments; use orchestration (Kubernetes) when necessary.
- Consider token rotation and refresh mechanisms for long-lived sessions.

---

## Contributing / Code structure

- Projects:
  - backend/src/Ambev.DeveloperEvaluation.Domain
  - backend/src/Ambev.DeveloperEvaluation.Application
  - backend/src/Ambev.DeveloperEvaluation.ORM
  - backend/src/Ambev.DeveloperEvaluation.WebApi
  - backend/src/Ambev.DeveloperEvaluation.IoC
- Patterns to follow:
  - Add a Command/Handler/Validator/Result for new features in Application.
  - Add mapping profiles in WebApi/Mappings.
  - Write unit tests for handlers and integration tests for flows.
