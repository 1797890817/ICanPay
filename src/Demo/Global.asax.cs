using Autofac;
using Autofac.Integration.Mvc;
using ICanPay;
using ICanPay.Events;
using ICanPay.Providers;
using System;
using System.Configuration;
using System.Web.Mvc;
using System.Web.Routing;

namespace Demo
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
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
                gateways.Add(new WeChatPayGataway()
                {
                    Merchant = new Merchant()
                    {
                        AppId = ConfigurationManager.AppSettings["wechat.appid"],
                        Partner = ConfigurationManager.AppSettings["wechat.mch_id"],
                        Key = ConfigurationManager.AppSettings["wechat.Key"],
                        NotifyUrl = new Uri(ConfigurationManager.AppSettings["wechat.notify_url"]),
                        ReturnUrl = new Uri(ConfigurationManager.AppSettings["wechat.return_url"]),
                    }
                });
                gateways.Add(new UnionPayGateway()
                {
                    Merchant = new Merchant()
                    {
                        Partner = ConfigurationManager.AppSettings["sdk.merId"],
                        NotifyUrl = new Uri(ConfigurationManager.AppSettings["frontUrl"]),
                        ReturnUrl = new Uri(ConfigurationManager.AppSettings["backUrl"]),
                    }
                });
                return gateways;
            }).As<IGateways>().InstancePerDependency();

            builder.Register(c => new PaymentNotify(c.Resolve<IGateways>().Merchants)).As<PaymentNotify>().InstancePerDependency();

            //autofac 注册依赖
            IContainer container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));

            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
    }
}
