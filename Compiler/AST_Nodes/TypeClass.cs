using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speedycloud.Compiler.AST_Nodes {
    class TypeClass : IStatement{
        public string Name { get; private set; }

        public override string ToString() {
            return string.Format("(TypeClass {0} {1} [{2}])", Name, Parameter, string.Join(", ", Signatures));
        }

        public TypeName Parameter { get; private set; }
        public IEnumerable<FunctionSignature> Signatures { get; private set; }

        public TypeClass(string name, TypeName parameter, IEnumerable<FunctionSignature> signatures) {
            Name = name;
            Parameter = parameter;
            Signatures = signatures;
        }

        public T Accept<T>(IAstVisitor<T> visitor) {
            return visitor.Visit(this);
        }

        protected bool Equals(TypeClass other) {
            return string.Equals(Name, other.Name) && Equals(Parameter, other.Parameter) && Signatures.SequenceEqual(other.Signatures);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((TypeClass) obj);
        }

        public override int GetHashCode() {
            unchecked {
                int hashCode = (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Parameter != null ? Parameter.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Signatures != null ? Signatures.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}
