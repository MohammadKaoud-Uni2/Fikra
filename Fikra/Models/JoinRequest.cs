using System.ComponentModel.DataAnnotations.Schema;

namespace Fikra.Models
{
    public class JoinRequest
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int   Id { get; set; }
        public string FreelancerId { get; set; }
        public string ideaTitle { get; set; }
        public string Message { get; set; }
        public string CvUrl { get; set; }
        public DateTime SentAt { get; set; }

        public string IdeaOwnerName { get; set; }

        public string Status { get; set; }
    }
}
