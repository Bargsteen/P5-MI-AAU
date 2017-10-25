using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarSystem.Classes
{
    public class Customer
    {
        public List<Order> CustomerOrderList { get; set; }

        public Customer(List<Order> customerOrderList)
        {
            CustomerOrderList = customerOrderList ?? throw new ArgumentNullException(nameof(customerOrderList));
        }
    }
}
