using ICanPay;
using ICanPay.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Demo.Core.Controllers
{
    public class QueryPaymentController : Controller
    {
        private readonly IGateways gateways;

        public QueryPaymentController(IGateways gateways)
        {
            this.gateways = gateways;
        }

        // GET: QueryPayment
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
    }
}