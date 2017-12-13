using ICanPay;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using ICanPay.Providers;
using System;

namespace Demo.Core
{
    public class Startup
    {
        public IConfigurationRoot Configuration { get; }

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)//增加环境配置文件，新建项目默认有
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.AddSingleton<IConfiguration>(Configuration);

         
            services.AddICanPay(a =>
            {
                var gateways = new Gateways();
                gateways.Add(new AlipayGateway()
                {
                    Merchant = new Merchant()
                    {
                        AppId = Configuration["alipay:appid"],
                        Partner = Configuration["alipay:partner"],
                        Email = Configuration["alipay:seller_email"],
                        Key = Configuration["alipay:key"],
                        PublicKeyPem = Configuration["alipay:publicKeypem"],
                        PrivateKeyPem = Configuration["alipay:privateKeypem"],
                        NotifyUrl = new Uri(Configuration["alipay:notifyurl"]),
                        ReturnUrl = new Uri(Configuration["alipay:returnurl"]),
                    }
                });
                gateways.Add(new WeChatPaymentGataway()
                {
                    Merchant = new Merchant()
                    {
                        AppId = Configuration["wechatpayment:appid"],
                        Partner = Configuration["wechatpayment:mch_id"],
                        Key = Configuration["wechatpayment:key"],
                        NotifyUrl = new Uri(Configuration["wechatpayment:notifyurl"]),
                        ReturnUrl = new Uri(Configuration["wechatpayment:returnurl"]),
                    }
                });
                gateways.Add(new UnionPayGateway()
                {
                    Merchant = new Merchant()
                    {
                        Partner = Configuration["unionpay:merid"],
                        NotifyUrl = new Uri(Configuration["unionpay:notifyurl"]),
                        ReturnUrl = new Uri(Configuration["unionpay:returnurl"]),
                    }
                });
                return gateways;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc(routes =>
            {
                routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });

            app.UseICanPay();
        }
    }
}
