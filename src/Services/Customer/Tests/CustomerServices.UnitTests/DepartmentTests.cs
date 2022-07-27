using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Enums;
using CustomerServices.Models;
using Xunit;

namespace CustomerServices.UnitTests
{
    public class DepartmentTests
    {
        [Fact]
        public void SubDepartments_CheckWithOneLevel_ReturnsSupDepartments()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var organizationPreferences = new OrganizationPreferences(customerId, Guid.Empty, null, null, null, false, string.Empty, 1);
            var location = new Location(string.Empty, string.Empty, string.Empty, string.Empty,
                string.Empty, string.Empty, string.Empty);
            var organization = new Organization(customerId, null, "COMPANY A", "999999999", new Address(),
                new ContactPerson(), organizationPreferences, location, null, true, 1, "");
            var l1DeptId = Guid.NewGuid();
            var topLevelDepartment = new Department("L1", "", "", organization, l1DeptId, Guid.Empty);
            organization.AddDepartment(topLevelDepartment, Guid.Empty);
            var l2ADeptId = Guid.NewGuid();
            var l2ADepartment = new Department("L2a", "", "", organization, l2ADeptId, Guid.Empty, topLevelDepartment);
            organization.AddDepartment(l2ADepartment, Guid.Empty);
            var l2BDeptId = Guid.NewGuid();
            var l2BDepartment = new Department("L2b", "", "", organization, l2BDeptId, Guid.Empty, topLevelDepartment);
            organization.AddDepartment(l2BDepartment, Guid.Empty);
            var l3A1DeptId = Guid.NewGuid();
            var l3A1Department = new Department("L3a1", "", "", organization, l3A1DeptId, Guid.Empty, l2ADepartment);
            organization.AddDepartment(l3A1Department, Guid.Empty);
            var l3A2DeptId = Guid.NewGuid();
            var l3A2Department = new Department("L3a2", "", "", organization, l3A2DeptId, Guid.Empty, l2ADepartment);
            organization.AddDepartment(l3A2Department, Guid.Empty);
            var l3B1DeptId = Guid.NewGuid();
            var l3B1Department = new Department("L3b1", "", "", organization, l3B1DeptId, Guid.Empty, l2BDepartment);
            organization.AddDepartment(l3B1Department, Guid.Empty);
            var l4A1DeptId = Guid.NewGuid();
            var l4A1Department = new Department("L4a1", "", "", organization, l4A1DeptId, Guid.Empty, l3A1Department);
            organization.AddDepartment(l4A1Department, Guid.Empty);

            // Act
            var subDepartments = l2ADepartment.SubDepartments(organization.Departments.ToList());

            // Assert
            Assert.Contains(subDepartments, d => d.ExternalDepartmentId == l2ADeptId);
            Assert.Contains(subDepartments, d => d.ExternalDepartmentId == l3A1DeptId);
            Assert.Contains(subDepartments, d => d.ExternalDepartmentId == l3A2DeptId);
            Assert.Contains(subDepartments, d => d.ExternalDepartmentId == l4A1DeptId);
            Assert.DoesNotContain(subDepartments, d => d.ExternalDepartmentId == l3B1DeptId);
            Assert.DoesNotContain(subDepartments, d => d.ExternalDepartmentId == l2BDeptId);
        }
    }
}
