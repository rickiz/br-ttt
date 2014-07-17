using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BR.ToteToToe.Web.ViewModels
{
    public class WishlistIndexViewModel
    {
        public WishlistIndexViewModel()
        {
            WishlistItems = new List<WishlistItem>();
        }

        public int WishlistID { get; set; }
        public bool IsShareWishlist { get; set; }
        public string ShareUrl { get; set; }
        public List<WishlistItem> WishlistItems { get; set; }
    }

    public class WishlistItem
    {
        public int WishlistItemID { get; set; }
        public string ModelName { get; set; }
        public string BrandName { get; set; }
        public string ColourName { get; set; }
        public string Description { get; set; }
        public string Size { get; set; }
        public string Image { get; set; }
        public int ModelSizeID { get; set; }
        public int ModelID { get; set; }
        public int ColourDescID { get; set; }
        public string SKU { get; set; }
        public string DetailsUrl { get; set; }
    }
}