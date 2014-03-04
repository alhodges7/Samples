using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using MTUtil.TypeManagement;
using WebMatrix.WebData;
using Astra.Helper;

namespace Astra.Models
{
  public abstract class AstraBaseModel
  {
    private DateTime _createdOn = DateTime.UtcNow;
    private string _createdByMID;
    private DateTime _lastModifiedOn = DateTime.UtcNow;
    private string _lastModifiedByMID;
    

    /// <summary>
    /// UTC Date/Time this object was created.
    /// </summary>
    public DateTime CreatedOn
    {
      get { return _createdOn; }
      set { _createdOn = value; }
    }

    /// <summary>
    /// MID of the user who created this object.  (May be null
    /// in the case of pre-seeded data).
    /// </summary>
    public string CreatedByMID
    {
      get { return _createdByMID; }
      set { _createdByMID = value; }
    }

    /// <summary>
    /// UTC Date/Time this object was last modified.
    /// </summary>
    public DateTime LastModifiedOn
    {
      get { return _lastModifiedOn; }
      set { _lastModifiedOn = value; }
    }

    /// <summary>
    /// MID of the user who last modified this object (might be null).
    /// </summary>
    public string LastModifiedByMID
    {
      get { return _lastModifiedByMID; }
      set { _lastModifiedByMID = value; }
    }

    [NotMapped]
    public virtual bool IsNew
    {
      get
      {
        object keyVal = this.GetKeyValue();
        if (keyVal == null)
          return false;

        if (keyVal.GetType() == typeof(int))
        {
          int keyValAsInt = (int)keyVal;
          return keyValAsInt <= 0;
        }

        return false;
      }
    }

    public virtual object GetKeyValue()
    {
      PropertyInfo keyProp = null;

      PropertyInfo[] props = this.GetType().GetProperties();
      foreach (PropertyInfo prop in props)
      {
        if (prop.Name.ToLower() == "id")
        {
          keyProp = prop;
          break;
        }
        else if (prop.Name.ToLower() ==  this.GetType().Name.ToLower() + "id")
        {
          keyProp = prop;
          break;
        }
      }

      if (keyProp != null)
        return keyProp.GetValue(this);
      else
        return null;
    }

    
    /// <summary>
    /// Returns true if any pre-save logic is evaluated successfully.
    /// </summary>
    /// <param name="errorMsg"></param>
    /// <returns></returns>
    public virtual object OnBeforeSave()
    {
      DateTime utcNow = DateTime.UtcNow;

      this.LastModifiedByMID = MembershipHelper.CurrentUserName();
      this.LastModifiedOn = utcNow;

      if (this.IsNew)
      {
        this.CreatedByMID = MembershipHelper.CurrentUserName();
        this.CreatedOn = utcNow;
      }
      
      return true;
    }
    

  }
}