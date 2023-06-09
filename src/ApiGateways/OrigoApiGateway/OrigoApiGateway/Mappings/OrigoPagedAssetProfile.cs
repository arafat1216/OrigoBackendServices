﻿using AutoMapper;
using OrigoApiGateway.Models;
using OrigoApiGateway.Models.BackendDTO;

namespace OrigoApiGateway.Mappings
{
    public class OrigoPagedAssetProfile : Profile
    {
        public OrigoPagedAssetProfile()
        {
            CreateMap<PagedAssetsDTO, OrigoPagedAssets>()
                     .ForMember(dest => dest.Items, opts => opts.MapFrom(src => src.Items));
        }
    }
}
