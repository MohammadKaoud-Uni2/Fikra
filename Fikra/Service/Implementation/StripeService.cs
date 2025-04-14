using Fikra.Helper;
using Fikra.Service.Interface;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.Identity.Client;
using MimeKit;
using SparkLink.Models.Identity;
using SparkLink.Service.Interface;
using Stripe;
using Stripe.Issuing;


namespace Fikra.Service.Implementation
{
    public class StripeService : IStripeService
    {
        private readonly IConfiguration _configuration ;
        private StripeSettings stripeSettings ;
     
      
        public StripeService(IConfiguration configuration)
        {
            _configuration = configuration;
            stripeSettings = new StripeSettings();
            _configuration.GetSection("StripeSettings").Bind(stripeSettings);
            StripeConfiguration.ApiKey = stripeSettings.Secretkey;

        }
      
    

        public async  Task<string> CreateCustomer(string email, string name)
        {
            var options = new CustomerCreateOptions
            {
                Email = email,
                Name = name,
            };

            var service = new CustomerService();
            var customer = await service.CreateAsync(options);

            return customer.Id;
           
        }

        public async Task<string> CreatePaymentIntent(decimal amount, string investorStripeCustomerId, string ideaOwnerAccountId, string currency = "usd")
        {
            var amountInSmallestUnit = (long)(amount * 100);

            var options = new PaymentIntentCreateOptions
            {
                Amount = amountInSmallestUnit,
                Currency = currency,
                Customer = investorStripeCustomerId,
                PaymentMethodTypes = new List<string> { "card" },
                ApplicationFeeAmount = (long)(amount * 0.10m * 100), 
                TransferData = new PaymentIntentTransferDataOptions
                {
                    Destination = ideaOwnerAccountId,
                },
            };

            var service = new PaymentIntentService();
            var paymentIntent = await service.CreateAsync(options);

            return paymentIntent.ClientSecret;
        }

        public async  Task<string> CreatePayout(string ideaOwnerAccountId, decimal amount, string currency = "usd")
        {
            var amountInSmallestUnit = (long)(amount * 100);

            var options = new PayoutCreateOptions
            {
                Amount = amountInSmallestUnit,
                Currency = currency,
                Destination = ideaOwnerAccountId,
            };

            var service = new PayoutService();
            var payout = await service.CreateAsync(options);

            return payout.Id;
        }

        public async  Task<decimal> GetPlatformBalance()
        {
            var service = new BalanceService();
            var balance = await service.GetAsync();

            return balance.Available[0].Amount / 100m;
        }

        public async Task<string> SimulateInvestment(string investorCustomerId, string ideaOwnerAccountId, decimal amount)
        {
            var accountService = new AccountService();

          
            var updateOptions = new AccountUpdateOptions
            {
                Capabilities = new AccountCapabilitiesOptions
                {
                    Transfers = new AccountCapabilitiesTransfersOptions
                    {
                        Requested = true
                    }
                }
            };
            await accountService.UpdateAsync(ideaOwnerAccountId, updateOptions);

           
            bool hasTransfers = false;
            int retries = 0;
            while (!hasTransfers && retries < 10)
            {
                await Task.Delay(1000);
                var updatedAccount = await accountService.GetAsync(ideaOwnerAccountId);
                hasTransfers = updatedAccount.Capabilities?.Transfers == "active";
                retries++;
            }

            if (!hasTransfers)
            {
                throw new Exception("Transfers capability not active after waiting.");
            }

          
            var amountInCents = (long)(amount * 100);
            var applicationFee = (long)(amount * 0.05m * 100); 

            var options = new PaymentIntentCreateOptions
            {
                Amount = amountInCents,
                Currency = "eur",
                Customer = investorCustomerId,
                PaymentMethodTypes = new List<string> { "card" },
                PaymentMethod = "pm_card_visa", 
                Confirm = true,
                OffSession = true,
                ApplicationFeeAmount = applicationFee,
                TransferData = new PaymentIntentTransferDataOptions
                {
                    Destination = ideaOwnerAccountId
                },
            };

            var paymentIntentService = new PaymentIntentService();
            var paymentIntent = await paymentIntentService.CreateAsync(options);

            return paymentIntent.Id; 
        }
    
        public async Task<bool> CloseConnectedAccountAsync(string accountId)
        {
            var service = new AccountService();

            try
            {
                var deletedAccount = await service.DeleteAsync(accountId);
                return (bool) deletedAccount.Deleted;
            }
            catch (StripeException ex)
            {
            

                return false;
            }
        }

       
        public async Task<string> TransferThroughAdminAsync(string investorAccountId, string ideaOwnerAccountId, decimal totalProfit)
        {
          
            long totalAmountInCents = (long)(totalProfit * 100);

      
            decimal platformFee = totalProfit * 0.05m;
            decimal payoutAmount = totalProfit - platformFee;

            long payoutInCents = (long)(payoutAmount * 100);

         
            var transferService = new TransferService();

            var transfer = await transferService.CreateAsync(new TransferCreateOptions
            {
                Amount = payoutInCents,
                Currency = "usd",
                Destination = ideaOwnerAccountId,
                Description = $"Payout to Idea Owner after 5% platform fee deduction from total profit ${totalProfit}"
            });

            return transfer.Id;
        }

     
        public async Task<string> CreateConnectedAccount(string Email,string FirstName,string LastName,string UserName,string ?PostCode,string? state,string? city)
        {
         
            try
            {
                
                var accountOptions = new AccountCreateOptions
                {
                    Type = "custom",
                    Country = "NL",
                    Email = Email,
                    BusinessType = "individual",
                    BusinessProfile = new AccountBusinessProfileOptions
                    {
                        Mcc = "5734", 
                        Url = "https://fikratest.com"
                    },
                    Capabilities = new AccountCapabilitiesOptions
                    {
                        CardPayments = new AccountCapabilitiesCardPaymentsOptions { Requested = true },
                        Transfers = new AccountCapabilitiesTransfersOptions { Requested = true }
                    },
                    Settings = new AccountSettingsOptions
                    {
                        Payouts = new AccountSettingsPayoutsOptions
                        {
                            Schedule = new AccountSettingsPayoutsScheduleOptions
                            {
                                Interval = "manual"
                            }
                        }
                    }
                };

                var accountService = new AccountService();
                var account = await accountService.CreateAsync(accountOptions);

        
                var personService = new AccountPersonService();
                var personOptions = new AccountPersonCreateOptions
                {
                    FirstName = FirstName,
                    LastName = LastName,
                    Relationship = new AccountPersonRelationshipOptions
                    {
                        Representative = true,
                        Executive = true,
                        Title = "CEO"
                    },
                    Dob = new AccountPersonDobOptions
                    {
                        Day = 1,
                        Month = 1,
                        Year = 1990
                    },
                    Address = new AddressOptions
                    {
                        Line1 = "Reigerbos 170",
                        City = "Huissen",
                        State = " Gelderland",
                        PostalCode = "6852 LR",
                        Country = "NL"
                    },
                    Email = Email,
                    Phone = "+3197010580492",
                    SsnLast4 = "0000" 
                };
                await personService.CreateAsync(account.Id, personOptions);

          
                var accountUpdateOptions = new AccountUpdateOptions
                {
                    TosAcceptance = new AccountTosAcceptanceOptions
                    {
                        Date = DateTime.UtcNow,
                        Ip = "127.0.0.1" 
                    }
                };
                await accountService.UpdateAsync(account.Id, accountUpdateOptions);

               
                var externalAccountService = new AccountExternalAccountService(); 
                var bankOptions = new AccountExternalAccountCreateOptions
                {
                    ExternalAccount = new AccountExternalAccountBankAccountOptions
                    {
                        Country = "NL",
                        Currency = "EUR",
                        AccountHolderName = UserName,
                        AccountHolderType = "individual",
                        
                        AccountNumber = "NL91ABNA0417164300"
                    }
                };
                await externalAccountService.CreateAsync(account.Id, bankOptions);

                var updatedAccount = await accountService.GetAsync(account.Id);
                if (updatedAccount.Capabilities?.Transfers != "active" ||
                    updatedAccount.Capabilities?.CardPayments != "active")
                {
                    throw new Exception("Capabilities not active. Check account requirements.");
                }

                return account.Id;
            }
            catch (StripeException e)
            {
                Console.WriteLine($"Stripe Error: {e.Message}");
                throw;
            }
        }

    
    }
}
