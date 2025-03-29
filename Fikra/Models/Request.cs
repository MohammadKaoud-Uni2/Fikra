using SparkLink.Models.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fikra.Models
{
    public class Request
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        public string InvestorId { get; set; }
        public ApplicationUser Investor {  get; set; }

        public string IdeaOwnerId { get; set; }
        public ApplicationUser IdeaOwner { get; set; }
    }
}
