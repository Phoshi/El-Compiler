using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speedycloud.Compiler.TypeChecker {
    public class BindingInformation {
        public string Name { get; private set; }
        public ITypeInformation Type { get; private set; }
        public bool Writable { get; private set; }

        public BindingInformation(string name, ITypeInformation type, bool writable) {
            Name = name;
            Type = type;
            Writable = writable;
        }

        public BindingInformation WithType(ITypeInformation type) {
            return new BindingInformation(Name, type, Writable);
        }

        public BindingInformation WithWritable(bool writable) {
            return new BindingInformation(Name, Type, writable);
        }

    }
}
