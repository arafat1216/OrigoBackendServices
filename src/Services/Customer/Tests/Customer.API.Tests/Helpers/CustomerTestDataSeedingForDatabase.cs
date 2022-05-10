using CustomerServices.Infrastructure.Context;
using CustomerServices.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Customer.API.IntegrationTests.Helpers
{
    internal static class CustomerTestDataSeedingForDatabase
    {

        public static readonly Guid ORGANIZATION_ID = Guid.Parse("f5635deb-9b38-411c-9577-5423c9290106");
        public static readonly Guid HEAD_DEPARTMENT_ID = Guid.Parse("37d6d1b1-54a5-465d-a313-b6c250d66db4");
        public static readonly Guid SUB_DEPARTMENT_ID = Guid.Parse("5355134f-4852-4c36-99d1-fa9d4a1d7a61");
        public static readonly Guid CALLER_ID = Guid.Parse("a05f97fc-2e3d-4be3-a64c-e2f30ed90b93");
        public static readonly Guid PARENT_ID = Guid.Parse("fa82e042-f4bc-4de1-b68d-dfcb95a64c65");
        public static readonly Guid LOCATION_ID = Guid.Parse("089f6c40-1ae4-4fd0-b2d1-c5181d9fbfde");
        public static readonly Guid USER_ONE_ID = Guid.Parse("a12c5f56-aee9-47e0-9f5f-a726818323a9");
        public static readonly Guid USER_TWO_ID = Guid.Parse("8246626C-3BDD-46E7-BCDF-10FC038C0463");
        public static readonly Guid USER_THREE_ID = Guid.Parse("9f19a9e5-a4f0-431e-9137-e8bfba285c7f");



        public static void PopulateData(CustomerContext customerContext)
        {
            customerContext.Database.EnsureCreated();
            var address = new Address("Billingstadsletta 19B", "1396", "Oslo", "NO");
            var contactPerson = new ContactPerson("Ola", "Normann", "ola@normann.no", "+4745454649");
            var organizationPreferences = new OrganizationPreferences(ORGANIZATION_ID,
                                                                      CALLER_ID,
                                                                        null,
                                                                        null,
                                                                        null,
                                                                        true,
                                                                        "NO",
                                                                        1);
            customerContext.OrganizationPreferences.Add(organizationPreferences);

            var location = new Location(LOCATION_ID,
                                        CALLER_ID,
                                        "Location1",
                                        "Description",
                                        "Billingstadsletta",
                                        "19 B",
                                        "1396",
                                        "Oslo",
                                        "NO");

            customerContext.Locations.Add(location);

            var organization = new Organization(ORGANIZATION_ID,
                                                CALLER_ID,
                                                PARENT_ID,
                                                "ORGANIZATION ONE",
                                                "ORGNUM12345",
                                                address,
                                                contactPerson,
                                                organizationPreferences,
                                                location,
                                                null,
                                                true
                                                );
            customerContext.Organizations.Add(organization);

            var headDepartment = new Department("Head department",
                                            "costCenterId",
                                            "Description",
                                            organization,
                                            HEAD_DEPARTMENT_ID,
                                            CALLER_ID);

            customerContext.Departments.Add(headDepartment);
            var subDepartment = new Department("Sub department",
                                            "costCenterId",
                                            "Description",
                                            organization,
                                            SUB_DEPARTMENT_ID,
                                            CALLER_ID,
                                            headDepartment);

            customerContext.Departments.Add(subDepartment);

            var userOne = new User(organization,
                                USER_ONE_ID,
                                "Kari",
                                "Normann",
                                "kari@normann.no",
                                "+4790603360",
                                "EID:909091",
                                new UserPreference("no", CALLER_ID),
                                CALLER_ID);

            var userTwo = new User(organization,
                                USER_TWO_ID,
                                "Atish",
                                "Kumar",
                                "atish@normann.no",
                                "+4790603360",
                                "EID:909092",
                                new UserPreference("no", CALLER_ID),
                                CALLER_ID);

            var userThree = new User(organization,
                              USER_THREE_ID,
                              "Ola",
                              "Normann",
                              "ola@normann.no",
                              "+4790608989",
                              "EID:909093",
                              new UserPreference("no", CALLER_ID),
                              CALLER_ID);

            customerContext.Users.Add(userOne);
            customerContext.Users.Add(userTwo);
            customerContext.Users.Add(userThree);


            var userOnePermission = new UserPermissions(userOne, new Role("EndUser"), new List<Guid> { ORGANIZATION_ID }, CALLER_ID);
            var userTwoPermission = new UserPermissions(userTwo, new Role("EndUser"), new List<Guid> { ORGANIZATION_ID }, CALLER_ID);
            var userThreePermission = new UserPermissions(userThree, new Role("DepartmentManager"), new List<Guid> { ORGANIZATION_ID }, CALLER_ID);


            customerContext.UserPermissions.Add(userOnePermission);
            customerContext.UserPermissions.Add(userTwoPermission);
            customerContext.UserPermissions.Add(userThreePermission);


            customerContext.SaveChanges();

        }
        public static void ResetDbForTests(CustomerContext dbContext)
        {
            var organizationPreferences = dbContext.OrganizationPreferences.ToArray();
            var organizations = dbContext.Organizations.ToArray();
            var locations = dbContext.Locations.ToArray();
            var departments = dbContext.Departments.ToArray();
            var users = dbContext.Users.ToArray();
            var userpermissions = dbContext.UserPermissions.ToArray();

            dbContext.OrganizationPreferences.RemoveRange(organizationPreferences);
            dbContext.Organizations.RemoveRange(organizations);
            dbContext.Locations.RemoveRange(locations);
            dbContext.Departments.RemoveRange(departments);
            dbContext.Users.RemoveRange(users);
            dbContext.UserPermissions.RemoveRange(userpermissions);

            dbContext.SaveChanges();

            PopulateData(dbContext);
        }
    }
}
