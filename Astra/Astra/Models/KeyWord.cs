using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Astra.Models
{
	public class KeyWord
	{
		private int _keyWordID;
		private string _word;

		[Key]
		public int KeyWordID
		{
			get { return _keyWordID; }
			set { _keyWordID = value; }
		}

		[StringLength(100)]
    [Required]
		public string Word
		{
			get { return _word; }
			set { _word = value; }
		}

	}
}