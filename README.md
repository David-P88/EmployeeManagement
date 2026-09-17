# Employee Management API

A production-style **ASP.NET Core 8 Web API** for managing employees and departments, built using **Clean Architecture**, **Entity Framework Core**, **SQL Server**, JWT authentication, refresh-token rotation, role-based authorization, validation, logging, and centralized exception handling.

## 🚀 Features

* Employee CRUD operations
* Department management
* Pagination, filtering, and sorting
* DTO-based API design
* FluentValidation
* Clean Architecture
* Repository Pattern
* Entity Framework Core
* SQL Server
* JWT Bearer Authentication
* Role-based Authorization
* Password hashing
* Refresh Token authentication
* Refresh Token rotation
* Refresh Token hashing using SHA-256
* Logout and token revocation
* Global exception handling middleware
* Request logging middleware
* Swagger / OpenAPI
* Dependency Injection
* EF Core migrations
* Proper HTTP status codes
* Secure configuration using ASP.NET Core User Secrets

---

## 🏗️ Architecture

The project follows **Clean Architecture** principles.

```text
EmployeeManagement
│
├── EmployeeManagement.API
│   ├── Controllers
│   ├── Middleware
│   ├── Program.cs
│   ├── appsettings.json
│   └── appsettings.Development.json
│
├── EmployeeManagement.Application
│   ├── DTOs
│   ├── Interfaces
│   ├── Services
│   └── Validators
│
├── EmployeeManagement.Domain
│   └── Entities
│
└── EmployeeManagement.Infrastructure
    ├── Configurations
    ├── Data
    ├── Migrations
    ├── Repositories
    └── Services
```

### Layer Responsibilities

#### API

Responsible for:

* HTTP endpoints
* Controllers
* Authentication and authorization configuration
* Middleware
* Dependency injection
* Swagger configuration

#### Application

Contains the application's business logic and contracts:

* DTOs
* Services
* Interfaces
* Validators
* Business rules

#### Domain

Contains the core business entities:

* Employee
* Department
* User
* RefreshToken

The Domain layer does not depend on Infrastructure or API.

#### Infrastructure

Responsible for external concerns:

* SQL Server
* Entity Framework Core
* DbContext
* Repository implementations
* JWT token implementation
* Password services
* Database migrations

---

## 🛠️ Technology Stack

| Technology              | Usage                 |
| ----------------------- | --------------------- |
| .NET 8                  | Application framework |
| ASP.NET Core Web API    | REST API              |
| C#                      | Programming language  |
| Entity Framework Core 8 | ORM                   |
| SQL Server              | Database              |
| FluentValidation        | Request validation    |
| JWT Bearer              | Authentication        |
| Swagger / OpenAPI       | API documentation     |
| Git / GitHub            | Source control        |

---

## 🔐 Authentication

The API uses **JWT Bearer Authentication**.

### Login Flow

```text
Client
  │
  │ Username + Password
  ▼
AuthController
  │
  ▼
AuthService
  │
  ├── Validate User
  ├── Verify Password
  ├── Generate JWT
  └── Generate Refresh Token
  │
  ▼
Client receives
  ├── Access Token
  ├── Refresh Token
  └── Expiration
```

### Refresh Token Flow

Refresh tokens are:

* Generated using cryptographically secure random bytes
* Stored as SHA-256 hashes
* Rotated when used
* Revoked after rotation
* Revoked during logout
* Validated against expiration and revocation status

```text
Client
  │
  │ Refresh Token
  ▼
AuthService
  │
  ├── Hash incoming token
  ├── Find token in database
  ├── Validate expiration/revocation
  ├── Revoke old token
  ├── Generate new refresh token
  └── Generate new JWT
  │
  ▼
New Access Token + Refresh Token
```

---

## 👤 Authorization

Employee modification endpoints are protected using role-based authorization.

Example:

```text
Admin
 ├── Create Employee
 ├── Update Employee
 └── Delete Employee

Authenticated User
 └── Read Employee information
```

---

## 👨‍💼 Employee API

### Get all employees

```http
GET /api/Employees
```

### Get employee by ID

```http
GET /api/Employees/{id}
```

### Get paginated employees

```http
GET /api/Employees/paged
```

Supports:

* Page number
* Page size
* Filtering
* Sorting

### Create employee

```http
POST /api/Employees
```

Requires:

```text
Authorization: Bearer <JWT>
```

Admin authorization is required.

### Update employee

```http
PUT /api/Employees/{id}
```

Admin authorization is required.

### Delete employee

```http
DELETE /api/Employees/{id}
```

Admin authorization is required.

---

## 🔑 Authentication API

### Login

```http
POST /api/Auth/login
```

Example request:

```json
{
  "username": "admin",
  "password": "your-password"
}
```

Response contains:

```json
{
  "token": "JWT_ACCESS_TOKEN",
  "refreshToken": "REFRESH_TOKEN",
  "expiresAt": "2026-01-01T12:00:00Z",
  "username": "admin",
  "role": "Admin"
}
```

### Refresh Token

```http
POST /api/Auth/refresh
```

Example:

```json
{
  "refreshToken": "REFRESH_TOKEN"
}
```

### Logout

```http
POST /api/Auth/logout
```

Example:

```json
{
  "refreshToken": "REFRESH_TOKEN"
}
```

---

## 🗄️ Database

The application uses **SQL Server** with Entity Framework Core.

Main entities:

```text
User
 │
 └── RefreshToken

Department
 │
 └── Employee
```

### Entity Relationship

```text
Department
    │
    │ 1
    │
    │ *
Employee


User
    │
    │ 1
    │
    │ *
RefreshToken
```

---

## 📦 Entity Framework Core Migrations

The project includes EF Core migrations for database creation and schema updates.

Current migrations include:

```text
InitialCreate
AddRefreshToken
```

To apply migrations:

```powershell
dotnet ef database update \
  --project EmployeeManagement.Infrastructure \
  --startup-project EmployeeManagement.API
```

---

## ⚙️ Configuration

The application uses `appsettings.json` for non-sensitive configuration.

Example:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=EmployeeManagementDb;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "Jwt": {
    "Key": "",
    "Issuer": "EmployeeManagement.API",
    "Audience": "EmployeeManagement.Client",
    "ExpiresInMinutes": 60,
    "RefreshTokenExpiresInDays": 7
  }
}
```

### JWT Secret

The actual JWT signing key should **not** be committed to source control.

For local development, use ASP.NET Core User Secrets:

```powershell
dotnet user-secrets set "Jwt:Key" "YOUR_DEVELOPMENT_SECRET"
```

The repository intentionally keeps the JWT key empty in `appsettings.json`.

---

## ▶️ Running the Project

### Prerequisites

Install:

* .NET 8 SDK
* SQL Server
* Visual Studio 2022 or VS Code
* Git

### 1. Clone the repository

```bash
git clone https://github.com/YOUR-USERNAME/EmployeeManagement.git
```

### 2. Navigate to the project

```bash
cd EmployeeManagement
```

### 3. Configure the JWT secret

```powershell
dotnet user-secrets set "Jwt:Key" "YOUR_DEVELOPMENT_SECRET" --project EmployeeManagement.API
```

### 4. Restore packages

```bash
dotnet restore
```

### 5. Build

```bash
dotnet build
```

### 6. Apply database migrations

```powershell
dotnet ef database update --project EmployeeManagement.Infrastructure --startup-project EmployeeManagement.API
```

### 7. Run the API

```bash
dotnet run --project EmployeeManagement.API
```

---

## 📖 Swagger

After starting the application, open the Swagger URL displayed by ASP.NET Core.

Swagger provides interactive documentation for:

* Authentication
* Employee APIs
* Pagination
* Employee CRUD
* Refresh tokens
* Logout

For protected endpoints:

1. Login using `/api/Auth/login`
2. Copy the JWT access token
3. Click **Authorize** in Swagger
4. Enter:

```text
Bearer YOUR_JWT_TOKEN
```

5. Execute the protected endpoints.

---

## 🧱 Design Patterns and Principles

The project demonstrates several commonly used enterprise development practices:

* Clean Architecture
* Repository Pattern
* Dependency Injection
* DTO Pattern
* Middleware Pipeline
* Service Layer
* Separation of Concerns
* SOLID principles
* Async/Await
* Entity Framework Core
* JWT Authentication
* Role-Based Authorization
* Secure Token Storage
* Centralized Exception Handling
* Input Validation

---

## 🛡️ Security Considerations

The project follows several security practices:

* JWT signing key is stored outside source control
* Passwords are stored as hashes
* Refresh tokens are stored as SHA-256 hashes
* Refresh tokens are rotated after use
* Revoked refresh tokens cannot be reused
* Authorization is enforced on protected endpoints
* Sensitive development secrets use User Secrets
* Database credentials are not committed

---

## 📂 Project Structure

```text
EmployeeManagement
│
├── .github
│   └── copilot-instructions.md
│
├── EmployeeManagement.API
│   ├── Controllers
│   │   ├── AuthController.cs
│   │   └── EmployeesController.cs
│   ├── Middleware
│   │   ├── ExceptionHandlingMiddleware.cs
│   │   └── RequestLoggingMiddleware.cs
│   ├── Properties
│   ├── Program.cs
│   ├── appsettings.json
│   └── appsettings.Development.json
│
├── EmployeeManagement.Application
│   ├── DTOs
│   ├── Interfaces
│   ├── Services
│   └── Validators
│
├── EmployeeManagement.Domain
│   └── Entities
│
├── EmployeeManagement.Infrastructure
│   ├── Configurations
│   ├── Data
│   ├── Migrations
│   ├── Repositories
│   └── Services
│
├── .gitignore
├── EmployeeManagement.sln
└── README.md
```

---

## 🎯 Learning Objectives

This project was developed to demonstrate practical experience with:

* ASP.NET Core Web API
* Modern C# development
* Clean Architecture
* REST API development
* Entity Framework Core
* SQL Server
* Authentication and authorization
* Secure refresh-token implementation
* Repository Pattern
* Dependency Injection
* Middleware
* API validation
* Database migrations
* Git and GitHub

---

## 🚀 Future Enhancements

Potential future improvements include:

* Automated unit and integration tests
* Docker containerization
* CI/CD pipeline
* Azure deployment
* Health checks
* API versioning
* Structured logging and observability
* Redis caching
* Rate limiting
* Automated API documentation
* Additional domain modules

---

## 👨‍💻 Author

**David P**

Senior Software Engineer
.NET | ASP.NET Core | C# | REST APIs | SQL Server | Clean Architecture | Microservices | Azure
