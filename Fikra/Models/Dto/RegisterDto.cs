using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SparkLink.Models.Dto
{
    public class RegisterDto
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string FatherName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Gender { get; set; }
        [Required]
        public string Country { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        
        public string Password { get; set; }
        [Required]
        [Compare("Password",ErrorMessage ="Password And Password Confirmation are not same")]
        public string ConfirmPassword { get; set; }

        public string? CompanyName { get; set; }
        public string? LinkedinUrl { get; set; }

        public string RoleRequestd { get; set; }
    }
}
