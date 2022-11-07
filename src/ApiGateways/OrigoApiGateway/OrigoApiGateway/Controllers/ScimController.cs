using AutoMapper;
using Common.Enums;
using Common.Extensions;
using Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrigoApiGateway.Authorization;
using OrigoApiGateway.Exceptions;
using OrigoApiGateway.Filters;
using OrigoApiGateway.Models;
using OrigoApiGateway.Models.BackendDTO;
using OrigoApiGateway.Models.SCIM;
using OrigoApiGateway.Services;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Security.Claims;

namespace OrigoApiGateway.Controllers;

[ServiceFilter(typeof(ErrorExceptionFilter))]
[ApiController]
[ApiVersion("1.0")]
[Authorize(Roles = "SystemAdmin")]
[ProducesResponseType(typeof(ScimException), (int)HttpStatusCode.InternalServerError)]
[Route("/origoapi/v{version:apiVersion}/[controller]/v2")]
public class ScimController : ControllerBase
{
    private readonly ILogger<ScimController> _logger;
    private readonly IUserServices _userServices;
    private readonly ICustomerServices _customerServices;
    private readonly IMapper _mapper;
    private readonly IProductCatalogServices _productCatalogServices;

    public ScimController(ILogger<ScimController> logger,
        IUserServices userServices,
        ICustomerServices customerServices,
        IProductCatalogServices productCatalogServices,
        IMapper mapper)
    {
        _logger = logger;
        _userServices = userServices;
        _customerServices = customerServices;
        _productCatalogServices = productCatalogServices;
        _mapper = mapper;
    }

    [Route("Users/{userId:Guid}")]
    [HttpGet]
    [ProducesResponseType(typeof(ListResponse), (int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [PermissionAuthorize(PermissionOperator.And, Permission.CanReadCustomer, Permission.CanUpdateCustomer)]
    public async Task<ActionResult<ListResponse>> GetUser(Guid? userId)
    {
        throw new NotImplementedException();
    }

    [Route("Users")]
    [HttpPut]
    [ProducesResponseType(typeof(ListResponse), (int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [PermissionAuthorize(PermissionOperator.And, Permission.CanReadCustomer, Permission.CanUpdateCustomer)]
    public async Task<ActionResult<ListResponse>> UpdateUser()
    {
        throw new NotImplementedException();
    }

    [Route("Users")]
    [HttpPost]
    [ProducesResponseType(typeof(ListResponse), (int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [PermissionAuthorize(PermissionOperator.And, Permission.CanReadCustomer, Permission.CanUpdateCustomer)]
    public async Task<ActionResult<ListResponse>> CreateUserForCustomer([FromBody] ScimUser newUser)
    {
        throw new NotImplementedException();
    }

    [Route("Users")]
    [HttpDelete]
    [ProducesResponseType(typeof(ListResponse), (int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [PermissionAuthorize(PermissionOperator.And, Permission.CanReadCustomer, Permission.CanUpdateCustomer)]
    public async Task<ActionResult<ListResponse>> DeleteUser()
    {
        throw new NotImplementedException();
    }
}
