# JTradeERP backend

ASP.NET Core Web API for the JTradeERP ERP backend. It uses Clean Architecture, CQRS/MediatR,
EF Core with SQL Server for transactional writes, and Dapper for read-only reporting queries.

## Implemented backend areas

- Foundation, Master Data, Authentication/Roles/Permissions, Inventory, Purchasing, and Billing.
- Administration provides user/role/permission operations and a paged, read-only audit-log endpoint.
- Reporting provides Sales Summary, Stock Valuation, Low Stock, Stock Movement, and Customer Aging queries.
- Customer Aging is evaluated at a required AsOfDate using InvoiceDate and the approved Current, 1–30, 31–60, 61–90, and 91+ day buckets.
- Report CSV endpoints export the selected filtered/sorted page; report endpoints require `Reports.View`.
- No Angular frontend or later ERP modules are included in this backend.

## Local setup

1. Configure `ConnectionStrings:DefaultConnection` using user secrets, an environment variable,
   or a local development configuration file. Never commit real credentials.
2. Restore, build, and test from this directory:

   ```powershell
   dotnet restore JTradeERP.sln
   dotnet build JTradeERP.sln
   dotnet test JTradeERP.sln
   ```

3. Apply the existing EF Core migrations to the intended database before starting the API:

   ```powershell
   dotnet ef database update --project src/JTradeERP.Infrastructure --startup-project src/JTradeERP.Api
   ```

## Integration-test database safety

- SQL Server integration tests use `JTradeERP_MD01_IntegrationTests`, configured by the test host.
- `ApiFactory` checks the database name and instance before it can reset the database.
- Tests must never point at, reset, or mutate the development `JTradeERP` database.

## Health endpoints

- `/health/live` checks API process health only.
- `/health/ready` includes the SQL Server/EF Core readiness check.
- `/health` exposes the configured health-check aggregate.

## Read-only reports

- `GET /api/reports/sales-summary`
- `GET /api/reports/stock-valuation`
- `GET /api/reports/low-stock`
- `GET /api/reports/stock-movement`
- `GET /api/reports/customer-aging?asOfDate=YYYY-MM-DD`
- Each report also has an `/export.csv` route. Filters, sorting, and paging are bound to the report
  query; sort fields are allow-listed and SQL values are parameterized.
