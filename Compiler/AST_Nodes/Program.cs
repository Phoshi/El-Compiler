using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speedycloud.Compiler.AST_Nodes {
    public class Program : INode {
        public IEnumerable<INode> Nodes { get; private set; }

        public Program(IEnumerable<INode> nodes) {
            Nodes = nodes;
        }

        public override string ToString() {
            return string.Format("(Program {0})", string.Join(", ", Nodes));
        }

        protected bool Equals(Program other) {
            return Nodes.SequenceEqual(other.Nodes);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Program) obj);
        }

        public override int GetHashCode() {
            return (Nodes != null ? Nodes.GetHashCode() : 0);
        }

        public T Accept<T>(IAstVisitor<T> visitor) {
            return visitor.Visit(this);
        }
    }
}
