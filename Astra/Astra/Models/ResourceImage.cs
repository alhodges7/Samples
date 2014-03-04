using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Astra.Models
{
    public class ResourceImage
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public byte[] ImageData { get; set; }

        [Required]
        public byte[] ImageThumbnail { get; set; }

        [StringLength(1000, MinimumLength = 0)]
        public string ContentType { get; set; }

        [StringLength(1000, MinimumLength = 0)]
        public string Caption { get; set; }
    }
}