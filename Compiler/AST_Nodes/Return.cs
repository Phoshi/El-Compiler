using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speedycloud.Compiler.AST_Nodes {
    class Return : IStatement {
        public IExpression Expression { get; private set; }

        public override string ToString() {
            return string.Format("(Return {0})", Expression);
        }

        protected bool Equals(Return other) {
            return Equals(Expression, other.Expression);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Return) obj);
        }

        public override int GetHashCode() {
            return (Expression != null ? Expression.GetHashCode() : 0);
        }

        public Return(IExpression expression) {
            Expression = expression;
        }

        public T Accept<T>(IAstVisitor<T> visitor) {
            return visitor.Visit(this);
        }
    }
}
