using System.ComponentModel.DataAnnotations.Schema;

namespace Fikra.Models
{
    public class IdeaRating
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public string Id { get; set; }

        public virtual Idea Idea { get; set; }
        public string  IdeaId { get; set; }
        public string UserId { get; set; } 
        public double?  Rating { get; set; }
        public DateTime RatedAt { get; set; }
        

      
        
    }
}
