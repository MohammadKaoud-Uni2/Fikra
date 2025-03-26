using AutoMapper;
using Fikra.Models.Dto;
using Fikra.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SparkLink.Models.Identity;

namespace Fikra.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContractController : ControllerBase
    {
        private readonly IContractRepo _contractRepo;
        private UserManager<ApplicationUser> _userManager;
        private IMapper _mapper;
        public ContractController(IContractRepo contractRepo,IMapper mapper)
        {
            _contractRepo = contractRepo;
            _mapper = mapper;
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
            return BadRequest("Problem While Fetching the contract !");

        }
    }
}
