using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OrigoApiGateway.Models;
using OrigoApiGateway.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Threading.Tasks;

namespace OrigoApiGateway.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    //[Authorize]
    [Route("origoapi/v{version:apiVersion}/Customers/{customerId:guid}/[controller]")]
    [SuppressMessage("ReSharper", "RouteTemplates.RouteParameterConstraintNotResolved")]
    [SuppressMessage("ReSharper", "RouteTemplates.ControllerRouteParameterIsNotPassedToMethods")]
    public class DepartmentsController : ControllerBase
    {
        private readonly ILogger<DepartmentsController> _logger;
        private readonly IDepartmentsServices _departmentServices;

        public DepartmentsController(ILogger<DepartmentsController> logger, IDepartmentsServices departmentServices)
        {
            _logger = logger;
            _departmentServices = departmentServices;
        }

        [Route("{departmentId:Guid}")]
        [HttpGet]
        [ProducesResponseType(typeof(OrigoDepartment), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<OrigoDepartment>> GetDepartment(Guid customerId, Guid departmentId)
        {
            try
            {
                var department = await _departmentServices.GetDepartment(customerId, departmentId);

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
        public async Task<ActionResult<OrigoDepartment>> GetDepartments(Guid customerId)
        {
            try
            {
                var departments = await _departmentServices.GetDepartments(customerId);

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
        public async Task<ActionResult<OrigoDepartment>> CreateDepartmentForCustomer(Guid customerId, [FromBody] NewDepartment newDepartment)
        {
            try
            {
                var createdDepartment = await _departmentServices.AddDepartmentAsync(customerId, newDepartment);

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
        public async Task<ActionResult<OrigoDepartment>> PutDepartmentForCustomer(Guid customerId, Guid departmentId, [FromBody] OrigoDepartment updateDepartment)
        {
            try
            {
                var updatedDepartment = await _departmentServices.UpdateDepartmentPutAsync(customerId, departmentId, updateDepartment);

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
        public async Task<ActionResult<OrigoDepartment>> PatchDepartmentForCustomer(Guid customerId, Guid departmentId, [FromBody] OrigoDepartment updateDepartment)
        {
            try
            {
                var updatedDepartment = await _departmentServices.UpdateDepartmentPatchAsync(customerId, departmentId, updateDepartment);

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
        public async Task<ActionResult<OrigoDepartment>> DeleteDepartmentForCustomer(Guid customerId, Guid departmentId)
        {
            try
            {
                var updatedDepartment = await _departmentServices.DeleteDepartmentPatchAsync(customerId, departmentId);

                return Ok(updatedDepartment);
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}
