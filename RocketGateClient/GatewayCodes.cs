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
	public static readonly int REASON_USER_BUSY = 119;
	public static readonly int REASON_INVALID_REGION = 120;
	public static readonly int REASON_UNKNOWN_CARRIER = 121;
	public static readonly int REASON_USER_DECLINED = 123;
	public static readonly int REASON_NETWORK_MISMATCH = 125;
	public static readonly int REASON_CELLPHONE_BLACKLISTED = 126;
	public static readonly int REASON_FULL_FAILURE = 127;
	public static readonly int REASON_DECLINED_PIN = 129;
	public static readonly int REASON_DECLINED_AVS_AUTOVOID = 150;
	public static readonly int REASON_DECLINED_CVV2_AUTOVOID = 151;
	public static readonly int REASON_INVALID_TICKET_AMT = 152;
	public static readonly int REASON_NO_SUCH_FILE = 153;
	public static readonly int REASON_INTEGRATION_ERROR = 154;
	public static readonly int REASON_DECLINED_CAVV = 155;
	public static readonly int REASON_UNSUPPORTED_CARDTYPE = 156;
	public static readonly int REASON_DECLINED_RISK = 157;
	public static readonly int REASON_INVALID_DEBIT_ACCOUNT = 158;
	public static readonly int REASON_INVALID_USER_DATA = 159;
	public static readonly int REASON_AUTH_HAS_EXPIRED = 160;
	public static readonly int REASON_PREVIOUS_HARD_DECLINE = 161;
	public static readonly int REASON_MERCHACCT_LIMIT = 162;
	public static readonly int REASON_DECLINED_CAVV_AUTOVOID = 163;
	public static readonly int REASON_DECLINED_STOLEN = 164;
	public static readonly int REASON_BANK_INVALID_TRANSACTION = 165;
	public static readonly int REASON_REFER_TO_MAKER = 166;
	public static readonly int REASON_CVV2_REQUIRED = 167;
	public static readonly int REASON_INVALID_TAX_ID = 169;

	public static readonly int REASON_RISK_FAIL = 200;
	public static readonly int REASON_CUSTOMER_BLOCKED = 201;
	public static readonly int REASON_3DSECURE_AUTHENTICATION_REQUIRED = 202;
	public static readonly int REASON_3DSECURE_NOT_ENROLLED = 203;
	public static readonly int REASON_3DSECURE_UNAVAILABLE = 204;
	public static readonly int REASON_3DSECURE_REJECTED = 205;
	public static readonly int REASON_RISK_AVS_VS_ISSUER = 207;
	public static readonly int REASON_RISK_DUPLICATE_MEMBERSHIP  = 208;
	public static readonly int REASON_RISK_DUPLICATE_CARD = 209;
	public static readonly int REASON_RISK_DUPLICATE_EMAIL = 210;
	public static readonly int REASON_RISK_EXCEEDED_MAX_PURCHASE = 211;
	public static readonly int REASON_RISK_DUPLICATE_PURCHASE = 212;
	public static readonly int REASON_RISK_VELOCITY_CUSTOMER = 213;
	public static readonly int REASON_RISK_VELOCITY_CARD = 214;
	public static readonly int REASON_RISK_VELOCITY_EMAIL = 215;
	public static readonly int REASON_IOVATION_DECLINE = 216;
	public static readonly int REASON_RISK_VELOCITY_DEVICE = 217;
	public static readonly int REASON_RISK_DUPLICATE_DEVICE = 218;
	public static readonly int REASON_RISK_1CLICK_SOURCE = 219;
	public static readonly int REASON_RISK_TOOMANYCARDS = 220;
	public static readonly int REASON_AFFILIATE_BLOCKED = 221;
	public static readonly int REASON_TRIAL_ABUSE = 222;
	public static readonly int REASON_3DSECURE_BYPASS = 223;
	public static readonly int REASON_RISK_NEWCARD_NODEVICE = 224;
	public static readonly int REASON_3DSECURE_INITIATION = 225;
	public static readonly int REASON_3DSECURE_FRICTIONLESS_FAILED_AUTH = 227;
	public static readonly int REASON_3DSECURE_SCA_REQUIRED = 228;
	public static readonly int REASON_3DSECURE_CARDHOLDER_CANCEL = 229;
	public static readonly int REASON_3DSECURE_ACS_TIMEOUT = 230;
	public static readonly int REASON_3DSECURE_INVALID_CARD = 231;
	public static readonly int REASON_3DSECURE_INVALID_TRANSACTION = 232;
	public static readonly int REASON_3DSECURE_ACS_TECHNICAL_ISSUE = 233;
	public static readonly int REASON_3DSECURE_EXCEEDS_MAX_CHALLENGES = 234;

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
	public static readonly int REASON_WEBSERVICE_FAILURE = 321;
	public static readonly int REASON_PROCESSOR_BACKEND_FAILURE = 322;
	public static readonly int REASON_JSON_FAILURE = 323;
	public static readonly int REASON_GPG_FAILURE = 324;
	public static readonly int REASON_3DS_SYSTEM_FAILURE = 325;
	public static readonly int REASON_USE_DIFFERENT_SERVER = 326;

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
	public static readonly int REASON_INVALID_CUSTOMER_ID = 414;
	public static readonly int REASON_INVALID_CURRENCY = 418;
	public static readonly int REASON_INCOMPATIBLE_CURRENCY = 419;
	public static readonly int REASON_INVALID_REBILL_ARGS = 420;
	public static readonly int REASON_INVALID_PHONE = 421;
	public static readonly int REASON_INVALID_COUNTRY_CODE = 422;
	public static readonly int REASON_INVALID_BILLING_MODE = 423;
	public static readonly int REASON_INCOMPATABLE_COUNTRY = 424;
	public static readonly int REASON_INVALID_TIMEOUT = 425;
	public static readonly int REASON_INVALID_ACCOUNT_NO = 426;
    public static readonly int REASON_INVALID_ROUTING_NO = 427;
    public static readonly int REASON_INVALID_LANGUAGE_CODE = 428;
    public static readonly int REASON_INVALID_BANK_NAME = 429;
    public static readonly int REASON_INVALID_BANK_CITY = 430;
    public static readonly int REASON_INVALID_CUSTOMER_NAME = 431;
    public static readonly int REASON_INVALID_BANKDATA_LENGTH = 432;
    public static readonly int REASON_INVALID_PIN_NO = 433;
    public static readonly int REASON_INVALID_PHONE_NO = 434;
    public static readonly int REASON_INVALID_ACCOUNT_HOLDER = 435;
    public static readonly int REASON_INCOMPATIBLE_DESCRIPTORS = 436;
    public static readonly int REASON_INVALID_REFERRAL_DATA = 437;
    public static readonly int REASON_INVALID_SITEID = 438;
    public static readonly int REASON_DUPLICATE_INVOICE_ID = 439;
    public static readonly int REASON_EXISTING_MEMBERSHIP  = 440;
    public static readonly int REASON_INVOICE_NOT_FOUND = 441;
	public static readonly int REASON_MISSING_CUSTOMER_ID = 443;
    public static readonly int REASON_MISSING_CUSTOMER_NAME = 444;
    public static readonly int REASON_MISSING_CUSTOMER_ADDRESS = 445;
	public static readonly int REASON_MISSING_CVV2 = 446;
	public static readonly int REASON_MISSING_PARES = 447;
	public static readonly int REASON_NO_ACTIVE_MEMBERSHIP = 448;
	public static readonly int REASON_INVALID_CVV2 = 449;
	public static readonly int REASON_INVALID_3D_DATA = 450;
	public static readonly int REASON_INVALID_CLONE_DATA = 451;
	public static readonly int REASON_REDUNDANT_SUSPEND_OR_RESUME = 452;
	public static readonly int REASON_INVALID_PAYINFO_TRANSACT_ID = 453;
	public static readonly int REASON_INVALID_CAPTURE_DAYS = 454;
	public static readonly int REASON_INVALID_SUBMERCHANT_ID = 455;
	public static readonly int REASON_INVALID_COF_FRAMEWORK = 458;
	public static readonly int REASON_INVALID_REFERENCE_SCHEME_TRANSACTION = 459;
	public static readonly int REASON_INVALID_CUSTOMER_ADDRESS = 460;
	public static readonly int REASON_INVALID_BUILD_PAYMENT_LINK_REQUEST = 462;
	public static readonly int REASON_INVALID_SS_NUMBER = 463;
}