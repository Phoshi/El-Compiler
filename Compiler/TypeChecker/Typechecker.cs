using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Speedycloud.Compiler.AST_Nodes;
using Array = Speedycloud.Compiler.AST_Nodes.Array;
using Boolean = Speedycloud.Compiler.AST_Nodes.Boolean;
using String = Speedycloud.Compiler.AST_Nodes.String;
using Type = Speedycloud.Compiler.AST_Nodes.Type;

namespace Speedycloud.Compiler.TypeChecker {
    class Typechecker : IAstVisitor<ITypeInformation> {
        public ITypeInformation Visit(INode node) {
            throw new NotImplementedException();
        }

        public ITypeInformation Visit(Array array) {
            throw new NotImplementedException();
        }

        public ITypeInformation Visit(ArrayIndex arrayIndex) {
            throw new NotImplementedException();
        }

        public ITypeInformation Visit(ArrayAssignment assignment) {
            throw new NotImplementedException();
        }

        public ITypeInformation Visit(Assignment assignment) {
            throw new NotImplementedException();
        }

        public ITypeInformation Visit(UnaryOp unaryOp) {
            throw new NotImplementedException();
        }

        public ITypeInformation Visit(BinaryOp binaryOp) {
            throw new NotImplementedException();
        }

        public ITypeInformation Visit(BindingDeclaration declaration) {
            throw new NotImplementedException();
        }

        public ITypeInformation Visit(Boolean boolean) {
            throw new NotImplementedException();
        }

        public ITypeInformation Visit(Constraint constraint) {
            throw new NotImplementedException();
        }

        public ITypeInformation Visit(Float number) {
            throw new NotImplementedException();
        }

        public ITypeInformation Visit(For forStatement) {
            throw new NotImplementedException();
        }

        public ITypeInformation Visit(FunctionCall call) {
            throw new NotImplementedException();
        }

        public ITypeInformation Visit(FunctionDefinition def) {
            throw new NotImplementedException();
        }

        public ITypeInformation Visit(FunctionSignature sig) {
            throw new NotImplementedException();
        }

        public ITypeInformation Visit(If ifStatement) {
            throw new NotImplementedException();
        }

        public ITypeInformation Visit(Instance instance) {
            throw new NotImplementedException();
        }

        public ITypeInformation Visit(Integer integer) {
            throw new NotImplementedException();
        }

        public ITypeInformation Visit(Name name) {
            throw new NotImplementedException();
        }

        public ITypeInformation Visit(NewAssignment assignment) {
            throw new NotImplementedException();
        }

        public ITypeInformation Visit(AST_Nodes.Program program) {
            throw new NotImplementedException();
        }

        public ITypeInformation Visit(Record record) {
            throw new NotImplementedException();
        }

        public ITypeInformation Visit(Return returnStatement) {
            throw new NotImplementedException();
        }

        public ITypeInformation Visit(String str) {
            throw new NotImplementedException();
        }

        public ITypeInformation Visit(Type type) {
            throw new NotImplementedException();
        }

        public ITypeInformation Visit(TypeClass typeClass) {
            throw new NotImplementedException();
        }

        public ITypeInformation Visit(TypeName typeName) {
            throw new NotImplementedException();
        }

        public ITypeInformation Visit(While whileStatement) {
            throw new NotImplementedException();
        }

        public ITypeInformation Visit(AST_Nodes.Bytecode code) {
            throw new NotImplementedException();
        }
    }
}
