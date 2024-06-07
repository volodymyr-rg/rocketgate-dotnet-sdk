namespace RocketGateClient;

public static class GatewayCodes
{

//////////////////////////////////////////////////////////////////////
//
//		Declaration of static response codes.
//
//////////////////////////////////////////////////////////////////////
//
	public static readonly int RESPONSE_SUCCESS = 0; // Function succeeded
	public static readonly int RESPONSE_BANK_FAIL = 1;
	// Bank decline/failure
	public static readonly int RESPONSE_RISK_FAIL = 2;
	// Risk failure
	public static readonly int RESPONSE_SYSTEM_ERROR = 3;
	// Hosting system error
	public static readonly int RESPONSE_REQUEST_ERROR = 4;
	// Invalid request

//////////////////////////////////////////////////////////////////////
//
//		Declaration of static reason codes.
//
//////////////////////////////////////////////////////////////////////
//
	public static readonly int REASON_SUCCESS = 0;	// Function succeeded

	public static readonly int REASON_NOMATCHING_XACT = 100;
	public static readonly int REASON_CANNOT_VOID = 101;
	public static readonly int REASON_CANNOT_CREDIT = 102;
	public static readonly int REASON_CANNOT_TICKET = 103;
	public static readonly int REASON_DECLINED = 104;
	public static readonly int REASON_DECLINED_OVERLIMIT = 105;
	public static readonly int REASON_DECLINED_CVV2 = 106;
	public static readonly int REASON_DECLINED_EXPIRED = 107;
	public static readonly int REASON_DECLINED_CALL = 108;
	public static readonly int REASON_DECLINED_PICKUP = 109;
	public static readonly int REASON_DECLINED_EXCESSIVEUSE = 110;
	public static readonly int REASON_DECLINE_INVALID_CARDNO = 111;
	public static readonly int REASON_DECLINE_INVALID_EXPIRATION = 112;
	public static readonly int REASON_BANK_UNAVAILABLE = 113;
	public static readonly int REASON_DECLINED_AVS = 117;
	public static readonly int REASON_CELLPHONE_BLACKLISTED = 126;

	public static readonly int REASON_RISK_FAIL = 200;
	public static readonly int REASON_CUSTOMER_BLOCKED = 201;
	public static readonly int REASON_3DSECURE_AUTHENTICATION_REQUIRED = 202;
	public static readonly int REASON_3DSECURE_NOT_ENROLLED = 203;
	public static readonly int REASON_3DSECURE_UNAVAILABLE = 204;
	public static readonly int REASON_3DSECURE_REJECTED = 205;
		
	public static readonly int REASON_3DSECURE_INITIATION = 225;
	public static readonly int REASON_3DSECURE_FRICTIONLESS_FAILED_AUTH = 227;
	public static readonly int REASON_3DSECURE_SCA_REQUIRED = 228;

	public static readonly int REASON_DNS_FAILURE = 300;
	public static readonly int REASON_UNABLE_TO_CONNECT = 301;
	public static readonly int REASON_REQUEST_XMIT_ERROR = 302;
	public static readonly int REASON_RESPONSE_READ_TIMEOUT = 303;
	public static readonly int REASON_RESPONSE_READ_ERROR = 304;
	public static readonly int REASON_SERVICE_UNAVAILABLE = 305;
	public static readonly int REASON_CONNECTION_UNAVAILABLE = 306;
	public static readonly int REASON_BUGCHECK = 307;
	public static readonly int REASON_UNHANDLED_EXCEPTION = 308;
	public static readonly int REASON_SQL_EXCEPTION = 309;
	public static readonly int REASON_SQL_INSERT_ERROR = 310;
	public static readonly int REASON_BANK_CONNECT_ERROR = 311;
	public static readonly int REASON_BANK_XMIT_ERROR = 312;
	public static readonly int REASON_BANK_READ_ERROR = 313;
	public static readonly int REASON_BANK_DISCONNECT_ERROR = 314;
	public static readonly int REASON_BANK_TIMEOUT_ERROR = 315;
	public static readonly int REASON_BANK_PROTOCOL_ERROR = 316;
	public static readonly int REASON_ENCRYPTION_ERROR = 317;
	public static readonly int REASON_BANK_XMIT_RETRIES = 318;
	public static readonly int REASON_BANK_RESPONSE_RETRIES = 319;
	public static readonly int REASON_BANK_REDUNDANT_RESPONSES = 320;

	public static readonly int REASON_XML_ERROR = 400;
	public static readonly int REASON_INVALID_URL = 401;
	public static readonly int REASON_INVALID_TRANSACTION = 402;
	public static readonly int REASON_INVALID_CARDNO = 403;
	public static readonly int REASON_INVALID_EXPIRATION = 404;
	public static readonly int REASON_INVALID_AMOUNT = 405;
	public static readonly int REASON_INVALID_MERCHANT_ID = 406;
	public static readonly int REASON_INVALID_MERCHANT_ACCOUNT = 407;
	public static readonly int REASON_INCOMPATABLE_CARDTYPE = 408;
	public static readonly int REASON_NO_SUITABLE_ACCOUNT = 409;
	public static readonly int REASON_INVALID_REFGUID = 410;
	public static readonly int REASON_INVALID_ACCESS_CODE = 411;
	public static readonly int REASON_INVALID_CUSTDATA_LENGTH = 412;
	public static readonly int REASON_INVALID_EXTDATA_LENGTH = 413;
	public static readonly int REASON_NO_ACTIVE_MEMBERSHIP = 448;

}