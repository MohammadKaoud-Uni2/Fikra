using SparkLink.Models.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fikra.Models.Dto
{
    public class GetContractDto
    {
   
        public string IdeaOwnerName { get; set; }


        public string InvestorName { get; set; }


        public double? Budget { get; set; }
        public DateTime CreateAt { get; set; }

        public double? IdeaOwnerpercentage { get; set; }
        public string ContractPdfUrl { get; set; }
    }
}
