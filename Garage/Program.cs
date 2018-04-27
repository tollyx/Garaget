using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage {
    class Program {
        static void Main(string[] args) {
            Console.CursorVisible = false;
            Dictionary<string, Garage<Vehicle>> garages = new Dictionary<string, Garage<Vehicle>>();

            bool running = true;
            Garage<Vehicle> selectedGarage = null;
            string selectedName = null;
            while (running) {
                if (garages.Count == 0) {
                    // No garage menu
                    Console.WriteLine("You have no garages.");
                    switch (Menu(new string[] { "Create a new garage", "-Quit-", })) {
                        case 0:
                            CreateGarage(garages);
                            break;
                        case 1:
                            running = false;
                            break;
                        default:
                            break;
                    }
                }
                else if (selectedGarage == null) {
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
                            selectedName = SelectGarage(garages);
                            if (selectedName != null) {
                                selectedGarage = garages[selectedName];
                            }
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
                    Console.WriteLine($"{selectedName}: {selectedGarage.Count}/{selectedGarage.Capacity}");
                    Console.WriteLine("What do you want to do?");
                    switch (Menu(new string[] { "List vehicles", "Add a vehicle", "Remove a vehicle", "Find a vehicle", "Get a vehicle by license plate", "-Back-" })) {
                        case 0:
                            ListVehicles(selectedGarage);
                            break;
                        case 1:
                            AddVehicle(selectedGarage);
                            break;
                        case 2:
                            RemoveVehicle(selectedGarage);
                            break;
                        case 3:
                            FindVehicle(selectedGarage);
                            break;
                        case 4:
                            GetVehicle(selectedGarage);
                            break;
                        case 5:
                            selectedGarage = null;
                            break;
                        default:
                            break;
                    }
                }
                Console.WriteLine();
            }
        }

        private static void GetVehicle(Garage<Vehicle> garage) {
            throw new NotImplementedException();
        }

        private static void FindVehicle(Garage<Vehicle> garage) {
            throw new NotImplementedException();
        }

        private static void RemoveVehicle(Garage<Vehicle> garage) {
            var options = garage.Select(x => " " + x.ToString()).ToList();
            options.Add("-Cancel-");
            Console.WriteLine("Select the vehicle to remove:");
            var sel = Menu(options.ToArray());
            if (sel >= garage.Count) {
                return;
            }

            var v = garage.Skip(sel).First();
            garage.Remove(v);
        }

        private static void AddVehicle(Garage<Vehicle> garage) {
            Console.WriteLine("Please select a vehicle type:");
            var types = new Type[] {
                typeof(Car),
            };
            var options = types.Select(x => x.Name).ToList();
            options.Add("-Cancel-");
            var sel = Menu(options.ToArray());
            if (sel >= types.Length) {
                return;
            }

            var selected = types[sel];

            var ctor = selected.GetConstructors()
                        .Where(x => !x.IsPrivate)
                        .OrderByDescending(x => x.GetParameters().Length)
                        .First();

            var para = ctor.GetParameters();
            var values = new object[para.Length];
            int i = 0;
            Console.WriteLine("Please enter the following values:");
            foreach (var item in para) {
                if (item.ParameterType == typeof(string)) {
                    values[i] = FormEntry(item.Name, item.DefaultValue as string);
                }
                else if (item.ParameterType == typeof(int)) {
                    values[i] = FormEntry(item.Name, item.DefaultValue as int?);
                }
                i++;
            }
            garage.Add((Vehicle)ctor.Invoke(values));
        }

        private static void ListVehicles(Garage<Vehicle> garage) {
            if (garage.Count == 0) {
                Console.WriteLine("There are no vehicles in this garage.");
                return;
            }

            // All the work is done in Vehicle.ToString()
            foreach (var item in garage) {
                Console.WriteLine(item);
            }
        }

        private static string SelectGarage(Dictionary<string, Garage<Vehicle>> garages) {
            var keys = garages.Keys.ToList();
            var entries = garages.Select(p => $"{p.Key} ({p.Value.Count}/{p.Value.Capacity})").ToList();
            entries.Add("-Cancel-");

            Console.WriteLine("Select a garage:");
            int sel = Menu(entries.ToArray());

            if (sel < keys.Count) {
                return keys[sel];
            }
            return null;
        }

        private static void DeleteGarage(Dictionary<string, Garage<Vehicle>> garages) {
            var list = garages.Select((p) => new { entry = $"{p.Key} ({p.Value.Count}/{p.Value.Capacity})", key = p.Key }).ToList();
            Console.WriteLine("Pick a garage to delete:");
            var options = list.Select(x => x.entry).ToList();
            options.Add("-Cancel-");
            var sel = Menu(options.ToArray());
            if (sel >= 0 && sel < garages.Count) {
                var entry = list[sel];
                garages.Remove(entry.key);
                Console.WriteLine($"Deleted Garage {entry.key}");
            }
        }

        private static void CreateGarage(Dictionary<string, Garage<Vehicle>> garages) {
            Console.WriteLine("Please enter a name for the garage: (enter nothing to cancel)");
            string name = Prompt();
            if (string.IsNullOrWhiteSpace(name)) return;

            string capacityInput;
            int capacity;
            do {
                Console.WriteLine("Please enter the capacity of the garage: (-1 to cancel)");
                capacityInput = Prompt();
            } while (!Int32.TryParse(capacityInput, out capacity));
            if (capacity > 0) {
                garages.Add(name, new Garage<Vehicle>(capacity));
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
                        if (selection > 0) {
                            Console.SetCursorPosition(0, startpos + selection);
                            Console.Write(' ');
                            selection--;
                        }
                        break;
                    case ConsoleKey.DownArrow:
                    case ConsoleKey.S:
                    case ConsoleKey.J:
                        if (selection < options.Length-1) {
                            Console.SetCursorPosition(0, startpos + selection);
                            Console.Write(' ');
                            selection++;
                        }
                        break;
                    case ConsoleKey.Enter:
                        finished = true;
                        break;
                    default:
                        break;
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

        static string FormEntry(string field, string defaultValue) {
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
                        entry += input.KeyChar;
                        break;
                }
            }
            Console.CursorVisible = false;
            Console.WriteLine();
            return entry;
        }

        static int FormEntry(string field, int? defaultValue) {
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
