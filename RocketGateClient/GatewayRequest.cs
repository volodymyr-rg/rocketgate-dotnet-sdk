using System.Collections.Specialized;
using System.Globalization;

namespace RocketGateClient;

public class GatewayRequest
{
	internal static readonly string VERSION_NO = "N6.0";

	private readonly NameValueCollection requestParams;

//////////////////////////////////////////////////////////////////////
//
//		GatewayRequest() - Constructor for class.
//
//////////////////////////////////////////////////////////////////////
//
	public GatewayRequest()
	{
		requestParams = new NameValueCollection();
		Set(VERSION, VERSION_NO);
	}

//////////////////////////////////////////////////////////////////////
//
//		Set() - Set a value in the parameters collection.
//
//////////////////////////////////////////////////////////////////////
//
	public void Set(string key, string value)
	{
		requestParams.Remove(key);		// Remove old value
		if (value != null) requestParams.Add(key, value);
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
//		Get() - Gat a value in the parameters collection.
//
//////////////////////////////////////////////////////////////////////
//
	public string Get(string key)
	{
		return requestParams.Get(key);	// Extract and return value
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
//		GetDouble() - Gat a value in the parameters collection.
//
//////////////////////////////////////////////////////////////////////
//
	public double GetDouble(string key, double defaultValue)
	{
		string value;				// Value from collection
		double returnValue;			// Decoded value

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
			returnValue = double.Parse(value);
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
//		GetDouble() - Gat a value in the parameters collection.
//
//////////////////////////////////////////////////////////////////////
//
	public double GetDouble(string key)
	{
		return GetDouble(key, (double)0.0);
	}


//////////////////////////////////////////////////////////////////////
//
//		GetReferenceGUID() - Extract a reference GUID for a void,
//							 ticket, or credit from the request.
//
//////////////////////////////////////////////////////////////////////
//
	public long GetReferenceGUID()
	{
		string value;				// String from request
		long returnValue;			// Long value returned to caller

//
//      Get the reference GUID string from the request.
//
		value = Get(REFERENCE_GUID);
		if (value == null) return 0;		// Not in request
		if (value.Equals("")) return 0;		// Don't want empty value

//
//		Parse the string into a 64 bit value.
//
		try {
			returnValue = long.Parse(value, NumberStyles.HexNumber);
		}

//
//		If there was an error, just return a zero value.
//
		catch {
			returnValue = 0;				// No usable value
		}
		return returnValue;					// Return converted value
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
		            "<" + DOCUMENT_BASE + ">";

//
//		Loop over the list of key values and extract the
//		associated hash table entry.
//
		foreach (string keyValue in requestParams.AllKeys) {
			hashValue = requestParams.Get(keyValue);
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
		xmlString = string.Concat(xmlString,
			"</", DOCUMENT_BASE, ">");
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
//	Definition of constant key values in request.
//
//////////////////////////////////////////////////////////////////////
//
	private static readonly string DOCUMENT_BASE = "gatewayRequest";
	private static readonly string VERSION = "version";

	public static readonly string ACCT_COMPROMISED_SCRUB = "AcctCompromisedScrub";
	public static readonly string ACCOUNT_HOLDER = "accountHolder";
	public static readonly string ACCOUNT_NO = "accountNo";
	public static readonly string AMOUNT = "amount";
	public static readonly string AFFILIATE = "affiliate";
	public static readonly string AVS_CHECK = "avsCheck";
	public static readonly string BILLING_ADDRESS = "billingAddress";
	public static readonly string BILLING_CITY = "billingCity";
	public static readonly string BILLING_COUNTRY = "billingCountry";
	public static readonly string BILLING_MODE = "billingMode";
	public static readonly string BILLING_STATE = "billingState";
	public static readonly string BILLING_TYPE = "billingType";
	public static readonly string BILLING_WINDOW = "billingWindow";
	public static readonly string BILLING_ZIPCODE = "billingZipCode";
	public static readonly string BROWSER_ACCEPT_HEADER = "browserAcceptHeader";
	public static readonly string BROWSER_USER_AGENT = "browserUserAgent";
	public static readonly string CAPTURE_DAYS = "captureDays";
	public static readonly string CARD_HASH = "cardHash";
	public static readonly string CARDNO = "cardNo";
	public static readonly string CARRIER_CODE = "carrierCode";
	public static readonly string CELLPHONE_NUMBER = "cellPhoneNumber";
	public static readonly string CLONE_CUSTOMER_RECORD = "cloneCustomerRecord";
	public static readonly string CLONE_TO_CUSTOMER_ID = "cloneToCustomerID";
	public static readonly string COUNTRY_CODE = "countryCode";
	public static readonly string CURRENCY = "currency";
	public static readonly string CUSTOMER_FIRSTNAME = "customerFirstName";
	public static readonly string CUSTOMER_LASTNAME = "customerLastName";
	public static readonly string CUSTOMER_PASSWORD = "customerPassword";
	public static readonly string CUSTOMER_PHONE_NO = "customerPhoneNo";
	public static readonly string CVV2 = "cvv2";
	public static readonly string CVV2_CHECK = "cvv2Check";
	public static readonly string EMAIL = "email";
	public static readonly string EXPIRE_MONTH = "expireMonth";
	public static readonly string EXPIRE_YEAR= "expireYear";
	public static readonly string GENERATE_POSTBACK = "generatePostback";
	public static readonly string IOVATION_BLACK_BOX = "iovationBlackBox";
	public static readonly string IOVATION_RULE = "iovationRule";
	public static readonly string IPADDRESS = "ipAddress";
	public static readonly string MERCHANT_ACCOUNT = "merchantAccount";
	public static readonly string MERCHANT_CUSTOMER_ID = "merchantCustomerID";
	public static readonly string MERCHANT_DESCRIPTOR = "merchantDescriptor";
	public static readonly string MERCHANT_INVOICE_ID = "merchantInvoiceID";
	public static readonly string MERCHANT_ID = "merchantID";
	public static readonly string MERCHANT_PASSWORD = "merchantPassword";
	public static readonly string MERCHANT_PRODUCT_ID = "merchantProductID";
	public static readonly string MERCHANT_SITE_ID = "merchantSiteID";
	public static readonly string OMIT_RECEIPT = "omitReceipt";
	public static readonly string PARES = "PARES";
	public static readonly string PARTIAL_AUTH_FLAG = "partialAuthFlag";
	public static readonly string PAY_HASH = CARD_HASH;
	public static readonly string PAYINFO_TRANSACT_ID = "payInfoTransactID";
	public static readonly string PROMPT_TIMEOUT = "promptTimeout";
	public static readonly string REBILL_COUNT = "rebillCount";
	public static readonly string REBILL_END_DATE = "rebillEndDate";
	public static readonly string REBILL_FREQUENCY = "rebillFrequency";
	public static readonly string REBILL_AMOUNT = "rebillAmount";
	public static readonly string REBILL_START = "rebillStart";
	public static readonly string REBILL_SUSPEND = "rebillSuspend";
	public static readonly string REBILL_RESUME = "rebillResume";
	public static readonly string REFERENCE_GUID = "referenceGUID";
	public static readonly string REFERRAL_NO = "referralNo";
	public static readonly string REFERRING_MERCHANT_ID = "referringMerchantID";
	public static readonly string REFERRED_CUSTOMER_ID = "referredCustomerID";
	public static readonly string ROUTING_NO = "routingNo";
	public static readonly string SAVINGS_ACCOUNT = "savingsAccount";
	public static readonly string TRANSACT_ID = REFERENCE_GUID;
	public static readonly string SCRUB = "scrub";
	public static readonly string SCRUB_PROFILE = "scrubProfile";
	public static readonly string SCRUB_ACTIVITY = "scrubActivity";
	public static readonly string SCRUB_NEGDB = "scrubNegDB";
	public static readonly string SS_NUMBER = "ssNumber";
	public static readonly string SUB_MERCHANT_ID = "subMerchantID";
	public static readonly string THREATMETRIX_SESSION_ID = "threatMetrixSessionID";
	public static readonly string TRANSACTION_TYPE = "transactionType";
	public static readonly string UDF01 = "udf01";
	public static readonly string UDF02 = "udf02";
	public static readonly string USERNAME = "username";
	public static readonly string USE_3D_SECURE = "use3DSecure";
		
	public static readonly string _3D_CHECK = "ThreeDCheck";
	public static readonly string _3D_ECI = "ThreeDECI";
	public static readonly string _3D_CAVV_UCAF = "ThreeDCavvUcaf";
	public static readonly string _3D_XID = "ThreeDXID";
		
	public static readonly string _3D_VERSION = "THREEDVERSION";
	public static readonly string _3D_VERSTATUS = "THREEDVERSTATUS";
	public static readonly string _3D_PARESSTATUS = "THREEDPARESSTATUS";
	public static readonly string _3D_CAVV_ALGORITHM = "THREEDCAVVALGORITHM";

	public static readonly string _3DSECURE_THREE_DS_SERVER_TRANSACTION_ID = "_3DSECURE_THREE_DS_SERVER_TRANSACTION_ID";
	public static readonly string _3DSECURE_DS_TRANSACTION_ID = "_3DSECURE_DS_TRANSACTION_ID";
	public static readonly string _3DSECURE_ACS_TRANSACTION_ID = "_3DSECURE_ACS_TRANSACTION_ID";
	public static readonly string _3DSECURE_DF_REFERENCE_ID = "_3DSECURE_DF_REFERENCE_ID";
	public static readonly string _3DSECURE_REDIRECT_URL = "_3DSECURE_REDIRECT_URL";
	public static readonly string _3DSECURE_CHALLENGE_MANDATED_INDICATOR = "_3DSECURE_CHALLENGE_MANDATED_INDICATOR";
		
	public static readonly string BROWSER_JAVA_ENABLED = "BROWSERJAVAENABLED";
	public static readonly string BROWSER_LANGUAGE = "BROWSERLANGUAGE";
	public static readonly string BROWSER_COLOR_DEPTH = "BROWSERCOLORDEPTH";
	public static readonly string BROWSER_SCREEN_HEIGHT = "BROWSERSCREENHEIGHT";
	public static readonly string BROWSER_SCREEN_WIDTH = "BROWSERSCREENWIDTH";
	public static readonly string BROWSER_TIME_ZONE = "BROWSERTIMEZONE";
	
		
//////////////////////////////////////////////////////////////////////
//
//      Definition of key constants that carry failure
//      information to the servers.
//
//////////////////////////////////////////////////////////////////////
//
	public static readonly string FAILED_SERVER = "failedServer";
	public static readonly string FAILED_GUID = "failedGUID";
	public static readonly string FAILED_RESPONSE_CODE = "failedResponseCode";
	public static readonly string FAILED_REASON_CODE = "failedReasonCode";

//////////////////////////////////////////////////////////////////////
//
//      Definition of key values used to override gateway
//      service URL.
//
//////////////////////////////////////////////////////////////////////
//
	public static readonly string GATEWAY_SERVER = "gatewayServer";
	public static readonly string GATEWAY_PROTOCOL = "gatewayProtocol";
	public static readonly string GATEWAY_PORTNO = "gatewayPortNo";
	public static readonly string GATEWAY_SERVLET = "gatewayServlet";
	public static readonly string GATEWAY_CONNECT_TIMEOUT = "gatewayConnectTimeout";
	public static readonly string GATEWAY_READ_TIMEOUT = "gatewayReadTimeout";

//////////////////////////////////////////////////////////////////////
//
//      Definition of constant transaction types.
//
//////////////////////////////////////////////////////////////////////
//
	public static readonly string TRANSACTION_ABORT = "ABORT";
	public static readonly string TRANSACTION_AUTH_ONLY = "AUTH";
	public static readonly string TRANSACTION_CONFIRM = "CONFIRM";
	public static readonly string TRANSACTION_CREDIT = "CREDIT";
	public static readonly string TRANSACTION_PRICE_CHECK = "PRICECHECK";
	public static readonly string TRANSACTION_SALE = "PURCHASE";
	public static readonly string TRANSACTION_TICKET = "TICKET";
	public static readonly string TRANSACTION_VOID = "VOID";

	public static readonly string XSELL_REFERENCE_XACT = "XSELLREFERENCEXACT";
}