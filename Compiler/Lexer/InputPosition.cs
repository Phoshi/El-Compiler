using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speedycloud.Compiler.Lexer {
    public struct InputPosition {
        public InputPosition(int character, int line) : this() {
            Character = character;
            Line = line;
        }

        int Line { get; set; }
        int Character { get; set; }

        public override string ToString() {
            if (Line == 0 && Character == 0) {
                return "Unknown Position";
            }
            return string.Format("line {0}, position {1}", Line, Character);
        }
    }
}
