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
        public StripeController(ApplicationDbContext context,UserManager<ApplicationUser>userManager,IStripeService stripeService,IStripeAccountsRepo stripeAccountsRepo,IIdentityServices identityServices)
        {
            _stripeAccountsRepo = stripeAccountsRepo;
            _stripeService = stripeService;
            _userManager = userManager;
            _identityServices = identityServices;

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
        public async Task<IActionResult> AdminTransfer([FromBody] TransferRequestDto request)
        {
            if (request.TotalProfit <= 0)
                return BadRequest("Invalid total profit amount.");

            try
            {
                var result = await _stripeService.SimulateInvestment(request.InvestorAccountId, request.IdeaOwnerAccountId, request.TotalProfit);
                return Ok(new { Message = "Transfer successful", TransferId = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Transfer failed: {ex.Message}");
            }
        }
        [HttpPost("Create-customer")]
        public async Task<IActionResult> CreateCustomer([FromBody] CreateCustomerDto request)
        {
            
            var user=await _userManager.FindByEmailAsync(request.Email);

            try
            {
                var result = await _stripeService.CreateCustomer(request.Email, user.UserName);
                return Ok($"customer Has Been Created Successfully With Customer Id {result}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Transfer failed: {ex.Message}");
            }
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
