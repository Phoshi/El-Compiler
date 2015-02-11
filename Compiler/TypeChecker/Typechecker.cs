using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Speedycloud.Compiler.AST_Nodes;
using Speedycloud.Compiler.TypeChecker.Constraints;
using Array = Speedycloud.Compiler.AST_Nodes.Array;
using Boolean = Speedycloud.Compiler.AST_Nodes.Boolean;
using String = Speedycloud.Compiler.AST_Nodes.String;
using Type = Speedycloud.Compiler.AST_Nodes.Type;

namespace Speedycloud.Compiler.TypeChecker {
    public class Typechecker : IAstVisitor<ITypeInformation> {
        public ITypeInformation Visit(INode node) {
            return node.Accept(this);
        }

        public ITypeInformation Visit(Array array) {
            if (array.Expressions.Any()) {
                var type = array.Expressions.Select(Visit).Aggregate((fst, snd) => fst.Union(snd));
                return new ConstrainedType(new ArrayType(type), new Eq(array.Expressions.Count()));
            }
            return new ConstrainedType(new ArrayType(new UnknownType()), new Eq(0));
        }

        public ITypeInformation Visit(ArrayIndex arrayIndex) {
            var arr = Visit(arrayIndex.Array);
            var index = Visit(arrayIndex.Index);
            if (!arr.IsAssignableTo(new ArrayType(new AnyType()))) {
                throw TypeCheckException.TypeMismatch(new ArrayType(new AnyType()), arr);
            }
            if (!index.IsAssignableTo(new IntegerType())) {
                throw TypeCheckException.TypeMismatch(new IntegerType(), index);
            }
            return ((ArrayType)((ConstrainedType) arr).Type).Type;
        }

        public ITypeInformation Visit(ArrayAssignment assignment) {
            var arr = Visit(assignment.Array);
            var index = Visit(assignment.Index);
            var value = Visit(assignment.Value);
            if (!arr.IsAssignableTo(new ArrayType(new AnyType()))) {
                throw TypeCheckException.TypeMismatch(new ArrayType(new AnyType()), arr);
            }
            if (!index.IsAssignableTo(new IntegerType())) {
                throw TypeCheckException.TypeMismatch(new IntegerType(), index);
            }
            var expected = ((ArrayType) ((ConstrainedType) arr).Type).Type;
            if (!value.IsAssignableTo(expected)) {
                throw TypeCheckException.TypeMismatch(expected, value);
            }
            var lowerBound = new ConstrainedType(new IntegerType(), new Gt(-1));
            var upperBound = new ConstrainedType(new IntegerType(), new Lt(((Eq)((ConstrainedType) arr).Constraint).Num));
            if (!index.IsAssignableTo(lowerBound)) {
                throw TypeCheckException.TypeMismatch(lowerBound, index);
            }
            if (!index.IsAssignableTo(upperBound)) {
                throw TypeCheckException.TypeMismatch(upperBound, index);
            }

            return new UnknownType();
        }

        public ITypeInformation Visit(Assignment assignment) {
            throw new NotImplementedException();
        }

        public ITypeInformation Visit(UnaryOp unaryOp) {
            return Visit(unaryOp.Expression).UnaryOp("-");
        }

        public ITypeInformation Visit(BinaryOp binaryOp) {
            throw new NotImplementedException();
        }

        public ITypeInformation Visit(BindingDeclaration declaration) {
            throw new NotImplementedException();
        }

        public ITypeInformation Visit(Boolean boolean) {
            return new BooleanType();
        }

        public ITypeInformation Visit(Constraint constraint) {
            throw new NotImplementedException();
        }

        public ITypeInformation Visit(Float number) {
            return new ConstrainedType(new DoubleType(), new Eq(new decimal(number.Num)));
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
            return new ConstrainedType(new IntegerType(), new Eq(integer.Num));
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
            return new ConstrainedType(new StringType(), new Eq(str.Str.Count()));
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
