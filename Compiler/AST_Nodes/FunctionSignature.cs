using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Speedycloud.Compiler.AST_Nodes {
    public class FunctionSignature : INode {
        public string Name { get; private set; }

        public override string ToString() {
            return string.Format("(FunctionSignature {0} {1} {2})", Name, string.Join(", ", Parameters), ReturnType);
        }

        protected bool Equals(FunctionSignature other) {
            return string.Equals(Name, other.Name) && Parameters.SequenceEqual(other.Parameters) && Equals(ReturnType, other.ReturnType);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((FunctionSignature) obj);
        }

        public override int GetHashCode() {
            unchecked {
                int hashCode = (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Parameters != null ? Parameters.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (ReturnType != null ? ReturnType.GetHashCode() : 0);
                return hashCode;
            }
        }

        public IEnumerable<BindingDeclaration> Parameters { get; private set; }
        public Type ReturnType { get; private set; }

        public FunctionSignature(string name, IEnumerable<BindingDeclaration> parameters, Type returnType) {
            Name = name;
            Parameters = parameters;
            ReturnType = returnType;
        }

        public T Accept<T>(IAstVisitor<T> visitor) {
            return visitor.Visit(this);
        }
    }
}