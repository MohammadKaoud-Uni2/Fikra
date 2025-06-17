using SparkLink.Models.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Fikra.Models
{
    public class Transaction
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        public ApplicationUser? Investor { get; set; }
        public string ?InvestorId { get; set; }
        public ApplicationUser? IdeaOwner { get; set; }
        public string ?IdeaOwnerId { get; set; }
        public double Amount { get; set; }
        public string ?StripePaymentIntentId { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }


       

    }
}
