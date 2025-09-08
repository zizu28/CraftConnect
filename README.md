# CraftConnect Modular Monolith

Welcome to **CraftConnect** – a modern, modular monolith platform built with .NET 9 and C# 13! This solution is designed for scalability, maintainability, and developer happiness. Whether you're a contributor, tester, or just exploring, this README will guide you through the essentials.

---

## Project Structure

The repository is organized into multiple projects, each responsible for a distinct domain or infrastructure concern:

- **Core.SharedKernel**: Shared domain abstractions, base entities, and value objects used across modules.
- **Core.Logging**: Centralized logging services for traceability and debugging.
- **Core.EventServices**: Event handling and integration event abstractions for decoupled communication.
- **Core.APIGateway**: API gateway for routing, rate limiting, and service orchestration.
- **UserManagement.Domain/Application/Infrastructure/Presentation**: User and craftsperson management, including domain models, application logic, repositories, and API endpoints.
- **BookingManagement.Domain/Application/Infrastructure/Presentation**: Booking lifecycle management, including domain models, application logic, repositories, and API endpoints.
- **ProductInventoryManagement.Domain/Application**: Product and inventory management, including domain models, business logic, and application services.
- **Infrastructure.BackgroundJobs**: Background job processing for asynchronous and scheduled operations.
- **Infrastructure.Cache**: Distributed caching to improve performance and reduce database load.
- **Infrastructure.EmailService**: Email notification services and templates.
- **Infrastructure.PDFGeneration**: PDF document generation for reports and invoices.
- **Infrastructure.Persistence**: Data persistence and repository implementations.
- **CraftConnect.AppHost**: Main application host and entry point for the platform.
- **CraftConnect.ServiceDefaults**: Service configuration defaults for the platform.
- **CraftConnect.Tests**: Unit and integration tests for all modules.

---

## Build & Test
[![.NET](https://github.com/zizu28/CraftConnect/actions/workflows/dotnet.yml/badge.svg)](https://github.com/zizu28/CraftConnect/actions/workflows/dotnet.yml)

### Technologies & Framework
- **Target Framework**: .NET 9
- **Language**: C# 13 with latest language features
- **Testing Framework**: xUnit with Moq for mocking
- **Architecture**: Clean Architecture with CQRS pattern
- **Messaging**: MassTransit for event-driven communication
- **ORM**: Entity Framework Core for data persistence
- **DI Container**: Built-in .NET dependency injection

### CI/CD Pipeline
- Automated builds and tests via GitHub Actions (`.github/workflows/dotnet.yml`)
- Unit tests run automatically on push to `main` branch
- Test coverage includes controller tests (e.g., CategoriesControllerTests)

### Development Commands

```sh
# Clone the repository
git clone https://github.com/zizu28/CraftConnect.git

# Restore dependencies
dotnet restore

# Build the entire solution
dotnet build

# Run all tests
dotnet test

# Run tests for specific project
dotnet test CraftConnect.Tests/CraftConnect.Tests.csproj

# Run the application
dotnet run --project CraftConnect.AppHost/CraftConnect.AppHost.csproj
```

---

## Key Concepts

- **Modular Monolith**: Each business domain (Booking, User Management, Product Inventory) is a separate module following Clean Architecture and Domain-Driven Design (DDD) principles. Each module has its own Domain, Application, Infrastructure, and Presentation layers.
- **CQRS with MediatR**: Command and Query Responsibility Segregation pattern implemented using MediatR for scalable, maintainable code. Commands handle write operations while queries handle read operations.
- **API Gateway**: Centralized routing and orchestration for all modules using Core.APIGateway project.
- **Event-Driven Architecture**: Integration events using MassTransit for decoupled communication between modules (e.g., BookingRequestedIntegrationEventConsumer).
- **Domain Entities**: Rich domain models like Product entity with business logic, validation, and aggregate root patterns.
- **Repository Pattern**: Abstracted data access through IUserRepository and other repository interfaces.
- **Background Processing**: Asynchronous task processing using Infrastructure.BackgroundJobs.
- **Comprehensive Testing**: Unit tests using xUnit, Moq for mocking, and test coverage for controllers like CategoriesController.
- **Validation & Logging**: Robust input validation and structured logging using Core.Logging for reliability and traceability.

---

## API Endpoints

The platform exposes RESTful APIs organized by domain modules:

### User Management
- `GET /api/users` – Get all users
- `GET /api/users/{id}` – Get user by ID
- `POST /api/users` – Create new user
- `PUT /api/users/{id}` – Update user
- `DELETE /api/users/{id}` – Delete user

### Booking Management
- `GET /api/bookings` – Get all bookings
- `GET /api/bookings/{id}` – Get booking by ID
- `POST /api/bookings` – Create new booking
- `PUT /api/bookings/{id}` – Update booking
- `DELETE /api/bookings/{id}` – Cancel booking

### Product Inventory Management
- `GET /api/products` – Get all products
- `GET /api/products/{id}` – Get product by ID
- `POST /api/products` – Create new product
- `PUT /api/products/{id}` – Update product
- `DELETE /api/products/{id}` – Delete product
- `GET /api/categories` – Get all categories (tested in CategoriesControllerTests)
- `GET /api/categories/{id}` – Get category by ID
- `POST /api/categories` – Create new category
- `PUT /api/categories/{id}` – Update category
- `DELETE /api/categories/{id}` – Delete category

### Integration Events
- `BookingRequestedIntegrationEvent` – Consumed by UserManagement module
- Domain events for cross-module communication via MassTransit

Explore endpoints with Swagger UI when running the application, or use Postman for API testing!

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

---

## Testing Strategy

The project implements comprehensive testing strategies to ensure code quality and reliability:

### Unit Testing
- **Framework**: xUnit with Moq for mocking dependencies
- **Coverage**: Controller tests, domain logic, and application services
- **Example**: `CategoriesControllerTests` with scenarios for CRUD operations
- **Patterns**: Arrange-Act-Assert (AAA) pattern for clear test structure

### Test Scenarios Covered
- **Happy Path**: Successful operations (e.g., GetAllCategoriesAsync returns OK)
- **Error Handling**: Not found scenarios, validation errors, model state validation
- **Edge Cases**: ID mismatches, null responses, invalid inputs
- **Async Operations**: All tests use async/await patterns with CancellationToken

### Test Structure Example
```csharp
[Fact]
public async Task GetCategoryByIdAsync_ReturnsOkResult_WhenCategoryExists()
{
    // Arrange - Setup test data and mocks
    var categoryId = Guid.NewGuid();
    var categoryResponse = new CategoryResponseDTO { CategoryId = categoryId };
    _mediatorMock.Setup(m => m.Send(It.Is<GetCategoryByIdQuery>(q => q.Id == categoryId), It.IsAny<CancellationToken>()))
                 .ReturnsAsync(categoryResponse);

    // Act - Execute the method under test
    var result = await _controller.GetCategoryByIdAsync(categoryId, CancellationToken.None);

    // Assert - Verify the expected outcome
    var okResult = Assert.IsType<OkObjectResult>(result);
    var returnValue = Assert.IsType<CategoryResponseDTO>(okResult.Value);
    Assert.Equal(categoryId, returnValue.CategoryId);
}
```

### Running Tests
```sh
# Run all tests
dotnet test

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run specific test class
dotnet test --filter "CategoriesControllerTests"
