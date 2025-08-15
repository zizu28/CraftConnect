# CraftConnect

# Booking Management System

## Overview

The Booking Management System is a robust, scalable solution for managing service bookings between customers and craftspersons. Built with modern .NET technologies (C# 13, .NET 9), it leverages CQRS, MediatR, and event-driven architecture to ensure maintainability, extensibility, and high performance.

## Table of Contents

- [Project Structure](#project-structure)
- [Core Features](#core-features)
- [Technology Stack](#technology-stack)
- [Domain Model](#domain-model)
- [CQRS & Handlers](#cqrs--handlers)
- [Validation](#validation)
- [Integration Events](#integration-events)
- [Logging](#logging)
- [How to Run](#how-to-run)
- [Contributing](#contributing)
- [License](#license)

---

## Project Structure

The solution is organized into several key layers:

- **Domain**: Core business entities and value objects (e.g., `Booking`, `Address`, `JobDetails`).
- **Application**: CQRS commands/queries, handlers, DTOs, validators, and service contracts.
- **Infrastructure**: Data access, event bus integration, and logging implementations.
- **Core**: Shared kernel components, integration events, and value objects.

Each layer is designed for separation of concerns, making the system easy to maintain and extend.

## Core Features

- **Booking Creation**: Customers can request bookings with craftspersons, specifying job details and service address.
- **Validation**: All booking requests are validated for correctness and completeness.
- **Event-Driven**: Booking creation triggers integration events for downstream processing.
- **Logging**: Comprehensive logging for traceability and debugging.
- **Extensible Domain Model**: Easily add new features such as booking line items, status updates, and cancellation reasons.

## Technology Stack

- **C# 13.0**
- **.NET 9**
- **MediatR**: For CQRS and request/response handling.
- **AutoMapper**: For mapping between domain entities and DTOs.
- **MassTransit**: For integration event publishing.
- **FluentValidation**: For input validation.
- **Custom Logging Service**: For structured logging.

## Domain Model

### Booking

Represents a service booking between a customer and a craftsperson. Key properties include:

- `CraftmanId`, `CustomerId`
- `ServiceAddress` (as a value object)
- `Details` (job description)
- `Status` (enum)
- `LineItems` (optional, for itemized jobs)
- `CreatedAt`, `Duration`

### Address

A value object encapsulating street, city, and postal code.

### JobDetails

Contains the job description and other relevant details.

## CQRS & Handlers

The system uses the Command Query Responsibility Segregation (CQRS) pattern:

- **Commands**: For write operations (e.g., `CreateBookingCommand`)
- **Handlers**: Process commands, validate input, persist data, and publish events.

Example:  
`CreateBookingCommandHandler` validates the booking request, creates a booking entity, saves it, and publishes a `BookingRequestedIntegrationEvent`.

## Validation

All incoming booking requests are validated using `BookingCreateDTOValidator`. Validation failures are logged and returned to the client with detailed error messages.

## Integration Events

Upon successful booking creation, a `BookingRequestedIntegrationEvent` is published via MassTransit. This enables other services to react to booking events (e.g., notifications, workflow triggers).

## Logging

The system uses a custom logging service (`ILoggingService<T>`) for structured, contextual logging at various levels (Trace, Debug, Information, Warning, Error, Critical).

## How to Run

1. **Clone the repository**  
2. **Restore dependencies**  
3. **Build the solution**  
4. **Run the application**  
5. **Test the API**  
   Use tools like Postman or Swagger to interact with the booking endpoints.

## Contributing

Contributions are welcome! Please follow these steps:

1. Fork the repository.
2. Create a feature branch (`git checkout -b feature/your-feature`).
3. Commit your changes with clear messages.
4. Push to your fork and submit a pull request.

All code should follow the existing style and include appropriate tests.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

---

## Continuous Updates

This README is designed to be updated automatically as new features, modules, or changes are pushed to the repository. Each section should be expanded to reflect new capabilities, architectural decisions, and usage instructions.

**Purpose:**  
- To provide clear documentation for developers and stakeholders.
- To explain the rationale behind architectural choices and implementation details.
- To ensure onboarding is smooth and maintenance is straightforward.

---

*For questions or suggestions, please open an issue or contact the maintainers.*
