using com.unionpay.acp.sdk;
using ICanPay.Enums;
using ICanPay.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Web;

namespace ICanPay.Providers
{
    /// <summary>
    /// 银联网关
    /// </summary>
    public class UnionPayGateway : GatewayBase, IPaymentForm, IWapPaymentForm, IAppParams, IQueryNow, IRefundReq
    {
        #region 构造函数

        /// <summary>
        /// 初始化中国银联网关
        /// </summary>
        public UnionPayGateway()
        {
        }


        /// <summary>
        /// 初始化中国银联网关
        /// </summary>
        /// <param name="gatewayParameterData">网关通知的数据集合</param>
        public UnionPayGateway(List<GatewayParameter> gatewayParameterData)
            : base(gatewayParameterData)
        {
        }

        #endregion

        #region 属性
        public override GatewayType GatewayType
        {
            get
            {
                return GatewayType.UnionPay;
            }
        }

        #endregion

        #region 方法
        public string BuildPaymentForm()
        {
            Dictionary<string, string> param = new Dictionary<string, string>();

            //以下信息非特殊情况不需要改动
            param["version"] = "5.0.0";//版本号
            param["encoding"] = "UTF-8";//编码方式
            param["txnType"] = "01";//交易类型
            param["txnSubType"] = "01";//交易子类
            param["bizType"] = "000201";//业务类型
            param["signMethod"] = "01";//签名方法
            param["channelType"] = "08";//渠道类型
            param["accessType"] = "0";//接入类型
            param["frontUrl"] = Merchant.NotifyUrl.ToString();  //前台通知地址      
            param["backUrl"] = Merchant.ReturnUrl.ToString();  //后台通知地址
            param["currencyCode"] = "156";//交易币种

            //TODO 以下信息需要填写
            param["merId"] = Merchant.Partner;//商户号，请改自己的测试商户号，此处默认取demo演示页面传递的参数
            param["orderId"] = Order.OrderNo;//商户订单号，8-32位数字字母，不能含“-”或“_”，此处默认取demo演示页面传递的参数，可以自行定制规则
            param["txnTime"] = Order.PaymentDate.ToString("yyyyMMddHHmmss");//订单发送时间，格式为YYYYMMDDhhmmss，取北京时间，此处默认取demo演示页面传递的参数，参考取法： DateTime.Now.ToString("yyyyMMddHHmmss")
            param["txnAmt"] = (Order.OrderAmount * 100).ToString();//交易金额，单位分，此处默认取demo演示页面传递的参数
            //param["reqReserved"] = "透传信息";//请求方保留域，透传字段，查询、通知、对账文件中均会原样出现，如有需要请启用并修改自己希望透传的数据

            //TODO 其他特殊用法请查看 pages/api_01_gateway/special_use_purchase.htm

            AcpService.Sign(param, System.Text.Encoding.UTF8);
            string html = AcpService.CreateAutoFormHtml(SDKConfig.FrontTransUrl, param, System.Text.Encoding.UTF8);// 将SDKUtil产生的Html文档写入页面，从而引导用户浏览器重定向 
            System.Web.HttpContext.Current.Response.ContentEncoding = Encoding.UTF8; // 指定输出编码  
            return html;
        }

        public string BuildWapPaymentForm()
        {
            return BuildPaymentForm();
        }

        public Dictionary<string, string> BuildPayParams()
        {
            //组装请求报文
            Dictionary<string, string> param = new Dictionary<string, string>();
            // 版本号
            param.Add("version", "5.0.0");
            // 字符集编码 默认"UTF-8"
            param.Add("encoding", "UTF-8");
            // 签名方法 01 RSA
            param.Add("signMethod", "01");
            // 交易类型 01-消费
            param.Add("txnType", "01");
            // 交易子类型 01:自助消费 02:订购 03:分期付款
            param.Add("txnSubType", "01");
            // 业务类型
            param.Add("bizType", "000201");
            // 渠道类型，07-PC，08-手机
            param.Add("channelType", "08");
            // 前台通知地址 ，控件接入方式无作用
            param.Add("frontUrl", Merchant.ReturnUrl.ToString());
            // 后台通知地址
            param.Add("backUrl", Merchant.NotifyUrl.ToString());
            // 接入类型，商户接入填0 0- 商户 ， 1： 收单， 2：平台商户
            param.Add("accessType", "0");
            // 商户号码，请改成自己的商户号
            param.Add("merId", Merchant.Partner);
            // 商户订单号，8-40位数字字母
            param.Add("orderId", Order.OrderNo);
            // 订单发送时间，取系统时间
            param.Add("txnTime", Order.PaymentDate.ToString("yyyyMMddHHmmss"));
            // 交易金额，单位分
            param.Add("txnAmt", (Order.OrderAmount * 100).ToString());
            // 交易币种
            param.Add("currencyCode", "156");
            // 请求方保留域，透传字段，查询、通知、对账文件中均会原样出现
            // param.Add("reqReserved", "透传信息");
            // 订单描述，可不上送，上送时控件中会显示该信息
            // param.Add("orderDesc", "订单描述");

            AcpService.Sign(param, Encoding.UTF8);

            Dictionary<String, String> resmap = AcpService.Post(param, SDKConfig.AppRequestUrl, Encoding.UTF8);
            Dictionary<string, string> resParam = new Dictionary<string, string>();
            resParam.Add("tn", resmap["tn"]);
            return resParam;
        }

        public Refund BuildRefund(Refund refund)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();

            //以下信息非特殊情况不需要改动
            param["version"] = "5.0.0";//版本号
            param["encoding"] = "UTF-8";//编码方式
            param["signMethod"] = "01";//签名方法
            param["txnType"] = "04";//交易类型
            param["txnSubType"] = "00";//交易子类
            param["bizType"] = "000201";//业务类型
            param["accessType"] = "0";//接入类型
            param["channelType"] = "07";//渠道类型
            param["backUrl"] = "";  //后台通知地址

            //TODO 以下信息需要填写
            param["orderId"] = refund.RefoundNo;//商户订单号，8-32位数字字母，不能含“-”或“_”，可以自行定制规则，重新产生，不同于原消费，此处默认取demo演示页面传递的参数
            param["merId"] =Merchant.Partner;//商户代码，请改成自己的测试商户号，此处默认取demo演示页面传递的参数
            param["origQryId"] = refund.TradeNo;//原消费的queryId，可以从查询接口或者通知接口中获取，此处默认取demo演示页面传递的参数
            param["txnTime"] = refund.PaymentDate.ToString("yyyyMMddHHmmss");//订单发送时间，格式为YYYYMMDDhhmmss，重新产生，不同于原消费，此处默认取demo演示页面传递的参数，参考取法： DateTime.Now.ToString("yyyyMMddHHmmss")
            param["txnAmt"] = (refund.RefundAmount * 100).ToString(); //交易金额，退货总金额需要小于等于原消费

            // 请求方保留域，
            // 透传字段，查询、通知、对账文件中均会原样出现，如有需要请启用并修改自己希望透传的数据。
            // 出现部分特殊字符时可能影响解析，请按下面建议的方式填写：
            // 1. 如果能确定内容不会出现&={}[]"'等符号时，可以直接填写数据，建议的方法如下。
            //param["reqReserved"] = "透传信息1|透传信息2|透传信息3";
            // 2. 内容可能出现&={}[]"'符号时：
            // 1) 如果需要对账文件里能显示，可将字符替换成全角＆＝｛｝【】“‘字符（自己写代码，此处不演示）；
            // 2) 如果对账文件没有显示要求，可做一下base64（如下）。
            //    注意控制数据长度，实际传输的数据长度不能超过1024位。
            //    查询、通知等接口解析时使用System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(reqReserved))解base64后再对数据做后续解析。
            //param["reqReserved"] = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("任意格式的信息都可以"));

            AcpService.Sign(param, System.Text.Encoding.UTF8);  // 签名
            string url = SDKConfig.BackTransUrl;

            Dictionary<String, String> rspData = AcpService.Post(param, url, System.Text.Encoding.UTF8);

            // HttpClient hc = new HttpClient(url);
            // int status = hc.Send(param, System.Text.Encoding.UTF8);
            // string result = hc.Result;

            if (rspData.Count != 0)
            {
                if (AcpService.Validate(rspData, System.Text.Encoding.UTF8))
                {
                    string respcode = rspData["respCode"]; //其他应答参数也可用此方法获取
                    if ("00" == respcode)
                    {
                        //交易已受理，等待接收后台通知更新订单状态，如果通知长时间未收到也可发起交易状态查询
                        //TODO
                        refund.TradeNo = rspData["origQryId"];
                        refund.RefoundId = rspData["queryId"];
                        refund.Status = true;
                    }
                    else if ("03" == respcode ||
                            "04" == respcode ||
                            "05" == respcode)
                    {
                        //后续需发起交易状态查询交易确定交易状态
                        //TODO
                    }
                    else
                    {
                        //其他应答码做以失败处理
                        //TODO
                    }
                }
                else
                {
                    //商户端验证返回报文签名失败
                    //TODO
                }
            }
            else
            {
                //请求失败
                //TODO
            }
            return refund;
        }

        public Refund BuildRefundQuery(Refund refund)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();

            //以下信息非特殊情况不需要改动
            param["version"] = "5.0.0";//版本号
            param["encoding"] = "UTF-8";//编码方式
            param["signMethod"] = "01";//签名方法
            param["txnType"] = "00";//交易类型
            param["txnSubType"] = "00";//交易子类
            param["bizType"] = "000000";//业务类型
            param["accessType"] = "0";//接入类型
            param["channelType"] = "07";//渠道类型

            //TODO 以下信息需要填写
            param["orderId"] = refund.RefoundNo;	//请修改被查询的交易的订单号，8-32位数字字母，不能含“-”或“_”，此处默认取demo演示页面传递的参数
            param["merId"] = Merchant.Partner;//商户代码，请改成自己的测试商户号，此处默认取demo演示页面传递的参数
            param["txnTime"] = refund.PaymentDate.ToString("yyyyMMddHHmmss"); ;//请修改被查询的交易的订单发送时间，格式为YYYYMMDDhhmmss，此处默认取demo演示页面传递的参数

            AcpService.Sign(param, System.Text.Encoding.UTF8);  // 签名
            string url = SDKConfig.SingleQueryUrl;

            Dictionary<String, String> rspData = AcpService.Post(param, url, System.Text.Encoding.UTF8);

            if (rspData.Count != 0)
            {

                if (AcpService.Validate(rspData, System.Text.Encoding.UTF8))
                {
                    string respcode = rspData["respCode"]; //其他应答参数也可用此方法获取
                    if ("00" == respcode)
                    {
                        string origRespCode = rspData["origRespCode"]; //其他应答参数也可用此方法获取
                        //处理被查询交易的应答码逻辑
                        if ("00" == origRespCode)
                        {
                            //交易成功，更新商户订单状态
                            //TODO
                            refund.RefoundId = rspData["queryId"]; 
                            refund.Status = true;
                        }
                        else if ("03" == origRespCode ||
                            "04" == origRespCode ||
                            "05" == origRespCode)
                        {
                            //需再次发起交易状态查询交易
                            //TODO
                        }
                        else
                        {
                            //其他应答码做以失败处理 ("交易失败：" + rspData["origRespMsg"] 
                            //TODO
                        }
                    }
                    else if ("03" == respcode ||
                            "04" == respcode ||
                            "05" == respcode)
                    {
                        //不明原因超时，后续继续发起交易查询。
                        //TODO
                    }
                    else
                    {
                        //其他应答码做以失败处理 查询操作失败：" + rspData["respMsg"]
                        //TODO
                    }
                }
            }
            else
            {
                //请求失败;
                //TODO
            }
            return refund;
        }


        public bool QueryNow(ProductSet productSet)
        {           
            Dictionary<string, string> param = new Dictionary<string, string>();

            //以下信息非特殊情况不需要改动
            param["version"] = "5.0.0";//版本号
            param["encoding"] = "UTF-8";//编码方式
            param["signMethod"] = "01";//签名方法
            param["txnType"] = "00";//交易类型
            param["txnSubType"] = "00";//交易子类
            param["bizType"] = "000000";//业务类型
            param["accessType"] = "0";//接入类型
            param["channelType"] = "07";//渠道类型

            //TODO 以下信息需要填写
            param["orderId"] = Order.OrderNo;	//请修改被查询的交易的订单号，8-32位数字字母，不能含“-”或“_”，此处默认取demo演示页面传递的参数
            param["merId"] = Merchant.Partner;//商户代码，请改成自己的测试商户号，此处默认取demo演示页面传递的参数
            param["txnTime"] = Order.PaymentDate.ToString("yyyyMMddHHmmss");;//请修改被查询的交易的订单发送时间，格式为YYYYMMDDhhmmss，此处默认取demo演示页面传递的参数

            AcpService.Sign(param, System.Text.Encoding.UTF8);  // 签名
            string url = SDKConfig.SingleQueryUrl;

            Dictionary<String, String> rspData = AcpService.Post(param, url, System.Text.Encoding.UTF8);
        
            if (rspData.Count != 0)
            {

                if (AcpService.Validate(rspData, System.Text.Encoding.UTF8))
                {
                    string respcode = rspData["respCode"]; //其他应答参数也可用此方法获取
                    if ("00" == respcode)
                    {
                        string origRespCode = rspData["origRespCode"]; //其他应答参数也可用此方法获取
                        //处理被查询交易的应答码逻辑
                        if ("00" == origRespCode)
                        {
                            //交易成功，更新商户订单状态
                            //TODO
                            //Response.Write("交易成功。<br>\n");
                            return true;
                        }
                        else if ("03" == origRespCode ||
                            "04" == origRespCode ||
                            "05" == origRespCode)
                        {
                            //需再次发起交易状态查询交易
                            //TODO
                            //Response.Write("稍后查询。<br>\n");
                            return false;
                        }
                        else
                        {
                            //其他应答码做以失败处理
                            //TODO
                           // Response.Write("交易失败：" + rspData["origRespMsg"] + "。<br>\n");
                            return false;
                        }
                    }
                    else if ("03" == respcode ||
                            "04" == respcode ||
                            "05" == respcode)
                    {
                        //不明原因超时，后续继续发起交易查询。
                        //TODO
                        //Response.Write("处理超时，请稍后查询。<br>\n");
                        return false;
                    }
                    else
                    {
                        //其他应答码做以失败处理
                        //TODO
                        //Response.Write("查询操作失败：" + rspData["respMsg"] + "。<br>\n");
                        return false;
                    }
                }
            }
            else
            {
                //Response.Write("请求失败\n");
                return false;
            }
            return false;
        }

        public override void WriteSucceedFlag()
        {
            if (PaymentNotifyMethod == PaymentNotifyMethod.ServerNotify)
            {
                HttpContext.Current.Response.Write("ok");
            }
        }

        protected override bool CheckNotifyData()
        {
            if (HttpContext.Current.Request.HttpMethod == "POST")
            {
                // 使用Dictionary保存参数
                Dictionary<string, string> resData = new Dictionary<string, string>();

                NameValueCollection coll = HttpContext.Current.Request.Form;

                string[] requestItem = coll.AllKeys;

                for (int i = 0; i < requestItem.Length; i++)
                {
                    resData.Add(requestItem[i], HttpContext.Current.Request.Form[requestItem[i]]);
                }

                // 返回报文中不包含UPOG,表示Server端正确接收交易请求,则需要验证Server端返回报文的签名
                if (AcpService.Validate(resData, Encoding.UTF8))
                {
                    Order.OrderNo = resData.ContainsKey("orderId") ? resData["orderId"] : "";
                    Order.OrderAmount = resData.ContainsKey("txnAmt") ? Convert.ToDouble(GetGatewayParameterValue("txnAmt")) * 0.01 : 0.0;
                    Order.TradeNo = resData.ContainsKey("queryId") ? resData["queryId"] : "";
                    if (resData["respMsg"].ToLower().Contains("success"))
                    {                     
                        return true;
                    }
                }
            }
            return false;
        }

        #endregion
    }
}
