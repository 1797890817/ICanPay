using System;

namespace ICanPay
{
    public class Refund
    {
        #region 私有字段
        double orderAmount;
        double refundAmount;
        string orderNo;
        string tradeNo;
        string refoundNo;
        DateTime paymentDate;
        #endregion


        #region 构造函数

        public Refund()
        {
        }

        #endregion


        #region 属性

        /// <summary>
        /// 订单总金额，以元为单位。例如：1.00，1元人民币。0.01，1角人民币。因为支付网关要求的最低支付金额为0.01元，所以OrderAmount最低为0.01。
        /// </summary>
        public double OrderAmount
        {
            get
            {
                if (orderAmount < 0.01)
                {
                    throw new ArgumentOutOfRangeException("OrderAmount", "订单金额没有设置");
                }
                return orderAmount;
            }
            set
            {
                if (value < (double)0.01)
                {
                    throw new ArgumentOutOfRangeException("OrderAmount", "订单金额必须大于或等于0.01");
                }
                orderAmount = value;
            }
        }

        /// <summary>
        ///退款金额，以元为单位。例如：1.00，1元人民币。0.01，1角人民币。因为支付网关要求的最低支付金额为0.01元，所以RefundAmount最低为0.01。
        /// </summary>
        public double RefundAmount
        {
            get
            {
                if (refundAmount < 0.01)
                {
                    throw new ArgumentOutOfRangeException("RefundAmount", "退款金额没有设置");
                }
                return refundAmount;
            }
            set
            {
                if (value < (double)0.01)
                {
                    throw new ArgumentOutOfRangeException("RefundAmount", "退款金额必须大于或等于0.01");
                }
                refundAmount = value;
            }
        }


        /// <summary>
        /// 订单单号
        /// </summary>
        public string OrderNo
        {
            get
            {
                if (string.IsNullOrEmpty(orderNo))
                {
                    throw new ArgumentNullException("OrderNo", "订单单号没有设置");
                }
                return orderNo;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("OrderNo", "订单单号不能为空");
                }
                orderNo = value;
            }
        }

        /// <summary>
        /// 交易流水号
        /// </summary>
        public string TradeNo
        {
            get
            { 
                return tradeNo;
            }
            set
            {
                tradeNo = value;
            }
        }

        /// <summary>
        ///商户退款单号
        /// </summary>
        public string RefoundNo
        {
            get
            {
                if (string.IsNullOrEmpty(refoundNo))
                {
                    throw new ArgumentNullException("RefoundNo", "商户退款单号没有设置");
                }
                return refoundNo;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("RefoundNo", "商户退款单号不能为空");
                }
                refoundNo = value;
            }
        }

        /// <summary>
        /// 订单支付时间
        /// </summary>
        public DateTime PaymentDate
        {
            get
            {
                if (paymentDate == DateTime.MinValue)
                {
                    throw new ArgumentNullException("PaymentDate", "订单创建时间未赋值");
                }

                return paymentDate;
            }

            set
            {
                if (value == DateTime.MinValue)
                {
                    throw new ArgumentNullException("PaymentDate", "订单创建时间未赋值");
                }
                paymentDate = value;
            }
        }

        /// <summary>
        /// 退款的原因说明
        /// </summary>
        public string RefundDes { set; get; }

        /// <summary>
        /// 支付渠道退款单号
        /// </summary>
        public string RefoundId { set; get; }

        /// <summary>
        /// 状态
        /// </summary>
        public bool Status { set; get; } = false;
   
        #endregion
    }
}
