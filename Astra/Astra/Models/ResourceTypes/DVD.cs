using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Astra.Models.ResourceTypes
{
  public class DVD : Resource
  {
    private string _actors;

    public string Actors
    {
      get { return _actors; }
      set { _actors = value; }
    }
    private string _directors;

    public string Directors
    {
      get { return _directors; }
      set { _directors = value; }
    }
    private string _writers;

    public string Writers
    {
      get { return _writers; }
      set { _writers = value; }
    }
    private string _producers;

    public string Producers
    {
      get { return _producers; }
      set { _producers = value; }
    }
    private string _format;

    public string Format
    {
      get { return _format; }
      set { _format = value; }
    }
    private string _language;

    public string Language
    {
      get { return _language; }
      set { _language = value; }
    }

    private string _subtitles;
    public string Subtitles
    {
      get { return _subtitles; }
      set { _subtitles = value; }
    }
   
    private string _studio;
    public string Studio
    {
      get { return _studio; }
      set { _studio = value; }
    }
   
    private DateTime? _releasedate;
    public DateTime? Releasedate
    {
      get { return _releasedate; }
      set { _releasedate = value; }
    }
 
    private string _runtime;
    public string Runtime
    {
      get { return _runtime; }
      set { _runtime = value; }
    }
  }
}