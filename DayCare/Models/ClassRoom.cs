using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DayCare.Models
{
    public class ClassRoom
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Room_Id { get; set; }

        [DisplayName("Acceptance Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Room_Date { get; set; }
        [Required]
        [DisplayName("Fee"), DataType(DataType.Currency)]
        public decimal Fee_Amount { get; set; }
        public decimal Total_Amount { get; set; }
        public decimal Amount12Months { get; set; }
        public decimal IfafterCare { get; set; }
        public string Employee_Id { get; set; }
        public virtual Employee Employee{ get; set; }
        public string Child_Id { get; set; }
        public virtual Child Child { get; set; }
        public string Driver_ID { get; set; }
        public virtual Driver Driver { get; set; }
        public int DeliveryStatus_ID { get; set; }

        public virtual Deliverystatus Deliverystatus { get; set; }

        public decimal FeeFor12Months(decimal fee_amt){
            return (fee_amt * 12);
        }
        public decimal AfterCare(decimal fee_amt){
            return (FeeFor12Months(fee_amt)/2);
        }

    }
}