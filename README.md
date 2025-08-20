# CraftConnect Modular Monolith

This repository contains the CraftConnect modular monolith, targeting .NET 9. The solution is organized into multiple projects grouped by domain and infrastructure responsibilities.

## Project Structure

| Project Name | Description |
|-------------|-------------|
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
| CraftConnect.Tests | xUnit test project for CraftConnect modules |
| ProductInventoryManagement.Domain | Product inventory domain models |
| ProductInventoryManagement.Application | Product inventory application logic |

## Build & Test

- The solution targets .NET 9.
- CI/CD is configured via GitHub Actions in `.github/workflows/dotnet.yml`.
- Unit tests are located in `CraftConnect.Tests` and run automatically on push to `main`.

## Getting Started

1. Clone the repository.
2. Restore dependencies: `dotnet restore`
3. Build the solution: `dotnet build`
4. Run tests: `dotnet test CraftConnect.Tests/CraftConnect.Tests.csproj`

## Contributing

Please submit issues and pull requests for improvements or bug fixes.

---

For more details, see individual project folders and documentation.