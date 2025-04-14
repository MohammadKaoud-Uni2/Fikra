using SparkLink.Models.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fikra.Models
{
    public class StripeAccount
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public string ApplicationUserId { get; set; }
        public string StripeAccountId { get; set; }
        public string BusinessType { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
