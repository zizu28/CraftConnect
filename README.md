# CraftConnect Modular Monolith ??

Welcome to **CraftConnect** – a modern, modular monolith platform built with .NET 9 and C# 13! This solution is designed for scalability, maintainability, and developer happiness. Whether you're a contributor, tester, or just exploring, this README will guide you through the essentials. 

---

## ??? Project Structure

| ?? Project Name | ?? Description |
|----------------|---------------|
| Infrastructure.BackgroundJobs | Background job processing infrastructure |
| Infrastructure.Cache | Caching infrastructure |
| Core.SharedKernel | Shared domain kernel and base types |
| Infrastructure.PDFGeneration | PDF generation infrastructure |
| Infrastructure.EmailService | Email service infrastructure |
| Core.Logging | Logging infrastructure |
| Core.EventServices | Event services infrastructure |
| Infrastructure.Persistence | Database context and persistence infrastructure |
| Core.APIGateway | API Gateway |
| UserManagement.Domain | User management domain models |
| UserManagement.Application | User management application logic |
| UserManagement.Infrastructure | User management infrastructure (repositories, etc.) |
| UserManagement.Presentation | User management API/controllers |
| CraftConnect.AppHost | Main application host |
| CraftConnect.ServiceDefaults | Service default configuration |
| BookingManagement.Domain | Booking management domain models |
| BookingManagement.Application | Booking management application logic |
| BookingManagement.Infrastructure | Booking management infrastructure |
| BookingManagement.Presentation | Booking management API/controllers |
| CraftConnect.Tests | ?? xUnit test project for CraftConnect modules |
| ProductInventoryManagement.Domain | Product inventory domain models |
| ProductInventoryManagement.Application | Product inventory application logic |

---

## ? Build & Test

- Target Framework: **.NET 9**
- CI/CD: GitHub Actions (`.github/workflows/dotnet.yml`)
- Unit tests: `CraftConnect.Tests` (run automatically on push to `main`)

### Quickstart ?????

```sh
# 1?? Clone the repository
git clone <repository-url>

# 2?? Restore dependencies
dotnet restore

# 3?? Build the solution
dotnet build

# 4?? Run tests
dotnet test CraftConnect.Tests/CraftConnect.Tests.csproj
```

---

## ?? Key Concepts

- **Modular Monolith**: Each business domain (Booking, User, Product Inventory) is a separate module, following Clean Architecture and DDD principles.
- **CQRS**: Command and Query Responsibility Segregation for scalable, maintainable code.
- **API Gateway**: Centralized routing and orchestration for all modules.
- **Event-Driven**: Integration events for decoupled communication between modules.
- **Validation & Logging**: Robust input validation and structured logging for reliability.

---

## ?? API Endpoints

- `/users/*` – User Management
- `/bookings/*` – Booking Management
- `/products/*` – Product Inventory

Explore endpoints with Swagger or Postman for a hands-on experience!

---

## ?? Contributing

We welcome your ideas, bug reports, and pull requests!

1. Fork the repo
2. Create a feature branch (`git checkout -b feature/your-feature`)
3. Commit your changes ??
4. Push and open a PR

---

## ?? License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

---

## ?? Need Help?

Open an issue or contact the maintainers. Happy coding! ?