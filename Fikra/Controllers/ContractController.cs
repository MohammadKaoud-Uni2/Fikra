﻿using AutoMapper;
using Fikra.Hubs;
using Fikra.Models.Dto;
using Fikra.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SparkLink.Models.Identity;
using SparkLink.Service.Interface;

namespace Fikra.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContractController : ControllerBase
    {
        private readonly IContractRepo _contractRepo;
        private UserManager<ApplicationUser> _userManager;
        private readonly IIdentityServices _IdentityService;
        private readonly IMapper _mapper;
        private readonly IPdfService _PdfService;
        private readonly IHubContext<ContractHub>_hubContext;
        private readonly IStripeCustomer _stripeCustomerRepo;
        private readonly IStripeAccountsRepo _stripeAccountsRepo;
        private readonly ITransictionRepo _transictionRepo;

        public ContractController(IContractRepo contractRepo,IMapper mapper,IIdentityServices identityServices,IPdfService pdfService,IHubContext<ContractHub> hubContext,UserManager<ApplicationUser>userManager,IStripeCustomer stripeCustomer,IStripeAccountsRepo stripeAccountsRepo,ITransictionRepo transictionRepo)
        {
            _contractRepo = contractRepo;
            _mapper = mapper;
            _IdentityService= identityServices; 
            _PdfService=pdfService;
            _userManager= userManager;
            _hubContext = hubContext;
            _stripeCustomerRepo= stripeCustomer;
            _stripeAccountsRepo= stripeAccountsRepo;
           _transictionRepo= transictionRepo;
        }

        [HttpGet]
        [Route("GetAllContract")]
        [Authorize(AuthenticationSchemes ="Bearer",Roles ="Admin")]
        public async Task<IActionResult> GetAllContract()
        {
            var contracts = await _contractRepo.GetTableAsNoTracking().ToListAsync();
            if (contracts.Any())
            {
               var contractsAfterMapping=_mapper.Map<List<GetContractDto>>(contracts);
               return Ok(contractsAfterMapping);
            }
            return BadRequest(new
            {
                message="There Are no Contract in DB"
            });

        }
        [HttpPost]
        [Route("GenerateContract")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "IdeaOwner,Investor")]
        public async Task<IActionResult> GenerateContract([FromBody] GenerateContractDto generateContractDto)
        {
            if (ModelState.IsValid) {
                string contractPdfUrl = "";
                var logoimage = await _PdfService.ReciveImage();
                var firstUserName= generateContractDto.IdeaOwnerName;
                var firstUser =await  _userManager.FindByNameAsync(firstUserName);
                var firstUserRoles = await _userManager.GetRolesAsync(firstUser);
                var secondUser=await _userManager.FindByNameAsync(generateContractDto.InvestorName);
                if (firstUserRoles.Contains("IdeaOwner"))
                {
                    contractPdfUrl = await _PdfService.GenerateContract(firstUserName, generateContractDto.InvestorName, generateContractDto.Budget, DateTime.Now, generateContractDto.IdeaOwnerSignature, generateContractDto
                       .InvestorSignature, logoimage,generateContractDto.IdeaTitle,generateContractDto.IdeaOwnerPercentage);


                }
                else
                {
                    contractPdfUrl = await _PdfService.GenerateContract(generateContractDto.InvestorName, firstUserName, generateContractDto.Budget, DateTime.Now, generateContractDto.IdeaOwnerSignature, generateContractDto
                          .InvestorSignature, logoimage,generateContractDto.IdeaTitle,generateContractDto.IdeaOwnerPercentage);
                }
                if (contractPdfUrl.StartsWith("ht"))
                {
                    return Ok($"Pdf Url    {contractPdfUrl} Copy to Download!:) ");
                }
                return BadRequest($"Error:{contractPdfUrl}");

            }
            return BadRequest("There was A problem While Generate the contract becuase there are Missing Fields ");
      
        }
        

    }
}
