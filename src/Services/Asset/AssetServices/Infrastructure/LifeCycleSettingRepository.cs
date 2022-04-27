using AssetServices.Models;
using Common.Logging;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetServices.Infrastructure
{
    public class LifeCycleSettingRepository
    {
        private readonly AssetsContext _assetContext;
        private readonly IFunctionalEventLogService _functionalEventLogService;
        private readonly IMediator _mediator;

        public LifeCycleSettingRepository(AssetsContext assetContext, IFunctionalEventLogService functionalEventLogService, IMediator mediator)
        {
            _assetContext = assetContext;
            _functionalEventLogService = functionalEventLogService;
            _mediator = mediator;
        }
        
    }
}
