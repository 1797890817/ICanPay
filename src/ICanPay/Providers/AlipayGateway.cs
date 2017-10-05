using Aop.Api;
using Aop.Api.Request;
using Aop.Api.Response;
using Aop.Api.Util;
using ICanPay.Enums;
using ICanPay.Interfaces;
using ICanPay.Utils;
using Newtonsoft.Json;
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
    public sealed class AlipayGateway : GatewayBase, IPaymentForm, IPaymentUrl, IWapPaymentUrl, IAppParams, IQueryNow
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
            InitOrderParameter("MD5");
            ValidatePaymentOrderParameter();
            return GetFormHtml(payGatewayUrl);
        }


        public string BuildPaymentUrl()
        {
            InitOrderParameter("MD5");
            ValidatePaymentOrderParameter();
            return string.Format("{0}?{1}", payGatewayUrl, GetPaymentQueryString());
        }


        public string BuildWapPaymentUrl(Dictionary<string, string> map)
        {
            IAopClient defaultAopClient = new DefaultAopClient(openapiGatewayUrl, Merchant.AppId, Merchant.PrivateKeyPem, true);
            AlipayTradeWapPayRequest request = new AlipayTradeWapPayRequest();
            request.SetReturnUrl(Merchant.ReturnUrl.ToString());
            request.SetNotifyUrl(Merchant.NotifyUrl.ToString());
            request.BizContent = JsonConvert.SerializeObject(new
            {
                subject = Order.Subject,
                out_trade_no = Order.OrderNo,
                timeout_express = "90m",
                total_amount = Order.OrderAmount,
                product_code = "QUICK_WAP_WAY"
            });
            AlipayTradeWapPayResponse response = defaultAopClient.pageExecute(request, null, "GET");
            return response.Body;
        }

        public Dictionary<string, string> BuildPayParams()
        {
            SetGatewayParameterValue("app_id", Merchant.AppId);
            SetGatewayParameterValue("method", "alipay.trade.app.pay");
            SetGatewayParameterValue("charset", Charset);
            SetGatewayParameterValue("sign_type", "RSA");
            SetGatewayParameterValue("timestamp", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            SetGatewayParameterValue("version", "1.0");
            SetGatewayParameterValue("notify_url", Merchant.NotifyUrl);
            SetGatewayParameterValue("biz_content",
                JsonConvert.SerializeObject(new
                {
                    subject = Order.Subject,
                    out_trade_no = Order.OrderNo,
                    total_amount = Order.OrderAmount,
                    product_code = "QUICK_MSECURITY_PAY"
                }));

            SetGatewayParameterValue("sign", AopUtils.SignAopRequest(GetSortedGatewayParameter(), Merchant.PrivateKeyPem, Charset, true, "RSA"));

            StringBuilder signBuilder = new StringBuilder();
            foreach (KeyValuePair<string, string> item in GetSortedGatewayParameter())
            {
                signBuilder.AppendFormat("{0}={1}&", item.Key, HttpUtility.UrlEncode(item.Value, Encoding.UTF8));
            }
            Dictionary<string, string> resParam = new Dictionary<string, string>();
            resParam.Add("body", signBuilder.ToString().TrimEnd('&'));
            return resParam;
        }

        #region QueryNow
        public bool QueryNow(ProductSet productSet)
        {
            if (productSet == ProductSet.Web)
            {
                //InitQueryParameter();
                //ReadResultXml(Utility.ReadPage(string.Format("{0}?{1}", payGatewayUrl, GetPaymentQueryString()), pageEncoding));
                //if (ValidateNotifyParameter() && ValidateOrder())
                //{
                //    return true;
                //}
                return false;
            }
            else
            {
                IAopClient client = new DefaultAopClient(openapiGatewayUrl, Merchant.AppId, Merchant.PrivateKeyPem, true);
                AlipayTradeQueryRequest request = new AlipayTradeQueryRequest();
                request.BizContent = JsonConvert.SerializeObject(
                    new
                    {
                        out_trade_no = Order.OrderNo
                    });
                AlipayTradeQueryResponse response = client.Execute(request);
                if (((string.Compare(response.TradeStatus, "TRADE_FINISHED") == 0 || string.Compare(response.TradeStatus, "TRADE_SUCCESS") == 0)))
                {
                    var orderAmount = double.Parse(response.TotalAmount);
                    if (Order.OrderAmount == orderAmount && string.Compare(Order.OrderNo, response.OutTradeNo) == 0)
                    {
                        return true;
                    }
                    return false;
                }
                return false;
            }
        }

        /// <summary>
        /// ��ȡ�����XML
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        private void ReadResultXml(string xml)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(xml);
            var node = xmlDocument.ChildNodes[1].ChildNodes[0];
            SetGatewayParameterValue(node.Name, node.InnerText);
            foreach (XmlNode rootNode in xmlDocument.ChildNodes)
            {
                foreach (XmlNode item in rootNode.ChildNodes)
                {
                    SetGatewayParameterValue(item.Name, item.InnerText);
                }
            }
        }

        /// <summary>
        /// ��ʼ����ѯ��������
        /// </summary>
        private void InitQueryParameter(string sign_type)
        {
            SetGatewayParameterValue("service", "single_trade_query");
            SetGatewayParameterValue("partner", Merchant.Partner);
            SetGatewayParameterValue("_input_charset", Charset);
            SetGatewayParameterValue("sign_type", sign_type);
            SetGatewayParameterValue("out_trade_no", Order.OrderNo);
            if (!string.IsNullOrEmpty(Order.TradeNo))
            {
                SetGatewayParameterValue("trade_no", Order.TradeNo);
            }
            SetGatewayParameterValue("sign", GetOrderSign());    // ǩ����Ҫ��������ã�����ȱ�ٲ�����
        }


        /// <summary>
        /// ���֪ͨǩ���Ƿ���ȷ�����������Ƿ�ΪRMB���Ƿ�֧���ɹ���
        /// </summary>
        /// <returns></returns>
        private bool ValidateNotifyParameter()
        {
            //string.Compare(GetGatewayParameterValue("sign"), GetOrderSign()) == 0 
            if (string.Compare(GetGatewayParameterValue("is_success"), "T") == 0
               && ((string.Compare(GetGatewayParameterValue("trade_status"), "TRADE_FINISHED") == 0 ||
                    string.Compare(GetGatewayParameterValue("trade_status"), "TRADE_SUCCESS") == 0))
                )
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// ��֤�������������Ƿ���֮ǰ��֪ͨ�Ľ����������
        /// </summary>
        /// <returns></returns>
        private bool ValidateOrder()
        {
            if (Order.OrderAmount == Convert.ToDouble(GetGatewayParameterValue("total_fee")) &&
               string.Compare(Order.OrderNo, GetGatewayParameterValue("out_trade_no")) == 0)
            {
                return true;
            }

            return false;
        }


        #endregion

        protected override bool CheckNotifyData()
        {
            if (GetGatewayParameterValue("sign_type").ToUpper() != "RSA")
            {
                if (ValidateAlipayNotify() && ValidateAlipayNotifySign())
                {
                    return ValidateTrade();
                }
            }
            else
            {
                if (ValidateAlipayNotifyRSASign())
                {
                    return ValidateTrade();
                }
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