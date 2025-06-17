using SparkLink.Models.Identity;
using System.Reflection.Metadata;

namespace Fikra.Models.Dto
{
    public class GetRequestDto
    {
       public string Id { get; set; }
        public string RequestDetail { get; set; }
   
        public string InvestorName { get; set; }


        public string IdeaTitle { get; set; }
        public string Status { get; set; }



    }
}
