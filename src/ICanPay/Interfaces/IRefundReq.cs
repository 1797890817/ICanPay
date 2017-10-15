namespace ICanPay.Interfaces
{
    internal interface IRefundReq
    {
        /// <summary>
        /// 创建退款
        /// </summary>
        /// <returns></returns>
        Refund BuildRefund(Refund refund);

        /// <summary>
        /// 查询退款结果
        /// </summary>
        /// <returns></returns>
        Refund BuildRefundQuery(Refund refund);
    }
}
