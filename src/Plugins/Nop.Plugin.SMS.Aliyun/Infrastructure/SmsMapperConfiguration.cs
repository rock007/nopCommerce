using AutoMapper;
using Nop.Core.Infrastructure.Mapper;
using Nop.Plugin.SMS.Aliyun.Domain;
using Nop.Plugin.SMS.Aliyun.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.SMS.Aliyun.Infrastructure
{
    public class SmsMapperConfiguration : Profile, IOrderedMapperProfile
    {
        public SmsMapperConfiguration()
        {
            CreateMap<SmsTemplate, SmsTemplateModel>()
                .ForMember(dest => dest.Locales, mo => mo.Ignore())
                .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
            CreateMap<SmsTemplateModel, SmsTemplate>()
                .ForMember(dest => dest.LimitedToStores, mo => mo.Ignore())
                .ForMember(dest => dest.DelayPeriod, mo => mo.Ignore());

            CreateMap<QueuedSms, QueuedSmsModel>()
                .ForMember(dest => dest.AvailableCountries, mo => mo.Ignore())
                .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
            CreateMap<QueuedSmsModel, QueuedSms>()
                .ForMember(dest => dest.CreatedOnUtc, mo => mo.Ignore())
                .ForMember(dest => dest.PriorityId, mo => mo.Ignore());
        }

        public int Order => 1;
    }
}
