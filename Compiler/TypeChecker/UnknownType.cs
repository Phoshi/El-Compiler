using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Speedycloud.Compiler.AST_Nodes;

namespace Speedycloud.Compiler.TypeChecker {
    public class UnknownType : ITypeInformation {
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

        public ITypeInformation UnaryOp(string op) {
            throw TypeCheckException.UnknowableTypeUsage(this);
        }

        public ITypeInformation BinaryOp(string op, ITypeInformation rhs) {
            throw TypeCheckException.UnknowableTypeUsage(this);
        }

        public override string ToString() {
            return "(Unknown)";
        }
    }
}
