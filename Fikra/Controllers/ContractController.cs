using AutoMapper;
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
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Eventing.Reader;

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
        private readonly IDraftRepo _DraftRepo;
        private readonly ISignatureRepo _SignatureRepo;
        private readonly IRSAService _RsaService;

        public ContractController(IContractRepo contractRepo,IMapper mapper,IIdentityServices identityServices,IPdfService pdfService,IHubContext<ContractHub> hubContext,UserManager<ApplicationUser>userManager,IStripeCustomer stripeCustomer,IStripeAccountsRepo stripeAccountsRepo,ITransictionRepo transictionRepo,IDraftRepo draftRepo,ISignatureRepo signatureRepo,IRSAService rSAService)
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
           _DraftRepo= draftRepo;
            _SignatureRepo= signatureRepo;  
            _RsaService= rSAService;
        }

        [HttpGet]
        [Route("GetAllContract")]
        //[Authorize(AuthenticationSchemes ="Bearer",Roles ="Admin")]
        public async Task<IActionResult> GetAllContract()
        {
            var contracts = await _contractRepo.GetTableAsNoTracking().Include(x=>x.IdeaOwner).Include(x=>x.Investor).ToListAsync();
            if (contracts.Any())
            {
               var contractsAfterMapping=_mapper.Map<List<GetContractDto>>(contracts);
               return Ok(contractsAfterMapping);
            }
          return Ok(new List<GetContractDto>());

        }
        //[HttpPost]
        //[Route("GenerateContract")]
        //[Authorize(AuthenticationSchemes = "Bearer", Roles = "IdeaOwner,Investor")]
        //public async Task<IActionResult> GenerateContract([FromBody] GenerateContractDto generateContractDto)
        //{
        //    if (ModelState.IsValid) {
        //        string contractPdfUrl = "";
        //        var logoimage = await _PdfService.ReciveImage();
        //        var firstUserName= generateContractDto.IdeaOwnerName;
        //        var firstUser =await  _userManager.FindByNameAsync(firstUserName);
        //        var firstUserRoles = await _userManager.GetRolesAsync(firstUser);
        //        var secondUser=await _userManager.FindByNameAsync(generateContractDto.InvestorName);
        //        if (firstUserRoles.Contains("IdeaOwner"))
        //        {
        //            contractPdfUrl = await _PdfService.GenerateContract(firstUserName, generateContractDto.InvestorName, generateContractDto.Budget, DateTime.Now, generateContractDto.IdeaOwnerSignature, generateContractDto
        //               .InvestorSignature, logoimage,generateContractDto.IdeaTitle,generateContractDto.IdeaOwnerPercentage);


        //        }
        //        else
        //        {
        //            contractPdfUrl = await _PdfService.GenerateContract(generateContractDto.InvestorName, firstUserName, generateContractDto.Budget, DateTime.Now, generateContractDto.IdeaOwnerSignature, generateContractDto
        //                  .InvestorSignature, logoimage,generateContractDto.IdeaTitle,generateContractDto.IdeaOwnerPercentage);
        //        }
        //        if (contractPdfUrl.StartsWith("ht"))
        //        {
        //            return Ok($"Pdf Url    {contractPdfUrl} Copy to Download!:) ");
        //        }
        //        return BadRequest($"Error:{contractPdfUrl}");

        //    }
        //    return BadRequest("There was A problem While Generate the contract becuase there are Missing Fields ");
      
        //}
        [HttpPost]
        [Authorize(AuthenticationSchemes ="Bearer",Roles ="Investor,IdeaOwner")]
        [Route("GenerateContractDraft")]
        public async Task<IActionResult> GenerateContractDraft([FromBody]GenerateDraftDto generateDraftDto )
        {
            var currentUserName=await _IdentityService.GetCurrentUserName();
            var EncryptedInvestorSignature = _RsaService.SignData(generateDraftDto.Signature);
            var Signature=await _SignatureRepo.GetTableAsNoTracking().Include(x=>x.ApplicationUser).FirstOrDefaultAsync(x=>x.ApplicationUser.UserName==currentUserName);
            if (Signature != null && !Signature.Sign.Equals(EncryptedInvestorSignature))
            {
                return BadRequest("Wrong Signature Entered");
            }
            var Drafts =await  _DraftRepo.GetTableAsNoTracking().ToListAsync();
            var checkdraft = Drafts.Where(x => x.IdeaTitle == generateDraftDto.IdeaTitle && x.UserName == currentUserName &&x.SecondUserName==generateDraftDto.SecondUserName && x.Statue == "Pending");
            if (checkdraft.Any())
            {
                return Ok("You are already Request To Generate Contract Draft Before ");
            }
            var newDraft = new Draft
            {
               
                UserName = currentUserName,
                SecondUserName = generateDraftDto.SecondUserName,
                Signature = generateDraftDto.Signature,
                Budget = generateDraftDto.Budget,
                IdeaOwnerPercentage = generateDraftDto.IdeaOwnerPercentage,
                IdeaTitle = generateDraftDto.IdeaTitle,
                Statue = "Pending"
            };
            await _DraftRepo.AddAsync(newDraft); 
            await _DraftRepo.SaveChangesAsync();
            return Ok();

        }
        [HttpGet]
        [Authorize(AuthenticationSchemes ="Bearer")]
        [Route("GetContractDraft")]
        public async Task<IActionResult> GetContractDraft([FromQuery]GetContractDraftDto getContractDraftDto)
        {
            var currentUserName=await _IdentityService.GetCurrentUserName();
            var user=await _userManager.FindByNameAsync(currentUserName);
            var checkRoles = await _userManager.GetRolesAsync(user);
            var  finalResult = new Draft();
            if (checkRoles.Contains("IdeaOwner"))
            {
                finalResult = await _DraftRepo.GetTableAsTracking().FirstOrDefaultAsync(x => x.SecondUserName == currentUserName && x.IdeaTitle == getContractDraftDto.IdeaTitle);
                if (finalResult == null)
                {
                    return Ok(new { });
                }
                else
                {
                    return Ok(new GetDraftDtoResponse { Id = finalResult.Id, IdeaOwnerPercentage = finalResult.IdeaOwnerPercentage, Budget = finalResult.Budget, IdeaTitle = finalResult.IdeaTitle });
                }
            }
            else
            {
                var draftAlreadyCreated = await _DraftRepo.GetTableAsNoTracking().FirstOrDefaultAsync(x => x.UserName == currentUserName && x.IdeaTitle == getContractDraftDto.IdeaTitle);
                if (draftAlreadyCreated == null)
                {
                    return Ok(new { });
                }

                var result = new GetDraftDtoResponse
                {
                    Id = draftAlreadyCreated.Id,
                    IdeaOwnerPercentage = draftAlreadyCreated.IdeaOwnerPercentage,
                    Budget = draftAlreadyCreated.Budget,
                    IdeaTitle = draftAlreadyCreated.IdeaTitle,

                };
                return Ok(result);
            }
        }
        
       
        [HttpDelete]
        [Route("DeleteDraft")]
        [Authorize(AuthenticationSchemes ="Bearer")]
        public async Task<IActionResult> DeleteDraft([FromQuery] string Id)
        {
            var DraftToDelete =await  _DraftRepo.GetTableAsNoTracking().FirstOrDefaultAsync(x => x.Id == Id);
            if(DraftToDelete == null)
            {
                return NotFound();
            }
          await  _DraftRepo.DeleteAsync(DraftToDelete);
          await _DraftRepo.SaveChangesAsync();
            return Ok();
        }
        [HttpPost]
        [Authorize(AuthenticationSchemes ="Bearer")]
        [Route("SubmitContract")]
        public async Task<IActionResult> SubmitContract([FromBody] SubmitContractDto submitContractDto)
        {
            var currentUserName = await _IdentityService.GetCurrentUserName();
            var draft=await _DraftRepo.GetTableAsNoTracking().FirstOrDefaultAsync(x=>x.Id==submitContractDto.DraftId);
            var EncryptedIdeaOwnerSignature = _RsaService.SignData(submitContractDto.SecondSignature);
            var Signature = await _SignatureRepo.GetTableAsNoTracking().Include(x => x.ApplicationUser).FirstOrDefaultAsync(x => x.ApplicationUser.UserName == currentUserName);
            if (Signature != null && !Signature.Sign.Equals(EncryptedIdeaOwnerSignature))
            {
                return BadRequest("Wrong Signature Entered");
            }
           
         
          
            string contractPdfUrl = "";
            var logoimage = await _PdfService.ReciveImage();
            var firstUserName = draft.UserName;
            var firstUser = await _userManager.FindByNameAsync(firstUserName);
            var firstUserRoles = await _userManager.GetRolesAsync(firstUser);
            var secondUser = await _userManager.FindByNameAsync(draft.SecondUserName);
            if (firstUserRoles.Contains("IdeaOwner"))
            {
                var contracttofind=await _contractRepo.GetTableAsNoTracking().Include(x=>x.IdeaOwner).Include(x=>x.Investor).FirstOrDefaultAsync(x=>x.IdeaOwner.UserName == firstUserName&&x.Investor.UserName==draft.SecondUserName&&x.IdeaTitle==draft.IdeaTitle);
                if (contracttofind != null)
                {
                    return BadRequest("Cannot Add Contract Between You and Him for the same Idea");
                }
                contractPdfUrl = await _PdfService.GenerateContract(firstUserName, draft.SecondUserName,draft.Budget, DateTime.Now, draft.Signature, submitContractDto.SecondSignature
                    , logoimage, draft.IdeaTitle, draft.IdeaOwnerPercentage);


            }
            else
            {
                var contracttofind = await _contractRepo.GetTableAsNoTracking().Include(x => x.IdeaOwner).Include(x => x.Investor).FirstOrDefaultAsync(x => x.IdeaOwner.UserName == draft.SecondUserName && x.Investor.UserName == firstUserName && x.IdeaTitle == draft.IdeaTitle);
                if (contracttofind != null)
                {
                    return BadRequest("Cannot Add Contract Between You and Him for the same Idea");
                }
                contractPdfUrl = await _PdfService.GenerateContract(draft.SecondUserName, firstUserName, draft.Budget, DateTime.Now, submitContractDto.SecondSignature, draft.Signature
                        , logoimage, draft.IdeaTitle, draft.IdeaOwnerPercentage);
            }
            if (contractPdfUrl.StartsWith("ht"))
            {
                return Ok($"Pdf Url    {contractPdfUrl} Copy to Download!:) ");
            }
            return BadRequest($"Error:{contractPdfUrl}");

    

        }
        

    }
    public class GetContractDraftDto 
    {
        public string IdeaTitle { get;set; }
       
    }
    public class GenerateDraftDto
    {
      
        public string Signature { get; set; }
        public string IdeaTitle { get; set; }
        public  double  IdeaOwnerPercentage { get; set; }
        public string SecondUserName { get; set; }
        public double Budget { get; set; }
    }
    public class Draft
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        public string UserName { get; set; }
        public string SecondUserName {get; set; }
        public  string Signature { get; set; }
        public string IdeaTitle { get;set; }
        public  double  IdeaOwnerPercentage { get;set; }
        public double Budget { get; set; }
        public string Statue { get; set; }
    }
    public class GetDraftDtoResponse { 
        public string Id { get; set; }
   
     

        public string IdeaTitle { get; set; }
        public double IdeaOwnerPercentage { get; set; }
        public double Budget { get; set; }

    }
    public class SubmitContractDto
    {
        public string SecondSignature { get; set; }
        public string DraftId { get; set; }

    }


}
