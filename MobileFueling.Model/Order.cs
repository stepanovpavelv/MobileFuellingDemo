using System.Collections.Generic;

namespace MobileFueling.Model
{
    public class Order : IEntity
    {
        public long Id { get; set; }
        public string Number { get; set; }
        //public long ClientId { get; set; }
        //public Client Client { get; set; }
        //public long ClientDetalizationId { get; set; }
        //public ClientOrderDetalization ClientDetalization { get; set; }
    }
}