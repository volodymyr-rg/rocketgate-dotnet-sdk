using RocketGateClient;

namespace RocketGateClientTests;

abstract class BaseTestCase
{
    /**
     * @var GatewayService
     */
    protected GatewayService service;

    /**
     * @var GatewayRequest
     */
    protected GatewayRequest request;

    /**
     * @var GatewayResponse
     */
    protected GatewayResponse response;

    /**
     * @var int
     */
    protected int merchantId = 1;

    /**
     * @var string
     */
    protected string merchantPassword = "testpassword";

    /**
     * @var string
     */
    protected string customerId;

    /**
     * @var string
     */
    protected string invoiceId;

    protected BaseTestCase()
    {
        // parent::setUp();

        // service  = new GatewayService(true);
        service = new GatewayService();
        service.SetTestMode(true);
        response = new GatewayResponse();
        request = new GatewayRequest();

        // Merchant data
        request.Set(GatewayRequest.MERCHANT_ID, merchantId);
        request.Set(GatewayRequest.MERCHANT_PASSWORD, merchantPassword);

        // Customer data
        request.Set(GatewayRequest.CUSTOMER_FIRSTNAME, "Joe");
        request.Set(GatewayRequest.CUSTOMER_LASTNAME, "DotNetTester");
        request.Set(GatewayRequest.EMAIL, "dotnettest@fakedomain.com");

        // Credit card data
        request.Set(GatewayRequest.CARDNO, "4111111111111111");
        request.Set(GatewayRequest.EXPIRE_MONTH, "02");
        request.Set(GatewayRequest.EXPIRE_YEAR, "2010");
        request.Set(GatewayRequest.CVV2, "999");

        long time = Time();
        customerId = time + ".DotNetTest";
        invoiceId = time + "." + GetTestName();

        request.Set(GatewayRequest.MERCHANT_CUSTOMER_ID, customerId);
        request.Set(GatewayRequest.MERCHANT_INVOICE_ID, invoiceId);
    }

    protected static long Time()
    {
     return DateTimeOffset.Now.ToUnixTimeMilliseconds()/1000;
    }

    protected abstract string GetTestName();
}