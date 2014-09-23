using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BR.ToteToToe.Web.Areas.Admin.ViewModels
{
    public class ColourDescModel : BaseRefModel
    {
        [Display(Name="Colour")]
        public int ColourID { get; set; }

        public string Colour { get; set; }
    }

    public class ColourDescSearchCriteria : BaseRefSearchCriteria
    {
        [Display(Name = "Colour")]
        public int? ColourID { get; set; }
    }

    public class ColourDescViewModel
    {
        public ColourDescSearchCriteria Criteria { get; set; }

        public List<ColourDescModel> SearchResults { get; set; }
    }
}