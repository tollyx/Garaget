using System.Linq;
using System.Text;

namespace Garage.Vehicles {
    public abstract class Vehicle {
        public string Color { get; }
        public int Wheels { get; }
        public string Plate { get; }

        public Vehicle(int wheels, string color, string plate) {
            Wheels = wheels;
            Color = color;
            Plate = plate;
        }

        public override string ToString() {
            var name = GetType().Name;
            var props = GetType().GetProperties();
            return $"{name}: ({string.Join(", ", props.Reverse().Select(x => $"{x.Name}:{x.GetValue(this)}"))})";
        }
    }
}