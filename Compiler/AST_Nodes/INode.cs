using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speedycloud.Compiler.AST_Nodes {
    interface INode {
        T Accept<T>(IAstVisitor<T> visitor);
    }
}
