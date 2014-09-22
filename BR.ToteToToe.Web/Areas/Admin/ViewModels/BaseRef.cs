using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BR.ToteToToe.Web.Areas.Admin.ViewModels
{
    public class BaseRefModel
    {
        public int ID { get; set; }

        [Required, StringLength(200)]
        public string Name { get; set; }

        [Required]
        public bool Active { get; set; }
    }

    public class BaseRefSearchCriteria
    {
        public int? ID { get; set; }

        [StringLength(200)]
        public string Name { get; set; }

        public bool? Active { get; set; }
    }

    public class BaseRefViewModel
    {
        public string ControllerName { get; set; }

        public BaseRefSearchCriteria Criteria { get; set; }

        public List<BaseRefModel> SearchResults { get; set; }
    }

    public class BaseRefCreateUpdateViewModel : BaseRefModel
    {
        public string ControllerName { get; set; }
    }
}