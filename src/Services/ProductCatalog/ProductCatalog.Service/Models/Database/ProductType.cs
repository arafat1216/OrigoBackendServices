﻿using ProductCatalog.Service.Models.Boilerplate;
using ProductCatalog.Service.Models.Database.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductCatalog.Service.Models.Database
{
    public class ProductType : Entity, IBaseType
    {
        public int Id { get; set; }

        public virtual ICollection<Translation> Translations { get; set; } = new HashSet<Translation>();

        // EF Navigation
        public virtual IReadOnlyCollection<Product> Products { get; set; } = new HashSet<Product>();
    }
}
