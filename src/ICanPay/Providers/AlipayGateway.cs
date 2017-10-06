using Aop.Api;
using Aop.Api.Domain;
using Aop.Api.Request;
using Aop.Api.Response;
using Aop.Api.Util;
using ICanPay.Enums;
using ICanPay.Interfaces;
using ICanPay.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;


namespace ICanPay.Providers
{
    /// <summary>
    /// ֧��������
    /// </summary>
    public sealed class AlipayGateway : GatewayBase, IPaymentForm,  IWapPaymentUrl, IAppParams, IQueryNow
    {

        #region ˽���ֶ�BuildPayParams

        const string payGatewayUrl = "https://mapi.alipay.com/gateway.do";
        const string openapiGatewayUrl = "https://openapi.alipay.com/gateway.do";
        const string emailRegexString = @"^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$";
        Encoding pageEncoding;

        #endregion


        #region ���캯��

        /// <summary>
        /// ��ʼ��֧��������
        /// </summary>
        public AlipayGateway()
        {
            pageEncoding = Encoding.GetEncoding(Charset);
        }


        /// <summary>
        /// ��ʼ��֧��������
        /// </summary>
        /// <param name="gatewayParameterData">����֪ͨ�����ݼ���</param>
        public AlipayGateway(List<GatewayParameter> gatewayParameterData)
            : base(gatewayParameterData)
        {
            pageEncoding = Encoding.GetEncoding(Charset);
        }

        #endregion


        #region ����

        public override GatewayType GatewayType
        {
            get
            {
                return GatewayType.Alipay;
            }
        }

        #endregion


        #region ����

        public string BuildPaymentForm()
        {
            IAopClient alipayClient = new DefaultAopClient(openapiGatewayUrl,
               Merchant.AppId, Merchant.PrivateKeyPem,
                "json", Charset, Merchant.PublicKeyPem, "RSA"); // ��ó�ʼ����AlipayClient

            AlipayTradePagePayRequest alipayRequest = new AlipayTradePagePayRequest();// ����API��Ӧ��request
            alipayRequest.SetReturnUrl(Merchant.ReturnUrl.ToString());
            alipayRequest.SetNotifyUrl(Merchant.NotifyUrl.ToString());

            AlipayTradePagePayModel model = new AlipayTradePagePayModel();
            model.Subject = Order.Subject;
            model.OutTradeNo = Order.OrderNo;
            model.TimeoutExpress = "30m";
            model.TotalAmount = Order.OrderAmount.ToString();
            model.ProductCode = "FAST_INSTANT_TRADE_PAY";
            alipayRequest.SetBizModel(model);

            return alipayClient.pageExecute(alipayRequest).Body; // ����SDK���ɱ�
        }

        //public string BuildPaymentForm()
        //{        
        //    InitOrderParameter("MD5");
        //    ValidatePaymentOrderParameter();
        //    return GetFormHtml(payGatewayUrl);
        //}


        public string BuildWapPaymentUrl(Dictionary<string, string> map)
        {
            IAopClient alipayClient = new DefaultAopClient(openapiGatewayUrl,
             Merchant.AppId, Merchant.PrivateKeyPem,
              "json", Charset, Merchant.PublicKeyPem, "RSA"); // ��ó�ʼ����AlipayClient

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
            IAopClient alipayClient = new DefaultAopClient(openapiGatewayUrl,
            Merchant.AppId, Merchant.PrivateKeyPem,
             "json", Charset, Merchant.PublicKeyPem, "RSA"); // ��ó�ʼ����AlipayClient

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

        //public Dictionary<string, string> BuildPayParams()
        //{
        //    SetGatewayParameterValue("app_id", Merchant.AppId);
        //    SetGatewayParameterValue("method", "alipay.trade.app.pay");
        //    SetGatewayParameterValue("charset", Charset);
        //    SetGatewayParameterValue("sign_type", "RSA");
        //    SetGatewayParameterValue("timestamp", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        //    SetGatewayParameterValue("version", "1.0");
        //    SetGatewayParameterValue("notify_url", Merchant.NotifyUrl);
        //    SetGatewayParameterValue("biz_content",
        //        JsonConvert.SerializeObject(new
        //        {
        //            subject = Order.Subject,
        //            out_trade_no = Order.OrderNo,
        //            total_amount = Order.OrderAmount,
        //            product_code = "QUICK_MSECURITY_PAY"
        //        }));

        //    SetGatewayParameterValue("sign", AopUtils.SignAopRequest(GetSortedGatewayParameter(), Merchant.PrivateKeyPem, Charset, true, "RSA"));

        //    StringBuilder signBuilder = new StringBuilder();
        //    foreach (KeyValuePair<string, string> item in GetSortedGatewayParameter())
        //    {
        //        signBuilder.AppendFormat("{0}={1}&", item.Key, HttpUtility.UrlEncode(item.Value, Encoding.UTF8));
        //    }
        //    Dictionary<string, string> resParam = new Dictionary<string, string>();
        //    resParam.Add("body", signBuilder.ToString().TrimEnd('&'));
        //    return resParam;
        //}

        public bool QueryNow(ProductSet productSet)
        {
            IAopClient alipayClient = new DefaultAopClient(openapiGatewayUrl,
            Merchant.AppId, Merchant.PrivateKeyPem,
             "json", Charset, Merchant.PublicKeyPem, "RSA"); // ��ó�ʼ����AlipayClient

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
                HttpContext.Current.Response.Write("success");
            }
        }

        /// <summary>
        /// ��ʼ����������
        /// </summary>
        private void InitOrderParameter(string sign_type)
        {
            SetGatewayParameterValue("seller_email", Merchant.Email);
            SetGatewayParameterValue("service", "create_direct_pay_by_user");
            SetGatewayParameterValue("partner", Merchant.Partner);
            SetGatewayParameterValue("notify_url", Merchant.NotifyUrl.ToString());
            SetGatewayParameterValue("return_url", Merchant.ReturnUrl.ToString());
            SetGatewayParameterValue("sign_type", sign_type);
            SetGatewayParameterValue("subject", Order.Subject);
            SetGatewayParameterValue("out_trade_no", Order.OrderNo);
            SetGatewayParameterValue("total_fee", Order.OrderAmount.ToString());
            SetGatewayParameterValue("payment_type", "1");
            SetGatewayParameterValue("_input_charset", Charset);
            SetGatewayParameterValue("sign", GetOrderSign());    // ǩ����Ҫ��������ã�����ȱ�ٲ�����
        }


        private string GetPaymentQueryString()
        {
            StringBuilder urlBuilder = new StringBuilder();
            foreach (KeyValuePair<string, string> item in GetSortedGatewayParameter())
            {
                urlBuilder.AppendFormat("{0}={1}&", item.Key, item.Value);
            }

            return urlBuilder.ToString().TrimEnd('&');
        }


        /// <summary>
        /// �������ǩ���Ĳ����ַ���
        /// </summary>
        private string GetSignParameter()
        {
            StringBuilder signBuilder = new StringBuilder();
            foreach (KeyValuePair<string, string> item in GetSortedGatewayParameter())
            {
                if (string.Compare("sign", item.Key) != 0 && string.Compare("sign_type", item.Key) != 0)
                {
                    signBuilder.AppendFormat("{0}={1}&", item.Key, item.Value);
                }
            }

            return signBuilder.ToString().TrimEnd('&');
        }

        /// <summary>
        /// ��֤֧�������Ĳ�������
        /// </summary>
        private void ValidatePaymentOrderParameter()
        {
            if (string.IsNullOrEmpty(GetGatewayParameterValue("seller_email")))
            {
                throw new ArgumentNullException("seller_email", "����ȱ��seller_email������seller_email������֧�����˺ŵ����䡣" +
                                                "����Ҫʹ��PaymentSetting<T>.SetGatewayParameterValue(\"seller_email\", \"youname@email.com\")������������֧�����˺ŵ����䡣");
            }

            if (!IsEmail(GetGatewayParameterValue("seller_email")))
            {
                throw new ArgumentException("Email��ʽ����ȷ", "seller_email");
            }
        }

   
        private bool ValidateTrade()
        {
            var orderAmount = GetGatewayParameterValue("total_amount");
            orderAmount = string.IsNullOrEmpty(orderAmount) ? GetGatewayParameterValue("total_fee") : orderAmount;
            Order.OrderAmount = double.Parse(orderAmount);
            Order.OrderNo = GetGatewayParameterValue("out_trade_no");
            Order.TradeNo = GetGatewayParameterValue("trade_no");
            // ֧��״̬�Ƿ�Ϊ�ɹ���TRADE_FINISHED����ͨ��ʱ���˵Ľ��׳ɹ�״̬��TRADE_SUCCESS����ͨ�˸߼���ʱ���˻��Ʊ������Ʒ��Ľ��׳ɹ�״̬��
            if (string.Compare(GetGatewayParameterValue("trade_status"), "TRADE_FINISHED") == 0 ||
                string.Compare(GetGatewayParameterValue("trade_status"), "TRADE_SUCCESS") == 0)
            {              
                return true;
            }
            return false;
        }

        /// <summary>
        /// ��֤֧����֪ͨ��ǩ��
        /// </summary>
        private bool ValidateAlipayNotifyRSASign()
        {
            bool checkSign = AlipaySignature.RSACheckV1(GetSortedGatewayParameter(), Merchant.PublicKeyPem, Charset);
            if (checkSign)
            {
                return true;
            }
            return false;
        }


        /// <summary>
        /// ��֤֧����֪ͨ��ǩ��
        /// </summary>
        private bool ValidateAlipayNotifySign()
        {
            // ��֤֪ͨ��ǩ��
            if (string.Compare(GetGatewayParameterValue("sign"), GetOrderSign()) == 0)
            {
                return true;
            }
            return false;
        }


        /// <summary>
        /// �����ز����ļ�������
        /// </summary>
        /// <param name="coll">ԭ���ز����ļ���</param>
        private SortedList<string, string> GatewayParameterDataSort(ICollection<GatewayParameter> coll)
        {
            SortedList<string, string> list = new SortedList<string, string>();
            foreach (GatewayParameter item in coll)
            {
                list.Add(item.Name, item.Value);
            }

            return list;
        }

        /// <summary>
        /// ��ö�����ǩ����
        /// </summary>
        private string GetOrderRSASign()
        {
            return AlipaySignature.RSASign(GetSignParameter(), Merchant.PrivateKeyPem, Charset, "RSA");
        }

        /// <summary>
        /// ��ö�����ǩ����
        /// </summary>
        private string GetOrderSign()
        {
            // ���MD5ֵʱ��Ҫʹ��GB2312���룬����������������ʱ����ʾǩ���쳣������MD5ֵ����ΪСд��
            return Utility.GetMD5(GetSignParameter() + Merchant.Key, pageEncoding).ToLower();
        }


        /// <summary>
        /// ��֤���ص�֪ͨId�Ƿ���Ч
        /// </summary>
        private bool ValidateAlipayNotify()
        {
            // ������Զ����ص�֪ͨId������֤��1����ʧЧ��
            // �������첽֪ͨ��֪ͨId����������־�ɹ����յ�֪ͨ��success�ַ�����ʧЧ��
            if (string.Compare(Utility.ReadPage(GetValidateAlipayNotifyUrl()), "true") == 0)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// �����֤֧����֪ͨ��Url
        /// </summary>
        private string GetValidateAlipayNotifyUrl()
        {
            return string.Format("{0}?service=notify_verify&partner={1}&notify_id={2}", payGatewayUrl, Merchant.Partner,
                                 GetGatewayParameterValue("notify_id"));
        }


        /// <summary>
        /// �Ƿ�����ȷ��ʽ��Email��ַ
        /// </summary>
        /// <param name="emailAddress">Email��ַ</param>
        public bool IsEmail(string emailAddress)
        {
            if (string.IsNullOrEmpty(emailAddress))
            {
                return false;
            }

            return Regex.IsMatch(emailAddress, emailRegexString);
        }

        #endregion
    }
}