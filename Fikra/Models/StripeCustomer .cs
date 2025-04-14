using SparkLink.Models.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Fikra.Models
{
    public class StripeCustomer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public string ApplicationUserId     { get; set; }
        public string StripeCustomerId { get; set; }
        public DateTime CreatedAt { get; set; }

    }
}
