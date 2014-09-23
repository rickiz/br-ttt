using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BR.ToteToToe.Web.Areas.Admin.ViewModels
{
    public class BrandModel : BaseRefModel
    {
        [Display(Name = "Category")]
        public int CategoryID { get; set; }

        public string Category { get; set; }
    }

    public class BrandSearchCriteria : BaseRefSearchCriteria
    {
        [Display(Name = "Category")]
        public int? CategoryID { get; set; }
    }

    public class BrandViewModel
    {
        public BrandSearchCriteria Criteria { get; set; }

        public List<BrandModel> SearchResults { get; set; }
    }
}