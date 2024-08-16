using AutoMapper;
using GrpcService.Notify;
using GrpcService.Model;

namespace GrpcService
{
    public class GrpcServiceAutoMapper : Profile
    {
        public GrpcServiceAutoMapper()
        {
            CreateMap<EmailSimpleRequest, EmailRequestSimpleModel>()
                .ForMember(dest => dest.ToAddress, opt => opt.MapFrom(src => src.ToAddress))
                .ForMember(dest => dest.HtmlBody, opt => opt.MapFrom(src => src.HtmlBody))
                .ForMember(dest => dest.Remark, opt => opt.MapFrom(src => src.Remark))
                .ForMember(dest => dest.Subject, opt => opt.MapFrom(src => src.Subject))
                .ReverseMap();

        }
    }
}
