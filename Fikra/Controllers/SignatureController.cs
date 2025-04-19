using Fikra.Models;
using Fikra.Models.Dto;
using Fikra.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Paddings;
using SparkLink.Models.Identity;
using SparkLink.Service.Interface;
using System.Security.Cryptography;
using System.Text;

namespace Fikra.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SignatureController : ControllerBase
    {
        private readonly IRSAService _IrsaService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IIdentityServices _identityServices;
        private readonly ISignatureRepo _signatureRepo;
        public SignatureController(IRSAService IrsaService,UserManager<ApplicationUser> userManager,IIdentityServices identityServices,ISignatureRepo signatureRepo)
        {
            _IrsaService = IrsaService;
            _userManager = userManager;
            _identityServices = identityServices;
            _signatureRepo = signatureRepo;
        }
        [HttpPost]
        [Authorize(AuthenticationSchemes ="Bearer")]
        [Route("AddSignature")]
        public async Task<IActionResult> AddSignature([FromBody] SignatureDto SignatureDto)
        {
            var userName = await _identityServices.GetCurrentUserName();
          var user=await   _userManager.FindByNameAsync(userName);
            var resultoffindExistSignature = await _signatureRepo.GetTableAsNoTracking().FirstOrDefaultAsync(x => x.ApplicationUserId == user.Id);
            if (resultoffindExistSignature != null)
            {
                return BadRequest("You have Signature Assigned Before !");
            }
           var resultsignature= _IrsaService.SignData(SignatureDto.Sign);
            var signature = new Signature()
            {
                ApplicationUserId=user.Id,
                Sign= resultsignature

            };
           await  _signatureRepo.AddAsync(signature);
            await _signatureRepo.SaveChangesAsync();
            return Ok("Successfully Operation");


        }
        [HttpGet]
        [Authorize(AuthenticationSchemes ="Bearer")]
        [Route("VerifySignature")]
        public async Task<IActionResult> VerifySignature([FromQuery]SignatureDto signatureDto)
        {
            var userName = await _identityServices.GetCurrentUserName();
            var user = await _userManager.FindByNameAsync(userName);
            var resultoffindExistSignature = await _signatureRepo.GetTableAsNoTracking().FirstOrDefaultAsync(x => x.ApplicationUserId == user.Id);
            var encrypted = _IrsaService.SignData(signatureDto.Sign);
            if (encrypted.Equals(resultoffindExistSignature.Sign))
            {
                return Ok("Person Signature has been  Verified Successfully");

            }
            return BadRequest("Person Signature is  Not Verified !!!!");






        }
        [HttpGet]
        [Route("TestDecryption")]
        public async Task<IActionResult> TestDecryption()
        {
            string original = "Secret Message";

            // Encrypt first
            var encrypted = _IrsaService.EncryptData(original);

            // Then decrypt
            var decrypted = _IrsaService.DecryptData(encrypted);

            if (decrypted.Equals(original))
            {
                return Ok("Successfully");
            }
            return BadRequest("Not Good");
        }
    }
}
