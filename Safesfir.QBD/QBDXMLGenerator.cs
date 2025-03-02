using System.Xml;
using System.Xml.Linq;

namespace Safesfir.QBD
{

    public class QBDXMLGenerator 
    {
        public XmlElement GetXmlElement(XmlDocument doc, string node)
        {
            return (XmlElement)doc.SelectSingleNode($"/QBXML/QBXMLMsgsRq/{node}");
        }

        private void BaseXML(out XmlDocument inputXMLDoc, out XmlElement qbXMLMsgsRq)
        {
            inputXMLDoc = new XmlDocument();
            inputXMLDoc.AppendChild(inputXMLDoc.CreateXmlDeclaration("1.0", null, null));
            inputXMLDoc.AppendChild(inputXMLDoc.CreateProcessingInstruction("qbxml", "version=\"14.0\""));
            XmlElement qbXML = inputXMLDoc.CreateElement("QBXML");
            inputXMLDoc.AppendChild(qbXML);
            qbXMLMsgsRq = inputXMLDoc.CreateElement("QBXMLMsgsRq");
            qbXML.AppendChild(qbXMLMsgsRq);
            qbXMLMsgsRq.SetAttribute("onError", "continueOnError");
        }

        
        public XmlDocument GetMultipleXmlDocument(List<string> Nodes,  List<(string, string)> values, string iterator = null, string maxReturned = null, string iteratorId = null)
        {
            XmlDocument inputXMLDoc;
            XmlElement qbXMLMsgsRq;
            BaseXML(out inputXMLDoc, out qbXMLMsgsRq);

            if (Nodes?.Contains("DataExtModRq") == true)
            {


                foreach (var item in values)
                {
                    XmlElement custmAddmaonRqChild = inputXMLDoc.CreateElement("DataExtModRq");
                    qbXMLMsgsRq.AppendChild(custmAddmaonRqChild);

                    XmlElement custmAddRqChild = inputXMLDoc.CreateElement("DataExtMod");
                    custmAddmaonRqChild.AppendChild(custmAddRqChild);

                    XmlElement cust1AddRqChild = inputXMLDoc.CreateElement("OwnerID");
                    cust1AddRqChild.InnerText = "0";
                    custmAddRqChild.AppendChild(cust1AddRqChild);

                    XmlElement cust2AddRqChild = inputXMLDoc.CreateElement("DataExtName");
                    cust2AddRqChild.InnerText = "SignedInvoice";
                    custmAddRqChild.AppendChild(cust2AddRqChild);

                    XmlElement cust3AddRqChild = inputXMLDoc.CreateElement("TxnDataExtType");
                    cust3AddRqChild.InnerText = "Invoice";
                    custmAddRqChild.AppendChild(cust3AddRqChild);

                    XmlElement cust4AddRqChild = inputXMLDoc.CreateElement("TxnID");
                    cust4AddRqChild.InnerText = item.Item1;
                    custmAddRqChild.AppendChild(cust4AddRqChild);

                    XmlElement cust5AddRqChild = inputXMLDoc.CreateElement("DataExtValue");
                    cust5AddRqChild.InnerText = item.Item2;
                    custmAddRqChild.AppendChild(cust5AddRqChild);
                }
            }
            foreach (var Node in Nodes)
            {
                if (Node != "DataExtModRq")
                    {
                    XmlElement custAddRq = inputXMLDoc.CreateElement(Node);
                    qbXMLMsgsRq.AppendChild(custAddRq);
                    custAddRq.SetAttribute("requestID", "1");

                    if (Node?.Contains("EstimateQueryRq") == true || Node?.Contains("ItemeQueryRq") == true || Node?.Contains("InvoiceQueryRq") == true || Node?.Contains("CustomerQueryRq") == true)
                    {
                        if (!string.IsNullOrEmpty(iterator))
                            custAddRq.SetAttribute("iterator", iterator);
                        if (!string.IsNullOrEmpty(iteratorId))
                            custAddRq.SetAttribute("iteratorID", iteratorId);
                        if (!string.IsNullOrEmpty(maxReturned))
                        {
                            XmlElement custAddRqChild = inputXMLDoc.CreateElement("MaxReturned");
                            custAddRqChild.InnerText = maxReturned;
                            custAddRq.AppendChild(custAddRqChild);
                        }

                    }
                    List<(string, string)> invoicerets = new() {
                ("IncludeLineItems","true"),
                ("IncludeRetElement","InvoiceLineRet"),
                ("IncludeRetElement","TxnID"),
                ("IncludeRetElement","RefNumber"),
                ("IncludeRetElement","CustomerRef"),
                ("IncludeRetElement","TxnDate"),
                ("IncludeRetElement","DueDate"),
                ("IncludeRetElement","Subtotal"),
                ("IncludeRetElement","BalanceRemaining"),
                ("IncludeRetElement","DataExtRet"),
                ("IncludeRetElement","BillAddress"),
                ("IncludeRetElement","ShipAddress"),
                ("IncludeRetElement","IsPending"),
                ("IncludeRetElement","PONumber"),
                ("IncludeRetElement","TermsRef"),
                ("IncludeRetElement","IsPaid"),
                ("IncludeRetElement","CustomerMsgRef"),
                ("IncludeRetElement","IsTaxIncluded"),
                ("IncludeRetElement","LinkedTxn"),
                ("IncludeRetElement","ShipDate"),
                ("IncludeRetElement","AppliedAmount"),
                ("IncludeRetElement","SalesTaxTotal"),
                ("IncludeRetElement","ItemSalesTaxRef"),
                ("IncludeRetElement","Memo"),
                ("OwnerID","0"),
                };
                    if (Node?.Contains("InvoiceQueryRq") == true)
                    {
                        XmlElement mdateAddRqChild = inputXMLDoc.CreateElement("ModifiedDateRangeFilter");
                        XmlElement startdateAddRqChild = inputXMLDoc.CreateElement("FromModifiedDate");
                        startdateAddRqChild.InnerText = DateTime.UtcNow.AddDays(-1).ToString("yyyy-MM-dd"); 
                        mdateAddRqChild.AppendChild(startdateAddRqChild);
                        XmlElement enddateAddRqChild = inputXMLDoc.CreateElement("ToModifiedDate");
                        enddateAddRqChild.InnerText = DateTime.UtcNow.AddDays(2).ToString("yyyy-MM-dd");
                        mdateAddRqChild.AppendChild(enddateAddRqChild);

                        custAddRq.AppendChild(mdateAddRqChild);

                        foreach (var item in invoicerets)
                        {
                            XmlElement cust1AddRqChild = inputXMLDoc.CreateElement(item.Item1);
                            cust1AddRqChild.InnerText = item.Item2;
                            custAddRq.AppendChild(cust1AddRqChild);
                        }
                    }

                    List<(string, string)> customerrets = new() {
                ("FromModifiedDate",DateTime.UtcNow.AddDays(-1).ToString("yyyy-MM-dd")),
                ("ToModifiedDate",DateTime.UtcNow.AddDays(2).ToString("yyyy-MM-dd")),
                ("IncludeRetElement","ListID"),
                ("IncludeRetElement","Email"),
                ("IncludeRetElement","Cc"),
                ("IncludeRetElement","Phone"),
                ("OwnerID","0"),
                };

                    if (Node?.Contains("CustomerQueryRq") == true)
                    {
                        foreach (var item in customerrets)
                        {
                            XmlElement cust1AddRqChild = inputXMLDoc.CreateElement(item.Item1);
                            cust1AddRqChild.InnerText = item.Item2;
                            custAddRq.AppendChild(cust1AddRqChild);
                        }
                    }
                }
            }

            return inputXMLDoc;
        }

      


       

        
    }
}
