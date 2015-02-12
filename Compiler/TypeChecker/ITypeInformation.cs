using Speedycloud.Compiler.AST_Nodes;

namespace Speedycloud.Compiler.TypeChecker {
    public interface ITypeInformation {
        bool IsAssignableTo(ITypeInformation other);
        bool Equals(ITypeInformation other);
        bool IsSubType(ITypeInformation other);
        bool IsSuperType(ITypeInformation other);
        ITypeInformation Union(ITypeInformation other);
        ITypeInformation UnaryOp(string op);
        ITypeInformation BinaryOp(string op, ITypeInformation rhs);
    }
}