using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speedycloud.Compiler.AST_Nodes {
    public class BinaryOp : IExpression{
        public string Op { get; private set; }
        public IExpression Lhs { get; private set; }
        public IExpression Rhs { get; private set; }

        public override string ToString() {
            return string.Format("(BinaryOp {0} {1} {2})", Op, Lhs, Rhs);
        }

        public BinaryOp(string op, IExpression lhs, IExpression rhs) {
            Op = op;
            Lhs = lhs;
            Rhs = rhs;
        }

        protected bool Equals(BinaryOp other) {
            return string.Equals(Op, other.Op) && Equals(Lhs, other.Lhs) && Equals(Rhs, other.Rhs);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((BinaryOp) obj);
        }

        public override int GetHashCode() {
            unchecked {
                int hashCode = (Op != null ? Op.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Lhs != null ? Lhs.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Rhs != null ? Rhs.GetHashCode() : 0);
                return hashCode;
            }
        }

        public T Accept<T>(IAstVisitor<T> visitor) {
            return visitor.Visit(this);
        }
    }
}
