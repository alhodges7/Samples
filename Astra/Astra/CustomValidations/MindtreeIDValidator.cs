using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Astra.CompositeRepository;
using Astra.Models;
using Astra.Repositories;

namespace Astra.CustomValidations
{
    public class MindtreeIDValidator : ValidationAttribute, IClientValidatable
    {
      
      public MindtreeIDValidator() : base("Mindtree Id not in the correct format (i.e. M1234567)")
      {
        
      }

      protected override ValidationResult IsValid(object value, ValidationContext validationContext)
      {
        if (value != null)
        {
          string mid = value.ToString();
          if (!MidIsValid(mid))
          {
            var errorMesage = FormatErrorMessage(validationContext.DisplayName);
            return new ValidationResult(errorMesage);
          }
          else
          {
            using (var repository = new ScopedCompositeRepository().Repositories.UserProfileRepository)
            {
              UserProfile up = repository.FindProfileByMID(mid);
              if (up != null)
              {
                var errorMesage = "ID is already registered.";
                return new ValidationResult(errorMesage);
              }
            }
          }
        }
        return ValidationResult.Success;
      }
      private bool MidIsValid(string mid)
      {
        if (char.ToLower(mid[0]) != 'm')
        {
          return false;
        }
        else if (mid.Length >= 9)
        {
          return false;
        }
        else if (mid.Length < 8)
        {
          return false;
        }
        else 
        {
          for (int i = 1; i < mid.Length; i++)
          {
            if (!char.IsDigit(mid[i]))
              return false;
          }
        }
        return true;
      }

      public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
      {
        yield return new ModelClientValidationRule
        {
          ErrorMessage = this.ErrorMessageString,
          ValidationType = "verifymid",
        };
      }
    }
}
