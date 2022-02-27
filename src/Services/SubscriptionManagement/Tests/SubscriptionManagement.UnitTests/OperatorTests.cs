using System;
using Microsoft.EntityFrameworkCore;
using SubscriptionManagementServices;
using SubscriptionManagementServices.Infrastructure;
using SubscriptionManagementServices.Models;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AutoMapper;
using SubscriptionManagementServices.Exceptions;
using Xunit;
using Moq;
using Common.Logging;
using MediatR;

namespace SubscriptionManagement.UnitTests
{
    public class OperatorTests : SubscriptionManagementServiceBaseTests
    {
        private readonly SubscriptionManagementContext _subscriptionManagementContext;
        private readonly IOperatorService _operatorService;
        private readonly IMapper? _mapper;

        public OperatorTests() : base(
                new DbContextOptionsBuilder<SubscriptionManagementContext>()
                    // ReSharper disable once StringLiteralTypo
                    .UseSqlite("Data Source=sqliteoperatorunittests.db")
                    .Options
            )
        {
            if (_mapper == null)
            {
                var mappingConfig = new MapperConfiguration(mc =>
                {
                    mc.AddMaps(Assembly.GetAssembly(typeof(SubscriptionProduct)));
                });
                _mapper = mappingConfig.CreateMapper();
            }
            _subscriptionManagementContext = new SubscriptionManagementContext(ContextOptions);
            var operatorRepository = new OperatorRepository(_subscriptionManagementContext);
            _operatorService = new OperatorService(operatorRepository, _mapper);
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async Task GetAllOperator()
        {
            var operators = await _operatorService.GetAllOperatorsAsync();
            Assert.NotNull(operators);
            Assert.Equal(7, operators.Count);
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async Task GetOperator()
        {
            var operator1 = await _operatorService.GetOperatorAsync(1);
            Assert.NotNull(operator1);

            var operator2 = await _operatorService.GetOperatorAsync(100);
            Assert.Null(operator2);
        }
    }
}
