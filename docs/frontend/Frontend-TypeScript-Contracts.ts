// Transcribed public Application records; generated from the current C# source.
// Nullable fields use null unions; optional record defaults use optional properties.
// JSON enum values are numeric: no string-enum converter is configured in the API.
export interface ResetPasswordResponse { message: string }
export interface ResetPasswordCommand { email: string; otp: string; newPassword: string }
export interface RefreshAccessTokenResponse { accessToken: string; expiresAt: string }
export interface RefreshAccessTokenCommand { refreshToken: string | null }
export interface LogoutCommand { userId: string | null; refreshToken: string | null }
export interface LoginResponse { accessToken: string; expiresAt: string; user: LoginUserResponse }
export interface LoginUserResponse { id: string; userName: string; email: string; roles: ReadonlyArray<string> }
export interface LoginCommand { userName: string; password: string }
export interface GetMeResponse { id: string; userName: string; email: string; roles: ReadonlyArray<string>; permissions: ReadonlyArray<EffectivePermission> }
export interface GetMeQuery {  }
export interface ForgotPasswordResponse { message: string }
export interface ForgotPasswordCommand { email: string }
export interface UpdateUserStatusCommand { id: string; isActive: boolean }
export interface UpdateUserStatusResponse { id: string; isActive: boolean }
export interface UpdateRoleCommand { id: string; name: string }
export interface UpdateRoleResponse { id: string; name: string }
export interface GetUsersQuery { search?: string | null; isActive?: boolean | null }
export interface UserListItemResponse { id: string; userName: string; email: string; isActive: boolean }
export interface GetUsersResponse { items: ReadonlyArray<UserListItemResponse> }
export interface GetUserByIdQuery { id: string }
export interface GetUserByIdResponse { id: string; userName: string; email: string; isActive: boolean; roles: ReadonlyArray<string> }
export interface GetRolesQuery { search?: string | null }
export interface RoleListItemResponse { id: string; name: string }
export interface GetRolesResponse { items: ReadonlyArray<RoleListItemResponse> }
export interface GetRoleByIdQuery { id: string }
export interface RolePermissionResponse { permissionId: string; code: string; capability: number }
export interface GetRoleByIdResponse { id: string; name: string; permissions: ReadonlyArray<RolePermissionResponse> }
export interface GetPermissionsQuery { search?: string | null }
export interface PermissionItemResponse { id: string; code: string }
export interface GetPermissionsResponse { items: ReadonlyArray<PermissionItemResponse> }
export interface AssignRolesCommand { userId: string; roleIds: ReadonlyArray<string> }
export interface AssignRolesResponse { userId: string; roleIds: ReadonlyArray<string> }
export interface PermissionGrantInput { permissionId: string; capability: number }
export interface AssignPermissionsCommand { roleId: string; grants: ReadonlyArray<PermissionGrantInput> }
export interface AssignPermissionsResponse { roleId: string; grants: ReadonlyArray<PermissionGrantInput> }
export interface CreateRoleCommand { name: string }
export interface CreateRoleResponse { id: string; name: string }
export interface GetAuditLogsQuery { page?: number; pageSize?: number }
export interface AuditLogItemResponse { id: string; actorUserId: string | null; occurredAt: string; action: string; entity: string; entityId: string; oldValue: string | null; newValue: string | null; correlationId: string }
export interface GetAuditLogsResponse { items: ReadonlyArray<AuditLogItemResponse>; page: number; pageSize: number; totalCount: number }
export interface CreateUserCommand { userName: string; email: string; password: string }
export interface CreateUserResponse { id: string; userName: string; email: string; isActive: boolean }
export interface AccessTokenResult { accessToken: string; expiresAt: string }
export interface EffectivePermission { code: string; capability: number }
export interface PermissionRequirement { code: string; capability: number }
export interface StockValuationRowResponse { warehouseId: string; warehouseCode: string; warehouseName: string; productId: string; productCode: string; productName: string; categoryId: string; categoryName: string; qtyOnHand: number; weightedAverageCost: number; inventoryValue: number }
export interface GetStockValuationQuery { warehouseId?: string | null; productId?: string | null; categoryId?: string | null; page?: number; pageSize?: number; sortBy?: string; sortDirection?: string }
export interface PagedReportResponse<T> { items: ReadonlyArray<T>; page: number; pageSize: number; totalCount: number }
export interface StockMovementRowResponse { ledgerEntryId: string; postedAt: string; warehouseId: string; warehouseCode: string; warehouseName: string; productId: string; productCode: string; productName: string; movementType: string; direction: string; quantity: number; unitCost: number; sourceDocumentType: string; sourceDocumentId: string; sourceLineId: string | null; reversalOfLedgerEntryId: string | null; correlationId: string }
export interface GetStockMovementQuery { fromUtc?: string | null; toUtc?: string | null; warehouseId?: string | null; productId?: string | null; movementType?: number | null; sourceDocumentType?: string | null; page?: number; pageSize?: number; sortBy?: string; sortDirection?: string }
export interface UpdateCustomerResponse { id: string; customerCode: string; name: string; billingAddress: string | null; shippingAddress: string | null; phone: string | null; email: string | null; trn: string | null; creditLimit: number | null }
export interface UpdateCustomerCommand { id: string; customerCode: string; name: string; billingAddress: string | null; shippingAddress: string | null; phone: string | null; email: string | null; trn: string | null; creditLimit: number | null }
export interface UpdateWarehouseResponse { id: string; warehouseCode: string; name: string; isActive: boolean }
export interface UpdateWarehouseCommand { id: string; warehouseCode: string; name: string }
export interface SalesSummaryRowResponse { date: string; customerId: string; customerName: string; productId: string; productCode: string; productName: string; subTotal: number; discountAmount: number; netAmount: number; taxAmount: number; grandTotal: number }
export interface GetSalesSummaryQuery { fromDate?: string | null; toDate?: string | null; customerId?: string | null; productId?: string | null; page?: number; pageSize?: number; sortBy?: string; sortDirection?: string }
export interface StockBalanceItemResponse { productId: string; productCode: string; productName: string; warehouseId: string; warehouseCode: string; warehouseName: string; qtyOnHand: number; qtyReserved: number; availableQuantity: number; weightedAverageCost: number; rowVersion: string }
export interface StockLedgerItemResponse { id: string; productId: string; warehouseId: string; movementType: string; direction: string; quantity: number; unitCost: number; sourceDocumentType: string; sourceDocumentId: string; sourceLineId: string | null; reversalOfLedgerEntryId: string | null; postedAt: string; postedByUserId: string; correlationId: string }
export interface ReorderAlertItemResponse { productId: string; productCode: string; productName: string; warehouseId: string; warehouseCode: string; warehouseName: string; qtyOnHand: number; qtyReserved: number; availableQuantity: number; minimumStockLevel: number; reorderQuantity: number }
export interface StockAdjustmentItemResponse { id: string; productId: string; warehouseId: string; quantityDelta: number; unitCost: number | null; reasonCode: string; status: string; postedAt: string | null; reversedAt: string | null }
export interface StockTransferLineItemResponse { id: string; productId: string; quantity: number }
export interface StockTransferItemResponse { id: string; sourceWarehouseId: string; destinationWarehouseId: string; status: string; postedAt: string | null; lines: ReadonlyArray<StockTransferLineItemResponse> }
export interface GetCustomersResponse { id: string; customerCode: string; name: string; billingAddress: string | null; shippingAddress: string | null; phone: string | null; email: string | null; trn: string | null; creditLimit: number | null }
export interface GetCustomersQuery {  }
export interface GetWarehousesResponse { id: string; warehouseCode: string; name: string; isActive: boolean }
export interface GetWarehousesQuery {  }
export interface LowStockRowResponse { warehouseId: string; warehouseCode: string; warehouseName: string; productId: string; productCode: string; productName: string; categoryId: string; categoryName: string; qtyOnHand: number; qtyReserved: number; availableQuantity: number; minimumStockLevel: number; reorderQuantity: number }
export interface GetLowStockQuery { warehouseId?: string | null; productId?: string | null; categoryId?: string | null; page?: number; pageSize?: number; sortBy?: string; sortDirection?: string }
export interface UpdateWarehouseNegativeStockSettingResponse { warehouseId: string; warehouseCode: string; allowNegativeStock: boolean }
export interface UpdateWarehouseNegativeStockSettingCommand { id: string; allowNegativeStock: boolean }
export interface GetCustomerByIdResponse { id: string; customerCode: string; name: string; billingAddress: string | null; shippingAddress: string | null; phone: string | null; email: string | null; trn: string | null; creditLimit: number | null }
export interface GetCustomerByIdQuery { id: string }
export interface GetCustomerAgingQuery { asOfDate: string; customerId?: string | null; page?: number; pageSize?: number; sortBy?: string; sortDirection?: string }
export interface CustomerAgingRowResponse { customerId: string; customerCode: string; customerName: string; asOfDate: string; current: number; days1To30: number; days31To60: number; days61To90: number; days91Plus: number; totalOutstanding: number }
export interface FoundationValidationRequest { value: string }
export interface GetWarehouseByIdResponse { id: string; warehouseCode: string; name: string; isActive: boolean }
export interface GetWarehouseByIdQuery { id: string }
export interface DeleteCustomerCommand { id: string }
export interface DeactivateWarehouseResponse { id: string; warehouseCode: string; name: string; isActive: boolean }
export interface DeactivateWarehouseCommand { id: string }
export interface CreateCustomerResponse { id: string; customerCode: string; name: string; billingAddress: string | null; shippingAddress: string | null; phone: string | null; email: string | null; trn: string | null; creditLimit: number | null }
export interface CreateCustomerCommand { customerCode: string; name: string; billingAddress: string | null; shippingAddress: string | null; phone: string | null; email: string | null; trn: string | null; creditLimit: number | null }
export interface UpdateStockTransferResponse { transfer: StockTransferResponse }
export interface UpdateStockTransferCommand { id: string; sourceWarehouseId: string; destinationWarehouseId: string; lines: ReadonlyArray<StockTransferLineRequest> }
export interface StockTransferLineRequest { productId: string; quantity: number }
export interface StockTransferLineResponse { id: string; productId: string; productCode: string; productName: string; quantity: number }
export interface StockTransferResponse { id: string; sourceWarehouseId: string; sourceWarehouseCode: string; destinationWarehouseId: string; destinationWarehouseCode: string; status: string; postedAt: string | null; lines: ReadonlyArray<StockTransferLineResponse> }
export interface CreateWarehouseResponse { id: string; warehouseCode: string; name: string; isActive: boolean }
export interface CreateWarehouseCommand { warehouseCode: string; name: string }
export interface StockTransferMovementResponse { lineId: string; productId: string; quantity: number; unitCost: number; outLedgerEntryId: string; inLedgerEntryId: string }
export interface PostStockTransferResponse { transfer: StockTransferResponse; movements: ReadonlyArray<StockTransferMovementResponse> }
export interface PostStockTransferCommand { id: string }
export interface UpdatePurchaseOrderResponse { purchaseOrder: PurchaseOrderDetailsResponse }
export interface UpdatePurchaseOrderCommand { id: string; supplierId: string; warehouseId: string; orderDate: string; expectedDate: string | null; lines: ReadonlyArray<PurchaseOrderLineInput> }
export interface UpdateCategoryResponse { id: string; name: string; isActive: boolean }
export interface UpdateCategoryCommand { id: string; name: string }
export interface ActivateWarehouseResponse { id: string; warehouseCode: string; name: string; isActive: boolean }
export interface ActivateWarehouseCommand { id: string }
export interface UpdatePurchasingConfigurationResponse { allowOverReceipt: boolean; rowVersion: string }
export interface UpdatePurchasingConfigurationCommand { allowOverReceipt: boolean }
export interface GetPurchaseOrdersResponse { items: ReadonlyArray<PurchaseOrderSummaryResponse>; pageNumber: number; pageSize: number; totalCount: number }
export interface GetPurchaseOrdersQuery { supplierId?: string | null; warehouseId?: string | null; status?: number | null; pageNumber?: number; pageSize?: number }
export interface GetStockTransfersResponse { items: ReadonlyArray<StockTransferResponse>; page: number; pageSize: number; totalCount: number }
export interface GetStockTransfersQuery { sourceWarehouseId?: string | null; destinationWarehouseId?: string | null; status?: string | null; page?: number; pageSize?: number }
export interface GetCategoryByIdResponse { id: string; name: string; isActive: boolean }
export interface GetCategoryByIdQuery { id: string }
export interface GetPurchasingConfigurationResponse { allowOverReceipt: boolean; rowVersion: string }
export interface GetPurchasingConfigurationQuery {  }
export interface GetPurchaseOrderByIdResponse { purchaseOrder: PurchaseOrderDetailsResponse }
export interface GetPurchaseOrderByIdQuery { id: string }
export interface GetCategoriesResponse { id: string; name: string; isActive: boolean }
export interface GetCategoriesQuery {  }
export interface GetStockTransferResponse { transfer: StockTransferResponse }
export interface GetStockTransferQuery { id: string }
export interface PurchaseOrderLineInput { productId: string; orderedQuantity: number; unitPurchaseCost: number; taxCodeId: string; discountAmount?: number }
export interface GoodsReceiptNoteLineInput { productId: string; purchaseOrderLineId: string | null; receivedQuantity: number; unitCost?: number | null; taxCodeId?: string | null }
export interface PurchaseOrderLineResponse { id: string; productId: string; orderedQuantity: number; receivedQuantity: number; unitPurchaseCost: number; taxCodeId: string; taxRate: number; discountAmount: number; lineSubTotal: number; taxAmount: number; lineTotal: number }
export interface PurchaseOrderDetailsResponse { id: string; supplierId: string; warehouseId: string; orderDate: string; expectedDate: string | null; status: string; subTotal: number; discountAmount: number; taxAmount: number; grandTotal: number; createdAt: string; updatedAt: string; rowVersion: string; lines: ReadonlyArray<PurchaseOrderLineResponse> }
export interface PurchaseOrderSummaryResponse { id: string; supplierId: string; warehouseId: string; orderDate: string; expectedDate: string | null; status: string; grandTotal: number }
export interface GoodsReceiptNoteLineResponse { id: string; purchaseOrderLineId: string | null; productId: string; receivedQuantity: number; unitCost: number; taxCodeId: string; taxRate: number; lineSubTotal: number; taxAmount: number; lineTotal: number }
export interface GoodsReceiptNoteDetailsResponse { id: string; purchaseOrderId: string | null; supplierId: string; warehouseId: string; receiptDate: string; status: string; postedAt: string | null; postedByUserId: string | null; cancelledAt: string | null; cancelledByUserId: string | null; createdAt: string; updatedAt: string; rowVersion: string; lines: ReadonlyArray<GoodsReceiptNoteLineResponse> }
export interface GoodsReceiptNoteSummaryResponse { id: string; purchaseOrderId: string | null; supplierId: string; warehouseId: string; receiptDate: string; status: string; lineCount: number }
export interface CreatePurchaseOrderResponse { purchaseOrder: PurchaseOrderDetailsResponse }
export interface CreatePurchaseOrderCommand { supplierId: string; warehouseId: string; orderDate: string; expectedDate: string | null; lines: ReadonlyArray<PurchaseOrderLineInput> }
export interface DeleteCategoryCommand { id: string }
export interface UpdateUnitOfMeasureResponse { id: string; code: string; name: string }
export interface UpdateUnitOfMeasureCommand { id: string; code: string; name: string }
export interface CreateStockTransferResponse { transfer: StockTransferResponse }
export interface CreateStockTransferCommand { sourceWarehouseId: string; destinationWarehouseId: string; lines: ReadonlyArray<StockTransferLineRequest> }
export interface DeactivateCategoryResponse { id: string; name: string; isActive: boolean }
export interface DeactivateCategoryCommand { id: string }
export interface GetUnitsOfMeasureResponse { id: string; code: string; name: string }
export interface GetUnitsOfMeasureQuery {  }
export interface UpdateGoodsReceiptNoteResponse { goodsReceiptNote: GoodsReceiptNoteDetailsResponse }
export interface UpdateGoodsReceiptNoteCommand { id: string; purchaseOrderId: string | null; supplierId: string; warehouseId: string; receiptDate: string; lines: ReadonlyArray<GoodsReceiptNoteLineInput> }
export interface ReverseOpeningBalanceResponse { openingBalanceId: string; originalLedgerEntryId: string; reversalLedgerEntryId: string; status: string }
export interface ReverseOpeningBalanceCommand { id: string }
export interface CreateCategoryResponse { id: string; name: string; isActive: boolean }
export interface CreateCategoryCommand { name: string }
export interface GetUnitOfMeasureByIdResponse { id: string; code: string; name: string }
export interface GetUnitOfMeasureByIdQuery { id: string }
export interface PostGoodsReceiptNoteResponse { goodsReceiptNote: GoodsReceiptNoteDetailsResponse }
export interface PostGoodsReceiptNoteCommand { id: string }
export interface GetStockLedgerResponse { items: ReadonlyArray<StockLedgerItemResponse>; page: number; pageSize: number; totalCount: number }
export interface GetStockLedgerQuery { productId?: string | null; warehouseId?: string | null; from?: string | null; to?: string | null; sourceDocumentType?: string | null; page?: number; pageSize?: number }
export interface PostOpeningBalanceResponse { id: string; sourceDocumentId: string; productId: string; warehouseId: string; quantity: number; unitCost: number; status: string; postedAt: string }
export interface PostOpeningBalanceCommand { sourceDocumentId: string; productId: string; warehouseId: string; quantity: number; unitCost: number }
export interface ActivateCategoryResponse { id: string; name: string; isActive: boolean }
export interface ActivateCategoryCommand { id: string }
export interface UpdateStockAdjustmentResponse { adjustment: StockAdjustmentItemResponse }
export interface UpdateStockAdjustmentCommand { id: string; productId: string; warehouseId: string; quantityDelta: number; reasonCode: string }
export interface GetStockBalancesResponse { items: ReadonlyArray<StockBalanceItemResponse>; page: number; pageSize: number; totalCount: number }
export interface GetStockBalancesQuery { productId?: string | null; warehouseId?: string | null; page?: number; pageSize?: number }
export interface UpdateTaxCodeResponse { id: string; code: string; name: string; rate: number; taxType: string; isActive: boolean }
export interface UpdateTaxCodeCommand { id: string; code: string; name: string; rate: number; taxType: string }
export interface ReverseStockAdjustmentResponse { adjustment: StockAdjustmentItemResponse; reversalLedgerEntryId: string; originalLedgerEntryId: string }
export interface ReverseStockAdjustmentCommand { id: string }
export interface GetReorderAlertsResponse { items: ReadonlyArray<ReorderAlertItemResponse>; totalCount: number }
export interface GetReorderAlertsQuery { warehouseId?: string | null }
export interface GetStockBalanceResponse { balance: StockBalanceItemResponse }
export interface GetStockBalanceQuery { productId: string; warehouseId: string }
export interface GetTaxCodesResponse { id: string; code: string; name: string; rate: number; taxType: string; isActive: boolean }
export interface GetTaxCodesQuery {  }
export interface PostStockAdjustmentResponse { adjustment: StockAdjustmentItemResponse; ledgerEntryId: string }
export interface PostStockAdjustmentCommand { id: string; unitCost?: number | null }
export interface CreateTaxCodeResponse { id: string; code: string; name: string; rate: number; taxType: string; isActive: boolean }
export interface CreateTaxCodeCommand { code: string; name: string; rate: number; taxType: string; isActive: boolean }
export interface GetTaxCodeByIdResponse { id: string; code: string; name: string; rate: number; taxType: string; isActive: boolean }
export interface GetTaxCodeByIdQuery { id: string }
export interface GetStockAdjustmentsResponse { items: ReadonlyArray<StockAdjustmentItemResponse>; page: number; pageSize: number; totalCount: number }
export interface GetStockAdjustmentsQuery { productId?: string | null; warehouseId?: string | null; status?: string | null; page?: number; pageSize?: number }
export interface DeactivateTaxCodeResponse { id: string; code: string; name: string; rate: number; taxType: string; isActive: boolean }
export interface DeactivateTaxCodeCommand { id: string }
export interface ActivateTaxCodeResponse { id: string; code: string; name: string; rate: number; taxType: string; isActive: boolean }
export interface ActivateTaxCodeCommand { id: string }
export interface UpdateProductResponse { id: string; productCode: string; name: string; categoryId: string; stockUomId: string; taxCodeId: string; minimumStockLevel: number; reorderQuantity: number; salesPrice: number; purchasePrice: number; isActive: boolean }
export interface UpdateProductCommand { id: string; productCode: string; name: string; categoryId: string; stockUomId: string; taxCodeId: string; minimumStockLevel: number; reorderQuantity: number; salesPrice: number; purchasePrice: number }
export interface GetStockAdjustmentResponse { adjustment: StockAdjustmentItemResponse; ledgerEntryId: string | null }
export interface GetStockAdjustmentQuery { id: string }
export interface CreateUnitOfMeasureResponse { id: string; code: string; name: string }
export interface CreateUnitOfMeasureCommand { code: string; name: string }
export interface GetProductsResponse { id: string; productCode: string; name: string; categoryId: string; stockUomId: string; taxCodeId: string; minimumStockLevel: number; reorderQuantity: number; salesPrice: number; purchasePrice: number; isActive: boolean }
export interface GetProductsQuery {  }
export interface CreateGoodsReceiptNoteResponse { goodsReceiptNote: GoodsReceiptNoteDetailsResponse }
export interface CreateGoodsReceiptNoteCommand { purchaseOrderId: string | null; supplierId: string; warehouseId: string; receiptDate: string; lines: ReadonlyArray<GoodsReceiptNoteLineInput> }
export interface CreateStockAdjustmentResponse { adjustment: StockAdjustmentItemResponse }
export interface CreateStockAdjustmentCommand { productId: string; warehouseId: string; quantityDelta: number; reasonCode: string }
export interface GetGoodsReceiptNoteByIdResponse { goodsReceiptNote: GoodsReceiptNoteDetailsResponse }
export interface GetGoodsReceiptNoteByIdQuery { id: string }
export interface GetProductByIdResponse { id: string; productCode: string; name: string; categoryId: string; stockUomId: string; taxCodeId: string; minimumStockLevel: number; reorderQuantity: number; salesPrice: number; purchasePrice: number; isActive: boolean }
export interface GetProductByIdQuery { id: string }
export interface CancelGoodsReceiptNoteResponse { goodsReceiptNote: GoodsReceiptNoteDetailsResponse }
export interface CancelGoodsReceiptNoteCommand { id: string }
export interface CreateProductResponse { id: string; productCode: string; name: string; categoryId: string; stockUomId: string; taxCodeId: string; minimumStockLevel: number; reorderQuantity: number; salesPrice: number; purchasePrice: number; isActive: boolean }
export interface CreateProductCommand { productCode: string; name: string; categoryId: string; stockUomId: string; taxCodeId: string; minimumStockLevel: number; reorderQuantity: number; salesPrice: number; purchasePrice: number }
export interface DeactivateProductResponse { id: string; productCode: string; name: string; isActive: boolean }
export interface DeactivateProductCommand { id: string }
export interface ActivateProductCommand { id: string }
export interface ActivateProductResponse { id: string; productCode: string; name: string; isActive: boolean }
export interface BillingLineInput { productId: string; quantity: number; unitPrice: number; discountAmount: number; taxCodeId: string }
export interface SalesOrderLineInput { productId: string; orderedQuantity: number; unitPrice: number; discountAmount: number; taxCodeId: string; sourceQuotationLineId?: string | null }
export interface BillingLineResponse { id: string; productId: string; quantity: number; unitPrice: number; discountAmount: number; taxCodeId: string; taxRate: number; taxAmount: number; lineTotal: number }
export interface QuotationDocumentResponse { id: string; customerId: string; quotationDate: string; validityDate: string; status: string; subTotal: number; discountAmount: number; taxAmount: number; grandTotal: number; lines: ReadonlyArray<BillingLineResponse> }
export interface SalesOrderDocumentResponse { id: string; sourceQuotationId: string | null; customerId: string; warehouseId: string; orderDate: string; deliveryDate: string | null; deliveryAddress: string | null; status: string; grandTotal: number; lines: ReadonlyArray<SalesOrderLineResponse> }
export interface SalesOrderLineResponse { id: string; productId: string; orderedQuantity: number; reservedQuantity: number; fulfilledQuantity: number; unitPrice: number; taxCodeId: string; taxRate: number; sourceQuotationLineId: string | null }
export interface InvoiceLineInput { productId: string; sourceSalesOrderLineId: string | null; quantity: number; unitPrice: number; discountAmount: number; taxCodeId: string }
export interface InvoiceLineResponse { id: string; productId: string; sourceSalesOrderLineId: string | null; quantity: number; unitPrice: number; discountAmount: number; taxCodeId: string; taxRateSnapshot: number; taxAmountSnapshot: number; lineTotal: number }
export interface InvoiceDocumentResponse { id: string; invoiceNumber: string | null; sourceSalesOrderId: string | null; customerId: string; warehouseId: string; invoiceDate: string; status: string; companyTrnSnapshot: string | null; customerTrnSnapshot: string | null; grandTotal: number; paidAmount: number; outstandingAmount: number; lines: ReadonlyArray<InvoiceLineResponse> }
export interface GetCreditNoteByIdResponse { creditNote: CreateCreditNoteResponse }
export interface GetCreditNoteByIdQuery { id: string }
export interface UpdateCreditNoteResponse { creditNote: CreateCreditNoteResponse }
export interface UpdateCreditNoteCommand { id: string; creditNoteDate: string; lines: ReadonlyArray<CreditNoteLineInput> }
export interface UpdateBillingConfigurationResponse { companyTrn: string | null }
export interface UpdateBillingConfigurationCommand { companyTrn: string | null }
export interface PostCreditNoteResponse { creditNote: CreateCreditNoteResponse }
export interface PostCreditNoteCommand { id: string }
export interface DeleteUnitOfMeasureCommand { id: string }
export interface GetBillingConfigurationResponse { companyTrn: string | null }
export interface GetBillingConfigurationQuery {  }
export interface CreditNoteLineResponse { invoiceLineId: string; productId: string; quantity: number; unitPrice: number; taxRateSnapshot: number; taxAmount: number; lineTotal: number }
export interface CreateCreditNoteResponse { id: string; invoiceId: string; creditNoteDate: string; status: string; grandTotal: number; lines: ReadonlyArray<CreditNoteLineResponse> }
export interface CreditNoteLineInput { invoiceLineId: string; quantity: number }
export interface CreateCreditNoteCommand { invoiceId: string; creditNoteDate: string; lines: ReadonlyArray<CreditNoteLineInput> }
export interface GetGoodsReceiptNotesQuery { purchaseOrderId?: string | null; supplierId?: string | null; warehouseId?: string | null; status?: number | null; pageNumber?: number; pageSize?: number }
export interface GetGoodsReceiptNotesResponse { items: ReadonlyArray<GoodsReceiptNoteSummaryResponse>; pageNumber: number; pageSize: number; totalCount: number }
export interface CancelCreditNoteResponse { creditNote: CreateCreditNoteResponse }
export interface CancelCreditNoteCommand { id: string }
export interface GetCreditNotesResponse { items: ReadonlyArray<CreateCreditNoteResponse>; totalCount: number; page: number; pageSize: number }
export interface GetCreditNotesQuery { invoiceId?: string | null; status?: number | null; page?: number; pageSize?: number }
export interface UpdateNumberSeriesResponse { numberSeries: NumberSeriesResponse }
export interface UpdateNumberSeriesCommand { id: string; documentType: string; prefix: string; nextNumber: number; padding: number }
export interface CreateNumberSeriesResponse { numberSeries: NumberSeriesResponse }
export interface CreateNumberSeriesCommand { documentType: string; prefix: string; nextNumber: number; padding: number }
export interface NumberSeriesResponse { id: string; documentType: string; prefix: string; nextNumber: number; padding: number }
export interface UpdateSalesOrderResponse { salesOrder: SalesOrderDocumentResponse }
export interface UpdateSalesOrderCommand { id: string; customerId: string; warehouseId: string; orderDate: string; deliveryDate: string | null; deliveryAddress: string | null; lines: ReadonlyArray<SalesOrderLineInput> }
export interface GetNumberSeriesByIdResponse { numberSeries: NumberSeriesResponse }
export interface GetNumberSeriesByIdQuery { id: string }
export interface UpdateQuotationResponse { quotation: QuotationDocumentResponse }
export interface UpdateQuotationCommand { id: string; customerId: string; quotationDate: string; validityDate: string; lines: ReadonlyArray<BillingLineInput> }
export interface UpdatePaymentMethodResponse { paymentMethod: PaymentMethodResponse }
export interface UpdatePaymentMethodCommand { id: string; code: string; name: string }
export interface PostSalesOrderResponse { salesOrder: SalesOrderDocumentResponse }
export interface PostSalesOrderCommand { id: string }
export interface SendQuotationResponse { quotation: QuotationDocumentResponse }
export interface SendQuotationCommand { id: string }
export interface PaymentMethodItemResponse { id: string; code: string; name: string; isActive: boolean }
export interface GetPaymentMethodsResponse { items: ReadonlyArray<PaymentMethodItemResponse> }
export interface GetPaymentMethodsQuery { includeInactive?: boolean }
export interface UpdateInvoiceResponse { invoice: InvoiceDocumentResponse }
export interface UpdateInvoiceCommand { id: string; customerId: string; warehouseId: string; invoiceDate: string; lines: ReadonlyArray<InvoiceLineInput> }
export interface RejectQuotationResponse { quotation: QuotationDocumentResponse }
export interface RejectQuotationCommand { id: string }
export interface GetSalesOrdersResponse { items: ReadonlyArray<SalesOrderDocumentResponse>; totalCount: number; page: number; pageSize: number }
export interface GetSalesOrdersQuery { customerId?: string | null; status?: number | null; page?: number; pageSize?: number }
export interface GetPaymentMethodByIdResponse { paymentMethod: PaymentMethodResponse }
export interface GetPaymentMethodByIdQuery { id: string }
export interface ReversePaymentResponse { id: string; status: string; reversedAt: string | null }
export interface ReversePaymentCommand { id: string }
export interface PostInvoiceResponse { invoice: InvoiceDocumentResponse }
export interface PostInvoiceCommand { id: string }
export interface GetSalesOrderByIdResponse { salesOrder: SalesOrderDocumentResponse }
export interface GetSalesOrderByIdQuery { id: string }
export interface GetQuotationsResponse { items: ReadonlyArray<QuotationDocumentResponse>; totalCount: number; page: number; pageSize: number }
export interface GetQuotationsQuery { customerId?: string | null; status?: number | null; page?: number; pageSize?: number }
export interface PaymentAllocationResponse { invoiceId: string; amount: number }
export interface RecordPaymentResponse { id: string; paymentDate: string; paymentMethodId: string; amount: number; referenceNumber: string | null; status: string; allocations: ReadonlyArray<PaymentAllocationResponse> }
export interface PaymentAllocationInput { invoiceId: string; amount: number }
export interface RecordPaymentCommand { paymentDate: string; paymentMethodId: string; amount: number; referenceNumber: string | null; allocations: ReadonlyArray<PaymentAllocationInput> }
export interface DeletePaymentMethodResponse { id: string; deleted: boolean }
export interface DeletePaymentMethodCommand { id: string }
export interface GetInvoicesResponse { items: ReadonlyArray<InvoiceDocumentResponse>; totalCount: number; page: number; pageSize: number }
export interface GetInvoicesQuery { customerId?: string | null; status?: number | null; from?: string | null; to?: string | null; page?: number; pageSize?: number }
export interface GetQuotationByIdResponse { quotation: QuotationDocumentResponse }
export interface GetQuotationByIdQuery { id: string }
export interface CreateSalesOrderResponse { salesOrder: SalesOrderDocumentResponse }
export interface CreateSalesOrderCommand { customerId: string; warehouseId: string; sourceQuotationId: string | null; orderDate: string; deliveryDate: string | null; deliveryAddress: string | null; lines: ReadonlyArray<SalesOrderLineInput> }
export interface PaymentAllocationItemResponse { invoiceId: string; amount: number }
export interface GetPaymentItemResponse { id: string; paymentDate: string; paymentMethodId: string; amount: number; referenceNumber: string | null; status: string; allocations: ReadonlyArray<PaymentAllocationItemResponse> }
export interface GetPaymentsResponse { items: ReadonlyArray<GetPaymentItemResponse>; totalCount: number; page: number; pageSize: number }
export interface GetPaymentsQuery { status?: number | null; from?: string | null; to?: string | null; page?: number; pageSize?: number }
export interface DeactivatePaymentMethodResponse { paymentMethod: PaymentMethodResponse }
export interface DeactivatePaymentMethodCommand { id: string }
export interface GetInvoiceByIdResponse { invoice: InvoiceDocumentResponse }
export interface GetInvoiceByIdQuery { id: string }
export interface ExpireQuotationResponse { quotation: QuotationDocumentResponse }
export interface ExpireQuotationCommand { id: string }
export interface CancelSalesOrderResponse { salesOrder: SalesOrderDocumentResponse }
export interface CancelSalesOrderCommand { id: string }
export interface PaymentAllocationDto { invoiceId: string; amount: number }
export interface GetPaymentByIdResponse { id: string; paymentDate: string; paymentMethodId: string; amount: number; referenceNumber: string | null; status: string; reversedAt: string | null; allocations: ReadonlyArray<PaymentAllocationDto> }
export interface GetPaymentByIdQuery { id: string }
export interface CreatePaymentMethodResponse { paymentMethod: PaymentMethodResponse }
export interface CreatePaymentMethodCommand { code: string; name: string }
export interface CreateInvoiceResponse { invoice: InvoiceDocumentResponse }
export interface CreateInvoiceCommand { customerId: string; warehouseId: string; sourceSalesOrderId: string | null; invoiceDate: string; lines: ReadonlyArray<InvoiceLineInput> }
export interface CreateQuotationResponse { quotation: QuotationDocumentResponse }
export interface CreateQuotationCommand { customerId: string; quotationDate: string; validityDate: string; lines: ReadonlyArray<BillingLineInput> }
export interface AcceptQuotationResponse { quotation: QuotationDocumentResponse }
export interface AcceptQuotationCommand { id: string }
export interface CancelInvoiceResponse { invoice: InvoiceDocumentResponse }
export interface CancelInvoiceCommand { id: string }
export interface GetNumberSeriesResponse { items: ReadonlyArray<NumberSeriesResponse> }
export interface GetNumberSeriesQuery {  }
export interface CopyQuotationResponse { quotation: QuotationDocumentResponse }
export interface CopyQuotationCommand { id: string; quotationDate: string; validityDate: string }
export interface ConvertQuotationToSalesOrderCommand { id: string; warehouseId: string; deliveryDate: string | null; deliveryAddress: string | null }
export interface ConvertQuotationToSalesOrderResponse { salesOrder: SalesOrderDocumentResponse }
export interface DeleteNumberSeriesResponse { id: string; deleted: boolean }
export interface DeleteNumberSeriesCommand { id: string }
export interface CancelQuotationCommand { id: string }
export interface CancelQuotationResponse { quotation: QuotationDocumentResponse }
export interface PaymentMethodResponse { id: string; code: string; name: string; isActive: boolean }
export interface ActivatePaymentMethodResponse { paymentMethod: PaymentMethodResponse }
export interface ActivatePaymentMethodCommand { id: string }
export interface UpdateSupplierResponse { id: string; supplierCode: string; name: string; address: string | null; contactInformation: string | null; paymentTerms: string | null; trn: string | null }
export interface UpdateSupplierCommand { id: string; supplierCode: string; name: string; address: string | null; contactInformation: string | null; paymentTerms: string | null; trn: string | null }
export interface DeleteSupplierCommand { id: string }
export interface GetSupplierByIdResponse { id: string; supplierCode: string; name: string; address: string | null; contactInformation: string | null; paymentTerms: string | null; trn: string | null }
export interface GetSupplierByIdQuery { id: string }
export interface CreateSupplierResponse { id: string; supplierCode: string; name: string; address: string | null; contactInformation: string | null; paymentTerms: string | null; trn: string | null }
export interface CreateSupplierCommand { supplierCode: string; name: string; address: string | null; contactInformation: string | null; paymentTerms: string | null; trn: string | null }
export interface GetSuppliersQuery {  }
export interface GetSuppliersResponse { id: string; supplierCode: string; name: string; address: string | null; contactInformation: string | null; paymentTerms: string | null; trn: string | null }

export enum Capability { None = 0, View = 1, CreateEdit = 2, Post = 3, Full = 4 }
export enum CreditNoteStatus { Draft = 1, Posted = 2, Cancelled = 3 }
export enum GoodsReceiptNoteStatus { Draft = 1, Posted = 2, Cancelled = 3 }
export enum InventoryDocumentStatus { Draft = 1, Posted = 2, Reversed = 3 }
export enum InvoiceStatus { Draft = 1, Posted = 2, PartiallyPaid = 3, Paid = 4, Cancelled = 5 }
export enum PaymentStatus { Posted = 1, Reversed = 2 }
export enum PurchaseOrderStatus { Draft = 1, Open = 2, PartiallyReceived = 3, FullyReceived = 4, Cancelled = 5 }
export enum QuotationStatus { Draft = 1, Sent = 2, Accepted = 3, Rejected = 4, Expired = 5, Cancelled = 6 }
export enum SalesOrderStatus { Draft = 1, Posted = 2, PartiallyFulfilled = 3, Fulfilled = 4, Cancelled = 5 }
export enum StockMovementDirection { In = 1, Out = 2 }
export enum StockMovementType { OpeningBalance = 1, Adjustment = 2, Transfer = 3, Receipt = 4, Issue = 5, Reversal = 6 }
export interface ProblemDetails { type?: string | null; title?: string | null; status?: number | null; detail?: string | null; instance?: string | null; traceId?: string; errors?: Record<string, string[]> }
// Record interfaces: 324; parameter parse misses: 0


