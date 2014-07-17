using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using BR.ToteToToe.Web.DataModels;

namespace BR.ToteToToe.Web.ViewModels
{
    public class CustomizeViewModels
    {
        public CustomizeViewModels()
        {
            CustomizeModelsList=new List<CustomizeModels>();
        }

        public List<CustomizeModels> CustomizeModelsList { get; set; }
    }

    public class CustomizeModels
    {
        public string Style { get; set; }
        public List<refcustomizemodel> Items { get; set; }
    }

    public class ShoeColourViewModel
    {
        public int CustomizeModelID { get; set; }
        public string Style { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int HeelHeight { get; set; }
        public List<lnkcustomizemodelimage> Items { get; set; }
    }

    public class ShoeDetailsViewModel
    {
        [Required]
        public string Size { get; set; }
        public refcustomizemodel CustomizeModel { get; set; }
        public lnkcustomizemodelimage ModelImage { get; set; }
        public string ShareUrl { get; set; }
    }
}