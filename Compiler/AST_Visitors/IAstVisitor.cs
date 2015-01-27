namespace Speedycloud.Compiler.AST_Nodes {
    internal interface IAstVisitor<T> {
        T Visit(INode node);
        T Visit(Array array);
        T Visit(ArrayIndex arrayIndex);
        T Visit(Assignment assignment);
        T Visit(BinaryOp binaryOp);
        T Visit(BindingDeclaration declaration);
        T Visit(Boolean boolean);
        T Visit(Constraint constraint);
        T Visit(Float number);
        T Visit(For for);
        T Visit(FunctionCall call);
        T Visit(FunctionDefinition def);
        T Visit(FunctionSignature sig);
        T Visit(If ifStatement);
        T Visit(Instance instance);
        T Visit(Integer integer);
        T Visit(Name name);
        T Visit(NewAssignment assignment);
        T Visit(Program program);
        T Visit(Record record);
        T Visit(Return returnStatement);
        T Visit(String str);
        T Visit(Type type);
        T Visit(TypeClass typeClass);
        T Visit(TypeName typeName);
        T Visit(While whileStatement);
    }
}