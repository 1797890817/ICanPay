using ICanPay.Enums;
using ICanPay;
using System;
using System.Web.Mvc;

namespace Demo.Controllers
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
            var gateway = gateways.Get(gatewayType);
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