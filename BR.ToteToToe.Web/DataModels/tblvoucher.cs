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
    
    public partial class tblvoucher
    {
        public tblvoucher()
        {
            this.trnsalesorders = new HashSet<trnsalesorder>();
        }
    
        public int ID { get; set; }
        public string Type { get; set; }
        public string Code { get; set; }
        public int Value { get; set; }
        public System.DateTime CreateDT { get; set; }
        public Nullable<System.DateTime> UpdateDT { get; set; }
        public bool Active { get; set; }
    
        public virtual ICollection<trnsalesorder> trnsalesorders { get; set; }
    }
}
