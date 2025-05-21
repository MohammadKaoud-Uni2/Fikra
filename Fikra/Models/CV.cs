using Fikra.Service.Interface;
using SparkLink.Models.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fikra.Models
{
    public class CV
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
      
            public ApplicationUser ApplicationUser {  get; set; }
            public string ApplicationUserId { get; set; }

            public string Title { get; set; }

            public string Phone { get; set; }
            public string Country { get; set; }
            public string Summary { get; set; }
            public List<SkillLevel> Technologies { get; set; }
            public List<string> Education { get; set; }
            public List<string> Experience { get; set; }
            public string CVPdfUrl { get; set; }

      
        
    }
    public class GenerateCVDto
    {
        public string Title { get; set; }

        public string Phone { get; set; }
        public string Summary { get; set; }
        public List<SkillLevelDto> Technologies { get; set; }
        public List<string> Education { get; set; }
        public List<string> Experience { get; set; }

    }
    public class SkillLevelDto
    {
        public string Technology { get; set; }
        public string Level { get; set; }
    }
}
