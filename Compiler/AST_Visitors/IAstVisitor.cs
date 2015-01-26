namespace Speedycloud.Compiler.AST_Nodes {
    internal interface IAstVisitor<T> {
        T Visit(INode node);
    }
}