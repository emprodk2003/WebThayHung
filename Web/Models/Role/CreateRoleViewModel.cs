using System.ComponentModel.DataAnnotations;

namespace Web.Models.Role
{
    public class CreateRoleViewModel
    {
        [Required]
        [Display(Name = "Role")]
        public string RoleName { get; set; }
        public string? Description { get; set; }
    }
}
