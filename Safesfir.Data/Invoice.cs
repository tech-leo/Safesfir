using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Safesfir.Data
{
    
    public class CustomerRef
    {
        [BsonElement("ListID")]
        public string ListID { get; set; }

        [BsonElement("FullName")]
        public string FullName { get; set; }
    }

    public class ClassRef
    {
        [BsonElement("ListID")]
        public string ListID { get; set; }

        [BsonElement("FullName")]
        public string FullName { get; set; }
    }

    public class ARAccountRef
    {
        [BsonElement("ListID")]
        public string ListID { get; set; }

        [BsonElement("FullName")]
        public string FullName { get; set; }
    }

    public class CustomerSalesTaxCodeRef
    {
        [BsonElement("ListID")]
        public string ListID { get; set; }

        [BsonElement("FullName")]
        public string FullName { get; set; }
    }


    public class DataExtModRs
    {
        [BsonElement("OwnerID")]
        public string OwnerID { get; set; }

        [BsonElement("DataExtName")]
        public string FullName { get; set; }
        [BsonElement("TxnDataExtType")]
        public string TxnDataExtType { get; set; }
        
        [BsonElement("TxnID")]
        public string TxnID { get; set; }

        [BsonElement("DataExtValue")]
        public string DataExtValue { get; set; }
    }

    public class Invoice
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        [BsonElement("TxnID")]
        public string TxnID { get; set; }

        [BsonElement("TimeCreated")]
        public DateTime? TimeCreated { get; set; }

        [BsonElement("TimeModified")]
        public DateTime? TimeModified { get; set; }

        [BsonElement("EditSequence")]
        public string EditSequence { get; set; }

        [BsonElement("TxnNumber")]
        public string TxnNumber { get; set; }
        [BsonElement("DueDate")]
        public string DueDate { get; set; }
        [BsonElement("RefNumber")]
        public string RefNumber { get; set; }

        [BsonElement("CustomerRef")]
        public CustomerRef CustomerRef { get; set; }

        [BsonElement("ClassRef")]
        public ClassRef ClassRef { get; set; }

        [BsonElement("ARAccountRef")]
        public ARAccountRef ARAccountRef { get; set; }

        [BsonElement("TxnDate")]
        public DateTime? TxnDate { get; set; }

        [BsonElement("Subtotal")]
        public decimal? Subtotal { get; set; }

        [BsonElement("SalesTaxTotal")]
        public decimal? SalesTaxTotal { get; set; }

        [BsonElement("BalanceRemaining")]
        public decimal? BalanceRemaining { get; set; }

        [BsonElement("Memo")]
        public string Memo { get; set; }

        [BsonElement("Phone")]
        public string Phone { get; set; }


        [BsonElement("ShipDate")]
        public string ShipDate { get; set; }

        [BsonElement("IsPaid")]
        public bool? IsPaid { get; set; }
        [BsonElement("BillAddress")]
        public InvoiceAddress BillAddress { get; set; }

        [BsonElement("ShipAddress")]
        public InvoiceAddress ShipAddress { get; set; }

        [BsonElement("ShipMethodRef")]
        public ShipMethodRef ShipMethodRef { get; set; }
        [BsonElement("TermsRef")]
        public TermsRef TermsRef { get; set; }

        [BsonElement("LinkedTxn")]
        [JsonConverter(typeof(SingleValueArrayConverter<LinkedTxn>))]
        public List<LinkedTxn> LinkedTxn { get; set; } = new List<LinkedTxn>();

        [BsonElement("InvoiceLineRet")]
        [JsonConverter(typeof(SingleValueArrayConverter<InvoiceLineRet>))]
        public List<InvoiceLineRet> InvoiceLineRet { get; set; } = new List<InvoiceLineRet>(); 

        [BsonElement("DataExtRet")]
        [JsonConverter(typeof(SingleValueArrayConverter<DataExtRet>))]
        public List<DataExtRet> DataExtRet { get; set; } = new List<DataExtRet>();
    }

    public class ShipMethodRef
    {
        public string ListID { get; set; }
        public string FullName { get; set; }
    }
    public class CustomerRet
    {
        public string ListID { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Cc { get; set; }
    }
    public class InvoiceAddress
    {
        [BsonElement("Addr1")] 
        public string Addr1 { get; set; }
        [BsonElement("Addr2")] 
        public string Addr2 { get; set; }
        [BsonElement("Addr3")] 
        public string Addr3 { get; set; }
        [BsonElement("Addr4")] 
        public string Addr4 { get; set; }
        [BsonElement("Addr5")] 
        public string Addr5 { get; set; }
        [BsonElement("City")] 
        public string City { get; set; }
        [BsonElement("State")] 
        public string State { get; set; }
        [BsonElement("PostalCode")] 
        public string PostalCode { get; set; }
        [BsonElement("Country")] 
        public string Country { get; set; }
        [BsonElement("Note")] 
        public string Note { get; set; }
    }
    public class TermsRef
    {
        public string ListID { get; set; }
        public string FullName { get; set; }
    }
    public class DataExtRet
    {
        public string OwnerID { get; set; }
        public string DataExtName { get; set; }
        public string DataExtType { get; set; }
        public string DataExtValue { get; set; }
    }
    [BsonIgnoreExtraElements] // ✅ Ignore unexpected fields like _id
    public class DriverInvoice
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string uniqueId { get; set; } = ObjectId.GenerateNewId().ToString();

        [BsonElement("Id")]
        public string? Id { get; set; }
        [BsonElement("domain")]
        public string? domain { get; set; }

        [BsonElement("Memo")]
        public string? Memo { get; set; }

        [BsonElement("CustomField")]
        public List<CustomField> CustomField { get; set; } = new List<CustomField>();

        [BsonElement("TotalAmt")]
        public decimal? TotalAmt { get; set; }

        [BsonElement("DocNumber")]
        public string? DocNumber { get; set; }

        [BsonElement("TxnDate")]
        public string? TxnDate { get; set; }

        [BsonElement("status")]
        public string? status { get; set; }

        [BsonElement("Balance")]
        public decimal? Balance { get; set; }

        [BsonElement("DueDate")]
        public string? DueDate { get; set; }

        [BsonElement("paymentDetails")]
        public InvoicePayment? paymentDetails { get; set; }
        [BsonElement("CustomerRef")]
        public CustmerRef? CustomerRef { get; set; }
        [BsonElement("Terms")]
        public string? Terms { get; set; }
        [BsonElement("BillEmail")]
        public EmailAddress? BillEmail { get; set; }


        [BsonElement("BillEmailCc")]
        public EmailAddress? BillEmailCc { get; set; }


    }
    public class EmailAddress
    {
        [BsonElement("Address")]
        public string Address { get; set; }

    }
    public class CustmerRef
    {
        public string name { get; set; }
        public string value { get; set; }
    }
    public class CustomField
    {


        [BsonElement("DefinitionId")]
        public string DefinitionId { get; set; }

        [BsonElement("Name")]
        public string Name { get; set; }

        [BsonElement("Type")]
        public string Type { get; set; }

        [BsonElement("StringValue")]
        public string StringValue { get; set; }
    }
    public class SingleValueArrayConverter<T> : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.StartArray)
            {
                return serializer.Deserialize<List<T>>(reader);
            }
            else
            {
                var obj = serializer.Deserialize<T>(reader);
                return new List<T>(new[] { obj });
            }
        }
        public override bool CanConvert(Type objectType)
        {
            return true;
        }
    }
    public class InvoiceQueryRs
    {
        [BsonElement("RequestID")]
        public string RequestID { get; set; }

        [BsonElement("StatusCode")]
        public string StatusCode { get; set; }

        [BsonElement("StatusMessage")]
        public string StatusMessage { get; set; }

        [BsonElement("InvoiceRet")]
        public List<Invoice> InvoiceRet { get; set; } = new List<Invoice>();
    }

    public class LinkedTxn
    {
        [BsonElement("TxnID")]
        public string TxnID { get; set; }

        [BsonElement("TxnLineID")]
        public string TxnLineID { get; set; }
    }

    public class InvoiceLineRet
    {
        [BsonElement("TxnLineID")]
        public string TxnLineID { get; set; }

        [BsonElement("ItemRef")]
        public ItemRef ItemRef { get; set; }

        [BsonElement("Desc")]
        public string Desc { get; set; }

        [BsonElement("Quantity")]
        public decimal? Quantity { get; set; }

        [BsonElement("Rate")]
        public decimal? Rate { get; set; }

        [BsonElement("Amount")]
        public decimal? Amount { get; set; }

        [BsonElement("UnitOfMeasure")]
        public decimal? UnitOfMeasure { get; set; }
    }

    public class ItemRef
    {
        [BsonElement("ListID")]
        public string ListID { get; set; }

        [BsonElement("FullName")]
        public string FullName { get; set; }
    }

    public class InvoiceAdd
    {
        [BsonElement("CustomerRef")]
        public CustomerRef CustomerRef { get; set; }

        [BsonElement("TxnDate")]
        public DateTime? TxnDate { get; set; }

        [BsonElement("RefNumber")]
        public string RefNumber { get; set; }

        [BsonElement("Subtotal")]
        public decimal? Subtotal { get; set; }

        [BsonElement("SalesTaxTotal")]
        public decimal? SalesTaxTotal { get; set; }

        [BsonElement("Memo")]
        public string Memo { get; set; }

        [BsonElement("InvoiceLineAdd")]
        public List<InvoiceLineRet> InvoiceLineAdd { get; set; } = new List<InvoiceLineRet>();
    }

    public class InvoiceAddRq
    {
        [BsonElement("InvoiceAdd")]
        public InvoiceAdd InvoiceAdd { get; set; }
    }

    public class InvoiceAddRs
    {
        [BsonElement("InvoiceRet")]
        public Invoice InvoiceRet { get; set; }

        [BsonElement("StatusCode")]
        public string StatusCode { get; set; }

        [BsonElement("StatusMessage")]
        public string StatusMessage { get; set; }

        [BsonElement("RequestID")]
        public string RequestID { get; set; }
    }



}
