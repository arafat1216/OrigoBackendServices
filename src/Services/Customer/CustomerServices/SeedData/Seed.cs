using Common.Enums;
using CustomerServices.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerServices.SeedData
{
    public static class Seed
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Minor Code Smell", "S1075:URIs should not be hardcoded", Justification = "Test data should be static")]
        public static IList<Organization> GetCustomersData()
        {
            IList<Organization> organizations = new List<Organization>();

            #region organizations 
            Organization organization1 = new Organization(
                organizationId: new Guid("A19EA756-86F2-423C-9B10-11CB10181858"),
                callerId: new Guid("00000000-0000-0000-0000-000000000000"),
                parentId: null,
                companyName: "Blume Corporation",
                orgNumber: "898742482",
                companyAddress: new Address(
                    street: "320 N Morgan St Suite 600",
                    postCode: "IL 60607",
                    city: "Chicago",
                    country: "United States of America"
                    ),
                organizationContactPerson: new ContactPerson(
                    firstName: "Dušan",
                    lastName: "Nemec",
                    email: "Dušan.Nemec@blumecorp.com",
                    phoneNumber: "+4791119914"
                    ),
                organizationPreferences: new OrganizationPreferences(
                    organizationId: new Guid("A19EA756-86F2-423C-9B10-11CB10181858"),
                    callerId: new Guid("00000000-0000-0000-0000-000000000000"),
                    webPage: "https://watchdogs.fandom.com/wiki/Blume_Corporation",
                    logoUrl: "https://static.wikia.nocookie.net/watchdogscombined/images/2/22/Blume_Logo.png/revision/latest?cb=20200331193456",
                    organizationNotes: "Blume Corporation is a technology company that specializes in communications technologies, information technologies as well as security technologies.It is the company that developed and owns CTOS as well as its iterations, and is responsible for their operation, maintenance and advancement.",
                    enforceTwoFactorAuth: true,
                    primaryLanguage: "EN",
                    defaultDepartmentClassification: 0
                   ),
                organizationLocation: new Location(
                    locationId: new Guid("90A72B84-01FA-4B9B-A22A-B455FC54948A"), // LocationId
                    callerId: new Guid("00000000-0000-0000-0000-000000000000"), // callerId
                    name: "Bellwether", // name
                    description: "Blume headquarters", // description
                    address1: "320 N Morgan St Suite 600", // address1
                    address2: "1200 W Harrison St", // address2
                    postalCode: "IL 60607", // postalCode
                    city: "Chicago", // city 
                    country: "United States of America" // country
                    )
                );

            Organization organization2 = new Organization(
                organizationId: new Guid("F2B5B8E5-78E1-4643-B97B-49239DAC74C2"),
                callerId: new Guid("00000000-0000-0000-0000-000000000000"),
                parentId: new Guid("A19EA756-86F2-423C-9B10-11CB10181858"),
                companyName: "Umeni Security Corps",
                orgNumber: "913992399",
                companyAddress: new Address(
                    street: "1200 W Harrison St",
                    postCode: "IL 60607",
                    city: "Chicago",
                    country: "United States of America"
                    ),
                organizationContactPerson: new ContactPerson(
                    firstName: "Dušan",
                    lastName: "Nemec",
                    email: "Dušan.Nemec@blumecorp.com",
                    phoneNumber: "+4791119914"
                    ),
                organizationPreferences: new OrganizationPreferences(
                    organizationId: new Guid("A19EA756-86F2-423C-9B10-11CB10181858"),
                    callerId: new Guid("00000000-0000-0000-0000-000000000000"),
                    webPage: "https://watchdogs.fandom.com/wiki/Umeni-Zulu",
                    logoUrl: "https://static.wikia.nocookie.net/watchdogscombined/images/6/6a/Umeni_Security_Corps_Logo.jpg/revision/latest/scale-to-width-down/350?cb=20190803142026",
                    organizationNotes: "Umeni-Zulu, better known as Umeni Security Corps or Umeni Corporation, is a private security company. Umeni-Zulu began by providing the U.S. government with cryptographic technology products before expanding their service into collecting intelligence both domestically and internationally.",
                    enforceTwoFactorAuth: true,
                    primaryLanguage: "EN",
                    defaultDepartmentClassification: 0
                   ),
                organizationLocation: new Location(
                    locationId: new Guid("39C3933E-40CC-4DA2-89D6-514105C3F34A"),
                    callerId: new Guid("00000000-0000-0000-0000-000000000000"),
                    name: "Sonarus",
                    description: "Umeni main location",
                    address1: "1200 W Harrison St",
                    address2: "320 N Morgan St Suite 600",
                    postalCode: "IL 60607",
                    city: "Chicago",
                    country: "United States of America"
                    )
                );
            #endregion

            organizations.Add(organization1);
            organizations.Add(organization2);

            return organizations;
        }

        public static IList<Department> GetDepartmentDataForOrganization(Organization organization)
        {
            IList<Department> departments = new List<Department>();
            if (organization.OrganizationId == new Guid("A19EA756-86F2-423C-9B10-11CB10181858"))
            {
                Department departmentRoot1 = new Department("Bellwether", "9911", "", organization, new Guid("8DFD4C18-A1E5-4A34-906C-FE5F25F01FAB"), null);
                Department departmentRoot2 = new Department("Blume Corporation", "9912", "", organization, new Guid("CFF07875-3E86-4834-B066-9DA308F3EF05"), null);
                departments.Add(departmentRoot1);
                departments.Add(departmentRoot2);
                departments.Add(new Department("Administration", "8012", "", organization, new Guid("D6785A43-1B81-4BAA-8314-42D78ED1B227"), departmentRoot1));
                departments.Add(new Department("Technology", "8014", "Research and development", organization, new Guid("6B748D35-21FA-491D-ACE2-B8643A26AAF2"), departmentRoot1));
                departments.Add(new Department("ctOS", "5401", "Smart city services", organization, new Guid("B54805A6-3792-4FCD-A57D-7C464E08FAF4"), departmentRoot2));
                departments.Add(new Department("Technology", "5402", "Research and development", organization, new Guid("B1564DAA-833A-46DB-95A9-62212B31E810"), departmentRoot2));
                departments.Add(new Department("Marketing", "5403", "Advertising and marketing", organization, new Guid("22661B52-6A25-4B23-A04B-33482BADEC02"), departmentRoot2));
                departments.Add(new Department("Administration", "6012", "", organization, new Guid("F071A053-28D2-4BEB-9C7C-2DEACA2EF85E"), departmentRoot2));
            }
            else if (organization.OrganizationId == new Guid("F2B5B8E5-78E1-4643-B97B-49239DAC74C2"))
            {
                Department departmentRoot1 = new Department("Umeni-Zulu", "391", "", organization, new Guid("071A5D79-10F9-4A70-9CAD-7A7EA51E253A"), null);
                Department departmentRoot2 = new Department("Umeni Security Corps", "415", "", organization, new Guid("35374FE2-CC30-49B6-8A3A-7AFF4F0DB8DB"), null);
                departments.Add(departmentRoot1);
                departments.Add(departmentRoot2);
                departments.Add(new Department("Marketing", "503", "Advertising and marketing", organization, new Guid("8AEAEDD6-BD0E-4957-9053-0E9B01F1D291"), departmentRoot1));
                departments.Add(new Department("Administration", "760", "Umeni-Zulu administration", organization, new Guid("26FC799C-4F52-4604-8123-B9A571429348"), departmentRoot1));
                departments.Add(new Department("Technology", "804", "Cyber security and research", organization, new Guid("515924FA-E94E-4281-AE13-8E04B049ED8D"), departmentRoot2));
                departments.Add(new Department("Administration", "712", "Umeni Security Corps administration", organization, new Guid("906944BA-CBDA-4699-B210-60E373B68A23"), departmentRoot2));
            }

            return departments;
        }

        public static IList<User> GetUsersForOrganization(Organization organization)
        {
            IList<User> users = new List<User>();
            if (organization.OrganizationId == new Guid("A19EA756-86F2-423C-9B10-11CB10181858"))
            {
                users.Add(new User(organization, new Guid("D0326090-631F-4138-9CD2-85249AD24BBB"), "Dušan", "Nemec", "Dusan.Nemec@blumecorp.com", "+4791119914", "01-4334", new UserPreference("EN")));
                users.Add(new User(organization, new Guid("0747105E-BE51-4BB9-A7C0-976C562A8A25"), "Charlotte", "Gardner", "charlotte.gardner@blumecorp.com", "+4745391457", "01-4455", new UserPreference("EN")));
                users.Add(new User(organization, new Guid("9C181422-1D23-4058-AEB8-FE3EBB33D130"), "Skye", "Larson", "skye.larson@blumecorp.com", "+4760391455", "01-2323", new UserPreference("EN")));
                users.Add(new User(organization, new Guid("B7F084EB-3A99-4384-AC8B-775A7D16FF99"), "Malcolm", "Deodato", "malcolm.deodato@blumecorp.com", "+4780567333", "01-3400", new UserPreference("EN")));
                users.Add(new User(organization, new Guid("49F4E089-FEED-4FE3-B5BC-719EB084F2BE"), "Bradley", "Caughlin", "bradley.caughlin@blumecorp.com", "+4792005194", "01-4467", new UserPreference("EN")));
                users.Add(new User(organization, new Guid("71B8C4FE-B24B-40D9-93DB-67A4AE95932D"), "Gene", "Deloray", "gene.deloray@blumecorp.com", "+4790111610", "01-4590", new UserPreference("EN")));
                users.Add(new User(organization, new Guid("0F986B95-60A2-4C74-B111-382E49B962F7"), "Andrew", "Novak", "andrew.novak@blumecorp.com", "+4795369914", "01-2391", new UserPreference("EN")));
                users.Add(new User(organization, new Guid("3C4CC49C-134C-4F62-BA31-6748A4637EB5"), "Raymond", "Kenney", "raymond.kenney@blumecorp.com", "+4792458839", "01-9963", new UserPreference("EN")));
                users.Add(new User(organization, new Guid("100172A8-B885-4ABC-8F72-F37DA6F6E052"), "Tobias", "Frewer", "tobias.frewer@blumecorp.com", "+4762455555", "01-4672", new UserPreference("EN")));
                users.Add(new User(organization, new Guid("16D973B5-4E57-4A26-858E-7EFA5B72695E"), "Roger", "Verrick", "roger.verrick@blumecorp.com", "+4798293100", "01-2362", new UserPreference("EN")));
                users.Add(new User(organization, new Guid("0CF5C2AC-DFCB-4B7D-A0F3-B0AB9AD309FA"), "Rose", "Washington", "rose.washington@blumecorp.com", "+4792739101", "01-9074", new UserPreference("EN")));
                users.Add(new User(organization, new Guid("F5C24E0A-2BEA-481F-AEAF-5F991CCE7F56"), "Ross", "Horitz", "ross.horitz@blumecorp.com", "+4745613910", "01-4520", new UserPreference("EN")));
                users.Add(new User(organization, new Guid("C0C178F6-0A5A-46DD-8B0B-9129BF92CE01"), "Angus", "Shostack", "angus.shostack@blumecorp.com", "+4745671212", "01-6721", new UserPreference("EN")));
                users.Add(new User(organization, new Guid("099F7F4F-C908-49B6-91BE-5A2908A6BA3D"), "JB", "Markovicz", "defalt@blumecorp.com", "+4745771720", "01-6084", new UserPreference("EN")));
                users.Add(new User(organization, new Guid("CC9F5DD4-FC5A-4CA2-9C96-91D40160714F"), "Aiden", "Pearce", "aiden.pearce@blumecorp.com", "+4790804545", "01-3091", new UserPreference("EN")));
            }
            else if (organization.OrganizationId == new Guid("F2B5B8E5-78E1-4643-B97B-49239DAC74C2"))
            {
                users.Add(new User(organization, new Guid("0A419CAA-5E5E-46A5-9F93-4C58CE57C630"), "Annette", "Noller", "annette.noller@umeni-zulu.com", "+4791145523", "01-434", new UserPreference("EN")));
                users.Add(new User(organization, new Guid("0626E84E-B301-4714-8403-926805CB5B29"), "Brandon", "Schous", "brandon.schous@umeni-zulu.com", "+4798023450", "01-355", new UserPreference("EN")));
                users.Add(new User(organization, new Guid("84C66597-D67D-4DC5-A598-761D76DD97AF"), "Zelda", "Thomsson", "zelda.thomsson@umeni-zulu.com", "+4761813245", "01-2323", new UserPreference("EN")));
                users.Add(new User(organization, new Guid("E0BFAAEA-E850-44C2-A224-3356ED0FB4ED"), "Jack", "Dale", "jack.dale@umeni-zulu.com", "+4762624544", "01-340", new UserPreference("EN")));
                users.Add(new User(organization, new Guid("5EF1B2AF-D695-4DEB-9FDB-23D1F725025A"), "Borris", "Johnson", "borris.johnson@umeni-zulu.com", "+4798002323", "01-767", new UserPreference("EN")));
                users.Add(new User(organization, new Guid("4806344A-23E4-4DF0-AD71-7D8826C7EB6B"), "Cathrine", "DeLain", "cathrine.delain@umeni-zulu.com", "+4799119203", "01-590", new UserPreference("EN")));
                users.Add(new User(organization, new Guid("BE131783-6196-4105-940D-83DC5271A06B"), "Christian", "Nettle", "christian.nettle@umeni-zulu.com", "+4745438194", "01-991", new UserPreference("EN")));
                users.Add(new User(organization, new Guid("D22CCB79-B474-4BC0-B93F-AB45FC945AFC"), "Raymond", "Cross", "raymond.cross@umeni-zulu.com", "+4790673222", "01-922", new UserPreference("EN")));
                users.Add(new User(organization, new Guid("A2A0ED0F-ED5D-4050-94CD-D1BC54E18617"), "Jane", "Larkssen", "jane.larkssen@umeni-zulu.com", "+4761230078", "01-301", new UserPreference("EN")));
                users.Add(new User(organization, new Guid("9F12135D-1AB1-4DB9-8F33-DE5385E57813"), "Amanda", "Tapping", "amanda.tapping@umeni-zulu.com", "+4799124562", "01-649", new UserPreference("EN")));
            }
            return users;
        }

        public static Dictionary<Guid, PredefinedRole> GetUserRoles()
        {
            Dictionary<Guid, PredefinedRole> roles = new Dictionary<Guid, PredefinedRole>();
            roles.Add(new Guid("D0326090-631F-4138-9CD2-85249AD24BBB"), PredefinedRole.CustomerAdmin);
            roles.Add(new Guid("0747105E-BE51-4BB9-A7C0-976C562A8A25"), PredefinedRole.CustomerAdmin);
            roles.Add(new Guid("9C181422-1D23-4058-AEB8-FE3EBB33D130"), PredefinedRole.DepartmentManager);
            roles.Add(new Guid("B7F084EB-3A99-4384-AC8B-775A7D16FF99"), PredefinedRole.DepartmentManager);
            roles.Add(new Guid("0A419CAA-5E5E-46A5-9F93-4C58CE57C630"), PredefinedRole.CustomerAdmin);
            roles.Add(new Guid("0626E84E-B301-4714-8403-926805CB5B29"), PredefinedRole.CustomerAdmin);
            roles.Add(new Guid("84C66597-D67D-4DC5-A598-761D76DD97AF"), PredefinedRole.DepartmentManager);

            return roles;
        }

        public static Guid[] GetAccessList(Guid organizationId)
        {
            if (organizationId == new Guid("A19EA756-86F2-423C-9B10-11CB10181858"))
            {
                return new Guid[] { new Guid("8DFD4C18-A1E5-4A34-906C-FE5F25F01FAB"), new Guid("CFF07875-3E86-4834-B066-9DA308F3EF05") };
            }
            else if (organizationId == new Guid("F2B5B8E5-78E1-4643-B97B-49239DAC74C2"))
            {
                return new Guid[] { new Guid("26FC799C-4F52-4604-8123-B9A571429348"), new Guid("071A5D79-10F9-4A70-9CAD-7A7EA51E253A") };
            }
            return new Guid[1];
        }
    }
}
