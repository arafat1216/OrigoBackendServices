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
        public static readonly Guid ORGANIZATION_TWO_ID = Guid.Parse("e454f5c8-f19c-4c76-ae9e-cc53ecefb21a"); 
        public static readonly Guid ORGANIZATION_THREE_ID = Guid.Parse("701d5de6-7264-41a5-9edd-f8e842edebda");
        public static readonly Guid TECHSTEP_CUSTOMER_ID = Guid.Parse("c601dd7f-9930-46e2-944a-d994855663da");

        public static readonly Guid HEAD_DEPARTMENT_ID = Guid.Parse("37d6d1b1-54a5-465d-a313-b6c250d66db4");
        public static readonly Guid SUB_DEPARTMENT_ID = Guid.Parse("5355134f-4852-4c36-99d1-fa9d4a1d7a61");
        public static readonly Guid INDEPENDENT_DEPARTMENT_ID = Guid.Parse("384821b4-1872-484e-af44-14cf61e16266");
        public static readonly Guid CALLER_ID = Guid.Parse("a05f97fc-2e3d-4be3-a64c-e2f30ed90b93");
        public static readonly Guid PARENT_ID = Guid.Parse("fa82e042-f4bc-4de1-b68d-dfcb95a64c65");
        public static readonly Guid LOCATION_ID = Guid.Parse("089f6c40-1ae4-4fd0-b2d1-c5181d9fbfde");
        public static readonly Guid USER_ONE_ID = Guid.Parse("a12c5f56-aee9-47e0-9f5f-a726818323a9");
        public static readonly Guid USER_TWO_ID = Guid.Parse("8246626C-3BDD-46E7-BCDF-10FC038C0463");
        public static readonly Guid USER_THREE_ID = Guid.Parse("9f19a9e5-a4f0-431e-9137-e8bfba285c7f");
        public static readonly Guid USER_SIX_ID = Guid.Parse("86ff1e04-97ff-4753-81e6-e69343ee30a8");
        public static readonly Guid USER_SEVEN_ID = Guid.Parse("71376607-5531-4140-9085-3b8e2b713bec");
        public static readonly Guid USER_EIGHT_ID = Guid.Parse("8d0435a3-8a71-4fa2-9f40-f011e9076b89");
        public static readonly string USER_ONE_EMAIL = "kari@normann.no";

        public static readonly Guid USER_FOUR_ID = Guid.Parse("208ad639-9fe8-476d-bd89-d9b8ddcb76bf");
        public static readonly Guid USER_FIVE_ID = Guid.Parse("15936b85-22c8-466f-848b-59191094a576");
        public static readonly string USER_FOUR_EMAIL = "petter@pan.no";





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

            var contactPersonTWO = new ContactPerson("Kong", "Harald", "kong@harald.no", "+4790000000");
            var organizationPreferencesTWO = new OrganizationPreferences(ORGANIZATION_TWO_ID,
                                                                      CALLER_ID,
                                                                        null,
                                                                        null,
                                                                        null,
                                                                        true,
                                                                        "NO",
                                                                        1);
            var organizationPreferencesTHREE = new OrganizationPreferences(ORGANIZATION_THREE_ID,
                                                                    CALLER_ID,
                                                                      null,
                                                                      null,
                                                                      null,
                                                                      true,
                                                                      "NO",
                                                                      1);

            customerContext.OrganizationPreferences.Add(organizationPreferences);
            customerContext.OrganizationPreferences.Add(organizationPreferencesTWO);
            customerContext.OrganizationPreferences.Add(organizationPreferencesTHREE);

            var location = new Location("Location1",
                                        "Description",
                                        "Billingstadsletta",
                                        "19 B",
                                        "1396",
                                        "Oslo",
                                        "NO");

            var locationTWO = new Location("Location2",
                                       "Description",
                                       "Slottsplassen",
                                       "1",
                                       "0010",
                                       "Oslo",
                                       "NO");

            customerContext.Locations.Add(location);
            customerContext.Locations.Add(locationTWO);

            var techstepOrganization = new Organization(TECHSTEP_CUSTOMER_ID, null, "TECHSTEP", "1111111111",
              new Address("Brynsalléen 4", "0667", "Oslo", "no"),
              new ContactPerson("Børge", "Astrup", "the@boss.com", "+4799999991"),
              new OrganizationPreferences(TECHSTEP_CUSTOMER_ID, CALLER_ID, "www.techstep.com", "www.techstep.com/logo", null, true, "nb", 0),
              new Location("TECHSTEP NORWAY", "Head office", "Brynsalléen", "4", "0667", "Oslo", "nb"),
              null, true, 15, "payroll@techstep.com", false);

            var techstepPartner = new Partner(techstepOrganization);
            customerContext.Add(techstepPartner);

            var organization = new Organization(ORGANIZATION_ID,
                                                PARENT_ID,
                                                "ORGANIZATION ONE",
                                                "ORGNUM12345",
                                                address,
                                                contactPerson,
                                                organizationPreferences,
                                                location,
                                                null,
                                                true,
                                                15,
                                                ""
                                                );

            var organizationTWO = new Organization(ORGANIZATION_TWO_ID,
                                                null,
                                                "ORGANIZATION TWO",
                                                "ORGNUM2222",
                                                null,
                                                contactPersonTWO,
                                                organizationPreferencesTWO,
                                                locationTWO,
                                                null,
                                                true,
                                                1,
                                                ""
                                                );
            organizationTWO.InitiateOnboarding();

            var organizationThree = new Organization(ORGANIZATION_THREE_ID,
                                               null,
                                               "ORGANIZATION THREE",
                                               "ORGNUM3333",
                                               null,
                                               contactPersonTWO,
                                               organizationPreferencesTHREE,
                                               locationTWO,
                                               techstepPartner,
                                               true,
                                               1,
                                               ""
                                               );
            organizationThree.AddTechstepAccountOwner("Rolf Sjødal");
            organizationThree.AddTechstepCustomerId(123456789);


            customerContext.Organizations.Add(organization);
            customerContext.Organizations.Add(organizationTWO);
            customerContext.Organizations.Add(organizationThree);

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
            var independentDepartment = new Department("Independent department",
                                            "costCenterId",
                                            "Description",
                                            organization,
                                            INDEPENDENT_DEPARTMENT_ID,
                                            CALLER_ID,
                                            null);

            customerContext.Departments.Add(independentDepartment);

            var userOne = new User(organization,
                                USER_ONE_ID,
                                "Kari",
                                "Normann",
                                USER_ONE_EMAIL,
                                "+4790603360",
                                "EID:909091",
                                new UserPreference("no", CALLER_ID),
                                CALLER_ID);
            userOne.AssignDepartment(headDepartment, CALLER_ID);

            var userTwo = new User(organization,
                                USER_TWO_ID,
                                "Atish",
                                "Kumar",
                                "atish@normann.no",
                                "+4790603360",
                                "EID:909092",
                                new UserPreference("no", CALLER_ID),
                                CALLER_ID);

            userTwo.OffboardingInitiated(DateTime.UtcNow.AddDays(60) ,CALLER_ID);
            userTwo.AssignDepartment(subDepartment, CALLER_ID);

            var userThree = new User(organization,
                              USER_THREE_ID,
                              "Ola",
                              "Normann",
                              "ola@normann.no",
                              "+4790608989",
                              "EID:909093",
                              new UserPreference("no", CALLER_ID),
                              CALLER_ID);

            var userFour = new User(organization,
                              USER_FOUR_ID,
                              "Petter",
                              "Pan",
                              USER_FOUR_EMAIL,
                              "+4790606022",
                              "EID:909093",
                              new UserPreference("no", CALLER_ID),
                              CALLER_ID);
            
            var userFive = new User(organization,
                              USER_FIVE_ID,
                              "Ole",
                              "Brum",
                              "ole@brum.no",
                              "+4790689778",
                              "EID:90005",
                              new UserPreference("en", CALLER_ID),
                              CALLER_ID);
            var userSix = new User(organizationTWO,
                            USER_SIX_ID,
                            "Christoper",
                            "Robin",
                            "chris@eobin.no",
                            "+4790680078",
                            "EID:90006",
                            new UserPreference("en", CALLER_ID),
                            CALLER_ID);
            var userSeven = new User(organizationTWO,
                           USER_SEVEN_ID,
                           "Tussi",
                           "Tuss",
                           "tussi@tuss.no",
                           "+4790622078",
                           "EID:90007",
                           new UserPreference("en", CALLER_ID),
                           CALLER_ID);
            var userEight = new User(organizationThree,
                           USER_SEVEN_ID,
                           "Nasse",
                           "Nøff",
                           "nasse@nøff.no",
                           "+4790680888",
                           "EID:90008",
                           new UserPreference("en", CALLER_ID),
                           CALLER_ID);

            userFour.ChangeUserStatus("123", Common.Enums.UserStatus.Activated);
            userSix.ChangeUserStatus("123", Common.Enums.UserStatus.Activated);
            userSeven.ChangeUserStatus("123", Common.Enums.UserStatus.OnboardInitiated);
            userThree.ChangeUserStatus(null, Common.Enums.UserStatus.OnboardInitiated);
            userEight.ChangeUserStatus("133", Common.Enums.UserStatus.OnboardInitiated);

            customerContext.Users.Add(userOne);
            customerContext.Users.Add(userTwo);
            customerContext.Users.Add(userThree);
            customerContext.Users.Add(userFour);
            customerContext.Users.Add(userFive);
            customerContext.Users.Add(userSix);
            customerContext.Users.Add(userSeven);
            customerContext.Users.Add(userEight);



            var userOnePermission = new UserPermissions(userOne, new Role("EndUser"), new List<Guid> { ORGANIZATION_ID }, CALLER_ID);
            var userTwoPermission = new UserPermissions(userTwo, new Role("EndUser"), new List<Guid> { ORGANIZATION_ID }, CALLER_ID);
            var userThreePermission = new UserPermissions(userThree, new Role("Manager"), new List<Guid> { ORGANIZATION_ID }, CALLER_ID);
            var userFourPermission = new UserPermissions(userFour, new Role("Manager"), new List<Guid> { ORGANIZATION_ID }, CALLER_ID);
            var userFivePermission = new UserPermissions(userFive, new Role("Manager"), new List<Guid> { ORGANIZATION_ID }, CALLER_ID);



            customerContext.UserPermissions.Add(userOnePermission);
            customerContext.UserPermissions.Add(userTwoPermission);
            customerContext.UserPermissions.Add(userThreePermission);
            customerContext.UserPermissions.Add(userFourPermission);
            customerContext.UserPermissions.Add(userFivePermission);




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
