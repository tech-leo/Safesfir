using CoreWCF;

namespace QBWCService
{
    [ServiceContract(Namespace = "http://developer.intuit.com/")]
    [XmlSerializerFormat]
    public interface IQBWebService
    {
        [OperationContract(Action = "http://developer.intuit.com/serverVersion")]
        string serverVersion();

        [OperationContract(Action = "http://developer.intuit.com/clientVersion")]
        string clientVersion(string productVersion);

        [OperationContract(Action = "http://developer.intuit.com/authenticate")]
        Task<List<string>> authenticate(string strUserName, string strPassword);

        [OperationContract(Action = "http://developer.intuit.com/sendRequestXML")]
        Task<string> sendRequestXML(string ticket, string strHCPResponse, string strCompanyFileName, string qbXMLCountry, int qbXMLMajorVers, int qbXMLMinorVers);

        [OperationContract(Action = "http://developer.intuit.com/receiveResponseXML")]
        Task<int> receiveResponseXML(string ticket, string response, string hresult, string message);

        [OperationContract(Action = "http://developer.intuit.com/closeConnection")]
        Task<string> closeConnection(string ticket);
    }

}
