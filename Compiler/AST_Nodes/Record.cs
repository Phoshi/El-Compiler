using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speedycloud.Compiler.AST_Nodes {
    public class Record : IExpression {
        public string Name { get; private set; }

        public override string ToString() {
            return string.Format("(Record {0} [{1}] [{2}])", Name, string.Join(", ", TypeParams), string.Join(", ", Members));
        }

        protected bool Equals(Record other) {
            return string.Equals(Name, other.Name) && TypeParams.SequenceEqual(other.TypeParams) &&
                   Members.SequenceEqual(other.Members);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Record) obj);
        }

        public override int GetHashCode() {
            unchecked {
                int hashCode = (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (TypeParams != null ? TypeParams.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Members != null ? Members.GetHashCode() : 0);
                return hashCode;
            }
        }

        public IEnumerable<TypeName> TypeParams { get; private set; }
        public IEnumerable<BindingDeclaration> Members { get; private set; }

        public Record(string name, IEnumerable<TypeName> typeParams, IEnumerable<BindingDeclaration> members) {
            Name = name;
            TypeParams = typeParams;
            Members = members;
        }

        public T Accept<T>(IAstVisitor<T> visitor) {
            return visitor.Visit(this);
        }
    }
}
