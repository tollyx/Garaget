using Garage.Vehicles;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage {
    public class Garage<T> : IEnumerable<T> where T : Vehicle {
        T[] vehicles;

        public int Capacity => vehicles.Length;
        public int Count { get; private set; }

        public Garage(T[] vehicles) {
            this.vehicles = vehicles;
        }

        public Garage(IList<T> vehicles) {
            this.vehicles = vehicles.ToArray();
        }

        public Garage(T[] vehicles, int capacity) {
            this.vehicles = new T[capacity];
            vehicles.CopyTo(vehicles, 0);
        }

        public Garage(IList<T> vehicles, int capacity) {
            this.vehicles = new T[capacity];
            vehicles.CopyTo(this.vehicles, 0);
        }

        public Garage(int capacity) {
            vehicles = new T[capacity];
            Count = 0;
        }

        public bool Add(T vehicle) {
            if (Count >= Capacity) return false;
            vehicles[Count++] = vehicle;
            return true;
        }

        public bool Remove(T vehicle) {
            if (Count <= 0) return false;

            int removedAt = -1;
            for (int i = 0; i < Count; i++) {
                if (vehicles[i] == vehicle) {
                    vehicles[i] = null;
                    removedAt = i;
                    break;
                }
            }
            if (removedAt < 0) return false;

            Count--;

            // Pack the vehicles to remove any null-gaps in the array
            // Changes the order of the vehicles but it's O(1)
            // TODO: Maybe switch to a proper order-preserving move?
            if (removedAt < Count) {
                vehicles[removedAt] = vehicles[Count];
                vehicles[Count] = null; // I Forgot this line, causing the enumerator iterate over duplicated elements. 
                                        // Last assert in the 'Remove_At_Front_Success' test caught it. 
                                        // Unit testing is nice. :)
            }

            return true;
        }

        public IEnumerator<T> GetEnumerator() {
            foreach (var item in vehicles) {
                if (item != null) {
                    yield return item;
                }
                else {
                    // We could just skip the item instead of breaking, 
                    // but we shouldn't have any null-gaps in the array anyway
                    // and those should be considered a bug.
                    // And if that would ever to occur, the unit tests would catch it (i hope)
                    yield break;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return vehicles.GetEnumerator();
        }
    }
}
