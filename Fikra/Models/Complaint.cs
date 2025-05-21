using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.IdentityModel.Tokens;
using SparkLink.Models.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fikra.Models
{
    public class Complaint
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
            public string Id { get; set; }
            public string FromUserId { get; set; }
            public string AgainstUserName { get; set; }
            public string  Reason { get; set; }
            public string? AdditonalNote { get; set; }
            public string? EvidenceUrl { get; set; } 
            public ComplaintStatus Status { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime? ReviewedAt { get; set; }

        
    }
    public enum ComplaintStatus
    {
        Pending,
        Rejected,
        Accepted
    }
    public class PenaltyPoint
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public  string ApplicationUserId { get; set; }
        public DateTime IssuedAt { get; set; }
        public string Reason { get; set; }
        public int Points { get; set; } 
        public Complaint Complaint { get; set; }
        public string ComplaintId { get; set; }
       

    }
    public enum ComplaintReason
    {
        Harassment ,
        ScamOrFraud ,
        SpamOrAdvertising, 
        OffensiveLanguage,
        IntellectualPropertyViolation,
        FakeIdentityOrProfile ,
        ThreatOrViolence ,
        InappropriateContent
        
    }
}
