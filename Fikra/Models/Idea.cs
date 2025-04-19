using SparkLink.Models.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fikra.Models
{
    public class Idea
    {
        public Idea()
        {
            Ratings=new HashSet<IdeaRating>();
        }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        public ApplicationUser IdeaOwner { get; set; }
        public string IdeaOwnerId { get; set; }
        public string Title { get; set; }
        public string ShortDescription { get; set; }

        public string ?ProblemStatement { get; set; }

        public string Category { get; set; }
        public string TargetAudience { get; set; } 

        public decimal ?EstimatedBudget { get; set; }
        public decimal ProfitPercentageOffered { get; set; }

        public double? ExpectedROI { get; set; }
        public DateTime CreatedAt { get; set; }
        public double? AverageRating { get; set; } 
        public int ?RatingCount { get; set; } 

       
        public virtual ICollection<IdeaRating>? Ratings { get; set; }



    }
}
