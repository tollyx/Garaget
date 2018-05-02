using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage.Vehicles {
    class Bus : Vehicle {
        public int Seats { get; }

        public Bus(string plate, int wheels=6, string color="Red", int seats=50) : base(wheels, color, plate) {
            Seats = seats;
        }
    }
}
