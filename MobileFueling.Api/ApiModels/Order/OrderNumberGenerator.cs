using MobileFueling.DB;
using System;
using System.Linq;
using System.Text;

namespace MobileFueling.Api.ApiModels.Order
{
    /// <summary>
    /// Генератор номеров заказов
    /// </summary>
    public static class OrderNumberGenerator
    {
        private static RandomGenerator _generator;

        static OrderNumberGenerator()
        {
            _generator = new RandomGenerator();
        }

        public static string GetNumber(FuelDbContext fuelContext)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(_generator.RandomString(5, false));
            builder.Append("-");
            builder.Append(_generator.RandomNumber(1, 9999));
            string resNum = builder.ToString();

            if (fuelContext.Orders.Any(x => x.Number == resNum))
            {
                return GetNumber(fuelContext);
            }

            return resNum;
        }

        private class RandomGenerator
        {
            public int RandomNumber(int min, int max)
            {
                Random random = new Random();
                return random.Next(min, max);
            }

            // Generate a random string with a given size    
            public string RandomString(int size, bool lowerCase)
            {
                StringBuilder builder = new StringBuilder();
                Random random = new Random();
                char ch;
                for (int i = 0; i < size; i++)
                {
                    ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                    builder.Append(ch);
                }
                if (lowerCase)
                {
                    return builder.ToString().ToLower();
                }
                return builder.ToString();
            }
        }
    }
}