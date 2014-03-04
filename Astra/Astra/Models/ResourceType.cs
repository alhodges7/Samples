using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Astra.Models
{
	public class ResourceType
	{
		private int _resourceTypeID;
		private string _name;
		private string _notes;
		private string _icon = "book.png";

		[Key]
		public int ResourceTypeID
		{
			get { return _resourceTypeID; }
			set { _resourceTypeID = value; }
		}

		[StringLength(255, MinimumLength = 1)]
		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		[StringLength(255, MinimumLength = 0)]
		public string Icon
		{
			get { return _icon; }
			set { _icon = value; }
		}

		public string Notes
		{
			get { return _notes; }
			set { _notes = value; }
		}

	}
}