using ProductCatalog.Infrastructure.Infrastructure.Context;
using ProductCatalog.Infrastructure.Infrastructure;
using ProductCatalog.Infrastructure.Models.Database;

namespace ProductCatalog.Infrastructure
{
    public class PopulateData
    {
        private readonly UnitOfWork _unitOfWork;

        public PopulateData()
        {
            string[] args = Array.Empty<string>();
            var context = new ProductCatalogContextFactory().CreateDbContext(args);

            _unitOfWork = new UnitOfWork(context);
        }


#pragma warning disable CS0618 // Type or member is obsolete
        public async Task PopulateDummyData()
        {
            Guid guid1 = Guid.Empty;
            Guid guid2 = Guid.NewGuid();
            Guid guid3 = Guid.NewGuid();


            FeatureType featureType1 = new()
            {
                Translations = new List<FeatureTypeTranslation>
                {
                    new FeatureTypeTranslation("en", "Feature Type 1", null, guid1),
                    new FeatureTypeTranslation("nb", "Feature Type 1", null, guid2)
                },
                UpdatedBy = guid1
            };

            FeatureType featureType2 = new()
            {
                Translations = new List<FeatureTypeTranslation>
                {
                    new FeatureTypeTranslation("en", "Feature Type 2", null, guid1),
                    new FeatureTypeTranslation("nb", "Feature Type 2", null, guid1)
                },
                UpdatedBy = guid1
            };

            await _unitOfWork.FeaturesTypes.AddAsync(featureType1);
            await _unitOfWork.FeaturesTypes.AddAsync(featureType2);

            await _unitOfWork.SaveAsync();

            Feature feature1 = new()
            {
                FeatureTypeId = featureType1.Id,
                UpdatedBy = guid3,
                AccessControlPermissionNode = "Feature1ControlNode",
                Translations = new List<FeatureTranslation>
                {
                    new FeatureTranslation("en", "Feature 1", null, guid3),
                    new FeatureTranslation("nb", "Feature 1", null, guid3),
                }
            };

            Feature feature2 = new()
            {
                FeatureTypeId = featureType2.Id,
                UpdatedBy = guid2,
                AccessControlPermissionNode = "Feature2ControlNode",
                Translations = new List<FeatureTranslation>
                {
                    new FeatureTranslation("en", "Feature 1", null, guid2),
                    new FeatureTranslation("nb", "Feature 1", null, guid1),
                }
            };

            await _unitOfWork.Features.AddAsync(feature1);
            await _unitOfWork.Features.AddAsync(feature2);

            await _unitOfWork.SaveAsync();


            ProductType productType1 = new()
            {
                UpdatedBy = guid2,
                Translations = new List<ProductTypeTranslation>()
                {
                    new ProductTypeTranslation("en", "Product Type 1", null, guid1),
                    new ProductTypeTranslation("nb", "Produkt Type 1", null, guid1),
                }
            };

            ProductType productType2 = new()
            {
                UpdatedBy = guid3,
                Translations = new List<ProductTypeTranslation>()
                {
                    new ProductTypeTranslation("en", "Product Type 2", null, guid3),
                    new ProductTypeTranslation("nb", "Produkt Type 2", null, guid3),
                }
            };

            await _unitOfWork.ProductTypes.AddAsync(productType1);
            await _unitOfWork.ProductTypes.AddAsync(productType2);

            await _unitOfWork.SaveAsync();


            Product product1 = new()
            {
                PartnerId = guid1,
                ProductTypeId = productType2.Id,
                UpdatedBy = guid2,
                Translations = new List<ProductTranslation>
                {
                    new ProductTranslation("en", "Product 1", null, guid2),
                    new ProductTranslation("nb", "Produkt 1", null, guid2),
                },
                Features = new List<Feature>
                {

                },
            };

            Product product2 = new()
            {
                PartnerId = guid1,
                ProductTypeId = productType1.Id,
                UpdatedBy = guid2,
                Translations = new List<ProductTranslation>
                {
                    new ProductTranslation("en", "Product 2", null, guid3),
                    new ProductTranslation("nb", "Produkt 2", null, guid2),
                },
                Features = new List<Feature>
                {
                    feature1
                },
            };

            Product product3 = new()
            {
                PartnerId = guid2,
                ProductTypeId = productType2.Id,
                UpdatedBy = guid1,
                Translations = new List<ProductTranslation>
                {
                    new ProductTranslation("en", "Product 2", null, guid1),
                    new ProductTranslation("nb", "Produkt 2", null, guid1),
                },
                Features = new List<Feature>
                {
                    feature1,
                    feature2,
                }
            };

            await _unitOfWork.Products.AddAsync(product1);
            await _unitOfWork.Products.AddAsync(product2);
            await _unitOfWork.Products.AddAsync(product3);

            await _unitOfWork.SaveAsync();


            Order order1 = new()
            {
                OrganizationId = guid1,
                ProductId = product1.Id,
                UpdatedBy = guid1,
            };

            Order order2 = new()
            {
                OrganizationId = guid2,
                ProductId = product1.Id,
                UpdatedBy = guid1,
            };

            Order order3 = new()
            {
                OrganizationId = guid2,
                ProductId = product2.Id,
                UpdatedBy = guid2,
            };

            await _unitOfWork.Orders.AddAsync(order1);
            await _unitOfWork.Orders.AddAsync(order2);
            await _unitOfWork.Orders.AddAsync(order3);

            await _unitOfWork.SaveAsync();
        }
#pragma warning restore CS0618 // Type or member is obsolete
    }
}
