using AutoMapper;
using Fikra.Models;
using Fikra.Models.Dto;

namespace Fikra.Mapper
{
    public class ContractProfile:Profile
    {
     public    ContractProfile()
        {
            CreateMap<Contract, GetContractDto>().ForMember(x => x.InvestorName, opt => opt.MapFrom(x => x.Investor.UserName)).ForMember(x => x.IdeaOwnerName, opt => opt.MapFrom(x => x.IdeaOwner.UserName));


        }

    }
}
