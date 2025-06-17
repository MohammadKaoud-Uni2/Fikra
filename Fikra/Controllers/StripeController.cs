using Fikra.Models.Dto;
using Fikra.Models;
using Fikra.Service.Implementation;
using Fikra.Service.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SparkLink.Data;
using SparkLink.Models.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Fikra.Helper;
using QuestPDF.Infrastructure;
using SparkLink.Service.Interface;
using System.Reflection.Metadata.Ecma335;
using Microsoft.Identity.Client;

namespace Fikra.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StripeController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IStripeService _stripeService ;
        private readonly IStripeAccountsRepo _stripeAccountsRepo ;
        private readonly IIdentityServices _identityServices ;
        private readonly IStripeCustomer _stripeCustomerRepo ;
        private readonly ITransictionRepo _transictionRepo ;
        public StripeController(ApplicationDbContext context,UserManager<ApplicationUser>userManager,IStripeService stripeService,IStripeAccountsRepo stripeAccountsRepo,IIdentityServices identityServices,IStripeCustomer stripeCustomers,ITransictionRepo transictionRepo)
        {
            _stripeAccountsRepo = stripeAccountsRepo;
            _stripeService = stripeService;
            _userManager = userManager;
            _identityServices = identityServices;
            _stripeCustomerRepo = stripeCustomers;
            _transictionRepo = transictionRepo;

        }
        [HttpPost("CreateConnectedAccount")]
       [Authorize(AuthenticationSchemes ="Bearer")]
        public async Task<IActionResult> CreateStripeAccount([FromBody] CreateAccountDto request)
        {
            var currentUserName=await _identityServices.GetCurrentUserName();
            var user=await _userManager.FindByNameAsync(currentUserName);
            if (string.IsNullOrEmpty(user.Email))
            {
                return BadRequest("Email Is required.");
            }
            var accounts = _stripeAccountsRepo.GetTableAsNoTracking();

            var accountsWithApplicationUsers = await accounts.Include(x => x.ApplicationUser).ToListAsync();
            var existUser= accountsWithApplicationUsers.FirstOrDefault(x=>x.ApplicationUser.Email == user.Email);
            if(existUser!=null)
            {
                return BadRequest("this User Already Has Account !!!");
            }
            var existUserEmail=await _userManager.FindByEmailAsync(user.Email);
            if (existUserEmail == null)
            {
                return BadRequest("This Email is  not Registered ");
            }

            var stripeAccountId = await _stripeService.CreateConnectedAccount(user.Email,user.FirstName,user.LastName,user.UserName, PostCode:null,city:null,state:null);
           
          
            var stripeAccount = new StripeAccount
            {
                ApplicationUserId = user.Id, 
                StripeAccountId = stripeAccountId,
                BusinessType = "Personal",
                CreatedAt = DateTime.UtcNow
            };

          await   _stripeAccountsRepo.AddAsync(stripeAccount);
            await _stripeAccountsRepo.SaveChangesAsync();

            return Ok(new { Message = "Stripe account created successfully!", StripeAccountId = stripeAccountId });
        }
        [HttpPost("admin-transfer")]
        [Authorize(AuthenticationSchemes ="Bearer")]
        public async Task<IActionResult> AdminTransfer([FromBody] TransferRequestDto request)
        {
            var stripeCustomersaccounts = _stripeCustomerRepo.GetTableIQueryable().Include(x=>x.ApplicationUser);
            var checkInvestor = await stripeCustomersaccounts.FirstOrDefaultAsync(x => x.StripeCustomerId == request.InvestorAccountId);
            if (checkInvestor == null)
            {
                return BadRequest("You Dont Have StripeConnected Account !!");
            }
          var User=await   _userManager.FindByIdAsync(checkInvestor.ApplicationUserId);
          var investorRolesCheck= await  _userManager.GetRolesAsync(User);
            var Admin = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == "KaoudAdmin");
            if (!investorRolesCheck.Contains("Investor"))
            {
                return BadRequest("Account With Sending Profit is not Investor role ");
            }
            
           
            if (request.TotalProfit <= 0)
                return BadRequest("Invalid total profit amount.");

            try
            {
                var result = await _stripeService.SimulateInvestmentToAdmin(request.InvestorAccountId, "acct_1REUOX2LG0bR7nld", request.TotalProfit);
               var newTransaction= new Transaction()
                {
                    Amount = request.TotalProfit,
                    CreatedAt = DateTime.UtcNow,
                    Status = "Pending",
                    StripePaymentIntentId = result,
                    InvestorId = User.Id,
                    IdeaOwnerId = Admin.Id,
                };
                await _transictionRepo.AddAsync(newTransaction);
                
                await _transictionRepo.SaveChangesAsync();
              
                return Ok(new { Message = "Transfer successful", TransferId = result });
                
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Transfer failed: {ex.Message}");
            }
        }
        [HttpPost]
        [Authorize(AuthenticationSchemes ="Bearer")]
        [Route("TransferFromAdmin")]
        public async Task<IActionResult> TransferFromAdmin([FromBody] TransferFromAdminDto request)
        {
            var adminCurrentName =await  _identityServices.GetCurrentUserName();
            var adminUser = await _userManager.FindByNameAsync(adminCurrentName);
            if (adminUser == null)
            {
                return BadRequest("Admin is Not Here");
            }

            var stripeCustomersaccounts = _stripeCustomerRepo.GetTableIQueryable().Include(x => x.ApplicationUser);
            var checkAdmin = await stripeCustomersaccounts.FirstOrDefaultAsync(x => x.ApplicationUserId == adminUser.Id);
            if (checkAdmin == null)
            {
                return BadRequest("You Dont Have StripeConnected Account !!");
            }
            var adminrolecheck = await _userManager.GetRolesAsync(adminUser);
            if (!adminrolecheck.Contains("Admin"))
            {
                return BadRequest("Account With Sending Profit is not Admin role ");
            }
            var ideaOwnerConnectedAccount = await _stripeAccountsRepo.GetTableAsNoTracking().FirstOrDefaultAsync(x => x.StripeAccountId == request.IdeaOwnerId);
            
            var ideaOwner =await _userManager.FindByIdAsync(ideaOwnerConnectedAccount.ApplicationUserId);
            if (ideaOwner == null)
            {
                return BadRequest("There is No IdeaOwner with this Id");
            }
            var ideaOwnerCheckRole=await _userManager.GetRolesAsync(ideaOwner);
            if (!ideaOwnerCheckRole.Contains("IdeaOwner"))
            {
                return BadRequest("This User Is Not Considered as IdeaOwner");
            }

            if (request.TotalAmount <= 0)
                return BadRequest("Invalid total profit amount.");

            try
            {
                var result = await _stripeService.SimulateInvestmentFromAdmin(checkAdmin.StripeCustomerId, ideaOwnerConnectedAccount.StripeAccountId, request.TotalAmount);
                var newTransaction = new Transaction()
                {
                    Amount = request.TotalAmount,
                    CreatedAt = DateTime.UtcNow,
                    Status = "Pending",
                    StripePaymentIntentId = result,
                    InvestorId = adminUser.Id,
                    IdeaOwnerId = ideaOwner.Id,
                };
                await _transictionRepo.AddAsync(newTransaction);

                await _transictionRepo.SaveChangesAsync();

                return Ok(new { Message = "Transfer successful", TransferId = result });

            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Transfer failed: {ex.Message}");
            }

        }
        [HttpPost("Create-customer")]
        [Authorize(AuthenticationSchemes ="Bearer")]
      
        public async Task<IActionResult> CreateCustomer()
        {
            var userName=await _identityServices.GetCurrentUserName();
            var CustomerUser = await _userManager.FindByNameAsync(userName);
            if(CustomerUser == null)
            {
                return BadRequest("Not Found");
            }

            var userCheck = await _stripeCustomerRepo.GetTableAsNoTracking().FirstOrDefaultAsync(x => x.ApplicationUserId == CustomerUser.Id);
            if (userCheck != null)
            {
                return BadRequest("This Account Already Consider as Customer");
            }
 ;
            try
            {
                var result = await _stripeService.CreateCustomer(CustomerUser.Email, CustomerUser.UserName);
                var newCustomerAccount = new StripeCustomer()
                {
                    ApplicationUser = CustomerUser,
                    ApplicationUserId = CustomerUser.Id,
                    CreatedAt = DateTime.Now,
                    StripeCustomerId = result,
                };
               await  _stripeCustomerRepo.AddAsync(newCustomerAccount);
                await _stripeCustomerRepo.SaveChangesAsync();
                return Ok($"customer Has Been Created Successfully With Customer Id {result}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Transfer failed: {ex.Message}");
            }
        }
        [HttpGet]
        [Authorize(AuthenticationSchemes ="Bearer")]
        [Route("GetCurrentBalance")]
        public async Task<IActionResult> GetBalanceForCustomer()
        {
          var currentUserName=await  _identityServices.GetCurrentUserName();
          var user=await _userManager.FindByNameAsync(currentUserName);
            var StripeCustomerAccount = await _stripeAccountsRepo.GetTableAsTracking().FirstOrDefaultAsync(x => x.ApplicationUserId == user.Id);
            if (StripeCustomerAccount == null)
            {
                return Ok(0);
            }
           var balanceforcustomer=await _stripeService.GetCustomerBalance(StripeCustomerAccount.StripeAccountId);
            if(balanceforcustomer !=0)
            {

                return Ok(balanceforcustomer);
            }
            return BadRequest("This Customer IsNvalid Id or NotFound ");
            
        }
        [HttpGet]
        //[Authorize(AuthenticationSchemes ="Bearer")]
        [Route("GetPlatformBalance")]
        public async Task<IActionResult> GetPlatformBalance()
        {
            var Balance=await  _stripeService.GetPlatformBalance();
            if (Balance != null) return Ok(Balance);
            else return BadRequest("There Was A problem While Get the Platfrom Balance");
        }
      
        //[HttpPost]
        //[Route("TestValidation")]
        
        //public async Task<IActionResult> TestValidation()
        //{
        //    var postGridAddressValidator = new PostGridAddressValidator();
        //   // UNIT 56 1575, SPRINGHILL DRIVE, KAMLOOPS, BC, V2E2N9

        //   var result =await postGridAddressValidator.ValidateAddressAsync("UNIT 56 1575", "SPRINGHILL DRIVE,", postalCode: "V2E2N9", country:"Syria");
        //    return Ok(result);
        //}


    }
}
