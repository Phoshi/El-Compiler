using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Threading.Tasks;

namespace Speedycloud.Compiler.AST_Nodes {
    public class NewAssignment : IStatement {
        public BindingDeclaration Declaration { get; private set; }
        public IExpression Assignment { get; private set; }
        public bool IsWritable { get; private set; }

        public NewAssignment(BindingDeclaration declaration, IExpression assignment, bool isWritable) {
            Declaration = declaration;
            Assignment = assignment;
            IsWritable = isWritable;
        }

        public override string ToString() {
            return string.Format("NewAssignment {0} {1} {2})", Declaration, Assignment,
                IsWritable ? "WRITABLE" : "READONLY");
        }

        protected bool Equals(NewAssignment other) {
            return Equals(Declaration, other.Declaration) && Equals(Assignment, other.Assignment) && IsWritable.Equals(other.IsWritable);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((NewAssignment) obj);
        }

        public override int GetHashCode() {
            unchecked {
                int hashCode = (Declaration != null ? Declaration.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Assignment != null ? Assignment.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ IsWritable.GetHashCode();
                return hashCode;
            }
        }

        public T Accept<T>(IAstVisitor<T> visitor) {
            return visitor.Visit(this);
        }
    }
}
