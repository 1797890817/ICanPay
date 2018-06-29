## ICanPay
https://github.com/hiihellox10/ICanPay 统一支付网关。对原代码优化。支持NET46和NETSTANDARD2_0。支持支付宝，微信，银联支付渠道通过Web，App，Wap，QRCode方式支付。简化订单的创建、查询、退款跟接收网关返回的支付通知等功能

## 初始网关信息

NET46，需依赖AuotoFac组件
```
    var builder = new ContainerBuilder();
	builder.RegisterControllers(typeof(MvcApplication).Assembly);
	builder.Register(c =>
	{
		var gateways = new Gateways();
		gateways.Add(new AlipayGateway()
		{
			Merchant = new Merchant()
			{
				AppId = ConfigurationManager.AppSettings["alipay.appid"],
				Partner = ConfigurationManager.AppSettings["alipay.partner"],
				Email = ConfigurationManager.AppSettings["alipay.seller_email"],
				Key = ConfigurationManager.AppSettings["alipay.key"],
				PublicKey = ConfigurationManager.AppSettings["alipay.publicKey"],
				PrivateKey = ConfigurationManager.AppSettings["alipay.privateKey"],
				NotifyUrl = new Uri(ConfigurationManager.AppSettings["alipay.notify_url"]),
				ReturnUrl = new Uri(ConfigurationManager.AppSettings["alipay.return_url"]),
			}
		});               
	   return gateways;
	}).As<IGateways>().InstancePerDependency();
	
	builder.Register(c => new PaymentNotify(c.Resolve<IGateways>().Merchants)).As<PaymentNotify>().InstancePerDependency();
	
	//autofac 注册依赖
	IContainer container = builder.Build();
	DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
```
NETSTANDARD2_0，在Startup初始化
```
    public void ConfigureServices(IServiceCollection services)
	{
		 services.AddMvc();

		services.AddSingleton<IConfiguration>(Configuration);

		services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

		services.AddTransient(c => {
			IGateways gateways = new Gateways();
			gateways.Add(new AlipayGateway()
			{
				Merchant = new Merchant()
				{
					AppId = Configuration["alipay:appid"],
					Partner = Configuration["alipay:partner"],
					Email = Configuration["alipay:seller_email"],
					Key = Configuration["alipay:key"],
					PublicKey = Configuration["alipay:publicKey"],
					PrivateKey = Configuration["alipay:privateKey"],
					NotifyUrl = new Uri(Configuration["alipay:notifyurl"]),
					ReturnUrl = new Uri(Configuration["alipay:returnurl"]),
				}
			});
			return gateways;
		});

		services.AddTransient(c=>new PaymentNotify(c.GetService<IGateways>().Merchants));
	}
```

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

		var refund = new Refund();
		refund.OutRefundNo = "000000000000000";
		paymentSetting.BuildRefund(refund);
		paymentSetting.BuildRefundQuery(refund);
	}
```


## Notify（异步通知）
```
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
```