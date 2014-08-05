using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BR.ToteToToe.Web.Areas.Admin.ViewModels
{
    public class VoucherSearchViewModel
    {
        public VoucherSearchViewModel()
        {
            Results = new List<VoucherDetails>();
        }

        public string CodeFrom { get; set; }
        public string CodeTo { get; set; }
        public string Active { get; set; }
        public List<VoucherDetails> Results { get; set; }
    }

    public class VoucherDetails
    {
        public string Code { get; set; }
        public bool Active { get; set; }
        public string OrderID { get; set; }
        public int Value { get; set; }
    }

    public class VoucherMaintainViewModel
    {
        [Required]
        public string CodeFrom { get; set; }

        [Required]
        public string CodeTo { get; set; }
        
        [Required]
        public int Value { get; set; }
    }
}