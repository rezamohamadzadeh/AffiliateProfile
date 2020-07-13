using AutoMapper;
using AffiliateProfile.Models;
using DAL.Models;

namespace AffiliateProfile.Utilitis.AutoMapper
{
    public class AutoMapperProfile : Profile
    {
        //private readonly IHttpContextAccessor _httpContextAccessor;
        public AutoMapperProfile(/*IHttpContextAccessor httpContextAccessor*/)
        {
            //_httpContextAccessor = httpContextAccessor;

            CreateMap<Tb_Sell, SellDto>()
            .ReverseMap();

            CreateMap<ApplicationUser, ProfileDto>()
            .ReverseMap();

            CreateMap<Tb_Support, SupportDto>()
               .ForMember(x => x.SupportType, opt => opt.MapFrom(src => src.SupportType.SupportTypeName))
               .ReverseMap()
               //.ForMember(x => x.SenderUserId, opt => opt.MapFrom(UserExtention.GetUserId(_httpContextAccessor.HttpContext.User)))
               //.ForMember(x => x.AnswerUserId, opt => opt.MapFrom(UserExtention.GetUserId(_httpContextAccessor.HttpContext.User)))
               .ForMember(x => x.SupportType, opt => opt.Ignore())
               //.ForMember(x => x.AnswerUser, opt => opt.Ignore())
               //.ForMember(x => x.SenderUser, opt => opt.Ignore())
               .ForMember(x => x.AnswerDateTime, opt => opt.Ignore())
               .ForMember(x => x.SendDateTime, opt => opt.Ignore());

        }
    }
}
