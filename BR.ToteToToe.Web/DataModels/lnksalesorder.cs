//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BR.ToteToToe.Web.DataModels
{
    using System;
    using System.Collections.Generic;
    
    public partial class lnksalesorder
    {
        public int ID { get; set; }
        public Nullable<int> SalesOrderID { get; set; }
        public Nullable<int> ModelSizeID { get; set; }
        public int Quantity { get; set; }
        public System.DateTime CreateDT { get; set; }
        public Nullable<System.DateTime> UpdateDT { get; set; }
        public bool Active { get; set; }
        public string SKU { get; set; }
        public string Size { get; set; }
        public string CookieID { get; set; }
        public decimal UnitPrice { get; set; }
    
        public virtual lnkmodelsize lnkmodelsize { get; set; }
        public virtual trnsalesorder trnsalesorder { get; set; }
    }
}
