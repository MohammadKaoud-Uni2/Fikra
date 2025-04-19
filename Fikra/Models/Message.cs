namespace Fikra.Models
{
    public class Message
    {
        public string Id { get; set; }
        public string SenderUserName { get; set; }
        public string ReciverUserName { get; set; }
        public string Content { get; set; }
        public DateTime SendAt { get; set; }
        public bool IsRead { get; set; }

        
    }
}
