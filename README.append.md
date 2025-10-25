---

## ?? Local Workspace Additions (synced from local repository)

The following items were recently added to the local workspace and are documented here to reflect the current implementation. These additions extend the existing README content and do not remove or modify prior sections.

### CraftConnect.WebUI (Blazor)
- Project: `CraftConnect.WebUI` — a Blazor WebAssembly frontend providing a modern, responsive UI for the platform.
- Key capabilities:
 - Reusable Blazor components and shared layouts
 - Real-time updates using SignalR for payment and booking status
 - Material Design components (e.g. MudBlazor) for consistent UX
 - PWA support and client-side caching
 - Client-side and server-side validation integrated with FluentValidation
- Development notes:
 - Frontend assets located under `src/CraftConnect.WebUI`
 - Use `dotnet watch run` for iterative frontend + backend development in local environments

### Payment Transaction Management
- New endpoints (server side):
 - `GET /api/payments/transactions` — retrieve all payment transactions with optional filtering
 - `GET /api/payments/{id}/transactions` — retrieve transactions for a specific payment
- Improvements:
 - Efficient transaction aggregation using LINQ
 - Chronological ordering of transactions (most recent first)
 - Enhanced logging for easier troubleshooting

### Paystack Integration (typed HTTP client plan)
- The local design favors a typed HTTP client for Paystack (via `IHttpClientFactory`) instead of a direct SDK in most application layers.
- Recommended approach:
 - Register a typed `HttpClient` configured with Paystack base address and default headers
 - Implement an `IPaystackClient` interface encapsulating: `VerifyTransactionAsync`, `InitializeTransactionAsync`, `CreateRefundAsync`, `GetTransactionAsync`
 - Perform Paystack calls in command handlers, webhook handlers, or background services (keep queries read-only)
 - Use webhook endpoints for real-time event delivery and a background reconciliation job for missed events
- Configuration keys expected in `appsettings`:
 - `Paystack:SecretKey`, `Paystack:PublicKey`, `Paystack:WebhookSecret`

### Tests and Coverage
- Additional unit tests were added for payment controller and transaction endpoints
- Tests use xUnit and Moq and follow the Arrange-Act-Assert pattern
- Run tests:
 - `dotnet test`
 - `dotnet test --filter "PaymentsControllerTests"`

### Projects of interest (local)
- `CraftConnect.WebUI` — Blazor WebAssembly frontend
- `PaymentManagement.Application` — business logic, CQRS handlers
- `PaymentManagement.Presentation` — API controllers including payment and transaction endpoints
- `CraftConnect.Tests` — unit and integration tests

---

If you prefer, I can open a pull request with this README extension instead of pushing directly to `main`.
