# CraftConnect Modular Monolith

Welcome to **CraftConnect** – a modern, modular monolith platform built with .NET 9 and C# 13! This solution connects craftspeople with customers through a comprehensive booking and product management system. The platform follows Clean Architecture principles, Domain-Driven Design patterns, and implements the **Unit of Work pattern** for robust transaction management.

---

## 🚀 **Latest Updates**

### **Unit of Work Pattern Implementation** ✅
- **Centralized Transaction Management**: All database operations now use the Unit of Work pattern
- **Enhanced Repository Pattern**: Removed `SaveChangesAsync()` from repositories - now handled by UnitOfWork
- **Transaction Safety**: All command handlers use `ExecuteInTransactionAsync()` for atomic operations
- **Improved Error Handling**: Automatic rollback on exceptions with proper transaction boundaries
- **Dependency Injection**: UnitOfWork registered in all module infrastructure extensions

---

## Project Overview

CraftConnect is a sophisticated platform that enables:
- **Craftspeople Management**: Registration, verification, and profile management for service providers
- **Customer Management**: Customer registration, preferences, and booking history  
- **Booking System**: End-to-end booking lifecycle from creation to completion with transactional integrity
- **Product Inventory**: Product catalog with inventory management for craftspeople
- **Integration Events**: Event-driven communication between modules using MassTransit
- **Unit of Work**: Consistent transaction management across all business operations

---

## Solution Architecture

The repository follows a **modular monolith** architecture with clear separation of concerns and robust transaction management:

### Core Infrastructure Projects
- **Core.SharedKernel**: Shared domain abstractions, base entities, value objects, and unified repository interfaces
- **Core.Logging**: Centralized structured logging infrastructure for traceability and debugging
- **Core.EventServices**: Event handling abstractions and integration event infrastructure
- **Core.APIGateway**: API gateway for routing, rate limiting, and service orchestration
- **Infrastructure.Persistence**: 
  - Centralized data persistence with Entity Framework Core
  - **Unit of Work Implementation** with transaction management helpers
  - Base repository pattern without `SaveChangesAsync()` 
  - ApplicationDbContext with all domain entities
- **Infrastructure.Cache**: Distributed caching implementation for performance optimization
- **Infrastructure.BackgroundJobs**: Background job processing for asynchronous and scheduled operations
- **Infrastructure.EmailService**: Email notification services with template support
- **Infrastructure.PDFGeneration**: PDF document generation for reports and invoices
- **CraftConnect.AppHost**: Main application host and bootstrapping entry point
- **CraftConnect.ServiceDefaults**: Default service configurations and dependency injection setup

### Business Domain Modules

#### User Management Module
- **UserManagement.Domain**: Domain entities (User, Customer, Craftman), value objects, and business rules
- **UserManagement.Application**: CQRS handlers with Unit of Work, DTOs, validators, and application services
- **UserManagement.Infrastructure**: Repository implementations and infrastructure services with UnitOfWork DI
- **UserManagement.Presentation**: API controllers and endpoints for user operations

#### Booking Management Module  
- **BookingManagement.Domain**: Booking lifecycle, line items, and business logic
- **BookingManagement.Application**: Booking CQRS operations with transactional workflow management
- **BookingManagement.Infrastructure**: Booking data persistence and external service integrations with UnitOfWork DI
- **BookingManagement.Presentation**: Booking API endpoints and controllers

#### Product Inventory Management Module
- **ProductInventoryManagement.Domain**: Product entities, categories, inventory management
- **ProductInventoryManagement.Application**: Product CQRS operations with transactional inventory updates
- **ProductInventoryManagement.Infrastructure**: Product repository implementations with UnitOfWork DI
- **ProductInventoryManagement.Presentation**: Product and category API controllers

### Testing
- **CraftConnect.Tests**: Comprehensive unit and integration tests using xUnit and Moq
- **Repository Tests**: Integration tests for Unit of Work pattern
- **Performance Benchmarks**: BenchmarkDotNet tests for repository performance

---

## 🏗️ **Technical Architecture Highlights**

### **Unit of Work Pattern**
```csharp
// Enhanced IUnitOfWork with helper methods
public interface IUnitOfWork : IDisposable
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
    Task ExecuteInTransactionAsync(Func<Task> operation, CancellationToken cancellationToken = default);
    Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> operation, CancellationToken cancellationToken = default);
}
```

### **Command Handler Pattern with UnitOfWork**
```csharp
public async Task<Unit> Handle(DeleteCraftmanCommand request, CancellationToken cancellationToken)
{
    return await unitOfWork.ExecuteInTransactionAsync(async () =>
    {
        var craftman = await craftmanRepository.GetByIdAsync(request.CraftmanId, cancellationToken)
            ?? throw new KeyNotFoundException($"Craftman with ID {request.CraftmanId} not found.");
        
        await craftmanRepository.DeleteAsync(craftman.Id, cancellationToken);
        logger.LogInformation("Craftman with ID {CraftmanId} deleted successfully.", craftman.Id);
        
        return Unit.Value;
    }, cancellationToken);
}
```

### **Repository Pattern**
- **No SaveChangesAsync**: Repositories focus on data access, not transaction management
- **Nullable Return Types**: Better null safety with `T?` return types
- **ExistsAsync Method**: Added to all repository interfaces for efficient existence checks
- **Base Repository**: Common CRUD operations with Entity Framework Core

---
