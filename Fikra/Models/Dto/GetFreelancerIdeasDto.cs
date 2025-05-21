using Fikra.Service.Implementation;
using SparkLink.Models.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fikra.Models.Dto
{
    public class GetFreelancerIdeasDto
    {
        public string Id { get; set; }



        public string IdeaOwnerName { get; set; }
        public string Title { get; set; }

        public string ProblemStatement { get; set; }

        public string Category { get; set; }

        public List<string> Tools { get; set; }

        public List<string> Features { get; set; }


        public string Country { get; set; }

        public bool RequiresRealTimeFeatures { get; set; }
        public string? ShortDescription { get; set; }

        public bool RequiresDevOpsSetup { get; set; }
        public string FrontendComplexity { get; set; } 
        public string SecurityCriticalLevel { get; set; } 
        public string DeploymentFrequency { get; set; }


    

    }
}
