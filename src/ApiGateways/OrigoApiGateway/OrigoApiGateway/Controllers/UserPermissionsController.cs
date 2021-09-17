using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OrigoApiGateway.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrigoApiGateway.Controllers
{
    public class UserPermissionsController : ControllerBase
    {
        private readonly ILogger<UserPermissionsController> _logger;
        private readonly IUserPermissionServices _customerServices;

        public UserPermissionsController(ILogger<UserPermissionsController> logger, IUserPermissionServices customerServices)
        {
            _logger = logger;
            _customerServices = customerServices;
        }
    }
}
