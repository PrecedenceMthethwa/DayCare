using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DayCare.Models
{
	public class Beneficiary_Signature
	{
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [DisplayName("Signature ID")]
        public int Signaturee_ID { get; set; }

        [DisplayName("Sign Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Sign_Date { get; set; }


        [UIHint("SignaturePad")]
        [DisplayName("      ")]
        public byte[] MySignature { get; set; }

        [DisplayName("Signed By")]
        public string SignedBy { get; set; }
        public string Child_Id { get; set; }
        public virtual Child Child { get; set; }
    }
}