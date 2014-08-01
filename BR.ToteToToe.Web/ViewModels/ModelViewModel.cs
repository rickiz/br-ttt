using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using BR.ToteToToe.Web.DataModels;

namespace BR.ToteToToe.Web.ViewModels
{
    public class ModelListViewModel
    {
        public ModelListViewModel()
        {
            Models = new List<ModelResult>();
            Models12 = new List<ModelResult>();
        }

        //public List<ModelResult> ModelResults { get; set; }
        public List<ModelResult> Models { get; set; }
        public List<ModelResult> Models12 { get; set; }

        public int TrendID { get; set; }
        public int BrandID { get; set; }
        public string BrandName { get; set; }
        public int CategoryID { get; set; }
        public int LifestyleID { get; set; }
        public int Size { get; set; }
        public int ColourID { get; set; }
        public string HeelHeight { get; set; }
        public string Price { get; set; }
    }

    public class ModelResult
    {
        public int ModelID { get; set; }
        public int CategoryID { get; set; }
        public int BrandID { get; set; }
        public int ColourID { get; set; }
        public int ColourDescID { get; set; }
        public string ModelName { get; set; }
        public string CategoryName { get; set; }
        public string BrandName { get; set; }
        public string ColourName { get; set; }
        public string ColourDescName { get; set; }
        public string CategoryImage { get; set; }
        public string ModelMainImage { get; set; }
        public string ModelSubImage { get; set; }
        public decimal Price { get; set; }
        public decimal DiscountPrice { get; set; }
    }

    public class ModelDetailsViewModel
    {
        public ModelDetailsViewModel()
        {
            ReommendedModels = new List<ReommendedModel>();
        }

        public int ModelID { get; set; }
        public string ModelName { get; set; }
        public string CategoryName { get; set; }
        public string BrandName { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public decimal? DiscountPrice { get; set; }
        public string SKU { get; set; }
        public string ModelMainImage { get; set; }
        public string ShareUrl { get; set; }
        public decimal HeelHeight { get; set; }

        [Required(ErrorMessage="Please select Colour")]
        public int ColourDescID { get; set; }
        [Required(ErrorMessage = "Please select Size")]
        public int ModelSizeID { get; set; }

        public List<lnkmodelimage> ModelImages { get; set; }
        public List<ReommendedModel> ReommendedModels { get; set; }
    }

    public class ReommendedModel
    {
        public int ModelID { get; set; }
        public int ColourDescID { get; set; }
        public string ModelName { get; set; }
        public string ColourDescName { get; set; }
        public string BrandName { get; set; }
        public string CategoryName { get; set; }
        public decimal Price { get; set; }
        public decimal DiscountPrice { get; set; }
        public string MainImage { get; set; }
    }
}