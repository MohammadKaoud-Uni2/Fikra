using Fikra.Models;
using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fikra.Service.Interface
{
    public interface IPdfService
    {
        public Task<string> GenerateContract(string ideaOwnerFullName, string investorFullName, double budget, DateTime date,
                                  string ownerSignature, string investorSignature, byte[] logoBytes,string ideaTitle,double  ideaOwnerPercentage);
        public Task<byte[]> ReciveImage();
        public Task<string> GenerateCV(GenerateCVDto freelancerCvDto,string UserName);
    }
    public class FreelancerCvDto
    {
        public string FullName { get; set; }
        public string Title { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string LinkedIn { get; set; }
        public string Country { get; set; }
        public string Summary { get; set; }
        public List<SkillLevel> Technologies { get; set; }
        public List<string> Education { get; set; }
        public List<string> Experience { get; set; }
        public byte[] ProfileImageBytes { get; set; }
    }

    public class SkillLevel
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        public string Technology { get; set; }
        public string Level { get; set; }
        public CV CV { get; set; }
        public string CVId { get; set; }
    }
}
