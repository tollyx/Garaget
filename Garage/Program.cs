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
            GarageHandler handler = new GarageHandler();

            bool running = true;

            while (running) {
                if (handler.SelectedGarage == null) {
                    // Create/Delete/Select a garage
                    var menu = new List<(string, Action)>() {
                        ("Create a new garage", () => CreateGarage(handler))
                    };
                    if (handler.AmountOfGarages > 0) {
                        menu.Add(("Delete a garage", () => DeleteGarage(handler)));
                        menu.Add(("Select a Garage", () => SelectGarage(handler)));
                    }
                    menu.Add(("-Quit-", () => running = false));

                    Console.WriteLine("What do you want to do?");
                    MenuActions(menu);
                }
                else {
                    // Do things to the selected garage
                    var menu = new List<(string, Action)>();
                    if (handler.SelectedGarageVehicleCount < handler.SelectedGarageCapacity) {
                        menu.Add(("Add a vehicle", () => AddVehicle(handler)));
                    }
                    if (handler.SelectedGarageVehicleCount > 0) {
                        menu.Add(("Stats", () => Stats(handler)));
                        menu.Add(("List vehicles", () => ListVehicles(handler, handler.GetSelectedVehicles())));
                        menu.Add(("Remove a vehicle", () => RemoveVehicle(handler)));
                        menu.Add(("Find a vehicle", () => FindVehicle(handler)));
                        menu.Add(("Get by License plate", () => GetVehicle(handler)));
                    }
                    menu.Add(("-Back-", () => handler.SelectGarage(null)));


                    Console.WriteLine($"{handler.SelectedGarage}: {handler.SelectedGarageVehicleCount}/{handler.SelectedGarageCapacity}");
                    Console.WriteLine("What do you want to do?");
                    MenuActions(menu);
                }
            }
        }

        private static void Stats(GarageHandler handler) {
            Console.WriteLine($"{handler.SelectedGarage}'s stats:");
            var vehicletypes = handler.GetAllVehicleTypes();
            var vehicles = handler.GetSelectedVehicles();
            foreach (var item in vehicletypes) {
                Console.WriteLine($"{item}s: {vehicles.Where(x => x.ToString().StartsWith(item)).Count()}");
            }
        }

        private static void GetVehicle(GarageHandler handler) {
            Console.WriteLine("Please enter the vehicle's license plate: ");
            string input = Prompt();
            var match = handler.GetSelectedVehicles().FirstOrDefault(x => x.Plate == input);
            if (match != null) {
                Console.WriteLine(match);
            }
            else {
                Console.WriteLine("Could not find vehicle.");
            }
        }

        private static void FindVehicle(GarageHandler handler) {
            Console.WriteLine("Please enter your search query: (enter nothing to cancel)");
            string query = Prompt();
            if (string.IsNullOrWhiteSpace(query)) return;

            IEnumerable<Vehicle> matches = handler.SearchVehicles(query);
            if (!matches.Any()) {
                Console.WriteLine("No matches.");
                return;
            }
            Console.WriteLine("Found vehicles:");
            ListVehicles(handler, matches);
        }

        private static void RemoveVehicle(GarageHandler handler) {
            var options = handler.GetSelectedVehicles().Select(x => x.ToString()).ToList();
            options.Add("-Cancel-");
            Console.WriteLine("Select the vehicle to remove:");
            var sel = Menu(options.ToArray());
            if (sel >= options.Count) return;
            handler.DeleteVehicle(sel);
        }

        private static void AddVehicle(GarageHandler handler) {
            if (handler.SelectedGarageVehicleCount >= handler.SelectedGarageCapacity) {
                Console.WriteLine("The garage is full.");
                return;
            }
            Console.WriteLine("Please select a vehicle type:");
            var types = handler.GetAllVehicleTypes().ToList();
            types.Add("-Cancel-");
            var sel = Menu(types.ToArray());
            if (sel >= types.Count - 1) return;

            var selected = types[sel];
            var form = handler.GetVehicleForm(selected);
            
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
            handler.CreateVehicle(form);
        }

        private static void ListVehicles(GarageHandler handler, IEnumerable<Vehicle> vehicles) {
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

            VehicleMenu(handler, vehicles.Skip(sel).First());
        }

        private static void SelectGarage(GarageHandler handler) {
            var entries = handler.FormattedGarageList.ToList();
            entries.Add("-Cancel-");

            Console.WriteLine("Select a garage:");
            int sel = Menu(entries.ToArray());

            if (sel < entries.Count-1) {
                handler.SelectGarage(handler.Garages.Skip(sel).First());
            }
        }

        private static void DeleteGarage(GarageHandler handler) {
            var list = handler.FormattedGarageList.ToList();
            list.Add("-Cancel-");
            Console.WriteLine("Pick a garage to delete:");
            var sel = Menu(list.ToArray());
            if (sel >= 0 && sel < handler.AmountOfGarages) {
                var entry = handler.Garages.Skip(sel).First();
                handler.DeleteGarage(entry);
                Console.WriteLine($"Deleted Garage {entry}");
            }
        }

        private static void CreateGarage(GarageHandler handler) {
            Console.WriteLine("Please enter a name for the garage: (enter nothing to cancel)");
            string name = Prompt();
            if (string.IsNullOrWhiteSpace(name)) return;
            if (handler.Garages.Any(x => x == name)) {
                Console.WriteLine($"A Garage with the name '{name}' already exists");
                return;
            }

            string capacityInput;
            int capacity;
            do {
                Console.WriteLine("Please enter the capacity of the garage: (enter '-1' to cancel)");
                capacityInput = Prompt();
            } while (!Int32.TryParse(capacityInput, out capacity));
            if (capacity >= 0) {
                handler.CreateGarage(name, capacity);
                handler.SelectGarage(name);
            }
            return;
        }

        static void VehicleMenu(GarageHandler handler, Vehicle vehicle) {
            List<(string, Action)> menu = new List<(string, Action)>() {
                ("Delete", () => handler.DeleteVehicle(vehicle)),
                ("-Back-", () => {}),
            };
            Console.WriteLine();
            Console.WriteLine("What do you want to do with this vehicle?");
            Console.WriteLine(vehicle);
            MenuActions(menu);
        }

        static void MenuActions(IList<(string name, Action action)> menu) {
            int sel = Menu(menu.Select(x => x.name).ToArray());
            Console.WriteLine();
            menu[sel].action();
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
                    case ConsoleKey.NumPad8:
                        Console.SetCursorPosition(0, startpos + selection);
                        Console.Write(' ');
                        selection--;
                        break;
                    case ConsoleKey.DownArrow:
                    case ConsoleKey.S:
                    case ConsoleKey.J:
                    case ConsoleKey.NumPad2:
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
