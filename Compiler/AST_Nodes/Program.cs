using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speedycloud.Compiler.AST_Nodes {
    public class Program : INode{
        public IEnumerable<IStatement> Statements { get; private set; }

        public Program(IEnumerable<IStatement> statements) {
            Statements = statements;
        }

        public T Accept<T>(IAstVisitor<T> visitor) {
            return visitor.Visit(this);
        }

        protected bool Equals(Program other) {
            return Statements.SequenceEqual(other.Statements);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Program) obj);
        }

        public override int GetHashCode() {
            return (Statements != null ? Statements.GetHashCode() : 0);
        }

        public override string ToString() {
            return string.Format("(Program [{0}])", string.Join(", ", Statements));
        }
    }
}
