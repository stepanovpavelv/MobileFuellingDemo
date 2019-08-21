using MobileFueling.Model;
using System.Collections.Generic;
using System.Linq;

namespace MobileFueling.DB
{
    /// <summary>
    /// using this class because of conflicts between HasData and autoincrement in postgresql
    /// </summary>
    public static class FuelInitializer
    {
        public static void InitializePredefinedData(FuelDbContext context)
        {
            InitializeFuelTypes(context);
        }

        private static void InitializeFuelTypes(FuelDbContext context)
        {
            if (context.FuelTypes.Any())
                return;

            var entities = new List<FuelType>()
            {
                new FuelType { Name = "АИ-80" },
                new FuelType { Name = "АИ-92" },
                new FuelType { Name = "АИ-95" },
                new FuelType { Name = "АИ-98" },
                new FuelType { Name = "Дизель" },
                new FuelType { Name = "Газ" }
            };
            context.FuelTypes.AddRange(entities);
            context.SaveChanges();
        }
    }
}