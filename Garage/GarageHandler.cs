using Garage.Vehicles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage {
    public class GarageHandler {

        // TODO: clean this class up

        static readonly Type[] vehicleTypes = new Type[] {
                typeof(Car),
            };

        Dictionary<string, Garage<Vehicle>> garages;
        public IEnumerable<string> Garages => garages.Keys;
        Garage<Vehicle> selectedGarage;
        string selectedKey;
        public string SelectedGarage => selectedKey;
        public int GarageCount => garages.Count;
        public int SelectedCount => selectedGarage.Count;
        public int SelectedCapacity => selectedGarage.Capacity;
        public IEnumerable<string> FormattedGaragesList => garages.Select((p) => $"{p.Key} ({p.Value.Count}/{p.Value.Capacity})");

        public GarageHandler() {
            garages = new Dictionary<string, Garage<Vehicle>>();
        }

        public void CreateGarage(string name, int capacity) {
            if (garages.ContainsKey(name)) {
                return;
            }
            garages.Add(name, new Garage<Vehicle>(capacity));
        }

        public void SelectGarage(string key) {
            if (garages.ContainsKey(key)) {
                selectedGarage = garages[key];
                selectedKey = key;
            }
            else {
                selectedGarage = null;
                selectedKey = null;
            }
        }

        public string[] GetAllVehicleTypes() {
            return vehicleTypes.Select(x => x.Name).ToArray();
        }

        public Form GetVehicleForm(string type) {
            var vehicletype = vehicleTypes.FirstOrDefault(x => x.Name == type);
            if (vehicletype == null) throw new ArgumentException($"The type {type} does not exist!");

            var ctorparams = vehicletype.GetConstructors().Select(x => x.GetParameters()).OrderByDescending(x => x.Length).First();

            List<Field> fields = new List<Field>(ctorparams.Length);
            foreach (var item in ctorparams) {
                var ptype = item.ParameterType;
                if (ptype == typeof(string)) {
                    fields.Add(new Field<string>(item.Name, item.DefaultValue as string));
                }
                else if (ptype == typeof(int)) {
                    fields.Add(new Field<int>(item.Name, item.DefaultValue as int? ?? 0));
                }
                else {
                    throw new ArgumentException($"Error creating form for type '{vehicletype}': Cannot create a field for parameter type '{ptype}'");
                }
            }

            return new Form(vehicletype.Name, fields);
        }

        public void CreateVehicle(Form vehicleform) {
            if (SelectedGarage == null) throw new InvalidOperationException("No garage selected!");
            var vehicletype = vehicleTypes.FirstOrDefault(x => x.Name == vehicleform.Name);
            var ctor = vehicletype.GetConstructors().OrderByDescending(x => x.GetParameters().Length).First();
            selectedGarage.Add((Vehicle)ctor.Invoke(vehicleform.EntryObjects));
        }

        public IEnumerable<Vehicle> GetVehicles() {
            if (SelectedGarage == null) throw new InvalidOperationException("No garage selected!");
            return selectedGarage;
        }

        public bool DeleteGarage(string entry) {
            if (selectedKey == entry) {
                selectedKey = null;
                selectedGarage = null;
            }
            return garages.Remove(entry);
        }

        public bool RemoveVehicle(Vehicle v) {
            if (SelectedGarage == null) throw new InvalidOperationException("No garage selected!");
            return selectedGarage.Remove(v);
        }
    }
}
