using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DayCare.Models
{
	public class QRCode
	{
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int QRId { get; set; }
        [Display(Name = "QRCode Text")]
        public string QRCodeText { get; set; }
        [Display(Name = "QRCode Image")]
        public string QRCodeImagePath { get; set; }
 
        public string Child_Id { get; set; }
        public virtual Child Child { get; set; }
    }
}