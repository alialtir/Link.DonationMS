using AutoMapper;
using Domain.Models;
using DTOs.CampaignDTOs;
using DTOs.CategoryDTOs;
using DTOs.DonationDTOs;
using DTOs.EmailNotificationDTOs;
using DTOs.ReceiptDTOs;
using DTOs.UserDTOs;
using DTOs.PaymentDTOs;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class MappingProfiles : Profile
    {

        public MappingProfiles()
        {

            CreateMap<Campaign, CampaignResultDto>()
                .ForMember(dest => dest.CategoryTitleAr, opt => opt.MapFrom(src => src.Category.TitleAr))
                .ForMember(dest => dest.CategoryTitleEn, opt => opt.MapFrom(src => src.Category.TitleEn))
                .ForMember(dest => dest.ImageData, opt => opt.MapFrom(src => src.ImageData))
                .ForMember(dest => dest.ImageExtension, opt => opt.MapFrom(src => src.ImageExtension))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));


            CreateMap<CreateCampaignDto, Campaign>();
            CreateMap<UpdateCampaignDto, Campaign>();

            CreateMap<Category, CategoryDto>();
            CreateMap<CreateCategoryDto, Category>();
            CreateMap<UpdateCategoryDto, Category>();

            CreateMap<Donation, DonationResultDto>()
                .ForMember(dest => dest.CampaignTitle, opt => opt.MapFrom(src => src.Campaign.TitleEn))
                .ForMember(dest => dest.DonorName, opt => opt.MapFrom(src => src.User != null ? src.User.DisplayName : "Anonymous"))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

            CreateMap<CreateDonationDto, Donation>();
            CreateMap<UpdateDonationStatusDto, Donation>();

            CreateMap<EmailNotification, EmailNotificationDto>();
            CreateMap<CreateEmailNotificationDto, EmailNotification>();

            CreateMap<Receipt, ReceiptDto>();
            CreateMap<CreateReceiptDto, Receipt>();

            CreateMap<User, UserDto>();
            CreateMap<RegisterUserDto, User>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email));
            CreateMap<UserDto, User>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email));

            CreateMap<PaymentIntent, PaymentIntentDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ClientSecret, opt => opt.MapFrom(src => src.ClientSecret));
        }

    }
}
