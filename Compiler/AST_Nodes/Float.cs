using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speedycloud.Compiler.AST_Nodes {
    class Float :IExpression {
        public double Num { get; private set; }

        public Float(double num) {
            Num = num;
        }

        public T Accept<T>(IAstVisitor<T> visitor) {
            return visitor.Visit(this);
        }

        protected bool Equals(Float other) {
            return Num.Equals(other.Num);
        }

        public override string ToString() {
            return string.Format("(Float {0})", Num);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Float) obj);
        }

        public override int GetHashCode() {
            return Num.GetHashCode();
        }
    }
}
