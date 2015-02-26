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
        private readonly Dictionary<string, ITypeInformation> names = new Dictionary<string, ITypeInformation>(); 
        public Dictionary<string, ITypeInformation> Names { get { return names; } }

        public HashSet<FunctionType> Functions { get { return new HashSet<FunctionType>(functions); } } 
        private readonly HashSet<FunctionType> functions = new HashSet<FunctionType>();

        private readonly Dictionary<FunctionDefinition, FunctionType> functionDefinitions =
            new Dictionary<FunctionDefinition, FunctionType>();
        public Dictionary<FunctionDefinition, FunctionType> FunctionDefinitions { get { return functionDefinitions; } } 
        private readonly Dictionary<FunctionCall, FunctionType> functionCalls =
            new Dictionary<FunctionCall, FunctionType>();
        public Dictionary<FunctionCall, FunctionType> FunctionCalls { get { return functionCalls; } } 

        private Dictionary<string, RecordTypeInformation> records = new Dictionary<string, RecordTypeInformation>();
        public Dictionary<string, RecordTypeInformation> Records { get { return records; } } 

        private CascadingDictionary<string, ITypeInformation> types = new CascadingDictionary<string, ITypeInformation>{
            {"Integer", new IntegerType()},
            {"Double", new DoubleType()},
            {"Boolean", new BooleanType()},
            {"String", new StringType()}
        };

        private void NewScope() {
            types = new CascadingDictionary<string, ITypeInformation>(types);
        }

        private void DeleteTopScope() {
            types = types.Parent;
        }
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
            var expected = names[assignment.Binding.Value];
            var actual = Visit(assignment.Expression);
            if (!actual.IsAssignableTo(expected)) {
                throw TypeCheckException.TypeMismatch(expected, actual);
            }
            return new UnknownType();
        }

        public ITypeInformation Visit(UnaryOp unaryOp) {
            return Visit(unaryOp.Expression).UnaryOp("-");
        }

        public ITypeInformation Visit(BinaryOp binaryOp) {
            return Visit(binaryOp.Lhs).BinaryOp(binaryOp.Op, Visit(binaryOp.Rhs));
        }

        private ITypeConstraint BuildConstraint(Constraint constraint) {
            var constraints = new Dictionary<string, Func<decimal, ITypeConstraint>> {
                {"Eq", n=>new Eq(n)},
                {"Lt", n=>new Lt(n)},
                {"Gt", n=>new Gt(n)}
            };
            var num = ((Integer) constraint.Expression).Num;
            return constraints[constraint.Name](num);
        }

        public ITypeInformation Visit(BindingDeclaration declaration) {
            var name = declaration.Name.Value;
            var type = Visit(declaration.Type);
            names[name] = type;
            return new UnknownType();
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
            NewScope();
            var enumerableType = Visit(forStatement.Enumerable);
            if (!enumerableType.IsAssignableTo(new ArrayType(new AnyType()))) {
                throw TypeCheckException.TypeMismatch(new ArrayType(new AnyType()), enumerableType);
            }
            Visit(forStatement.Binding);
            var arrayType = ((ArrayType) ((ConstrainedType) enumerableType).Type).Type;
            if (!arrayType.IsAssignableTo(names[forStatement.Binding.Name.Value])) {
                throw TypeCheckException.TypeMismatch(names[forStatement.Binding.Name.Value], arrayType);
            }

            NewScope();
            Visit(forStatement.Executable);
            DeleteTopScope();
            DeleteTopScope();
            return new UnknownType();
        }

        public ITypeInformation Visit(FunctionCall call) {
            var definitions = functions.Where(f=>f.Name == call.Name).ToList();
            foreach (var def in definitions) {
                var correctTypeMatch =
                    def.Parameters.Zip(call.Parameters, (type, expr) => Visit(expr).IsAssignableTo(type)).All(t => t);
                if (!correctTypeMatch) {
                    continue;
                }

                functionCalls[call] = def;
                return def.ReturnType;
            }
            throw TypeCheckException.UnknownOverload(call, definitions);
        }

        public ITypeInformation Visit(FunctionDefinition def) {
            NewScope();
            Visit(def.Signature);
            Visit(def.Statement);
            DeleteTopScope();

            functionDefinitions[def] = Functions.Last();
            return new UnknownType();
        }

        public ITypeInformation Visit(FunctionSignature sig) {
            var name = sig.Name;
            var parameters = new List<ITypeInformation>();
            foreach (var binding in sig.Parameters) {
                Visit(binding);
                parameters.Add(names[binding.Name.Value]);
            }
            var returnType = Visit(sig.ReturnType);
            functions.Add(new FunctionType(name, parameters, returnType));
            names["$RETURN"] = returnType;
            return new UnknownType();
        }

        public ITypeInformation Visit(If ifStatement) {
            var condition = Visit(ifStatement.Condition);
            NewScope();
            if (!condition.IsAssignableTo(new BooleanType())) {
                throw TypeCheckException.TypeMismatch(new BooleanType(), condition);
            }
            NewScope();
            Visit(ifStatement.Concequent);
            DeleteTopScope();
            NewScope();
            Visit(ifStatement.Otherwise);
            DeleteTopScope();
            DeleteTopScope();
            return new UnknownType();
        }

        public ITypeInformation Visit(Instance instance) {
            throw new NotImplementedException();
        }

        public ITypeInformation Visit(Integer integer) {
            return new ConstrainedType(new IntegerType(), new Eq(integer.Num));
        }

        public ITypeInformation Visit(Name name) {
            return names[name.Value];
        }

        public ITypeInformation Visit(NewAssignment assignment) {
            Visit(assignment.Declaration);
            var declaredType = names[assignment.Declaration.Name.Value];

            var assignmentType = Visit(assignment.Assignment);
            if (!assignmentType.IsAssignableTo(declaredType)) {
                throw TypeCheckException.TypeMismatch(declaredType, assignmentType);
            }
            return new UnknownType();
        }

        public ITypeInformation Visit(AST_Nodes.Block block) {
            foreach (var statement in block.Statements) {
                Visit(statement);
            }
            return new UnknownType();
        }

        public ITypeInformation Visit(AST_Nodes.Program program) {
            foreach (var node in program.Nodes) {
                Visit(node);
            }
            return new UnknownType();
        }

        public ITypeInformation Visit(Record record) {
            var name = record.Name;
            var typeParams = record.TypeParams;
            var members = record.Members.Select(m=>new {Name = m.Name.Value, Type = Visit(m.Type)}).ToList();

            var memberTypes = members.Select(m => m.Type).ToList();
            var type = new ComplexType(memberTypes.ToArray());
            var ctor = new FunctionType(name, memberTypes, type);
            var accessors =
                members.Select(member => new FunctionType(member.Name, new List<ITypeInformation> {type}, member.Type));

            records[name] = new RecordTypeInformation(type, ctor, accessors);

            functions.Add(ctor);
            foreach (var accessor in accessors) {
                functions.Add(accessor);
            }
            types[name] = type;

            return new UnknownType();
        }

        public ITypeInformation Visit(Return returnStatement) {
            var ret = Visit(returnStatement.Expression);
            if (!ret.IsAssignableTo(names["$RETURN"])) {
                throw TypeCheckException.TypeMismatch(names["$RETURN"], ret);
            }
            return new UnknownType();
        }

        public ITypeInformation Visit(String str) {
            return new ConstrainedType(new StringType(), new Eq(str.Str.Count()));
        }

        public ITypeInformation Visit(Type type) {
            if (type.IsRuntimeCheck) {
                return new AnyType();
            }
            else {
                var baseType = types[type.Name.Name];
                var constraints = type.Constraints.Select(BuildConstraint).ToList();
                var isArray = type.IsArrayType;

                ITypeInformation newType = baseType;

                if (constraints.Any()) {
                    newType = new ConstrainedType(newType, constraints.Aggregate((a, b) => new AndConstraint(a, b)));
                }
                if (isArray) {
                    newType = new ArrayType(newType);
                }
                return newType;
            }
        }

        public ITypeInformation Visit(TypeClass typeClass) {
            throw new NotImplementedException();
        }

        public ITypeInformation Visit(TypeName typeName) {
            throw new NotImplementedException();
        }

        public ITypeInformation Visit(While whileStatement) {
            var condition = Visit(whileStatement.Expression);
            if (!condition.IsAssignableTo(new BooleanType())) {
                throw TypeCheckException.TypeMismatch(new BooleanType(), condition);
            }
            Visit(whileStatement.Executable);
            return new UnknownType();
        }

        public ITypeInformation Visit(AST_Nodes.Bytecode code) {
            throw new NotImplementedException();
        }
    }
}
