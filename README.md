# CraftConnect

# Booking Management System

## Overview

The Booking Management System is a modular monolith designed using Clean Architecture and Domain-Driven Design (DDD) principles for each module. Built with .NET 9 and C# 13, it leverages CQRS, MediatR, and a layered architecture for scalability, maintainability, and clear separation of concerns. Each module encapsulates its own domain logic, application services, infrastructure, and presentation layers, ensuring high cohesion and low coupling across the system.

## Table of Contents
- [Project Structure](#project-structure)
- [Modular Monolith & Clean Architecture](#modular-monolith--clean-architecture)
- [Core Features](#core-features)
- [Technology Stack](#technology-stack)
- [Domain Model](#domain-model)
- [CQRS & Handlers](#cqrs--handlers)
- [Validation](#validation)
- [Integration Events](#integration-events)
- [Logging](#logging)
- [API Endpoints](#api-endpoints)
- [How to Run](#how-to-run)
- [Contributing](#contributing)
- [License](#license)

---

## Project Structure

- **Domain**: Business entities (Booking, BookingLineItem, Address, JobDetails) and value objects, organized per module.
- **Application**: CQRS commands/queries, handlers, DTOs, validators, and service contracts, per module.
- **Infrastructure**: Data access, repository implementations, event bus, logging, per module.
- **Presentation**: API controllers for bookings, line items, and other modules.
- **Core**: Shared kernel, integration events, value objects.

## Modular Monolith & Clean Architecture

- The project is structured as a modular monolith, where each business domain (e.g., Booking Management, User Management) is implemented as a distinct module.
- Each module follows Clean Architecture, separating domain, application, infrastructure, and presentation concerns.
- DDD is applied within each module, ensuring that domain logic is encapsulated and business rules are enforced at the core.
- Modules communicate via well-defined interfaces and integration events, supporting extensibility and maintainability.

## Core Features

- **Booking Creation**: Customers request bookings with craftspersons, specifying job details and service address.
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
- **Custom Logging Service**: Structured logging.

## Domain Model

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

## CQRS & Handlers

- **Commands**: For write operations (e.g., CreateBookingCommand, BookingLineItemCreateCommand).
- **Handlers**: Validate input, persist data, publish events.
- **BookingLineItemCreateCommandHandler**: Validates and adds line items to bookings, updates repository, logs actions.

## Validation

- **BookingCreateDTOValidator**: Validates booking requests.
- **BookingLineItemCreateDTOValidator**: Validates line item requests.
- Validation failures are logged and returned with error messages.

## Integration Events

- **BookingRequestedIntegrationEvent**: Published on booking creation.
- **BookingLineItemIntegrationEvent**: Published on line item addition.
- Enables other services to react to booking events (e.g., notifications).

## Logging

- Uses custom logging service (`ILoggingService<T>`) for contextual logging at various levels.

## API Endpoints

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
   Use tools like Postman or Swagger to interact with the booking endpoints.

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

**Purpose:**
- Provide clear documentation for developers and stakeholders.
- Explain architectural choices and implementation details.
- Ensure smooth onboarding and maintenance.

*For questions or suggestions, please open an issue or contact the maintainers.*