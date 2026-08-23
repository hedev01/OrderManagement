# Order Management Service

A backend API for managing customers, products, inventory, and orders for an online store.

The project is implemented using **ASP.NET Core**, **Entity Framework Core**, **SQL Server**, **JWT Authentication**, **Serilog**, **Swagger**, and **xUnit + Moq**, following **Clean Architecture** principles.

---

## 📌 Overview

The main purpose of this project is to provide a reliable and maintainable API for managing the order lifecycle of an online store.

The service supports:

* User management
* JWT authentication
* Role-based authorization
* Customer management
* Product management
* Inventory management
* Order creation
* Order retrieval
* Order filtering and pagination
* Order status management
* Order deletion
* Bulk order creation
* Inventory validation
* Database migrations
* Seed data
* Structured logging with Serilog
* Unit testing
* Swagger API documentation

---

# 🚀 Quick Setup

Follow these steps to run the project locally.

## 1. Clone the Repository

```bash
git clone https://github.com/hedev01/OrderManagement.git
cd OrderManagement
```

---

## 2. Configure the Database

Update the connection string in:

```text
OrderManagement.API/appsettings.json
```

Example:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=OrderManagementDb;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

If you are using SQL Server authentication:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=OrderManagementDb;User Id=YOUR_USERNAME;Password=YOUR_PASSWORD;TrustServerCertificate=True;"
  }
}
```

> Do not commit real database credentials or production secrets to Git.

---

## 3. Configure JWT

Configure the JWT settings in:

```text
OrderManagement.API/appsettings.json
```

Example:

```json
{
  "Jwt": {
    "Key": "YOUR_DEVELOPMENT_SECRET_KEY",
    "Issuer": "OrderManagement",
    "Audience": "OrderManagementClient",
    "ExpirationMinutes": 60
  }
}
```

For production environments, sensitive values should be stored using environment variables, User Secrets, or another secure configuration provider.

---

## 4. Apply Database Migrations

From the solution directory:

```bash
dotnet ef database update
```

If EF Core cannot find the correct startup project, use:

```bash
dotnet ef database update \
    --project OrderManagement.Infrastructure \
    --startup-project OrderManagement.API
```

---

## 5. Run the Application

```bash
dotnet run --project OrderManagement.API
```

Or run the project directly from Visual Studio.

---

## 6. Open Swagger

After starting the API, open:

```text
https://localhost:<PORT>/swagger
```

Swagger provides interactive documentation for all available API endpoints.

---

## 7. Get a JWT Token

Use the authentication endpoint:

```text
POST /api/auth/login
```

Example:

```json
{
  "username": "admin",
  "password": "YOUR_PASSWORD"
}
```

Copy the returned `accessToken`.

In Swagger:

```text
Authorize
    ↓
Bearer YOUR_ACCESS_TOKEN
    ↓
Authorize
```

You can now call protected endpoints.

---

# 🏗️ Architecture

The project follows **Clean Architecture** with four main layers:

```text
OrderManagement
│
├── OrderManagement.Domain
├── OrderManagement.Application
├── OrderManagement.Infrastructure
└── OrderManagement.API
```

---

## Domain

Contains the core business model and rules.

Responsibilities:

* Entities
* Value Objects
* Enums
* Domain behavior
* Business rules

Examples:

```text
User
Customer
Product
Inventory
Order
OrderItem
```

The Domain layer does not depend on Infrastructure or API.

---

## Application

Contains application business logic and use cases.

Responsibilities:

* Use Cases
* DTOs
* Requests
* Responses
* Repository abstractions
* Unit of Work abstractions
* Application interfaces

Examples:

```text
CreateOrderUseCase
GetOrderUseCase
GetOrdersUseCase
ChangeOrderStatusUseCase
DeleteOrderUseCase
BulkCreateOrdersUseCase
```

The Application layer depends on the Domain layer.

---

## Infrastructure

Contains implementations of external concerns.

Responsibilities:

* Entity Framework Core
* SQL Server
* Repository implementations
* Unit of Work
* Database configuration
* JWT implementation
* Password hashing
* Seed Data
* Logging configuration

The Infrastructure layer implements abstractions defined by the Application layer.

---

## API

Contains the HTTP API layer.

Responsibilities:

* Controllers
* Authentication
* Authorization
* Dependency Injection
* Swagger
* HTTP configuration
* Application startup

---

# 🔐 Authentication & Authorization

The API uses **JWT Bearer Authentication**.

Two main roles are supported:

```text
Admin
User
```

Protected endpoints require a valid JWT token.

Role-based authorization is used for operations that require elevated permissions.

For example:

```text
DELETE /api/orders/{id}
```

is restricted to users with the `Admin` role.

---

# 👤 User Management

The User module handles authentication and application users.

Main capabilities:

* Create user
* Login
* Password hashing
* JWT token generation
* Role management

Passwords are never stored as plain text.

---

# 👥 Customer Management

Customers represent the business customers who place orders.

Supported operations include:

```text
GET /customers
GET /customers/{id}
POST /customers
```

Customer information is separated from the authentication `User` entity.

This separation allows authentication-related data and business customer data to evolve independently.

---

# 📦 Product & Inventory

Products represent items that can be ordered.

Inventory is modeled separately from Product to keep stock-related information isolated.

The system validates inventory before confirming an order.

Example:

```text
Available Inventory: 10
Requested Quantity: 15

Result:
Order cannot be confirmed.
```

---

# 🛒 Order Management

The Order module is the core part of the application.

## Create Order

```text
POST /api/orders
```

An order contains one or more order items.

Example:

```json
{
  "customerId": "customer-id",
  "items": [
    {
      "productId": "product-id",
      "quantity": 2
    }
  ]
}
```

The system validates:

* Customer existence
* Product existence
* Quantity
* Inventory availability

---

## Get Order

```text
GET /api/orders/{id}
```

Returns detailed information about an order and its items.

---

## Search & Filter Orders

Orders can be filtered by:

* Customer ID
* Order status
* From date
* To date

Pagination is supported for order search results.

Example:

```text
GET /api/orders?page=1&pageSize=20
```

---

# 🔄 Order Status

Order status follows a predefined lifecycle:

```text
Pending
   ↓
Confirmed
   ↓
Shipped
   ↓
Delivered
```

Invalid status transitions are rejected.

For example:

```text
Pending → Confirmed     ✓
Confirmed → Shipped     ✓
Shipped → Delivered     ✓

Pending → Shipped       ✗
Pending → Delivered     ✗
Delivered → Pending     ✗
```

Inventory is checked when an order is confirmed.

---

# 🗑️ Delete Order

Orders can only be deleted by users with the `Admin` role.

```text
DELETE /api/orders/{id}
```

Authorization is enforced using JWT role-based authorization.

---

# ⚡ Bulk Order Creation

The API supports creating multiple orders in a single request.

```text
POST /api/orders/bulk
```

A maximum of **1000 orders** is allowed per request.

The implementation validates:

* Customer existence
* Product existence
* Quantity
* Total requested inventory
* Order items

Inventory is validated across the complete bulk request to prevent creating orders that collectively exceed the available stock.

---

# 🗄️ Database

The project uses:

* SQL Server
* Entity Framework Core
* Code First
* EF Core Migrations

Main entities include:

```text
Users
Customers
Products
Inventories
Orders
OrderItems
```

---

# 🌱 Seed Data

The project includes seed data for development/testing.

Seeded data includes:

```text
50 Customers
200 Products
Inventory records
Test Users
```

Database seeding is executed during application startup through:

```csharp
await app.SeedDatabaseAsync();
```

---

# 🔄 Database Migrations

Create a migration:

```bash
dotnet ef migrations add MigrationName \
    --project OrderManagement.Infrastructure \
    --startup-project OrderManagement.API
```

Apply migrations:

```bash
dotnet ef database update \
    --project OrderManagement.Infrastructure \
    --startup-project OrderManagement.API
```

---

# 📝 Logging

The project uses **Serilog** for application logging.

Logging is implemented across application use cases to provide information about important operations such as:

* Order creation
* Order retrieval
* Order deletion
* Status changes
* Bulk operations
* Customer operations
* Authentication operations

Example:

```text
[INF] Creating customer.
[INF] Customer created successfully.
[INF] Getting order.
[WRN] Order was not found.
```

Sensitive information such as passwords and JWT tokens should never be written to logs.

---

# 🧪 Testing

Unit tests are implemented using:

* xUnit
* Moq

The tests focus primarily on application business logic and Use Cases.

Run all tests:

```bash
dotnet test
```

Run only the test project:

```bash
dotnet test OrderManagement.Tests
```

Example test scenarios include:

```text
CreateOrderUseCase
├── Empty items
├── Invalid quantity
├── Customer not found
├── Product not found
├── Inventory not found
├── Insufficient inventory
└── Successful order creation
```

---

# 📚 API Documentation

Swagger is enabled for interactive API documentation.

After running the application:

```text
https://localhost:<PORT>/swagger
```

Swagger can be used to:

* Explore endpoints
* View request/response models
* Authenticate using JWT
* Execute API requests
* Inspect HTTP responses

---

# 📁 Project Structure

```text
OrderManagement
│
├── OrderManagement.Domain
│   ├── Entities
│   ├── Enums
│   └── ...
│
├── OrderManagement.Application
│   ├── Features
│   │   ├── Users
│   │   ├── Customers
│   │   └── Orders
│   ├── Interfaces
│   └── ...
│
├── OrderManagement.Infrastructure
│   ├── Persistence
│   │   ├── Configurations
│   │   ├── Migrations
│   │   └── ApplicationDbContext
│   ├── Repositories
│   ├── Services
│   └── Seed
│
├── OrderManagement.API
│   ├── Controllers
│   ├── Extensions
│   ├── Middleware
│   └── Program.cs
│
└── OrderManagement.Tests
    └── Application
        └── Orders
```

---

# 🧠 Design Decisions

## Clean Architecture

The project separates business logic from infrastructure and HTTP concerns.

This makes the application:

* Testable
* Maintainable
* Loosely coupled
* Easier to extend

---

## Repository Pattern

Repositories abstract database access from the Application layer.

Use Cases work with interfaces rather than directly depending on Entity Framework Core.

---

## Unit of Work

Database changes that belong to a single business operation are coordinated through the Unit of Work.

Transactions are used for operations that require atomic behavior.

---

## AsNoTracking

Read-only queries use `AsNoTracking` where appropriate to reduce Entity Framework Core tracking overhead.

---

## Transactions

Transactions are used where multiple database operations must succeed or fail together.

For example:

```text
Create Order
    ↓
Save Changes
    ↓
Commit Transaction
```

If an unexpected error occurs:

```text
Rollback Transaction
```

---

## Business Logic in Domain/Application

Order status transitions and inventory-related rules are enforced in the application/domain logic rather than being handled only at the controller level.

This prevents business rules from being bypassed by other entry points.

---

# 🛡️ Security

The application uses:

* JWT Bearer Authentication
* Role-based Authorization
* Password Hashing
* Configuration-based JWT settings

Production secrets should not be stored directly in source control.

For local development, use:

* User Secrets
* Environment Variables
* Local configuration

---

# ⚙️ Requirements

Before running the project, make sure you have:

* .NET SDK 7 or later
* SQL Server
* Entity Framework Core CLI
* Git

Install EF Core CLI if necessary:

```bash
dotnet tool install --global dotnet-ef
```

---

# 🏃 Running the Project

Restore dependencies:

```bash
dotnet restore
```

Build:

```bash
dotnet build
```

Apply database migrations:

```bash
dotnet ef database update \
    --project OrderManagement.Infrastructure \
    --startup-project OrderManagement.API
```

Run:

```bash
dotnet run --project OrderManagement.API
```

Run tests:

```bash
dotnet test
```

---

# ✅ Project Status

The following requirements have been implemented:

* [x] Clean Architecture
* [x] User Management
* [x] JWT Authentication
* [x] Role-based Authorization
* [x] Customer Management
* [x] Product Management
* [x] Inventory Management
* [x] Create Order
* [x] Get Order
* [x] Search and Filter Orders
* [x] Pagination
* [x] Change Order Status
* [x] Delete Order
* [x] Bulk Create Orders
* [x] Inventory Validation
* [x] EF Core Code First
* [x] Database Migrations
* [x] Seed Data
* [x] Serilog Logging
* [x] Swagger Documentation
* [x] Unit Tests

---

# 📌 Notes

This project was developed as a technical assignment for an Order Management Service.

The main focus was on clean architecture, separation of concerns, business rule enforcement, database consistency, authentication/authorization, performance considerations, logging, and testability.
