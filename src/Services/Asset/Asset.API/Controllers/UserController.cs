using System;
using System.Threading.Tasks;
using Dapr;
using Microsoft.AspNetCore.Mvc;
using AssetServices;
using Common.Extensions;
using Common.Model.EventModels;

namespace Asset.API.Controllers;

[ApiController]
[Route("/")]
public class UserController : ControllerBase
{
    private readonly IAssetServices _assetServices;

    public UserController(IAssetServices assetServices)
    {
        _assetServices = assetServices;
    }

    [Topic("customer-pub-sub", "user-deleted")]
    [HttpPost("user-deleted")]
    public async Task UserDeleted([FromBody] UserEvent userEvent)
    {
        await _assetServices.UnAssignAssetLifecyclesForUserAsync(userEvent.CustomerId, userEvent.UserId, userEvent.DepartmentId, Guid.Empty.SystemUserId());
    }


    [Topic("customer-pub-sub", "user-assign-department")]
    [HttpPost("user-assign-department")]
    public async Task UserAssignDepartment([FromBody] UserChangedDepartmentEvent userDeletedEvent)
    {
        await _assetServices.SyncDepartmentForUserToAssetLifecycleAsync(userDeletedEvent.CustomerId, userDeletedEvent.UserId, userDeletedEvent.DepartmentId, Guid.Empty.SystemUserId());
    }
}