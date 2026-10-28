# JTradeERP API Contract Inventory (FE-00)

**Discovery date:** 2026-10-12 (Asia/Dubai)  
**Status:** source-audited inventory of the current backend; not an OpenAPI-generated contract.

## Authority, coverage and evidence notation

- The checked-in backend is authoritative for routes and currently serialized DTOs. `docs/Parent-SRS-Traceability.md` supplies parent requirement references, not the full parent SRS text.
- The React Frontend SRS and the complete parent SRS were not present under this repository's `docs/`, `src/` or `tests/` at discovery time. Proposed screen mapping is therefore a frontend adaptation based on backend features, not a claim about an unseen SRS.
- Routes are registered in `src/JTradeERP.Api/Program.cs` through feature endpoint extensions. Prefixes below are the actual `MapGroup` prefixes; suffixes come from endpoint files. Source paths are repository-relative.
- **Source-confirmed** means directly implemented in endpoint/handler/validator/persistence source. **Test-confirmed** means exercised by the named integration tests. If only a test suite is cited, it does not imply every field/rule in that area has an individual assertion.
- Standard JSON property naming follows ASP.NET Core web defaults (camelCase); enum serialization configuration was not established as a string converter in the inspected API setup, so enum wire values must be treated as numeric unless runtime configuration/tests prove otherwise.

## Endpoint totals

There are **150 `MapGet`/`MapPost`/`MapPut` route mappings in source**, including two diagnostics routes registered only when the environment is `Testing`. Excluding those diagnostics leaves 148 route mappings; three `MapHealthChecks` registrations add 3 production endpoints, for **151 production HTTP endpoints** total.

| Module / base path | Production route count | Coverage |
|---|---:|---|
| Root | 1 | `/` |
| Master Data `/api/master-data` | 40 | Categories, UoM, tax codes, products, customers, suppliers, warehouses |
| Security `/api/auth`, `/api/admin` | 18 | 6 auth operations, 12 administration operations |
| Inventory `/api/inventory` | 18 | opening balances, adjustments, transfers, ledger, balances, reorder, setting |
| Purchasing `/api/purchasing` | 12 | POs, GRNs, over-receipt setting |
| Billing `/api/billing` | 49 | quotations, sales orders, invoices, payments, credit notes, configuration, payment methods, number series, print |
| Reports `/api/reports` | 10 | 5 report queries + 5 CSV exports |
| Health checks | 3 | `/health`, `/health/live`, `/health/ready` |
| **Total production** | **151** | 147 feature-prefix routes + root + 3 health endpoints |

**Count reconciliation:** 49 Billing + 18 Inventory + 40 Master Data + 12 Purchasing + 18 Security + 10 Reports = 147 feature routes. Add `/` and three health routes for 151 production endpoints. The two Testing-only diagnostics bring the source-defined total to 153 when that environment is enabled.

The TypeScript companion contains **324 public Application record contracts**, **11 domain enum declarations**, and the shared `ProblemDetails` shape. Route-only scalar bindings are documented at the endpoint tables and are not converted into artificial request-body records.

## Hosting, response and authentication conventions

| Concern | Confirmed behavior | Evidence |
|---|---|---|
| Authentication | JWT bearer is registered; most feature groups require an authenticated principal, then finer permissions are enforced by endpoint policies or the Master Data filter. Login, refresh, forgot/reset password are anonymous; logout and `/me` require authentication. | `src/JTradeERP.Api/Program.cs`; `src/JTradeERP.Api/Security/AuthenticationRegistration.cs`; Security endpoint files; `tests/JTradeERP.Api.IntegrationTests/AuthenticationAndAuthorizationTests.cs` |
| Authorization | Permission codes are checked against capabilities such as `View`, `CreateEdit`, `Post`, `Full`; API failures are 403 for authorization denial. Master Data derives permission from path/method: `MasterData.View` for GET, `MasterData.Manage` for non-customer/supplier writes, `Customers.Manage` / `Suppliers.Manage` for those routes; delete requires Full. | `src/JTradeERP.Application/Common/Security/PermissionAuthorization.cs`; `src/JTradeERP.Api/Security/PermissionAuthorizationHandler.cs`; endpoint `.RequirePermission(...)`; integration authorization tests |
| Request body | Commands are bound directly as JSON bodies on write endpoints; query records are bound from query parameters (often `[AsParameters]`). Route identifiers are separate parameters. No redundant `XRequest` layer is used by the contracts inspected. | Individual endpoint source files |
| Success codes | Explicit endpoint result behavior is noted per family: created routes generally return 201, `Results.Ok` routes 200, delete/logout routes 204; routes returning MediatR directly rely on minimal API result inference and are not assumed to be 200/201 beyond observed tests. | Endpoint source; test files |
| Errors | Central `ProblemDetails`: validation 400 with `errors`; missing entity 404; conflict, uniqueness/FK, optimistic concurrency 409; authentication failure 401; unexpected exception 500. Authorization middleware can return 401/403. | `src/JTradeERP.Api/Infrastructure/ApiExceptionHandler.cs`; integration tests |
| Nullability | C# nullable annotations and DTO declaration are the contract. `?` denotes nullable; collections are generally read-only collections/lists. See response/request source files linked by the slice name below. | DTO source files |
| Pagination | Only operations whose query/validator explicitly contains Page/PageSize support pagination. Defaults are endpoint-specific; common list defaults in billing/inventory/reports are often page 1 and size 50, with maximums enforced by validators where present. Master Data lists are not paginated. | Query, endpoint and validator files; see operation index |
| Sorting | Source-defined fixed ordering is used for lists/reports; no generic client sort key is exposed unless explicitly identified in the query DTO. | Query handlers and report service |
| Transactions | EF write use cases execute through `SaveChanges` and transactional application services; posting stock-impacting documents has atomic inventory behavior. SQL-backed relational tests exist for core module flows. | handlers, `IApplicationTransaction`, Infrastructure persistence; integration tests by module |
| Concurrency / idempotency | Stock balance uses SQL Server rowversion; stock-document posting and number allocation use explicit concurrency/idempotency mechanisms where implemented. Do not assume retry-safe behavior for every POST. | `StockBalance` mapping, stock posting services, number-series allocator, billing/inventory tests |

### Authentication / authorization coverage

| Route group | Count | Access rule |
|---|---:|---|
| Public platform routes | 4 | `/`, `/health`, `/health/live`, `/health/ready` are anonymous. |
| Anonymous auth actions | 4 | login, refresh, forgot-password, reset-password. Refresh token is cookie-read; auth source controls cookie mutation. |
| Authenticated session | 2 | `/api/auth/me` and `/api/auth/logout` require an authenticated JWT principal. |
| Master Data | 40 | Group requires authorization plus `MasterDataPermissionFilter`; GET and mutation capabilities are resolved by path/method, with customer/supplier and delete distinctions. |
| Administration | 12 | All require named `Admin.*` or `AuditLog.View` permission/capability. |
| Inventory, Purchasing, Billing | 79 | Group requires authorization; each endpoint source declares one or more explicit permission requirements. Where multiple are chained, all are required. |
| Reports | 10 | Each JSON/CSV route requires `Reports.View:View`; route group itself does not add a group-level auth requirement. |
| Testing diagnostics | 2 | `/_foundation/throw` and `/_foundation/validation` are registered only for Testing and allow anonymous access. |

The production protected count is 143; 8 routes are anonymous (4 platform + 4 anonymous auth). The two diagnostics routes are excluded from production counts.

### Validation and business-rule source map

The TypeScript types describe shape, not the server's full acceptance logic. Validation runs in the MediatR pipeline and domain/application handlers add reference, lifecycle, uniqueness and transaction checks. The listed validator directories are the authoritative per-operation rule set; they are intentionally cited instead of duplicating rules with possible drift.

| Area | Typical explicit input rules and non-validator checks | Source / test evidence |
|---|---|---|
| Master Data | Required codes/names; max lengths; valid non-empty references; non-negative prices/stock values/credit limit when supplied; valid optional email; uniqueness and FK/delete restrictions. Product name required. | `Features/MasterData/**/<Operation>Validator.cs`, handlers, Domain/MasterData; `MasterDataEndpointTests.cs`, `MasterDataFeatureTests.cs` |
| Billing | Non-empty line collections; positive quantities; non-negative price/discount; valid customer/product/tax references; quotation validity not before quotation date; page/page-size and date-range checks; draft-only document edits and lifecycle checks enforced in handlers/domain. | `Features/Billing/**/<Operation>Validator.cs`, handlers, Domain/Billing; `BillingEndpointTests.cs` |
| Inventory | Non-empty IDs; non-zero adjustment quantity; positive transfer/opening quantities; no same source/destination warehouse; quantity/cost precision checks; page bounds/status parsing; posting checks include warehouse/product activity, balance/negative-stock, idempotency and concurrency. | `Features/Inventory/**/<Operation>Validator.cs`, `Common/Inventory/InventoryPrecision.cs`, posting services; inventory tests |
| Purchasing | Required supplier/warehouse/date/lines; positive ordered/received quantity; non-negative costs/discounts; precision limits; expected date not before order date; page number > 0 and page size 1–200; PO/GRN state and over-receipt checks in handlers. | `Features/Purchasing/**/<Operation>Validator.cs`, `PurchasingPrecision.cs`, handlers; purchasing tests |
| Reports | Page/page-size bounds, valid sort keys/directions, valid date ranges; Customer Aging AsOfDate required and page size 1–500. Read paths only. | `Features/Reports/**/*Validator.cs`; `ReportsEndpointTests.cs`, `ReportsRelationalQueryTests.cs`, `ReportContractTests.cs` |
| Security/Admin | Login credentials required; emails validated; reset OTP length six; audit page size 1–200; unique usernames/emails/role names and role/permission references checked server-side. | `Features/Security/**`; `AuthenticationAndAuthorizationTests.cs`, `AuditLogEndpointTests.cs` |

Individual validator names follow operation names and can be located next to their command/query; operations without a `*Validator.cs` rely on handler/domain preconditions and endpoint authorization. Do not reproduce server-only business invariants solely in React.

### Document lifecycle contracts

| Document | Statuses / permitted transitions confirmed in source | Edit / side-effect notes |
|---|---|---|
| Quotation | Draft → Sent → Accepted or Rejected; Sent can expire after validity date; Cancelled is allowed except from Rejected/Expired/Cancelled. | Only Draft is editable; accepted quotation can convert to Sales Order. |
| Sales Order | Draft → Posted → PartiallyFulfilled → Fulfilled; cancellation allowed unless already Cancelled or Fulfilled. | Only Draft editable; posting reserves quantity; cancellation releases remaining reservation; invoices fulfill line quantities. |
| Invoice | Draft → Posted → PartiallyPaid → Paid; payment reversal returns to Posted or PartiallyPaid; Cancelled is allowed from Posted/PartiallyPaid/Paid only when amount paid is zero. | Only Draft editable; posting issues stock transactionally and snapshots tax/TRNs; cancelled invoices immutable. |
| Credit Note | Draft → Posted; Draft → Cancelled. | Only Draft editable; post applies stock/tax effects in the handler; posted credit note cannot be cancelled by the domain operation. |
| Payment | Posted → Reversed. | Reversal is explicit; allocations restore invoice outstanding balances/statuses. |
| Purchase Order | Draft is initial; receipt application recalculates Open, PartiallyReceived or FullyReceived. `Cancelled` exists in the enum but no cancellation route/command was found. | Editable only while Draft; partial receipts link through GRN; no dedicated PO post endpoint. See GAP-017. |
| Goods Receipt Note | Draft → Posted → Cancelled. | Only Draft editable; posting receives stock; cancel is only allowed after posting and reverses the stock effect. |
| Stock Adjustment / Transfer | Draft → Posted; adjustment can be reversed into Reversed status. Transfer has Draft → Posted only; no transfer reversal route was found. | Draft-only edits. Transfer posting atomically creates paired movements; adjustment reversal is ledger-based. |

Status enums serialize as numeric values unless endpoint-level JSON configuration changes; response `Status` properties declared as `string` are already mapped to enum names by the Application layer.

## Endpoint source lookup

- Master Data route entries are registered by the operation-named `*EndPoint.cs` file in `src/JTradeERP.Application/Features/MasterData/<Feature>/<Operation>/`; registration and prefix are in `MasterDataEndpointExtensions.cs`.
- Billing entries are registered by operation-named `*EndPoint.cs` files under `Features/Billing/<Feature>/<Operation>/`; print entries are under `Features/Billing/PrintInvoice/` and `PrintQuotation/`; full registrations and prefix are in `BillingEndpointExtensions.cs`.
- Inventory and Purchasing entries use the corresponding operation-named endpoint files under `Features/Inventory/` and `Features/Purchasing/`; prefixes are in their `*EndpointExtensions.cs`.
- Report query and CSV routes are each declared in their `Get<Report>EndPoint.cs`; both routes share the same query record and `Reports.View` permission.
- Security/admin routes use `Features/Security/<Operation>/<Operation>Endpoint.cs` or admin `*Slice.cs`; route prefixes are in `SecurityEndpointExtensions.cs`.
- Root/health/conditional diagnostics are mapped directly in `src/JTradeERP.Api/Program.cs` and `Endpoints/FoundationEndpoints.cs`.

## Routes by module

Each row names the operation folder/file containing route registration. `:guid` is an ASP.NET Core GUID route constraint. DTO details are the command/query and response named by the operation; route-only IDs do not have body DTOs.

### Foundation and health

| Method | Route | Input / output | Auth | Source / verification |
|---|---|---|---|---|
| GET | `/` | no input; anonymous service/status object | Anonymous | `src/JTradeERP.Api/Program.cs`; `ApiFoundationTests.cs` |
| GET | `/health` | health-check response | Anonymous | `Program.cs`; `ApiFoundationTests.cs` |
| GET | `/health/live` | liveness health response | Anonymous | `Program.cs` |
| GET | `/health/ready` | tagged readiness checks incl. database | Anonymous | `Program.cs` |
| GET | `/_foundation/throw` | test-only diagnostic; ProblemDetails | Anonymous; Testing environment only | `src/JTradeERP.Api/Endpoints/FoundationEndpoints.cs`; `ApiFoundationTests.cs` |
| GET | `/_foundation/validation?value=` | test-only validation diagnostic | Anonymous; Testing environment only | same |

### Master Data — `/api/master-data`

All routes require JWT and the `MasterDataPermissionFilter`. The endpoint filter applies `MasterData.View` to GET, `MasterData.Manage` to general writes, `Customers.Manage` / `Suppliers.Manage` to customer/supplier writes, with Full capability required for DELETE. List endpoints return collections without pagination in the inspected source.

| Operation | Method + route | Input contract / output contract |
|---|---|---|
| Create category | POST `/categories` | `CreateCategoryCommand` → `CreateCategoryResponse` (201) |
| Update category | PUT `/categories/{id:guid}` | route `id` + `UpdateCategoryCommand` (ID overwritten from route) → `UpdateCategoryResponse` |
| Get category | GET `/categories/{id:guid}` | route ID → `GetCategoryByIdResponse` |
| List categories | GET `/categories` | none → collection of `GetCategoriesResponse` |
| Activate / Deactivate category | POST `/categories/{id:guid}/activate`; POST `/categories/{id:guid}/deactivate` | route ID only → respective response DTO |
| Delete category | DELETE `/categories/{id:guid}` | route ID only → 204; restricted when referenced |
| Create / Update UoM | POST `/units-of-measure`; PUT `/units-of-measure/{id:guid}` | `CreateUnitOfMeasureCommand` / `UpdateUnitOfMeasureCommand` → respective response DTO |
| Get / List UoM | GET `/units-of-measure/{id:guid}`; GET `/units-of-measure` | route ID / none → `GetUnitOfMeasureByIdResponse` / collection of `GetUnitsOfMeasureResponse` |
| Delete UoM | DELETE `/units-of-measure/{id:guid}` | route ID only → 204 |
| Create / Update tax code | POST `/tax-codes`; PUT `/tax-codes/{id:guid}` | respective command → response DTO (create 201) |
| Get / List tax codes | GET `/tax-codes/{id:guid}`; GET `/tax-codes` | route ID / none → response DTO / collection |
| Activate / Deactivate tax code | POST `/tax-codes/{id:guid}/activate`; POST `/tax-codes/{id:guid}/deactivate` | route ID only → respective response DTO |
| Create / Update product | POST `/products`; PUT `/products/{id:guid}` | respective command → response DTO (create 201); Product.Name is present in current DTOs |
| Get / List products | GET `/products/{id:guid}`; GET `/products` | route ID / none → response DTO / collection |
| Activate / Deactivate product | POST `/products/{id:guid}/activate`; POST `/products/{id:guid}/deactivate` | route ID only → respective response DTO |
| Create / Update customer | POST `/customers`; PUT `/customers/{id:guid}` | respective command → response DTO (create 201); `CreditLimit` is nullable |
| Get / List customers | GET `/customers/{id:guid}`; GET `/customers` | route ID / none → response DTO / collection |
| Delete customer | DELETE `/customers/{id:guid}` | route ID only → 204; references may prevent deletion |
| Create / Update supplier | POST `/suppliers`; PUT `/suppliers/{id:guid}` | respective command → response DTO (create 201) |
| Get / List suppliers | GET `/suppliers/{id:guid}`; GET `/suppliers` | route ID / none → response DTO / collection |
| Delete supplier | DELETE `/suppliers/{id:guid}` | route ID only → 204; references may prevent deletion |
| Create / Update warehouse | POST `/warehouses`; PUT `/warehouses/{id:guid}` | respective command → response DTO (create 201) |
| Get / List warehouses | GET `/warehouses/{id:guid}`; GET `/warehouses` | route ID / none → response DTO / collection |
| Activate / Deactivate warehouse | POST `/warehouses/{id:guid}/activate`; POST `/warehouses/{id:guid}/deactivate` | route ID only → respective response DTO |

Source root and per-endpoint references: `src/JTradeERP.Application/Features/MasterData/{Categories,UnitsOfMeasure,TaxCodes,Products,Customers,Suppliers,Warehouses}/<Operation>/<Operation>EndPoint.cs`; endpoint registration: `MasterDataEndpointExtensions.cs`. Verification suites: `MasterDataEndpointTests.cs`, `MasterDataModelTests.cs`, `MasterDataFeatureTests.cs`, `MasterDataContractTests.cs`.

### Security and Administration

| Method + route | Operation/input → output | Auth / permission | Source / tests |
|---|---|---|---|
| POST `/api/auth/login` | `LoginCommand` → `LoginResponse`; sets access-token result and refresh cookie behavior per endpoint | Anonymous | `Security/Login`; `AuthenticationAndAuthorizationTests.cs` |
| POST `/api/auth/refresh` | refresh token read from cookie → `RefreshAccessTokenResponse`; refresh cookie updated | Anonymous | `Security/RefreshAccessToken`; same tests |
| POST `/api/auth/logout` | current user + refresh cookie → 204; clears cookie | Authenticated | `Security/Logout`; same tests |
| GET `/api/auth/me` | no input → `GetMeResponse` | Authenticated | `Security/GetMe`; same tests |
| POST `/api/auth/forgot-password` | `ForgotPasswordCommand` → `ForgotPasswordResponse` | Anonymous | `Security/ForgotPassword`; same tests |
| POST `/api/auth/reset-password` | `ResetPasswordCommand` → `ResetPasswordResponse` | Anonymous | `Security/ResetPassword`; same tests |
| POST `/api/admin/users` | `CreateUserCommand` → created-user response | `Admin.Users.Manage:Full` | Administration/CreateUser slice |
| GET `/api/admin/users` | optional `search`, `isActive` → users result | `Admin.Users.Manage:Full` | GetUsers slice |
| GET `/api/admin/users/{id:guid}` | route ID → user response | same | GetUserById slice |
| PUT `/api/admin/users/{id:guid}/status` | route ID + `UpdateUserStatusCommand` → response | same | UpdateUserStatus slice |
| PUT `/api/admin/users/{id:guid}/roles` | route ID + `AssignRolesCommand` → response | same | AssignRoles slice |
| POST `/api/admin/roles` | `CreateRoleCommand` → response | `Admin.Roles.Manage:Full` | CreateRole slice |
| GET `/api/admin/roles` | optional `search` → role result | same | GetRoles slice |
| GET `/api/admin/roles/{id:guid}` | route ID → role response | same | GetRoleById slice |
| PUT `/api/admin/roles/{id:guid}` | route ID + `UpdateRoleCommand` → response | same | UpdateRole slice |
| PUT `/api/admin/roles/{id:guid}/permissions` | route ID + `AssignPermissionsCommand` → response | same | AssignPermissions slice |
| GET `/api/admin/permissions` | optional `search` → permission result | same | GetPermissions slice |
| GET `/api/admin/audit-logs` | `GetAuditLogsQuery` query parameters → paged audit response | `Admin.Audit.View` / capability as endpoint declares | `Administration/GetAuditLogs`; `AuditLogEndpointTests.cs` |

Admin endpoint DTOs are declared in `src/JTradeERP.Application/Features/Security/Administration/**`; several are consolidated in `*Slice.cs`. Inspect each declaration for exact fields. Passwords are accepted as write inputs but must not be echoed as response properties. Forgot-password response intentionally avoids confirming account existence (source behavior; exercise via tests before relying on exact wording).

### Inventory — `/api/inventory`

All routes require authentication. Permission decorators are endpoint-specific; see each endpoint file. Authorization must not be inferred as one module-wide permission.

| Method + route | Input / output | Permission / evidence |
|---|---|---|
| POST `/opening-balances` | `PostOpeningBalanceCommand` → `PostOpeningBalanceResponse` | `Stock.OpeningBalance:Full` |
| POST `/opening-balances/{id:guid}/reverse` | route ID → `ReverseOpeningBalanceResponse` | `Stock.Post:Post` AND `Stock.OpeningBalance:Full` |
| GET `/stock-balances/{productId:guid}/{warehouseId:guid}` | route IDs → `GetStockBalanceResponse` | `Stock.View:View` |
| GET `/stock-balances?productId=&warehouseId=&page=&pageSize=` | `GetStockBalancesQuery` → `GetStockBalancesResponse` | `Stock.View:View` |
| GET `/stock-ledger?productId=&warehouseId=&from=&to=&sourceDocumentType=&page=&pageSize=` | `GetStockLedgerQuery` → `GetStockLedgerResponse` | `Stock.View:View` |
| GET `/reorder-alerts?warehouseId=` | `GetReorderAlertsQuery` → `GetReorderAlertsResponse` | `Stock.View:View` |
| POST `/adjustments` | `CreateStockAdjustmentCommand` → `CreateStockAdjustmentResponse` | `Stock.Adjust:CreateEdit` |
| PUT `/adjustments/{id:guid}` | `UpdateStockAdjustmentCommand` + route ID → `UpdateStockAdjustmentResponse` | `Stock.Adjust:CreateEdit` |
| GET `/adjustments/{id:guid}` | `GetStockAdjustmentQuery` → `GetStockAdjustmentResponse` | `Stock.View:View` |
| GET `/adjustments?productId=&warehouseId=&status=&page=&pageSize=` | `GetStockAdjustmentsQuery` → `GetStockAdjustmentsResponse` | `Stock.View:View` |
| POST `/adjustments/{id:guid}/post` | `PostStockAdjustmentCommand` with route ID → `PostStockAdjustmentResponse` | `Stock.Post:Post` AND `Stock.Adjust:CreateEdit` |
| POST `/adjustments/{id:guid}/reverse` | route ID → `ReverseStockAdjustmentResponse` | `Stock.Post:Post` AND `Stock.Adjust:CreateEdit` |
| POST `/transfers` | `CreateStockTransferCommand` → `CreateStockTransferResponse` | `Stock.Transfer:CreateEdit` |
| PUT `/transfers/{id:guid}` | `UpdateStockTransferCommand` + route ID → `UpdateStockTransferResponse` | `Stock.Transfer:CreateEdit` |
| GET `/transfers/{id:guid}` | `GetStockTransferQuery` → `GetStockTransferResponse` | `Stock.View:View` |
| GET `/transfers?sourceWarehouseId=&destinationWarehouseId=&page=&pageSize=` | `GetStockTransfersQuery` → `GetStockTransfersResponse` | `Stock.View:View` |
| POST `/transfers/{id:guid}/post` | route ID → `PostStockTransferResponse` | `Stock.Post:Post` AND `Stock.Transfer:CreateEdit` |
| PUT `/warehouses/{id:guid}/negative-stock` | `UpdateWarehouseNegativeStockSettingCommand` + route ID → response | `Admin.Configuration.Manage:Full` |

Source: `src/JTradeERP.Application/Features/Inventory/**`, `Common/Inventory/**`; tests `InventoryEndpointTests.cs`, `InventoryModelTests.cs`, `InventoryDomainTests.cs`.

### Purchasing — `/api/purchasing`

All routes require authorization; operation-specific capability is declared in each endpoint. POs and GRNs have draft/update/post/cancel semantics; posting a GRN has stock receipt side effects.

| Method + route | Input / output |
|---|---|
| POST `/purchase-orders` | `CreatePurchaseOrderCommand` → `CreatePurchaseOrderResponse` |
| PUT `/purchase-orders/{id:guid}` | route ID + `UpdatePurchaseOrderCommand` → response |
| GET `/purchase-orders/{id:guid}` | route ID → `GetPurchaseOrderByIdResponse` |
| GET `/purchase-orders?supplierId=&warehouseId=&status=&pageNumber=&pageSize=` | `GetPurchaseOrdersQuery` → `GetPurchaseOrdersResponse` |
| POST `/goods-receipt-notes` | `CreateGoodsReceiptNoteCommand` → response |
| PUT `/goods-receipt-notes/{id:guid}` | route ID + `UpdateGoodsReceiptNoteCommand` → response |
| POST `/goods-receipt-notes/{id:guid}/post` | route ID → posted GRN response; stock receipt transaction |
| POST `/goods-receipt-notes/{id:guid}/cancel` | route ID → cancel result; reversal when posted per handler/domain behavior |
| GET `/goods-receipt-notes/{id:guid}` | route ID → details response |
| GET `/goods-receipt-notes?purchaseOrderId=&supplierId=&warehouseId=&status=&pageNumber=&pageSize=` | `GetGoodsReceiptNotesQuery` → `GetGoodsReceiptNotesResponse` |
| GET `/configuration/over-receipt` | none → configuration response |
| PUT `/configuration/over-receipt` | `UpdatePurchasingConfigurationCommand` → response |

Source: `Features/Purchasing/**`; tests `PurchasingEndpointTests.cs`, `PurchasingModelTests.cs`, `PurchasingDomainTests.cs`.

### Billing — `/api/billing`

Group requires authorization; route-level permission decorators apply. Query and command fields are defined in the named operation files; line contracts are shared in `Billing/Common/BillingContracts.cs`. Draft-only editability, posting, inventory integration, VAT snapshot, payment allocation, and state transitions are business behavior, not inferred from route names.

| Operation | Method + exact route | Input contract / output |
|---|---|---|
| Create quotation | POST `/api/billing/quotations` | `CreateQuotationCommand` → `CreateQuotationResponse` |
| List quotations | GET `/api/billing/quotations` | `GetQuotationsQuery` → `GetQuotationsResponse` |
| Get quotation | GET `/api/billing/quotations/{id:guid}` | `GetQuotationByIdQuery` → `GetQuotationByIdResponse` |
| Update quotation | PUT `/api/billing/quotations/{id:guid}` | `UpdateQuotationCommand` + route ID → `UpdateQuotationResponse` |
| Copy quotation | POST `/api/billing/quotations/{id:guid}/copy` | `CopyQuotationCommand` → `CopyQuotationResponse` |
| Send quotation | POST `/api/billing/quotations/{id:guid}/send` | `SendQuotationCommand` → `SendQuotationResponse` |
| Accept quotation | POST `/api/billing/quotations/{id:guid}/accept` | `AcceptQuotationCommand` → `AcceptQuotationResponse` |
| Reject quotation | POST `/api/billing/quotations/{id:guid}/reject` | `RejectQuotationCommand` → `RejectQuotationResponse` |
| Cancel quotation | POST `/api/billing/quotations/{id:guid}/cancel` | `CancelQuotationCommand` → `CancelQuotationResponse` |
| Expire quotation | POST `/api/billing/quotations/{id:guid}/expire` | `ExpireQuotationCommand` → `ExpireQuotationResponse` |
| Convert quotation | POST `/api/billing/quotations/{id:guid}/convert-to-sales-order` | `ConvertQuotationToSalesOrderCommand` → `ConvertQuotationToSalesOrderResponse` |
| Print quotation | GET `/api/billing/quotations/{id:guid}/print` | route ID; reuses get-by-ID response; HTML |
| Create sales order | POST `/api/billing/sales-orders` | `CreateSalesOrderCommand` → `CreateSalesOrderResponse` |
| List sales orders | GET `/api/billing/sales-orders` | `GetSalesOrdersQuery` → `GetSalesOrdersResponse` |
| Get sales order | GET `/api/billing/sales-orders/{id:guid}` | `GetSalesOrderByIdQuery` → `GetSalesOrderByIdResponse` |
| Update sales order | PUT `/api/billing/sales-orders/{id:guid}` | `UpdateSalesOrderCommand` + route ID → `UpdateSalesOrderResponse` |
| Post sales order | POST `/api/billing/sales-orders/{id:guid}/post` | route ID only → `PostSalesOrderResponse` |
| Cancel sales order | POST `/api/billing/sales-orders/{id:guid}/cancel` | route ID only → `CancelSalesOrderResponse` |
| Create invoice | POST `/api/billing/invoices` | `CreateInvoiceCommand` → `CreateInvoiceResponse` |
| List invoices | GET `/api/billing/invoices` | `GetInvoicesQuery` → `GetInvoicesResponse` |
| Get invoice | GET `/api/billing/invoices/{id:guid}` | `GetInvoiceByIdQuery` → `GetInvoiceByIdResponse` |
| Update invoice | PUT `/api/billing/invoices/{id:guid}` | `UpdateInvoiceCommand` + route ID → `UpdateInvoiceResponse` |
| Post invoice | POST `/api/billing/invoices/{id:guid}/post` | route ID only → `PostInvoiceResponse`; stock issue is transactional |
| Cancel invoice | POST `/api/billing/invoices/{id:guid}/cancel` | route ID only → `CancelInvoiceResponse` |
| Print invoice | GET `/api/billing/invoices/{id:guid}/print` | route ID; reuses get-by-ID response; HTML |
| Record payment | POST `/api/billing/payments` | `RecordPaymentCommand` → `RecordPaymentResponse` |
| List payments | GET `/api/billing/payments` | `GetPaymentsQuery` → `GetPaymentsResponse` |
| Get payment | GET `/api/billing/payments/{id:guid}` | `GetPaymentByIdQuery` → `GetPaymentByIdResponse` |
| Reverse payment | POST `/api/billing/payments/{id:guid}/reverse` | `ReversePaymentCommand` → `ReversePaymentResponse` |
| Create credit note | POST `/api/billing/credit-notes` | `CreateCreditNoteCommand` → `CreateCreditNoteResponse` |
| List credit notes | GET `/api/billing/credit-notes` | `GetCreditNotesQuery` → `GetCreditNotesResponse` |
| Get credit note | GET `/api/billing/credit-notes/{id:guid}` | `GetCreditNoteByIdQuery` → `GetCreditNoteByIdResponse` |
| Update credit note | PUT `/api/billing/credit-notes/{id:guid}` | `UpdateCreditNoteCommand` + route ID → `UpdateCreditNoteResponse` |
| Post credit note | POST `/api/billing/credit-notes/{id:guid}/post` | route ID only → `PostCreditNoteResponse` |
| Cancel credit note | POST `/api/billing/credit-notes/{id:guid}/cancel` | route ID only → `CancelCreditNoteResponse` |
| Get billing configuration | GET `/api/billing/configuration` | `GetBillingConfigurationQuery` → `GetBillingConfigurationResponse` |
| Update billing configuration | PUT `/api/billing/configuration` | `UpdateBillingConfigurationCommand` → `UpdateBillingConfigurationResponse` |
| Create payment method | POST `/api/billing/payment-methods` | `CreatePaymentMethodCommand` → `CreatePaymentMethodResponse` |
| List payment methods | GET `/api/billing/payment-methods` | `GetPaymentMethodsQuery` → `GetPaymentMethodsResponse` |
| Get payment method | GET `/api/billing/payment-methods/{id:guid}` | `GetPaymentMethodByIdQuery` → `GetPaymentMethodByIdResponse` |
| Update payment method | PUT `/api/billing/payment-methods/{id:guid}` | `UpdatePaymentMethodCommand` + route ID → `UpdatePaymentMethodResponse` |
| Delete payment method | DELETE `/api/billing/payment-methods/{id:guid}` | route ID command → deletion response |
| Activate payment method | POST `/api/billing/payment-methods/{id:guid}/activate` | route ID command → response |
| Deactivate payment method | POST `/api/billing/payment-methods/{id:guid}/deactivate` | route ID command → response |
| Create number series | POST `/api/billing/number-series` | `CreateNumberSeriesCommand` → response |
| List number series | GET `/api/billing/number-series` | `GetNumberSeriesQuery` → response |
| Get number series | GET `/api/billing/number-series/{id:guid}` | `GetNumberSeriesByIdQuery` → response |
| Update number series | PUT `/api/billing/number-series/{id:guid}` | `UpdateNumberSeriesCommand` + route ID → response |
| Delete number series | DELETE `/api/billing/number-series/{id:guid}` | route ID command → deletion response |

All routes above are prefixed `/api/billing`. **Printing:** both print handlers reuse `GetInvoiceByIdQuery` / `GetQuotationByIdQuery` and return HTML (`text/html; charset=utf-8`) from `BillingPrintDocument`; they are not PDF endpoints in the current implementation. This is inconsistent with a parent-level expectation of QuestPDF/PDF if that expectation remains applicable; see `API-Contract-Gaps.md`.

Source: `Features/Billing/BillingEndpointExtensions.cs`, operation endpoint files, `Common/BillingContracts.cs`, `Common/BillingPrintDocument.cs`; integration tests `BillingEndpointTests.cs`.

### Reports — `/api/reports`

Report routes are mapped without a group-level `.RequireAuthorization()` in `ReportsEndpointExtensions.cs`; each route explicitly requires `Reports.View`. CSV endpoints use the corresponding same query and CSV formatter. Reports are read-only and backed by Dapper.

| Method + route | Query / output |
|---|---|
| GET `/sales-summary` | `GetSalesSummaryQuery` → `PagedReportResponse<SalesSummaryRowResponse>` (verify exact paging/filter fields in query type) |
| GET `/sales-summary/export.csv` | same query → `text/csv` download |
| GET `/stock-valuation` | `GetStockValuationQuery` → paged valuation rows |
| GET `/stock-valuation/export.csv` | same query → CSV |
| GET `/low-stock` | `GetLowStockQuery` → paged low-stock rows |
| GET `/low-stock/export.csv` | same query → CSV |
| GET `/stock-movement` | `GetStockMovementQuery` → paged movement rows |
| GET `/stock-movement/export.csv` | same query → CSV |
| GET `/customer-aging` | `GetCustomerAgingQuery` → paged aging rows/bucket totals |
| GET `/customer-aging/export.csv` | same query → CSV |

Source: `Features/Reports/**`, `Common/Reports/CsvReportFormatter.cs`, `Infrastructure/Reports/DapperReportReadService.cs`; tests `ReportsEndpointTests.cs`, `ReportsRelationalQueryTests.cs`, `ReportContractTests.cs`.

## Contract references and exact field discovery

The endpoint inventory deliberately points to the named, compiled request/response records rather than duplicate their definitions in prose. Exact field names, C# types and nullability are captured in `Frontend-TypeScript-Contracts.ts`; source-file links below are the definitive schema locations.

| Area | Contract source roots | Verification suite |
|---|---|---|
| Master Data | `src/JTradeERP.Application/Features/MasterData/**`; shared Domain refs in `src/JTradeERP.Domain/MasterData/**` | `MasterDataEndpointTests.cs`, `MasterDataContractTests.cs`, `MasterDataFeatureTests.cs`, `MasterDataModelTests.cs` |
| Security/Admin | `src/JTradeERP.Application/Features/Security/**` | `AuthenticationAndAuthorizationTests.cs`, `AuditLogEndpointTests.cs` |
| Inventory | `src/JTradeERP.Application/Features/Inventory/**`, `Common/Inventory/**` | Inventory endpoint/model tests |
| Purchasing | `src/JTradeERP.Application/Features/Purchasing/**` | Purchasing endpoint/model tests |
| Billing | `src/JTradeERP.Application/Features/Billing/**` | `BillingEndpointTests.cs` |
| Reports | `src/JTradeERP.Application/Features/Reports/**`, `Common/Reports/**` | report endpoint, relational, contract tests |

## Response and state notes

- Validation error body is RFC ProblemDetails shape with `status`, `title`, `instance`, `traceId`, plus `errors` for validation failures.
- Not-found response is 404; conflict and SQL uniqueness/FK/concurrency failures map to 409; unexpected errors return 500 without exposing exception details.
- Route IDs for state transitions are the only required input unless the endpoint signature also binds a command body (e.g. stock adjustment posting can carry unit cost).
- Invoice/quotation printing returns rendered HTML, not a generated binary file. Reports CSV endpoints return CSV; no file-upload endpoint was identified in mapped route source.
- No general client-selectable sorting contract was found; use the deterministic server ordering implemented by each handler/read service.
- Operation-specific state transitions and retries are source-controlled by handlers/domain rules. A repeated POST is not automatically idempotent unless its operation implements an idempotency/source-document guard.
