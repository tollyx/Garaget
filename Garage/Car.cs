namespace Garage {
    internal class Car : Vehicle {
        public int Seats { get; private set; }

        public Car(string plate, string color="Yellow", int seats=5) : base(4, color, plate) {
            Seats = 5;
        }
    }
}