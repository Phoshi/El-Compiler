using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speedycloud.Compiler.AST_Nodes {
    class Name : IExpression{
        public string Value { get; private set; }
        public bool IsWrite { get; private set; }

        public override string ToString() {
            return string.Format("(Name {0} {1})", Value, IsWrite ? "WRITE" : "READ");
        }

        protected bool Equals(Name other) {
            return string.Equals(Value, other.Value) && IsWrite.Equals(other.IsWrite);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Name) obj);
        }

        public override int GetHashCode() {
            unchecked {
                return ((Value != null ? Value.GetHashCode() : 0)*397) ^ IsWrite.GetHashCode();
            }
        }

        public Name(string value, bool isWrite) {
            Value = value;
            IsWrite = isWrite;
        }

        public T Accept<T>(IAstVisitor<T> visitor) {
            return visitor.Visit(this);
        }
    }
}
