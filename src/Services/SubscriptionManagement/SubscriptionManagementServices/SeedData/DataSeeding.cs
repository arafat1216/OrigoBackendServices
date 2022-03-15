using Microsoft.EntityFrameworkCore;
using SubscriptionManagementServices.Models;

namespace SubscriptionManagementServices.SeedData
{
    public static class DataSeeding
    {
        public static void SeedGlobalProducts(this ModelBuilder modelBuilder)
        {
            Guid generatedDataGuid = Guid.Parse("00000000-0000-0000-0000-000000000000");
            
            var bedriftFriPlusDP = new List<Object>();

            modelBuilder.Entity<DataPackage>(e =>
            {
                #region Telenor
                #region Bedrift fri +
                e.HasData(new { Id = 1, DataPackageName = "1 GB", SubscriptionProductId = 1, CreatedBy = generatedDataGuid, DeletedBy = generatedDataGuid, UpdatedBy = generatedDataGuid, IsDeleted = false});
                e.HasData(new { Id = 2, DataPackageName = "4 GB", SubscriptionProductId = 1, CreatedBy = generatedDataGuid, DeletedBy = generatedDataGuid, UpdatedBy = generatedDataGuid, IsDeleted = false });
                e.HasData(new { Id = 3, DataPackageName = "8 GB", SubscriptionProductId = 1, CreatedBy = generatedDataGuid, DeletedBy = generatedDataGuid, UpdatedBy = generatedDataGuid, IsDeleted = false });
                e.HasData(new { Id = 4, DataPackageName = "20 GB",SubscriptionProductId = 1, CreatedBy = generatedDataGuid, DeletedBy = generatedDataGuid, UpdatedBy = generatedDataGuid, IsDeleted = false });
                e.HasData(new { Id = 5, DataPackageName = "40 GB",SubscriptionProductId = 1, CreatedBy = generatedDataGuid, DeletedBy = generatedDataGuid, UpdatedBy = generatedDataGuid, IsDeleted = false });
                e.HasData(new { Id = 6, DataPackageName = "60 GB",SubscriptionProductId = 1, CreatedBy = generatedDataGuid, DeletedBy = generatedDataGuid, UpdatedBy = generatedDataGuid, IsDeleted = false });
                #endregion
                #region Bedrift total
                e.HasData(new { Id = 7, DataPackageName = "Surf 1 GB",  SubscriptionProductId = 3, CreatedBy = generatedDataGuid, DeletedBy = generatedDataGuid, UpdatedBy = generatedDataGuid, IsDeleted = false });
                e.HasData(new { Id = 8, DataPackageName = "Surf 2 GB",  SubscriptionProductId = 3, CreatedBy = generatedDataGuid, DeletedBy = generatedDataGuid, UpdatedBy = generatedDataGuid, IsDeleted = false });
                e.HasData(new { Id = 9, DataPackageName = "Surf 3 GB",  SubscriptionProductId = 3, CreatedBy = generatedDataGuid, DeletedBy = generatedDataGuid, UpdatedBy = generatedDataGuid, IsDeleted = false });
                e.HasData(new { Id = 10,DataPackageName = "Surf 5 GB",  SubscriptionProductId = 3, CreatedBy = generatedDataGuid, DeletedBy = generatedDataGuid, UpdatedBy = generatedDataGuid, IsDeleted = false });
                e.HasData(new { Id = 11,DataPackageName = "Surf 10 GB", SubscriptionProductId = 3, CreatedBy = generatedDataGuid, DeletedBy = generatedDataGuid, UpdatedBy = generatedDataGuid, IsDeleted = false });
                e.HasData(new { Id = 12,DataPackageName = "Surf 15 GB", SubscriptionProductId = 3, CreatedBy = generatedDataGuid, DeletedBy = generatedDataGuid, UpdatedBy = generatedDataGuid, IsDeleted = false });
                e.HasData(new { Id = 13,DataPackageName = "Surf 25 GB", SubscriptionProductId = 3,CreatedBy = generatedDataGuid, DeletedBy = generatedDataGuid, UpdatedBy = generatedDataGuid, IsDeleted = false });
                e.HasData(new { Id = 14,DataPackageName = "Surf 35 GB", SubscriptionProductId = 3, CreatedBy = generatedDataGuid, DeletedBy = generatedDataGuid, UpdatedBy = generatedDataGuid, IsDeleted = false });
                e.HasData(new { Id = 15,DataPackageName = "Surf 50 GB", SubscriptionProductId = 3, CreatedBy = generatedDataGuid, DeletedBy = generatedDataGuid, UpdatedBy = generatedDataGuid, IsDeleted = false });
                e.HasData(new { Id = 16,DataPackageName = "Surf 75 GB", SubscriptionProductId = 3, CreatedBy = generatedDataGuid, DeletedBy = generatedDataGuid, UpdatedBy = generatedDataGuid, IsDeleted = false });
                e.HasData(new { Id = 17,DataPackageName = "Surf 100 GB",SubscriptionProductId = 3, CreatedBy = generatedDataGuid, DeletedBy = generatedDataGuid, UpdatedBy = generatedDataGuid, IsDeleted = false });
                e.HasData(new { Id = 18, DataPackageName = "Surf 150 GB", SubscriptionProductId = 3, CreatedBy = generatedDataGuid, DeletedBy = generatedDataGuid, UpdatedBy = generatedDataGuid, IsDeleted = false });
                #endregion
                #region Bedrift total extra
                e.HasData(new { Id = 19, DataPackageName = "Surf 200 MB",SubscriptionProductId = 4, CreatedBy = generatedDataGuid, DeletedBy = generatedDataGuid, UpdatedBy = generatedDataGuid, IsDeleted = false });
                e.HasData(new { Id = 20, DataPackageName = "Surf 1 GB",  SubscriptionProductId = 4, CreatedBy = generatedDataGuid, DeletedBy = generatedDataGuid, UpdatedBy = generatedDataGuid, IsDeleted = false });
                e.HasData(new { Id = 21, DataPackageName = "Surf 3 GB",  SubscriptionProductId = 4, CreatedBy = generatedDataGuid, DeletedBy = generatedDataGuid, UpdatedBy = generatedDataGuid, IsDeleted = false });
                e.HasData(new { Id = 22, DataPackageName = "Surf 5 GB",  SubscriptionProductId = 4, CreatedBy = generatedDataGuid, DeletedBy = generatedDataGuid, UpdatedBy = generatedDataGuid, IsDeleted = false });
                e.HasData(new { Id = 23, DataPackageName = "Surf 10 GB", SubscriptionProductId = 4, CreatedBy = generatedDataGuid, DeletedBy = generatedDataGuid, UpdatedBy = generatedDataGuid, IsDeleted = false });
                e.HasData(new { Id = 24, DataPackageName = "Surf 15 GB", SubscriptionProductId = 4, CreatedBy = generatedDataGuid, DeletedBy = generatedDataGuid, UpdatedBy = generatedDataGuid, IsDeleted = false });
                e.HasData(new { Id = 25, DataPackageName = "Surf 25 GB", SubscriptionProductId = 4, CreatedBy = generatedDataGuid, DeletedBy = generatedDataGuid, UpdatedBy = generatedDataGuid, IsDeleted = false });
                e.HasData(new { Id = 26, DataPackageName = "Surf 35 GB", SubscriptionProductId = 4, CreatedBy = generatedDataGuid, DeletedBy = generatedDataGuid, UpdatedBy = generatedDataGuid, IsDeleted = false });
                e.HasData(new { Id = 27, DataPackageName = "Surf 50 GB", SubscriptionProductId = 4, CreatedBy = generatedDataGuid, DeletedBy = generatedDataGuid, UpdatedBy = generatedDataGuid, IsDeleted = false });
                e.HasData(new { Id = 28, DataPackageName = "Surf 75 GB", SubscriptionProductId = 4, CreatedBy = generatedDataGuid, DeletedBy = generatedDataGuid, UpdatedBy = generatedDataGuid, IsDeleted = false });
                e.HasData(new { Id = 29, DataPackageName = "Surf 100 GB",SubscriptionProductId = 4, CreatedBy = generatedDataGuid, DeletedBy = generatedDataGuid, UpdatedBy = generatedDataGuid, IsDeleted = false });
                #endregion
                #region Bedrift small
                e.HasData(new { Id = 30, DataPackageName = "100 MB", SubscriptionProductId = 5, CreatedBy = generatedDataGuid, DeletedBy = generatedDataGuid, UpdatedBy = generatedDataGuid, IsDeleted = false });
                e.HasData(new { Id = 31, DataPackageName = "200 MB", SubscriptionProductId = 5, CreatedBy = generatedDataGuid, DeletedBy = generatedDataGuid, UpdatedBy = generatedDataGuid, IsDeleted = false });
                #endregion
                #region Bedrift total A
                e.HasData(new { Id = 32, DataPackageName = "10 GB", SubscriptionProductId = 6, CreatedBy = generatedDataGuid, DeletedBy = generatedDataGuid, UpdatedBy = generatedDataGuid, IsDeleted = false });
                e.HasData(new { Id = 33, DataPackageName = "15 GB", SubscriptionProductId = 6, CreatedBy = generatedDataGuid, DeletedBy = generatedDataGuid, UpdatedBy = generatedDataGuid, IsDeleted = false });
                e.HasData(new { Id = 34, DataPackageName = "20 GB", SubscriptionProductId = 6, CreatedBy = generatedDataGuid, DeletedBy = generatedDataGuid, UpdatedBy = generatedDataGuid, IsDeleted = false });
                #endregion
                #region Bedrift total B
                e.HasData(new { Id = 35, DataPackageName = "25 GB", SubscriptionProductId = 7, CreatedBy = generatedDataGuid, DeletedBy = generatedDataGuid, UpdatedBy = generatedDataGuid, IsDeleted = false });
                e.HasData(new { Id = 36, DataPackageName = "35 GB", SubscriptionProductId = 7, CreatedBy = generatedDataGuid, DeletedBy = generatedDataGuid, UpdatedBy = generatedDataGuid, IsDeleted = false });
                e.HasData(new { Id = 37, DataPackageName = "40 GB", SubscriptionProductId = 7, CreatedBy = generatedDataGuid, DeletedBy = generatedDataGuid, UpdatedBy = generatedDataGuid, IsDeleted = false });
                #endregion
                #region Bedrift total C
                e.HasData(new { Id = 38, DataPackageName = "30 GB", SubscriptionProductId = 8, CreatedBy = generatedDataGuid, DeletedBy = generatedDataGuid, UpdatedBy = generatedDataGuid, IsDeleted = false });
                e.HasData(new { Id = 39, DataPackageName = "50 GB", SubscriptionProductId = 8, CreatedBy = generatedDataGuid, DeletedBy = generatedDataGuid, UpdatedBy = generatedDataGuid, IsDeleted = false });
                e.HasData(new { Id = 40, DataPackageName = "100 GB", SubscriptionProductId = 8, CreatedBy = generatedDataGuid, DeletedBy = generatedDataGuid, UpdatedBy = generatedDataGuid, IsDeleted = false });
                #endregion
                #region Bedrift total D
                e.HasData(new { Id = 41, DataPackageName = "60 GB", SubscriptionProductId = 9, CreatedBy = generatedDataGuid, DeletedBy = generatedDataGuid, UpdatedBy = generatedDataGuid, IsDeleted = false });
                #endregion
                #region Bedrift total E
                e.HasData(new { Id = 42, DataPackageName = "100 GB", SubscriptionProductId = 10, CreatedBy = generatedDataGuid, DeletedBy = generatedDataGuid, UpdatedBy = generatedDataGuid, IsDeleted = false });
                #endregion
                #region Bedrift total+ A
                e.HasData(new { Id = 43, DataPackageName = "5 GB", SubscriptionProductId = 11, CreatedBy = generatedDataGuid, DeletedBy = generatedDataGuid, UpdatedBy = generatedDataGuid, IsDeleted = false });
                #endregion
                #region Bedrift total+ B
                e.HasData(new { Id = 44, DataPackageName = "10 GB", SubscriptionProductId = 12, CreatedBy = generatedDataGuid, DeletedBy = generatedDataGuid, UpdatedBy = generatedDataGuid, IsDeleted = false });
                #endregion
                #region Bedrift total+ C
                e.HasData(new { Id = 45, DataPackageName = "15 GB", SubscriptionProductId = 13, CreatedBy = generatedDataGuid, DeletedBy = generatedDataGuid, UpdatedBy = generatedDataGuid, IsDeleted = false });
                #endregion
                #region Bedrift total+ D
                e.HasData(new { Id = 46, DataPackageName = "100 GB", SubscriptionProductId = 14, CreatedBy = generatedDataGuid, DeletedBy = generatedDataGuid, UpdatedBy = generatedDataGuid, IsDeleted = false });
                #endregion
                #endregion

                #region Telia
                #region Telia Smart
                e.HasData(new { Id = 47, DataPackageName = "Avtalepris S", SubscriptionProductId = 18, CreatedBy = generatedDataGuid, DeletedBy = generatedDataGuid, UpdatedBy = generatedDataGuid, IsDeleted = false });
                e.HasData(new { Id = 48, DataPackageName = "Avtalepris small+", SubscriptionProductId = 18, CreatedBy = generatedDataGuid, DeletedBy = generatedDataGuid, UpdatedBy = generatedDataGuid, IsDeleted = false });
                e.HasData(new { Id = 49, DataPackageName = "Avtalepris M", SubscriptionProductId = 18, CreatedBy = generatedDataGuid, DeletedBy = generatedDataGuid, UpdatedBy = generatedDataGuid, IsDeleted = false });
                e.HasData(new { Id = 50, DataPackageName = "Avtalepris Basis", SubscriptionProductId = 18, CreatedBy = generatedDataGuid, DeletedBy = generatedDataGuid, UpdatedBy = generatedDataGuid, IsDeleted = false });
                e.HasData(new { Id = 51, DataPackageName = "Avtalepris Fri", SubscriptionProductId = 18, CreatedBy = generatedDataGuid, DeletedBy = generatedDataGuid, UpdatedBy = generatedDataGuid, IsDeleted = false });
                e.HasData(new { Id = 52, DataPackageName = "Avtalepris Respons", SubscriptionProductId = 18, CreatedBy = generatedDataGuid, DeletedBy = generatedDataGuid, UpdatedBy = generatedDataGuid, IsDeleted = false });
                e.HasData(new { Id = 53, DataPackageName = "Avtalepris Variabel", SubscriptionProductId = 18, CreatedBy = generatedDataGuid, DeletedBy = generatedDataGuid, UpdatedBy = generatedDataGuid, IsDeleted = false });
                #endregion
                #region Telia Smart Avtalepris L
                e.HasData(new { Id = 54, DataPackageName = "10 GB", SubscriptionProductId = 19, CreatedBy = generatedDataGuid, DeletedBy = generatedDataGuid, UpdatedBy = generatedDataGuid, IsDeleted = false });
                e.HasData(new { Id = 55, DataPackageName = "15 GB",SubscriptionProductId = 19, CreatedBy = generatedDataGuid, DeletedBy = generatedDataGuid, UpdatedBy = generatedDataGuid, IsDeleted = false });
                e.HasData(new { Id = 56, DataPackageName = "30 GB",SubscriptionProductId = 19, CreatedBy = generatedDataGuid, DeletedBy = generatedDataGuid, UpdatedBy = generatedDataGuid, IsDeleted = false });
                e.HasData(new { Id = 57, DataPackageName = "50 GB",SubscriptionProductId = 19, CreatedBy = generatedDataGuid, DeletedBy = generatedDataGuid, UpdatedBy = generatedDataGuid, IsDeleted = false });
                #endregion
                #region Datapackages Mobilt bredbånd
                e.HasData(new { Id = 58, DataPackageName = "5 GB", SubscriptionProductId = 20, CreatedBy = generatedDataGuid, DeletedBy = generatedDataGuid, UpdatedBy = generatedDataGuid, IsDeleted = false });
                e.HasData(new { Id = 59, DataPackageName = "20 GB", SubscriptionProductId = 20, CreatedBy = generatedDataGuid, DeletedBy = generatedDataGuid, UpdatedBy = generatedDataGuid, IsDeleted = false });
                e.HasData(new { Id = 60, DataPackageName = "40 GB", SubscriptionProductId = 20, CreatedBy = generatedDataGuid, DeletedBy = generatedDataGuid, UpdatedBy = generatedDataGuid, IsDeleted = false });
                e.HasData(new { Id = 61, DataPackageName = "80 GB", SubscriptionProductId = 20, CreatedBy = generatedDataGuid, DeletedBy = generatedDataGuid, UpdatedBy = generatedDataGuid, IsDeleted = false });
                e.HasData(new { Id = 62, DataPackageName = "150 GB", SubscriptionProductId = 20, CreatedBy = generatedDataGuid, DeletedBy = generatedDataGuid, UpdatedBy = generatedDataGuid, IsDeleted = false });
                e.HasData(new { Id = 63, DataPackageName = "500 GB", SubscriptionProductId = 20, CreatedBy = generatedDataGuid, DeletedBy = generatedDataGuid, UpdatedBy = generatedDataGuid, IsDeleted = false });
                #endregion
                #endregion

            });

            

            modelBuilder.Entity<SubscriptionProduct>(e =>
            {
                #region Telenor
                e.HasData(new { Id = 1, SubscriptionName = "Bedrift fri +", OperatorId = 3, CreatedBy = generatedDataGuid, DeletedBy = generatedDataGuid, UpdatedBy = generatedDataGuid, IsDeleted = false });
                e.HasData(new { Id = 2, SubscriptionName = "Bedrift fri L", OperatorId = 3, CreatedBy = generatedDataGuid, DeletedBy = generatedDataGuid, UpdatedBy = generatedDataGuid, IsDeleted = false });
                e.HasData(new { Id = 3, SubscriptionName = "Bedrift total", OperatorId = 3, CreatedBy = generatedDataGuid, DeletedBy = generatedDataGuid, UpdatedBy = generatedDataGuid, IsDeleted = false });
                e.HasData(new { Id = 4, SubscriptionName = "Bedrift total extra", OperatorId = 3, CreatedBy = generatedDataGuid, DeletedBy = generatedDataGuid, UpdatedBy = generatedDataGuid, IsDeleted = false });
                e.HasData(new { Id = 5, SubscriptionName = "Bedrift small", OperatorId = 3, CreatedBy = generatedDataGuid, DeletedBy = generatedDataGuid, UpdatedBy = generatedDataGuid, IsDeleted = false });
                e.HasData(new { Id = 6, SubscriptionName = "Bedrift total A", OperatorId = 3, CreatedBy = generatedDataGuid, DeletedBy = generatedDataGuid, UpdatedBy = generatedDataGuid, IsDeleted = false });
                e.HasData(new { Id = 7, SubscriptionName = "Bedrift total B", OperatorId = 3, CreatedBy = generatedDataGuid, DeletedBy = generatedDataGuid, UpdatedBy = generatedDataGuid, IsDeleted = false });
                e.HasData(new { Id = 8, SubscriptionName = "Bedrift total C", OperatorId = 3, CreatedBy = generatedDataGuid, DeletedBy = generatedDataGuid, UpdatedBy = generatedDataGuid, IsDeleted = false });
                e.HasData(new { Id = 9, SubscriptionName = "Bedrift total D", OperatorId = 3, CreatedBy = generatedDataGuid, DeletedBy = generatedDataGuid, UpdatedBy = generatedDataGuid, IsDeleted = false });
                e.HasData(new { Id = 10, SubscriptionName = "Bedrift total E", OperatorId = 3, CreatedBy = generatedDataGuid, DeletedBy = generatedDataGuid, UpdatedBy = generatedDataGuid, IsDeleted = false });
                e.HasData(new { Id = 11, SubscriptionName = "Bedrift total+ A", OperatorId = 3, CreatedBy = generatedDataGuid, DeletedBy = generatedDataGuid, UpdatedBy = generatedDataGuid, IsDeleted = false });
                e.HasData(new { Id = 12, SubscriptionName = "Bedrift total+ B", OperatorId = 3, CreatedBy = generatedDataGuid, DeletedBy = generatedDataGuid, UpdatedBy = generatedDataGuid, IsDeleted = false });
                e.HasData(new { Id = 13, SubscriptionName = "Bedrift total+ C", OperatorId = 3, CreatedBy = generatedDataGuid, DeletedBy = generatedDataGuid, UpdatedBy = generatedDataGuid, IsDeleted = false });
                e.HasData(new { Id = 14, SubscriptionName = "Bedrift total+ D", OperatorId = 3, CreatedBy = generatedDataGuid, DeletedBy = generatedDataGuid, UpdatedBy = generatedDataGuid, IsDeleted = false });
                e.HasData(new { Id = 15, SubscriptionName = "Bedrift flyt M", OperatorId = 3, CreatedBy = generatedDataGuid, DeletedBy = generatedDataGuid, UpdatedBy = generatedDataGuid, IsDeleted = false });
                e.HasData(new { Id = 16, SubscriptionName = "Bedrift flyt S", OperatorId = 3, CreatedBy = generatedDataGuid, DeletedBy = generatedDataGuid, UpdatedBy = generatedDataGuid, IsDeleted = false });
                e.HasData(new { Id = 17, SubscriptionName = "Bedrift XS", OperatorId = 3, CreatedBy = generatedDataGuid, DeletedBy = generatedDataGuid, UpdatedBy = generatedDataGuid, IsDeleted = false });
                #endregion
                #region Telia
                e.HasData(new { Id = 18, SubscriptionName = "Telia smart", OperatorId = 1, CreatedBy = generatedDataGuid, DeletedBy = generatedDataGuid, UpdatedBy = generatedDataGuid, IsDeleted = false });
                e.HasData(new { Id = 19, SubscriptionName = "Telia Smart Avtalepris L", OperatorId = 1, CreatedBy = generatedDataGuid, DeletedBy = generatedDataGuid, UpdatedBy = generatedDataGuid, IsDeleted = false });
                e.HasData(new { Id = 20, SubscriptionName = "Mobilt bredbånd", OperatorId = 1, CreatedBy = generatedDataGuid, DeletedBy = generatedDataGuid, UpdatedBy = generatedDataGuid, IsDeleted = false });
                e.HasData(new { Id = 21, SubscriptionName = "Telia Ubegrenset", OperatorId = 1, CreatedBy = generatedDataGuid, DeletedBy = generatedDataGuid, UpdatedBy = generatedDataGuid, IsDeleted = false });

                #endregion

            });
        }

    }
}
