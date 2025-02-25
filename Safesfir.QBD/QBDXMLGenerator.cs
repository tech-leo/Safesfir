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

        public XmlDocument GetXmlDocument(string Node, string iterator = null, string maxReturned = null, string iteratorId = null)
        {
            XmlDocument inputXMLDoc;
            XmlElement qbXMLMsgsRq;
            BaseXML(out inputXMLDoc, out qbXMLMsgsRq);
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
            if (Node?.Contains("InvoiceQueryRq") == true ||Node?.Contains("ReceivePaymentQueryRq") == true)
            {
                XmlElement cust1AddRqChild = inputXMLDoc.CreateElement("IncludeLineItems");
                cust1AddRqChild.InnerText = "true";
                custAddRq.AppendChild(cust1AddRqChild);
            }

            return inputXMLDoc;
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
                ("IncludeRetElement","DueDate"),
                ("OwnerID","0"),
                };
                    if (Node?.Contains("InvoiceQueryRq") == true)
                    {
                        foreach (var item in invoicerets)
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

        public void AppendElements(XmlDocument xmlDoc, XmlElement el, List<string> list, string element)
        {
            if (list != null && list.Count > 0)
                foreach (var i in list)
                {
                    if (element == "NameFilter" || element == "RefNumberFilter")
                    {
                        XmlElement elem = xmlDoc.CreateElement(element);
                        el.AppendChild(elem);

                        AppendElement(xmlDoc, elem, "Contains", "MatchCriterion");
                        AppendElement(xmlDoc, elem, i, element == "NameFilter" ? "Name" : element == "RefNumberFilter" ? "RefNumber" : "Name");
                    }
                    else
                    {
                        XmlElement elem = xmlDoc.CreateElement(element);
                        elem.InnerText = i;
                        el.AppendChild(elem);
                    }
                }
        }

        public void AppendElement(XmlDocument xmlDoc, XmlElement el, string value, string element)
        {
            if (element == "NameFilter" || element == "RefNumberFilter")
            {
                XmlElement elem = xmlDoc.CreateElement(element);
                el.AppendChild(elem);

                AppendElement(xmlDoc, elem, "Contains", "MatchCriterion");
                AppendElement(xmlDoc, elem, value, element == "NameFilter" ? "Name" : element == "RefNumberFilter" ? "RefNumber" : "Name");
            }
            else
            {
                XmlElement elem = xmlDoc.CreateElement(element);
                elem.InnerText = value;
                el.AppendChild(elem);
            }


            if (el?.Name?.Contains("ReceivePaymentQueryRq") == true)
            {
                XmlElement elem = xmlDoc.CreateElement("IncludeLineItems");
                elem.InnerText = "true";
                el.AppendChild(elem);

                //elem = xmlDoc.CreateElement("IncludeRetElement");
                //elem.InnerText = "true";
                //el.AppendChild(elem);
            }
        }

        public string AddNewData(string Node, string json = null, int requestId = 0)
        {
            var str = Newtonsoft.Json.JsonConvert.DeserializeXmlNode(json);
            XmlDocument inputXMLDoc = GetAddXMLDOC(Node, str, requestId);

            return inputXMLDoc.OuterXml;
        }

        private XmlDocument GetAddXMLDOC(string doc, XmlDocument xmlElem, int requestId)
        {
            XmlDocument inputXMLDoc;
            XmlElement qbXMLMsgsRq;
            BaseXML(out inputXMLDoc, out qbXMLMsgsRq);
            XmlElement custAddRq = inputXMLDoc.CreateElement(doc);
            qbXMLMsgsRq.AppendChild(custAddRq);
            if (requestId > 0)
                custAddRq.SetAttribute("requestID", requestId.ToString());


            foreach (XmlNode item in xmlElem.FirstChild.SelectNodes("*"))
            {
                if (item.InnerXml == String.Empty)
                {
                    item.ParentNode.RemoveChild(item);
                }
                else if (item.ChildNodes.Count > 0)
                {
                    foreach (XmlNode childitem in item.SelectNodes("*"))
                    {
                        if (childitem.InnerXml == String.Empty)
                        {
                            childitem.ParentNode.RemoveChild(childitem);
                        }
                    }
                    if (item.InnerXml == String.Empty)
                    {
                        item.ParentNode.RemoveChild(item);
                    }
                }
            }
            XmlNode importNode = custAddRq.OwnerDocument.ImportNode(xmlElem.FirstChild, true);

            custAddRq.AppendChild(importNode);
            return inputXMLDoc;
        }

        public XmlElement InsertNode(XmlDocument inputXMLDoc, XmlElement qbXMLMsgsRq, string Node, int? requestId, string iterator = null, string maxReturned = null, string iteratorId = null)
        {
            XmlElement custAddRq = inputXMLDoc.CreateElement(Node);
            qbXMLMsgsRq.AppendChild(custAddRq);
            custAddRq.SetAttribute("requestID", requestId?.ToString() ?? "1");

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

            return custAddRq;
        }

        public async Task AddNewData(XmlDocument inputXMLDoc, XmlElement qbXMLMsgsRq, string Node, int requestId, string json)
        {
            var jsonXml = (XmlDocument)Newtonsoft.Json.JsonConvert.DeserializeXmlNode(json);
            var xmlElem = InsertNode(inputXMLDoc, qbXMLMsgsRq, Node, requestId);

            foreach (XmlNode item in jsonXml.FirstChild.SelectNodes("*"))
            {
                if (item.InnerXml == String.Empty)
                {
                    item.ParentNode.RemoveChild(item);
                }
                else if (item.ChildNodes.Count > 0)
                {
                    foreach (XmlNode childitem in item.SelectNodes("*"))
                    {
                        if (childitem.InnerXml == String.Empty)
                        {
                            childitem.ParentNode.RemoveChild(childitem);
                        }
                    }
                    if (item.InnerXml == String.Empty)
                    {
                        item.ParentNode.RemoveChild(item);
                    }
                }
            }

            XmlNode importNode = xmlElem.OwnerDocument.ImportNode(jsonXml.FirstChild, true);

            xmlElem.AppendChild(importNode);
        }

        public void AddNewData(XmlDocument inputXMLDoc, XmlElement qbXMLMsgsRq, string Node, int requestId, string value, string column)
        {
            var xmlElem = InsertNode(inputXMLDoc, qbXMLMsgsRq, Node, requestId);
            AppendElement(inputXMLDoc, xmlElem, value, column);
        }
    }
}
