/*
 * Copyright notice:
 * (c) Copyright 2024 RocketGate
 * All rights reserved.
 *
 * The copyright notice must not be removed without specific, prior
 * written permission from RocketGate.
 *
 * This software is protected as an unpublished work under the U.S. copyright
 * laws. The above copyright notice is not intended to effect a publication of
 * this work.
 * This software is the confidential and proprietary information of RocketGate.
 * Neither the binaries nor the source code may be redistributed without prior
 * written permission from RocketGate.
 *
 * The software is provided "as-is" and without warranty of any kind, express, implied
 * or otherwise, including without limitation, any warranty of merchantability or fitness
 * for a particular purpose.  In no event shall RocketGate be liable for any direct,
 * special, incidental, indirect, consequential or other damages of any kind, or any damages
 * whatsoever arising out of or in connection with the use or performance of this software,
 * including, without limitation, damages resulting from loss of use, data or profits, and
 * whether or not advised of the possibility of damage, regardless of the theory of liability.
 *
 */

using RocketGateClient;

namespace RocketGateClientTests;

class ThreeDSecureTest : BaseTestCase
{
    protected override string GetTestName()
    {
        return "3DSecureTest";
    }

    [Test]
    public void Test()
    {
        long time = Time();
        string cust_id = time + ".DotNetTest";
        string inv_id = time + ".3DSTestiNov24";


        request.Set(GatewayRequest.MERCHANT_CUSTOMER_ID, cust_id);
        request.Set(GatewayRequest.MERCHANT_INVOICE_ID, inv_id);

        request.Set(GatewayRequest.CURRENCY, "USD");
        request.Set(GatewayRequest.AMOUNT, "9.99"); // bill 9.99 now

        request.Set(GatewayRequest.CARDNO, "4111111111111111");
        request.Set(GatewayRequest.EXPIRE_MONTH, "01");
        request.Set(GatewayRequest.EXPIRE_YEAR, "2030");
        request.Set(GatewayRequest.CVV2, "999");

        request.Set(GatewayRequest.CUSTOMER_FIRSTNAME, "Joe");
        request.Set(GatewayRequest.CUSTOMER_LASTNAME, "DotNetTester");
        request.Set(GatewayRequest.EMAIL, "dotnettest@fakedomain.com");

        request.Set(GatewayRequest.BILLING_ADDRESS, "123 Main St");
        request.Set(GatewayRequest.BILLING_CITY, "Las Vegas");
        request.Set(GatewayRequest.BILLING_STATE, "NV");
        request.Set(GatewayRequest.BILLING_ZIPCODE, "89141");
        request.Set(GatewayRequest.BILLING_COUNTRY, "US");
        request.Set(GatewayRequest.MERCHANT_ACCOUNT, "59"); // 3DS 1.0 MID.

        // Risk/Scrub Request Setting
        request.Set(GatewayRequest.SCRUB, "IGNORE");
        request.Set(GatewayRequest.CVV2_CHECK, "IGNORE");
        request.Set(GatewayRequest.AVS_CHECK, "IGNORE");

        // Request 3DS
        request.Set(GatewayRequest.USE_3D_SECURE, "TRUE");
        request.Set(GatewayRequest.BROWSER_USER_AGENT,
            "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_14_3) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/72.0.3626.96 Safari/537.36");
        request.Set(GatewayRequest.BROWSER_ACCEPT_HEADER,
            "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");

        //
        //	Perform the Lookup transaction.
        //
        service.PerformPurchase(request, response);

        string reason_code = response.Get(GatewayResponse.REASON_CODE); // reason code 202 is expected
        Assert.That(reason_code == "202", Is.True,
            "Perform 3D Lookup"
        );

        //
        //	Setup the 2nd request.
        //
        request = new GatewayRequest();
        request.Set(GatewayRequest.MERCHANT_ID, merchantId);
        request.Set(GatewayRequest.MERCHANT_PASSWORD, merchantPassword);

        request.Set(GatewayRequest.CVV2, "999");

        request.Set(GatewayRequest.REFERENCE_GUID, response.Get(GatewayResponse.TRANSACT_ID));

        // In a real transaction this would include the PARES returned from the Authentication
        // On dev we send through the SimulatedPARES + TRANSACT_ID
        string pares = "SimulatedPARES" + response.Get(GatewayResponse.TRANSACT_ID);
        request.Set(GatewayRequest.PARES, pares);

        // Risk/Scrub Request Setting
        request.Set(GatewayRequest.SCRUB, "IGNORE");
        request.Set(GatewayRequest.CVV2_CHECK, "IGNORE");
        request.Set(GatewayRequest.AVS_CHECK, "IGNORE");

        //
        //	Perform the Purchase transaction.
        //
        Assert.That(
            service.PerformPurchase(request, response), Is.True,
            "Perform Purchase"
        );
    }
}