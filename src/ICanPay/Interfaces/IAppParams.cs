using System.Collections.Generic;

namespace ICanPay.Interfaces
{
    /// <summary>
    /// 手机端SDK支付
    /// </summary>
    interface IAppParams
    {
        /// <summary>
        ///创建手机端SDK支付需要信息
        /// </summary>
        Dictionary<string, string> BuildPayParams();
    }
}
