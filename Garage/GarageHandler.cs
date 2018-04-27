using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage {
    public class GarageHandler {
        static readonly Type[] vehicleTypes = new Type[] {
                typeof(Car),
            };

        Dictionary<string, Garage<Vehicle>> garages;
        Garage<Vehicle> selectedGarage;
        string selectedKey;

        public GarageHandler() {
            garages = new Dictionary<string, Garage<Vehicle>>();
        }

        public void SelectGarage(string key) {
            if (garages.ContainsKey(key)) {
                selectedGarage = garages[key];
                selectedKey = key;
            }
        }

        public string[] GetAllVehicleTypes() {
            return vehicleTypes.Select(x => x.Name).ToArray();
        }

        public Form GetVehicleForm(string type) {
            var vehicletype = vehicleTypes.FirstOrDefault(x => x.Name == type);
            return new Form();
        }

        public void CreateVehicle(Form vehicleform) {

        }
    }
}
