# CraftConnect Modular Monolith

Welcome to **CraftConnect** – a modern, modular monolith platform built with .NET 9 and C# 13! This solution connects craftspeople with customers through a comprehensive booking and product management system. The platform follows Clean Architecture principles and Domain-Driven Design patterns for scalability, maintainability, and developer happiness.

---

## Project Overview

CraftConnect is a sophisticated platform that enables:
- **Craftspeople Management**: Registration, verification, and profile management for service providers
- **Customer Management**: Customer registration, preferences, and booking history
- **Booking System**: End-to-end booking lifecycle from creation to completion
- **Product Inventory**: Product catalog with inventory management for craftspeople
- **Integration Events**: Event-driven communication between modules using MassTransit

---

## Solution Architecture

The repository follows a **modular monolith** architecture with clear separation of concerns:

### Core Infrastructure Projects
- **Core.SharedKernel**: Shared domain abstractions, base entities, value objects, and common patterns
- **Core.Logging**: Centralized structured logging infrastructure for traceability and debugging
- **Core.EventServices**: Event handling abstractions and integration event infrastructure
- **Core.APIGateway**: API gateway for routing, rate limiting, and service orchestration
- **Infrastructure.Persistence**: Centralized data persistence with Entity Framework Core and ApplicationDbContext
- **Infrastructure.Cache**: Distributed caching implementation for performance optimization
- **Infrastructure.BackgroundJobs**: Background job processing for asynchronous and scheduled operations
- **Infrastructure.EmailService**: Email notification services with template support
- **Infrastructure.PDFGeneration**: PDF document generation for reports and invoices
- **CraftConnect.AppHost**: Main application host and bootstrapping entry point
- **CraftConnect.ServiceDefaults**: Default service configurations and dependency injection setup

### Business Domain Modules

#### User Management Module
- **UserManagement.Domain**: Domain entities (User, Customer, Craftman), value objects, and business rules
- **UserManagement.Application**: CQRS handlers, DTOs, validators, and application services
- **UserManagement.Infrastructure**: Repository implementations and infrastructure services
- **UserManagement.Presentation**: API controllers and endpoints for user operations

#### Booking Management Module
- **BookingManagement.Domain**: Booking lifecycle, line items, and business logic
- **BookingManagement.Application**: Booking CQRS operations and workflow management
- **BookingManagement.Infrastructure**: Booking data persistence and external service integrations
- **BookingManagement.Presentation**: Booking API endpoints and controllers

#### Product Inventory Management Module
- **ProductInventoryManagement.Domain**: Product entities, categories, inventory management
- **ProductInventoryManagement.Application**: Product CQRS operations, DTOs, and business logic
- **ProductInventoryManagement.Infrastructure**: Product repository implementations and data access
- **ProductInventoryManagement.Presentation**: Product and category API controllers

### Testing
- **CraftConnect.Tests**: Comprehensive unit and integration tests using xUnit and Moq

---

## Database Schema

The `ApplicationDbContext` manages the following entities:

### Core Entities
- **Users**: Base user entity with authentication and profile information
- **Customers**: Customer-specific data extending User (addresses, payment preferences)
- **Craftmen**: Craftspeople with skills, hourly rates, and verification status
- **RefreshTokens**: JWT refresh token management for authentication

### Booking Entities
- **Bookings**: Booking lifecycle management with status tracking
- **BookingLineItems**: Individual service items within a booking

### Product Entities
- **Products**: Product catalog with inventory management
- **Categories**: Hierarchical product categorization system

### Value Objects (Owned Types)
- **Money**: Currency and amount handling
- **Email**: Email validation and formatting
- **PhoneNumber**: Phone number validation
- **UserAddress**: Address information for users
- **GeoLocation**: Geographic coordinates
- **Skill**: Craftsperson skills with experience levels
- **Address**: Service location addresses
- **JobDetails**: Booking job specifications
- **Image**: Product and profile image management
- **Inventory**: Stock quantity tracking

### MassTransit Integration
- **Inbox/Outbox Pattern**: Message reliability with state entities
- **Event Sourcing**: Domain event handling and integration events

---

## Technology Stack

### Framework & Language
- **.NET 9**: Latest .NET framework with performance improvements and new features
- **C# 13**: Modern C# with collection expressions, primary constructors, and enhanced pattern matching
- **Entity Framework Core**: ORM for database operations with Code First approach
- **ASP.NET Core**: Web API framework with minimal APIs and controllers

### Architecture Patterns
- **Clean Architecture**: Separation of concerns with clear layer boundaries
- **Domain-Driven Design (DDD)**: Rich domain models and aggregate roots
- **CQRS with MediatR**: Command and Query separation for scalable operations
- **Repository Pattern**: Abstracted data access layer
- **Event-Driven Architecture**: Decoupled communication via domain and integration events

### Messaging & Communication
- **MassTransit**: Message bus for reliable message handling and event processing
- **Integration Events**: Cross-module communication (e.g., BookingRequestedIntegrationEvent)
- **Inbox/Outbox Pattern**: Message reliability and transactional messaging

### Data & Persistence
- **SQL Server**: Primary database (configurable via connection strings)
- **Entity Type Configurations**: Modular entity configuration per domain
- **Value Objects**: Owned entity types for complex value objects
- **Optimistic Concurrency**: RowVersion/Timestamp for conflict resolution

### Testing Framework
- **xUnit**: Unit testing framework with comprehensive test coverage
- **Moq**: Mocking framework for dependency isolation
- **Arrange-Act-Assert Pattern**: Consistent test structure
- **Integration Tests**: End-to-end testing capabilities

### DevOps & Quality
- **GitHub Actions**: CI/CD pipeline for automated builds and tests
- **Swagger/OpenAPI**: API documentation and testing interface
- **Structured Logging**: Comprehensive logging with Core.Logging
- **Background Jobs**: Asynchronous processing capabilities

---

## API Endpoints

The platform exposes RESTful APIs organized by domain modules with comprehensive CRUD operations:

### User Management APIs
```
GET    /api/users              - Get all users with filtering and pagination
GET    /api/users/{id}         - Get user by ID with full profile details
POST   /api/users              - Register new user (customer or craftsperson)
PUT    /api/users/{id}         - Update user profile information
DELETE /api/users/{id}         - Soft delete user account
GET    /api/users/craftsmen    - Get all verified craftspeople
GET    /api/users/customers    - Get all active customers
POST   /api/users/verify       - Verify craftsperson credentials
```

### Booking Management APIs
```
GET    /api/bookings                    - Get bookings with filters (status, date range)
GET    /api/bookings/{id}               - Get booking details with line items
POST   /api/bookings                    - Create new booking request
PUT    /api/bookings/{id}               - Update booking details
DELETE /api/bookings/{id}               - Cancel booking
POST   /api/bookings/{id}/confirm       - Confirm booking (craftsperson action)
POST   /api/bookings/{id}/complete      - Mark booking as completed
GET    /api/bookings/customer/{id}      - Get customer's booking history
GET    /api/bookings/craftsman/{id}     - Get craftsperson's bookings
```

### Product Inventory Management APIs
```
GET    /api/products                    - Get products with filtering and search
GET    /api/products/{id}               - Get product details with inventory
POST   /api/products                    - Create new product (craftsperson only)
PUT    /api/products/{id}               - Update product information
DELETE /api/products/{id}               - Delete product
POST   /api/products/{id}/images        - Upload product images
DELETE /api/products/{id}/images/{imageId} - Remove product image
PUT    /api/products/{id}/inventory     - Update product inventory
POST   /api/products/{id}/reserve       - Reserve product quantity
POST   /api/products/{id}/release       - Release reserved quantity

GET    /api/categories                  - Get all categories (hierarchical)
GET    /api/categories/{id}             - Get category by ID
POST   /api/categories                  - Create new category
PUT    /api/categories/{id}             - Update category details
DELETE /api/categories/{id}             - Delete category
GET    /api/categories/{id}/products    - Get products in category
```

### Integration Events (Internal)
```
BookingRequestedIntegrationEvent     - Triggers user notification workflow
BookingConfirmedIntegrationEvent     - Updates inventory and schedules
BookingCompletedIntegrationEvent     - Processes payments and reviews
ProductOutOfStockIntegrationEvent    - Notifies inventory management
UserVerifiedIntegrationEvent         - Enables craftsperson features
```

### API Features
- **Authentication**: JWT-based with refresh token support
- **Authorization**: Role-based access control (Customer, Craftsperson, Admin)
- **Validation**: Comprehensive input validation with detailed error responses
- **Pagination**: Cursor-based pagination for large datasets
- **Filtering**: Advanced filtering and search capabilities
- **Rate Limiting**: API throttling to prevent abuse
- **Swagger Documentation**: Interactive API documentation at `/swagger`

---

## Getting Started

### Prerequisites
- **.NET 9 SDK** - [Download here](https://dotnet.microsoft.com/download/dotnet/9.0)
- **SQL Server** (Local DB, Express, or full version)
- **Visual Studio 2022** or **VS Code** with C# extensions
- **Git** for version control

### Quick Start Guide

1. **Clone the Repository**
   ```bash
   git clone https://github.com/zizu28/CraftConnect.git
   cd CraftConnect
   ```

2. **Restore Dependencies**
   ```bash
   dotnet restore
   ```

3. **Update Database Connection**
   - Configure your SQL Server connection string in `appsettings.json`
   - Ensure SQL Server is running and accessible

4. **Run Database Migrations**
   ```bash
   dotnet ef database update --project Infrastructure.Persistence
   ```

5. **Build the Solution**
   ```bash
   dotnet build
   ```

6. **Run the Application**
   ```bash
   dotnet run --project CraftConnect.AppHost
   ```

7. **Access the Application**
   - API: `https://localhost:5001`
   - Swagger UI: `https://localhost:5001/swagger`

### Development Workflow

```bash
# Create a new feature branch
git checkout -b feature/your-feature-name

# Make your changes and test
dotnet test

# Build and verify no errors
dotnet build

# Commit your changes
git add .
git commit -m "feat: add your feature description"

# Push to remote
git push origin feature/your-feature-name

# Create pull request on GitHub
```

---

## Testing Strategy

### Unit Testing Framework
- **xUnit**: Primary testing framework for all test scenarios
- **Moq**: Dependency mocking for isolated unit tests
- **FluentAssertions**: Readable and comprehensive assertions
- **Test Coverage**: Comprehensive coverage across all layers

### Test Organization
```
CraftConnect.Tests/
├── Controllers/           # API controller tests
│   ├── CategoriesControllerTests.cs
│   ├── UsersControllerTests.cs
│   └── BookingsControllerTests.cs
├── Domain/               # Domain logic tests
├── Application/          # Application service tests
├── Infrastructure/       # Repository and infrastructure tests
└── Integration/          # End-to-end integration tests
```

### Test Patterns and Examples

**Controller Testing Pattern** (from `CategoriesControllerTests`):
```csharp
[Fact]
public async Task GetCategoryByIdAsync_ReturnsOkResult_WhenCategoryExists()
{
    // Arrange - Setup test data and mock dependencies
    var categoryId = Guid.NewGuid();
    var categoryResponse = new CategoryResponseDTO { CategoryId = categoryId };
    _mediatorMock.Setup(m => m.Send(It.Is<GetCategoryByIdQuery>(q => q.Id == categoryId), 
                      It.IsAny<CancellationToken>()))
                 .ReturnsAsync(categoryResponse);

    // Act - Execute the method under test
    var result = await _controller.GetCategoryByIdAsync(categoryId, CancellationToken.None);

    // Assert - Verify expected outcomes
    var okResult = Assert.IsType<OkObjectResult>(result);
    var returnValue = Assert.IsType<CategoryResponseDTO>(okResult.Value);
    Assert.Equal(categoryId, returnValue.CategoryId);
}
```

### Test Scenarios Covered
- **Happy Path Scenarios**: Successful CRUD operations, valid data processing
- **Error Handling**: Not found scenarios, validation failures, business rule violations
- **Edge Cases**: Boundary conditions, null inputs, empty collections
- **Concurrency**: Optimistic concurrency control with RowVersion
- **Integration Events**: Event publishing and consumption workflows
- **Authentication**: JWT token validation and role-based authorization

### Running Tests
```bash
# Run all tests
dotnet test

# Run tests with detailed output
dotnet test --verbosity normal

# Run tests with coverage collection
dotnet test --collect:"XPlat Code Coverage"

# Run specific test class
dotnet test --filter "CategoriesControllerTests"

# Run tests matching pattern
dotnet test --filter "Category"

# Generate coverage report (requires coverlet)
dotnet test --collect:"XPlat Code Coverage" --results-directory ./TestResults
```

---

## Project Structure Deep Dive

### Clean Architecture Layers

```
src/
├── Core/                           # Shared kernel and cross-cutting concerns
│   ├── Core.SharedKernel/         # Domain base classes, value objects
│   ├── Core.Logging/              # Structured logging abstraction
│   ├── Core.EventServices/        # Event handling infrastructure
│   └── Core.APIGateway/           # API gateway and routing
│
├── Infrastructure/                 # External concerns and cross-cutting infrastructure
│   ├── Infrastructure.Persistence/      # Database context and configurations
│   ├── Infrastructure.Cache/           # Distributed caching implementation
│   ├── Infrastructure.BackgroundJobs/  # Hangfire/Quartz job processing
│   ├── Infrastructure.EmailService/    # SMTP and email templating
│   └── Infrastructure.PDFGeneration/   # PDF creation and manipulation
│
├── Modules/                        # Business domain modules (bounded contexts)
│   ├── UserManagement/
│   │   ├── UserManagement.Domain/        # Entities, value objects, domain services
│   │   ├── UserManagement.Application/   # CQRS, DTOs, validators, app services
│   │   ├── UserManagement.Infrastructure/ # Repositories, external service adapters
│   │   └── UserManagement.Presentation/  # Controllers, SignalR hubs, API contracts
│   │
│   ├── BookingManagement/          # Booking lifecycle and workflow management
│   └── ProductInventoryManagement/ # Product catalog and inventory control
│
├── Host/                          # Application composition root
│   ├── CraftConnect.AppHost/      # Main application startup and configuration
│   └── CraftConnect.ServiceDefaults/ # Shared service registrations
│
└── Tests/
    └── CraftConnect.Tests/        # Unit, integration, and end-to-end tests
```

### Domain Model Relationships

```
User (Abstract Base)
├── Customer
│   ├── UserAddress (Value Object)
│   └── PaymentMethod (Enum)
└── Craftman
    ├── Skills[] (Value Object Collection)
    ├── HourlyRate (Money Value Object)
    └── VerificationStatus (Enum)

Booking (Aggregate Root)
├── BookingLineItems[]
├── Address (Value Object)
├── JobDetails (Value Object)
└── BookingStatus (Enum)

Product (Aggregate Root)
├── Category (Reference)
├── Inventory (Value Object)
├── Images[] (Value Object Collection)
└── Craftman (Reference)
```

---

## Configuration and Deployment

### Environment Configuration
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=CraftConnectDb;Trusted_Connection=true"
  },
  "JwtSettings": {
    "Secret": "your-secret-key",
    "ExpiryInHours": 24,
    "RefreshTokenExpiryInDays": 30
  },
  "MassTransit": {
    "ConnectionString": "your-message-broker-connection"
  },
  "EmailSettings": {
    "SmtpServer": "smtp.your-provider.com",
    "SmtpPort": 587,
    "EnableSsl": true
  }
}
```

### Docker Support
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["CraftConnect.AppHost/CraftConnect.AppHost.csproj", "CraftConnect.AppHost/"]
RUN dotnet restore "CraftConnect.AppHost/CraftConnect.AppHost.csproj"
COPY . .
WORKDIR "/src/CraftConnect.AppHost"
RUN dotnet build "CraftConnect.AppHost.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CraftConnect.AppHost.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CraftConnect.AppHost.dll"]
```

---

## Contributing

We welcome contributions from the community! Please follow these guidelines:

### Development Guidelines
1. **Fork the repository** and create a feature branch
2. **Follow C# coding conventions** and maintain consistency
3. **Write unit tests** for new functionality
4. **Update documentation** for API changes
5. **Ensure all tests pass** before submitting PR

### Code Quality Standards
- **Clean Code**: Follow SOLID principles and clean code practices
- **Domain-Driven Design**: Maintain rich domain models and business logic in domain layer
- **CQRS Pattern**: Separate commands and queries appropriately
- **Error Handling**: Implement comprehensive error handling and logging
- **Performance**: Consider performance implications and optimization opportunities

### Pull Request Process
1. Ensure your branch is up to date with main
2. Run all tests and ensure they pass
3. Update README.md if needed for new features
4. Submit PR with clear description and test evidence
5. Address feedback promptly during code review

---

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

---

## Support and Documentation

- **GitHub Issues**: Report bugs and request features
- **GitHub Discussions**: Community discussions and Q&A
- **API Documentation**: Available at `/swagger` when running the application
- **Architecture Documentation**: See `/docs` folder for detailed design documents

### Contact Information
- **Repository**: [https://github.com/zizu28/CraftConnect](https://github.com/zizu28/CraftConnect)
- **Maintainer**: [@zizu28](https://github.com/zizu28)

---

**Happy Coding with CraftConnect! 🚀**
