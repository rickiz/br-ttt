using BR.ToteToToe.Web.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BR.ToteToToe.Web.ViewModels
{
    public class SignInRegisterViewModel
    {
        //public string FacebookAccessToken { get; set; }
        //public string FacebookID { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(50)]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(70, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [StringLength(200)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(200)]
        public string LastName { get; set; }

        //public int? CountryID { get; set; }
        //public string AddressLine1 { get; set; }
        //public string AddressLine2 { get; set; }
        //public string CityTown { get; set; }
        //public string State { get; set; }
        //public string Postcode { get; set; }
        //public string Phone { get; set; }
        //public string Gender { get; set; }
        //public string BirthDateDay { get; set; }
        //public string BirthDateMonth { get; set; }
        //public string BirthDateYear { get; set; }

        public string ReturnUrl { get; set; }
    }

    public class SignInViewModel
    {
        [Required]
        [EmailAddress]
        [StringLength(50)]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string ReturnUrl { get; set; }

        public SignInRegisterViewModel RegisterViewModel { get; set; }
    }

    public class SignInConfirmEmailViewModel
    {
        public tblaccess User { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        public string Email { get; set; }
        public string NewPassword { get; set; }
        public string ErrorMessage { get; set; }
    }
}