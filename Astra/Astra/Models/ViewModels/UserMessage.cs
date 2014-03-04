using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Astra.Models.ViewModels
{
  public class UserMessage
  {
    public enum UserMessageType
    {
      GOOD,
      BAD
    }

    private UserMessageType _currentMessageType = UserMessageType.GOOD;
    public UserMessageType CurrentMessageType
    {
      get { return _currentMessageType; }
      set { _currentMessageType = value; }
    }

    private string _message;
    public string Message
    {
      get { return _message; }
      set { _message = value; }
    }

    public UserMessage(string message, UserMessageType type)
    {
      Message = message;
      CurrentMessageType = type;
    }
  }
}