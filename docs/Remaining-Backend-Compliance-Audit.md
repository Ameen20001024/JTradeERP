# Remaining Backend Compliance Audit

Audit date: 2026-09-29. The report records final repository evidence for ADM-02, RPT-01 and REL-01. SQL Server integration tests use only `JTradeERP_MD01_IntegrationTests`.

## ADM-02 acceptance

| Criterion | Result | Evidence |
|---|---|---|
| ADM-AC-001 | PASS | `docs/Parent-SRS-Traceability.md` maps FR-ADM-001–009; `AuthenticationAndAuthorizationTests.cs`, `AuditLogEndpointTests.cs`, and `BillingEndpointTests.cs`. |
| ADM-AC-002 | PASS | Existing ARP-01/BIL-01 users, roles, permissions, number series and audit infrastructure reused; no duplicate subsystem introduced. |
| ADM-AC-003 | PASS | `SecuritySeeds_ExactlyMatchAllPublishedRolePermissionCapabilityCells`; `DatabasePermissionChanges_ApplyToExistingJwt_AndAuditorRemainsReadOnly`; audit/reports read-only authorization tests. |
| ADM-AC-004 | PASS | `ConcurrentInvoicePosts_AllocateOnlyOneNumberAndIssueAvailableStockOnce`; `NumberSeries_ReadEndpointExposesConfiguredFieldsWithMatrixPermissions`; SQL-backed `SqlNumberSeriesAllocator`. |
| ADM-AC-005 | PASS | Billing audit behavior records aggregate before/after state; `AuditLogs_AuditorCanReadAndFilterButCannotMutate` and `AuditLogs_RequireAuditLogViewPermission`. |
| ADM-AC-006 | PASS | Final full regression: 116 passed, 0 failed, 0 skipped. |

## RPT-01 acceptance

| Criterion | Result | Evidence |
|---|---|---|
| RPT-AC-001 | PASS | Sales Summary uses posted invoice/line snapshots; cancelled invoices excluded; `ReportsRelationalQueryTests`. |
| RPT-AC-002 | PASS | Stock Valuation reads StockBalances quantity × persisted Weighted Average Cost; relational assertion verifies value. |
| RPT-AC-003 | PASS | Customer Aging Dapper query reconstructs AsOfDate outstanding from posted invoices, payment allocations, payment creation/payment dates and reversal timestamps; approved buckets verified at ages 0/1/30/31/60/61/90/91 and future invoice-date. |
| RPT-AC-004 | PASS | Low Stock uses `QtyOnHand - QtyReserved`; SQL integration checks reservation and release results. |
| RPT-AC-005 | PASS | Stock Movement returns original and reversal entries with source/reversal references; ledger remains append-only. |
| RPT-AC-006 | PASS | Supported report filters are implemented and parameterized: dates, customer/product, warehouse/category and movement/source dimensions where available. |
| RPT-AC-007 | PASS | Allow-listed sorting and deterministic paging; Customer Aging integration verifies adjacent pages, stable ordering and exact filtered CSV values. CSV is the configured export. |
| RPT-AC-008 | PASS | `DapperReportReadService` contains parameterized read SQL; `DapperReportImplementation_IsInInfrastructureOnly` passes. |
| RPT-AC-009 | PASS | Report endpoints require `Reports.View`; Auditor reads; anonymous and unauthorized users are denied; no report command routes. |
| RPT-AC-010 | PASS | Full SQL Server regression: 72 integration tests passed, 0 failed, 0 skipped. |

Customer Aging uses `InvoiceDate` because Billing has no authoritative invoice due-date/payment-term date. Age is calculated from the query's required `AsOfDate`: `<=0`, `1–30`, `31–60`, `61–90`, `91+`. The query includes activity effective by the end of that date and excludes later invoice posting, payment creation/payment dates, and reversals; a cancellation after the as-of date does not erase the historical receivable. No DueDate field or schema change was introduced.

## REL-01 verification

| Criterion | Result | Evidence |
|---|---|---|
| REL-AC-001 | PASS | `docs/Parent-SRS-Traceability.md` has 110 individual FR/user-story evidence rows and linked concise summaries; all 110 rows are PASS. |
| REL-AC-002 | PASS | SQL tests cover GRN, transfer and invoice forced-failure rollback; inventory RowVersion conflict; concurrent GRNs, invoice numbering and payment allocation; retry/idempotency and reversal behavior. |
| REL-AC-003 | PASS | 401/403/authorized cases, exact permission matrix, audit immutability/correlation, validation and ProblemDetails tests pass. |
| REL-AC-004 | PASS | Integration fixture safely recreates only the dedicated test database and applies the complete existing migration chain; `dotnet ef migrations has-pending-model-changes` reports no model changes. |
| REL-AC-005 | PASS | Scope scan found no Angular, GL, multi-currency, barcode, batch/serial/expiry, HR/payroll or manufacturing implementation. Customer Aging is a read-only projection only. |
| REL-AC-006 | PASS | Restore succeeded; build has 0 warnings/0 errors; complete test suite has 0 failed/0 skipped. |
| REL-AC-007 | PASS | All preceding criteria are PASS; backend is complete for the defined initial-release scope. |

## Final verification record

| Check | Result |
|---|---|
| Restore | `dotnet restore JTradeERP.sln -m:1 --verbosity minimal` — succeeded, all projects up to date. |
| Build | `dotnet build JTradeERP.sln --no-restore -m:1 --verbosity minimal` — succeeded, 0 warnings, 0 errors. |
| Architecture tests | 4 passed, 0 failed, 0 skipped. |
| Application unit tests | 40 passed, 0 failed, 0 skipped. |
| API integration tests | 72 passed, 0 failed, 0 skipped; SQL Server provider and existing migrations. |
| Complete suite | 116 passed, 0 failed, 0 skipped. |
| Migration/model drift | No changes since the last migration. No reporting migration was created. |
| Development database/config | Test reset guard allows only `JTradeERP_MD01_IntegrationTests`; `appsettings.Development.json` and `JTradeERP` were not modified. |
| Credentials/scope | No real SQL credentials or production signing secrets found in source configuration; no out-of-scope module was introduced. |

