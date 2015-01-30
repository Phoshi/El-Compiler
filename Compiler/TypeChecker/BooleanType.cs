using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speedycloud.Compiler.TypeChecker {
    public class BooleanType : ITypeInformation{
        public bool IsAssignableTo(ITypeInformation other) {
            return other is BooleanType;
        }

        public bool Equals(ITypeInformation other) {
            return other is BooleanType;
        }

        public bool IsSubType(ITypeInformation other) {
            return false;
        }

        public bool IsSuperType(ITypeInformation other) {
            return false;
        }

        public ITypeInformation Union(ITypeInformation other) {
            if (other is BooleanType) {
                return new BooleanType();
            }
            throw TypeCheckException.UnresolvedUnion(this, other);
        }
    }
}
