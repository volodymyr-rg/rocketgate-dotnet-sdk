using System.Net;
using System.Net.Cache;
using System.Text;

namespace RocketGateClient;

public class GatewayService
{
	private readonly Random dnsDistribution;	// DNS random distribution

//////////////////////////////////////////////////////////////////////
//
//		GatewayService() - Constructor for class.
//
//////////////////////////////////////////////////////////////////////
//
	public GatewayService()
	{
//
//		Create the random number generator used for DNS.
//
		dnsDistribution = new Random();

//
//		Default URL information to live hosts.
//
		ROCKETGATE_HOST = ROCKETGATE_LIVE_HOST;
		ROCKETGATE_PROTOCOL = ROCKETGATE_LIVE_PROTOCOL;
		ROCKETGATE_PORTNO = ROCKETGATE_LIVE_PORTNO;
	}

//////////////////////////////////////////////////////////////////////
//
//		PerformTransaction() - Perform the transaction outlined
//							   in a GatewayRequest.
//
//////////////////////////////////////////////////////////////////////
//

//////////////////////////////////////////////////////////////////////
//
//		11-16-2017	darcy
//
//		Changes for DDOS protection.
//
//		private int PerformTransaction(IPAddress address,
//
	private int PerformTransaction(string server,
//
//////////////////////////////////////////////////////////////////////

		string servlet,
		GatewayRequest request,
		GatewayResponse response)
	{
		int readTimeout;			// Timeout for read
		int connectTimeout;			// Timeout for connection
		string requestString;		// Request XML
		string responseString;		// Response XML

		int urlPortNo;				// Destination port number
		string urlServlet;			// Destination servlet
		string urlProtocol;			// Protocol used for connection
		string urlString;			// Complete URL string

//////////////////////////////////////////////////////////////////////
//
//		08-05-2009	darcy
//
//		Changed StreamWriter to a simple Stream.
//
//			StreamWriter postWriter;	// Writer for post data
//
		Stream postWriter;			// Writer for post data
		UTF8Encoding utf8;			// Encoder for XML data
		byte[] xmlArray;			// UTF-8 encoded data
//
//////////////////////////////////////////////////////////////////////

		Stream receiveStream;		// Readers for response
		StreamReader readStream;

		HttpWebRequest webRequest;	// HTTP request
		HttpWebResponse webResponse;// HTTP response

//
//		Reset the response object and turn the request into
//		a string that can be transmitted.
//
		response.Reset();					// Clear old contents
		requestString = request.ToXMLString();

//////////////////////////////////////////////////////////////////////
//
//		08-05-2009	darcy
//		
//		Prepare the UTF-8 array.
//
		utf8 = new UTF8Encoding();			// Create an encoder
		xmlArray = utf8.GetBytes(requestString);
//
//////////////////////////////////////////////////////////////////////

//
//		Gather the attributes used for the connection URL.
//
		urlProtocol = request.Get(GatewayRequest.GATEWAY_PROTOCOL);
		urlServlet = request.Get(GatewayRequest.GATEWAY_SERVLET);
		urlPortNo = request.GetInteger(GatewayRequest.GATEWAY_PORTNO);

//
//		If the parameters were not set in the request,
//		use the system defaults.
//
		if (urlProtocol == null) urlProtocol = ROCKETGATE_PROTOCOL;
		if (urlPortNo < 1) urlPortNo = ROCKETGATE_PORTNO;
		if (urlServlet == null) urlServlet = servlet;

//
//		Build the URL of the RocketGate gateway service.
//

//////////////////////////////////////////////////////////////////////
//
//		11-16-2017	darcy
//
//		Changes for DDOS protection.
//
//			urlString = urlProtocol + "://" + address.ToString() + ":"
//
		urlString = urlProtocol + "://" + server + ":"
//
//////////////////////////////////////////////////////////////////////

		            + urlPortNo.ToString() + "/"
		            + urlServlet;

//
//		Gather the timeout values that will be used
//		for the request.
//
		connectTimeout = request.GetInteger(
			GatewayRequest.GATEWAY_CONNECT_TIMEOUT);
		readTimeout = request.GetInteger(
			GatewayRequest.GATEWAY_READ_TIMEOUT);

//
//		Use default values if the parameters were not set.
//
		if (connectTimeout < 0)
			connectTimeout = ROCKETGATE_CONNECT_TIMEOUT;
		if (readTimeout < 0)
			readTimeout = ROCKETGATE_READ_TIMEOUT;

//
//		Timeout values need to be expressed in milliseconds.
//		Scale the values for proper use.
//
		connectTimeout *= 1000;			// Scale to milliseconds
		readTimeout *= 1000;

//
//		Build the WebRequest that gets sent to the server.
//

		try {
			webRequest = (HttpWebRequest)WebRequest.Create(urlString);
			webRequest.Method = "POST";

//////////////////////////////////////////////////////////////////////
//
//		08-05-2009	darcy
//		
//		Set the length based upon the array not the string.
//
//				webRequest.ContentLength = requestString.Length;
//
			webRequest.ContentLength = xmlArray.Length;
//
//////////////////////////////////////////////////////////////////////

			webRequest.ContentType = "text/xml";
			webRequest.Timeout = readTimeout + connectTimeout;
			webRequest.SendChunked = false;
			webRequest.UserAgent = "RG Client " + GatewayRequest.VERSION_NO + " (.NET)";
			webRequest.Accept = "*/*";
			webRequest.CachePolicy =
				new HttpRequestCachePolicy(
					HttpRequestCacheLevel.NoCacheNoStore);

//
//		Add a callback that will allow us to bypass the certificates
//		on the server.
//

//////////////////////////////////////////////////////////////////////
//
//      07-07-2015  darcy
//
//      Limit security protocol to TLS 1.2.
//
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
//
//////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////
//
//		11-16-2017	darcy
//
//		Changes for DDOS protection.
//
//				ServicePointManager.ServerCertificateValidationCallback +=
//					new System.Net.Security.RemoteCertificateValidationCallback(
//													CertificateCallback);
//
//////////////////////////////////////////////////////////////////////
													
		}
				
//
//		If there was a problem forming the URL, we must quit.
//	
		catch(Exception ex) {
			response.Set(GatewayResponse.EXCEPTION, ex.Message);
			response.SetResults(GatewayCodes.RESPONSE_REQUEST_ERROR,
				GatewayCodes.REASON_INVALID_URL);
			return GatewayCodes.RESPONSE_REQUEST_ERROR;
		}

//
//		Initialize values used in HTTP request/response.
//
		postWriter = null;					// Not allocated yet
		webResponse = null;
		receiveStream = null;
		readStream = null;
				
//
//		Post the request to the server.
//
		try {

//////////////////////////////////////////////////////////////////////
//
//		08-05-2009	darcy
//
//		Post the XML array data using the Stream.
//
//				postWriter = new StreamWriter(webRequest.GetRequestStream());
//				postWriter.Write(requestString);
//
			postWriter = webRequest.GetRequestStream();
			postWriter.Write(xmlArray, 0, xmlArray.Length);
//
//////////////////////////////////////////////////////////////////////

		}

//
//		If there were any errors posting the request,
//		we need to quit.
//
		catch(Exception ex) {
			response.Set(GatewayResponse.EXCEPTION, ex.Message);
			response.SetResults(GatewayCodes.RESPONSE_SYSTEM_ERROR,
				GatewayCodes.REASON_REQUEST_XMIT_ERROR);
			return GatewayCodes.RESPONSE_SYSTEM_ERROR;
		}

//
//		Close the post writer when we are done.
//
		finally {
			if (postWriter != null) postWriter.Close();
		}

//
//		Try to read the results from the server.
//
		try {
			webResponse = (HttpWebResponse)webRequest.GetResponse();
			receiveStream = webResponse.GetResponseStream();
			readStream = new StreamReader(receiveStream, Encoding.UTF8);
			responseString = readStream.ReadToEnd();
		}

//
//		If there a timeout reading the response, we need to quit.
//
		catch(WebException ex) {
			if (ex.Status == WebExceptionStatus.Timeout) {
				response.SetResults(
					GatewayCodes.RESPONSE_SYSTEM_ERROR,
					GatewayCodes.REASON_RESPONSE_READ_TIMEOUT);
			} else {
				response.SetResults(
					GatewayCodes.RESPONSE_SYSTEM_ERROR,
					GatewayCodes.REASON_RESPONSE_READ_ERROR);
			}
			response.Set(GatewayResponse.EXCEPTION, ex.Message);
			return GatewayCodes.RESPONSE_SYSTEM_ERROR;
		}

//
//		If there was an error reading the response, we need to quit.
//
		catch(Exception ex) {
			response.Set(GatewayResponse.EXCEPTION, ex.Message);
			response.SetResults(GatewayCodes.RESPONSE_SYSTEM_ERROR,
				GatewayCodes.REASON_RESPONSE_READ_ERROR);
			return GatewayCodes.RESPONSE_SYSTEM_ERROR;
		}

//
//		Clean up when we are done.
//
		finally {
			if (webResponse != null) webResponse.Close();
			if (readStream != null) readStream.Close();
		}

//
//		Copy the contents of the response message into
//		the response object.
//
		response.SetFromXMLString(responseString);
		return response.GetResponseCode();	// Return success/failure
	}


//////////////////////////////////////////////////////////////////////
//
//		CertificateCallback() - Callback that validates the
//								rocketgate servers.
//
//////////////////////////////////////////////////////////////////////
//

//////////////////////////////////////////////////////////////////////
//
//		11-16-2017	darcy
//
//		Changes for DDOS protection.
//
//		private static bool CertificateCallback(object sender, 
//												X509Certificate certificate,
//												X509Chain chain,
//												SslPolicyErrors sslPolicyErrors)
//		{
//			return true;
//		}
//
//////////////////////////////////////////////////////////////////////


//////////////////////////////////////////////////////////////////////
//
//		PerformTransaction() - Perform the transaction outlined
//							   in a GatewayRequest.
//
//////////////////////////////////////////////////////////////////////
//
	private bool PerformTransaction(string servlet,
		GatewayRequest request,
		GatewayResponse response)
	{
		int index;					// Index for host list
		int results;				// Results of transaction
		string serverName;			// Name of server for request
		IPHostEntry hostInfo;		// DNS information
		IPAddress[] addressList;	// List of RocketGate hosts

//////////////////////////////////////////////////////////////////////
//
//		11-16-2017	darcy
//
//		Changes for DDOS protection.
//
//			IPAddress swapper;			// Address for swapping
//
		string swapper;				// String for swapping
		string[] serverList;		// List of RocketGate hosts
//			
//////////////////////////////////////////////////////////////////////

//
//		If the request has specified a server name, use it.
//		Otherwise, use the default for the service.
//
		serverName = request.Get(GatewayRequest.GATEWAY_SERVER);
		if (serverName == null) serverName = ROCKETGATE_HOST;

//
//		Clear any error tracking that may be leftover in
//		a re-used request.
//
		request.Set(GatewayRequest.FAILED_SERVER, null);
		request.Set(GatewayRequest.FAILED_RESPONSE_CODE, null);
		request.Set(GatewayRequest.FAILED_REASON_CODE, null);
		request.Set(GatewayRequest.FAILED_GUID, null);

//////////////////////////////////////////////////////////////////////
//
//		11-16-2017	darcy
//
//		Changes for DDOS protection.
//
////
////		Get the list of hosts that can handle this transaction.
////
//			addressList = null;					// Set empty list
//			try {
//				hostInfo = Dns.GetHostByName(serverName);
//				addressList = hostInfo.AddressList;
//				if (addressList.Length < 1) {	// No list?
//					addressList = null;			// Reset the list
//					if (!(serverName.Equals(this.ROCKETGATE_LIVE_HOST))) {
//						response.SetResults(GatewayCodes.RESPONSE_SYSTEM_ERROR,
//											GatewayCodes.REASON_DNS_FAILURE);
//						return false;			// This is a failure
//					}
//				}
//			}
//
////
////		If we are unable to get the list of hosts, we cannot
////		process the transaction.
////
//			catch(Exception ex) {
//				addressList = null;				// Reset the list
//				if (!(serverName.Equals(this.ROCKETGATE_LIVE_HOST))) {
//					response.Set(GatewayResponse.EXCEPTION, ex.Message);
//					response.SetResults(GatewayCodes.RESPONSE_SYSTEM_ERROR,
//										GatewayCodes.REASON_DNS_FAILURE);
//					return false;				// This is a failure
//				}
//			}
//
////
////		If we failed to get a host list, try to build one from
////		our static IP addresses.
////
//			try {
//				if (addressList == null) {		// No list yet?
//					addressList = new IPAddress[2];
//					hostInfo = Dns.Resolve(this.ROCKETGATE_GW16_IP);
//					addressList[0] = hostInfo.AddressList[0];
//					hostInfo = Dns.Resolve(this.ROCKETGATE_GW17_IP);
//					addressList[1] = hostInfo.AddressList[0];
//				}
//
////
////		Randomize the DNS distribution.
////
//				index = this.dnsDistribution.Next(addressList.Length);
//				if (index > 0) {				// Need to swap?
//					swapper = addressList[0];	// Get the first entry
//					addressList[0] = addressList[index];
//					addressList[index] = swapper;
//				}
//			}
//
////
////		If we are unable to get the list of hosts, we cannot
////		process the transaction.
////
//			catch(Exception ex) {
//				response.Set(GatewayResponse.EXCEPTION, ex.Message);
//				response.SetResults(GatewayCodes.RESPONSE_SYSTEM_ERROR,
//									GatewayCodes.REASON_DNS_FAILURE);
//				return false;					// This is a failure
//			}
//

//
//		If we are not accessing the gateway, use the server
//		name as-is.
//
		if (!serverName.Equals(ROCKETGATE_LIVE_HOST)) {
			serverList = new string[1];		// Create a list
			serverList[0] = serverName;		// Insert name
		} else {

//
//		Get the list of hosts that can handle this transaction.
//
			addressList = null;				// Set empty list
			try {
				hostInfo = Dns.GetHostEntry(serverName);
				addressList = hostInfo.AddressList;
				if (addressList.Length < 1) addressList = null;
			}
			catch (Exception)
			{
				// ignored
			}

//
//		If we failed to get a host list, try to build one from
//		our static IP addresses.
//
			if (addressList == null) {		// No list?
				serverList = new string[2];	// Create a list
				serverList[0] = ROCKETGATE_GW16_STRING;
				serverList[1] = ROCKETGATE_GW17_STRING;
			} else {

//
//		Copy the DNS data to a server list.
//
				index = 0;					// Start at base of list
				serverList = new string[addressList.Length];
				while (index < addressList.Length) {
					serverList[index] = addressList[index].ToString();
					if (serverList[index].Equals(ROCKETGATE_GW16_IP))
						serverList[index] = ROCKETGATE_GW16_STRING;
					if (serverList[index].Equals(ROCKETGATE_GW17_IP))
						serverList[index] = ROCKETGATE_GW17_STRING;
					index++;				// Next in list
				}
			}
		}

//
//		Randomize the DNS distribution.
//
		if (serverList.Length > 1) {
			index = dnsDistribution.Next(serverList.Length);
			if (index > 0) {			// Need to swap?
				swapper = serverList[0];// Get the first entry
				serverList[0] = serverList[index];
				serverList[index] = swapper;
			}
		}
//
//////////////////////////////////////////////////////////////////////

//
//		Loop over the hosts in the DNS entry.  Try to send
//		the transaction to each host until it finally succeeds.
//		If it fails due to an unrecoverable error, just quit.
//
		index = 0; 							// Start with first entry

//////////////////////////////////////////////////////////////////////
//
//		11-16-2017	darcy
//
//		Changes for DDOS protection.
//
//			while (index < addressList.Length) {// Loop over all entries
//				results = this.PerformTransaction(addressList[index],
//
		while (index < serverList.Length) {	// Loop over all entries
			results = PerformTransaction(serverList[index],
//
//////////////////////////////////////////////////////////////////////

				servlet,
				request,
				response);
			if (results == GatewayCodes.RESPONSE_SUCCESS) return true;
			if (results != GatewayCodes.RESPONSE_SYSTEM_ERROR) return false;

//
//		Save any errors in the response so they can be
//		transmitted along with the next request.
//
			request.Set(GatewayRequest.FAILED_SERVER,

//////////////////////////////////////////////////////////////////////
//
//		11-16-2017	darcy
//
//		Changes for DDOS protection.
//
//							addressList[index].ToString());
//
				serverList[index]);
//
//////////////////////////////////////////////////////////////////////

			request.Set(GatewayRequest.FAILED_RESPONSE_CODE,
				response.Get(GatewayResponse.RESPONSE_CODE));
			request.Set(GatewayRequest.FAILED_REASON_CODE,
				response.Get(GatewayResponse.REASON_CODE));
			request.Set(GatewayRequest.FAILED_GUID,
				response.Get(GatewayResponse.TRANSACT_ID));
			index++;						// Try next host in list
		}
		return false;						// Transaction failed
	}


//////////////////////////////////////////////////////////////////////
//
//		PerformTargetedTransaction() - Send a transaction to a server
//									   based upon the reference GUID.
//
//////////////////////////////////////////////////////////////////////
//
	private bool PerformTargetedTransaction(string servlet,
		GatewayRequest request,
		GatewayResponse response)
	{
		int siteNo;					// Original processing site
		int results;				// Results of transaction processing
		int separator;				// Location of first . in server name
		string prefix;				// gw. portion of domain
		string serverName;			// gw-X.domain
		long referenceGUID;			// Original transaction GUID

//////////////////////////////////////////////////////////////////////
//
//		11-16-2017	darcy
//
//		Changes for DDOS protection.
//
//			IPHostEntry hostInfo;		// DNS information
//			IPAddress[] addressList;	// List of RocketGate hosts
//
//////////////////////////////////////////////////////////////////////

//
//		Clear any error tracking that may be leftover in
//		a re-used request.
//
		request.Set(GatewayRequest.FAILED_SERVER, null);
		request.Set(GatewayRequest.FAILED_RESPONSE_CODE, null);
		request.Set(GatewayRequest.FAILED_REASON_CODE, null);
		request.Set(GatewayRequest.FAILED_GUID, null);

//
//		This transaction must go to the host that processed
//		a previous referenced transaction.  Get the GUID of the
//		reference transaction.
// 
		if ((referenceGUID = request.GetReferenceGUID()) == 0) {
			response.SetResults(GatewayCodes.RESPONSE_REQUEST_ERROR,
				GatewayCodes.REASON_INVALID_REFGUID);
			return false;					// Transaction failed
		}

//
//		Build a hostname using the site number from the GUID.
//
		siteNo = (int)((referenceGUID >> 56) & 0xff);
		serverName = request.Get(GatewayRequest.GATEWAY_SERVER);
		if (serverName == null) {			// No servername in request?
			serverName = ROCKETGATE_HOST;
			if ((separator = serverName.IndexOf('.')) > 0) {
				prefix = serverName.Substring(0, separator);
				serverName = serverName.Substring(separator);
				serverName = prefix + "-" + siteNo + serverName;
			}
		}

//////////////////////////////////////////////////////////////////////
//
//		11-16-2017	darcy
//
//		Changes for DDOS protection.
//
////
////		Get the list of hosts that can handle this transaction.
////
//			addressList = null;					// Assume no list
//			try {
//				hostInfo = Dns.GetHostByName(serverName);
//				addressList = hostInfo.AddressList;
//				if (addressList.Length < 1) {	// No list?
//					addressList = null;
//					if ((!(serverName.Equals(this.ROCKETGATE_GW16_STRING))) &&
//						(!(serverName.Equals(this.ROCKETGATE_GW17_STRING)))) {
//						response.SetResults(GatewayCodes.RESPONSE_SYSTEM_ERROR,
//											GatewayCodes.REASON_DNS_FAILURE);
//						return false;				// This is a failure
//					}
//				}
//			}
////
////
////		If we are unable to get the address, we cannot
////		process the transaction.
////
//			catch(Exception ex) {
//				addressList = null;				// Assume no list
//				if ((!(serverName.Equals(this.ROCKETGATE_GW16_STRING))) &&
//					(!(serverName.Equals(this.ROCKETGATE_GW17_STRING)))) {
//					response.Set(GatewayResponse.EXCEPTION, ex.Message);
//					response.SetResults(GatewayCodes.RESPONSE_SYSTEM_ERROR,
//										GatewayCodes.REASON_DNS_FAILURE);
//					return false;				// This is a failure
//				}
//			}
//
//
//		If the DNS lookup failed, try using static IP addresses.
//
//			try {
//				if (addressList == null) {		// Don't have list?
//					if (serverName.Equals(this.ROCKETGATE_GW16_STRING))
//						hostInfo = Dns.GetHostByName(this.ROCKETGATE_GW16_IP);
//					else
//						hostInfo = Dns.GetHostByName(this.ROCKETGATE_GW17_IP);
//					addressList = hostInfo.AddressList;
//				}
//			}
//
////
////		If we are unable to get the address, we cannot
////		process the transaction.
////
//			catch(Exception ex) {
//				response.Set(GatewayResponse.EXCEPTION, ex.Message);
//				response.SetResults(GatewayCodes.RESPONSE_SYSTEM_ERROR,
//									GatewayCodes.REASON_DNS_FAILURE);
//				return false;					// This is a failure
//			}
//
//////////////////////////////////////////////////////////////////////

//
//		Send the transaction to the named host.
//

//////////////////////////////////////////////////////////////////////
//
//		11-16-2017	darcy
//
//		Changes for DDOS protection.
//
//			results = this.PerformTransaction(addressList[0],
//
		results = PerformTransaction(serverName,
//
//////////////////////////////////////////////////////////////////////

			servlet,
			request,
			response);
		if (results == GatewayCodes.RESPONSE_SUCCESS) return true;
		return false;						// Request failed
	}


//////////////////////////////////////////////////////////////////////
//
//		PerformConfirmation() - Perform the confirmation pass
//								that tells the server we have
//								received the transaction reply.
//
//////////////////////////////////////////////////////////////////////
//
	private bool PerformConfirmation(string servlet,
		string confirmationType,
		GatewayRequest request,
		GatewayResponse response)
	{
		string confirmGUID;			// GUID to be confirmed
		GatewayResponse confirmResponse;

//
//		Verify that we have a transaction ID for the
//		confirmation message.
//
		confirmGUID = response.Get(GatewayResponse.TRANSACT_ID);
		if (confirmGUID == null) {			// Missing?
			response.Set(GatewayResponse.EXCEPTION,
				"BUGCHECK - Missing confirmation GUID");
			response.SetResults(GatewayCodes.RESPONSE_SYSTEM_ERROR,
				GatewayCodes.REASON_BUGCHECK);
			return false;					// This is a failure
		}

//
//		Add the GUID to the request and send it back to
//		original server for confirmation.
//
		confirmResponse = new GatewayResponse();
		request.Set(GatewayRequest.TRANSACTION_TYPE, confirmationType);
		request.Set(GatewayRequest.REFERENCE_GUID, confirmGUID);
		if (PerformTargetedTransaction(servlet,
			    request,
			    confirmResponse)) return true;

//
//		If the confirmation failed, copy the reason and response code
//		into the original response object to override the success.
//
		response.SetResults(
			confirmResponse.GetInteger(GatewayResponse.RESPONSE_CODE),
			confirmResponse.GetInteger(GatewayResponse.REASON_CODE));
		return false;						// And quit
	}


//////////////////////////////////////////////////////////////////////
//
//		PerformAuthOnly() - Perform an auth-only transaction using
//							the information contained in a request.
//
//////////////////////////////////////////////////////////////////////
//
	public bool PerformAuthOnly(GatewayRequest request,
		GatewayResponse response)
	{
		request.Set(GatewayRequest.TRANSACTION_TYPE,
			TRANSACTION_CC_AUTH);
		if (request.Get(GatewayRequest.REFERENCE_GUID) != null) {
			if (!PerformTargetedTransaction(ROCKETGATE_SERVLET,
				    request,
				    response)) return false;
		} else {
			if (!PerformTransaction(ROCKETGATE_SERVLET,
				    request,
				    response)) return false;
		}
		return PerformConfirmation(ROCKETGATE_SERVLET,
			TRANSACTION_CC_CONFIRM,
			request,
			response);
	}


//////////////////////////////////////////////////////////////////////
//
//		PerformTicket() - Perform a ticket transaction for a
//						  previous auth-only transaction.
//
//////////////////////////////////////////////////////////////////////
//
	public bool PerformTicket(GatewayRequest request,
		GatewayResponse response)
	{
		request.Set(GatewayRequest.TRANSACTION_TYPE,
			TRANSACTION_CC_TICKET);
		return PerformTargetedTransaction(ROCKETGATE_SERVLET,
			request,
			response);
	}


//////////////////////////////////////////////////////////////////////
//
//		PerformPurchase() - Perform a complete purchase transaction
//							using the information contained in a
//							request.
//
//////////////////////////////////////////////////////////////////////
//
	public bool PerformPurchase(GatewayRequest request,
		GatewayResponse response)
	{
		request.Set(GatewayRequest.TRANSACTION_TYPE,
			TRANSACTION_CC_SALE);
		if (request.Get(GatewayRequest.REFERENCE_GUID) != null) {
			if (!PerformTargetedTransaction(ROCKETGATE_SERVLET,
				    request,
				    response)) return false;
		} else {
			if (!PerformTransaction(ROCKETGATE_SERVLET,
				    request,
				    response)) return false;
		}
		return PerformConfirmation(ROCKETGATE_SERVLET,
			TRANSACTION_CC_CONFIRM,
			request,
			response);
	}


//////////////////////////////////////////////////////////////////////
//
//		PerformCredit() - Perform a credit transaction.
//
//////////////////////////////////////////////////////////////////////
//
	public bool PerformCredit(GatewayRequest request,
		GatewayResponse response)
	{
//
//		Apply the transaction type to the request.
//
		request.Set(GatewayRequest.TRANSACTION_TYPE,
			TRANSACTION_CC_CREDIT);

//
//		If the credit references a previous transaction, we
//		need to send it back to the origination site.  Otherwise,
//		it can be sent to any server.
//
		if (request.Get(GatewayRequest.REFERENCE_GUID) != null)
			return PerformTargetedTransaction(
				ROCKETGATE_SERVLET,
				request,
				response);
		return PerformTransaction(ROCKETGATE_SERVLET,
			request,
			response);
	}


//////////////////////////////////////////////////////////////////////
//
//		PerformVoid() - Perform a void for a previously
//						completed transaction.
//
//////////////////////////////////////////////////////////////////////
//
	public bool PerformVoid(GatewayRequest request,
		GatewayResponse response)
	{
		request.Set(GatewayRequest.TRANSACTION_TYPE,
			TRANSACTION_CC_VOID);
		return PerformTargetedTransaction(ROCKETGATE_SERVLET,
			request,
			response);
	}


//////////////////////////////////////////////////////////////////////
//
//      PerformCardScrub() - Perform scrubbing on a card/customer.
//
//////////////////////////////////////////////////////////////////////
//
	public bool PerformCardScrub(GatewayRequest request,
		GatewayResponse response)
	{
		request.Set(GatewayRequest.TRANSACTION_TYPE,
			TRANSACTION_CARD_SCRUB);
		return PerformTransaction(ROCKETGATE_SERVLET,
			request,
			response);
	}
		

//////////////////////////////////////////////////////////////////////
//
//		PerformRebillUpdate() - Update a rebilling record.
//
//////////////////////////////////////////////////////////////////////
//
	public bool PerformRebillUpdate(GatewayRequest request,
		GatewayResponse response)
	{
//
//		Apply the transaction type to the request.
//
		request.Set(GatewayRequest.TRANSACTION_TYPE,
			TRANSACTION_REBILL_UPDATE);

//
//		If the transaction does not include a prorated amount,
//		just perform the update.
//
		if (request.GetDouble(GatewayRequest.AMOUNT) <= (double)0.0)
			return PerformTransaction(ROCKETGATE_SERVLET,
				request,
				response);

//
//		If we have a prorated amount, perform the update and
//		confirm the charge.
//
		if (!PerformTransaction(ROCKETGATE_SERVLET,
			    request,
			    response)) return false;
		return PerformConfirmation(ROCKETGATE_SERVLET,
			TRANSACTION_CC_CONFIRM,
			request,
			response);
	}


//////////////////////////////////////////////////////////////////////
//
//		PerformRebillCancel() - Cancel a rebilling record.
//
//////////////////////////////////////////////////////////////////////
//
	public bool PerformRebillCancel(GatewayRequest request,
		GatewayResponse response)
	{
		request.Set(GatewayRequest.TRANSACTION_TYPE,
			TRANSACTION_REBILL_CANCEL);
		return PerformTransaction(ROCKETGATE_SERVLET,
			request,
			response);
	}


//////////////////////////////////////////////////////////////////////
//
//		PerformLookup() - Lookup a previous transaction.
//
//////////////////////////////////////////////////////////////////////
//
	public bool PerformLookup(GatewayRequest request,
		GatewayResponse response)
	{
//
//		Apply the transaction type to the request.
//
		request.Set(GatewayRequest.TRANSACTION_TYPE,
			TRANSACTION_LOOKUP);

//
//		If the lookup references a previous transaction, we
//		need to send it back to the origination site.  Otherwise,
//		it can be sent to any server.
//
		if (request.Get(GatewayRequest.REFERENCE_GUID) != null)
			return PerformTargetedTransaction(
				ROCKETGATE_SERVLET,
				request,
				response);
		return PerformTransaction(ROCKETGATE_SERVLET,
			request,
			response);
	}


//////////////////////////////////////////////////////////////////////
//
//		PerformCardUpload() - Upload card data to the servers.
//
//////////////////////////////////////////////////////////////////////
//
	public bool PerformCardUpload(GatewayRequest request,
		GatewayResponse response)
	{
//
//		Apply the transaction type to the request.
//
		request.Set(GatewayRequest.TRANSACTION_TYPE,
			TRANSACTION_CARD_UPLOAD);
		return PerformTransaction(ROCKETGATE_SERVLET,
			request,
			response);
	}
		
//////////////////////////////////////////////////////////////////////
//
//	GenerateXsell() - Add an entry to the XsellQueue.
//
//////////////////////////////////////////////////////////////////////
//
	public bool GenerateXsell(GatewayRequest request,
		GatewayResponse response)
	{
		//
		//		Apply the transaction type to the request.
		//
		request.Set(GatewayRequest.TRANSACTION_TYPE, "GENERATEXSELL");
		request.Set(GatewayRequest.REFERENCE_GUID, request.Get(GatewayRequest.XSELL_REFERENCE_XACT));
		if (request.Get(GatewayRequest.REFERENCE_GUID) != null)
		{
			return PerformTargetedTransaction(ROCKETGATE_SERVLET, request, response);
		}
		return PerformTransaction(ROCKETGATE_SERVLET, request, response);
	}

//////////////////////////////////////////////////////////////////////
//
//		SetTestMode() - Enable/Disable testing mode.
//
//////////////////////////////////////////////////////////////////////
//
	public void SetTestMode(bool testingMode)
	{
//
//		If testing is enabled, use the testing mode destinations.
//
		if (testingMode) {					// Using testing mode?
			ROCKETGATE_HOST = ROCKETGATE_TEST_HOST;
			ROCKETGATE_PROTOCOL = ROCKETGATE_TEST_PROTOCOL;
			ROCKETGATE_PORTNO = ROCKETGATE_TEST_PORTNO;
		} else {

//
//		Change the destinations to the live servers.
//
			ROCKETGATE_HOST = ROCKETGATE_LIVE_HOST;
			ROCKETGATE_PROTOCOL = ROCKETGATE_LIVE_PROTOCOL;
			ROCKETGATE_PORTNO = ROCKETGATE_LIVE_PORTNO;
		}
	}


//////////////////////////////////////////////////////////////////////
//
//		SetHost() - Set the host used by the GatewayService.
//
//////////////////////////////////////////////////////////////////////
//
	public void SetHost(string hostname)
	{
		if (hostname != null &&			// Have a string?
		    !hostname.Equals("")) ROCKETGATE_HOST = hostname;
	}


//////////////////////////////////////////////////////////////////////
//
//		SetProtocol() - Set the protocol used by the GatewayService.
//
//////////////////////////////////////////////////////////////////////
//
	public void SetProtocol(string protocol)
	{
		if (protocol != null &&			// Have a string?
		    !protocol.Equals("")) ROCKETGATE_PROTOCOL = protocol;
	}


//////////////////////////////////////////////////////////////////////
//
//		SetPortNo() - Set the port number used by the GatewayService.
//
//////////////////////////////////////////////////////////////////////
//
	public void SetPortNo(int portNo)
	{
		if (portNo > 0) ROCKETGATE_PORTNO = portNo;
	}


//////////////////////////////////////////////////////////////////////
//
//		SetConnectTimeout() - Set the timeout for connecting
//							  to a remote host.
//
//////////////////////////////////////////////////////////////////////
//
	public void SetConnectTimeout(int timeout)
	{
		if (timeout > 0) ROCKETGATE_CONNECT_TIMEOUT = timeout;
	}


//////////////////////////////////////////////////////////////////////
//
//		SetReadTimeout() - Set the timeout for reading a response 
//						   from a remote host.
//
//////////////////////////////////////////////////////////////////////
//
	public void SetReadTimeout(int timeout)
	{
		if (timeout > 0) ROCKETGATE_READ_TIMEOUT = timeout;
	}


//////////////////////////////////////////////////////////////////////
//
//		Definition of destination constants used for testing.
//
//////////////////////////////////////////////////////////////////////
//

//////////////////////////////////////////////////////////////////////
//
//		11-16-2017	darcy
//
//		Changes for DDOS protection.
//
//		private String ROCKETGATE_TEST_HOST = "dev-gw.rocketgate.com";
//
	private string ROCKETGATE_TEST_HOST = "dev-gateway.rocketgate.com";
//
//////////////////////////////////////////////////////////////////////

	private string ROCKETGATE_TEST_PROTOCOL = "https";
	private int ROCKETGATE_TEST_PORTNO = 443;


//////////////////////////////////////////////////////////////////////
//
//		Definition of destination constants used for production.
//
//////////////////////////////////////////////////////////////////////
//

//////////////////////////////////////////////////////////////////////
//
//		11-16-2017	darcy
//
//		Changes for DDOS protection.
//
//		private String ROCKETGATE_LIVE_HOST = "gw.rocketgate.com";
//
	private static readonly string ROCKETGATE_LIVE_HOST = "gateway.rocketgate.com";
//
//////////////////////////////////////////////////////////////////////

	private static readonly string ROCKETGATE_LIVE_PROTOCOL = "https";
	private static readonly int ROCKETGATE_LIVE_PORTNO = 443;

//////////////////////////////////////////////////////////////////////
//
//		Definition of constant IP addresses used for DNS failover.
//
//////////////////////////////////////////////////////////////////////
//

//////////////////////////////////////////////////////////////////////
//
//		11-16-2017	darcy
//
//		Changes for DDOS protection.
//
//		private String ROCKETGATE_GW16_STRING = "gw-16.rocketgate.com";
//		private String ROCKETGATE_GW17_STRING = "gw-17.rocketgate.com";
//
	private static readonly string ROCKETGATE_GW16_STRING = "gateway-16.rocketgate.com";
	private static readonly string ROCKETGATE_GW17_STRING = "gateway-17.rocketgate.com";
//
//////////////////////////////////////////////////////////////////////

	private static readonly string ROCKETGATE_GW16_IP = "69.20.127.91";
	private static readonly string ROCKETGATE_GW17_IP = "72.32.126.131";

//////////////////////////////////////////////////////////////////////
//
//	Definition of constants required by class.
//
//////////////////////////////////////////////////////////////////////
//
	private string ROCKETGATE_HOST;
	private string ROCKETGATE_PROTOCOL;
	private int ROCKETGATE_PORTNO;
	private int ROCKETGATE_CONNECT_TIMEOUT = 10;
	private int ROCKETGATE_READ_TIMEOUT = 90;

//////////////////////////////////////////////////////////////////////
//
//		Declaration of servlets used credit card transactions.
//
//////////////////////////////////////////////////////////////////////
//
	private static readonly string ROCKETGATE_SERVLET = "gateway/servlet/ServiceDispatcherAccess";

//
//      Define constants for transaction types.
//
	private static readonly string TRANSACTION_CARD_SCRUB = "CARDSCRUB";
	private static readonly string TRANSACTION_CC_AUTH = "CC_AUTH";
	private static readonly string TRANSACTION_CC_TICKET = "CC_TICKET";
	private static readonly string TRANSACTION_CC_SALE = "CC_PURCHASE";
	private static readonly string TRANSACTION_CC_CREDIT = "CC_CREDIT";
	private static readonly string TRANSACTION_CC_VOID = "CC_VOID";
	private static readonly string TRANSACTION_CC_CONFIRM = "CC_CONFIRM";
	private static readonly string TRANSACTION_REBILL_UPDATE = "REBILL_UPDATE";
	private static readonly string TRANSACTION_REBILL_CANCEL = "REBILL_CANCEL";
	private static readonly string TRANSACTION_LOOKUP = "LOOKUP";
	private static readonly string TRANSACTION_CARD_UPLOAD = "CARDUPLOAD";

}