using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Xml;


namespace ICanPay.Providers
{
    /// <summary>
    /// ΢��֧������
    /// </summary>
    /// <remarks>
    /// ʹ��ģʽ��ʵ��΢��֧��
    /// </remarks>
    public sealed class WeChatPaymentGataway : GatewayBase, IPaymentQRCode, IQueryNow, IWapPaymentUrl, IAppParams
    {

        #region ˽���ֶ�

        const string payGatewayUrl = "https://api.mch.weixin.qq.com/pay/unifiedorder";
        const string queryGatewayUrl = "https://api.mch.weixin.qq.com/pay/orderquery";

        #endregion


        #region ���캯��

        /// <summary>
        /// ��ʼ��΢��֧������
        /// </summary>
        public WeChatPaymentGataway()
        {
        }


        /// <summary>
        /// ��ʼ��΢��֧������
        /// </summary>
        /// <param name="gatewayParameterData">����֪ͨ�����ݼ���</param>
        public WeChatPaymentGataway(List<GatewayParameter> gatewayParameterData)
            : base(gatewayParameterData)
        {
        }

        #endregion


        public override GatewayType GatewayType
        {
            get { return GatewayType.WeChatPayment; }
        }

        protected override bool CheckNotifyData()
        {
            ReadNotifyOrderParameter();
            if (IsSuccessResult())
            {
                return true;
            }

            return false;
        }

        public string GetPaymentQRCodeContent()
        {
            InitPaymentOrderParameter();
            return GetWeixinPaymentUrl(PostOrder(ConvertGatewayParameterDataToXml(), payGatewayUrl));
        }

        public bool QueryNow(ProductSet productSet)
        {
            InitQueryOrderParameter();
            return CheckQueryResult(PostOrder(ConvertGatewayParameterDataToXml(), queryGatewayUrl));
        }


        /// <summary>
        /// ��ʼ��֧�������Ĳ���
        /// </summary>
        private void InitPaymentOrderParameter(string trade_type = "NATIVE", string spbill_create_ip = "127.0.0.1")
        {
            SetGatewayParameterValue("appid", Merchant.AppId);
            SetGatewayParameterValue("mch_id", Merchant.Partner);
            SetGatewayParameterValue("nonce_str", GenerateNonceString());
            SetGatewayParameterValue("body", Order.Subject);
            SetGatewayParameterValue("out_trade_no", Order.OrderNo);
            SetGatewayParameterValue("total_fee", (Order.OrderAmount * 100).ToString());
            SetGatewayParameterValue("spbill_create_ip", spbill_create_ip);
            SetGatewayParameterValue("notify_url", Merchant.NotifyUrl.ToString());
            SetGatewayParameterValue("trade_type", trade_type);
            SetGatewayParameterValue("sign", GetSign());    // ǩ����Ҫ��������ã�����ȱ�ٲ�����
        }


        private void ReadNotifyOrderParameter()
        {
            Order.OrderNo = GetGatewayParameterValue("out_trade_no");
            Order.OrderAmount = Convert.ToDouble(GetGatewayParameterValue("total_fee")) * 0.01;
            Order.TradeNo = GetGatewayParameterValue("transaction_id");
        }


        /// <summary>
        /// ��������ַ���
        /// </summary>
        /// <returns></returns>
        private string GenerateNonceString()
        {
            return Guid.NewGuid().ToString().Replace("-", "");
        }


        /// <summary>
        /// ����������ת����XML
        /// </summary>
        /// <returns></returns>
        private string ConvertGatewayParameterDataToXml()
        {
            StringBuilder xmlBuilder = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.OmitXmlDeclaration = true;

            using (XmlWriter writer = XmlWriter.Create(xmlBuilder, settings))
            {
                writer.WriteStartElement("xml");
                foreach (var item in GetSortedGatewayParameter())
                {
                    writer.WriteElementString(item.Key, item.Value);
                }
                writer.WriteEndElement();
                writer.Flush();
            }

            return xmlBuilder.ToString();
        }


        /// <summary>
        /// ���ǩ��
        /// </summary>
        /// <returns></returns>
        private string GetSign()
        {
            StringBuilder signBuilder = new StringBuilder();
            foreach (var item in GetSortedGatewayParameter())
            {
                // ��ֵ�Ĳ�����sign����������ǩ��
                if (!string.IsNullOrEmpty(item.Value) && string.Compare("sign", item.Key) != 0)
                {
                    signBuilder.AppendFormat("{0}={1}&", item.Key, item.Value);
                }
            }

            signBuilder.Append("key=" + Merchant.Key);
            return Utility.GetMD5(signBuilder.ToString()).ToUpper();
        }


        /// <summary>
        /// �ύ����
        /// </summary>
        /// <param name="orderXml">������XML����</param>
        /// <param name="gatewayUrl">����URL</param>
        /// <returns></returns>
        private string PostOrder(string orderXml, string gatewayUrl)
        {
            byte[] dataByte = Encoding.UTF8.GetBytes(orderXml);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(gatewayUrl);
            request.Method = "POST";
            request.ContentType = "text/xml";
            request.ContentLength = dataByte.Length;

            try
            {
                using (Stream outStream = request.GetRequestStream())
                {
                    outStream.Write(dataByte, 0, dataByte.Length);
                }

                using (WebResponse response = request.GetResponse())
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        if (reader != null)
                        {
                            return reader.ReadToEnd();
                        }
                    }
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                request.Abort();
            }

            return string.Empty;
        }


        /// <summary>
        /// ���΢��֧����URL
        /// </summary>
        /// <param name="resultXml">�����������ص�����</param>
        /// <returns></returns>
        private string GetWeixinPaymentUrl(string resultXml)
        {
            // ��Ҫ�����֮ǰ���������Ĳ����������Խ��յ��Ĳ�����ɸ��š�
            ClearGatewayParameterData();
            ReadResultXml(resultXml);
            if (IsSuccessResult())
            {
                return string.IsNullOrEmpty(GetGatewayParameterValue("code_url")) ? GetGatewayParameterValue("mweb_url") : GetGatewayParameterValue("code_url");
            }

            return string.Empty;
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
            SortedDictionary<string, string> parma = new SortedDictionary<string, string>();
            if (xmlDocument.FirstChild != null && xmlDocument.FirstChild.ChildNodes != null)
            {
                foreach (XmlNode item in xmlDocument.FirstChild.ChildNodes)
                {
                    SetGatewayParameterValue(item.Name, item.InnerText);
                }
            }
        }


        /// <summary>
        /// �Ƿ��ǳɹ��Ľ��
        /// </summary>
        /// <param name="parma"></param>
        /// <returns></returns>
        private bool IsSuccessResult()
        {
            if (ValidateResult() && ValidateSign())
            {
                return true;
            }

            return false;
        }


        /// <summary>
        /// ��֤���صĽ��
        /// </summary>
        /// <returns></returns>
        private bool ValidateResult()
        {
            if (string.Compare(GetGatewayParameterValue("return_code"), "SUCCESS") == 0 &&
                string.Compare(GetGatewayParameterValue("result_code"), "SUCCESS") == 0)
            {
                return true;
            }

            return false;

        }


        /// <summary>
        /// ��֤ǩ��
        /// </summary>
        /// <returns></returns>
        private bool ValidateSign()
        {
            if (string.Compare(GetGatewayParameterValue("sign"), GetSign()) == 0)
            {
                return true;
            }

            return false;
        }


        /// <summary>
        /// ����ѯ���
        /// </summary>
        /// <param name="resultXml">��ѯ�����XML</param>
        /// <returns></returns>
        private bool CheckQueryResult(string resultXml)
        {
            // ��Ҫ�����֮ǰ��ѯ�����Ĳ����������Խ��յ��Ĳ�����ɸ��š�
            ClearGatewayParameterData();
            ReadResultXml(resultXml);
            if (IsSuccessResult())
            {
                if (!string.IsNullOrEmpty(GetGatewayParameterValue("total_fee")))
                {
                    if (string.Compare(Order.OrderNo, GetGatewayParameterValue("out_trade_no")) == 0 &&
                         Order.OrderAmount == int.Parse(GetGatewayParameterValue("total_fee")) / 100.0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// ��ʼ����ѯ��������
        /// </summary>
        private void InitQueryOrderParameter()
        {
            SetGatewayParameterValue("appid", Merchant.AppId);
            SetGatewayParameterValue("mch_id", Merchant.Partner);
            SetGatewayParameterValue("out_trade_no", Order.OrderNo);
            SetGatewayParameterValue("nonce_str", GenerateNonceString());
            SetGatewayParameterValue("sign", GetSign());    // ǩ����Ҫ��������ã�����ȱ�ٲ�����
        }


        /// <summary>
        /// ������ص�����
        /// </summary>
        private void ClearGatewayParameterData()
        {
            GatewayParameterData.Clear();
        }


        /// <summary>
        /// ��ʼ����ʾ�ѳɹ����յ�֧��֪ͨ������
        /// </summary>
        private void InitProcessSuccessParameter()
        {
            SetGatewayParameterValue("return_code", "SUCCESS");
        }


        public override void WriteSucceedFlag()
        {
            // ��Ҫ�����֮ǰ���յ���֪ͨ�Ĳ��������������ɱ�־�ɹ����յ�֪ͨ��XML��ɸ��š�
            ClearGatewayParameterData();
            InitProcessSuccessParameter();
            HttpContext.Current.Response.Write(ConvertGatewayParameterDataToXml());
        }

        public Dictionary<string, object> BuildPayParams()
        {
            InitPaymentOrderParameter("APP");
            GetWeixinPaymentUrl(PostOrder(ConvertGatewayParameterDataToXml(), payGatewayUrl));

            var prepayid = GetGatewayParameterValue("prepay_id");
            ClearGatewayParameterData();

            SetGatewayParameterValue("appid", Merchant.AppId);
            SetGatewayParameterValue("partnerid", Merchant.Partner);
            SetGatewayParameterValue("prepayid", prepayid);
            SetGatewayParameterValue("package", "Sign=WXPay");
            SetGatewayParameterValue("noncestr", GenerateNonceString());
            SetGatewayParameterValue("timestamp", DateTime.Now.TimeStamp());
            SetGatewayParameterValue("sign", GetSign());

            Dictionary<string, object> resParam = new Dictionary<string, object>();
            resParam.Add("prepayid", prepayid);
            resParam.Add("noncestr", GetGatewayParameterValue("noncestr"));
            resParam.Add("sign", GetGatewayParameterValue("sign"));
            resParam.Add("timestamp", GetGatewayParameterValue("timestamp"));
            resParam.Add("partnerid", GetGatewayParameterValue("partnerid"));
            return resParam;
        }

        /// <summary>
        /// https://pay.weixin.qq.com/wiki/doc/api/H5.php?chapter=15_4
        /// </summary>
        /// <returns></returns>
        public string BuildWapPaymentUrl(string redirect_url = "")
        {
            InitPaymentOrderParameter("MWEB", Utility.GetClientIP());
            return string.Format("{0}&redirect_url={1}", GetWeixinPaymentUrl(PostOrder(ConvertGatewayParameterDataToXml(), payGatewayUrl)), HttpUtility.UrlEncode(string.IsNullOrEmpty(redirect_url) ? Merchant.ReturnUrl.ToString() : redirect_url, Encoding.UTF8));
        }     
    }
}
