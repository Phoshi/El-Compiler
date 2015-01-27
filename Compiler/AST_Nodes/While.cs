using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speedycloud.Compiler.AST_Nodes {
    public class While : IStatement{
        public IExpression Expression { get; private set; }
        public IStatement Executable { get; private set; }

        public While(IExpression expression, IStatement executable) {
            Expression = expression;
            Executable = executable;
        }

        public override string ToString() {
            return string.Format("(While {0} {1})", Expression, Executable);
        }

        protected bool Equals(While other) {
            return Equals(Expression, other.Expression) && Equals(Executable, other.Executable);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((While) obj);
        }

        public override int GetHashCode() {
            unchecked {
                return ((Expression != null ? Expression.GetHashCode() : 0)*397) ^ (Executable != null ? Executable.GetHashCode() : 0);
            }
        }

        public T Accept<T>(IAstVisitor<T> visitor) {
            return visitor.Visit(this);
        }
    }
}
