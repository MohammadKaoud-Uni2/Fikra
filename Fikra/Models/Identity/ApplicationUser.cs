using EntityFrameworkCore.EncryptColumn.Attribute;
using Fikra.Models;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;



namespace SparkLink.Models.Identity

{
    public class ApplicationUser:IdentityUser
    {
        public ApplicationUser()
        {
            contracts=new HashSet<Contract>();
            Requests = new HashSet<Request>();
            transactions=new HashSet<Transaction>();
        }
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
        public  ICollection<Contract> contracts { get; set; }
        public ICollection<Request>Requests { get; set; }
        public Signature signature { get; set; }
        public ICollection<Transaction> transactions { get; set; }


    }
}
