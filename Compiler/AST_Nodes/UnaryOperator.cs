using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speedycloud.Compiler.AST_Nodes {
    class UnaryOperator : IExpression{
        public string Op { get; private set; }
        public IExpression Expr { get; private set; }

        public UnaryOperator(string op, IExpression expr) {
            Op = op;
            Expr = expr;
        }

        public override string ToString() {
            return string.Format("(UnaryOperator {0} {1})", Op, Expr);
        }

        protected bool Equals(UnaryOperator other) {
            return string.Equals(Op, other.Op) && Equals(Expr, other.Expr);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((UnaryOperator) obj);
        }

        public override int GetHashCode() {
            unchecked {
                return ((Op != null ? Op.GetHashCode() : 0)*397) ^ (Expr != null ? Expr.GetHashCode() : 0);
            }
        }

        public T Accept<T>(IAstVisitor<T> visitor) {
            return visitor.Visit(this);
        }
    }
}
