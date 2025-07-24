using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace OIF.Cams.Model.ViewModels
{
    public class CreateUserViewModel
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.(com)$", ErrorMessage = "Email must be in the format abc@xyz.com")]
        [Display(Name = "Email")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
            ErrorMessage = "Password must be at least 8 characters long and include an uppercase letter, a lowercase letter, a number, and a special character.")]
        public string UserPassword { get; set; }

        [Required]
        public string Role { get; set; }
        public List<SelectListItem> AllRoles { get; set; } = new List<SelectListItem>();

        public string Salutation { get; set; }

        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime? Dob { get; set; }

        [Required]
        public long? GenderId { get; set; }
        public List<SelectListItem> GenderList { get; set; } = new List<SelectListItem>();

        [Required]
        public string MobileNo { get; set; }
        public string FullAddress { get; set; }

        [Required]
        public long? NationalityId { get; set; }
        public List<SelectListItem> NationalityList { get; set; } = new List<SelectListItem>();

        [Required]
        public long? BranchId { get; set; }
        public List<SelectListItem> BranchList { get; set; } = new List<SelectListItem>();

        [Required]
        public long? OrganisationId { get; set; }
        public List<SelectListItem> OrganisationList { get; set; } = new List<SelectListItem>();

        public string AssignUnderUser { get; set; }
        public List<SelectListItem> AssignUnderUserList { get; set; } = new List<SelectListItem>();
    }

}
