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

class NonRebillUpdateToRebillTest : BaseTestCase
{
    protected override string GetTestName()
    {
        return "UpgrdToRebillTest";
    }

    [Test]
    public void Test()
    {
        request.Set(GatewayRequest.CURRENCY, "USD");
        request.Set(GatewayRequest.AMOUNT, "29.99"); // bill 9.99 now
        request.Set(GatewayRequest.REBILL_FREQUENCY, "MONTHLY"); // a monthly rebill
        // set w/0 rebills results in 1 month non-recurring membership
        request.Set(GatewayRequest.REBILL_COUNT, "0");

        request.Set(GatewayRequest.BILLING_ADDRESS, "123 Main St");
        request.Set(GatewayRequest.BILLING_CITY, "Las Vegas");
        request.Set(GatewayRequest.BILLING_STATE, "NV");
        request.Set(GatewayRequest.BILLING_ZIPCODE, "89141");
        request.Set(GatewayRequest.BILLING_COUNTRY, "US");

        request.Set(GatewayRequest.CARDNO, "4111111111111111");
        request.Set(GatewayRequest.EXPIRE_MONTH, "02");
        request.Set(GatewayRequest.EXPIRE_YEAR, "2010");
        request.Set(GatewayRequest.CVV2, "999");


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

// UPGRADE MEMBERSHIP
//
//  This would normally be two separate processes,
//  but for example's sake is in one process (thus we clear and set a new GatewayRequest object)
//  The key values required are MERCHANT_CUSTOMER_ID and MERCHANT_INVOICE_ID.
//
//  Modify to 19.95/month recurring
//
        request = new GatewayRequest();
        request.Set(GatewayRequest.MERCHANT_ID, merchantId);
        request.Set(GatewayRequest.MERCHANT_PASSWORD, merchantPassword);

        request.Set(GatewayRequest.MERCHANT_CUSTOMER_ID, customerId);
        request.Set(GatewayRequest.MERCHANT_INVOICE_ID, invoiceId);

        request.Set(GatewayRequest.REBILL_AMOUNT, "19.95");
        request.Set(GatewayRequest.REBILL_FREQUENCY, "MONTHLY"); // a monthly rebill
        request.Set(GatewayRequest.REBILL_END_DATE, "CLEAR"); // Clear previous end date

        Assert.That(
            service.PerformRebillUpdate(request, response), Is.True,
            "Update to Recurring"
        );
    }
}