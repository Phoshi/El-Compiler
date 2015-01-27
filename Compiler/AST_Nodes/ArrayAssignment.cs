using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speedycloud.Compiler.AST_Nodes {
    class ArrayAssignment : IStatement {
        public IExpression Array { get; private set; }
        public IExpression Index { get; private set; }
        public IExpression Value { get; private set; }

        public ArrayAssignment(IExpression array, IExpression index, IExpression value) {
            Array = array;
            Index = index;
            Value = value;
        }

        protected bool Equals(ArrayAssignment other) {
            return Equals(Array, other.Array) && Equals(Index, other.Index) && Equals(Value, other.Value);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ArrayAssignment) obj);
        }

        public override int GetHashCode() {
            unchecked {
                int hashCode = (Array != null ? Array.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Index != null ? Index.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Value != null ? Value.GetHashCode() : 0);
                return hashCode;
            }
        }

        public override string ToString() {
            return string.Format("(ArrayAssignment {0} {1} {2})", Array, Index, Value);
        }

        public T Accept<T>(IAstVisitor<T> visitor) {
            return visitor.Visit(this);
        }
    }
}
