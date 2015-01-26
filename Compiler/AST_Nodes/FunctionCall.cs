using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speedycloud.Compiler.AST_Nodes {
    class FunctionCall : IExpression{
        public string Name { get; private set; }

        public override string ToString() {
            return string.Format("(FunctionCall {0} {1})", Name, string.Join(", ", Parameters));
        }

        protected bool Equals(FunctionCall other) {
            return string.Equals(Name, other.Name) && Parameters.SequenceEqual(other.Parameters);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((FunctionCall) obj);
        }

        public override int GetHashCode() {
            unchecked {
                return ((Name != null ? Name.GetHashCode() : 0)*397) ^ (Parameters != null ? Parameters.GetHashCode() : 0);
            }
        }

        public IEnumerable<IExpression> Parameters { get; private set; }

        public FunctionCall(string name, IEnumerable<IExpression> parameters) {
            Name = name;
            Parameters = parameters;
        }

        public T Accept<T>(IAstVisitor<T> visitor) {
            return visitor.Visit(this);
        }
    }
}
