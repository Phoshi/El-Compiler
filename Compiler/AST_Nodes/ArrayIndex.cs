using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speedycloud.Compiler.AST_Nodes {
    public class ArrayIndex :IExpression {
        public IExpression Array { get; private set; }
        public IExpression Index { get; private set; }

        public ArrayIndex(IExpression array, IExpression index) {
            Array = array;
            Index = index;
        }

        public T Accept<T>(IAstVisitor<T> visitor) {
            return visitor.Visit(this);
        }

        public override string ToString() {
            return string.Format("(ArrayIndex {0} {1})", Array, Index);
        }

        protected bool Equals(ArrayIndex other) {
            return Equals(Array, other.Array) && Equals(Index, other.Index);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ArrayIndex) obj);
        }

        public override int GetHashCode() {
            unchecked {
                return ((Array != null ? Array.GetHashCode() : 0)*397) ^ (Index != null ? Index.GetHashCode() : 0);
            }
        }
    }
}
