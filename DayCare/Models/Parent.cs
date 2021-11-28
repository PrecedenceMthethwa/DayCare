using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DayCare.Models
{
    public class Parent
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [DisplayName("Supplier ID")]
        [Required(ErrorMessage = " ID number must be 13 digits")]
        public string Parent_Id { get; set; }

        [Required(ErrorMessage = "Perant name must not be empty")]
        [DisplayName("Supplier Name")]
        public string Parent_Name { get; set; }

        [Required(ErrorMessage = "Perant lastname must not be empty")]
        [DisplayName("Supplier Last Name")]
        public string Parent_LastName { get; set; }

        [Required]
        [DisplayName("Supplier Residence Address")]
        public string Parent_Address { get; set; }

        [DataType(DataType.PhoneNumber)]
        [Required(ErrorMessage = "Recipient contact number is required!")]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$",
                 ErrorMessage = "Entered Contact number format is not valid.")]
        [DisplayName("Supplier Contact Number")]
        public string Parent_ContactNumber { get; set; }

        [Required]
        [DisplayName("Supplier Email Address")]
        [EmailAddress]
        public string Parent_Email { get; set; }

        [DisplayName("Supporting  Documents")]
        public byte[] Parent_Ducuments { get; set; }
        public virtual List<Child> Children { get; set; }

    }
}