
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace Safesfir.Data
{
    public class Company
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public bool IsSampleCompany { get; set; }
        public string CompanyName { get; set; }
        public string LegalCompanyName { get; set; }
        public Address Address { get; set; }
        public AddressBlock AddressBlock { get; set; }
        public LegalAddress LegalAddress { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        public string CompanyWebSite { get; set; }
        public string FirstMonthFiscalYear { get; set; }
        public string FirstMonthIncomeTaxYear { get; set; }
        public string CompanyType { get; set; }
        public string TaxForm { get; set; }
    }

    public class Address
    {
        public string Addr1 { get; set; }
        public string Country { get; set; }
    }

    public class AddressBlock
    {
        public string Addr1 { get; set; }
    }

    public class LegalAddress
    {
        public string Addr1 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
    }


}
