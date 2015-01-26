using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speedycloud.Compiler.AST_Nodes {
    class Array : IExpression{
        public IEnumerable<IExpression> Expressions { get; private set; }

        public override string ToString() {
            return string.Format("(Array [{0}])", string.Join(", ", Expressions));
        }

        protected bool Equals(Array other) {
            return Expressions.SequenceEqual(other.Expressions);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Array) obj);
        }

        public override int GetHashCode() {
            return (Expressions != null ? Expressions.GetHashCode() : 0);
        }

        public Array(IEnumerable<IExpression> expressions) {
            Expressions = expressions;
        }

        public T Accept<T>(IAstVisitor<T> visitor) {
            return visitor.Visit(this);
        }
    }
}
