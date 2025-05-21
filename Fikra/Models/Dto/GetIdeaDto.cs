namespace Fikra.Models.Dto
{
    public class GetIdeaDto
    {
        public string Id { get; set; } 

        public string Title { get; set; }
        public string IdeaOwnerName { get; set; }

        public string? CompetitiveAdvantage { get; set; }
        public string? ProblemStatement { get; set; }
        public string ? ShortDescription { get; set; }
        public string Category { get; set; }
        public string TargetAudience { get; set; }

        public string Country { get; set; }


        public double? AverageRating { get; set; }
        public int? RatingCount { get; set; }

    }
}
