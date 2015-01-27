using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speedycloud.Compiler.AST_Nodes {
    public class For : IStatement{
        public BindingDeclaration Binding { get; private set; }
        public IExpression Enumerable { get; private set; }
        public IStatement Executable { get; private set; }

        public For(BindingDeclaration binding, IExpression enumerable, IStatement executable) {
            Binding = binding;
            Enumerable = enumerable;
            Executable = executable;
        }

        public override string ToString() {
            return string.Format("(For {0} {1} {2})", Binding, Enumerable, Executable);
        }

        protected bool Equals(For other) {
            return Equals(Binding, other.Binding) && Equals(Enumerable, other.Enumerable) && Equals(Executable, other.Executable);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((For) obj);
        }

        public override int GetHashCode() {
            unchecked {
                int hashCode = (Binding != null ? Binding.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Enumerable != null ? Enumerable.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Executable != null ? Executable.GetHashCode() : 0);
                return hashCode;
            }
        }

        public T Accept<T>(IAstVisitor<T> visitor) {
            return visitor.Visit(this);
        }
    }
}
