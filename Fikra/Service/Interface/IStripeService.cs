using Stripe;

namespace Fikra.Service.Interface
{
    public interface IStripeService
    {
        
        public Task<string> CreatePaymentIntent(decimal amount, string investorStripeCustomerId,
    string ideaOwnerAccountId, string currency = "usd");
        public Task<string> CreatePayout(string ideaOwnerAccountId, decimal amount, string currency = "usd");
        public Task<string> CreateCustomer(string email, string name);
        public  Task<decimal> GetPlatformBalance();
        public  Task<string> SimulateInvestment(string investorCustomerId, string ideaOwnerAccountId, decimal amount);

        public Task<bool> CloseConnectedAccountAsync(string accountId);
        public Task<string> TransferThroughAdminAsync(string investorAccountId, string ideaOwnerAccountId, decimal totalProfit);

        public Task<string> CreateConnectedAccount(string Email,string FirstName,string LastName,string UserName,string? PostCode=null,string ?state=null,string ?city=null);





    }
}
