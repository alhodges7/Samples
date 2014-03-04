using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Astra.Models
{
  [Table("UserProfile")]
  public class UserProfile
  {
        public UserProfile()
        {
          IsActive = true;
        }

        [Key]
        public int Id { get; set; }

        public string MID { get; set; }

        [StringLength(75)]
        [Required(ErrorMessage = "You must enter a last name.")]
        [MinLength(1)]
        [DisplayName("Last Name")]
        [RegularExpression(@"([a-zA-Z]+(_[a-zA-Z]+)*)", ErrorMessage = "Last Name must be alphabetical")]
        public string LastName { get; set; }

        [StringLength(75)]
        [Required(ErrorMessage = "You must enter a first name.")]
        [MinLength(1)]
        [DisplayName("First Name")]
        [RegularExpression(@"([a-zA-Z]+(_[a-zA-Z]+)*)", ErrorMessage = "First Name must be alphabetical")]
        public string FirstName { get; set; }

        [StringLength(255)]
        public string Email { get; set; }

        public bool IsActive { get; set; }
        public string Preferences { get; set; }

  }
}