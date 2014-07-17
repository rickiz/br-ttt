using BR.ToteToToe.Web.DataModels;
using BR.ToteToToe.Web.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BR.ToteToToe.Web.ViewModels
{
    #region Index

    public class MyAccountViewModel
    {
        public tblaccess User { get; set; }
        public List<tbladdress> Addresses { get; set; }
        public List<trnsalesorder> RecentOrders { get; set; }
        public List<MyAccountIndexImageDetail> RecentOrderImages { get; set; }
        public List<MyAccountIndexImageDetail> WishlistImages { get; set; }
    }

    public class MyAccountIndexImageDetail
    {
        public int? ModelSizeID { get; set; }
        public int? ModelID { get; set; }
        public int? ColourDescID { get; set; }
        public string SKU { get; set; }
        public string ImageContentUrl { get; set; }
        public string LinkUrl { get; set; }
    }

    #endregion

    #region Edit

    public class MyAccountEditViewModel
    {
        public int AccessID { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(50)]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        [StringLength(70, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [StringLength(200)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(200)]
        public string LastName { get; set; }

        [StringLength(20)]
        public string Phone { get; set; }

        public Gender? Gender { get; set; }

        public string BirthDateDay { get; set; }
        public string BirthDateMonth { get; set; }
        public string BirthDateYear { get; set; }

        public int? BillingFlag { get; set; }
        public int? ShippingFlag { get; set; }
        public bool EnableHomeFlag { get; set; }
        public bool EnableWorkFlag { get; set; }

        public bool IsFBLogin { get; set; }

        public MyAccountAddress PrimaryAddress { get; set; }
        public MyAccountAddress SecondaryAddress { get; set; }

        public MyAccountEditViewModel()
        {
            PrimaryAddress = new MyAccountAddress { IsPrimary = true };
            SecondaryAddress = new MyAccountAddress();
        }
    }

    public class MyAccountAddress
    {
        public int ID { get; set; }

        public int? CountryID { get; set; }

        [StringLength(200)]
        public string AddressLine1 { get; set; }

        [StringLength(200)]
        public string AddressLine2 { get; set; }

        [StringLength(200)]
        public string CityTown { get; set; }

        [StringLength(200)]
        public string State { get; set; }

        [StringLength(20)]
        public string Postcode { get; set; }

        public bool IsPrimary { get; set; }
    }

    public class RecentViewModel
    {
        public string ImageUrl { get; set; }
        public string LinkUrl { get; set; }
        public DateTime ViewDT { get; set; }
    }


    #endregion

    #region OrderSummary

    public class MyAccountOrderSummaryViewModel
    {
        public int SalesOrderID { get; set; }
        public lnksoaddress BillingAddress { get; set; }
        public lnksoaddress ShippingAddress { get; set; }
        public List<MyAccountOrderSummaryItem> Items { get; set; }
        public decimal Subtotal { get; set; }
        public decimal ShippingPrice { get; set; }
        public decimal OrderTotalPrice { get; set; }
        public string PaymentStatus { get; set; }
        public string PaymentDT { get; set; }
        public string VoucherCode { get; set; }
        public int RebateCashValue { get; set; }
    }

    public class MyAccountOrderSummaryItem
    {
        public string SKU { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public string Size { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Discount { get; set; }
        public decimal SubTotal { get; set; }
    }

    #endregion

    #region OrderHistory

    public class MyAccountOrderHistoryViewModel
    {
        public int StatusID { get; set; }

        public List<MyAccountOrderHistoryItem> Items { get; set; }
    }

    public class MyAccountOrderHistoryItem
    {
        public int SalesOrderID { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; }
        public string ETA { get; set; }
        public string ImageUrl { get; set; }
    }

    #endregion
}