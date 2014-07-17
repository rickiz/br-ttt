using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BR.ToteToToe.Web.ViewModels
{
    public class PaymentStatusResponseViewModel
    {
        public string TransID { get; set; }
        public string AuthCode { get; set; }
        public string Status { get; set; }
        public string ErrDesc { get; set; }
        public CheckoutPaymentInfo PaymentInfo { get; set; }
    }
}