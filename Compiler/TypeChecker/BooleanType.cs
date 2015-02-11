using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speedycloud.Compiler.TypeChecker {
    public class BooleanType : ITypeInformation{
        public bool IsAssignableTo(ITypeInformation other) {
            if (other is AnyType) return true;
            return other is BooleanType;
        }

        public bool Equals(ITypeInformation other) {
            if (other is AnyType) return true;
            return other is BooleanType;
        }

        public override string ToString() {
            return "(Boolean)";
        }

        public bool IsSubType(ITypeInformation other) {
            if (other is AnyType) return true;
            return false;
        }

        public bool IsSuperType(ITypeInformation other) {
            if (other is AnyType) return true;
            return false;
        }

        public ITypeInformation Union(ITypeInformation other) {
            if (other is AnyType) return this;
            if (other is BooleanType) {
                return new BooleanType();
            }
            throw TypeCheckException.UnresolvedUnion(this, other);
        }

        public ITypeInformation UnaryOp(string op) {
            return new BooleanType();
        }
    }
}
