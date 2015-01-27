using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speedycloud.Compiler.AST_Nodes {
    public class Instance : IStatement {
        public string TypeclassName { get; private set; }
        public TypeName Parameter { get; private set; }
        public IEnumerable<FunctionDefinition> Definitions { get; private set; }

        public Instance(string typeclassName, TypeName parameter, IEnumerable<FunctionDefinition> definitions) {
            TypeclassName = typeclassName;
            Parameter = parameter;
            Definitions = definitions;
        }

        public override string ToString() {
            return string.Format("(Instance {0} {1} [{2}])", TypeclassName, Parameter, string.Join(", ", Definitions));
        }

        protected bool Equals(Instance other) {
            return string.Equals(TypeclassName, other.TypeclassName) && Equals(Parameter, other.Parameter) && Definitions.SequenceEqual(other.Definitions);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Instance) obj);
        }

        public override int GetHashCode() {
            unchecked {
                int hashCode = (TypeclassName != null ? TypeclassName.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Parameter != null ? Parameter.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Definitions != null ? Definitions.GetHashCode() : 0);
                return hashCode;
            }
        }

        public T Accept<T>(IAstVisitor<T> visitor) {
            return visitor.Visit(this);
        }
    }
}
