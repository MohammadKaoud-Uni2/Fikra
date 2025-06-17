using Fikra.Models;
using Fikra.Service.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SparkLink.Models.Identity;
using SparkLink.Service.Interface;

namespace Fikra.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class TransferMoneyController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMoneyTransferRepo _moneyTransferRepo;
        private readonly IIdentityServices _IdentityService;
        private readonly IStripeService _stripeService;
        private readonly IStripeAccountsRepo _stripeAccountsRepo;
        private readonly IStripeCustomer _stripeCustomerRepo;
        public TransferMoneyController (UserManager<ApplicationUser> userManager,IMoneyTransferRepo moneyTransferRepo,IIdentityServices identityServices ,IStripeService stripeService,IStripeAccountsRepo stripeAccountsRepo,IStripeCustomer stripeCustomer)
        {
            _userManager = userManager;
            _moneyTransferRepo = moneyTransferRepo;
            _IdentityService = identityServices;
            _stripeService=stripeService;
            _stripeAccountsRepo = stripeAccountsRepo;
            _stripeCustomerRepo = stripeCustomer;
        }
        [HttpGet]
        [Route("GetAllMoneyRequests")]

        public async Task<IActionResult> GetAllMoneyTransferRequest()
        {
            var MoneyRequests =await  _moneyTransferRepo.GetTableAsNoTracking().Where(x=>x.Statue=="Pending").ToListAsync();
            if (MoneyRequests.Any())
            {
                return Ok(MoneyRequests);   
            }
            return Ok(new
            {
                message = "there are no Request In Your Mailbox Sir!"
            });
        }
        [HttpPost]
        [Route("TransferPercentage")]
        public async Task<IActionResult> TransferPercentageToIdeaOwner([FromQuery] string Id)
        {
            var moneyTransfer = await _moneyTransferRepo.GetTableAsNoTracking().FirstOrDefaultAsync(x => x.Id == Id);
            var Admin = await _userManager.FindByNameAsync("KaoudAdmin");
            StripeCustomer AdminAccount = null;
            AdminAccount = await _stripeCustomerRepo.GetTableAsNoTracking().Include(x => x.ApplicationUser).FirstOrDefaultAsync(x => x.ApplicationUser.UserName == "KaoudAdmin");
            if (AdminAccount == null)
            {
                var newAdminCustomerAccount =await  _stripeService.CreateCustomer(Admin.Email, Admin.UserName);
                var newCustomerAccount = new StripeCustomer()
                {
                    ApplicationUser = Admin,
                    ApplicationUserId = Admin.Id,
                    CreatedAt = DateTime.Now,
                    StripeCustomerId = newAdminCustomerAccount,
                };
                await _stripeCustomerRepo.AddAsync(newCustomerAccount);
                await _stripeCustomerRepo.SaveChangesAsync();
                AdminAccount = newCustomerAccount;
            }
           var IdeaOwner=await _userManager.FindByNameAsync(moneyTransfer.ReceiverUserName);
            StripeAccount IdeaOwnerAccount = null;
            IdeaOwnerAccount = await _stripeAccountsRepo.GetTableAsNoTracking().Include(x=>x.ApplicationUser).FirstOrDefaultAsync(x=>x.ApplicationUser.UserName==moneyTransfer.ReceiverUserName);
            if (IdeaOwnerAccount == null)
            {
                var newStripeAccount = await _stripeService.CreateConnectedAccount(IdeaOwner.Email, IdeaOwner.FirstName, IdeaOwner.LastName, IdeaOwner.UserName);

                var stripeAccount = new StripeAccount
                {
                    ApplicationUserId = IdeaOwner.Id,
                    StripeAccountId = newStripeAccount,
                    BusinessType = "Personal",
                    CreatedAt = DateTime.UtcNow
                };

                await _stripeAccountsRepo.AddAsync(stripeAccount);
                await _stripeAccountsRepo.SaveChangesAsync();
                IdeaOwnerAccount=stripeAccount;
            }

            var result = await _stripeService.SimulateInvestmentToAdmin(AdminAccount.StripeCustomerId, IdeaOwnerAccount.StripeAccountId, moneyTransfer.Amount);

            moneyTransfer.Statue = "Accepted";
            await _moneyTransferRepo.UpdateAsync(moneyTransfer);
           await  _moneyTransferRepo.SaveChangesAsync();
            return Ok(new
            {
                message = "Money Has Been Transfered"
            });
        }
    }
}
