using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speedycloud.Compiler.AST_Nodes {
    class If : IStatement{
        public IExpression Condition { get; private set; }
        public IStatement Concequent { get; private set; }
        public IStatement Otherwise { get; private set; }

        public override string ToString() {
            return string.Format("(If {0} {1} {2})", Condition, Concequent, Otherwise);
        }

        protected bool Equals(If other) {
            return Equals(Condition, other.Condition) && Equals(Concequent, other.Concequent) && Equals(Otherwise, other.Otherwise);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((If) obj);
        }

        public override int GetHashCode() {
            unchecked {
                int hashCode = (Condition != null ? Condition.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Concequent != null ? Concequent.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Otherwise != null ? Otherwise.GetHashCode() : 0);
                return hashCode;
            }
        }

        public If(IExpression condition, IStatement concequent, IStatement otherwise) {
            Condition = condition;
            Concequent = concequent;
            Otherwise = otherwise;
        }

        public T Accept<T>(IAstVisitor<T> visitor) {
            return visitor.Visit(this);
        }
    }
}
