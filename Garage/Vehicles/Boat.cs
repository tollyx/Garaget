using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage.Vehicles {
    class Boat : Vehicle {
        public int Sails { get; }

        public Boat(string plate, string color, int sails) : base(0, color, plate) {
            Sails = sails;
        }
    }
}
