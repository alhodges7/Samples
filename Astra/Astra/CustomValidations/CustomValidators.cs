using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Astra.Models;
using Astra.Models.ResourceTypes;

namespace Astra.CustomValidations
{
  public class ISBNValidator : ValidationAttribute
  {
    private readonly int _isbnLength;
    public ISBNValidator(int isbnLength)
      : base("{0} Is not in the correct length it needs " + isbnLength + " digits")
    {
      _isbnLength = isbnLength;
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
      if (value != null)
      {
        var valueAsString = value.ToString();

        if (Book.CleanUpISBN(valueAsString).Length != _isbnLength)
        {

          var errorMesage = FormatErrorMessage(validationContext.DisplayName);
          return new ValidationResult(errorMesage);

        }

      }
      return ValidationResult.Success;

    }


  }
}