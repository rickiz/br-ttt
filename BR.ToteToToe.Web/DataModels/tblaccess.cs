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
    
    public partial class tblaccess
    {
        public tblaccess()
        {
            this.tbladdresses = new HashSet<tbladdress>();
        }
    
        public int ID { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool IsAdmin { get; set; }
        public System.DateTime CreateDT { get; set; }
        public Nullable<System.DateTime> UpdateDT { get; set; }
        public bool Active { get; set; }
        public string FacebookAccessToken { get; set; }
        public string FacebookID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string Gender { get; set; }
        public string BirthDateDay { get; set; }
        public string BirthDateMonth { get; set; }
        public string BirthDateYear { get; set; }
        public string EmailToken { get; set; }
        public bool ConfirmedEmail { get; set; }
    
        public virtual ICollection<tbladdress> tbladdresses { get; set; }
    }
}
