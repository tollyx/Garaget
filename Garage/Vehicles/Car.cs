namespace Garage.Vehicles {
    internal class Car : Vehicle {
        public int TankVolume { get; }

        public Car(string plate, string color="Yellow", int tankvolume=20) : base(4, color, plate) {
            TankVolume = tankvolume;
        }
    }
}