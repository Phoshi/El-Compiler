using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Speedycloud.Compiler.AST_Nodes;

namespace Speedycloud.Compiler.TypeChecker {
    public class IntegerType : ITypeInformation{
        public bool IsAssignableTo(ITypeInformation other) {
            if (other is AnyType) return true;
            if (other is IntegerType) {
                return true;
            }
            if (other is DoubleType) {
                return true;
            }
            return false;
        }

        public bool Equals(ITypeInformation other) {
            if (other is AnyType) return true;
            return IsAssignableTo(other);
        }

        public bool IsSubType(ITypeInformation other) {
            if (other is AnyType) return true;
            return other is DoubleType;
        }

        public override string ToString() {
            return "(Integer)";
        }

        public bool IsSuperType(ITypeInformation other) {
            if (other is AnyType) return true;
            return false;
        }

        public ITypeInformation Union(ITypeInformation other) {
            if (other is AnyType) return this;
            if (other is IntegerType) {
                return new IntegerType();
            }
            if (other is DoubleType) {
                return new DoubleType();
            }
            throw TypeCheckException.UnresolvedUnion(this, other);
        }

        public ITypeInformation UnaryOp(string op) {
            return new IntegerType();
        }

        public ITypeInformation BinaryOp(string op, ITypeInformation rhs) {
            if (rhs is IntegerType) {
                return new IntegerType();
            }
            if (rhs is DoubleType) {
                return new DoubleType();
            }
            throw TypeCheckException.InvalidBinaryOp(this, op, rhs);
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
