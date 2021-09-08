using Customer.API.ViewModels;
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
    [Route("api/v{version:apiVersion}/customers/{customerId:Guid}/[controller]")]
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
            var createdDepartment = await _departmentServices.AddDepartmentAsync(customerId, newDepartmentId, department.ParentDepartmentId, department.Name, department.CostCenterId, department.Description);
            var departmentView = new Department(createdDepartment);

            return CreatedAtAction(nameof(CreateDepartment), new { id = departmentView.DepartmentId }, departmentView);
        }
    }
}
