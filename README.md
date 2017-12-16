## ICanPay
https://github.com/hiihellox10/ICanPay 统一支付网关。对原代码优化。支持NET46和NETSTANDARD2_0。支持支付宝，微信，银联支付渠道通过Web，App，Wap，QRCode方式支付。简化订单的创建、查询、退款跟接收网关返回的支付通知等功能

## WebPayment（网站支付）
```
   public void CreateOrder(GatewayType gatewayType)
        {
            var gateway = gateways.Get(gatewayType, GatewayTradeType.Web);
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
```


## WapPayment（手机网站支付）
```
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
```

## QRCodePayment（二维码支付）
```
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
```


## AppPayment（手机APP支付）
```
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
```

## QueryPayment（查询订单）
```
    public void QueryOrder(GatewayType gatewayType)
        {
            var gateway = gateways.Get(gatewayType);
            var querySetting = new PaymentSetting(gateway);

            // 查询时需要设置订单的Id与金额，在查询结果中将会核对订单的Id与金额，如果不相符会返回查询失败。
            querySetting.Order.OrderNo = "20";
            querySetting.Order.OrderAmount = 0.01;

            if (querySetting.QueryNow())
            {
                // 订单已支付
            }
        }
```

## Refund（退款和退款查询）
```
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
```


## Notify（异步通知）
```
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
```