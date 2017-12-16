using Aop.Api;
using Aop.Api.Domain;
using Aop.Api.Request;
using Aop.Api.Response;
using Aop.Api.Util;
using ICanPay.Enums;
using ICanPay.Interfaces;
using ICanPay.Utils;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace ICanPay.Providers
{
    /// <summary>
    /// 支付宝网关
    /// </summary>
    public sealed class AlipayGateway : GatewayBase, IPaymentForm,  IWapPaymentUrl, IAppParams, IQueryNow, IRefundReq
    {
        #region 私有字段BuildPayParams

        const string payGatewayUrl = "https://mapi.alipay.com/gateway.do";
        const string openapiGatewayUrl = "https://openapi.alipay.com/gateway.do";
        const string emailRegexString = @"^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$";
        Encoding pageEncoding;

        #endregion

        #region 构造函数

        /// <summary>
        /// 初始化支付宝网关
        /// </summary>
        public AlipayGateway()
        {
            pageEncoding = Encoding.GetEncoding(Charset);
        }


        /// <summary>
        /// 初始化支付宝网关
        /// </summary>
        /// <param name="gatewayParameterData">网关通知的数据集合</param>
        public AlipayGateway(List<GatewayParameter> gatewayParameterData)
            : base(gatewayParameterData)
        {
            pageEncoding = Encoding.GetEncoding(Charset);
        }

        #endregion

        #region 属性

        public override GatewayType GatewayType
        {
            get
            {
                return GatewayType.Alipay;
            }
        }

        #endregion

        #region 方法

        public string BuildPaymentForm()
        {
            IAopClient alipayClient = GetAopClient();
            AlipayTradePagePayRequest alipayRequest = new AlipayTradePagePayRequest();// 创建API对应的request
            alipayRequest.SetReturnUrl(Merchant.ReturnUrl.ToString());
            alipayRequest.SetNotifyUrl(Merchant.NotifyUrl.ToString());
            AlipayTradePagePayModel model = new AlipayTradePagePayModel();
            model.Subject = Order.Subject;
            model.OutTradeNo = Order.OrderNo;
            model.TimeoutExpress = "30m";
            model.TotalAmount = Order.OrderAmount.ToString();
            model.ProductCode = "FAST_INSTANT_TRADE_PAY";
            alipayRequest.SetBizModel(model);
            return alipayClient.pageExecute(alipayRequest).Body; // 调用SDK生成表单
        }

        public string BuildWapPaymentUrl(Dictionary<string, string> map)
        {
            IAopClient alipayClient = GetAopClient();
            AlipayTradeWapPayRequest alipayRequest = new AlipayTradeWapPayRequest();
            alipayRequest.SetReturnUrl(Merchant.ReturnUrl.ToString());
            alipayRequest.SetNotifyUrl(Merchant.NotifyUrl.ToString());
            AlipayTradeWapPayModel model = new AlipayTradeWapPayModel();
            model.Subject = Order.Subject;
            model.OutTradeNo = Order.OrderNo;
            model.TimeoutExpress = "30m";
            model.TotalAmount = Order.OrderAmount.ToString();
            model.ProductCode = "QUICK_WAP_PAY";
            alipayRequest.SetBizModel(model);
            return alipayClient.pageExecute(alipayRequest).Body;
        }

        public Dictionary<string, string> BuildPayParams()
        {
            IAopClient alipayClient = GetAopClient(); 
            AlipayTradeAppPayRequest alipayRequest = new AlipayTradeAppPayRequest();
            alipayRequest.SetReturnUrl(Merchant.ReturnUrl.ToString());
            alipayRequest.SetNotifyUrl(Merchant.NotifyUrl.ToString());
            AlipayTradeAppPayModel model = new AlipayTradeAppPayModel();
            model.Subject = Order.Subject;
            model.OutTradeNo = Order.OrderNo;
            model.TimeoutExpress = "30m";
            model.TotalAmount = Order.OrderAmount.ToString();
            model.ProductCode = "QUICK_MSECURITY_PAY";
            alipayRequest.SetBizModel(model);
            Dictionary<string, string> resParam = new Dictionary<string, string>();
            resParam.Add("body", alipayClient.pageExecute(alipayRequest).Body);
            return resParam;
        }

        public bool QueryNow()
        {
            IAopClient alipayClient = GetAopClient(); 
            AlipayTradeQueryRequest alipayRequest = new AlipayTradeQueryRequest();
            AlipayTradeQueryModel model = new AlipayTradeQueryModel();
            model.OutTradeNo = Order.OrderNo;
            alipayRequest.SetBizModel(model);
            AlipayTradeQueryResponse response = alipayClient.Execute(alipayRequest);
            if (((string.Compare(response.TradeStatus, "TRADE_FINISHED") == 0 || string.Compare(response.TradeStatus, "TRADE_SUCCESS") == 0)))
            {
                var orderAmount = double.Parse(response.TotalAmount);
                if (Order.OrderAmount == orderAmount && string.Compare(Order.OrderNo, response.OutTradeNo) == 0)
                {
                    return true;
                }
            }
            return false;
        }

        public Refund BuildRefund(Refund refund)
        {
            IAopClient alipayClient = GetAopClient();
            AlipayTradeRefundRequest alipayRequest = new AlipayTradeRefundRequest();
            AlipayTradeRefundModel model = new AlipayTradeRefundModel();
            model.OutTradeNo = refund.OrderNo;
            if (!string.IsNullOrEmpty(refund.TradeNo))
            {
                model.TradeNo = refund.TradeNo;
            }
            model.OutRequestNo = refund.OutRefundNo;
            model.RefundAmount = refund.RefundAmount.ToString();
            model.RefundReason = refund.RefundDesc;
            alipayRequest.SetBizModel(model);
            AlipayTradeRefundResponse response = alipayClient.Execute(alipayRequest);
            if (response.Code == "10000")
            {
                refund.TradeNo = response.TradeNo;
                refund.RefundStatus = true;
            }
            return refund;
        }

        public Refund BuildRefundQuery(Refund refund)
        {
            IAopClient alipayClient = GetAopClient();
            AlipayTradeFastpayRefundQueryRequest request = new AlipayTradeFastpayRefundQueryRequest();
            AlipayTradeFastpayRefundQueryModel model = new AlipayTradeFastpayRefundQueryModel();
            model.OutTradeNo = refund.OrderNo;
            if (!string.IsNullOrEmpty(refund.TradeNo))
            {
                model.TradeNo = refund.TradeNo;
            }
            model.OutRequestNo = refund.OutRefundNo;
            request.SetBizModel(model);
            AlipayTradeFastpayRefundQueryResponse response = alipayClient.Execute(request);
            if (response.Code == "10000" && !string.IsNullOrEmpty(response.RefundAmount))
            {
                double refundAmount;
                if (double.TryParse(response.RefundAmount, out refundAmount))
                {
                    if (refundAmount > 0.0)
                    {
                        refund.TradeNo = response.TradeNo;
                        refund.RefundAmount = refundAmount;
                        refund.RefundStatus = true;
                    }
                }
            }
            return refund;
        }

        protected override bool CheckNotifyData()
        {
            if (ValidateAlipayNotifyRSASign())
            {
                return ValidateTrade();
            }
            return false;
        }

        public override void WriteSucceedFlag()
        {
            if (PaymentNotifyMethod == PaymentNotifyMethod.ServerNotify)
            {
                HttpUtil.Write("success");
            }
        }

        public IAopClient GetAopClient()
        {
            return new DefaultAopClient(openapiGatewayUrl, Merchant.AppId, Merchant.PrivateKey, "json", Charset, Merchant.PublicKey, "RSA"); 
        }

        /// <summary>
        /// 验证交易状态
        /// </summary>
        /// <returns></returns>
        private bool ValidateTrade()
        {
            var orderAmount = GetGatewayParameterValue("total_amount");
            orderAmount = string.IsNullOrEmpty(orderAmount) ? GetGatewayParameterValue("total_fee") : orderAmount;
            Order.OrderAmount = double.Parse(orderAmount);
            Order.OrderNo = GetGatewayParameterValue("out_trade_no");
            Order.TradeNo = GetGatewayParameterValue("trade_no");
            // 支付状态是否为成功。TRADE_FINISHED（普通即时到账的交易成功状态，TRADE_SUCCESS（开通了高级即时到账或机票分销产品后的交易成功状态）
            if (string.Compare(GetGatewayParameterValue("trade_status"), "TRADE_FINISHED") == 0 ||
                string.Compare(GetGatewayParameterValue("trade_status"), "TRADE_SUCCESS") == 0)
            {              
                return true;
            }
            return false;
        }

        /// <summary>
        /// 验证支付宝通知的签名
        /// </summary>
        private bool ValidateAlipayNotifyRSASign()
        {
            bool checkSign = AlipaySignature.RSACheckV2(GetSortedGatewayParameter(), Merchant.PublicKey, Charset);
            if (checkSign)
            {
                return true;
            }
            return false;
        }
 
        #endregion
    }
}