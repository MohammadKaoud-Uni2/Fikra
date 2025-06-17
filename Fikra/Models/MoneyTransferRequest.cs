using System.ComponentModel.DataAnnotations.Schema;

namespace Fikra.Models
{
    public class MoneyTransferRequest
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        public string ReceiverUserName { get; set; }
        public  string Statue {  get; set; }
        public double Amount { get; set; }
    }
}
