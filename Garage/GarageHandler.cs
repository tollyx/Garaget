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
                typeof(Airplane),
                typeof(Boat),
                typeof(Bus),
                typeof(Car),
                typeof(Motorcycle),
            };

        Dictionary<string, Garage<Vehicle>> garages;
        Garage<Vehicle> selectedGarage;
        string selectedKey;

        public IEnumerable<string> Garages => garages.Keys;
        public string SelectedGarage => selectedKey;
        public int AmountOfGarages => garages.Count;
        public int SelectedGarageVehicleCount => selectedGarage.Count;
        public int SelectedGarageCapacity => selectedGarage.Capacity;
        public IEnumerable<string> FormattedGarageList => garages.Select((p) => $"{p.Key} ({p.Value.Count}/{p.Value.Capacity})");

        public GarageHandler() {
            garages = new Dictionary<string, Garage<Vehicle>>();
        }

        public void CreateGarage(string name, int capacity) {
            if (string.IsNullOrWhiteSpace(name)) return;
            if (garages.ContainsKey(name)) return;
            garages.Add(name, new Garage<Vehicle>(capacity));
        }

        public void SelectGarage(string key) {
            if (key != null && garages.ContainsKey(key)) {
                selectedGarage = garages[key];
                selectedKey = key;
            }
            else {
                selectedGarage = null;
                selectedKey = null;
            }
        }

        public IEnumerable<string> GetAllVehicleTypes() {
            return vehicleTypes.Select(x => x.Name);
        }
        
        /// <summary>
        /// Creates a new form for a vehicle type that the UI can fill in 
        /// and send back to the CreateVehicle method in order to create a new vehicle
        /// </summary>
        /// <param name="type">The name of the type to be created</param>
        /// <returns>A form for the specified vehicle type</returns>
        public Form GetVehicleForm(string type) {
            var vehicletype = vehicleTypes.FirstOrDefault(x => x.Name == type);
            if (vehicletype == null) throw new ArgumentException($"The type {type} does not exist!");

            var ctorparams = vehicletype.GetConstructors().Select(x => x.GetParameters()).OrderByDescending(x => x.Length).First();

            List<Field> fields = new List<Field>(ctorparams.Length);
            foreach (var param in ctorparams) {
                var ptype = param.ParameterType;
                if (ptype == typeof(string)) {
                    fields.Add(new Field<string>(param.Name, param.DefaultValue as string));
                }
                else if (ptype == typeof(int)) {
                    fields.Add(new Field<int>(param.Name, param.DefaultValue as int? ?? 0));
                }
                else {
                    throw new ArgumentException($"Error creating form for type '{vehicletype}': Cannot create a field for parameter type '{ptype}'");
                }
            }

            return new Form(vehicletype.Name, fields);
        }

        /// <summary>
        /// Creates a vehicle from a filled out form
        /// </summary>
        /// <param name="vehicleform">The filled out form to be used to create a new vehicle</param>
        public bool CreateVehicle(Form vehicleform) {
            if (SelectedGarage == null) throw new InvalidOperationException("No garage selected!");
            var vehicletype = vehicleTypes.FirstOrDefault(x => x.Name == vehicleform.Name);
            var ctor = vehicletype.GetConstructors().OrderByDescending(x => x.GetParameters().Length).First();
            return selectedGarage.Add((Vehicle)ctor.Invoke(vehicleform.EntryObjects));
        }



        public IEnumerable<Vehicle> SearchVehicles(string query) {
            if (SelectedGarage == null) throw new InvalidOperationException("No garage selected!");

            // This is so simple and works so well I love it
            // We match whatever the user types in with the vehicle's tostring, 
            // Which already lists all properties and values.
            // This also makes it possible to specify specific values 
            // for properties by simply inputting '<property>:<value>'.
            // No need for any complex form-input view!
            var terms = query.Split().Select(x => x.ToLower()).ToList();
            return selectedGarage.Where(
                    x => terms.All(q => x.ToString().ToLower().Contains(q)));
        }

        public IEnumerable<Vehicle> GetSelectedVehicles() {
            if (SelectedGarage == null) throw new InvalidOperationException("No garage selected!");
            return selectedGarage;
        }

        public bool DeleteGarage(string key) {
            if (selectedKey == key) {
                selectedKey = null;
                selectedGarage = null;
            }
            return garages.Remove(key);
        }

        public bool DeleteVehicle(Vehicle v) {
            if (SelectedGarage == null) throw new InvalidOperationException("No garage selected!");
            return selectedGarage.Remove(v);
        }

        public bool DeleteVehicle(int index) {
            if (SelectedGarage == null) throw new InvalidOperationException("No garage selected!");
            return selectedGarage.Remove(selectedGarage.Skip(index).First());
        }
    }
}
