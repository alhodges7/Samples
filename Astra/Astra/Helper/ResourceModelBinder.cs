using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Astra.Models;
using Astra.Models.ResourceTypes;

namespace Astra.Helper
{
  public class ResourceModelBinder : DefaultModelBinder
  {
    public override object BindModel(ControllerContext controllerContext,
 ModelBindingContext bindingContext)
    {
      //prepare the model
      var type = controllerContext.HttpContext.Request.Form["Discriminator"];
      bindingContext.ModelName = type; // store type name
      bindingContext.ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(null, resourceTypeMap[type]); // get metadata for that type
      return base.BindModel(controllerContext, bindingContext);
    }

    protected override void BindProperty(ControllerContext controllerContext, ModelBindingContext bindingContext, System.ComponentModel.PropertyDescriptor propertyDescriptor)
    {
      if (propertyDescriptor.DisplayName != null)
      {
        var Form = controllerContext.HttpContext.Request.Form;
        string currentPropertyFormValue = string.Empty;
        string formDerivedTypeKey = bindingContext.ModelName.ToLower() + "." + propertyDescriptor.DisplayName;
        string formBaseTypeKey = propertyDescriptor.DisplayName;
        List<string> keywordList = null;
        Type conversionType = propertyDescriptor.PropertyType;

        if (!string.IsNullOrEmpty(Form[formDerivedTypeKey]) || !string.IsNullOrEmpty(Form[formBaseTypeKey]))
        {
          if (!string.IsNullOrEmpty(Form[formDerivedTypeKey]))
          {
            //store current derived type property
            currentPropertyFormValue = Form[formDerivedTypeKey];
          }
          if (!string.IsNullOrEmpty(Form[formBaseTypeKey]))
          {
            //store current base type property
            currentPropertyFormValue = Form[formBaseTypeKey];
          }
        }

        if (conversionType.IsGenericType)
        {
          if (conversionType.GetGenericTypeDefinition() == typeof(List<>))
          {
            if (propertyDescriptor.DisplayName == "KeyWords")
            {
              string[] keywords = currentPropertyFormValue.Split(',');
              if (keywords != null && keywords.Count() > 0)
              {
                //create keyword list
                keywordList = new List<string>();
                foreach (var item in keywords)
                {
                  if (!string.IsNullOrEmpty(item) && !item.Contains(','))
                  {
                    keywordList.Add(item);
                  }
                }
              }
            }
          }
          if (conversionType.GetGenericTypeDefinition() == typeof(Nullable<>))
          {
            // nullable type property.. re-store nullable type to a safe type
            conversionType = Nullable.GetUnderlyingType(conversionType) ?? propertyDescriptor.PropertyType;
          }
        }
        if (!string.IsNullOrEmpty(currentPropertyFormValue))
        {
          //bind property
          if (propertyDescriptor.DisplayName != "KeyWords")
          {
            propertyDescriptor.SetValue(bindingContext.Model, Convert.ChangeType(currentPropertyFormValue, conversionType));
          }
          else
            propertyDescriptor.SetValue(bindingContext.Model, Convert.ChangeType(keywordList, conversionType));
        }
      }
      else
        base.BindProperty(controllerContext, bindingContext, propertyDescriptor); //default condition
    }

    private static Dictionary<string, Type> resourceTypeMap = new Dictionary<string, Type>
    {
      {"Resource", typeof(Resource)},
      {"Book", typeof(Book)},
      {"DVD", typeof(DVD)},
      {"EBook", typeof(EBook)},
      {"Hardware", typeof(Hardware)},
      {"Software", typeof(Software)},
      {"WhitePaper", typeof(WhitePaper)},
    };
  }
}