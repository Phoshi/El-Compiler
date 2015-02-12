using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speedycloud.Compiler.TypeChecker.Constraints {
    public interface ITypeConstraint {
        bool Equals(ITypeConstraint constraint);
        bool IsAssignableTo(ITypeConstraint constraint);
        bool IsSupertypeOf(ITypeConstraint constraint);
        bool IsSubtypeOf(ITypeConstraint constraint);
        ITypeConstraint UnaryOp(string op);
        ITypeConstraint BinaryOp(string op, ITypeConstraint constraint);
    }
}
