using ICanPay;
using ICanPay.Enums;
using ICanPay.Events;
using Microsoft.AspNetCore.Mvc;

namespace Demo.Core.Controllers
{
    public class NotifyController : Controller
    {
        private readonly IGateways gateways;

        private PaymentNotify notify;

        public NotifyController(IGateways gateways)
        {
            this.gateways = gateways;

            notify = new PaymentNotify(gateways.Merchants);
            notify.PaymentSucceed += new PaymentSucceedEventHandler(notify_PaymentSucceed);
            notify.PaymentFailed += new PaymentFailedEventHandler(notify_PaymentFailed);
            notify.UnknownGateway += new UnknownGatewayEventHandler(notify_UnknownGateway);
        }

    
        public void ServerNotify()
        {          
            // 接收并处理支付通知
            notify.Received(PaymentNotifyMethod.ServerNotify);
        }

        public void AutoReturn()
        {
            // 接收并处理支付通知
            notify.Received(PaymentNotifyMethod.AutoReturn);
        }

        private void notify_PaymentSucceed(object sender, PaymentSucceedEventArgs e)
        {
            // 支付成功时时的处理代码
            if (e.PaymentNotifyMethod == PaymentNotifyMethod.AutoReturn)
            {
                // 当前是用户的浏览器自动返回时显示充值成功页面
            }
            else
            {
                // 支付结果的发送方式，以服务端接收为准
                
            }
        }

        private void notify_PaymentFailed(object sender, PaymentFailedEventArgs e)
        {
            // 支付失败时的处理代码
        }

        private void notify_UnknownGateway(object sender, UnknownGatewayEventArgs e)
        {
            // 无法识别支付网关时的处理代码
        }
    }

}