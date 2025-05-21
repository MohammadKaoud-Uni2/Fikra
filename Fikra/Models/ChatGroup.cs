using Fikra.Hubs;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using SparkLink.Models.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fikra.Models
{
    public class ChatGroup
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        public string Name { get; set; }
        public string IdeaTitle { get; set; }
        public string IdeaOwnerId { get; set; }
        public List<GroupMember> Members { get; set; } = new List<GroupMember>();
        public List<GroupMessage> Messages { get; set; }=new List<GroupMessage>();

        public DateTime CreatedAt { get; set; }
    }
    public class GroupMember
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public string Id { get; set; }
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public string GroupId { get; set; }
        public ChatGroup Group { get; set; }
        public DateTime JoinedAt { get; set; }
        public string UserName { get; set; }
    }
    public class GroupMessage
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        public ChatGroup ChatGroup { get; set; }
        public string ChatGroupId { get; set; }

        public string SenderName { get; set; }
        public string message { get; set; }

        public DateTime SentAt { get; set; }

    }
    public class getChatGroupDto
    {
        public string Id {  set; get; }
        public string IdeaTitle { get; set; }
        public string Name { get; set; }
        public string LastMessage { get; set; }
        public List<string>memebers { get; set; }


    }
}


