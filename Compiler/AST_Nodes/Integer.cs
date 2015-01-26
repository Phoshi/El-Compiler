using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speedycloud.Compiler.AST_Nodes {
    class Integer : IExpression{
        public int Num { get; private set; }

        public Integer(int num) {
            Num = num;
        }

        public T Accept<T>(IAstVisitor<T> visitor) {
            return visitor.Visit(this);
        }

        protected bool Equals(Integer other) {
            return Num == other.Num;
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Integer) obj);
        }

        public override int GetHashCode() {
            return Num;
        }

        public override string ToString() {
            return string.Format("(Integer {0})", Num);
        }
    }
}
