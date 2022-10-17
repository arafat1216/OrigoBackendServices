using AutoMapper;
using Common.Enums;
using Microsoft.AspNetCore.Authorization;
using Common.Interfaces;
using Microsoft.AspNetCore.Mvc;
using OrigoApiGateway.Authorization;
using OrigoApiGateway.Models;
using OrigoApiGateway.Models.BackendDTO;
using OrigoApiGateway.Services;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Security.Claims;

namespace OrigoApiGateway.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Authorize]
    [Route("origoapi/v{version:apiVersion}/Customers/{organizationId:guid}/[controller]")]
    [SuppressMessage("ReSharper", "RouteTemplates.RouteParameterConstraintNotResolved")]
    [SuppressMessage("ReSharper", "RouteTemplates.ControllerRouteParameterIsNotPassedToMethods")]
    public class DepartmentsController : ControllerBase
    {
        private readonly ILogger<DepartmentsController> _logger;
        private readonly IDepartmentsServices _departmentServices;
        private readonly IMapper _mapper;

        public DepartmentsController(ILogger<DepartmentsController> logger, IDepartmentsServices departmentServices, IMapper mapper)
        {
            _logger = logger;
            _departmentServices = departmentServices;
            _mapper = mapper;
        }

        [Route("{departmentId:Guid}")]
        [HttpGet]
        [ProducesResponseType(typeof(OrigoDepartment), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [PermissionAuthorize(PermissionOperator.And,Permission.CanReadCustomer,Permission.DepartmentStructure)]
        public async Task<ActionResult<OrigoDepartment>> GetDepartment(Guid organizationId, Guid departmentId)
        {
            try
            {
                // If role is not System admin, check access list
                var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                if (role != PredefinedRole.SystemAdmin.ToString())
                {
                    // All roles have access to Department, as long as they have access to this customer/organization.
                    var accessList = HttpContext.User.Claims.Where(c => c.Type == "AccessList").Select(y => y.Value).ToList();
                    if (accessList == null || !accessList.Any() || !accessList.Contains(organizationId.ToString()))
                    {
                        return Forbid();
                    }
                }

                var department = await _departmentServices.GetDepartmentAsync(organizationId, departmentId);

                return Ok(department);
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpGet]
        [ProducesResponseType(typeof(IList<OrigoDepartment>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [PermissionAuthorize(PermissionOperator.And, Permission.CanReadCustomer, Permission.DepartmentStructure)]
        public async Task<ActionResult<OrigoDepartment>> GetDepartments(Guid organizationId)
        {
            try
            {
                // If role is not System admin, check access list
                var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                if (role != PredefinedRole.SystemAdmin.ToString())
                {
                    // All roles have access to an organizations departments, as long as the organization is in the caller accesslist
                    var accessList = HttpContext.User.Claims.Where(c => c.Type == "AccessList").Select(y => y.Value).ToList();
                    if (accessList == null || !accessList.Any() || !accessList.Contains(organizationId.ToString()))
                    {
                        return Forbid();
                    }
                }

                var departments = await _departmentServices.GetDepartmentsAsync(organizationId);

                return Ok(departments);
            }
            catch
            {
                return BadRequest();
            }
        }

        /// <summary>
        ///     Returns a paginated department-list.
        /// </summary>
        /// <remarks>
        ///     Retrieves a pagination-set that contains the departments for the requested organization.
        /// </remarks>
        /// <param name="organizationId"> The organization you are retrieving departments from. </param>
        /// <param name="cancellationToken"> A dependency-injected <see cref="CancellationToken"/>. </param>
        /// <param name="includeManagers"> When <c><see langword="true"/></c>, the <c>ManagedBy</c> property is
        /// loaded/included in the retrieved data.   
        /// 
        /// This property will not be included unless it's explicitly requested. </param>
        /// <param name="page"> The current page number. </param>
        /// <param name="limit"> The number of items to retrieve for each <paramref name="page"/>. </param>
        /// <returns> An asynchronous task. The task results contain the appropriate <see cref="ActionResult"/>. </returns>
        [HttpGet("paginated")]
        [ProducesResponseType(typeof(PagedModel<OrigoDepartment>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [PermissionAuthorize(PermissionOperator.And, Permission.CanReadCustomer, Permission.DepartmentStructure)]

        public async Task<ActionResult<PagedModel<OrigoDepartment>>> GetPaginatedDepartmentsAsync([FromRoute] Guid organizationId, CancellationToken cancellationToken, [FromQuery] bool includeManagers = false, [FromQuery] int page = 1, [FromQuery][Range(1, 100)] int limit = 25)
        {
            try
            {
                // If role is not System admin, check access list
                var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                if (role != PredefinedRole.SystemAdmin.ToString())
                {
                    // All roles have access to an organizations departments, as long as the organization is in the caller's access-list
                    var accessList = HttpContext.User.Claims.Where(c => c.Type == "AccessList").Select(y => y.Value).ToList();
                    if (accessList == null || !accessList.Any() || !accessList.Contains(organizationId.ToString()))
                    {
                        return Forbid();
                    }
                }

                var departments = await _departmentServices.GetPaginatedDepartmentsAsync(organizationId, cancellationToken, includeManagers, page, limit);

                return Ok(departments);
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(OrigoDepartment), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [PermissionAuthorize(PermissionOperator.And, Permission.CanReadCustomer, Permission.CanUpdateCustomer, Permission.DepartmentStructure)]
        public async Task<ActionResult<OrigoDepartment>> CreateDepartmentForCustomer(Guid organizationId, [FromBody] NewDepartment newDepartment)
        {
            try
            {
                // Only admin roles are allowed to create departments
                var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                if (role == PredefinedRole.EndUser.ToString() || role == PredefinedRole.DepartmentManager.ToString() || role == PredefinedRole.Manager.ToString())
                {
                    return Forbid();
                }

                // Partner Admin, Group Admin and Customer Admin have access if organization is in their access list
                if (role != PredefinedRole.SystemAdmin.ToString())
                {

                    var accessList = HttpContext.User.Claims.Where(c => c.Type == "AccessList").Select(y => y.Value).ToList();
                    if (accessList == null || !accessList.Any() || !accessList.Contains(organizationId.ToString()))
                    {
                        return Forbid();
                    }
                }

                //Mapping the frontend model to backend dto and assigning a caller id
                var newDepartmentDTO = _mapper.Map<NewDepartmentDTO>(newDepartment);

                var actor = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Actor)?.Value;
                Guid callerId;
                Guid.TryParse(actor, out callerId);
                newDepartmentDTO.CallerId = callerId;

                var createdDepartment = await _departmentServices.AddDepartmentAsync(organizationId, newDepartmentDTO);

                return CreatedAtAction(nameof(CreateDepartmentForCustomer), new { id = createdDepartment.DepartmentId }, createdDepartment);
            }
            catch
            {
                return BadRequest();
            }
        }

        [Route("{departmentId:Guid}")]
        [HttpPut]
        [ProducesResponseType(typeof(OrigoDepartment), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [PermissionAuthorize(PermissionOperator.And, Permission.CanReadCustomer, Permission.CanUpdateCustomer, Permission.DepartmentStructure)]
        public async Task<ActionResult<OrigoDepartment>> PutDepartmentForCustomer(Guid organizationId, Guid departmentId, [FromBody] UpdateDepartment updateDepartment)
        {
            try
            {
                // Only admin roles are allowed to create departments
                var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                if (role == PredefinedRole.EndUser.ToString() || role == PredefinedRole.DepartmentManager.ToString() || role == PredefinedRole.Manager.ToString())
                {
                    return Forbid();
                }

                // Partner Admin, Group Admin and Customer Admin have access if organization is in their access list
                if (role != PredefinedRole.SystemAdmin.ToString())
                {
                    var accessList = HttpContext.User.Claims.Where(c => c.Type == "AccessList").Select(y => y.Value).ToList();
                    if (accessList == null || !accessList.Any() || !accessList.Contains(organizationId.ToString()))
                    {
                        return Forbid();
                    }
                }

                //Mapping the frontend model to backend dto and assigning a caller id
                var updateDepartmentDTO = _mapper.Map<UpdateDepartmentDTO>(updateDepartment);

                var actor = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Actor)?.Value;
                Guid callerId;
                Guid.TryParse(actor, out callerId);
                updateDepartmentDTO.CallerId = callerId;

                var updatedDepartment = await _departmentServices.UpdateDepartmentPutAsync(organizationId, departmentId, updateDepartmentDTO);

                return Ok(updatedDepartment);
            }
            catch
            {
                return BadRequest();
            }
        }

        [Route("{departmentId:Guid}")]
        [HttpPatch]
        [ProducesResponseType(typeof(OrigoDepartment), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [PermissionAuthorize(PermissionOperator.And, Permission.CanReadCustomer, Permission.CanUpdateCustomer, Permission.DepartmentStructure)]
        public async Task<ActionResult<OrigoDepartment>> PatchDepartmentForCustomer(Guid organizationId, Guid departmentId, [FromBody] UpdateDepartment updateDepartment)
        {
            try
            {
                // Only admin roles are allowed to create departments
                var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                if (role == PredefinedRole.EndUser.ToString() || role == PredefinedRole.DepartmentManager.ToString() || role == PredefinedRole.Manager.ToString())
                {
                    return Forbid();
                }

                // Partner Admin, Group Admin and Customer Admin/Admin have access if organization is in their access list
                if (role != PredefinedRole.SystemAdmin.ToString())
                {
                    var accessList = HttpContext.User.Claims.Where(c => c.Type == "AccessList").Select(y => y.Value).ToList();
                    if (accessList == null || !accessList.Any() || !accessList.Contains(organizationId.ToString()))
                    {
                        return Forbid();
                    }
                }

                //Mapping the frontend model to backend dto and assigning a caller id
                var updateDepartmentDTO = _mapper.Map<UpdateDepartmentDTO>(updateDepartment);

                var actor = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Actor)?.Value;
                Guid callerId;
                Guid.TryParse(actor, out callerId);
                updateDepartmentDTO.CallerId = callerId;

                var updatedDepartment = await _departmentServices.UpdateDepartmentPatchAsync(organizationId, departmentId, updateDepartmentDTO);

                return Ok(updatedDepartment);
            }
            catch
            {
                return BadRequest();
            }
        }

        [Route("{departmentId:Guid}")]
        [HttpDelete]
        [ProducesResponseType(typeof(OrigoDepartment), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [PermissionAuthorize(PermissionOperator.And, Permission.CanReadCustomer, Permission.CanUpdateCustomer, Permission.DepartmentStructure)]
        public async Task<ActionResult<OrigoDepartment>> DeleteDepartmentForCustomer(Guid organizationId, Guid departmentId)
        {
            try
            {
                // Only admin roles are allowed to delete departments
                var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                if (role == PredefinedRole.EndUser.ToString() || role == PredefinedRole.DepartmentManager.ToString() || role == PredefinedRole.Manager.ToString())
                {
                    return Forbid();
                }

                // Partner Admin, Group Admin and Customer Admin have access if organization is in their access list
                if (role != PredefinedRole.SystemAdmin.ToString())
                {
                    var accessList = HttpContext.User.Claims.Where(c => c.Type == "AccessList").Select(y => y.Value).ToList();
                    if (accessList == null || !accessList.Any() || !accessList.Contains(organizationId.ToString()))
                    {
                        return Forbid();
                    }
                }
                var actor = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Actor)?.Value;
                Guid callerId;
                Guid.TryParse(actor, out callerId);

                var updatedDepartment = await _departmentServices.DeleteDepartmentPatchAsync(organizationId, departmentId, callerId);

                return Ok(updatedDepartment);
            }
            catch (BadHttpRequestException ex)
            {
                return BadRequest(ex.Message);
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}
