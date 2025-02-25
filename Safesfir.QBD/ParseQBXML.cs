using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml;

namespace Safesfir.QBD
{

    public class ParseQBXML<T>
    {
        public T ParseString(string obj, bool single = false)
        {
            if (!obj.Contains("xml") && !obj.Contains("<QBXML"))
            {
                throw new Exception("Response is not XML. " + obj);

            }
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(obj);
            string jsonText = JsonConvert.SerializeXmlNode(doc.SelectSingleNode("QBXML")?.FirstChild?.FirstChild);
            if (single == true)
            {
                jsonText = JsonConvert.SerializeXmlNode(doc.SelectSingleNode("QBXML")?.FirstChild);

            }
            try
            {
                return JsonConvert.DeserializeObject<T>(jsonText);
            }
            catch (Exception ex)
            {
                return JsonConvert.DeserializeObject<T>(jsonText);
            }
        }

        public T ParseJson(XElement invoiceRet)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeXNode(invoiceRet, Newtonsoft.Json.Formatting.Indented, true));
            }
            catch
            {
                return JsonConvert.DeserializeObject<T>("");
            }
        }

        public List<T> GetQueryRets(XElement xmlDoc, string descendant)
        {
            var list = new List<T>();
            foreach (XElement ret in xmlDoc.Descendants(descendant))
            {
                try
                {
                    var retRes = JsonConvert.DeserializeObject<T>(JsonConvert.SerializeXNode(ret, Newtonsoft.Json.Formatting.Indented, true));
                    list.Add(retRes);
                }
                catch
                { }
            }

            return list ?? new();
        }
    }
}
