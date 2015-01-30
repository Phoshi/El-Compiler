namespace Speedycloud.Compiler.TypeChecker {
    public interface ITypeInformation {
        bool IsAssignableTo(ITypeInformation other);
        bool Equals(ITypeInformation other);
        bool IsSubType(ITypeInformation other);
        bool IsSuperType(ITypeInformation other);
        ITypeInformation Union(ITypeInformation other);
    }
}