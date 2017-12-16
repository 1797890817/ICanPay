using ICanPay;
using ICanPay.Enums;
using System;
using System.Web.Mvc;

namespace Demo.Controllers
{
    public class QRCodePaymentController : Controller
    {
        private readonly IGateways gateways;

        public QRCodePaymentController(IGateways gateways)
        {
            this.gateways = gateways;
        }

        public void CreateOrder(GatewayType gatewayType)
        {
            var gateway = gateways.Get(gatewayType, GatewayTradeType.QRCode);
            var paymentSetting = new PaymentSetting(gateway);
            paymentSetting.Order = new Order()
            {
                OrderAmount = 0.01,
                OrderNo = DateTime.Now.ToString("yyyyMMddhhmmss"),
                Subject = "QRCodePayment",
                PaymentDate = DateTime.Now
            };
            paymentSetting.Payment();
        }
    }
}