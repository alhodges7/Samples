using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Astra.CustomValidations;

namespace Astra.Models
{
  public class RegisterModel
  {
    [Required]
    [MindtreeIDValidator()]
    [Display(Name = "Mindtree ID (MID):")]
    public string MID { get; set; }

    [Required]
    [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = "Confirm password")]
    [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; }

    [StringLength(50, MinimumLength = 2)]
    [RegularExpression(@"([a-zA-Z\d]+[\w\d]*|)[a-zA-Z]+[\w\d.]*", ErrorMessage = "No symbols in last name")]
    [Display(Name = "Last Name")]
    public string LastName { get; set; }
    [StringLength(50, MinimumLength = 2)]
    [RegularExpression(@"([a-zA-Z\d]+[\w\d]*|)[a-zA-Z]+[\w\d.]*", ErrorMessage = "No symbols in first name")]
    [Display(Name = "First Name")]
    public string FirstName { get; set; }

    [Display(Name = "Email")]
    [EmailAddress(ErrorMessage = "Make sure e-mail is in correct format.")]
    public string Email { get; set; }
  }

}