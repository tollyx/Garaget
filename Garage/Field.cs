using System;

namespace Garage {
    public abstract class Field {
        internal Field(string name) {
            Name = name;
        }

        public readonly string Name;
        public abstract Type Type { get; }
        public virtual object EntryObject { get; set; }
    }

    public class Field<T> : Field {
        public T Entry {
            get {
                return (T)EntryObject;
            }
            set {
                EntryObject = value;
            }
        }
        public override Type Type => typeof(T);

        internal Field(string name) : base(name) {

        }

        internal Field(string name, T defaultvalue) : this(name) {
            Entry = defaultvalue;
        }
    }
}