using System.Collections.Generic;

namespace MobileFueling.Model
{
    public class Order : IEntity
    {
        public long Id { get; set; }
        public string Number { get; set; }
        public long ClientId { get; set; }
        public Client Client { get; set; }
        public virtual ClientOrderDetalization ClientDetalization { get; set; }
        public virtual ICollection<DriverOrderDetalization> DriverDetalizations { get; set; }
    }
}