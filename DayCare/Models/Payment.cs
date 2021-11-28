using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DayCare.Models
{
	public class Payment
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		[DisplayName("Child ID")]
		public string Payment_Id { get; set; }
	
	}
}