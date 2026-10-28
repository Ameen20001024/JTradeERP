# Frontend-to-API Mapping (FE-00)

This is a verified backend capability map to likely React screens. The React Frontend SRS was not available in the repository, so screen names are **provisional implementation groupings**, not authoritative UI requirements. Do not treat an absent route as approved scope without checking that SRS.

| Frontend area / screen | User operations | Verified API routes | Frontend notes |
|---|---|---|---|
| Sign in / session | login, refresh, logout, current user | `POST /api/auth/login`; `POST /api/auth/refresh`; `POST /api/auth/logout`; `GET /api/auth/me` | Refresh uses a cookie; client must follow backend cookie/CORS/SameSite settings. Store access-token handling according to security design. |
| Account recovery | request reset, submit OTP/new password | `POST /api/auth/forgot-password`; `POST /api/auth/reset-password` | Exact response message/anti-enumeration behavior is server-defined. |
| User administration | create, search/filter, inspect, change active status, assign roles | `POST /api/admin/users`; `GET /api/admin/users`; `GET /api/admin/users/{id}`; `PUT /api/admin/users/{id}/status`; `PUT /api/admin/users/{id}/roles` | Permissions are `Admin.Users.Manage:Full`. |
| Role and permission administration | create/update roles, search/list, assign permissions, inspect permission catalog | `POST /api/admin/roles`; `GET /api/admin/roles`; `GET /api/admin/roles/{id}`; `PUT /api/admin/roles/{id}`; `PUT /api/admin/roles/{id}/permissions`; `GET /api/admin/permissions` | Permissions are `Admin.Roles.Manage:Full`. |
| Audit history | filter/page audit events | `GET /api/admin/audit-logs` | Read-only; use query fields from `GetAuditLogsQuery`. |
| Master Data maintenance | CRUD/lifecycle pages for Category, UoM, TaxCode, Product, Customer, Supplier, Warehouse | `/api/master-data/categories*`; `/units-of-measure*`; `/tax-codes*`; `/products*`; `/customers*`; `/suppliers*`; `/warehouses*` | Exact operation list and DTOs in API inventory. No server-side paging/search exposed for these list routes. Customer credit limit nullable; Product name required per current contract. |
| Stock overview | balances, ledger, reorder alerts | `GET /api/inventory/stock-balances`; `GET /api/inventory/stock-balances/{productId}/{warehouseId}`; `GET /api/inventory/stock-ledger`; `GET /api/inventory/reorder-alerts` | Apply only supported filters and page fields; server-defined ordering. |
| Opening stock | create opening balance, reverse it | `POST /api/inventory/opening-balances`; `POST /api/inventory/opening-balances/{id}/reverse` | Route ID only on reversal; posting changes stock and ledger atomically. |
| Stock adjustment | create/edit draft, inspect/list, post, reverse | `POST /api/inventory/adjustments`; `PUT /api/inventory/adjustments/{id}`; `GET /api/inventory/adjustments`; `GET /api/inventory/adjustments/{id}`; `POST /api/inventory/adjustments/{id}/post`; `POST /api/inventory/adjustments/{id}/reverse` | Post may accept unit-cost body; authorization requires stock adjust/post capabilities. |
| Stock transfer | create/edit, list/inspect, post | `POST /api/inventory/transfers`; `PUT /api/inventory/transfers/{id}`; `GET /api/inventory/transfers`; `GET /api/inventory/transfers/{id}`; `POST /api/inventory/transfers/{id}/post` | Posting creates paired stock movements; no API to reverse a posted transfer was found. |
| Negative-stock policy | set warehouse-level option | `PUT /api/inventory/warehouses/{id}/negative-stock` | Restricted configuration permission; boolean request field. |
| Purchasing setup | view/update over-receipt setting | `GET /api/purchasing/configuration/over-receipt`; `PUT /api/purchasing/configuration/over-receipt` | Capability is declared in endpoints. |
| Purchase orders | create, edit, filter/list, inspect | `POST /api/purchasing/purchase-orders`; `PUT /api/purchasing/purchase-orders/{id}`; `GET /api/purchasing/purchase-orders`; `GET /api/purchasing/purchase-orders/{id}` | Draft PO; no dedicated PO post/cancel route exists in mapped API. Receipt is via GRN. |
| Goods receipts | create/edit, list/inspect, post, cancel | `POST /api/purchasing/goods-receipt-notes`; `PUT /api/purchasing/goods-receipt-notes/{id}`; `GET /api/purchasing/goods-receipt-notes`; `GET /api/purchasing/goods-receipt-notes/{id}`; `POST .../{id}/post`; `POST .../{id}/cancel` | Posting receives stock; do not treat as a client-side-only status update. |
| Quotations | create/edit/list/detail, copy, send, accept/reject/cancel/expire, convert to Sales Order, print | `/api/billing/quotations` and operation routes documented in inventory | Print route returns HTML; browser print is possible, but no PDF contract is exposed. |
| Sales orders | create/edit/list/detail, post, cancel | `/api/billing/sales-orders` and operation routes | Creating/posting can reserve stock according to backend flow; check returned status and balances rather than predicting state in UI. |
| Invoices | create/edit/list/detail, post, cancel, print | `/api/billing/invoices` and operation routes | Posting is stock-impacting and atomic; invoice print returns HTML; TRN/tax totals appear in DTOs where defined. |
| Payments | record with allocations, list/detail, reverse | `POST /api/billing/payments`; `GET /api/billing/payments`; `GET /api/billing/payments/{id}`; `POST /api/billing/payments/{id}/reverse` | Partial allocations are supported; reverse is a separate operation. |
| Credit notes | create/edit/list/detail, post, cancel | `/api/billing/credit-notes` and operation routes | Posting may return stock per reason/domain rules; inspect response and specification. |
| Billing configuration | view/update company TRN | `GET /api/billing/configuration`; `PUT /api/billing/configuration` | Nullable TRN field per command/response. |
| Payment methods | CRUD, activate/deactivate | `/api/billing/payment-methods` and operation routes | Delete may conflict if referenced; prefer deactivate unless API contract/SRS says otherwise. |
| Number series | create/list/detail/update/delete | `/api/billing/number-series` and operation routes | Server owns allocation; clients should not calculate document numbers. |
| Sales Summary report | filter, page, export CSV | `/api/reports/sales-summary`; `/api/reports/sales-summary/export.csv` | Query fields in `GetSalesSummaryQuery`; same filters should be passed to export. |
| Stock Valuation report | filter, page, export CSV | `/api/reports/stock-valuation`; CSV counterpart | Authoritative weighted-average valuation from backend. |
| Customer Aging report | As-of/customer filters, page, CSV | `/api/reports/customer-aging`; CSV counterpart | Approved buckets are Current, 1–30, 31–60, 61–90, 91+; as-of semantics are server-defined. |
| Low Stock report | filter, page, CSV | `/api/reports/low-stock`; CSV counterpart | Available quantity vs reorder/minimum stock. |
| Stock Movement report | date/product/warehouse/source filters, page, CSV | `/api/reports/stock-movement`; CSV counterpart | Read-only Dapper-backed query. |

## Frontend integration conventions

- Build API wrappers around the exact routes; do not add guessed routes for search, dropdowns, deletes, or status changes.
- Bind route-only identifiers in URL and do not send artificial `{ id }` bodies unless the corresponding command requires a body.
- Keep request types distinct from response types; use the command/query records as the input DTOs.
- Use ProblemDetails `errors` for validation display and 409 responses for conflict messaging/reload flows.
- Gate navigation/actions using effective permissions from `/api/auth/me`, but treat server authorization as definitive.
- For list screens, use only server-exposed filter and pagination parameters. Master Data list endpoints currently return full collections.
- For printable documents, render returned HTML through a safe same-origin/new-tab workflow; do not expect a PDF blob.
- For downloads, call report `.csv` routes with the same filter parameters and handle `text/csv` content disposition.

## FE-01 readiness

Backend route discovery is sufficient to begin UI prototyping of existing workflows. Production FE-01 should wait until the React SRS is available and the open items in `API-Contract-Gaps.md`—especially print format, route-level permission semantics and list/search requirements—are accepted.
