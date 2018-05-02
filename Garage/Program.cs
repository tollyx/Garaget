using Garage.Vehicles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage {
    class Program {
        static void Main(string[] args) {
            Console.CursorVisible = false;
            GarageHandler garages = new GarageHandler();

            bool running = true;
            bool selecting = true;
            while (running) {
                if (garages.GarageCount == 0) {
                    // No garage menu
                    Console.WriteLine("You have no garages.");
                    switch (Menu(new string[] { "Create a new garage", "-Quit-", })) {
                        case 0:
                            CreateGarage(garages);
                            selecting = false;
                            break;
                        case 1:
                            running = false;
                            break;
                        default:
                            break;
                    }
                }
                else if (garages.SelectedGarage == null || selecting) {
                    // Create/Delete/Select a garage
                    Console.WriteLine("What do you want to do?");
                    switch (Menu(new string[] { "Create a new garage", "Delete a garage", "Select a Garage", "-Quit-" })) {
                        case 0:
                            CreateGarage(garages);
                            break;
                        case 1:
                            DeleteGarage(garages);
                            break;
                        case 2:
                            SelectGarage(garages);
                            selecting = false;
                            break;
                        case 3:
                            running = false;
                            break;
                        default:
                            break;
                    }
                }
                else {
                    // Do things to the selected garage
                    Console.WriteLine($"{garages.SelectedGarage}: {garages.SelectedCount}/{garages.SelectedCapacity}");
                    Console.WriteLine("What do you want to do?");
                    switch (Menu(new string[] { "Stats", "List vehicles", "Add a vehicle", "Remove a vehicle", "Find a vehicle", "Get a vehicle by license plate", "-Back-" })) {
                        case 0:
                            Stats(garages);
                            break;
                        case 1:
                            ListVehicles(garages.GetVehicles());
                            break;
                        case 2:
                            AddVehicle(garages);
                            break;
                        case 3:
                            RemoveVehicle(garages);
                            break;
                        case 4:
                            FindVehicle(garages);
                            break;
                        case 5:
                            GetVehicle(garages);
                            break;
                        case 6:
                            selecting = true;
                            break;
                        default:
                            break;
                    }
                }
                Console.WriteLine();
            }
        }

        private static void Stats(GarageHandler garages) {
            Console.WriteLine($"{garages.SelectedGarage}'s stats:");
            var vehicletypes = garages.GetAllVehicleTypes();
            var vehicles = garages.GetVehicles();
            foreach (var item in vehicletypes) {
                Console.WriteLine($"{item}s: {vehicles.Where(x => x.ToString().StartsWith(item)).Count()}");
            }
        }

        private static void GetVehicle(GarageHandler garage) {
            Console.WriteLine("Please enter the vehicle's license plate: ");
            string input = Prompt();
            var match = garage.GetVehicles().FirstOrDefault(x => x.Plate == input);
            if (match != null) {
                Console.WriteLine(match);
            }
            else {
                Console.WriteLine("Could not find vehicle.");
            }
        }

        private static void FindVehicle(GarageHandler garage) {
            Console.WriteLine("Please enter your search query: (enter nothing to cancel)");
            string query = Prompt();
            if (string.IsNullOrWhiteSpace(query)) return;

            // This is so simple and works so well I love it
            // We match whatever the user types in with the vehicle's tostring, 
            // Which already lists all properties and values.
            // This also makes it possible to specify specific values 
            // for properties by simply inputting '<property>:<value>'.
            // No need for any complex form-input view!
            var terms = query.Split().Select(x => x.ToLower()).ToList();
            var matches = garage.GetVehicles()
                .Where(
                    x => terms.All(q => x.ToString().ToLower().Contains(q))
                    );
            if (!matches.Any()) {
                Console.WriteLine("No matches.");
                return;
            }
            Console.WriteLine("Found vehicles:");
            ListVehicles(matches);
        }

        private static void RemoveVehicle(GarageHandler garage) {
            var options = garage.GetVehicles().Select(x => " " + x.ToString()).ToList();
            options.Add("-Cancel-");
            Console.WriteLine("Select the vehicle to remove:");
            var sel = Menu(options.ToArray());
            if (sel >= options.Count) return;

            // TODO: remove this and add it as a feature in the list view

            var v = garage.GetVehicles().Skip(sel).First();
            garage.RemoveVehicle(v);
        }

        private static void AddVehicle(GarageHandler garage) {
            Console.WriteLine("Please select a vehicle type:");
            var types = garage.GetAllVehicleTypes().ToList();
            types.Add("-Cancel-");
            var sel = Menu(types.ToArray());
            if (sel >= types.Count - 1) return;

            var selected = types[sel];
            var form = garage.GetVehicleForm(selected);
            
            // TODO: FormEntry edit view that allows you to see all fields and switch between them as you please?

            int i = 0;
            Console.WriteLine("Please enter the following values:");
            foreach (var item in form.Fields) {
                if (item is Field<string> strfield) {
                    strfield.Entry = FormEntry(strfield.Name, strfield.Entry);
                }
                else if (item is Field<int> intfield) {
                    intfield.Entry = FormEntry(intfield.Name, intfield.Entry);
                }
                i++;
            }
            garage.CreateVehicle(form);
        }

        private static void ListVehicles(IEnumerable<Vehicle> vehicles) {
            var list = vehicles.Select(x => x.ToString()).ToList();
            if (list.Count == 0) {
                Console.WriteLine("There are no vehicles to list.");
                return;
            }
            list.Add("-Back-");
            var arr = list.ToArray();
            int sel = Menu(arr);
            if (sel >= arr.Length - 1) {
                return;
            }

            // TODO: Do something with the selected vehicle
        }

        private static void SelectGarage(GarageHandler garages) {
            var entries = garages.FormattedGaragesList.ToList();
            entries.Add("-Cancel-");

            Console.WriteLine("Select a garage:");
            int sel = Menu(entries.ToArray());

            if (sel < entries.Count-1) {
                garages.SelectGarage(garages.Garages.Skip(sel).First());
            }
        }

        private static void DeleteGarage(GarageHandler garages) {
            var list = garages.FormattedGaragesList.ToList();
            Console.WriteLine("Pick a garage to delete:");
            list.Add("-Cancel-");
            var sel = Menu(list.ToArray());
            if (sel >= 0 && sel < garages.GarageCount) {
                var entry = garages.Garages.Skip(sel).First();
                garages.DeleteGarage(entry);
                Console.WriteLine($"Deleted Garage {entry}");
            }
        }

        private static void CreateGarage(GarageHandler garages) {
            Console.WriteLine("Please enter a name for the garage: (enter nothing to cancel)");
            string name = Prompt();
            if (string.IsNullOrWhiteSpace(name)) return;
            if (garages.Garages.Any(x => x == name)) {
                Console.WriteLine($"A Garage with the name '{name}' already exists");
                return;
            }

            string capacityInput;
            int capacity;
            do {
                Console.WriteLine("Please enter the capacity of the garage: (enter '-1' to cancel)");
                capacityInput = Prompt();
            } while (!Int32.TryParse(capacityInput, out capacity));
            if (capacity > 0) {
                garages.CreateGarage(name, capacity);
                garages.SelectGarage(name);
            }
            return;
        }

        static int Menu(string[] options) {
            int startpos = Console.CursorTop;
            int selection = 0;
            bool finished = false;
            foreach (var item in options) {
                Console.WriteLine($" {item}");
            }
            while (!finished) {
                Console.SetCursorPosition(0, startpos + selection);
                Console.Write('>');
                var input = Console.ReadKey(true);
                switch (input.Key) {
                    case ConsoleKey.UpArrow:
                    case ConsoleKey.W:
                    case ConsoleKey.K:
                        Console.SetCursorPosition(0, startpos + selection);
                        Console.Write(' ');
                        selection--;
                        break;
                    case ConsoleKey.DownArrow:
                    case ConsoleKey.S:
                    case ConsoleKey.J:
                        Console.SetCursorPosition(0, startpos + selection);
                        Console.Write(' ');
                        selection++;
                        break;
                    case ConsoleKey.Enter:
                        finished = true;
                        break;
                    default:
                        break;
                }
                if (selection < 0) {
                    selection += options.Length;
                }
                else if(selection >= options.Length) {
                    selection -= options.Length;
                }
            }
            Console.SetCursorPosition(0, startpos + options.Length);
            return selection;
        }

        static string Prompt() {
            Console.CursorVisible = true;
            Console.Write(">");
            string input = Console.ReadLine();
            Console.CursorVisible = false;
            return input;
        }

        static string FormEntry(string field, string defaultValue = null) {
            string entry = defaultValue ?? "";
            ConsoleKeyInfo input;
            bool typing = true;
            int line = Console.CursorTop;
            
            while (typing) {
                Console.CursorVisible = false;
                int pos = Console.CursorLeft;
                string str = $"{field}: {entry}";
                Console.CursorLeft = 0;
                Console.Write(str.PadRight(pos));
                Console.CursorLeft = str.Length;

                Console.CursorVisible = true;
                input = Console.ReadKey(true);
                switch (input.Key) {
                    case ConsoleKey.Backspace:
                        if (entry.Length > 0) {
                            entry = entry.Remove(entry.Length - 1);
                        }
                        break;
                    case ConsoleKey.Enter:
                        typing = false;
                        break;
                    default:
                        if (char.IsLetterOrDigit(input.KeyChar)) {
                            entry += input.KeyChar;
                        }
                        break;
                }
            }
            Console.CursorVisible = false;
            Console.WriteLine();
            return entry;
        }

        static int FormEntry(string field, int? defaultValue = null) {
            int? entry = defaultValue;
            ConsoleKeyInfo input;
            bool typing = true;
            while (typing) {
                Console.CursorVisible = false;
                int pos = Console.CursorLeft;
                string str = $"{field}: {entry}";
                Console.CursorLeft = 0;
                Console.Write(str.PadRight(pos+1));
                Console.CursorLeft = str.Length;

                Console.CursorVisible = true;
                input = Console.ReadKey(true);
                switch (input.Key) {
                    case ConsoleKey.Backspace:
                        if (entry != 0) {
                            entry -= entry % 10;
                            entry /= 10;
                            if (entry == 0) {
                                entry = null;
                            }
                        }
                        else {
                            entry = null;
                        }
                        break;
                    case ConsoleKey.Enter:
                        typing = false;
                        break;
                    default:
                        if (char.IsNumber(input.KeyChar)) {
                            if (entry == null) {
                                entry = 0;
                            }
                            entry *= 10;
                            entry += int.Parse(input.KeyChar.ToString());
                        }
                        break;
                }
            }
            Console.CursorVisible = false;
            Console.WriteLine();
            return entry ?? 0;
        }
    }
}
