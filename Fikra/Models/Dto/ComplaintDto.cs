namespace Fikra.Models.Dto
{
    public class SubmitComplaintRequest
    {
       
        public string  AgainstUserName { get; set; }
        public ComplaintReason Reason { get; set; }
        public string? AdditionalNotes { get; set; }
        public string? EvidenceUrl { get; set; }
        
    }

    public class ReviewComplaintRequest
    {
        public string Id { get; set; }
        public string FromUserId { get; set; }
        public string AgainstUserName { get; set; }
        public string  Reason { get; set; }
        public string? AdditonalNote { get; set; }
        public string? EvidenceUrl { get; set; }
        public ComplaintStatus Status { get; set; }
     
    }
}
