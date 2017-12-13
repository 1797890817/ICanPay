using ICanPay;
using ICanPay.Enums;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Demo.Core.Controllers
{
    public class AppPaymentController : Controller
    {
        private readonly IGateways gateways;

        public AppPaymentController(IGateways gateways)
        {
            this.gateways = gateways;
        }

        public JsonResult CreateOrder(GatewayType gatewayType)
        {
            var gateway = gateways.Get(gatewayType);
            var paymentSetting = new PaymentSetting(gateway);
            paymentSetting.Order = new Order()
            {
                OrderAmount = 0.01,
                OrderNo = DateTime.Now.ToString("yyyyMMddhhmmss"),
                Subject = "AppPayment",
                PaymentDate = DateTime.Now
            };
            return Json(paymentSetting.BuildPayParams());
        }
    }
}