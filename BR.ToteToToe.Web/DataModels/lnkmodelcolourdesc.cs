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
    
    public partial class lnkmodelcolourdesc
    {
        public lnkmodelcolourdesc()
        {
            this.lnkmodelimages = new HashSet<lnkmodelimage>();
        }
    
        public int ID { get; set; }
        public int ModelID { get; set; }
        public int ColourDescID { get; set; }
        public System.DateTime CreateDT { get; set; }
        public Nullable<System.DateTime> UpdateDT { get; set; }
        public bool Active { get; set; }
        public string MainImage { get; set; }
        public string SubImage { get; set; }
        public string SKU { get; set; }
        public int HeelHeight { get; set; }
    
        public virtual ICollection<lnkmodelimage> lnkmodelimages { get; set; }
        public virtual refcolourdesc refcolourdesc { get; set; }
        public virtual refmodel refmodel { get; set; }
    }
}