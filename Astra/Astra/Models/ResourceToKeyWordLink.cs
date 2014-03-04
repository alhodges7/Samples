using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Astra.Models
{
	public class ResourceToKeyWordLink
	{
		private int _linkID;
		private int _keyWordID;
		private KeyWord _keyWord;
		private int _resourceID;
		private Resource _resource;
		

		[Key]
		public int LinkID
		{
			get { return _linkID; }
			set { _linkID = value; }
		}

		public int KeyWordID
		{
			get { return _keyWordID; }
			set { _keyWordID = value; }
		}

		[ForeignKey("KeyWordID")]
		public KeyWord KeyWord
		{
			get { return _keyWord; }
			set { _keyWord = value; }
		}

		public int ResourceID
		{
			get { return _resourceID; }
			set { _resourceID = value; }
		}

		[ForeignKey("ResourceID")]
		public Resource Resource
		{
			get { return _resource; }
			set { _resource = value; }
		}

		
		


	}
}