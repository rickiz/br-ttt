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
    
    public partial class refbrand
    {
        public refbrand()
        {
            this.refmodels = new HashSet<refmodel>();
        }
    
        public int ID { get; set; }
        public int CategoryID { get; set; }
        public string Name { get; set; }
        public System.DateTime CreateDT { get; set; }
        public Nullable<System.DateTime> UpdateDT { get; set; }
        public bool Active { get; set; }
    
        public virtual refcategory refcategory { get; set; }
        public virtual ICollection<refmodel> refmodels { get; set; }
    }
}
