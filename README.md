# CraftConnect Modular Monolith

Welcome to **CraftConnect** – a modern, modular monolith platform built with .NET 9 and C# 13! This solution is designed for scalability, maintainability, and developer happiness. Whether you're a contributor, tester, or just exploring, this README will guide you through the essentials.

---

## Project Structure

Below is a detailed breakdown of the main projects in the repository:

### Infrastructure.BackgroundJobs
Provides background job processing capabilities for asynchronous tasks and scheduled operations.

### Infrastructure.Cache
Implements caching mechanisms to improve performance and reduce database load.

### Core.SharedKernel
Contains shared domain kernel types, base entities, and value objects used across modules.

### Infrastructure.PDFGeneration
Handles PDF generation for reports, invoices, and other document needs.

### Infrastructure.EmailService
Manages email sending, templates, and notifications for the platform.

### Core.Logging
Provides structured logging infrastructure for traceability and debugging.

### Core.EventServices
Implements event-driven communication and integration events between modules.

### Infrastructure.Persistence
Defines the database context and persistence logic for all modules.

### Core.APIGateway
Acts as the API Gateway, centralizing routing, rate limiting, and service orchestration.

### UserManagement.Domain
Defines domain models and business logic for user management.

### UserManagement.Application
Contains application logic, CQRS handlers, DTOs, and validators for user management.

### UserManagement.Infrastructure
Implements repositories and infrastructure services for user management.

### UserManagement.Presentation
Exposes user management API controllers and endpoints.

### CraftConnect.AppHost
The main application host, responsible for bootstrapping and running the platform.

### CraftConnect.ServiceDefaults
Provides default configuration and service registration for the platform.

### BookingManagement.Domain
Defines domain models and business logic for booking management.

### BookingManagement.Application
Contains application logic, CQRS handlers, DTOs, and validators for booking management.

### BookingManagement.Infrastructure
Implements repositories and infrastructure services for booking management.

### BookingManagement.Presentation
Exposes booking management API controllers and endpoints.

### CraftConnect.Tests
Contains xUnit tests for CraftConnect modules, ensuring code quality and reliability.

### ProductInventoryManagement.Domain
Defines domain models and business logic for product inventory management.

### ProductInventoryManagement.Application
Contains application logic, CQRS handlers, DTOs, and validators for product inventory management.

---

## Build & Test
[![.NET](https://github.com/zizu28/CraftConnect/actions/workflows/dotnet.yml/badge.svg)](https://github.com/zizu28/CraftConnect/actions/workflows/dotnet.yml)
- Target Framework: **.NET 9**
- CI/CD: GitHub Actions (`.github/workflows/dotnet.yml`)
- Unit tests: `CraftConnect.Tests` (run automatically on push to `main`)

### Quickstart

```sh
# Clone the repository
git clone <repository-url>

# Restore dependencies
dotnet restore

# Build the solution
dotnet build

# Run tests
dotnet test CraftConnect.Tests/CraftConnect.Tests.csproj
```

---

## Key Concepts

- **Modular Monolith**: Each business domain (Booking, User, Product Inventory) is a separate module, following Clean Architecture and DDD principles.
- **CQRS**: Command and Query Responsibility Segregation for scalable, maintainable code.
- **API Gateway**: Centralized routing and orchestration for all modules.
- **Event-Driven**: Integration events for decoupled communication between modules.
- **Validation & Logging**: Robust input validation and structured logging for reliability.

---

## API Endpoints

- `/users/*` – User Management
- `/bookings/*` – Booking Management
- `/products/*` – Product Inventory

Explore endpoints with Swagger or Postman for a hands-on experience!

---

## Contributing

We welcome your ideas, bug reports, and pull requests!

1. Fork the repo
2. Create a feature branch (`git checkout -b feature/your-feature`)
3. Commit your changes
4. Push and open a PR

---

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

---

## Need Help?

Open an issue or contact the maintainers. Happy coding!
