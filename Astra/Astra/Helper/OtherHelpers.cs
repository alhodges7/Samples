using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Html;

namespace Astra.Helper
{
  public class OtherHelpers
  {
    public static string StandardDateTimeFormat(DateTime someDateTime)
    {
      return someDateTime.ToString("MMMM d, yyyy - h:mm tt");
    }

    public static string ParseUserText(string userText)
    {
      return userText.Replace(Environment.NewLine, "<br/>");
    }

    public static string GetDatabaseThumbnailImagePath(int resourceId)
    {
      return HttpContext.Current.Request.ApplicationPath + resourceId + "_thumb.mti";
    }

    public static string GetDatabaseFullImagePath(int resourceId)
    {
      return HttpContext.Current.Request.ApplicationPath + resourceId + "_full.mti";
    }
    public static string SanitizeHtml(string html)
    {
        string decodedMessageContent = HttpUtility.HtmlDecode(html);
        HtmlSanitizer sanitizer = new HtmlSanitizer();
        return sanitizer.Sanitize(decodedMessageContent); //clean bad tags
    }
    public static bool IsEmptyHtmlOrString(string html)
    {
      if (HttpUtility.Equals("<p></p>\n", html) || html == string.Empty)
      {
        return true;
      }
      return false;
    }
  }
}

