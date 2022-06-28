using System;
using Dapr;
using Microsoft.AspNetCore.Mvc;
using AssetServices;
using Common.Extensions;
using Common.Model.EventModels;

namespace Asset.API.Controllers;

[Route("api/v{version:apiVersion}/assets")]
public class UserController : Controller
{
    private IAssetServices _assetServices;

    public UserController(IAssetServices assetServices)
    {
        _assetServices = assetServices;
    }

    [Topic("customer-pub-sub", "user-deleted")]
    [HttpPost("user-deleted")]
    public void UserDeleted([FromBody] UserDeletedEvent userDeletedEvent)
    {
        _assetServices.UnAssignAssetLifecyclesForUserAsync(userDeletedEvent.CustomerId, userDeletedEvent.UserId, userDeletedEvent.DepartmentId, Guid.Empty.SystemUserId());
    }


    [Topic("customer-pub-sub", "user-change-department-assignment")]
    [HttpPost("user-change-department-assignment")]
    public void UserAssignDepartment([FromBody] UserChangedDepartmentEvent userDeletedEvent)
    {
        _assetServices.SyncDepartmentForUserToAssetLifecycle(userDeletedEvent.CustomerId, userDeletedEvent.UserId, userDeletedEvent.DepartmentId, Guid.Empty.SystemUserId());
    }
}