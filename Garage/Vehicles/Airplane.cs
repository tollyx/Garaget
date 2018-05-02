using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage.Vehicles {
    class Airplane : Vehicle {
        public int Propellers { get; }

        public Airplane(string Plate, string Color="White", int Propellers=1) : base(3, Color, Plate) {
            this.Propellers = Propellers;
        }
    }
}
