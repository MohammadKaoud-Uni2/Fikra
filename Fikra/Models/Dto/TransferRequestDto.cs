namespace Fikra.Models.Dto
{
    public class TransferRequestDto
    {
        public string InvestorAccountId { get; set; }
        public string IdeaOwnerAccountId { get; set; }
        public decimal TotalProfit { get; set; } 
    }
}
