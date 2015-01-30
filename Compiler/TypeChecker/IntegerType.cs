using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speedycloud.Compiler.TypeChecker {
    public class IntegerType : ITypeInformation{
        public bool IsAssignableTo(ITypeInformation other) {
            if (other is IntegerType) {
                return true;
            }
            if (other is DoubleType) {
                return true;
            }
            return false;
        }

        public bool Equals(ITypeInformation other) {
            return IsAssignableTo(other);
        }

        public bool IsSubType(ITypeInformation other) {
            return other is DoubleType;
        }

        public bool IsSuperType(ITypeInformation other) {
            return false;
        }

        public ITypeInformation Union(ITypeInformation other) {
            if (other is IntegerType) {
                return new IntegerType();
            }
            if (other is DoubleType) {
                return new DoubleType();
            }
            throw TypeCheckException.UnresolvedUnion(this, other);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((IntegerType) obj);
        }

        public override int GetHashCode() {
            throw new NotImplementedException();
        }
    }
}
