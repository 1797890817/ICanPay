## ICanPay
https://github.com/hiihellox10/ICanPay 统一支付网关。对原代码优化。支持支付宝，微信，银联支付渠道通过Web，App，Wap方式支付。简化订单的创建、查询跟接收网关返回的支付通知等功能


## WebPayment
```
 public void CreateOrder(GatewayType gatewayType)
        {
            PaymentSetting paymentSetting = new PaymentSetting(gatewayType);
            paymentSetting.Merchant.AppId = "appid000000000000000";
            paymentSetting.Merchant.Email = "yourname@address.com";
            paymentSetting.Merchant.Partner = "000000000000000";
            paymentSetting.Merchant.Key = "000000000000000000000000000000000000000000";
            paymentSetting.Merchant.NotifyUrl = new Uri("http://yourwebsite.com/Notify.aspx");
            paymentSetting.Merchant.ReturnUrl = new Uri("http://yourwebsite.com/Return.aspx");

            paymentSetting.Order.OrderAmount = 0.01;
            paymentSetting.Order.OrderNo = "35";
            paymentSetting.Order.Subject = "WebPayment";
            paymentSetting.Payment();
        }
```


## WapPayment
```
  public void CreateOrder(GatewayType gatewayType)
        {
            PaymentSetting paymentSetting = new PaymentSetting(gatewayType);
            paymentSetting.Merchant.AppId = "appid000000000000000";
            paymentSetting.Merchant.Email = "yourname@address.com";
            paymentSetting.Merchant.Partner = "000000000000000";
            paymentSetting.Merchant.Key = "000000000000000000000000000000000000000000";           
            paymentSetting.Merchant.PrivateKeyPem = "yourrsa_private_key.pem";
            paymentSetting.Merchant.PublicKeyPem = "yourrsa_public_key.pem";
            paymentSetting.Merchant.NotifyUrl = new Uri("http://yourwebsite.com/Notify.aspx");
            paymentSetting.Merchant.ReturnUrl = new Uri("http://yourwebsite.com/Return.aspx");

            paymentSetting.Order.OrderAmount = 0.01;
            paymentSetting.Order.OrderNo = "35";
            paymentSetting.Order.Subject = "WapPayment";
            paymentSetting.WapPayment();
        }
```


## AppPayment
```
   public JsonResult CreateOrder(GatewayType gatewayType)
        {
            PaymentSetting paymentSetting = new PaymentSetting(gatewayType);
            paymentSetting.Merchant.AppId = "appid000000000000000";
            paymentSetting.Merchant.Email = "yourname@address.com";
            paymentSetting.Merchant.Partner = "000000000000000";
            paymentSetting.Merchant.Key = "000000000000000000000000000000000000000000";
            paymentSetting.Merchant.PrivateKeyPem = "yourrsa_private_key.pem";
            paymentSetting.Merchant.PublicKeyPem = "yourrsa_public_key.pem";
            paymentSetting.Merchant.NotifyUrl = new Uri("http://yourwebsite.com/Notify.aspx");

            paymentSetting.Order.OrderAmount = 0.01;
            paymentSetting.Order.OrderNo = "35";
            paymentSetting.Order.Subject = "AppPayment";
            return Json(paymentSetting.BuildPayParams()) ;
        }
```

