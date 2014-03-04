using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

using MTUtil.TypeManagement;
using MTUtil.Xml;
using Astra.Models;
using Astra.Logging;

namespace Astra.SharePointIntegration
{
  /// <summary>
  /// A simple helper class for transmitting a Resource Suggestion to the 
  /// "Suggested Resources" list on the USDC's SharePoint server.
  /// </summary>
  public class SuggestedResourceToSP
  {
    private XmlDocument _TargetXml;
    private ListsSoapClient _ListsService;

    /// <summary>
    /// The blob of Xml that will be passed to the SharePoint service.
    /// </summary>
    private XmlDocument TargetXml
    {      
      get 
      {
        if (_TargetXml == null)
        {
          _TargetXml = XmlUtils.XmlFromString(@"<Batch OnError='Continue' ListVersion='1'>
             <Method ID='1' Cmd='New'>
                <Field Name='Title'>Cool Book 7</Field>
                <Field Name='Description'>A very cool book.</Field>
                <Field Name='Price'>100</Field>              
                <Field Name='Reason_x0020_Needed'>This will help us.</Field>
                <Field Name='Url'>http://www.amazon.com</Field>
                <Field Name='Librarians_x0020_Note'>Sounds good!</Field>
                <Field Name='ISBN10'>1111111111</Field>
                <Field Name='ISBN13'>1111111111111</Field>
             </Method>
          </Batch>");           
        }
        return _TargetXml;
      }
      set { _TargetXml = value; }
    }

   
    /// <summary>
    /// A handle to the SharePoint Lists Web Service Client.
    /// </summary>
    public ListsSoapClient ListsClient
    {
      get { return _ListsService; }
      set { _ListsService = value; }
    }

    /// <summary>
    /// Sets a target XmlElement in our Xml blob.
    /// </summary>
    /// <param name="fieldName">The name of the column we want to update.</param>
    /// <param name="value">The value we want to update.</param>
    /// <returns>The corresponding XmlElement for the target column.</returns>
    private XmlElement SetFieldNode(string fieldName, object value)
    {
      XmlElement fieldNode = (XmlElement)this.TargetXml.SelectSingleNode("//Field[@Name='" + fieldName + "']");
      if (fieldNode != null)
        fieldNode.InnerText = TypeUtils.ToString(value);

      return fieldNode;
    }

    /// <summary>
    /// Transmits a ResourceSuggestion to "Suggested Resources" list on 
    /// the USDC's SharePoint server.
    /// </summary>
    /// <param name="sr">The ResourceSuggestion we are trying to submit.</param>
    /// <returns>True, if successful.</returns>
    public bool TransmitSuggestedResource(ResourceSuggestion sr)
    {
      // set the fields
      this.SetFieldNode("Title", sr.Title);
      this.SetFieldNode("Description", sr.Description);
      this.SetFieldNode("Price", sr.Price);
      this.SetFieldNode("Reason_x0020_Needed", sr.ReasonNeeded);
      this.SetFieldNode("Url", sr.URL);
      this.SetFieldNode("Librarians_x0020_Note", sr.LibrariansNote);
      this.SetFieldNode("ISBN10", sr.ISBN10);
      this.SetFieldNode("ISBN13", sr.ISBN13);

      AstraLogger.LogDebug("About to call UpdateListItems on ListClient...");
      AstraLogger.LogDebug("Xml to be posted: " + this.TargetXml.OuterXml);

      // make the call
      this.ListsClient.UpdateListItems("Suggested Resources", this.TargetXml.DocumentElement);

      return true;

    }


  }
}
