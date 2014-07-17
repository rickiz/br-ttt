using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BR.ToteToToe.Web.ViewModels
{
    public class ShoppingBagIndexViewModel
    {
        public ShoppingBagIndexViewModel()
        {
            ShoppingBagItems = new List<ShoppingBagItem>();
        }

        public bool HasReturn { get; set; }
        public int SalesOrderID { get; set; }
        public decimal GrandTotal { get; set; }
        public List<ShoppingBagItem> ShoppingBagItems{ get; set; }
    }

    public class ShoppingBagItem
    {
        public int SalesOrderItemID { get; set; }
        public string ModelName { get; set; }
        public string ColourDescName { get; set; }
        public string Brand { get; set; }
        public string Size { get; set; }
        public string Availability { get; set; }
        public int AvailableQuantity { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string Image { get; set; }
        public int ModelSizeID { get; set; }
        public int ModelID { get; set; }
        public int ColourDescID { get; set; }
        public string SKU { get; set; }
        public string CookieID { get; set; }
    }
}