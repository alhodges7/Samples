using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Astra.Models.ViewModels
{
  public class UserMailMessages_IndexViewModel
  {
    private IEnumerable<UserMailMessage> _readMessages;
    public IEnumerable<UserMailMessage> ReadMessages
    {
      get { return _readMessages; }
      set { _readMessages = value; }
    }

    private IEnumerable<UserMailMessage> _unreadMessages;
    public IEnumerable<UserMailMessage> UnreadMessages
    {
      get { return _unreadMessages; }
      set { _unreadMessages = value; }
    }

    private UserProfile _readerUserProfile;
    public UserProfile ReaderUserProfile
    {
      get { return _readerUserProfile; }
      set { _readerUserProfile = value; }
    }
    
  }
}