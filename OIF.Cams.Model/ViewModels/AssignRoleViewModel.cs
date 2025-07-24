using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace OIF.Cams.Model.ViewModels
{
    public class AssignRoleViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public List<string> AssignedRoles { get; set; } = new List<string>();

        public string SelectedRole { get; set; }

        public List<SelectListItem> AvailableRoles { get; set; } = new List<SelectListItem>();
    }
}
