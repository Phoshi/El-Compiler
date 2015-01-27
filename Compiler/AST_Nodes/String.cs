using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speedycloud.Compiler.AST_Nodes {
    public class String : IExpression{
        public string Str { get; private set; }

        public override string ToString() {
            return string.Format("(String {0})", Str);
        }

        protected bool Equals(String other) {
            return string.Equals(Str, other.Str);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((String) obj);
        }

        public override int GetHashCode() {
            return (Str != null ? Str.GetHashCode() : 0);
        }

        public String(string str) {
            Str = str;
        }

        public T Accept<T>(IAstVisitor<T> visitor) {
            return visitor.Visit(this);
        }
    }
}
