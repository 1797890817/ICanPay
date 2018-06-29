using ICanPay.Enums;
using System;

namespace ICanPay
{
    /// <summary>
    /// 商户数据
    /// </summary>
    [Serializable]
    public class Merchant
    {
        #region 私有字段

        string partner;
        string key;
        string email;
        string appId;
        Uri notifyUrl;
        Uri returnUrl;

        #endregion

        #region 构造函数

        public Merchant()
        {
        }


        public Merchant(string userName, string key, Uri notifyUrl, GatewayType gatewayType)
        {
            this.partner = userName;
            this.key = key;
            this.notifyUrl = notifyUrl;
            GatewayType = gatewayType;
        }

        #endregion

        #region 属性

        /// <summary>
        /// 商户帐号
        /// </summary>
        public string Partner
        {
            get
            {
                if (string.IsNullOrEmpty(partner))
                {
                    throw new ArgumentNullException("Partner", "商户帐号没有设置");
                }
                return partner;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("Partner", "商户帐号不能为空");
                }
                partner = value;
            }
        }


        /// <summary>
        /// 商户密钥
        /// </summary>
        public string Key
        {
            get
            {
                if (string.IsNullOrEmpty(key))
                {
                    throw new ArgumentNullException("Key", "商户密钥没有设置");
                }
                return key;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("Key", "商户密钥不能为空");
                }
                key = value;
            }
        }

        /// <summary>
        /// 商户邮箱
        /// </summary>
        public string Email
        {
            get
            {
                return email;
            }
            set
            {
                email = value;
            }
        }

        /// <summary>
        ///  微信支付等需要
        /// </summary>
        public string AppId
        {
            get
            {
                return appId;
            }
            set
            {
                appId = value;
            }
        }

        /// <summary>
        /// 网关回发通知URL
        /// </summary>
        public Uri NotifyUrl
        {
            get
            {
                if (notifyUrl == null)
                {
                    throw new ArgumentNullException("NotifyUrl", "网关通知Url没有设置");
                }
                return notifyUrl;
            }
            set
            {
                notifyUrl = value ?? throw new ArgumentNullException("NotifyUrl", "网关通知Url不能为空");
            }
        }

        /// <summary>
        /// 网关主动跳转通知URL
        /// </summary>
        public Uri ReturnUrl
        {
            get
            {
                return returnUrl;
            }
            set
            {
                returnUrl = value;
            }
        }

        /// <summary>
        /// 私钥地址
        /// </summary>
        public string PrivateKey { get; set; }

        /// <summary>
        /// 公钥地址
        /// </summary>
        public string PublicKey { get; set; }

        /// <summary>
        /// 文件读取密钥
        /// </summary>
        public bool KeyFromFile { get; set; } = false;

        /// <summary>
        /// 网关类型
        /// </summary>
        public GatewayType GatewayType { get; set; }

        #endregion
    }
}