using Microsoft.EntityFrameworkCore;
using MobileFueling.Model;

namespace MobileFueling.DB
{
    public static class FuelMobileBuilder
    {
        public static void OnModelCreating(ModelBuilder builder)
        {
            // fuel types
            builder.Entity<FuelType>().ToTable("FuelTypes");
            builder.Entity<FuelType>().HasKey(x => x.Id);
            builder.Entity<FuelType>().Property(x => x.Name).HasMaxLength(50);
            
            // users - clients
            builder.Entity<Client>().HasMany(x => x.Cars).WithOne(x => x.Client).HasForeignKey(x => x.ClientId);

            // cars
            builder.Entity<Car>().ToTable("Cars");
            builder.Entity<Car>().HasKey(x => x.Id);
            builder.Entity<Car>().Property(x => x.Description).HasMaxLength(200);
            builder.Entity<Car>().HasOne(x => x.FuelType).WithMany().HasForeignKey(x => x.FuelTypeId);

            // client order's detalization
            builder.Entity<ClientOrderDetalization>().ToTable("ClientOrderDetalizations");
            builder.Entity<ClientOrderDetalization>().HasKey(x => x.Id);
            builder.Entity<ClientOrderDetalization>().HasOne(x => x.FuelType).WithMany().HasForeignKey(x => x.FuelTypeId);
            builder.Entity<ClientOrderDetalization>().Property(x => x.Address).HasMaxLength(250);
            builder.Entity<ClientOrderDetalization>().Property(x => x.Latitude).HasColumnType("decimal(10,6)");
            builder.Entity<ClientOrderDetalization>().Property(x => x.Longitude).HasColumnType("decimal(10,6)");

            // driver's order's detalizations
            builder.Entity<DriverOrderDetalization>().ToTable("DriverOrderDetalizations");
            builder.Entity<DriverOrderDetalization>().HasKey(x => x.Id);
            builder.Entity<DriverOrderDetalization>().HasOne(x => x.Driver).WithMany().HasForeignKey(x => x.DriverId);

            // orders
            builder.Entity<Order>().ToTable("Orders");
            builder.Entity<Order>().HasKey(x => x.Id);
            builder.Entity<Order>().HasOne(x => x.Client).WithMany().HasForeignKey(x => x.ClientId);
            builder.Entity<Order>().HasOne(x => x.ClientDetalization).WithOne(x => x.Order).HasForeignKey<ClientOrderDetalization>(x => x.OrderId);
            builder.Entity<Order>().HasMany(x => x.DriverDetalizations).WithOne(x => x.Order).HasForeignKey(x => x.OrderId);

            // order status history
            builder.Entity<OrderStatusHistory>().ToTable("OrderStatusesHistory");
            builder.Entity<OrderStatusHistory>().HasKey(x => x.Id);
            builder.Entity<OrderStatusHistory>().HasOne(x => x.Order).WithMany().HasForeignKey(x => x.OrderId);
        }
    }
}