using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Astra.Models
{
  public class Report
  {
    private int _ReportID;
    private string _Title;
    private string _Description;
    private bool _IsRemote = true;
    private string _ReportPath;
    private string _Context;

    
    [Key]
    public int ReportID
    {
      get { return _ReportID; }
      set { _ReportID = value; }
    }
    

    [StringLength(255)]
    [Required]
    public string Title
    {
      get { return _Title; }
      set { _Title = value; }
    }

    [StringLength(2000)]
    public string Description
    {
      get { return _Description; }
      set { _Description = value; }
    }


    public bool IsRemote
    {
      get { return _IsRemote; }
      set { _IsRemote = value; }
    }

    [StringLength(255)]
    public string ReportPath
    {
      get { return _ReportPath; }
      set { _ReportPath = value; }
    }

    [StringLength(255)]
    public string Context
    {
      get { return _Context; }
      set { _Context = value; }
    }


  }
}