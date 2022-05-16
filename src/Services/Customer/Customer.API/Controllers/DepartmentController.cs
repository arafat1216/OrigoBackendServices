using AutoMapper;
using Customer.API.ViewModels;
using Customer.API.WriteModels;
using CustomerServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
        public async Task<ActionResult<Department>> GetDepartment(Guid customerId, Guid departmentId)
        {
            var departmentEntity = await _departmentServices.GetDepartmentAsync(customerId, departmentId);
            if (departmentEntity == null)
                return NotFound();

            var departmentView = _mapper.Map<Department>(departmentEntity);

            return Ok(departmentView);
        }

        [HttpGet]
        [ProducesResponseType(typeof(IList<Department>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<Department>> GetDepartments(Guid customerId)
        {
            var departmentList = await _departmentServices.GetDepartmentsAsync(customerId);
            var departmentView = _mapper.Map<IList<Department>>(departmentList);

            return Ok(departmentView);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Department), (int)HttpStatusCode.Created)]
        public async Task<ActionResult<Department>> CreateDepartment(Guid customerId, [FromBody] NewDepartment department)
        {
            Guid newDepartmentId = Guid.NewGuid();
            var createdDepartment = await _departmentServices.AddDepartmentAsync(customerId, newDepartmentId, department.ParentDepartmentId, department.Name, department.CostCenterId, department.Description, department.ManagedBy, department.CallerId);
            var departmentView = _mapper.Map<Department>(createdDepartment);

            return CreatedAtAction(nameof(CreateDepartment), new { id = departmentView.DepartmentId }, departmentView);
        }

        [Route("{departmentId:Guid}")]
        [HttpPut]
        [ProducesResponseType(typeof(Department), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<Department>> UpdateDepartmentPut(Guid customerId, Guid departmentId, [FromBody] UpdateDepartment department)
        {
            var updatedDepartment = await _departmentServices.UpdateDepartmentPutAsync(customerId, departmentId, department.ParentDepartmentId, department.Name, department.CostCenterId, department.Description,department.ManagedBy, department.CallerId);
            var departmentView = _mapper.Map<Department>(updatedDepartment);

            return Ok(departmentView);
        }

        [Route("{departmentId:Guid}")]
        [HttpPost]
        [ProducesResponseType(typeof(Department), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<Department>> UpdateDepartmentPatch(Guid customerId, Guid departmentId, [FromBody] UpdateDepartment department)
        {
            var updatedDepartment = await _departmentServices.UpdateDepartmentPatchAsync(customerId, departmentId, department.ParentDepartmentId, department.Name, department.CostCenterId, department.Description, department.ManagedBy, department.CallerId);
            var departmentView = _mapper.Map<Department>(updatedDepartment);

            return Ok(departmentView);
        }

        [Route("{departmentId:Guid}")]
        [HttpDelete]
        [ProducesResponseType(typeof(Department), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<Department>> DeleteDepartment(Guid customerId, Guid departmentId, [FromBody] Guid callerId)
        {
            var updatedDepartment = await _departmentServices.DeleteDepartmentAsync(customerId, departmentId, callerId);
            var departmentView = _mapper.Map<Department>(updatedDepartment);

            return Ok(departmentView);
        }
    }
}
