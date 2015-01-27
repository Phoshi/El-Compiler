using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speedycloud.Compiler.AST_Nodes {
    public class Boolean : IExpression{
        public bool Flag { get; private set; }

        protected bool Equals(Boolean other) {
            return Flag.Equals(other.Flag);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Boolean) obj);
        }

        public override int GetHashCode() {
            return Flag.GetHashCode();
        }

        public override string ToString() {
            return string.Format("(Boolean {0})", Flag);
        }

        public Boolean(bool flag) {
            Flag = flag;
        }

        public T Accept<T>(IAstVisitor<T> visitor) {
            return visitor.Visit(this);
        }
    }
}
