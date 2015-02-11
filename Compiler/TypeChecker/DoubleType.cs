using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Speedycloud.Compiler.AST_Nodes;

namespace Speedycloud.Compiler.TypeChecker {
    public class DoubleType : ITypeInformation{
        public bool IsAssignableTo(ITypeInformation other) {
            return other is DoubleType;
        }

        public bool Equals(ITypeInformation other) {
            return other is DoubleType;
        }

        public bool IsSubType(ITypeInformation other) {
            return false;
        }

        public override string ToString() {
            return "(Double)";
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((DoubleType) obj);
        }

        public override int GetHashCode() {
            throw new NotImplementedException();
        }

        public bool IsSuperType(ITypeInformation other) {
            return other is IntegerType;
        }

        public ITypeInformation Union(ITypeInformation other) {
            if (other is DoubleType) {
                return new DoubleType();
            }
            if (other is IntegerType) {
                return new DoubleType();
            }
            throw TypeCheckException.UnresolvedUnion(this, other);
        }
    }
}
