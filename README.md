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
