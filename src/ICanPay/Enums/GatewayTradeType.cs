namespace ICanPay.Enums
{
    /// <summary>
    ///  网关的交易类型 
    /// </summary>
    public enum GatewayTradeType
    {
        /// <summary>
        ///未知交易类型
        /// </summary>
        None = -1,

        /// <summary>
        /// 电脑网站支付
        /// </summary>
        Web = 0,

        /// <summary>
        /// 手机网站支付
        /// </summary>
        Wap = 1,

        /// <summary>
        ///  App支付
        /// </summary>
        APP = 2,

        /// <summary>
        /// 二维码支付
        /// </summary>
        QRCode = 3,

        /// <summary>
        /// 公众号支付
        /// </summary>
        Public = 4,

        /// <summary>
        /// 条码支付
        /// </summary>
        BarCode = 5,

        /// <summary>
        /// 小程序支付
        /// </summary>
        Applet = 6,
    }
}
