using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Speedycloud.Bytecode;
using Speedycloud.Compiler.AST_Nodes;
using Speedycloud.Compiler.TypeChecker.Constraints;
using Array = Speedycloud.Compiler.AST_Nodes.Array;
using Boolean = Speedycloud.Compiler.AST_Nodes.Boolean;
using String = Speedycloud.Compiler.AST_Nodes.String;
using Type = Speedycloud.Compiler.AST_Nodes.Type;

namespace Speedycloud.Compiler.TypeChecker {
    public class Typechecker : IAstVisitor<ITypeInformation> {
        private CascadingDictionary<string, BindingInformation> names = new CascadingDictionary<string, BindingInformation>(); 
        public CascadingDictionary<string, BindingInformation> Names { get { return names; } }

        public HashSet<FunctionType> Functions { get { return new HashSet<FunctionType>(functions); } }
        private readonly HashSet<FunctionType> functions = new HashSet<FunctionType>();

        private readonly Dictionary<FunctionSignature, FunctionType> functionDefinitions =
            new Dictionary<FunctionSignature, FunctionType> {};
        public Dictionary<FunctionSignature, FunctionType> FunctionDefinitions { get { return functionDefinitions; } } 
        private readonly Dictionary<FunctionCall, FunctionType> functionCalls =
            new Dictionary<FunctionCall, FunctionType>();
        public Dictionary<FunctionCall, FunctionType> FunctionCalls { get { return functionCalls; } } 

        private Dictionary<string, RecordTypeInformation> records = new Dictionary<string, RecordTypeInformation>();
        public Dictionary<string, RecordTypeInformation> Records { get { return records; } } 

        private CascadingDictionary<string, ITypeInformation> types = new CascadingDictionary<string, ITypeInformation>{
            {"Integer", new IntegerType()},
            {"Double", new DoubleType()},
            {"Boolean", new BooleanType()},
            {"String", new StringType()},
            {"Void", new UnknownType()},
            {"Any", new AnyType()}
        };

        private void NewScope() {
            names = new CascadingDictionary<string, BindingInformation>(names);
            //types = new CascadingDictionary<string, ITypeInformation>(types);
        }

        private void DeleteTopScope() {
            //types = types.Parent;
            names = names.Parent;
        }

        public Typechecker() {}

        public Typechecker(Dictionary<FunctionDefinition, FunctionType> preludeFunctionTypes) {
            foreach (var preludeFunctionType in preludeFunctionTypes) {
                functions.Add(preludeFunctionType.Value);
                functionDefinitions[preludeFunctionType.Key.Signature] = preludeFunctionType.Value;
            }
        }
        public ITypeInformation Visit(INode node) {

            return node.Accept(this);
        }

        public ITypeInformation Visit(Array array) {
            if (array.Expressions.Any()) {
                var type = array.Expressions.Select(Visit).Aggregate((fst, snd) => fst.Union(snd));
                return new ConstrainedType(new ArrayType(type), new Eq(array.Expressions.Count()));
            }
            return new ConstrainedType(new ArrayType(new AnyType()), new Eq(0));
        }

        public ITypeInformation Visit(ArrayIndex arrayIndex) {
            var arr = Visit(arrayIndex.Array);
            var index = Visit(arrayIndex.Index);
            Program.LogIn("Typechecker", "Checking array index " + arrayIndex);
            Program.Log("Typechecker", string.Format("Array {0} is array?", arr));
            if (!arr.IsAssignableTo(new ArrayType(new AnyType()))) {
                throw TypeCheckException.TypeMismatch(new ArrayType(new AnyType()), arr);
            }
            Program.Log("Typechecker", string.Format("Index {0} numeric?", index));
            if (!index.IsAssignableTo(new IntegerType())) {
                throw TypeCheckException.TypeMismatch(new IntegerType(), index);
            }
            var probablyValidIndex = TryCheckArrayBounds(arr, index);
            if (!probablyValidIndex) {
                throw TypeCheckException.IndexOutOfRange(arr, index);
            }
            Program.LogOut("Typechecker", "Array index is good.");
            return GetInnerArrayType(arr);
        }

        private bool TryCheckArrayBounds(ITypeInformation array, ITypeInformation index) {
            if (index is IntegerType) {
                Program.Log("Typechecker", string.Format("Index is probably okay. We don't have the type information to be sure."));
                //We have no index information!
                return true;
            }
            if (array is ArrayType) {
                Program.Log("Typechecker", string.Format("Index is probably okay. We don't have the array length information to be sure."));
                //We have no length information. Whoops.
                return true;
            }
            var upperBound = ((Eq)((ConstrainedType) array).Constraint).Num;
            var bounds = new AndConstraint(new Gt(-1), new Lt(upperBound));
            Program.Log("Typechecker", string.Format("Array index {0} fits into {1}?", index, bounds));
            return index.IsAssignableTo(new ConstrainedType(new IntegerType(), bounds));
        }

        public ITypeInformation Visit(ArrayAssignment assignment) {
            var arr = Visit(assignment.Array);
            var index = Visit(assignment.Index);
            var value = Visit(assignment.Value);
            Program.LogIn("Typechecker", "Checking array assignment " + assignment);
            Program.Log("Typechecker", string.Format("Array {0} is array?", arr));
            if (!arr.IsAssignableTo(new ArrayType(new AnyType()))) {
                throw TypeCheckException.TypeMismatch(new ArrayType(new AnyType()), arr);
            }
            Program.Log("Typechecker", string.Format("Index {0} is numeric?", index));
            if (!index.IsAssignableTo(new IntegerType())) {
                throw TypeCheckException.TypeMismatch(new IntegerType(), index);
            }
            var expected = ((ArrayType) ((ConstrainedType) arr).Type).Type;
            Program.Log("Typechecker", string.Format("Value {0} can be assigned to array type {1}?", value, expected));
            if (!value.IsAssignableTo(expected)) {
                throw TypeCheckException.TypeMismatch(expected, value);
            }

            var probablyValidIndex = TryCheckArrayBounds(arr, index);
            if (!probablyValidIndex) {
                throw TypeCheckException.IndexOutOfRange(arr, index);
            }
            Program.LogOut("Typechecker", "Array assignment is good.");
            return new UnknownType();
        }

        public ITypeInformation Visit(Assignment assignment) {
            Program.LogIn("Typechecker", string.Format("Checking assignment {0}", assignment));
            Program.Log("Typechecker", string.Format("Binding {0} is assignable?", assignment));
            if (!names[assignment.Binding.Value].Writable) {
                throw TypeCheckException.ReadonlyAssignment(assignment);
            }
            var expected = names[assignment.Binding.Value].Type;
            var actual = Visit(assignment.Expression);
            Program.Log("Typechecker", string.Format("Value {0} assignable to {1}?", actual, expected));
            if (!actual.IsAssignableTo(expected)) {
                throw TypeCheckException.TypeMismatch(expected, actual);
            }
            Program.LogOut("Typechecker", "Assignment is good.");
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
                {"Gt", n=>new Gt(n)},
                {"Mod", n=>new Mod(n)}
            };
            var num = ((Integer) constraint.Expression).Num;
            return constraints[constraint.Name](num);
        }

        public ITypeInformation Visit(BindingDeclaration declaration) {
            var name = declaration.Name.Value;
            var type = Visit(declaration.Type);
            names[name] = new BindingInformation(name, type, true);
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
            Program.LogIn("Typechecker", "Checking for loop " + forStatement);
            Program.Log("Typechecker", string.Format("Array {0} is array?", enumerableType));
            if (!enumerableType.IsAssignableTo(new ArrayType(new AnyType()))) {
                throw TypeCheckException.TypeMismatch(new ArrayType(new AnyType()), enumerableType);
            }
            Visit(forStatement.Binding);
            var arrayType = GetInnerArrayType(enumerableType);
            if (names[forStatement.Binding.Name.Value].Type is AnyType) {
                names[forStatement.Binding.Name.Value] = names[forStatement.Binding.Name.Value].WithType(arrayType);
            }
            Program.Log("Typechecker", string.Format("Array {0} and binding {1} match?", arrayType, names[forStatement.Binding.Name.Value].Type));
            if (!arrayType.IsAssignableTo(names[forStatement.Binding.Name.Value].Type)) {
                throw TypeCheckException.TypeMismatch(names[forStatement.Binding.Name.Value].Type, arrayType);
            }

            NewScope();
            Visit(forStatement.Executable);
            DeleteTopScope();
            DeleteTopScope();
            Program.LogOut("Typechecker", "For loop is good.");
            return new UnknownType();
        }

        private ITypeInformation GetInnerArrayType(ITypeInformation arr) {
            var subtype = arr;
            if (subtype is StringType) {
                return new IntegerType();
            }
            if (subtype is ConstrainedType) {
                subtype = ((ConstrainedType) arr).Type;
            }
            if (subtype is ArrayType) {
                return ((ArrayType) subtype).Type;
            }
            throw TypeCheckException.TypeMismatch(new ArrayType(new AnyType()), arr);
        }

        public ITypeInformation Visit(FunctionCall call) {
            Program.LogIn("Typechecker", "Checking function call " + call);
            if (records.ContainsKey(call.Name)) {
                var record = records[call.Name];
                var types = call.Parameters.Select(Visit);
                var parameterMatch = record.ParameterTypes.Zip(types, (expected, actual) => new {expected, actual}).Where(pt => record.TypeParameters.Contains(pt.expected.Name));
                var typeParameters = new Dictionary<TypeName, ITypeInformation>();
                foreach (var typeParam in parameterMatch) {
                    var expected = typeParam.expected;
                    var actual = typeParam.actual;
                    typeParameters[expected.Name] = actual;
                }
                InstantiateRecordType(call.Name, typeParameters);
            }
            var definitions = functions.Where(f=>f.Name == call.Name).ToList();
            var validDefinitions = new List<FunctionType>();
            Program.LogIn("Typechecker", "Searching for overload match...");
            foreach (var def in definitions) {
                var correctTypeMatch =
                    def.Parameters.Zip(call.Parameters, (type, expr) => Visit(expr).IsAssignableTo(type)).All(t => t);
                if (!correctTypeMatch) {
                    continue;
                }
                validDefinitions.Add(def);
            }

            if (validDefinitions.Count == 1) {
                var def = validDefinitions.First();
                Program.LogOut("Typechecker", "Overload " + def + " matched, and there are no other possible matches.");
                Program.LogOut("Typechecker", "Function call is good.");
                functionCalls[call] = def;
                if (records.ContainsKey(call.Name)) {
                    ClearRecordType(call.Name);
                }
                return def.ReturnType;
            }
            else {
                var exactMatches =
                    validDefinitions.Where(
                        def => def.Parameters.Zip(call.Parameters, (type, expr) => Visit(expr).Equals(type)).All(t => t)).ToList();
                if (exactMatches.Count() == 1) {
                    var def = exactMatches.First();
                    functionCalls[call] = def;
                    if (records.ContainsKey(call.Name)) {
                        ClearRecordType(call.Name);
                    }
                    Program.LogOut("Typechecker", "Multiple possible overloads found, but " + def + " is the only exact match.");
                    Program.LogOut("Typechecker", "Function call is good.");
                    return def.ReturnType;
                }
            }
            throw TypeCheckException.UnknownOverload(call, definitions);
        }

        private void InstantiateRecordType(string name, Dictionary<TypeName, ITypeInformation> typeParameters = null) {
            if (types.ContainsKey(name)) {
                return;
            }
            typeParameters = typeParameters ?? new Dictionary<TypeName, ITypeInformation>();
            var recordType = records[name].BuildType(typeParameters);
            var type = recordType.Item1;
            var ctor = recordType.Item2;
            var accessors = recordType.Item3;
            if (!functions.Any(f=>f.Equals(ctor))){
                functions.Add(ctor);
                foreach (var accessor in accessors) {
                    if (!functions.Contains(accessor)) {
                        functions.Add(accessor);
                    }
                }
            }
            types[name] = type;
            foreach (var typeInformation in typeParameters) {
                types[typeInformation.Key.Name] = typeInformation.Value;
            }
        }

        private void ClearRecordType(string name) {
            if (!types.ContainsKey(name)) {
                return;
            }
            types.Remove(name);
            foreach (var typeInformation in records[name].TypeParameters) {
                types.Remove(typeInformation.Name);
            }
        }

        public ITypeInformation Visit(FunctionDefinition def) {
            NewScope();
            Visit(def.Signature);
            Visit(def.Statement);
            DeleteTopScope();

            
            return new UnknownType();
        }

        public ITypeInformation Visit(FunctionSignature sig) {
            var name = sig.Name;
            var parameters = new List<ITypeInformation>();
            foreach (var binding in sig.Parameters) {
                Visit(binding);
                parameters.Add(names[binding.Name.Value].Type);
            }
            var returnType = Visit(sig.ReturnType);
            var func = new FunctionType(name, parameters, returnType);
            functions.Add(func);
            functionDefinitions[sig] = func;
            names["$RETURN"] = new BindingInformation("$RETURN", returnType, false);
            return new UnknownType();
        }

        public ITypeInformation Visit(If ifStatement) {
            var condition = Visit(ifStatement.Condition);
            NewScope();
            Program.Log("Typechecker", "Checking " + ifStatement);
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
            return names[name.Value].Type;
        }

        public ITypeInformation Visit(NewAssignment assignment) {
            if (names.TopLevel.ContainsKey(assignment.Declaration.Name.Value)) {
                throw TypeCheckException.BindingReassignmentInSameScope(assignment);
            }
            Program.LogIn("Typechecker", "Checking " + assignment);
            Visit(assignment.Declaration);
            var declaredType = names[assignment.Declaration.Name.Value].Type;

            var assignmentType = Visit(assignment.Assignment);
            if (declaredType is AnyType) {
                declaredType = assignmentType.LeastSpecificType();
                names[assignment.Declaration.Name.Value] = names[assignment.Declaration.Name.Value].WithType(declaredType);
            }
            var flagAwareType = SplitFlag(declaredType);
            var flagAssignment = SplitFlag(assignmentType);
            Program.Log("Typechecker",
                string.Format("Value {0} is assignable to type {1}, and the flags ({2}, {3}) match.", assignmentType,
                    flagAwareType.Item1, flagAwareType.Item2, flagAssignment.Item2));
            if (!assignmentType.IsAssignableTo(flagAwareType.Item1) || !flagAwareType.Item2.Equals(flagAssignment.Item2)) {
                throw TypeCheckException.TypeMismatch(declaredType, assignmentType);
            }

            if (!assignment.IsWritable) {
                names[assignment.Declaration.Name.Value] = names[assignment.Declaration.Name.Value].WithWritable(false);
            }
            Program.LogOut("Typechecker", "Assignment is good.");
            return new UnknownType();
        }

        private Tuple<ITypeInformation, Flag> SplitFlag(ITypeInformation flagType) {
            var con = flagType as ConstrainedType;
            if (con == null) {
                return Tuple.Create(flagType, new Flag());
            }
            var flag = con.Constraint as Flag;
            if (flag != null) {
                return Tuple.Create(con.Type, flag);
            }
            return Tuple.Create(flagType, new Flag());
        }

        public ITypeInformation Visit(AST_Nodes.Block block) {
            NewScope();
            foreach (var statement in block.Statements) {
                Visit(statement);
            }
            DeleteTopScope();
            return new UnknownType();
        }

        public ITypeInformation Visit(AST_Nodes.Program program) {
            foreach (var type in program.Nodes.OfType<Record>()) {
                Visit(type);
            }
            foreach (var function in program.Nodes.OfType<FunctionDefinition>()) {
                NewScope();
                Visit(function.Signature);
                DeleteTopScope();
            }
            foreach (var node in program.Nodes) {
                Visit(node);
            }
            return new UnknownType();
        }

        public ITypeInformation Visit(Record record) {
            var name = record.Name;
            var typeParams = record.TypeParams;

            Func<Dictionary<TypeName, ITypeInformation>, Tuple<ComplexType, FunctionType, IEnumerable<FunctionType>>> builderFunction = parameters => {
                foreach (var typeInformation in parameters) {
                    types[typeInformation.Key.Name] = typeInformation.Value;
                }
                var members = record.Members.Select(m => new { Name = m.Name.Value, Type = Visit(m.Type) }).ToList();
                var memberTypes = members.Select(m => m.Type).ToList();
                var type = new ComplexType(name, parameters, memberTypes.ToArray());
                var ctor = new FunctionType(name, memberTypes, type);
                var accessors =
                    members.Select(
                        member => new FunctionType(member.Name, new List<ITypeInformation> {type}, member.Type));
                return Tuple.Create(type, ctor, accessors);
            };
            var paramTypes = record.Members.Select(m => m.Type).ToList();
            records[name] = new RecordTypeInformation(typeParams, paramTypes, builderFunction);

            return new UnknownType();
        }

        public ITypeInformation Visit(Return returnStatement) {
            var ret = Visit(returnStatement.Expression);
            if (!ret.IsAssignableTo(names["$RETURN"].Type)) {
                throw TypeCheckException.TypeMismatch(names["$RETURN"].Type, ret);
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
                if (records.ContainsKey(type.Name.Name)) {
                    var record = records[type.Name.Name];
                    var typeParameters = new Dictionary<TypeName, ITypeInformation>();
                    foreach (var parameters in record.TypeParameters.Zip(type.TypeParameters, (expected, actual)=>new{expected, actual})) {
                        typeParameters[parameters.expected] = Visit(parameters.actual);
                    }
                    InstantiateRecordType(type.Name.Name, typeParameters);
                }
                var baseType = types[type.Name.Name];
                var constraints = type.Constraints.Select(list=>list.Select(BuildConstraint).Aggregate((a, b)=>new AndConstraint(a, b))).ToList();
                var isArray = type.IsArrayType;

                ITypeInformation newType = baseType;

                if (constraints.Any()) {
                    newType = new ConstrainedType(newType, constraints.Aggregate((a, b) => new OrConstraint(a, b)));
                }
                if (isArray) {
                    newType = new ArrayType(newType);
                }
                if (type.Flag != "") {
                    newType = new ConstrainedType(newType, new Flag(type.Flag));
                }
                if (records.ContainsKey(type.Name.Name)) {
                    ClearRecordType(type.Name.Name);
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
