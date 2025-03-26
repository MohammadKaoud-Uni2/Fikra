using SparkLink.Models.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fikra.Models
{
    public class Contract
    {
        [Key]

        public string Id { get; set; }
        [ForeignKey(nameof(IdeaOwnerId))]
        public string IdeaOwnerId { get; set; }
        public ApplicationUser IdeaOwner { get; set; }

        [ForeignKey(nameof(InvestorId))]
        public string InvestorId { get; set; }
        public ApplicationUser Investor {  get; set; }
        
        
        public double ?Budget {  get; set; }
        public DateTime  CreateAt { get; set; }
        
        public double ?IdeaOwnerpercentage { get; set; }
        public string ContractPdfUrl { get; set; }

    }
}
