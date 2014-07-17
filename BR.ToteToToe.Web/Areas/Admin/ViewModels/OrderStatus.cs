using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BR.ToteToToe.Web.Areas.Admin.ViewModels
{
    public class OrderStatusViewModel
    {
        public OrderStatusSearchCriteria Criteria { get; set; }
        public List<OrderStatusSearchResult> Results { get; set; }
        public OrderStatusUpdateViewModel UpdateModel { get; set; }

        public OrderStatusViewModel()
        {
            Criteria = new OrderStatusSearchCriteria();
            UpdateModel = new OrderStatusUpdateViewModel();
        }
    }

    public class OrderStatusSearchCriteria
    {
        [Display(Name = "Order Number")]
        public int? SalesOrderID { get; set; }

        [Display(Name = "Status")]
        public int StatusID { get; set; }
    }

    public class OrderStatusSearchResult
    {
        public int SalesOrderID { get; set; }
        public string Status { get; set; }
        public string Email { get; set; }
    }

    public class OrderStatusUpdateViewModel
    {
        public int SalesOrderID { get; set; }
        public DateTime DeliveryDT { get; set; }
    }
}