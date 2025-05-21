using AutoMapper;
using Fikra.Models;
using Fikra.Models.Dto;
using Fikra.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SparkLink.Models.Identity;
using SparkLink.Service.Interface;
using System.Formats.Asn1;

namespace Fikra.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CVController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IIdentityServices _identityService;
        private readonly IPdfService _pdfService;
        private readonly ICVService _cvService;
        public CVController(IMapper mapper,UserManager<ApplicationUser>userManager,IIdentityServices identityService,IPdfService pdfService,ICVService cVService)
        {
            _mapper = mapper;
            _userManager = userManager;
            _identityService = identityService;
            _pdfService = pdfService;
            _cvService = cVService;
        }
        [HttpPost]
        [Route("GenerateCVFile")]
        [Authorize(AuthenticationSchemes ="Bearer",Roles = "Freelancer")]
        public async Task<IActionResult> GenerateCvFile([FromBody ]GenerateCVDto generateCVDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Model State is not Valid Check the DTO Fields ");

            }
            var currentUserName=await _identityService.GetCurrentUserName();
          var cvUrl=await   _pdfService.GenerateCV(generateCVDto, currentUserName);
            if (cvUrl != null)
            {
                return Ok(cvUrl);
            }
            return BadRequest("There Was A problem While Generate the CV");
        }
        [HttpGet]
        [Route("GetAllCvs")]
        //[Authorize(AuthenticationSchemes ="Bearer",Roles ="Admin")]
        public async Task<IActionResult> GetAllCvs()
        {
           var cvs=await  _cvService.GetTableAsNoTracking().Include(x=>x.ApplicationUser).ToListAsync();
            if (cvs.Any())
            {
                var cvsAfterMapping=_mapper.Map<List<GetCVDto>>(cvs);
                return Ok(cvsAfterMapping); 
            }

            return Ok(new
            {
                message = "There are no Cvs in Db"
            });

        }
    }
}
