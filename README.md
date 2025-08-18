# CraftConnect

## Overview

CraftConnect is a modular monolith platform designed to connect customers with craftspersons and manage service bookings. Built with .NET 9 and C# 13, it utilizes Clean Architecture and Domain-Driven Design (DDD) principles for each module, ensuring scalability, maintainability, and clear separation of concerns. The platform is composed of several business modules, including Booking Management and User Management, and is orchestrated via an API Gateway for unified routing and service management.

## Table of Contents
- [Project Structure](#project-structure)
- [Modular Monolith & Clean Architecture](#modular-monolith--clean-architecture)
- [API Gateway](#api-gateway)
- [Core Features](#core-features)
- [Technology Stack](#technology-stack)
- [Modules](#modules)
- [Domain Model (Booking Management)](#domain-model-booking-management)
- [CQRS & Handlers (Booking Management)](#cqrs--handlers-booking-management)
- [Validation (Booking Management)](#validation-booking-management)
- [Integration Events (Booking Management)](#integration-events-booking-management)
- [Logging](#logging)
- [API Endpoints (Booking Management)](#api-endpoints-booking-management)
- [How to Run](#how-to-run)
- [Contributing](#contributing)
- [License](#license)

---

## Project Structure

- **Domain**: Business entities and value objects, organized per module (e.g., Booking, User).
- **Application**: CQRS commands/queries, handlers, DTOs, validators, and service contracts, per module.
- **Infrastructure**: Data access, repository implementations, event bus, logging, per module.
- **Presentation**: API controllers for each module.
- **Core**: Shared kernel, integration events, value objects.
- **API Gateway**: Centralized routing, rate limiting, and service orchestration using Ocelot.

## Modular Monolith & Clean Architecture

- CraftConnect is structured as a modular monolith, where each business domain (e.g., Booking Management, User Management) is implemented as a distinct module.
- Each module follows Clean Architecture, separating domain, application, infrastructure, and presentation concerns.
- DDD is applied within each module, ensuring that domain logic is encapsulated and business rules are enforced at the core.
- Modules communicate via well-defined interfaces and integration events, supporting extensibility and maintainability.

## API Gateway

- The API Gateway (Ocelot) provides unified routing for all modules, including Booking Management and User Management.
- Supports rate limiting, downstream service mapping, and centralized endpoint management.
- Example routes:
  - `/users/{everything}` ? User Management Service
  - `/bookings/{everything}` ? Booking Management Service

## Core Features

- **Service Booking**: Customers can request and manage bookings with craftspersons.
- **User Management**: Registration, authentication, and profile management for users and craftspersons.
- **Booking Line Items**: Add itemized jobs to bookings, each with description, price, and quantity.
- **Validation**: All requests are validated for correctness and completeness.
- **Event-Driven**: Booking and line item creation trigger integration events for downstream processing.
- **Logging**: Structured logging for traceability and debugging.
- **Extensible Domain Model**: Easily add new features such as booking status updates, cancellation, and more.

## Technology Stack

- **C# 13.0**
- **.NET 9**
- **MediatR**: CQRS and request/response handling.
- **AutoMapper**: Mapping between domain entities and DTOs.
- **MassTransit**: Integration event publishing.
- **FluentValidation**: Input validation.
- **Ocelot**: API Gateway for routing and orchestration.
- **Custom Logging Service**: Structured logging.

## Modules

- **Booking Management**: Handles all booking-related operations, aggregates, and events.
- **User Management**: Manages user registration, authentication, and profiles.
- **Infrastructure**: Shared services such as email, background jobs, caching, and PDF generation.
- **Core**: Shared kernel and cross-cutting concerns.

## Domain Model (Booking Management)

### Booking
- Represents a service booking between a customer and a craftsperson.
- Properties: CraftmanId, CustomerId, ServiceAddress, Details, Status, Duration, CreatedAt, LineItems.
- Methods: Create, AddLineItem, ConfirmBooking.

### BookingLineItem
- Represents an itemized job within a booking.
- Properties: Description, Price, Quantity.
- Methods: Created and managed via Booking aggregate.

### Address & JobDetails
- Value objects for encapsulating address and job description.

## CQRS & Handlers (Booking Management)

- **Commands**: For write operations (e.g., CreateBookingCommand, BookingLineItemCreateCommand).
- **Handlers**: Validate input, persist data, publish events.
- **BookingLineItemCreateCommandHandler**: Validates and adds line items to bookings, updates repository, logs actions.

## Validation (Booking Management)

- **BookingCreateDTOValidator**: Validates booking requests.
- **BookingLineItemCreateDTOValidator**: Validates line item requests.
- Validation failures are logged and returned with error messages.

## Integration Events (Booking Management)

- **BookingRequestedIntegrationEvent**: Published on booking creation.
- **BookingLineItemIntegrationEvent**: Published on line item addition.
- Enables other services to react to booking events (e.g., notifications).

## Logging

- Uses custom logging service (`ILoggingService<T>`) for contextual logging at various levels.

## API Endpoints (Booking Management)

- **Create Booking**: `POST /bookings`
- **Update Booking**: `PUT /update-booking`
- **Delete Booking**: `DELETE /{id:guid}`
- **Add Line Item to Booking**: (commented, but available for extension) `POST /{id:guid}/add-line-item`

## How to Run

1. **Clone the repository**
   ```
   git clone <repository-url>
   ```
2. **Restore dependencies**
   ```
   dotnet restore
   ```
3. **Build the solution**
   ```
   dotnet build
   ```
4. **Run the application**
   ```
   dotnet run --project <StartupProject>
   ```
5. **Test the API**
   Use tools like Postman or Swagger to interact with the endpoints.

## Contributing

1. Fork the repository.
2. Create a feature branch (`git checkout -b feature/your-feature`).
3. Commit your changes with clear messages.
4. Push to your fork and submit a pull request.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

---

## Continuous Updates

This README is updated to reflect recent changes:
- Added BookingLineItem aggregate and repository.
- Implemented BookingLineItemCreateCommand and handler for itemized bookings.
- Enhanced validation and logging for booking and line item creation.
- API endpoints for booking and line item management.
- Project restructured as a modular monolith using Clean Architecture and DDD for each module.
- Clarified CraftConnect as the main project, with Booking Management as a module.
- Added API Gateway details for unified routing and orchestration.

**Purpose:**
- Provide clear documentation for developers and stakeholders.
- Explain architectural choices and implementation details.
- Ensure smooth onboarding and maintenance.

*For questions or suggestions, please open an issue or contact the maintainers.*