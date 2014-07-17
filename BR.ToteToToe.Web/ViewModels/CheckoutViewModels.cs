using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

using BR.ToteToToe.Web.DataModels;
using BR.ToteToToe.Web.Helpers;

namespace BR.ToteToToe.Web.ViewModels
{
    public class CheckoutBillingViewModel
    {
        public CheckoutAddress Address { get; set; }

        public int SalesOrderBillingAddressID { get; set; }

        [StringLength(200)]
        [Required]
        public string FirstName { get; set; }

        [StringLength(200)]
        [Required]
        public string LastName { get; set; }

        [StringLength(20)]
        [Required]
        public string Phone { get; set; }

        public int AddressTypeID { get; set; }

        public bool IsAddressReadOnly { get; set; }

        public CheckoutAddressType AddressType { get; set; }

        public CheckoutBillingViewModel()
        {
            Address = new CheckoutAddress();
        }
    }

    public class CheckoutShippingViewModel
    {
        public CheckoutAddress Address { get; set; }

        public int SalesOrderAddressID { get; set; }

        [StringLength(200)]
        [Required]
        public string FirstName { get; set; }

        [StringLength(200)]
        [Required]
        public string LastName { get; set; }

        [StringLength(20)]
        [Required]
        public string Phone { get; set; }

        public int AddressTypeID { get; set; }

        public bool IsAddressReadOnly { get; set; }

        public CheckoutAddressType AddressType { get; set; }

        public CheckoutShippingViewModel()
        {
            Address = new CheckoutAddress();
        }
    }

    public class CheckoutSummaryViewModel
    {
        public int SalesOrderID { get; set; }
        public lnksoaddress BillingAddress { get; set; }
        public lnksoaddress ShippingAddress { get; set; }
        public List<CheckoutSummaryItem> Items { get; set; }
        public decimal Subtotal { get; set; }
        public decimal ShippingPrice { get; set; }
        public decimal OrderTotalPrice { get; set; }
        public CheckoutPaymentInfo PaymentInfo { get; set; }
        public bool AllowPayment { get; set; }

        public string VoucherCode { get; set; }
        public int RebateCashValue { get; set; }
    }

    public class CheckoutSummaryItem
    {
        public string ImageUrl { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public string Size { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }

    public class CheckoutPaymentInfo
    {
        public string EntryUrl { get; set; }
        public string MerchantCode { get; set; }
        public string PaymentId { get; set; }
        public string RefNo { get; set; }
        public string Amount { get; set; }
        public string Currency { get; set; }
        public string ProdDesc { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string UserContact { get; set; }
        public string Remark { get; set; }
        public string Lang { get; set; }
        public string Signature { get; set; }
        public string ResponseUrl { get; set; }
        public string BackendUrl { get; set; }
    }

    public class CheckoutSuccessViewModel
    {
        public int SalesOrderID { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
    }

    public class CheckoutAddress
    {
        public int ID { get; set; }

        public int CountryID { get; set; }

        [StringLength(200), Required]
        public string AddressLine1 { get; set; }

        [StringLength(200), Required]
        public string AddressLine2 { get; set; }

        [StringLength(200), Required]
        public string CityTown { get; set; }

        [StringLength(200), Required]
        public string State { get; set; }

        [StringLength(20), Required]
        public string Postcode { get; set; }

        public bool IsPrimary { get; set; }
    }
}