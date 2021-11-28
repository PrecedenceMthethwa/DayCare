using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DayCare.Models
{
    public class Child
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [DisplayName("Child ID")]
        public string Child_Id { get; set; }

        [Required]
        [DisplayName("Child Name")]
        public string Child_Name { get; set; }

        [Required]
        [DisplayName("Child Last Name")]
        public string Child_LastName { get; set; }

        [DisplayName("Supporting  Documents")]
        public byte[] Child_Ducuments { get; set; }
        [DisplayName("child Image")]
        public byte[] Child_Image { get; set; }

        [Display(Name = "QRCode Text")]
        public string QRCodeText { get; set; }
        [Display(Name = "QRCode Image")]
        [DisplayName("Payment Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Payment_Date { get; set; }
        public string QRCodeImagePath { get; set; }
        public string Parent_Id { get; set; }
        public virtual Parent Parent { get; set; }
        public bool AfterCare { get; set; }
        public bool Approved { get; set; }
        public bool Payment { get; set; }

    }
}