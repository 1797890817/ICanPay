using ICanPay;
using ICanPay.Enums;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Demo.Core.Controllers
{
    public class WapPaymentController : Controller
    {
        private readonly IGateways gateways;

        public WapPaymentController(IGateways gateways)
        {
            this.gateways = gateways;
        }

        public void CreateOrder(GatewayType gatewayType)
        {
            var gateway = gateways.Get(gatewayType, GatewayTradeType.Wap);
            var paymentSetting = new PaymentSetting(gateway);
            paymentSetting.Order = new Order()
            {
                OrderAmount = 0.01,
                OrderNo = DateTime.Now.ToString("yyyyMMddhhmmss"),
                Subject = "WapPayment",
                PaymentDate = DateTime.Now
            };
            paymentSetting.Payment();
        }
    }
}