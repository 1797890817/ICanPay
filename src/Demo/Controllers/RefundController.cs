using ICanPay;
using ICanPay.Enums;

namespace Demo.Controllers
{
    public class RefundController
    {
        public void Refund()
        {
            PaymentSetting querySetting = new PaymentSetting(GatewayType.WeChatPayment);
            querySetting.Merchant.AppId = "wx000000000000000";
            querySetting.Merchant.Partner = "000000000000000";
            querySetting.Merchant.Key = "0000000000000000000000000000000000000000";
            if (querySetting.CanRefund)
            { 
                var refund = new Refund();
                refund.RefoundNo = "000000000000000";
                querySetting.BuildRefund(refund);
                querySetting.BuildRefundQuery(refund);
            }
        }
    }
}