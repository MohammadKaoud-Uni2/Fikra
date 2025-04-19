using AutoMapper;
using Fikra.Service.Implementation;
using Fikra.Service.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OpenAI;
using Org.BouncyCastle.Crypto.Digests;
using SparkLink.Models.Identity;

namespace Fikra.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IdeaController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IIdeaService _IdeaService;
        private readonly IMapper _mapper;
     
        public IdeaController(UserManager<ApplicationUser>userManager,IIdeaService ideaService,IMapper mapper)
        {
            _userManager = userManager;
           _IdeaService = ideaService;
           _mapper = mapper;

        }
        //[HttpPost]
        //public async Task<IActionResult>CreateIdea([FromBody]CreateIdeaDto createIdeaDto)
        //{
        //    if (createIdeaDto == null)
        //    {
        //        return BadRequest("cannot Add Empty fields for idea");
        //    }
        //    if (ModelState.IsValid)
        //    {
        //        await _IdeaService.AddAsync(createIdeaDto);
        //        await _IdeaService.SaveChangesAsync():

        //        return Ok(createIdeaDto);
        //    }


        //}
      
    }
}
