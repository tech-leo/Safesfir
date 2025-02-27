using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace Safesfir.Data
{

    [BsonIgnoreExtraElements] // ✅ Ignore unexpected fields like _id

    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = null!;

        [BsonElement("name")]
        public string? Name { get; set; }

        [BsonElement("email")]
        public string? Email { get; set; }

        [BsonElement("role")]
        [BsonRepresentation(BsonType.String)]
        public string? Role { get; set; } = "Driver";

        [BsonElement("profileImage")]
        public string? ProfileImage { get; set; }

        [BsonElement("auth0Id")]
        public string? Auth0Id { get; set; }


        [BsonElement("lastLogin")]
        public DateTime? LastLogin { get; set; }

        [BsonElement("quickbooks")]
        public QuickBooksInfo? QuickBooks { get; set; } = new QuickBooksInfo();

        [BsonElement("QuickBooksCompany")]
        public Company? QuickBooksCompany { get; set; } = new Company();

        [BsonElement("DriverInvoice")]  
        public List<DriverInvoice>? DriverInvoice { get; set; } = new List<DriverInvoice>();


        [BsonElement("invoicePayments")]
        public List<InvoicePayment>? InvoicePayments { get; set; } = new List<InvoicePayment>();
        [BsonElement("Invoices")]
        public List<Invoice?>? Invoices { get; set; } = new List<Invoice>();

        [BsonElement("time")]
        [BsonDefaultValue(typeof(DateTime))]
        public DateTime? Time { get; set; } = DateTime.UtcNow;
    }

    // Enum for user roles
    public enum UserRole
    {
        Admin,
        Driver
    }

    // QuickBooks information
    [BsonIgnoreExtraElements] // ✅ Ignore unexpected fields like _id

    public class QuickBooksInfo
    {
        [BsonElement("type")]
        [BsonRepresentation(BsonType.String)]
        public QuickBooksType? Type { get; set; }

        // QBO fields
        [BsonElement("connected")]
        public bool Connected { get; set; } = false;

        [BsonElement("accessToken")]
        public string? AccessToken { get; set; }

        [BsonElement("refreshToken")]
        public string? RefreshToken { get; set; }

        [BsonElement("realmId")]
        public string? RealmId { get; set; }

        [BsonElement("tokenExpiryDate")]
        public DateTime? TokenExpiryDate { get; set; }

        // QBWD fields
        [BsonElement("qbwdUsername")]
        public string? QbwdUsername { get; set; }

        [BsonElement("qbwdPassword")]
        public string? QbwdPassword { get; set; }

        [BsonElement("qbwdCompany")]
        public string? QbwdCompany { get; set; }

        [BsonElement("qbwdError")]
        public string? QbwdError { get; set; }

        [BsonElement("qbwdLastSync")]
        public DateTime? QbwdLastSync { get; set; }
    }

    // Enum for QuickBooks Type
    public enum QuickBooksType
    {
        QBO,
        QBWD
    }

    // Invoice Payment Model
    [BsonIgnoreExtraElements] // ✅ Ignore unexpected fields like _id

    public class InvoicePayment
    {
        [BsonElement("invoiceId")]
        public string? InvoiceId { get; set; } = null!;

        [BsonElement("paymentMode")]
        [BsonRepresentation(BsonType.String)]
        public string? PaymentMode { get; set; }

        [BsonElement("cashCollected")]
        public decimal? CashCollected { get; set; }

        [BsonElement("chequeNumber")]
        public string? ChequeNumber { get; set; }

        [BsonElement("status")]
        [BsonRepresentation(BsonType.String)]
        public string? Status { get; set; } = "Pending";

        [BsonElement("signedPdfUrl")]
        public string? SignedPdfUrl { get; set; }


        [BsonElement("quickbooksPaymentUpdated")]
        public bool? QuickbooksPaymentUpdated { get; set; }

        [BsonElement("paymentHistory")]
        public List<PaymentHistory>? PaymentHistory { get; set; } = new List<PaymentHistory>();

        [BsonElement("time")]
        public DateTime? Time { get; set; } = DateTime.UtcNow;
    }

    // Enums for Payment
    public enum PaymentMode
    {
        Cash,
        Cheque,
        PayLater,
        OnlinePayment
    }

    public enum PaymentStatus
    {
        Pending,
        Partial,
        Completed
    }

    // Payment History Model
    [BsonIgnoreExtraElements] // ✅ Ignore unexpected fields like _id

    public class PaymentHistory
    {
        [BsonElement("amount")]
        public decimal? Amount { get; set; }

        [BsonElement("paymentMode")]
        [BsonRepresentation(BsonType.String)]
        public PaymentMode? PaymentMode { get; set; }

        [BsonElement("chequeNumber")]
        public string? ChequeNumber { get; set; }

        [BsonElement("quickbooksPaymentId")]
        public string? QuickBooksPaymentId { get; set; }

        [BsonElement("date")]
        public DateTime? Date { get; set; } = DateTime.UtcNow;
    }

}
