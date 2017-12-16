using Microsoft.AspNetCore.Mvc;
using ICanPay.Enums;
using ICanPay;
using System;

namespace Demo.Core.Controllers
{
    public class WebPaymentController : Controller
    {
        private readonly IGateways gateways;

        public WebPaymentController(IGateways gateways)
        {
            this.gateways = gateways;
        }

        public void CreateOrder(GatewayType gatewayType)
        {
            var gateway = gateways.Get(gatewayType, GatewayTradeType.Web);
            var paymentSetting = new PaymentSetting(gateway);
            paymentSetting.Order = new Order()
            {
                OrderAmount = 0.01,
                OrderNo = DateTime.Now.ToString("yyyyMMddhhmmss"),
                Subject = "WebPayment",
                PaymentDate = DateTime.Now
            };
            paymentSetting.Payment();
        }
    }
}