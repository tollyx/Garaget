using System.Collections.Generic;
using System.Linq;

namespace Garage {
    public class Form {
        public readonly string Name;
        public readonly Field[] Fields;
        public object[] EntryObjects => Fields.Select(x => x.EntryObject).ToArray();

        internal Form(string name, List<Field> fields) : this(name, fields.ToArray()) {}

        internal Form(string name, Field[] fields) {
            Name = name;
            Fields = fields;
        }
    }
}