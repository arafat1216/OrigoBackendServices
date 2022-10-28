using System.Net;
using System.Text.Json;
using AutoMapper;
using Common.Extensions;
using Common.Interfaces;
using Common.Model.EventModels.DatasyncModels;
using Customer.API.ViewModels;
using Customer.API.WriteModels;
using CustomerServices;
using CustomerServices.Exceptions;
using Dapr;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

#nullable enable

namespace Customer.API.Controllers;

/// <summary>
/// Customer Data Sync Employe endpoints
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/employee-datasync/organizations/{customerId:Guid}/departments")]
[Tags("Customer Data Sync API: Departments")]
[SwaggerResponse(StatusCodes.Status500InternalServerError, "Returned when the system encountered an unexpected problem.")]
public class EmployeeDatasyncDepartmentsController : ControllerBase
{
    private readonly ILogger<EmployeeDatasyncDepartmentsController> _logger;
    private readonly IUserServices _userServices;
    private readonly IDepartmentsServices _departmentServices;
    private readonly IMapper _mapper;

    /// <summary>
    /// The controller needs access to the logger service, the user service.
    /// </summary>
    /// <param name="logger"> The injected <see cref="ILogger"/> instance. </param>
    /// <param name="userServices"> The injected <see cref="IUserServices"/> instance. </param>
    /// <param name="departmentServices"> The injected <see cref="IDepartmentsServices"/> instance. </param>
    /// <param name="mapper"> The injected <see cref="IMapper"/> (automapper) instance. </param>
    public EmployeeDatasyncDepartmentsController(ILogger<EmployeeDatasyncDepartmentsController> logger, IUserServices userServices, IDepartmentsServices departmentServices, IMapper mapper)
    {
        _logger = logger;
        _userServices = userServices;
        _departmentServices = departmentServices;
        _mapper = mapper;
    }

    /// <summary>
    /// Get information of a single Department based on departmentId
    /// </summary>
    /// <param name="customerId"></param>
    /// <param name="departmentId"></param>
    /// <returns></returns>
    [HttpGet("{departmentId:Guid}")]
    [ProducesResponseType(typeof(Department), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<Department>> GetDepartment([FromRoute] Guid customerId, [FromRoute] Guid departmentId)
    {
        var departmentEntity = await _departmentServices.GetDepartmentAsync(customerId, departmentId);
        if (departmentEntity == null)
            return NotFound();

        var departmentView = _mapper.Map<Department>(departmentEntity);

        return Ok(departmentView);
    }


    /// <summary>
    ///     Returns a paginated department-list.
    /// </summary>
    /// <remarks>
    ///     Retrieves a pagination-set that contains the departments for the requested organization.
    /// </remarks>
    /// <param name="customerId"> The organization you are retrieving departments from. </param>
    /// <param name="cancellationToken"> A dependency-injected <see cref="CancellationToken"/>. </param>
    /// <param name="includeManagers"> When <c><see langword="true"/></c>, the <c>ManagedBy</c> property is
    /// loaded/included in the retrieved data. 
    /// 
    /// This property will not be included unless it's explicitly requested. </param>
    /// <param name="page"> The current page number. </param>
    /// <param name="limit"> The highest number of items that can be added in a single page. </param>
    /// <returns> An asynchronous task. The task results contain the appropriate <see cref="ActionResult"/>. </returns>
    [HttpGet("paginated")]
    [ProducesResponseType(typeof(PagedModel<Department>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<PagedModel<Department>>> GetPaginatedDepartmentsAsync([FromRoute] Guid customerId, CancellationToken cancellationToken, [FromQuery] bool includeManagers = false, int page = 1, int limit = 25)
    {
        var departmentList = await _departmentServices.GetPaginatedDepartmentsAsync(customerId, includeManagers, cancellationToken, page, limit);

        PagedModel<Department> remapped = new()
        {
            Items = _mapper.Map<IList<Department>>(departmentList.Items),
            CurrentPage = departmentList.CurrentPage,
            PageSize = departmentList.PageSize,
            TotalItems = departmentList.TotalItems,
            TotalPages = departmentList.TotalPages
        };

        return Ok(remapped);
    }


    /// <summary>
    /// Get a list of Users under a particular department
    /// </summary>
    /// <param name="customerId"></param>
    /// <param name="departmentId"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="page"></param>
    /// <param name="limit"></param>
    /// <returns></returns>
    [HttpGet("{departmentId:Guid}/users")]
    [ProducesResponseType(typeof(User), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<User>> GetUsersByDepartment([FromRoute] Guid customerId, [FromRoute] Guid departmentId, 
        CancellationToken cancellationToken, 
        [FromQuery] int page = 1,
        [FromQuery] int limit = 25)
    {
        var users = await _userServices.GetAllUsersAsync(customerId,
            null,
            new []{ departmentId },
            null,
            cancellationToken,
            null,
            page,
            limit);

        var response = new PagedModel<User>()
        {
            Items = _mapper.Map<IList<User>>(users.Items),
            CurrentPage = users.CurrentPage,
            PageSize = users.PageSize,
            TotalItems = users.TotalItems,
            TotalPages = users.TotalPages
        };
        return Ok(response);
    }


    /// <summary>
    /// Create a new Department under an Organization
    /// </summary>
    /// <param name="customerId"></param>
    /// <param name="department"></param>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(typeof(Department), (int)HttpStatusCode.Created)]
    public async Task<ActionResult<Department>> CreateDepartment([FromRoute] Guid customerId, [FromBody] NewDepartment department)
    {
        Guid newDepartmentId = Guid.NewGuid();
        var createdDepartment = await _departmentServices.AddDepartmentAsync(customerId, newDepartmentId, department.ParentDepartmentId, department.Name, department.CostCenterId, department.Description, department.ManagedBy, department.CallerId);
        var departmentView = _mapper.Map<Department>(createdDepartment);

        return CreatedAtAction(nameof(CreateDepartment), new { id = departmentView.DepartmentId }, departmentView);
    }


    /// <summary>
    /// Update Department information.
    /// </summary>
    /// <param name="customerId"></param>
    /// <param name="departmentId"></param>
    /// <param name="department"></param>
    /// <returns></returns>
    [HttpPut("{departmentId:Guid}")]
    [ProducesResponseType(typeof(Department), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<Department>> UpdateDepartmentPut([FromRoute] Guid customerId, [FromRoute] Guid departmentId, [FromBody] UpdateDepartment department)
    {
        var updatedDepartment = await _departmentServices.UpdateDepartmentAsync(customerId, departmentId, department.ParentDepartmentId, department.Name, department.CostCenterId, department.Description, department.ManagedBy, department.CallerId);
        var departmentView = _mapper.Map<Department>(updatedDepartment);

        return Ok(departmentView);
    }


    /// <summary>
    /// Delete a Department
    /// </summary>
    /// <param name="customerId"></param>
    /// <param name="departmentId"></param>
    /// <param name="callerId"></param>
    /// <returns></returns>
    [HttpDelete("{departmentId:Guid}")]
    [ProducesResponseType(typeof(Department), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult<Department>> DeleteDepartment([FromRoute] Guid customerId, [FromRoute] Guid departmentId, [FromBody] Guid callerId)
    {
        try
        {
            var updatedDepartment = await _departmentServices.DeleteDepartmentAsync(customerId, departmentId, callerId);
            var departmentView = _mapper.Map<Department>(updatedDepartment);

            return Ok(departmentView);

        }
        catch (DbUpdateException ex)
        {
            return BadRequest("Could not be deleted. Move all users before deleting the department.");
        }
    }


    /// <summary>
    /// Assign User to a Department
    /// </summary>
    /// <param name="customerId"></param>
    /// <param name="userId"></param>
    /// <param name="departmentId"></param>
    /// <param name="callerId"></param>
    /// <returns></returns>
    [HttpPost("{departmentId:Guid}/users/{userId:Guid}")]
    [Topic("customer-datasync-pub-sub", "employee-assign-department")]
    [ProducesResponseType(typeof(User), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<User>> AssignDepartment([FromRoute] Guid customerId, [FromRoute] Guid userId, [FromRoute] Guid departmentId, [FromBody] Guid callerId)
    {
        var user = await _userServices.AssignDepartment(customerId, userId, departmentId, callerId);
        if (user == null) return NotFound();
        return Ok(_mapper.Map<User>(user));
    }


    /// <summary>
    /// Remove a User from a Department
    /// </summary>
    /// <param name="customerId"></param>
    /// <param name="userId"></param>
    /// <param name="departmentId"></param>
    /// <param name="callerId"></param>
    /// <returns></returns>
    [HttpDelete("{departmentId:Guid}/users/{userId:Guid}")]
    [Topic("customer-datasync-pub-sub", "unassign-department")]
    [ProducesResponseType(typeof(User), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<User>> UnnassignDepartment([FromRoute] Guid customerId, [FromRoute] Guid userId, [FromRoute] Guid departmentId, [FromBody] Guid callerId)
    {
        var user = await _userServices.UnassignDepartment(customerId, userId, departmentId, callerId);
        if (user == null) return NotFound();
        return Ok(_mapper.Map<User>(user));
    }


    /// <summary>
    /// Assign Manager to a Department
    /// </summary>
    /// <param name="customerId"></param>
    /// <param name="userId"></param>
    /// <param name="departmentId"></param>
    /// <param name="callerId"></param>
    /// <returns></returns>
    [HttpPost("{departmentId:Guid}/users/{userId:Guid}/manager")]
    [Topic("customer-datasync-pub-sub", "assign-department-manager")]
    [ProducesResponseType(typeof(User), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult> AssignManagerToDepartment([FromRoute] Guid customerId, [FromRoute] Guid userId, [FromRoute] Guid departmentId, [FromBody] Guid callerId)
    {
        try
        {
            await _userServices.AssignManagerToDepartment(customerId, userId, departmentId, callerId);
            return Ok();
        }
        catch (DepartmentNotFoundException exception)
        {

            return BadRequest(exception.Message);
        }
        catch (UserNotFoundException exception)
        {

            return BadRequest(exception.Message);
        }
        catch (Exception)
        {
            return BadRequest();
        }
    }


    /// <summary>
    /// Remove a Manager from a Department
    /// </summary>
    /// <param name="customerId"></param>
    /// <param name="userId"></param>
    /// <param name="departmentId"></param>
    /// <param name="callerId"></param>
    /// <returns></returns>
    [HttpDelete("{departmentId:Guid}/users/{userId:Guid}/manager")]
    [Topic("customer-datasync-pub-sub", "unnassign-department-manager")]
    [ProducesResponseType(typeof(User), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult> UnassignManagerFromDepartment([FromRoute] Guid customerId, [FromRoute] Guid userId, [FromRoute] Guid departmentId, [FromBody] Guid callerId)
    {
        try
        {
            await _userServices.UnassignManagerFromDepartment(customerId, userId, departmentId, callerId);
            return Ok();
        }
        catch (DepartmentNotFoundException exception)
        {

            return BadRequest(exception.Message);
        }
        catch (UserNotFoundException exception)
        {

            return BadRequest(exception.Message);
        }
        catch (Exception)
        {
            return BadRequest();
        }
    }
}