# CraftConnect Modular Monolith

Welcome to **CraftConnect** – a modern, modular monolith platform built with .NET 9 and C# 13! This solution connects craftspeople with customers through a comprehensive booking and product management system. The platform follows Clean Architecture principles, Domain-Driven Design patterns, and implements the **Unit of Work pattern** for robust transaction management.

---

## 🚀 **Latest Updates**

### **Payment Verification Enhancement** ✅ **(December 2024)**
- **New VerifyPaymentAsync Endpoint**: Added payment verification functionality to PaymentsController
- **Paystack Integration**: Enhanced payment verification with external payment gateway support
- **Comprehensive Test Coverage**: Added 14 new unit tests for payment verification scenarios
- **Robust Error Handling**: Improved validation and exception handling for payment verification

### **Unit of Work Pattern Implementation** ✅
- **Centralized Transaction Management**: All database operations now use the Unit of Work pattern
- **Transaction Safety**: Atomic operations across modules

### **Modules Expansion** 🆕
- **PaymentManagement**: Full payment processing and invoicing
- **AuditManagement**: System-wide audit logging
- **ContentAndSkills**: CMS capabilities for platform skills and content
- **NotificationManagement**: Centralized notification system
- **ReportingAnalytics**: System health and snapshots

---

## Project Overview

CraftConnect is a sophisticated platform that enables:
- **Craftspeople Management**: Registration, verification, and profile management for service providers
- **Customer Management**: Customer registration, preferences, and booking history
- **Booking System**: End-to-end booking lifecycle from creation to completion
- **Product Inventory**: Product catalog with inventory management for craftspeople
- **Payment Processing**: Secure payment handling, invoicing, and refund management
- **Content Moderation**: Managing platform skills and moderation reports
- **System Auditing**: Tracking critical system changes and user actions
- **Analyitcs**: Monitoring system health and business metrics

---

## Solution Architecture

The repository follows a **modular monolith** architecture with clear separation of concerns, split into **Core**, **Infrastructure**, **Modules**, and **Presentation** layers.

### Core Infrastructure Projects
- **Core.SharedKernel**: Shared domain abstractions, base entities, value objects
- **Core.Logging**: Centralized structured logging infrastructure
- **Core.EventServices**: Event handling abstractions
- **Core.APIGateway**: API gateway for routing
- **Core.BFF**: Backend for Frontend patterns

### Business Domain Modules

#### User Management Module
- **Domain**: User, Customer, Craftman entities
- **Application**: User registration, profile management
- **Infrastructure**: Repositories for user data

#### Booking Management Module
- **Domain**: Booking lifecycle, line items
- **Application**: Booking workflows, status updates
- **Infrastructure**: Booking persistence

#### Product Inventory Management Module
- **Domain**: Product entities, categories, inventory
- **Application**: Product catalog management
- **Infrastructure**: Product persistence

#### Payment Management Module 🆕
- **Domain**: Payment, Invoice, PaymentTransaction, Refund
- **Application**: Payment processing, invoicing, external gateway integration
- **Infrastructure**: Payment persistence

#### Audit Management Module 🆕
- **Domain**: `AuditLog` entity for tracking changes
- **Purpose**: Provides accountability and traceability for system actions

#### Content & Skills Management Module 🆕
- **Domain**: `Skill`, `Category`, `ContentModerationReport`
- **Purpose**: Manages variable content like list of available skills and moderates user-generated content

#### Notification Management Module 🆕
- **Domain**: `Notification`, `Announcement`, `EmailTemplate`
- **Purpose**: Centralized systems for sending Application notifications and system-wide announcements

#### Reporting & Analytics Management Module 🆕
- **Domain**: `AnalyticsSnapshot`, `SystemHealth`
- **Purpose**: Aggregates data for business insights and monitors system status

#### Review & Rating Management Module 🆕
- **Domain**: `Review`
- **Purpose**: Captures user feedback and ratings for services and products

### Frontend Architecture (Blazor WASM) 🆕

The **CraftConnect.WASM** project is a Blazor WebAssembly client providing a rich, interactive user interface.

- **Layout**: MainLayout, NavMenu for application structure
- **Pages**: Razor components mapping to business domains (Auth, Booking, User, etc.)
- **Components**: Reusable UI elements (Buttons, Inputs, Modals)
- **Services**: Frontend services for API integration (HttpClients)
- **ViewModels**: View models for binding UI data decoupled from API DTOs
- **Auth**: Authentication state providers and login flows

---

## Technology Stack

### Framework & Language
- **.NET 9**: Latest framework
- **C# 13**: Modern language features
- **Blazor WASM**: Single Page Application frontend

### Architecture Patterns
- **Clean Architecture**
- **Domain-Driven Design (DDD)**
- **CQRS with MediatR**
- **Unit of Work Pattern**
- **Event-Driven Architecture**

### Data & Persistence
- **SQL Server**: Primary database
- **Entity Framework Core**: ORM
- **Distributed Cache**: Redis/Memory cache

---

## Getting Started

### Prerequisites
- **.NET 9 SDK**
- **SQL Server**
- **Visual Studio 2022** or **VS Code**

### Quick Start Guide

1. **Clone the Repository**
   ```bash
   git clone https://github.com/zizu28/CraftConnect.git
   cd CraftConnect
   ```

2. **Run Database Migrations**
   ```bash
   dotnet ef database update --project Infrastructure.Persistence --startup-project CraftConnect.AppHost
   ```

3. **Run the Application**
   ```bash
   dotnet run --project CraftConnect.AppHost
   ```
   
4. **Access the App**
   - API: `https://localhost:5001`
   - Frontend: `https://localhost:5002` (or configured port)

---

## Database Schema

The `ApplicationDbContext` centrally manages all module entities. Key entities include:

- **Identity**: Users, Customers, Craftmen
- **Operation**: Bookings, Products, Payments, Invoices
- **Support**: AuditLogs, Notifications, Reviews, Skills

---

## Contributing

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Create a new Pull Request

---

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.
