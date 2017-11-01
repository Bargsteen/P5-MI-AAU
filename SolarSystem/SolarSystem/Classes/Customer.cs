using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarSystem.Classes
{
    public class Customer
    {
        public string Name { get;}
        private readonly List<Order> _orders;

        public Customer(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            _orders = new List<Order>();
        }
    }
}
