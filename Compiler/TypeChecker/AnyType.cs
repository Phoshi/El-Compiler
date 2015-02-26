using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Speedycloud.Compiler.AST_Nodes;

namespace Speedycloud.Compiler.TypeChecker {
    public class AnyType : ITypeInformation {
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

        public ITypeInformation UnaryOp(string op) {
            throw TypeCheckException.UnknowableTypeUsage(this);
        }

        public ITypeInformation BinaryOp(string op, ITypeInformation rhs) {
            throw TypeCheckException.UnknowableTypeUsage(this);
        }

        public ITypeInformation LeastSpecificType() {
            return new AnyType();
        }

        public override string ToString() {
            return "(Any)";
        }
    }
}
