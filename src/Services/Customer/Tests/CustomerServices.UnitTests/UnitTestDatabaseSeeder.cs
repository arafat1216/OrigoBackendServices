using System;
using System.Collections.Generic;
using Common.Enums;
using Common.Infrastructure;
using CustomerServices.Infrastructure.Context;
using CustomerServices.Models;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace CustomerServices.UnitTests;

public class UnitTestDatabaseSeeder
{
    public static readonly Guid TECHSTEP_PARTNER_ID = new("d20ec73e-42d4-49d5-b5e2-82e061cf59f0");
    public static readonly Guid PARTNER_ID = new("a28e408f-00c3-4ee9-9aee-165c32c7e912");

    public static readonly Guid CUSTOMER_ONE_ID = new("661b01e0-481b-4e7d-8076-a0e7b6496ae3");
    public static readonly Guid CUSTOMER_TWO_ID = new("f1530515-fe2e-4e2f-84c2-c60da5875e22");
    public static readonly Guid CUSTOMER_THREE_ID = new("6fb371c9-da3e-4ce4-b4e4-bc7f020eebf9");
    public static readonly Guid CUSTOMER_FOUR_ID = new("2C005777-ED56-43D9-9B1E-2B8112E67D10");
    public static readonly Guid CUSTOMER_FIVE_ID = new("94e47c5b-9486-4017-8bab-252422b1262a");
    public static readonly Guid PARTNER_CUSTOMER_ID = new("ea8db27c-560f-4c42-8787-9646b6d0509f");
    public static readonly Guid TECHSTEP_CUSTOMER_ID = new("c601dd7f-9930-46e2-944a-d994855663da");

    public static readonly Guid DEPARTMENT_ONE_ID = new("f0680388-145a-11ec-a469-00155d98690f");
    public static readonly Guid DEPARTMENT_TWO_ID = new("424f4485-53cc-4e59-8fae-59b27f12ff61");

    public static readonly Guid USER_ONE_ID = new("42803f8e-5608-4beb-a3e6-029b8e229d91");
    public static readonly Guid USER_TWO_ID = new("39349c24-6e47-4a5e-9bab-7b65f438fac5");
    public static readonly Guid USER_THREE_ID = new("08DB1C4F-FAA3-436A-9598-90822649B793");
    public static readonly Guid USER_FOUR_ID = new("a0c3ee8d-b543-4dc9-88b5-958c54b9d270");
    public static readonly Guid USER_FIVE_ID = new("07c8ce13-a6a1-4a80-b029-619098a76bb5");
    public static readonly Guid USER_SIX_ID = new("cee1e91f-e6d0-44ef-866a-23b032f3a214");
    public static readonly Guid USER_SEVEN_ID = new("c43a8c1e-8ded-42ab-a0d1-0e4c2ce92cdc");

    public static readonly Guid EMPTY_CALLER_ID = Guid.Empty;
    public static readonly Guid CALLER_ID = new("5E9729DA-8D91-41AD-9D88-BB0E0BC25C99");

    private DbContextOptions<CustomerContext> ContextOptions { get; set; } = null!;

    public void SeedUnitTestDatabase(DbContextOptions<CustomerContext> contextOptions)
    {
        ContextOptions = contextOptions;
        Seed();
    }

    private void Seed()
    {
        var apiRequesterServiceMock = new Mock<IApiRequesterService>();
        apiRequesterServiceMock.Setup(r => r.AuthenticatedUserId).Returns(CALLER_ID);
        using var context = new CustomerContext(ContextOptions, apiRequesterServiceMock.Object);
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        var techstepOrganization = new Organization(TECHSTEP_CUSTOMER_ID, null, "TECHSTEP", "1111111111",
            new Address("Brynsalléen 4", "0667", "Oslo", "no"),
            new ContactPerson("Børge", "Astrup", "the@boss.com", "+4799999991"),
            new OrganizationPreferences(TECHSTEP_CUSTOMER_ID, EMPTY_CALLER_ID, "www.techstep.com",
                "www.techstep.com/logo", null, true, "nb", 0),
            new Location("TECHSTEP NORWAY", "Head office", "Brynsalléen", "4", "0667", "Oslo", "nb"), null, true, 15,
            null, "", "payroll@techstep.com");
        var partnerOrganization = new Organization(PARTNER_CUSTOMER_ID, null, "PARTNER", "22222222222",
            new Address("Billingstadsletta 19B", "1396", "Oslo", "no"),
            new ContactPerson("Svein", "Hansen", "le@boss.com", "+4799999992"),
            new OrganizationPreferences(PARTNER_CUSTOMER_ID, EMPTY_CALLER_ID, "www.mytos.com", "www.mytos.com/logo",
                null, true, "nb", 0),
            new Location("MYTOS", "R&D office", "Billingstadsletta", "19B", "1396", "Oslo", "nb"), null, true, 15, null,
            null, "payroll@mytos.com");

        var techstepPartner = new Partner(techstepOrganization, TECHSTEP_PARTNER_ID);
        var partner = new Partner(partnerOrganization, PARTNER_ID);
        context.Add(techstepPartner);
        context.Add(partner);
        var customerOne = new Organization(CUSTOMER_ONE_ID, null, "COMPANY ONE", "999888777",
            new Address("My Way 1", "1111", "My City", "NO"),
            new ContactPerson("JOHN", "DOE", "john.doe@example.com", "99999999"),
            new OrganizationPreferences(CUSTOMER_ONE_ID, USER_ONE_ID, "webPage 1", "logoUrl 1", "organizationNotes 1",
                true, "nb", 0),
            new Location("COMPANY ONE", "Location of COMPANY ONE", "My Way 1A", "My Way 1B", "0585", "Oslo", "NO"),
            null, true, 1, null, null);

        var customerTwo = new Organization(CUSTOMER_TWO_ID, null, "COMPANY TWO", "999777666",
            new Address("My Way 2", "1111", "My City", "NO"),
            new ContactPerson("Ola", "Nordmann", "ola.nordmann@example.com", "99999998"),
            new OrganizationPreferences(CUSTOMER_TWO_ID, USER_ONE_ID, "webPage 2", "logoUrl 2", "organizationNotes 2",
                true, "nb", 0), new Location("name", "description", "My Way 2A", "My Way 2B", "0585", "Oslo", "NO"),
            null, true, 1, null, null, "", true);

        var customerThree = new Organization(CUSTOMER_THREE_ID, CUSTOMER_ONE_ID, "COMPANY THREE", "999666555",
            new Address("My Way 3", "1111", "My Other City", "NO"),
            new ContactPerson("Kari", "Nordmann", "kari.nordmann@example.com", "99999997"),
            new OrganizationPreferences(CUSTOMER_THREE_ID, USER_ONE_ID, "webPage 3", "logoUrl 3", "organizationNotes 3",
                true, "nb", 0), new Location("name", "description", "My Way 3A", "My Way 3B", "0585", "Oslo", "NO"),
            partner, true, 1, null, null);

        var customerFour = new Organization(CUSTOMER_FOUR_ID, null, "COMPANY FOUR", "999555444",
            new Address("My Way 4", "1111", "My City", "NO"),
            new ContactPerson("Petter", "Smart", "petter.smart@example.com", "99999996"),
            new OrganizationPreferences(CUSTOMER_FOUR_ID, USER_ONE_ID, "webPage 4", "logoUrl 4", "organizationNotes 4",
                true, "nb", 0), new Location("name", "description", "My Way 4A", "My Way 4B", "0585", "Oslo", "NO"),
            techstepPartner, true, 1, null, null, "", true);
        customerFour.AddTechstepCustomerId(123456700);
        customerFour.InitiateOnboarding();

        var customerFive = new Organization(CUSTOMER_FIVE_ID, null, "COMPANY FIVE", "999555555",
            new Address("My Way 5", "1111", "My City", "NO"),
            new ContactPerson("Donald", "Duck", "donald.duck@example.com", "99999995"),
            new OrganizationPreferences(CUSTOMER_FIVE_ID, USER_ONE_ID, "webPage 5", "logoUrl 5", "organizationNotes 5",
                true, "nb", 0), new Location("name", "description", "My Way 5A", "My Way 5B", "0585", "Oslo", "NO"),
            null, true, 1, null, null);
        customerFive.AddTechstepCustomerId(123456789);


        context.AddRange(customerOne, customerTwo, customerThree, customerFour, customerFive, techstepOrganization);
        context.OrganizationPreferences.AddRange(customerOne.Preferences, customerTwo.Preferences,
            customerThree.Preferences, customerFour.Preferences);
        context.Locations.AddRange(customerOne.PrimaryLocation!, customerTwo.PrimaryLocation!,
            customerThree.PrimaryLocation!, customerFour.PrimaryLocation!);
        var departmentOneForCustomerOne = new Department("Cust1Dept1", "1123", "Department one for customer one",
            customerOne, DEPARTMENT_ONE_ID, Guid.Empty);
        var departmentTwoForCustomerOne =
            new Department("Department1", "456", "Desc", customerOne, DEPARTMENT_TWO_ID, Guid.Empty);

        context.Add(departmentTwoForCustomerOne);
        context.Add(departmentOneForCustomerOne);

        var userPreferences1 = new UserPreference("NO", EMPTY_CALLER_ID);
        var userPreferences2 = new UserPreference("EN", EMPTY_CALLER_ID);
        var userPreferences3 = new UserPreference("EN", EMPTY_CALLER_ID);
        var userOne = new User(customerOne, USER_ONE_ID, "Jane", "Doe", "jane@doe.com", "+4799999999", "007",
            userPreferences1, EMPTY_CALLER_ID);
        userOne.ChangeUserStatus("123", UserStatus.Deactivated);

        var userTwo = new User(customerOne, USER_THREE_ID, "Gordon", "Freeman", "gordon@freeman.com", "+4755555555",
            "DH-101", userPreferences2, EMPTY_CALLER_ID);
        userTwo.AssignDepartment(departmentOneForCustomerOne, EMPTY_CALLER_ID);
        userTwo.ChangeUserStatus("123", UserStatus.Activated);

        var userPreferences4 = new UserPreference("EN", EMPTY_CALLER_ID);
        var userPreferences5 = new UserPreference("EN", EMPTY_CALLER_ID);
        var userFour = new User(customerOne, USER_FOUR_ID, "Al", "Pacino", "al@Pacino.com", "+4755555555", "DH-104",
            userPreferences4, EMPTY_CALLER_ID);
        var userFive = new User(customerOne, USER_FIVE_ID, "Robert", "De Niro", "robert@deniro.com", "+4755555555",
            "DH-105", userPreferences5, EMPTY_CALLER_ID);
        userFive.AssignDepartment(departmentTwoForCustomerOne, EMPTY_CALLER_ID);

        //CustomerTwo
        var userThree = new User(customerTwo, USER_TWO_ID, "John", "Doe", "john@doe.com", "+4791111111", "X",
            userPreferences3, EMPTY_CALLER_ID);
        userThree.ChangeUserStatus("123", UserStatus.Invited);
        var userSix = new User(customerTwo, USER_SIX_ID, "Elvis", "Presley", "the@king.com", "+4790000000", "X",
            userPreferences3, EMPTY_CALLER_ID);

        var userSeven = new User(customerTwo, USER_SEVEN_ID, "Lisa Marie", "Presley", "the@princess.com", "+4790000001",
            "X", userPreferences3, EMPTY_CALLER_ID);
        userSeven.ChangeUserStatus("123", UserStatus.OnboardInitiated);


        var role1 = new Role("admin");
        var managerRole = new Role("Manager");
        var userPermissions = new UserPermissions(userOne, role1, new List<Guid>(), EMPTY_CALLER_ID);
        var managerPermissions = new UserPermissions(userFour, managerRole, new List<Guid>(), EMPTY_CALLER_ID);
        var managerPermissionsTwo = new UserPermissions(userFive, managerRole, new List<Guid>(), EMPTY_CALLER_ID);

        userFour.AssignManagerToDepartment(departmentTwoForCustomerOne, EMPTY_CALLER_ID);
        userFive.AssignManagerToDepartment(departmentTwoForCustomerOne, EMPTY_CALLER_ID);

        context.AddRange(role1, managerRole);
        context.AddRange(userPermissions, managerPermissions, managerPermissionsTwo);
        context.AddRange(userOne, userTwo, userThree, userFour, userFive, userSix, userSeven);

        context.SaveChanges();
    }
}