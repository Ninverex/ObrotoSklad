using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ObrotoSklad.Application.Common;
using ObrotoSklad.Domain;
using ObrotoSklad.Domain.Entities;


namespace ObrotoSklad.Infrastructure.Database;

public class AppDbContext : IdentityDbContext<User>, IAppDbContext
{
    public AppDbContext (
        DbContextOptions<AppDbContext> options
    ) : base (options) { }

    public DbSet<Product> Products { get; set; }
    public DbSet<StockItem> StockItems { get; set; }
    public DbSet<StockMovement> StockMovement { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItem { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Invoice> Invoices { get; set; }
    public DbSet<InvoiceItem> InvoiceItems { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        builder.Entity<Product>(entity =>
        {
            entity.Property(p => p.SKU)
                .IsRequired()
                .HasMaxLength(50);
            
            entity.HasIndex(p => p.SKU)
                .IsUnique();
            
            entity.Property(p => p.Name) 
                .IsRequired()
                .HasMaxLength(200);
            
            entity.Property(p => p.Description) 
                .IsRequired()
                .HasMaxLength(1000);
            
            entity.Property(p => p.Unit)
                .IsRequired()
                .HasMaxLength(20);
            
            entity.Property (p => p.Price)
                .HasColumnType("decimal(18,2)");
        });

        builder.Entity<StockItem>(entity =>
        {
           entity.HasKey(si => si.ProductId);

           entity.HasOne(si => si.Product)
                .WithOne()
                .HasForeignKey<StockItem>(si => si.ProductId);
        });

        builder.Entity<StockMovement>(entity =>
        {
            entity.HasKey(sm => sm.Id);

            entity.Property(sm => sm.Type)
                .IsRequired();

             entity.Property(sm => sm.Quantity)
                .IsRequired();

            entity.Property(sm => sm.CreatedByUserId)
                .IsRequired();

            entity.Property(sm => sm.CreatedAt)
                .IsRequired();

            entity.HasOne(sm => sm.Product)
                .WithMany()
                .HasForeignKey(sm => sm.ProductId)
                .OnDelete(DeleteBehavior.Restrict);    
        });

        builder.Entity<Order>(entity =>
        {
            entity.Property(o => o.OrderNumber)
                .IsRequired()
                .HasMaxLength(50);
            
            entity.HasOne(o => o.Customer)
                .WithMany()
                .HasForeignKey(o => o.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(o => o.Items)
                .WithOne(i => i.Order)
                .HasForeignKey(i => i.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<OrderItem>(entity =>
        {
            entity.Property(oi => oi.Quantity)
                .IsRequired();

            entity.Property(oi => oi.UnitPrice)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            entity.HasOne(oi => oi.Product)
                .WithMany()
                .HasForeignKey(oi => oi.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<Customer>(entity =>
        {
            entity.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(50);
            
            entity.Property(c => c.NIP)
                .HasMaxLength(10);

            entity.HasIndex(c => c.NIP)
                .IsUnique();
        });

        builder.Entity<Invoice>(entity =>
        {
           entity.Property(i => i.InvoiceNumber)
                .IsRequired()
                .HasMaxLength(50);

            entity.HasIndex(i => i.InvoiceNumber)
                .IsUnique();

            entity.HasOne(i => i.Order)
                .WithOne()
                .HasForeignKey<Invoice>(i => i.OrderId)
                .OnDelete(DeleteBehavior.Restrict);
            
            entity.Property(i => i.Status)
                .IsRequired();

            entity.Property(i => i.TotalNet)
                .HasColumnType("decimal(18,2)")
                .IsRequired();
            
            entity.Property(i => i.TotalVat)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            entity.Property(i => i.TotalGross)
                .HasColumnType("decimal(18,2)")
                .IsRequired();
        });

        builder.Entity<InvoiceItem> (entity =>
        {
             entity.Property(ii => ii.Quantity)
                .IsRequired();

            entity.Property(ii => ii.UnitPrice)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            entity.Property(ii => ii.NetAmount)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            entity.Property(ii => ii.GrossAmount)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            entity.Property(ii => ii.VatRate)
                .HasColumnType("decimal(5,2)")
                .IsRequired();

            entity.HasOne(ii => ii.Product)
                .WithMany()
                .HasForeignKey(ii => ii.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
                });
    }

}


