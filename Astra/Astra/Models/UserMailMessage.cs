using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MTUtil.DateTimes;

namespace Astra.Models
{
  public enum UserMailMessageType
  {
    SystemToUser,
    UserToUser
  }

  public enum UserMailMessageReadState
  {
    Read,
    Unread
  }

  public class UserMailMessage
  {
    public UserMailMessage()
    {
      OriginatorMid = string.Empty;
      SentOn = DateTimeUtils.UTCToLocal(DateTime.UtcNow);
      ReadState = UserMailMessageReadState.Unread;
    }

    private UserMailMessageType _type;
    public UserMailMessageType Type
    {
      get { return _type; }
      set { _type = value; }
    }

    private UserMailMessageReadState _readState;
    public UserMailMessageReadState ReadState
    {
      get { return _readState; }
      set { _readState = value; }
    }

    private int _messageId;
    [Key]
    public int MessageId
    {
      get { return _messageId; }
      set { _messageId = value; }
    }
    

    // We'll just leave this as string.Empty if there is no originator, as in the case of a system message. 
    private string _originatorMid;
    public string OriginatorMid
    {
      get { return _originatorMid; }
      set { _originatorMid = value; }
    }

    private string _recipientMid;
    public string RecipientMid
    {
      get { return _recipientMid; }
      set { _recipientMid = value; }
    }

    private string _messageContent;
    
    [Display(Name="Message Content")]
    [MaxLength(8000)]
    public string MessageContent
    {
      get { return _messageContent; }
      set { _messageContent = value; }
    }

    private DateTime _sentOn;
    public DateTime SentOn
    {
      get { return _sentOn; }
      set { _sentOn = value; }
    }
  }
}