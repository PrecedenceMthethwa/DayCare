using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DayCare.Models
{
	public class Deliverystatus
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int DeliveryStatus_ID { get; set; }

		[DisplayName("child delivery status")]
		public string Status_Name { get; set; }

		public virtual List<ClassRoom> ClassRooms { get; set; }
	}
}