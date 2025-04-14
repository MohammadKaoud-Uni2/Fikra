namespace Fikra.Models.Dto
{
    public class PlatformFeeLog
    {
        public decimal TotalProfit { get; set; }
        public decimal PlatformFee { get; set; }
        public decimal PaidToIdeaOwner { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
