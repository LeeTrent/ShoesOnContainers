using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductCatalogApi.Domain;

namespace ProductCatalogApi.Data
{
    public class CatalogContext:DbContext
    {
        public DbSet<CatalogType>   CatalogTypes    { get; set; }
        public DbSet<CatalogBrand>  CatalogBrands   { get; set; }
        public DbSet<CatalogItem>   CatalogItems    { get; set; }

        public CatalogContext(DbContextOptions options):base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<CatalogBrand>(ConfigurationCatalogBrand);
            builder.Entity<CatalogType>(ConfigurationCatalogType);
            builder.Entity<CatalogItem>(ConfigurationCatalogItem);
        }

        private void ConfigurationCatalogItem(EntityTypeBuilder<CatalogItem> builder)
        {
            builder.ToTable("Catalog");

            builder.Property(c => c.Id)
                   .ForSqlServerUseSequenceHiLo("catalog_hilo")
                   .IsRequired(true);
            
            builder.Property(c => c.Name)
                   .IsRequired(true)
                   .HasMaxLength(50);

            builder.Property(c => c.Price)
                   .IsRequired(true);     

            builder.Property(c => c.PictureUrl)
                   .IsRequired(false);

            builder.HasOne(c => c.CatalogBrand)
                   .WithMany()
                   .HasForeignKey(c => c.CatalogBrandId);

            builder.HasOne(c => c.CatalogType)
                   .WithMany()
                   .HasForeignKey(c => c.CatalogTypeId);
        }

        private void ConfigurationCatalogType(EntityTypeBuilder<CatalogType> builder)
        {
            builder.ToTable("CatalogType");

            builder.Property(c => c.Id)
                   .ForSqlServerUseSequenceHiLo("catalog_type_hilo")
                   .IsRequired(true);

            builder.Property(c => c.Type)
                   .IsRequired(true)
                   .HasMaxLength(100);
        }

        private void ConfigurationCatalogBrand(EntityTypeBuilder<CatalogBrand> builder)
        {
            builder.ToTable("CatalogBrand");

            builder.Property(c => c.Id)
                   .ForSqlServerUseSequenceHiLo("catalog_brand_hilo")
                   .IsRequired(true);

            builder.Property(c => c.Brand)
                   .IsRequired(true)
                   .HasMaxLength(100);
            
        }
    }
}
