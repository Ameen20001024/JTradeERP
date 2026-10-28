namespace JTradeERP.Domain.Billing;

public enum QuotationStatus { Draft = 1, Sent = 2, Accepted = 3, Rejected = 4, Expired = 5, Cancelled = 6 }
public enum SalesOrderStatus { Draft = 1, Posted = 2, PartiallyFulfilled = 3, Fulfilled = 4, Cancelled = 5 }
public enum InvoiceStatus { Draft = 1, Posted = 2, PartiallyPaid = 3, Paid = 4, Cancelled = 5 }
public enum CreditNoteStatus { Draft = 1, Posted = 2, Cancelled = 3 }
public enum PaymentStatus { Posted = 1, Reversed = 2 }
