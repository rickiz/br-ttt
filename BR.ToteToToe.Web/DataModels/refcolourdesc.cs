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
    
    public partial class refcolourdesc
    {
        public refcolourdesc()
        {
            this.lnkmodelsizes = new HashSet<lnkmodelsize>();
            this.lnkwishlists = new HashSet<lnkwishlist>();
            this.lnkmodelcolourdescs = new HashSet<lnkmodelcolourdesc>();
        }
    
        public int ID { get; set; }
        public int ColourID { get; set; }
        public string Name { get; set; }
        public System.DateTime CreateDT { get; set; }
        public Nullable<System.DateTime> UpdateDT { get; set; }
        public bool Active { get; set; }
    
        public virtual refcolour refcolour { get; set; }
        public virtual ICollection<lnkmodelsize> lnkmodelsizes { get; set; }
        public virtual ICollection<lnkwishlist> lnkwishlists { get; set; }
        public virtual ICollection<lnkmodelcolourdesc> lnkmodelcolourdescs { get; set; }
    }
}
