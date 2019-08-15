using Microsoft.EntityFrameworkCore;
using MobileFueling.Model;

namespace MobileFueling.DB
{
    public static class MobileFuelingMobileBuilder
    {
        public static void OnModelCreating(ModelBuilder builder)
        {
            // fuel types
            builder.Entity<FuelType>().ToTable("FuelTypes");
            builder.Entity<FuelType>().HasKey(x => x.Id);
            builder.Entity<FuelType>().Property(x => x.Name).HasMaxLength(50);

            // users
            //builder.Entity
        }
    }
}