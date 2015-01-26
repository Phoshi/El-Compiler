using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speedycloud.Compiler.AST_Nodes {
    class BinaryOp : IExpression{
        public string Op { get; private set; }
        public IExpression Lhs { get; private set; }
        public IExpression Rhs { get; private set; }

        public override string ToString() {
            return string.Format("(BinaryOp {0} {1} {2})", Op, Lhs, Rhs);
        }

        public BinaryOp(string op, IExpression lhs, IExpression rhs) {
            Op = op;
            Lhs = lhs;
            Rhs = rhs;
        }

        public T Accept<T>(IAstVisitor<T> visitor) {
            return visitor.Visit(this);
        }
    }
}
