using System.Collections.Specialized;
using System.Xml;

namespace RocketGateClient;

public class GatewayResponse
{
	private readonly NameValueCollection responseParams;

//////////////////////////////////////////////////////////////////////
//
//		GatewayResponse() - Constructor for class.
//
//////////////////////////////////////////////////////////////////////
//
	public GatewayResponse()
	{
		responseParams = new NameValueCollection();
	}

//////////////////////////////////////////////////////////////////////
//
//		Reset() - Reset the response parameters so that
//				  the object can be reused.
//
//////////////////////////////////////////////////////////////////////
//
	public void Reset()
	{
		responseParams.Clear();		// Empty the list
	}


//////////////////////////////////////////////////////////////////////
//
//		Get() - Gat a value in the parameters collection.
//
//////////////////////////////////////////////////////////////////////
//
	public string Get(string key)
	{
		return responseParams.Get(key);	// Extract and return value
	}


//////////////////////////////////////////////////////////////////////
//
//		GetInteger() - Gat a value in the parameters collection.
//
//////////////////////////////////////////////////////////////////////
//
	public int GetInteger(string key, int defaultValue)
	{
		string value;				// Value from collection
		int returnValue;			// Decoded value

//
//		Extract and prepare the parameter string.
//
		if ((value = Get(key)) == null) return defaultValue;
		value = value.Trim();				// Remove white-space
		if (value.Equals("")) return defaultValue;

//
//		Convert the parameter to an integer.
//
		try {								// Trap parse failures
			returnValue = Convert.ToInt32(value);
		}

//
//		If the conversion failed, return the default value.
//
		catch {
			returnValue = defaultValue;		// Failed to convert
		}
		return returnValue;					// Give back final value
	}


//////////////////////////////////////////////////////////////////////
//
//		GetInteger() - Gat a value in the parameters collection.
//
//////////////////////////////////////////////////////////////////////
//
	public int GetInteger(string key)
	{
		return GetInteger(key, -1);	// Use default of -1
	}


//////////////////////////////////////////////////////////////////////
//
//		GetResponseCode() - Return the <responseCode> element
//							from the hashtable.
//
//////////////////////////////////////////////////////////////////////
//
	public int GetResponseCode()
	{
		return GetInteger(RESPONSE_CODE);
	}


//////////////////////////////////////////////////////////////////////
//
//		Set() - Set a value in the parameters collection.
//
//////////////////////////////////////////////////////////////////////
//
	public void Set(string key, string value)
	{
		responseParams.Remove(key);		// Remove old value
		if (value != null) responseParams.Add(key, value);
	}


//////////////////////////////////////////////////////////////////////
//
//		Set() - Set a value in the parameters collection.
//
//////////////////////////////////////////////////////////////////////
//
	public void Set(string key, int value)
	{
		Set(key, value.ToString());
	}


//////////////////////////////////////////////////////////////////////
//
//		SetResults() - Set the response and reason codes in
//					   a response packet.
//
//////////////////////////////////////////////////////////////////////
//
	public void SetResults(int response, int reason)
	{
//
//		Set the reason and response codes in the result packet.
//
		Set(RESPONSE_CODE, response);
		Set(REASON_CODE, reason);
	}


//////////////////////////////////////////////////////////////////////
//
//      SetFromXMLString() - Set the internals of the Hashtable
//							 using an XML document carried in
//							 a string.
//
//////////////////////////////////////////////////////////////////////
//
	public void SetFromXMLString(string xmlString)
	{
		XmlDocument document;		// Response as XML document
		XmlNode responseNode;		// <gatewayResponse> document
		XmlNodeList childNodeList;	// Elements beneath <gatewayResponse>

//
//		Parse the XML string into a document.
//
		try {
			document = new XmlDocument();	// Create the empty document
			document.LoadXml(xmlString);	// Load the document
		}

//
//		If we experienced a parsing error, setup a return
//		response.
//
		catch(Exception ex) {				// Parsing problem?
			Set(EXCEPTION, ex.Message);
			SetResults(GatewayCodes.RESPONSE_SYSTEM_ERROR,
				GatewayCodes.REASON_XML_ERROR);
			return;							// And quit
		}

//
//		Find the base of the <gatewayResponse> element in the
//		input document.
//
		responseNode = document.SelectSingleNode(
			DOCUMENT_BASE);
		if (responseNode == null) {			// Missing document?
			SetResults(GatewayCodes.RESPONSE_SYSTEM_ERROR,
				GatewayCodes.REASON_XML_ERROR);
			return;					// And we're done
		}

//
//		Loop through the nodes beneath <gatewayResponse> and
//		transfer there contents to the response list.
//
		childNodeList = responseNode.ChildNodes;
		foreach (XmlNode childNode in childNodeList)
			Set(childNode.LocalName, childNode.InnerText);
	}	


//////////////////////////////////////////////////////////////////////
//
//		ToXMLString() - Convert the internals of the request
//						into an XML string.
//
//////////////////////////////////////////////////////////////////////
//
	public string ToXMLString()
	{
		string hashValue;			// Value from hash table
		string xmlString;			// Generated XML String

//
//		Build the header of XML document.
//
		xmlString = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
		            "<gatewayResponse>";

//
//		Loop over the list of key values and extract the
//		associated hash table entry.
//
		foreach (string keyValue in responseParams.AllKeys) {
			hashValue = responseParams.Get(keyValue);
			if (hashValue != null) {		// Have a value?

//
//		Add the key value and it's hash table entry to
//		the return XML string.
//
				xmlString = string.Concat(xmlString, "<");
				xmlString = string.Concat(xmlString, keyValue);
				xmlString = string.Concat(xmlString, ">");
				xmlString = string.Concat(xmlString,
					TranslateXML(hashValue));
				xmlString = string.Concat(xmlString, "</");
				xmlString = string.Concat(xmlString, keyValue);
				xmlString = string.Concat(xmlString, ">");
			}
		}
				
//
//		Add the trailer to the XML document.
//
		xmlString = string.Concat(xmlString, "</gatewayResponse>");
		return xmlString;					// Return final XML
	}


//////////////////////////////////////////////////////////////////////
//
//		TranslateXML() - Translate a string to a valid XML
//						 string that can be used in an
//						 attribute or text node.
//
//////////////////////////////////////////////////////////////////////
//
	private string TranslateXML(string sourceString)
	{
		string resultString;

//
//      Replace all &, >, and < symbols to the XML equivalents.
//
		resultString = sourceString.Replace("&", "&amp;");
		resultString = resultString.Replace("<", "&lt;");
		resultString = resultString.Replace(">", "&gt;");
		resultString = resultString.Replace("\"", "&quot;");
		resultString = resultString.Replace("\'", "&apos;");
		return resultString;				// Return the clean string
	}


//////////////////////////////////////////////////////////////////////
//
//		Definition of constant key values in response.
//
//////////////////////////////////////////////////////////////////////
//
	private static readonly string DOCUMENT_BASE = "/gatewayResponse";
    public static readonly string VERSION = "version";
    public static readonly string AUTH_NO = "authNo";
    public static readonly string AVS_RESPONSE = "avsResponse";
    public static readonly string CVV2_CODE = "cvv2Code";
    public static readonly string EXCEPTION = "exception";
    public static readonly string MERCHANT_ACCOUNT = "merchantAccount";
    public static readonly string REASON_CODE = "reasonCode";
    public static readonly string RESPONSE_CODE = "responseCode";
    public static readonly string TRANSACT_ID = "guidNo";
    public static readonly string TRANSACTION_TIME = "transactionTime";
    public static readonly string MERCHANT_ADVICE_CODE = "merchantAdviceCode";
    public static readonly string SCRUB_RESULTS = "scrubResults";
    public static readonly string SETTLED_AMOUNT = "approvedAmount";
    public static readonly string SETTLED_CURRENCY = "approvedCurrency";
    public static readonly string CARD_TYPE = "cardType";
    public static readonly string CARD_HASH = "cardHash";
    public static readonly string CARD_LAST_FOUR= "cardLastFour";
    public static readonly string CARD_EXPIRATION = "cardExpiration";
    public static readonly string CARD_COUNTRY = "cardCountry";
    public static readonly string CARD_ISSUER_NAME = "cardIssuerName";
    public static readonly string CARD_ISSUER_PHONE = "cardIssuerPhone";
    public static readonly string CARD_ISSUER_URL = "cardIssuerURL";
    public static readonly string CARD_REGION = "cardRegion";
    public static readonly string CARD_DESCRIPTION = "cardDescription";
    public static readonly string CARD_DEBIT_CREDIT = "cardDebitCredit";
    public static readonly string CARD_BIN = "cardBin";
    public static readonly string BILLING_ADDRESS = "billingAddress";
    public static readonly string BILLING_CITY = "billingCity";
    public static readonly string BILLING_COUNTRY = "billingCountry";
    public static readonly string BILLING_STATE = "billingState";
    public static readonly string BILLING_ZIPCODE = "billingZipCode";
    public static readonly string CUSTOMER_FIRSTNAME = "customerFirstName";
    public static readonly string CUSTOMER_LASTNAME = "customerLastName";
    public static readonly string EMAIL = "email";
    public static readonly string ROCKETPAY_INDICATOR = "rocketPayIndicator";
    public static readonly string PAY_TYPE = "payType";
    public static readonly string PAY_HASH = CARD_HASH;
    public static readonly string PAY_LAST_FOUR = CARD_LAST_FOUR;
    public static readonly string ACS_URL= "acsURL";
    public static readonly string PAREQ = "PAREQ";
    public static readonly string CAVV_RESPONSE = "cavvResponse";
    public static readonly string REBILL_END_DATE = "rebillEndDate";
    public static readonly string REBILL_DATE = "rebillDate";
    public static readonly string REBILL_AMOUNT = "rebillAmount";
    public static readonly string REBILL_CURRENCY = "rebillCurrency";
    public static readonly string REBILL_FREQUENCY = "rebillFrequency";
    public static readonly string LAST_BILLING_DATE = "lastBillingDate";
    public static readonly string LAST_BILLING_AMOUNT = "lastBillingAmount";
    public static readonly string JOIN_DATE = "joinDate";
    public static readonly string JOIN_AMOUNT = "joinAmount";
    public static readonly string REBILL_STATUS = "rebillStatus";
    public static readonly string LAST_REASON_CODE = "lastReasonCode";
    public static readonly string BALANCE_AMOUNT = "balanceAmount";
    public static readonly string BALANCE_CURRENCY = "balanceCurrency";
    public static readonly string MERCHANT_SITE_ID = "merchantSiteID";
    public static readonly string MERCHANT_PRODUCT_ID = "merchantProductID";
    public static readonly string MERCHANT_CUSTOMER_ID = "merchantCustomerID";
    public static readonly string MERCHANT_INVOICE_ID = "merchantInvoiceID";
    public static readonly string SCHEME_TRANSACTION_ID = "schemeTransactionID";
    public static readonly string SCHEME_SETTLEMENT_DATE = "schemeSettlementDate";
    public static readonly string IOVATION_TRACKING_NO = "IOVATIONTRACKINGNO";
    public static readonly string IOVATION_DEVICE = "IOVATIONDEVICE";
    public static readonly string IOVATION_RESULTS = "IOVATIONRESULTS";
    public static readonly string IOVATION_SCORE = "IOVATIONSCORE";
    public static readonly string IOVATION_RULE_COUNT = "IOVATIONRULECOUNT";
    public static readonly string IOVATION_RULE_TYPE_ = "IOVATIONRULETYPE_";
    public static readonly string IOVATION_RULE_REASON_ = "IOVATIONRULEREASON_";
    public static readonly string IOVATION_RULE_SCORE_ = "IOVATIONRULESCORE_";
    public static readonly string BILLING_DURATION = "billingDuration";
    public static readonly string BILLING_METHOD = "billingMethod";
    public static readonly string BILLING_WINDOW = "billingWindow";
    public static readonly string CARRIER_LIST = "carrierList";
    public static readonly string CARRIER_NETWORK = "carrierNetwork";
    public static readonly string MESSAGE_COUNT = "messageCount";
    public static readonly string MSISDN = "msisdn";
    public static readonly string PROMPT_TIMEOUT = "promptTimeout";
    public static readonly string SHORT_CODE = "shortCode";
    public static readonly string USER_AMOUNT = "userAmount";
    public static readonly string USER_CURRENCY = "userCurrency";
    public static readonly string IS_BANK_HARD_DECLINE = "isBankHardDecline";
    public static readonly string _3DSECURE_DEVICE_COLLECTION_JWT = "_3DSECURE_DEVICE_COLLECTION_JWT";
    public static readonly string _3DSECURE_DEVICE_COLLECTION_URL = "_3DSECURE_DEVICE_COLLECTION_URL";
    public static readonly string _3DSECURE_STEP_UP_URL = "_3DSECURE_STEP_UP_URL";
    public static readonly string _3DSECURE_STEP_UP_JWT = "_3DSECURE_STEP_UP_JWT";
    public static readonly string _3DSECURE_VERSION = "_3DSECURE_VERSION";
    public static readonly string _3DSECURE_CHALLENGE_INDICATOR = "_3DSECURE_CHALLENGE_INDICATOR";
    public static readonly string PAYMENT_LINK_URL = "PAYMENT_LINK_URL";
    public static readonly string PROCESSOR_3DS = "PROCESSOR3DS";
    public static readonly string PARES = "PARES";
    public static readonly string _3DSECURE_DS_TRANSACTION_ID = "_3DSECURE_DS_TRANSACTION_ID";
    public static readonly string _3DSECURE_PARESSTATUS = "_3DSECURE_PARESSTATUS";
    public static readonly string _3DSECURE_CAVV_UCAF = "_3DSECURE_CAVV_UCAF";
    public static readonly string _3DSECURE_CAVV_ALGORITHM = "_3DSECURE_CAVV_ALGORITHM";
    public static readonly string _3DSECURE_LOOKUP_SIGNATURE = "_3DSECURE_LOOKUP_SIGNATURE";
    public static readonly string _3DSECURE_XID = "_3DSECURE_XID";
    public static readonly string _3DSECURE_ACS_TRANSACTION_ID = "_3DSECURE_ACS_TRANSACTION_ID";
    public static readonly string _3DSECURE_THREE_DS_SERVER_TRANSACTION_ID = "_3DSECURE_THREE_DS_SERVER_TRANSACTION_ID";
    public static readonly string _3DSECURE_LOOKUP_CHALLENGE_INDICATOR = "_3DSECURE_LOOKUP_CHALLENGE_INDICATOR";
    public static readonly string _3DSECURE_CHALLENGE_MANDATED_INDICATOR = "_3DSECURE_CHALLENGE_MANDATED_INDICATOR";
    public static readonly string _3DSECURE_VERSTATUS = "_3DSECURE_VERSTATUS";
    public static readonly string _3DSECURE_LOOKUP_REFERENCE_GUID = "_3D_LOOKUP_REFERENCE_GUID";
    public static readonly string PAN_TOKEN = "panToken";
}