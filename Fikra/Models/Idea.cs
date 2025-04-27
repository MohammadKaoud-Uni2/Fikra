using Fikra.Service.Implementation;
using Microsoft.Identity.Client;
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
        public string ?CompetitiveAdvantage {  get; set; }
        public string ?ProblemStatement { get; set; }
        public decimal ? InitialInvestment { get; set; }
        public string Category { get; set; }
        public string TargetAudience { get; set; } 
        public List<string>Tools {  get; set; }
        public int ExpectedUserCount { get; set; }
        public decimal ?EstimatedBudget { get; set; }
        public List<string>Features { get; set; }

        [NotMapped]
        public CalculatedInvestment Investment { get; set; }
        public string Country { get; set; }
       
        public DateTime CreatedAt { get; set; }
        public double? AverageRating { get; set; } 
        public int ?RatingCount { get; set; }

        public decimal EstimatedMonthlyRevenuePerUser { get; set; }
        public double RealisticConversionRate { get; set; } // in percent
        public int RetentionMonths { get; set; } // average months a user stays
        public virtual ICollection<IdeaRating>? Ratings { get; set; }
     
        public bool RequiresRealTimeFeatures { get; set; }
        public bool RequiresMobileApp { get; set; }
        public bool RequiresDevOpsSetup { get; set; }
        public string FrontendComplexity { get; set; } // Simple/Medium/Complex // Small/Medium/Big Data
        public string SecurityCriticalLevel { get; set; } // Normal/Sensitive/Highly Sensitive
        public string DeploymentFrequency { get; set; }



    }
}
