using ICanPay;
using ICanPay.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Demo.Core.Controllers
{
    public class RefundController : Controller
    {
        private readonly IGateways gateways;

        public RefundController(IGateways gateways)
        {
            this.gateways = gateways;
        }

        public void CreateRefund(GatewayType gatewayType)
        {
            var gateway = gateways.Get(gatewayType);
            var paymentSetting = new PaymentSetting(gateway);

            if (paymentSetting.CanRefund)
            {
                var refund = new Refund();
                refund.OutRefundNo = "000000000000000";
                paymentSetting.BuildRefund(refund);
                paymentSetting.BuildRefundQuery(refund);
            }
        }

    }
}