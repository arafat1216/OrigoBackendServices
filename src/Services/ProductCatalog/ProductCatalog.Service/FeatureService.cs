﻿using Microsoft.EntityFrameworkCore;
using ProductCatalog.Service.Infrastructure;
using ProductCatalog.Service.Infrastructure.Context;
using ProductCatalog.Service.Models.Database;

namespace ProductCatalog.Service
{
    public class FeatureService
    {
        //private readonly ProductCatalogContext _context;
        private readonly UnitOfWork _unitOfWork;

        public FeatureService()
        {
            string[] args = Array.Empty<string>();
            var context = new ProductCatalogContextFactory().CreateDbContext(args);

            //_context = context;
            _unitOfWork = new UnitOfWork(context);
        }

        // TODO: Take a look at this. We may need it for unit-testing and service runtime injection registration.
        internal FeatureService(ProductCatalogContext context)
        {
            //_context = context;
            _unitOfWork = new UnitOfWork(context);
        }

        // TODO: Remove
        public async Task<IEnumerable<Feature>> Test()
        {
            return await _unitOfWork.Features.GetAllAsync();
        }

        
        public async Task<IEnumerable<string>> GetPermissionNodesByOrganization(Guid organizationId)
        {
            return await _unitOfWork.Features.GetPermissionNodesByOrganizationAsync(organizationId);
        }
        

    }
}
