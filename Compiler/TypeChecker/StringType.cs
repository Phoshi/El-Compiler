using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speedycloud.Compiler.TypeChecker {
    public class StringType : ITypeInformation{
        public bool IsAssignableTo(ITypeInformation other) {
            return other is StringType;
        }

        public bool Equals(ITypeInformation other) {
            return other is StringType;
        }

        public bool IsSubType(ITypeInformation other) {
            return false;
        }

        public bool IsSuperType(ITypeInformation other) {
            return false;
        }

        public ITypeInformation Union(ITypeInformation other) {
            if (other is StringType) {
                return new StringType();
            }
            throw TypeCheckException.UnresolvedUnion(this, other);
        }
    }
}
