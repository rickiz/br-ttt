using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

using BR.ToteToToe.Web.DataModels;

namespace BR.ToteToToe.Web.Areas.Admin.ViewModels
{
    public class ModelViewModel
    {
        public ModelViewModel()
        {
            ModelDetails = new List<ModelDetails>();
        }

        public List<ModelDetails> ModelDetails { get; set; }
    }

    public class ModelDetails
    {
        public string CategoryName { get; set; }
        public string BrandName { get; set; }
        public string ModelName { get; set; }
        public int ModelID { get; set; }
        public int ColourDescID { get; set; }
        public int ModelColourDescID { get; set; }
        public string ColourName { get; set; }
        public bool Active { get; set; }
    }

    public class MaintainModelViewModel
    {
        [Display(Name = "Category")]
        public int CategoryID { get; set; }
        [Display(Name = "Brand")]
        public int BrandID { get; set; }
        [Display(Name = "Brand")]
        public string BrandName { get; set; }
        [Display(Name = "Colour")]
        public int ColourID { get; set; }

        [Required]
        [Display(Name = "Actual Colour")]
        public int ColourDescID { get; set; }
        [Required]
        [Display(Name = "ID")]
        public int ModelID { get; set; }
        [Required]
        [Display(Name = "Name")]
        public string ModelName { get; set; }
        [Required]
        [Display(Name = "Price")]
        public Decimal Price { get; set; }
        [Display(Name = "Discount Price")]
        public Decimal DiscountPrice { get; set; }
        [Required]
        [Display(Name = "Heel Height")]
        public Decimal HeelHeight { get; set; }
        [Required]
        [Display(Name = "SKU")]
        public string SKU { get; set; }
        [Required]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Style")]
        public string Style { get; set; }
        [Display(Name = "Lining Sock")]
        public string LiningSock { get; set; }
        [Display(Name = "Sole")]
        public string Sole { get; set; }
        [Display(Name = "Make")]
        public string Make { get; set; }
        [Display(Name = "Upper Material")]
        public string UpperMaterial { get; set; }
        [Display(Name = "Heel Description")]
        public string HeelDesc { get; set; }

        public List<reftrend> AvailableTrends { get; set; }
        public List<reflifestyle> AvailableLifestyles { get; set; }
        public List<int> SelectedTrend { get; set; }
        public List<int> SelectedLifestyle { get; set; }
    }

    
}