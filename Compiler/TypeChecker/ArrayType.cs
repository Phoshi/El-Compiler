using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Speedycloud.Compiler.AST_Nodes;

namespace Speedycloud.Compiler.TypeChecker {
    public class ArrayType : ITypeInformation {
        public ITypeInformation Type { get; set; }

        public ArrayType(ITypeInformation type) {
            Type = type;
        }

        public bool IsAssignableTo(ITypeInformation other) {
            if (other is AnyType) return true;
            return other is ArrayType && Type.IsAssignableTo(((ArrayType)other).Type);
        }

        public bool Equals(ITypeInformation other) {
            if (other is AnyType) return true;
            return other is ArrayType && Type.Equals(((ArrayType)other).Type);
        }

        public override string ToString() {
            return string.Format("(Array {0})", Type);
        }

        public bool IsSubType(ITypeInformation other) {
            if (other is AnyType) return true;
            if (other is ArrayType) return Type.IsSubType(((ArrayType) other).Type);
            return false;
        }

        public bool IsSuperType(ITypeInformation other) {
            if (other is AnyType) return true;
            if (other is ArrayType) return Type.IsSuperType(((ArrayType)other).Type);
            return false;
        }

        public ITypeInformation Union(ITypeInformation other) {
            if (other is AnyType) return this;
            if (other is ArrayType) {
                var otherType = (other as ArrayType);
                return new ArrayType(Type.Union(otherType));
            }
            throw TypeCheckException.UnresolvedUnion(this, other);
        }

        public ITypeInformation UnaryOp(string op) {
            throw new NotImplementedException();
        }

        public ITypeInformation BinaryOp(string op, ITypeInformation rhs) {
            throw new NotImplementedException();
        }
    }
}
