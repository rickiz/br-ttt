using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BR.ToteToToe.Web.Areas.Admin.ViewModels
{
    public class CategoryModel : BaseRefModel
    {
        [Required]
        public string Type { get; set; }
        public string Image { get; set; }
    }

    public class CategorySearchCriteria : BaseRefSearchCriteria
    {
        public string Type { get; set; }
        public string Image { get; set; }
    }

    public class CategoryViewModel
    {
        public CategorySearchCriteria Criteria { get; set; }

        public List<CategoryModel> SearchResults { get; set; }
    }
}