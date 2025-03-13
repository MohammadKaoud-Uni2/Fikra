using EntityFrameworkCore.EncryptColumn.Attribute;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace SparkLink.Models.Identity

{
    public class ApplicationUser:IdentityUser
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string FatherName { get; set; }
        [Required]
        public string LastName  { get; set; }
        [Required]
        public string Gender { get; set; }
        [Required]
        public string Country { get; set; }
        [EncryptColumn]
        public string? Code { get; set; }

       
        public string? CompanyName { get; set; }
        public string?LinkedinUrl { get; set; }
        public string ? ImageProfileUrl {  get; set; } 
    }
}
