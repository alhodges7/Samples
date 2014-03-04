using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Astra.Models
{
  [Table("EventLog")]
  public class EventLog
  {
    private int _eventLogID;
    private DateTime _logDate;
    private string _thread;
    private string _level;
    private string _logger;
    private string _message;
    private string _exception;

    [Key]
    public int EventLogID
    {
      get { return _eventLogID; }
      set { _eventLogID = value; }
    }

    public DateTime LogDate
    {
      get { return _logDate; }
      set { _logDate = value; }
    }

    [StringLength(255)]
    public string Thread
    {
      get { return _thread; }
      set { _thread = value; }
    }

    [StringLength(20)]
    public string Level
    {
      get { return _level; }
      set { _level = value; }
    }

    [StringLength(255)]
    public string Logger
    {
      get { return _logger; }
      set { _logger = value; }
    }

    [StringLength(4000)]
    public string Message
    {
      get { return _message; }
      set { _message = value; }
    }    

    [StringLength(4000)]
    public string Exception
    {
      get { return _exception; }
      set { _exception = value; }
    }


  }
}