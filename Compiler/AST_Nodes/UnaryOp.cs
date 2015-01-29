using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speedycloud.Compiler.AST_Nodes {
    public class UnaryOp : IExpression{
        public string Op { get; private set; }
        public IExpression Expression { get; private set; }

        public override string ToString() {
            return string.Format("(UnaryOp {0} {1})", Op, Expression);
        }

        protected bool Equals(UnaryOp other) {
            return string.Equals(Op, other.Op) && Equals(Expression, other.Expression);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((UnaryOp) obj);
        }

        public override int GetHashCode() {
            unchecked {
                return ((Op != null ? Op.GetHashCode() : 0)*397) ^ (Expression != null ? Expression.GetHashCode() : 0);
            }
        }

        public UnaryOp(string op, IExpression expression) {
            Op = op;
            Expression = expression;
        }

        public T Accept<T>(IAstVisitor<T> visitor) {
            return visitor.Visit(this);
        }
    }
}
