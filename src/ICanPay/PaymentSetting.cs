using ICanPay.Enums;
using ICanPay.Interfaces;
using ICanPay.Providers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Web;
using ThoughtWorks.QRCode.Codec;

namespace ICanPay
{
    /// <summary>
    /// ������Ҫ֧���Ķ��������ݣ�����֧������URL��ַ��HTML��
    /// </summary>
    /// <remarks>
    ///����֧��GB2312���롣 ͨ���� Web.config �е� configuration/system.web �ڵ����� <globalization requestEncoding="gb2312" responseEncoding="gb2312" />
    /// </remarks>
    public class PaymentSetting
    {

        #region �ֶ�

        GatewayBase gateway;

        #endregion


        #region ���캯��

        public PaymentSetting(GatewayType gatewayType)
        {
            gateway = CreateGateway(gatewayType);
        }


        public PaymentSetting(GatewayType gatewayType, Merchant merchant, Order order)
            : this(gatewayType)
        {
            gateway.Merchant = merchant;
            gateway.Order = order;
        }

        #endregion


        #region ����

        /// <summary>
        /// ����
        /// </summary>
        public GatewayBase Gateway
        {
            get
            {
                return gateway;
            }
        }


        /// <summary>
        /// �̼�����
        /// </summary>
        public Merchant Merchant
        {
            get
            {
                return gateway.Merchant;
            }

            set
            {
                gateway.Merchant = value;
            }
        }


        /// <summary>
        /// ��������
        /// </summary>
        public Order Order
        {
            get
            {
                return gateway.Order;
            }

            set
            {
                gateway.Order = value;
            }
        }


        public bool CanQueryNotify
        {
            get
            {
                if (gateway is IQueryUrl || gateway is IQueryForm)
                {
                    return true;
                }

                return false;
            }
        }


        public bool CanQueryNow
        {
            get
            {
                return gateway is IQueryNow;
            }
        }

        public bool CanBuildAppParams
        {
            get
            {
                return gateway is IAppParams;
            }
        }


        #endregion


        #region ����


        private GatewayBase CreateGateway(GatewayType gatewayType)
        {
            switch (gatewayType)
            {
                case GatewayType.Alipay:
                    {
                        return new AlipayGateway();
                    }
                case GatewayType.WeChatPayment:
                    {
                        return new WeChatPaymentGataway();
                    }
                case GatewayType.Tenpay:
                    {
                        return new TenpayGateway();
                    }
                case GatewayType.UnionPay:
                    {
                        return new UnionPayGateway();
                    }
                default:
                    {
                        return new NullGateway();
                    }
            }
        }


        /// <summary>
        /// ����������֧��Url��Form������ά�롣
        /// </summary>
        /// <remarks>
        /// ����������Ƕ�����Url��Form������ת����Ӧ����֧��������Ƕ�ά�뽫�����ά��ͼƬ��
        /// </remarks>
        public void Payment()
        {
            HttpContext.Current.Response.ContentEncoding = Encoding.GetEncoding(Gateway.Charset);
            IPaymentUrl paymentUrl = gateway as IPaymentUrl;
            if (paymentUrl != null)
            {          
                HttpContext.Current.Response.Redirect(paymentUrl.BuildPaymentUrl());
                return;
            }

            IPaymentForm paymentForm = gateway as IPaymentForm;
            if (paymentForm != null)
            {
                HttpContext.Current.Response.Write(paymentForm.BuildPaymentForm());
                return;
            }

            IPaymentQRCode paymentQRCode = gateway as IPaymentQRCode;
            if (paymentQRCode != null)
            {
                BuildQRCodeImage(paymentQRCode.GetPaymentQRCodeContent());
                return;
            }

            throw new NotSupportedException(gateway.GatewayType + " û��ʵ��֧���ӿ�");
        }

        /// <summary>
        /// ����WAP֧��
        /// </summary>
        /// <param name="map"></param>
        public void WapPayment(Dictionary<string, string> map =null)
        {
            HttpContext.Current.Response.ContentEncoding = Encoding.GetEncoding(Gateway.Charset);
            IWapPaymentUrl paymentUrl = gateway as IWapPaymentUrl;
            if (paymentUrl != null)
            {
                if (gateway.GatewayType == GatewayType.WeChatPayment)
                {
                    HttpContext.Current.Response.Write($"<script language='javascript'>window.location='{paymentUrl.BuildWapPaymentUrl(map)}'</script>");
                }
                else
                {
                    HttpContext.Current.Response.Redirect(paymentUrl.BuildWapPaymentUrl(map));
                }
                return;
            }

            IWapPaymentForm paymentForm = gateway as IWapPaymentForm;
            if (paymentForm != null)
            {
                HttpContext.Current.Response.Write(paymentForm.BuildWapPaymentForm());
                return;
            }

            throw new NotSupportedException(gateway.GatewayType + " û��ʵ��֧���ӿ�");
        }

        /// <summary>
        /// ��ѯ�����������Ĳ�ѯ֪ͨ����ͨ����֧��֪ͨһ������ʽ���ء��ô�������֪ͨһ���ķ������ܲ�ѯ���������ݡ�
        /// </summary>
        public void QueryNotify()
        {
            IQueryUrl queryUrl = gateway as IQueryUrl;
            if (queryUrl != null)
            {
                HttpContext.Current.Response.Redirect(queryUrl.BuildQueryUrl());
                return;
            }

            IQueryForm queryForm = gateway as IQueryForm;
            if (queryForm != null)
            {
                HttpContext.Current.Response.Write(queryForm.BuildQueryForm());
                return;
            }

            throw new NotSupportedException(gateway.GatewayType + " û��ʵ�� IQueryUrl �� IQueryForm ��ѯ�ӿ�");
        }

        /// <summary>
        /// ��ѯ������������ö����Ĳ�ѯ���
        /// </summary>
        /// <returns></returns>
        public bool QueryNow(ProductSet productSet = ProductSet.APP)
        {
            IQueryNow queryNow = gateway as IQueryNow;
            if (queryNow != null)
            {
                return queryNow.QueryNow(productSet);
            }

            throw new NotSupportedException(gateway.GatewayType + " û��ʵ�� IQueryNow ��ѯ�ӿ�");
        }

        /// <summary>
        /// ����APP��SDK֧����Ҫ�Ĳ���
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> BuildPayParams()
        {
            IAppParams appParams = gateway as IAppParams;
            if (appParams != null)
            {
                return appParams.BuildPayParams();
            }

            throw new NotSupportedException(gateway.GatewayType + " û��ʵ�� IAppParams ��ѯ�ӿ�");
        }

        /// <summary>
        /// �������ص�����
        /// </summary>
        /// <param name="gatewayParameterName">���صĲ�������</param>
        /// <param name="gatewayParameterValue">���صĲ���ֵ</param>
        public void SetGatewayParameterValue(string gatewayParameterName, string gatewayParameterValue)
        {
            Gateway.SetGatewayParameterValue(gatewayParameterName, gatewayParameterValue);
        }
    
        /// <summary>
        /// ���ɲ������ά��ͼƬ
        /// </summary>
        /// <param name="qrCodeContent">��ά������</param>
        private void BuildQRCodeImage(string qrCodeContent)
        {
            QRCodeEncoder qrCodeEncoder = new QRCodeEncoder();
            qrCodeEncoder.QRCodeScale = 4;  // ��ά���С
            Bitmap image = qrCodeEncoder.Encode(qrCodeContent, Encoding.Default);
            MemoryStream ms = new MemoryStream();
            image.Save(ms, ImageFormat.Png);
            HttpContext.Current.Response.ContentType = "image/x-png";
            HttpContext.Current.Response.BinaryWrite(ms.GetBuffer());
        }
        #endregion
    }
}
