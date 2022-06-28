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

        public static IList<AssetServices.Models.Asset> GetAssetData()
        {
            AssetCategory phone = new AssetCategory(1, null, null);
            AssetCategory tablet = new AssetCategory(2, null, null);
            IList<AssetServices.Models.Asset> assets = new List<AssetServices.Models.Asset>();
            assets.Add(new Tablet(new Guid("40a488fc-9ffa-4544-887f-956f577c116e"), CallerId, "YDMYF2LYHF0L", "Apple", "iPad Pro 11", new List<AssetImei>() { new AssetImei(336327301203750) }, "60:8B:0E:EF:18:83"));
            assets.Add(new MobilePhone(new Guid("d3f45808-d985-4cd8-895e-bde6ab9c087d"), CallerId, "RFCN80WJJ5K", "Samsung", "Galaxy S21", new List<AssetImei>() { new AssetImei(357879702624426), new AssetImei(103735096024176) }, "44:18:FD:C9:77:D3"));
            assets.Add(new MobilePhone(new Guid("945d827e-e2b8-4244-81db-27bfc503acc2"), CallerId, "D1PSDX7X25VJ", "Apple", "iPhone 11", new List<AssetImei>() { new AssetImei(549536515219674) }, "FC:03:9F:5C:31:C2"));
            assets.Add(new Tablet(new Guid("0e1898ef-ff9c-4474-90cc-0762cf5647bc"), CallerId, "F9FDN4NKQ1GC", "Apple", "iPad Air", new List<AssetImei>() { new AssetImei(502798806423077) }, "44:18:FD:D8:11:1D"));
            assets.Add(new Tablet(new Guid("60806057-52d6-4443-bdfd-92b523e8ea98"), CallerId, "FSD23AKGG90", "Lenovo", "Tab M10", new List<AssetImei>() { new AssetImei(991500210767489) }, "9C:B1:56:CF:3F:48"));
            assets.Add(new MobilePhone(new Guid("0d5b2a2b-6482-468c-9806-e69cd554695b"), CallerId, "DEDF4JJ51GC", "Sony", "Xperia 1 III", new List<AssetImei>() { new AssetImei(990384170861959), new AssetImei(549122924948565) }, "5B:F3:A3:03:97:1A"));
            assets.Add(new MobilePhone(new Guid("41d6e754-9758-4bda-83a0-9bcae06c7a0a"), CallerId, "F9JSSX7X81EJ", "Apple", "iPhone 11", new List<AssetImei>() { new AssetImei(490639762437186), new AssetImei(107845616492344) }, "E5:BC:9F:8C:B9:CD"));
            assets.Add(new MobilePhone(new Guid("1462eb21-6f68-4220-83d6-e3e1932d8346"), CallerId, "FUQZZ8R9EHA", "Apple", "iPhone 11", new List<AssetImei>() { new AssetImei(861835036738083) }, "D1:9C:4E:0E:21:0C"));
            assets.Add(new MobilePhone(new Guid("db3b39d9-dc78-41da-bdce-2df56c318070"), CallerId, "FJC1C0BJK5G", "Samsung", "Galaxy S21", new List<AssetImei>() { new AssetImei(987727049317519), new AssetImei(535252855568516) }, "35:22:FA:F8:7F:8F"));
            assets.Add(new MobilePhone(new Guid("2e9bf5cd-9d1d-43b4-84e6-c8f7632f0605"), CallerId, "V00000JW0T0", "Apple", "iPhone 12 Mini", new List<AssetImei>() { new AssetImei(525453578370337), new AssetImei(351592473242643) }, "B4:A8:C1:61:6D:7A"));
            assets.Add(new Tablet(new Guid("eeaa0a01-c4b0-4740-879b-792e6122fa19"), CallerId, "XX0XY1XYX2XX", "Samsung", "Galaxy Tab A7 Lite", new List<AssetImei>() { new AssetImei(546708765777346) }, "82:0A:5D:7C:AD:AB"));
            assets.Add(new MobilePhone(new Guid("dcc5dff3-a9fd-4abc-9424-9a66d828dad8"), CallerId, "4J5CXDN3", "Apple", "iPhone 12 Mini", new List<AssetImei>() { new AssetImei(358408075666688), new AssetImei(911584253029525) }, "6E:77:55:00:FC:71"));
            assets.Add(new MobilePhone(new Guid("39be18fe-5549-4aa9-be13-038783286a84"), CallerId, "XpWkBPwN", "Samsung", "Galaxy S21", new List<AssetImei>() { new AssetImei(357879702624426), new AssetImei(499408168621566) }, "6E:85:FC:5A:F9:1E"));
            assets.Add(new MobilePhone(new Guid("d2748169-cc69-44ef-833f-c9389cad1048"), CallerId, "ACLNN5K9JAL", "Lenovo", "A7000-a", new List<AssetImei>() { new AssetImei(869045028978273) }, "1b:83:33:2e:a7:b0"));
            assets.Add(new MobilePhone(new Guid("72a74c17-43ad-4066-a59c-747ccbf0e260"), CallerId, "K88H8256HVT", "Samsung", "Note 20", new List<AssetImei>() { new AssetImei(868273044994436), new AssetImei(359042064800419) }, "70:58:91:a3:c9:a4"));

            assets.Add(new MobilePhone(new Guid("dc10c4d6-3e26-4321-87d8-45d5b1ff8741"), CallerId, "VJWZFSMZJKH", "Samsung", "Note 20", new List<AssetImei>() { new AssetImei(359884912624420), new AssetImei(357879702624426) }, "43:fc:59:e8:93:2f"));
            assets.Add(new MobilePhone(new Guid("dd1a5864-1b9d-48d6-a507-c22696618597"), CallerId, "P4CJ2UEV3V9", "Samsung", "Note 20", new List<AssetImei>() { new AssetImei(359054076994825), new AssetImei(861102032716690) }, "d3:61:da:c2:64:83"));
            assets.Add(new Tablet(new Guid("86498f60-36d9-4e7d-b6c7-c0865568fef0"), CallerId, "AEA6SRSHUUC", "Apple", "iPad Pro 11", new List<AssetImei>() { new AssetImei(354995075297294) }, "cd:30:e7:77:d5:1b"));
            assets.Add(new MobilePhone(new Guid("a95c5cf4-dbbe-4fc9-8382-45f28bec62a6"), CallerId, "X3SXZC4M6GE", "Apple", "iPhone 12 Mini", new List<AssetImei>() { new AssetImei(358408075666688), new AssetImei(356348063779774) }, "6E:77:55:00:FC:71"));
            assets.Add(new MobilePhone(new Guid("f67b995d-7d52-46e5-8638-23a3dd824bd1"), CallerId, "KRQVHBRSPJW", "Apple", "iPhone 12", new List<AssetImei>() { new AssetImei(354318103385421) }, "dc:6f:6b:ef:4b:b9"));

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
