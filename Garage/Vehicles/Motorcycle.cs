using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage.Vehicles {
    class Motorcycle : Vehicle {
        public int Weight { get; }

        public Motorcycle(string plate, string color="Green", int weight=0) : base(2, color, plate) {
            Weight = weight;
        }
    }
}
