﻿using System;

namespace SolarSystem.Backend.Classes
{
    public class OrderBoxEventArgs: EventArgs
    {
        public OrderBoxEventArgs(OrderBox orderBox)
        {
            OrderBox = orderBox;
        }

        public OrderBox OrderBox { get; set; }
    }
}
