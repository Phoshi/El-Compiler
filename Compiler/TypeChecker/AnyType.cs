using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speedycloud.Compiler.TypeChecker {
    class AnyType : ITypeInformation {
        public bool IsAssignableTo(ITypeInformation other) {
            return true;
        }

        public bool Equals(ITypeInformation other) {
            return true;
        }

        public bool IsSubType(ITypeInformation other) {
            return true;
        }

        public bool IsSuperType(ITypeInformation other) {
            return true;
        }

        public ITypeInformation Union(ITypeInformation other) {
            return other;
        }

        public override string ToString() {
            return "(Any)";
        }
    }
}
