using System.Collections.Generic;

namespace MobileFueling.Model
{
    public class Client : ApplicationUser
    {
        public virtual ICollection<Car> Cars { get; set; }
    }
}