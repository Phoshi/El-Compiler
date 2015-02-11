using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speedycloud.Compiler.TypeChecker {
    class UnknownType : ITypeInformation {
        public bool IsAssignableTo(ITypeInformation other) {
            return false;
        }

        public bool Equals(ITypeInformation other) {
            return false;
        }

        public bool IsSubType(ITypeInformation other) {
            return false;
        }

        public bool IsSuperType(ITypeInformation other) {
            return false;
        }

        public ITypeInformation Union(ITypeInformation other) {
            throw TypeCheckException.UnknowableTypeUsage(other);
        }

        public override string ToString() {
            return "(Unknown)";
        }
    }
}
