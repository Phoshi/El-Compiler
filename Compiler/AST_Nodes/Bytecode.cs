using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Speedycloud.Bytecode;

namespace Speedycloud.Compiler.AST_Nodes {
    public class Bytecode : IStatement {
        public IEnumerable<Opcode> Code { get; private set; }

        public override string ToString() {
            return string.Format("(Bytecode {0})", string.Join(", ", Code));
        }

        protected bool Equals(Bytecode other) {
            return Code.SequenceEqual(other.Code);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Bytecode) obj);
        }

        public override int GetHashCode() {
            return (Code != null ? Code.GetHashCode() : 0);
        }

        public Bytecode(IEnumerable<Opcode> code) {
            Code = code;
        }

        public T Accept<T>(IAstVisitor<T> visitor) {
            return visitor.Visit(this);
        }
    }
}
