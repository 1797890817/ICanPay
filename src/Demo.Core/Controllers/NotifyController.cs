using ICanPay.Enums;
using ICanPay.Events;
using Microsoft.AspNetCore.Mvc;

namespace Demo.Core.Controllers
{
    public class NotifyController : Controller
    {     
        private readonly PaymentNotify paymentNotify;

        public NotifyController(PaymentNotify paymentNotify)
        {
            this.paymentNotify = paymentNotify;
            paymentNotify.PaymentSucceed += @event => {
                //支付成功时时的处理代码
                if (@event.PaymentNotifyMethod == PaymentNotifyMethod.AutoReturn)
                {
                    //当前是用户的浏览器自动返回时显示充值成功页面
                }
                else
                {
                    //支付结果的发送方式，以服务端接收为准
                }
            };

            paymentNotify.PaymentFailed += @event=> {
                //支付失败时的处理代码
            };

            paymentNotify.UnknownGateway += @event => {
                //无法识别支付网关时的处理代码
            };
        }

    
        public void ServerNotify()
        {          
            //接收并处理支付通知
            paymentNotify.Received(PaymentNotifyMethod.ServerNotify);
        }

        public void AutoReturn()
        {
            //接收并处理支付通知
            paymentNotify.Received(PaymentNotifyMethod.AutoReturn);
        }
    }
}