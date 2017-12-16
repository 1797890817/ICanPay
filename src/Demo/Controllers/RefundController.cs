using ICanPay;
using ICanPay.Enums;
using System.Web.Mvc;

namespace Demo.Controllers
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

            var refund = new Refund();
            refund.OutRefundNo = "000000000000000";
            paymentSetting.BuildRefund(refund);
            paymentSetting.BuildRefundQuery(refund);

        }
    }
}