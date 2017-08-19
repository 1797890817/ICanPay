using System;
using System.Runtime.Serialization;

namespace ICanPay
{
    /// <summary>
    /// �̻�����
    /// </summary>
    [DataContract]
    [Serializable]
    public class Merchant
    {

        #region ˽���ֶ�

        string partner;
        string key;
        string email;
        string appId;
        Uri notifyUrl;
        Uri returnUrl;

        #endregion


        #region ���캯��

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


        #region ����

        /// <summary>
        /// �̻��ʺ�
        /// </summary>
        [DataMember]
        public string Partner
        {
            get
            {
                if (string.IsNullOrEmpty(partner))
                {
                    throw new ArgumentNullException("Partner", "�̻��ʺ�û������");
                }
                return partner;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("Partner", "�̻��ʺŲ���Ϊ��");
                }
                partner = value;
            }
        }


        /// <summary>
        /// �̻���Կ
        /// </summary>
        [DataMember]
        public string Key
        {
            get
            {
                if (string.IsNullOrEmpty(key))
                {
                    throw new ArgumentNullException("Key", "�̻���Կû������");
                }
                return key;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("Key", "�̻���Կ����Ϊ��");
                }
                key = value;
            }
        }

        /// <summary>
        /// �̻�����
        /// </summary>
        [DataMember]
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
        ///  ΢��֧������Ҫ
        /// </summary>
        [DataMember]
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
        /// ���ػط�֪ͨURL
        /// </summary>
        [DataMember]
        public Uri NotifyUrl
        {
            get
            {
                if (notifyUrl == null)
                {
                    throw new ArgumentNullException("NotifyUrl", "����֪ͨUrlû������");
                }
                return notifyUrl;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("NotifyUrl", "����֪ͨUrl����Ϊ��");
                }
                notifyUrl = value;
            }
        }

        /// <summary>
        /// ����������ת֪ͨURL
        /// </summary>
        [DataMember]
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
        /// ˽Կ��ַ
        /// </summary>
        [DataMember]
        public string PrivateKeyPem { get; set; }

        /// <summary>
        /// ��Կ��ַ
        /// </summary>
        [DataMember]
        public string PublicKeyPem { get; set; }

        /// <summary>
        /// ��������
        /// </summary>
        [DataMember]
        public GatewayType GatewayType { get; set; }

        #endregion

    }
}