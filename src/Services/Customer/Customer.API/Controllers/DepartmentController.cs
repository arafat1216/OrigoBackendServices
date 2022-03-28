using Customer.API.ViewModels;
using Customer.API.WriteModels;
using CustomerServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

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

        public DepartmentsController(ILogger<DepartmentsController> logger, IDepartmentsServices departmentServices)
        {
            _logger = logger;
            _departmentServices = departmentServices;
        }

        [Route("{departmentId:Guid}")]
        [HttpGet]
        [ProducesResponseType(typeof(IList<Department>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<Department>> GetDepartments(Guid customerId, Guid departmentId)
        {
            var departmentEntity = await _departmentServices.GetDepartmentAsync(customerId, departmentId);
            if (departmentEntity == null)
                return NotFound();

            var department = new Department(departmentEntity);

            return Ok(department);
        }

        [HttpGet]
        [ProducesResponseType(typeof(IList<Department>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<Department>> GetDepartments(Guid customerId)
        {
            var departmentList = await _departmentServices.GetDepartmentsAsync(customerId);
            var departments = departmentList.Select(d => new Department(d)).ToList();

            return Ok(departments);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Department), (int)HttpStatusCode.Created)]
        public async Task<ActionResult<Department>> CreateDepartment(Guid customerId, [FromBody] NewDepartment department)
        {
            Guid newDepartmentId = Guid.NewGuid();
            var createdDepartment = await _departmentServices.AddDepartmentAsync(customerId, newDepartmentId, department.ParentDepartmentId, department.Name, department.CostCenterId, department.Description,department.CallerId);
            var departmentView = new Department(createdDepartment);

            return CreatedAtAction(nameof(CreateDepartment), new { id = departmentView.DepartmentId }, departmentView);
        }

        [Route("{departmentId:Guid}")]
        [HttpPut]
        [ProducesResponseType(typeof(Department), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<Department>> UpdateDepartmentPut(Guid customerId, Guid departmentId, [FromBody] Department department)
        {
            var updatedDepartment = await _departmentServices.UpdateDepartmentPutAsync(customerId, departmentId, department.ParentDepartmentId, department.Name, department.CostCenterId, department.Description, department.CallerId);
            var departmentView = new Department(updatedDepartment);

            return Ok(departmentView);
        }

        [Route("{departmentId:Guid}")]
        [HttpPost]
        [ProducesResponseType(typeof(Department), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<Department>> UpdateDepartmentPatch(Guid customerId, Guid departmentId, [FromBody] Department department)
        {
            var updatedDepartment = await _departmentServices.UpdateDepartmentPatchAsync(customerId, departmentId, department.ParentDepartmentId, department.Name, department.CostCenterId, department.Description, department.CallerId);
            var departmentView = new Department(updatedDepartment);

            return Ok(departmentView);
        }

        [Route("{departmentId:Guid}")]
        [HttpDelete]
        [ProducesResponseType(typeof(Department), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<Department>> DeleteDepartment(Guid customerId, Guid departmentId, [FromBody] Guid callerId)
        {
            var updatedDepartment = await _departmentServices.DeleteDepartmentAsync(customerId, departmentId, callerId);
            var departmentView = new Department(updatedDepartment);

            return Ok(departmentView);
        }
    }
}
