namespace Garage.Tests {
    internal class TestVehicle : Vehicle {
        public TestVehicle(int wheels = 4, string color = "White", string plate = "I1I11I1") 
            : base(wheels, color, plate) {
        }
    }
}