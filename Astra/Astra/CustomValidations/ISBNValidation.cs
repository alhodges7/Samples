using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Astra.Models;
using Astra.Models.ResourceTypes;

namespace Astra.CustomValidations
{
  public class ISBNValidation : ValidationAttribute
  {
   
    public ISBNValidation()
      : base("{0} Is not in the correct length it needs to be 10 or 13 digits")
    {
      
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
      if (value != null)
      {
        var valueAsString = value.ToString();
        int valueLenght = Book.CleanUpISBN(valueAsString).Length;
        if (valueLenght == 10)
        {
          return ValidationResult.Success;
        }
        else if (valueLenght == 13)
        {
          return ValidationResult.Success;
        }
        else
        {
          var errorMesage = FormatErrorMessage(validationContext.DisplayName);
          return new ValidationResult(errorMesage);
        }
      }
      return ValidationResult.Success;
    }
  }
}