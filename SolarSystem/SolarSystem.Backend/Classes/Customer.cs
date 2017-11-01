using System;
using System.Collections.Generic;

namespace SolarSystem.Backend.Classes
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
