using System.Linq;
using System.Text;

namespace Garage {
    public abstract class Vehicle {
        public string Color { get; private set; }
        public int Wheels { get; private set; }
        public string Plate { get; private set; }

        public Vehicle(int wheels, string color, string plate) {
            Wheels = wheels;
            Color = color;
            Plate = plate;
        }

        public override string ToString() {
            var name = GetType().Name;
            var props = GetType().GetProperties();
            return $"{name}: ({string.Join(", ", props.Reverse().Select(x => $"{x.Name}: {x.GetValue(this)}"))})";
        }
    }
}