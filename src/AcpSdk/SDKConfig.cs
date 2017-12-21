#if NETSTANDARD2_0
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
#elif NET46
using System.Configuration;
#endif

namespace com.unionpay.acp.sdk
{

    public class SDKConfig
    {
#if NETSTANDARD2_0
        private static IConfigurationRoot _config;

        private static IConfigurationRoot config
        {
            get
            {
                return _config == null ?
                            new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("acp_sdk.json", optional: true, reloadOnChange: true).Build()
                            : _config;
            }
        }

        private static string signCertPath = config["sdk.signCert.path"];  //功能：读取配置文件获取签名证书路径
        private static string signCertPwd = config["sdk.signCert.pwd"];//功能：读取配置文件获取签名证书密码
        private static string validateCertDir = config["sdk.validateCert.dir"];//功能：读取配置文件获取验签目录
        private static string encryptCert = config["sdk.encryptCert.path"];  //功能：加密公钥证书路径

        private static string cardRequestUrl = config["sdk.cardRequestUrl"];  //功能：有卡交易路径;
        private static string appRequestUrl = config["sdk.appRequestUrl"];  //功能：appj交易路径;
        private static string singleQueryUrl = config["sdk.singleQueryUrl"]; //功能：读取配置文件获取交易查询地址
        private static string fileTransUrl = config["sdk.fileTransUrl"];  //功能：读取配置文件获取文件传输类交易地址
        private static string frontTransUrl = config["sdk.frontTransUrl"]; //功能：读取配置文件获取前台交易地址
        private static string backTransUrl = config["sdk.backTransUrl"];//功能：读取配置文件获取后台交易地址
        private static string batTransUrl = config["sdk.batTransUrl"];//功能：读取配批量交易地址

        private static string frontUrl = config["frontUrl"];//功能：读取配置文件获取前台通知地址
        private static string backUrl = config["backUrl"];//功能：读取配置文件获取前台通知地址

        //private static string jfCardRequestUrl = config["sdk.jf.cardRequestUrl"];  //功能：缴费产品有卡交易路径;
        //private static string jfAppRequestUrl = config["sdk.jf.appRequestUrl"];  //功能：缴费产品app交易路径;
        //private static string jfSingleQueryUrl = config["sdk.jf.singleQueryUrl"]; //功能：读取配置文件获取缴费产品交易查询地址
        //private static string jfFrontTransUrl = config["sdk.jf.frontTransUrl"]; //功能：读取配置文件获取缴费产品前台交易地址
        //private static string jfBackTransUrl = config["sdk.jf.backTransUrl"];//功能：读取配置文件获取缴费产品后台交易地址

        private static string ifValidateRemoteCert = config["ifValidateRemoteCert"];//功能：是否验证后台https证书
     
#elif NET46
       private static Configuration config = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");

        private static string signCertPath = config.AppSettings.Settings["sdk.signCert.path"].Value;  //功能：读取配置文件获取签名证书路径
        private static string signCertPwd = config.AppSettings.Settings["sdk.signCert.pwd"].Value;//功能：读取配置文件获取签名证书密码
        private static string validateCertDir = config.AppSettings.Settings["sdk.validateCert.dir"].Value;//功能：读取配置文件获取验签目录
        public static string encryptCert = config.AppSettings.Settings["sdk.encryptCert.path"].Value;  //功能：加密公钥证书路径
  
        private static string cardRequestUrl = config.AppSettings.Settings["sdk.cardRequestUrl"].Value;  //功能：有卡交易路径;
        private static string appRequestUrl = config.AppSettings.Settings["sdk.appRequestUrl"].Value;  //功能：appj交易路径;
        private static string singleQueryUrl = config.AppSettings.Settings["sdk.singleQueryUrl"].Value; //功能：读取配置文件获取交易查询地址
        private static string fileTransUrl = config.AppSettings.Settings["sdk.fileTransUrl"].Value;  //功能：读取配置文件获取文件传输类交易地址
        private static string frontTransUrl = config.AppSettings.Settings["sdk.frontTransUrl"].Value; //功能：读取配置文件获取前台交易地址
        private static string backTransUrl = config.AppSettings.Settings["sdk.backTransUrl"].Value;//功能：读取配置文件获取后台交易地址
        private static string batTransUrl = config.AppSettings.Settings["sdk.batTransUrl"].Value;//功能：读取配批量交易地址

        private static string frontUrl = config.AppSettings.Settings["frontUrl"].Value;//功能：读取配置文件获取前台通知地址
        private static string backUrl = config.AppSettings.Settings["backUrl"].Value;//功能：读取配置文件获取前台通知地址

        //private static string jfCardRequestUrl = config.AppSettings.Settings["sdk.jf.cardRequestUrl"].Value;  //功能：缴费产品有卡交易路径;
        //private static string jfAppRequestUrl = config.AppSettings.Settings["sdk.jf.appRequestUrl"].Value;  //功能：缴费产品app交易路径;
        //private static string jfSingleQueryUrl = config.AppSettings.Settings["sdk.jf.singleQueryUrl"].Value; //功能：读取配置文件获取缴费产品交易查询地址
        //private static string jfFrontTransUrl = config.AppSettings.Settings["sdk.jf.frontTransUrl"].Value; //功能：读取配置文件获取缴费产品前台交易地址
        //private static string jfBackTransUrl = config.AppSettings.Settings["sdk.jf.backTransUrl"].Value;//功能：读取配置文件获取缴费产品后台交易地址

        private static string ifValidateRemoteCert = config.AppSettings.Settings["ifValidateRemoteCert"].Value;//功能：是否验证后台https证书
#endif
        public static string CardRequestUrl
        {
            get { return SDKConfig.cardRequestUrl; }
            set { SDKConfig.cardRequestUrl = value; }
        }
        public static string AppRequestUrl
        {
            get { return SDKConfig.appRequestUrl; }
            set { SDKConfig.appRequestUrl = value; }
        }

        public static string FrontTransUrl
        {
            get { return SDKConfig.frontTransUrl; }
            set { SDKConfig.frontTransUrl = value; }
        }
        public static string EncryptCert
        {
            get { return SDKConfig.encryptCert; }
            set { SDKConfig.encryptCert = value; }
        }


        public static string BackTransUrl
        {
            get { return SDKConfig.backTransUrl; }
            set { SDKConfig.backTransUrl = value; }
        }

        public static string SingleQueryUrl
        {
            get { return SDKConfig.singleQueryUrl; }
            set { SDKConfig.singleQueryUrl = value; }
        }

        public static string FileTransUrl
        {
            get { return SDKConfig.fileTransUrl; }
            set { SDKConfig.fileTransUrl = value; }
        }

        public static string SignCertPath
        {
            get { return SDKConfig.signCertPath; }
            set { SDKConfig.signCertPath = value; }
        }

        public static string SignCertPwd
        {
            get { return SDKConfig.signCertPwd; }
            set { SDKConfig.signCertPwd = value; }
        }

        public static string ValidateCertDir
        {
            get { return SDKConfig.validateCertDir; }
            set { SDKConfig.validateCertDir = value; }
        }
        public static string BatTransUrl
        {
            get { return SDKConfig.batTransUrl; }
            set { SDKConfig.batTransUrl = value; }
        }
        public static string BackUrl
        {
            get { return SDKConfig.backUrl; }
            set { SDKConfig.backUrl = value; }
        }
        public static string FrontUrl
        {
            get { return SDKConfig.frontUrl; }
            set { SDKConfig.frontUrl = value; }
        }
        public static string JfCardRequestUrl
        {
            get { return SDKConfig.cardRequestUrl; }
            set { SDKConfig.cardRequestUrl = value; }
        }
        //public static string JfAppRequestUrl
        //{
        //    get { return SDKConfig.jfAppRequestUrl; }
        //    set { SDKConfig.jfAppRequestUrl = value; }
        //}

        //public static string JfFrontTransUrl
        //{
        //    get { return SDKConfig.jfFrontTransUrl; }
        //    set { SDKConfig.jfFrontTransUrl = value; }
        //}
        //public static string JfBackTransUrl
        //{
        //    get { return SDKConfig.jfBackTransUrl; }
        //    set { SDKConfig.jfBackTransUrl = value; }
        //}
        //public static string JfSingleQueryUrl
        //{
        //    get { return SDKConfig.jfSingleQueryUrl; }
        //    set { SDKConfig.jfSingleQueryUrl = value; }
        //}

        public static string IfValidateRemoteCert
        {
            get { return SDKConfig.ifValidateRemoteCert; }
            set { SDKConfig.ifValidateRemoteCert = value; }
        }
    }
}