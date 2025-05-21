using AutoMapper;
using Fikra.Controllers;
using Fikra.Models;
using Fikra.Models.Dto;
using Fikra.Service.Interface;
using SparkLink.Models.Identity;

namespace Fikra.Mapper
{
    public class ContractProfile:Profile
    {
     public    ContractProfile()
        {
            CreateMap<Contract, GetContractDto>().ForMember(x => x.InvestorName, opt => opt.MapFrom(x => x.Investor.UserName)).ForMember(x => x.IdeaOwnerName, opt => opt.MapFrom(x => x.IdeaOwner.UserName));
            CreateMap<Request, GetRequestDto>();
            CreateMap<AddIdeaDto, Idea>();
            CreateMap<ApplicationUser,GetProfileDto>();
            CreateMap<Idea,GetIdeaDto>();
            CreateMap<SkillLevelDto, SkillLevel>();
            CreateMap<JoinRequest, ReceiveJobRequest>();
            CreateMap<ChatGroup, getChatGroupDto>()
        .ForMember(dest => dest.LastMessage, opt => opt.MapFrom(src =>
             src.Messages
             .OrderByDescending(m => m.SentAt)
             .Select(m => m.message)
             .FirstOrDefault()
     ));
            CreateMap<CV,GetCVDto>().ForMember(x=>x.FreelancerUserName,opt=>opt.MapFrom(x=>x.ApplicationUser.UserName));
            CreateMap<Idea, GetFreelancerIdeasDto>();
            CreateMap<Complaint,ReviewComplaintRequest>();

        }

    }
}
