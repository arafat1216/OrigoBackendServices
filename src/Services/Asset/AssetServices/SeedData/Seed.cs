using AssetServices.Models;
using Common.Enums;
using System;
using System.Collections.Generic;

namespace CustomerServices.SeedData
{
    public class Seed
    {
        private static Guid OrganizationId1 { get; set; } = new Guid("A19EA756-86F2-423C-9B10-11CB10181858");
        private static Guid OrganizationId2 { get; set; } = new Guid("F2B5B8E5-78E1-4643-B97B-49239DAC74C2");
        private static Guid CallerId { get; set; } = new Guid("D0326090-631F-4138-9CD2-85249AD24BBB");

        public static IList<Asset> GetAssetData(AssetCategory phone, AssetCategory tablet)
        {
            IList<Asset> assets = new List<Asset>();
            assets.Add(new Tablet(new Guid("40a488fc-9ffa-4544-887f-956f577c116e"), OrganizationId1, CallerId, "", tablet, "YDMYF2LYHF0L", "Apple", "iPad Pro 11", LifecycleType.NoLifecycle, new DateTime(2021, 6, 13), new Guid("9C181422-1D23-4058-AEB8-FE3EBB33D130"), new List<AssetImei>() { new AssetImei(336327301203750) }, "60:8B:0E:EF:18:83", AssetStatus.Active, "This iPad is used to order new equipment and services for the office.", "Office", "iPad Pro 11, 2021 512 GB WiFi (stellargrå)", new Guid("8DFD4C18-A1E5-4A34-906C-FE5F25F01FAB")));
            assets.Add(new MobilePhone(new Guid("d3f45808-d985-4cd8-895e-bde6ab9c087d"), OrganizationId1, CallerId, "", phone, "RFCN80WJJ5K", "Samsung", "Galaxy S21", LifecycleType.NoLifecycle, new DateTime(2021, 6, 11), Guid.Empty, new List<AssetImei>() { new AssetImei(357879702624426), new AssetImei(103735096024176) }, "44:18:FD:C9:77:D3", AssetStatus.Active, "", "", "Samsung Galaxy S21 Ultra 5G 12/256GB (phantom black)", new Guid("8DFD4C18-A1E5-4A34-906C-FE5F25F01FAB")));
            assets.Add(new MobilePhone(new Guid("945d827e-e2b8-4244-81db-27bfc503acc2"), OrganizationId1, CallerId, "", phone, "D1PSDX7X25VJ", "Apple", "iPhone 11", LifecycleType.NoLifecycle, new DateTime(2021, 10, 8), new Guid("49F4E089-FEED-4FE3-B5BC-719EB084F2BE"), new List<AssetImei>() { new AssetImei(549536515219674) }, "FC:03:9F:5C:31:C2", AssetStatus.Active, "", "", "iPhone 11 smarttelefon 128 GB (sort)", Guid.Empty));
            assets.Add(new Tablet(new Guid("0e1898ef-ff9c-4474-90cc-0762cf5647bc"), OrganizationId1, CallerId, "", tablet, "F9FDN4NKQ1GC", "Apple", "iPad Air", LifecycleType.NoLifecycle, new DateTime(2020, 7, 28), new Guid("16D973B5-4E57-4A26-858E-7EFA5B72695E"), new List<AssetImei>() { new AssetImei(502798806423077) }, "44:18:FD:D8:11:1D", AssetStatus.Active, "", "", "iPad Air (2020) 64 GB WiFi (rosegull)", Guid.Empty));
            assets.Add(new Tablet(new Guid("60806057-52d6-4443-bdfd-92b523e8ea98"), OrganizationId1, CallerId, "", tablet, "FSD23AKGG90", "Lenovo", "Tab M10", LifecycleType.NoLifecycle, new DateTime(2020, 12, 19), new Guid("C0C178F6-0A5A-46DD-8B0B-9129BF92CE01"), new List<AssetImei>() { new AssetImei(991500210767489) }, "9C:B1:56:CF:3F:48", AssetStatus.Active, "This asset should not have a leasing cost.", "", "Lenovo Tab M10 HD (2nd Gen) 10,1'' nettbrett", Guid.Empty));
            assets.Add(new MobilePhone(new Guid("0d5b2a2b-6482-468c-9806-e69cd554695b"), OrganizationId1, CallerId, "", phone, "DEDF4JJ51GC", "Sony", "Xperia 1 III", LifecycleType.NoLifecycle, new DateTime(2021, 12, 9), Guid.Empty, new List<AssetImei>() { new AssetImei(990384170861959), new AssetImei(549122924948565) }, "5B:F3:A3:03:97:1A", AssetStatus.Active, "", "", "Sony Xperia 1 III - 5G smarttelefon 12/256GB (frosted black)", new Guid("6B748D35-21FA-491D-ACE2-B8643A26AAF2")));
            assets.Add(new MobilePhone(new Guid("41d6e754-9758-4bda-83a0-9bcae06c7a0a"), OrganizationId1, CallerId, "", phone, "F9JSSX7X81EJ", "Apple", "iPhone 11", LifecycleType.NoLifecycle, new DateTime(2021, 4, 25), Guid.Empty, new List<AssetImei>() { new AssetImei(490639762437186), new AssetImei(107845616492344) }, "E5:BC:9F:8C:B9:CD", AssetStatus.Active, "", "", "iPhone 11 smarttelefon 128 GB (hvit)", Guid.Empty));
            assets.Add(new MobilePhone(new Guid("1462eb21-6f68-4220-83d6-e3e1932d8346"), OrganizationId1, CallerId, "", phone, "FUQZZ8R9EHA", "Apple", "iPhone 11", LifecycleType.NoLifecycle, new DateTime(2021, 3, 28), Guid.Empty, new List<AssetImei>() { new AssetImei(861835036738083) }, "D1:9C:4E:0E:21:0C", AssetStatus.Active, "", "", "iPhone 11 smarttelefon 128 GB (hvit)", Guid.Empty));
            assets.Add(new MobilePhone(new Guid("db3b39d9-dc78-41da-bdce-2df56c318070"), OrganizationId1, CallerId, "", phone, "FJC1C0BJK5G", "Samsung", "Galaxy S21", LifecycleType.NoLifecycle, new DateTime(2021, 10, 3), new Guid("0747105E-BE51-4BB9-A7C0-976C562A8A25"), new List<AssetImei>() { new AssetImei(987727049317519), new AssetImei(535252855568516) }, "35:22:FA:F8:7F:8F", AssetStatus.Active, "", "", "Samsung Galaxy S21 5G 8/128GB (phantom pink)", new Guid("CFF07875-3E86-4834-B066-9DA308F3EF05")));
            assets.Add(new MobilePhone(new Guid("2e9bf5cd-9d1d-43b4-84e6-c8f7632f0605"), OrganizationId1, CallerId, "", phone, "V00000JW0T0", "Apple", "iPhone 12 Mini", LifecycleType.NoLifecycle, new DateTime(2021, 1, 30), Guid.Empty, new List<AssetImei>() { new AssetImei(525453578370337), new AssetImei(351592473242643) }, "B4:A8:C1:61:6D:7A", AssetStatus.Active, "Remember to order a SIM card for this phone before the 21. of January", "", "iPhone 12 Mini - 5G smarttelefon 128 GB (hvit)", Guid.Empty));
            assets.Add(new Tablet(new Guid("eeaa0a01-c4b0-4740-879b-792e6122fa19"), OrganizationId1, CallerId, "", tablet, "XX0XY1XYX2XX", "Samsung", "Galaxy Tab A7 Lite", LifecycleType.NoLifecycle, new DateTime(2021, 6, 11), new Guid("71B8C4FE-B24B-40D9-93DB-67A4AE95932D"), new List<AssetImei>() { new AssetImei(546708765777346) }, "82:0A:5D:7C:AD:AB", AssetStatus.Active, "", "", "Samsung Galaxy Tab A7 Lite WiFi 8,7'' nettbrett (32 GB)", Guid.Empty));
            assets.Add(new MobilePhone(new Guid("dcc5dff3-a9fd-4abc-9424-9a66d828dad8"), OrganizationId1, CallerId, "", phone, "4J5CXDN3", "Apple", "iPhone 12 Mini", LifecycleType.NoLifecycle, new DateTime(2021, 3, 21), Guid.Empty, new List<AssetImei>() { new AssetImei(358408075666688), new AssetImei(911584253029525) }, "6E:77:55:00:FC:71", AssetStatus.Active, "This device is used by external contractors", "", "iPhone 12 Mini - 5G smarttelefon 128 GB (sort)", Guid.Empty));
            assets.Add(new MobilePhone(new Guid("39be18fe-5549-4aa9-be13-038783286a84"), OrganizationId1, CallerId, "", phone, "XpWkBPwN", "Samsung", "Galaxy S21", LifecycleType.NoLifecycle, new DateTime(2021, 9, 24), new Guid("100172A8-B885-4ABC-8F72-F37DA6F6E052"), new List<AssetImei>() { new AssetImei(357879702624426), new AssetImei(499408168621566) }, "6E:85:FC:5A:F9:1E", AssetStatus.Active, "", "", "Samsung Galaxy S21 Ultra 5G 12/256GB (phantom black)", Guid.Empty));
            assets.Add(new MobilePhone(new Guid("d2748169-cc69-44ef-833f-c9389cad1048"), OrganizationId1, CallerId, "", phone, "ACLNN5K9JAL", "Lenovo", "A7000-a", LifecycleType.NoLifecycle, new DateTime(2021, 11, 22), Guid.Empty, new List<AssetImei>() { new AssetImei(869045028978273) }, "1b:83:33:2e:a7:b0", AssetStatus.Active, "", "", "Lenovo A7000-a", new Guid("CFF07875-3E86-4834-B066-9DA308F3EF05")));
            assets.Add(new MobilePhone(new Guid("72a74c17-43ad-4066-a59c-747ccbf0e260"), OrganizationId1, CallerId, "", phone, "K88H8256HVT", "Samsung", "Note 20", LifecycleType.NoLifecycle, new DateTime(2021, 2, 2), new Guid("CC9F5DD4-FC5A-4CA2-9C96-91D40160714F"), new List<AssetImei>() { new AssetImei(868273044994436), new AssetImei(359042064800419) }, "70:58:91:a3:c9:a4", AssetStatus.Active, "", "", "Samsung Galaxy Note20 5G 256GB Mystic Gray", Guid.Empty));

            assets.Add(new MobilePhone(new Guid("dc10c4d6-3e26-4321-87d8-45d5b1ff8741"), OrganizationId2, CallerId, "", phone, "VJWZFSMZJKH", "Samsung", "Note 20", LifecycleType.NoLifecycle, new DateTime(2021, 2, 2), new Guid("0626E84E-B301-4714-8403-926805CB5B29"), new List<AssetImei>() { new AssetImei(359884912624420), new AssetImei(357879702624426) }, "43:fc:59:e8:93:2f", AssetStatus.Active, "Order new cover and screen protection by the end of the year.", "HR", "Samsung Galaxy Note20 5G 256GB Mystic Gray", new Guid("35374FE2-CC30-49B6-8A3A-7AFF4F0DB8DB")));
            assets.Add(new MobilePhone(new Guid("dd1a5864-1b9d-48d6-a507-c22696618597"), OrganizationId2, CallerId, "", phone, "P4CJ2UEV3V9", "Samsung", "Note 20", LifecycleType.NoLifecycle, new DateTime(2020, 11, 9), Guid.Empty, new List<AssetImei>() { new AssetImei(359054076994825), new AssetImei(861102032716690) }, "d3:61:da:c2:64:83", AssetStatus.Active, "", "", "Samsung Galaxy Note20 5G 256GB Mystic Blue", new Guid("35374FE2-CC30-49B6-8A3A-7AFF4F0DB8DB")));
            assets.Add(new Tablet(new Guid("86498f60-36d9-4e7d-b6c7-c0865568fef0"), OrganizationId2, CallerId, "", tablet, "AEA6SRSHUUC", "Apple", "iPad Pro 11", LifecycleType.NoLifecycle, new DateTime(2020, 9, 7), Guid.Empty, new List<AssetImei>() { new AssetImei(354995075297294) }, "cd:30:e7:77:d5:1b", AssetStatus.Active, "This iPad is used to order new equipment and services for the office.", "Office", "iPad Pro 11, 2021 512 GB WiFi (white)", new Guid("071A5D79-10F9-4A70-9CAD-7A7EA51E253A")));
            assets.Add(new MobilePhone(new Guid("a95c5cf4-dbbe-4fc9-8382-45f28bec62a6"), OrganizationId2, CallerId, "", phone, "X3SXZC4M6GE", "Apple", "iPhone 12 Mini", LifecycleType.NoLifecycle, new DateTime(2021, 10, 11), new Guid("4806344A-23E4-4DF0-AD71-7D8826C7EB6B"), new List<AssetImei>() { new AssetImei(358408075666688), new AssetImei(356348063779774) }, "6E:77:55:00:FC:71", AssetStatus.Active, "This device is used by external contractors", "", "iPhone 12 Mini - 5G smarttelefon 128 GB (hvit)", new Guid("515924FA-E94E-4281-AE13-8E04B049ED8D")));
            assets.Add(new MobilePhone(new Guid("f67b995d-7d52-46e5-8638-23a3dd824bd1"), OrganizationId2, CallerId, "", phone, "KRQVHBRSPJW", "Apple", "iPhone 12", LifecycleType.NoLifecycle, new DateTime(2021, 4, 23), new Guid("9F12135D-1AB1-4DB9-8F33-DE5385E57813"), new List<AssetImei>() { new AssetImei(354318103385421) }, "dc:6f:6b:ef:4b:b9", AssetStatus.Active, "", "", "iPhone 12 - 5G smarttelefon 65 GB (mystic gray)", new Guid("09ba661c-8231-42a9-9181-01b45435e8b9")));

            //assets.Add(new MobilePhone(new Guid("externalId"), OrganizationId1, CallerId, "alias", phone, "serialNumber", "brand", "productName", LifecycleType.NoLifecycle, new DateTime(2021, 1, 12), new Guid("assetholder"), new List<AssetImei>() { },"macAddress", AssetStatus.Active, "note", "assetTag", "description", new Guid("department")))

            return assets;
        }

        public static IList<CustomerLabel> GetCustomerLables(Guid organization)
        {
            List<CustomerLabel> customerLabels = new List<CustomerLabel>();
            if (organization == OrganizationId1)
            {
                customerLabels.Add(new CustomerLabel(new Guid("a184f7a3-e874-4a91-b886-c832f7ce69b4"), organization, CallerId, new Label("Security", LabelColor.Orange)));
                customerLabels.Add(new CustomerLabel(new Guid("d81a5260-f8ca-41fc-9b2d-764baff5677f"), organization, CallerId, new Label("IT-support", LabelColor.Blue)));
                customerLabels.Add(new CustomerLabel(new Guid("d722ced8-6352-4771-9d3b-2a9212dcec21"), organization, CallerId, new Label("External Contractor", LabelColor.Red)));
            }
            else if (organization == OrganizationId2)
            {
                customerLabels.Add(new CustomerLabel(new Guid("d9262c1d-3e8a-4485-8660-24b4903307f4"), organization, CallerId, new Label("Security", LabelColor.Orange)));
                customerLabels.Add(new CustomerLabel(new Guid("f3a915a5-211d-4164-a5b5-042e7b2ffe32"), organization, CallerId, new Label("External Contractor", LabelColor.Red)));
                customerLabels.Add(new CustomerLabel(new Guid("c7b9643e-b86c-43ee-a6f3-d028c07f8a43"), organization, CallerId, new Label("Marketing", LabelColor.Green)));
            }
            return customerLabels;
        }

        public static Dictionary<Guid, Guid> LabelsForAssets()
        {
            Dictionary<Guid, Guid> assetsAndLabels = new Dictionary<Guid, Guid>();
            assetsAndLabels.Add(new Guid("72a74c17-43ad-4066-a59c-747ccbf0e260"), new Guid("d81a5260-f8ca-41fc-9b2d-764baff5677f"));
            assetsAndLabels.Add(new Guid("60806057-52d6-4443-bdfd-92b523e8ea98"), new Guid("d81a5260-f8ca-41fc-9b2d-764baff5677f"));
            assetsAndLabels.Add(new Guid("2e9bf5cd-9d1d-43b4-84e6-c8f7632f0605"), new Guid("d81a5260-f8ca-41fc-9b2d-764baff5677f"));
            assetsAndLabels.Add(new Guid("dcc5dff3-a9fd-4abc-9424-9a66d828dad8"), new Guid("a184f7a3-e874-4a91-b886-c832f7ce69b4"));
            assetsAndLabels.Add(new Guid("eeaa0a01-c4b0-4740-879b-792e6122fa19"), new Guid("a184f7a3-e874-4a91-b886-c832f7ce69b4"));
            assetsAndLabels.Add(new Guid("945d827e-e2b8-4244-81db-27bfc503acc2"), new Guid("d722ced8-6352-4771-9d3b-2a9212dcec21"));
            assetsAndLabels.Add(new Guid("39be18fe-5549-4aa9-be13-038783286a84"), new Guid("d722ced8-6352-4771-9d3b-2a9212dcec21"));

            assetsAndLabels.Add(new Guid("dc10c4d6-3e26-4321-87d8-45d5b1ff8741"), new Guid("c7b9643e-b86c-43ee-a6f3-d028c07f8a43"));
            assetsAndLabels.Add(new Guid("a95c5cf4-dbbe-4fc9-8382-45f28bec62a6"), new Guid("c7b9643e-b86c-43ee-a6f3-d028c07f8a43"));
            assetsAndLabels.Add(new Guid("dd1a5864-1b9d-48d6-a507-c22696618597"), new Guid("d9262c1d-3e8a-4485-8660-24b4903307f4"));
            assetsAndLabels.Add(new Guid("f67b995d-7d52-46e5-8638-23a3dd824bd1"), new Guid("f3a915a5-211d-4164-a5b5-042e7b2ffe32"));

            return assetsAndLabels;
        }
    }
}
