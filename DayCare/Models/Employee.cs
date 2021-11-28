using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DayCare.Models
{
    public class Employee
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Display(Name = "Employee ID")]
        public string Employee_Id { get; set; }

        [Display(Name = "Name")]
        public string Employee_Name { get; set; }

        [Display(Name = "Email")]
        [EmailAddress]
        public string Employee_Email { get; set; }

        [Display(Name = "Address")]
        public string Employee_Address { get; set; }

        [Display(Name = "Contact Number")]
        [DataType(DataType.PhoneNumber)]
        [Required(ErrorMessage = "Recipient contact number is required!")]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$",
                 ErrorMessage = "Entered Contact number format is not valid.")]
        public string Employee_contact { get; set; }
        public string Employee_Pass { get; set; }

    }
}