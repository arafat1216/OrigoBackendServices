using AutoMapper;
using HardwareServiceOrderServices.Mappings;
using HardwareServiceOrderServices.Models;
using HardwareServiceOrderServices.ServiceModels;
using System;
using System.Collections.Generic;
using Xunit;

namespace HardwareServiceOrder.UnitTests
{
    public class AssetInfoProfileTests
    {
        private readonly IMapper _mapper;

        public AssetInfoProfileTests()
        {
            _mapper = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new AssetInfoProfile());
            }).CreateMapper();
        }

        [Fact]
        public void MapFrom_AssetInfo_To_AssetInfoDTO_ShouldMap_SingleImei()
        {
#pragma warning disable CS0618 // Type or member is obsolete
            var assetInfo = new AssetInfo
            {
                Imei = new HashSet<string> { "500119468586675", "123456789012345", "123456789012346" }
            };
#pragma warning restore CS0618 // Type or member is obsolete

            var dto = _mapper.Map<AssetInfoDTO>(assetInfo);

            Assert.NotNull(dto);
            Assert.NotNull(dto.Imei);
            Assert.Equal("500119468586675", dto.Imei);
        }
    }
}
