using AutoMapper;
using DatingApp.DomainModels;
using DatingApp.ViewModels;
using System.Linq;

namespace DatingApp.API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<User, UserForListViewModel>()
                .ForMember(dest => dest.PhotoUrl, opt => opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).Url))
                .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.DateOfBirth.CalculateAge()));
            CreateMap<User, UserForDetailViewModel>()
                .ForMember(dest => dest.PhotoUrl, opt => opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).Url))
                 .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.DateOfBirth.CalculateAge()));
            CreateMap<Photo, PhotosForDetailViewModel>();
            CreateMap<UserForUpdateViewModel, User>();
            CreateMap<Photo, PhotoForReturnViewModel>();
            CreateMap<PhotoForCreatingViewModel, Photo>();
            CreateMap<UserForRegisterViewModel, User>();
            CreateMap<MessageForCreationViewModel, Message>();
            CreateMap<Message, MessageForCreationViewModel>();
            CreateMap<Message, MessageToReturnViewModel>().ForMember(dest => dest.SenderPhotoUrl, opt => opt.MapFrom(src => src.Sender.Photos.FirstOrDefault(p => p.IsMain).Url))
                                                           .ForMember(dest => dest.RecipientPhotoUrl, opt => opt.MapFrom(src => src.Recipient.Photos.FirstOrDefault(p => p.IsMain).Url));
        }
    }
}