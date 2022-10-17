using AutoMapper;
using Common.Interfaces;
using Customer.API.ViewModels;
using Customer.API.WriteModels;
using CustomerServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using CustomerServices.ServiceModels;

namespace Customer.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/organizations/{customerId:Guid}/[controller]")]
    [SuppressMessage("ReSharper", "RouteTemplates.RouteParameterConstraintNotResolved")]
    [SuppressMessage("ReSharper", "RouteTemplates.ControllerRouteParameterIsNotPassedToMethods")]
    public class DepartmentsController : ControllerBase
    {
        private readonly IDepartmentsServices _departmentServices;
        private readonly ILogger<DepartmentsController> _logger;
        private readonly IMapper _mapper;

        public DepartmentsController(ILogger<DepartmentsController> logger, IDepartmentsServices departmentServices, IMapper mapper)
        {
            _logger = logger;
            _departmentServices = departmentServices;
            _mapper = mapper;
        }

        [Route("{departmentId:Guid}")]
        [HttpGet]
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
        ///     Returns all departments
        /// </summary>
        /// <param name="customerId"> The organization you are retrieving departments from. </param>
        /// <param name="cancellationToken"></param>
        /// <param name="onlyNames">Will only return the id and name of the departments</param>
        /// <returns> An asynchronous task. The task results contain the appropriate <see cref="ActionResult"/>. </returns>
        [HttpGet]
        [ProducesResponseType(typeof(IList<Department>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IList<Department>>> GetDepartments([FromRoute] Guid customerId, CancellationToken cancellationToken, [FromQuery] bool onlyNames = false)
        {
            if (onlyNames)
            {
                return Ok(await _departmentServices.GetAllDepartmentNamesAsync(customerId, cancellationToken));
            }
            var departmentList = await _departmentServices.GetDepartmentsAsync(customerId);
            var departmentView = _mapper.Map<IList<Department>>(departmentList);

            return Ok(departmentView);
        }

        /// <summary>
        ///     Returns all department names including department id.
        /// </summary>
        /// <param name="customerId"> The organization you are retrieving departments from. </param>
        /// <param name="cancellationToken"></param>
        /// <returns> An asynchronous task. The task results contain the appropriate <see cref="ActionResult"/>. </returns>
        [Route("names")]
        [HttpGet]
        [ProducesResponseType(typeof(List<DepartmentNamesDTO>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<List<DepartmentNamesDTO>>> GetDepartmentNames([FromRoute] Guid customerId, CancellationToken cancellationToken)
        {
                return Ok(await _departmentServices.GetAllDepartmentNamesAsync(customerId, cancellationToken));
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

        [HttpPost]
        [ProducesResponseType(typeof(Department), (int)HttpStatusCode.Created)]
        public async Task<ActionResult<Department>> CreateDepartment([FromRoute] Guid customerId, [FromBody] NewDepartment department)
        {
            Guid newDepartmentId = Guid.NewGuid();
            var createdDepartment = await _departmentServices.AddDepartmentAsync(customerId, newDepartmentId, department.ParentDepartmentId, department.Name, department.CostCenterId, department.Description, department.ManagedBy, department.CallerId);
            var departmentView = _mapper.Map<Department>(createdDepartment);

            return CreatedAtAction(nameof(CreateDepartment), new { id = departmentView.DepartmentId }, departmentView);
        }

        [Route("{departmentId:Guid}")]
        [HttpPut]
        [ProducesResponseType(typeof(Department), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<Department>> UpdateDepartmentPut([FromRoute] Guid customerId, [FromRoute] Guid departmentId, [FromBody] UpdateDepartment department)
        {
            var updatedDepartment = await _departmentServices.UpdateDepartmentAsync(customerId, departmentId, department.ParentDepartmentId, department.Name, department.CostCenterId, department.Description, department.ManagedBy, department.CallerId);
            var departmentView = _mapper.Map<Department>(updatedDepartment);

            return Ok(departmentView);
        }

        [Route("{departmentId:Guid}")]
        [HttpPost]
        [ProducesResponseType(typeof(Department), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<Department>> UpdateDepartmentPatch([FromRoute] Guid customerId, [FromRoute] Guid departmentId, [FromBody] UpdateDepartment department)
        {
            var updatedDepartment = await _departmentServices.UpdateDepartmentAsync(customerId, departmentId, department.ParentDepartmentId, department.Name, department.CostCenterId, department.Description, department.ManagedBy, department.CallerId);
            var departmentView = _mapper.Map<Department>(updatedDepartment);

            return Ok(departmentView);
        }

        [Route("{departmentId:Guid}")]
        [HttpDelete]
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
    }
}
