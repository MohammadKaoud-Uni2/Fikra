using Fikra.Service.Implementation;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fikra.Models.Dto
{
    public class AddIdeaDto
    {
        
        public string Title { get; set; }

        public string? CompetitiveAdvantage { get; set; }
        public string? ProblemStatement { get; set; }

        public string Category { get; set; }
        public string TargetAudience { get; set; }
        public List<string> Tools { get; set; }
        public int ExpectedUserCount { get; set; }

        public List<string> Features { get; set; }
        public bool BigServerNeeded { get; set; }

        public string? ShortDescription { get; set; }
        public bool HaveBigFiles { get; set; }

 
        public string Country { get; set; }



        public decimal EstimatedMonthlyRevenuePerUser { get; set; }
        public double RealisticConversionRate { get; set; } // in percent
        public int RetentionMonths { get; set; } // average months a user stays


        public bool RequiresRealTimeFeatures { get; set; }


        public bool RequiresDevOpsSetup { get; set; }
        public string FrontendComplexity { get; set; } // Simple/Medium/Complex 
        public string SecurityCriticalLevel { get; set; } // Normal/Sensitive/Highly Sensitive
        public string DeploymentFrequency { get; set; }

    }
}
