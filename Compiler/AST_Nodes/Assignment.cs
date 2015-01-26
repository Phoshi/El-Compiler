using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speedycloud.Compiler.AST_Nodes {
    class Assignment : IStatement{
        public Name Binding { get; private set; }
        public IExpression Expression { get; private set; }

        public override string ToString() {
            return string.Format("(Assignment {0} {1})", Binding, Expression);
        }

        protected bool Equals(Assignment other) {
            return Equals(Binding, other.Binding) && Equals(Expression, other.Expression);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Assignment) obj);
        }

        public override int GetHashCode() {
            unchecked {
                return ((Binding != null ? Binding.GetHashCode() : 0)*397) ^ (Expression != null ? Expression.GetHashCode() : 0);
            }
        }

        public Assignment(Name binding, IExpression expression) {
            Binding = binding;
            Expression = expression;
        }

        public T Accept<T>(IAstVisitor<T> visitor) {
            return visitor.Visit(this);
        }
    }
}
