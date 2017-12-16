using ICanPay;
using ICanPay.Enums;
using System;
using System.Web.Mvc;

namespace Demo.Controllers
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
            var gateway = gateways.Get(gatewayType, GatewayTradeType.APP);
            var paymentSetting = new PaymentSetting(gateway);
            paymentSetting.Order = new Order()
            {
                OrderAmount = 0.01,
                OrderNo = DateTime.Now.ToString("yyyyMMddhhmmss"),
                Subject = "AppPayment",
                PaymentDate = DateTime.Now
            };
            return Json(paymentSetting.Payment());
        }
    }
}