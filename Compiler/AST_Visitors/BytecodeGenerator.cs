using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Speedycloud.Bytecode;
using Speedycloud.Bytecode.ValueTypes;
using Speedycloud.Compiler.AST_Nodes;
using Speedycloud.Runtime;
using Speedycloud.Runtime.ValueTypes;
using Array = Speedycloud.Compiler.AST_Nodes.Array;
using Boolean = Speedycloud.Compiler.AST_Nodes.Boolean;
using Name = Speedycloud.Compiler.AST_Nodes.Name;
using String = Speedycloud.Compiler.AST_Nodes.String;
using Type = Speedycloud.Compiler.AST_Nodes.Type;

namespace Speedycloud.Compiler.AST_Visitors {
    public class BytecodeGenerator : IAstVisitor<IEnumerable<Opcode>> {
        public Dictionary<int, IValue> Constants { get { return new Dictionary<int, IValue>(constTable);} } 
        private readonly Dictionary<int, IValue> constTable = new Dictionary<int, IValue>();

        private int AddConstant(IValue val) {
            constTable[constTable.Count] = val;
            return constTable.Count - 1;
        }

        private int AddConstant(long num) {
            return AddConstant(new IntValue(num));
        }

        private int AddConstant(string str) {
            return AddConstant(new StringValue(str));
        }

        private int AddConstant(double num) {
            return AddConstant(new DoubleValue(num));
        }

        private int AddConstant(bool flag) {
            return AddConstant(new BooleanValue(flag));
        }

        public Dictionary<int, FunctionDefinition> Functions { get { return new Dictionary<int, FunctionDefinition>(funcTable);} }
        private readonly Dictionary<int, FunctionDefinition> funcTable = new Dictionary<int, FunctionDefinition>();

        private int AddFunction(FunctionDefinition def) {
            var address = AddConstant(-funcTable.Count-1);
            funcTable[address] = def;
            return address;
        }

        private KeyValuePair<int, FunctionDefinition> GetFunctionByName(string name) {
            return funcTable.First(kv => kv.Value.Signature.Name == name);
        }

        public Dictionary<string, int> Names { get { return nameTable; } } 
        private readonly Dictionary<string, int> nameTable = new Dictionary<string, int>();
        private readonly Dictionary<string, int> funcParameterNames = new Dictionary<string, int>(); 

        private int GetNameEntry(string name) {
            if (funcParameterNames.ContainsKey(name)) {
                return funcParameterNames[name];
            }
            if (!nameTable.ContainsKey(name)) {
                nameTable[name] = name.GetHashCode();
            }
            return nameTable[name];
        }

        private Dictionary<string, TypeClass> typeclasses = new Dictionary<string, TypeClass>(); 

        public IEnumerable<Opcode> Finalise(IEnumerable<Opcode> opcodes) {
            var bytecode = new List<Opcode>();
            foreach (var functionDefinition in funcTable) {
                var i = 0;
                foreach (var parameter in functionDefinition.Value.Signature.Parameters) {
                    funcParameterNames[parameter.Name.Value] = i++;
                }
                constTable[functionDefinition.Key] = new IntValue(bytecode.Count);
                bytecode.AddRange(Visit(functionDefinition.Value.Statement));
                funcParameterNames.Clear();
                if (bytecode.Last().Instruction != Instruction.RETURN) {
                    bytecode.Add(new Opcode(Instruction.RETURN));
                }
            }

            bytecode.Add(new Opcode(Instruction.CODE_START));
            bytecode.AddRange(opcodes);
            bytecode.Add(new Opcode(Instruction.CODE_STOP));
            return bytecode;
        }

        public IEnumerable<Opcode> Visit(INode node) {
            return node.Accept(this);
        }

        public IEnumerable<Opcode> Visit(Array array) {
            var bytecode = new List<Opcode>();
            foreach (var expression in array.Expressions) {
                bytecode.AddRange(Visit(expression));
            }
            bytecode.Add(new Opcode(Instruction.MAKE_ARR, array.Expressions.Count()));

            return bytecode;
        }

        public IEnumerable<Opcode> Visit(ArrayIndex arrayIndex) {
            var bytecode = Visit(arrayIndex.Array).Concat(Visit(arrayIndex.Index)).ToList();
            bytecode.Add(new Opcode(Instruction.BINARY_INDEX));
            return bytecode;
        }

        public IEnumerable<Opcode> Visit(ArrayAssignment assignment) {
            var bytecode = Visit(assignment.Array).Concat(Visit(assignment.Index)).Concat(Visit(assignment.Value)).ToList();
            bytecode.Add(new Opcode(Instruction.BINARY_INDEX_UPDATE));
            return bytecode;
        }

        public IEnumerable<Opcode> Visit(Assignment assignment) {
            return Visit(assignment.Expression).Concat(Visit(assignment.Binding));
        }

        private readonly Dictionary<string, Instruction> unOpTable = new Dictionary<string, Instruction> {
            {"-", Instruction.UNARY_NEG},
            {"!", Instruction.UNARY_NOT},
        }; 
        public IEnumerable<Opcode> Visit(UnaryOp unaryOp) {
            var bytecode = Visit(unaryOp.Expression).ToList();
            bytecode.Add(new Opcode(unOpTable[unaryOp.Op]));
            return bytecode;
        }

        private readonly Dictionary<string, Instruction> binOpTable = new Dictionary<string, Instruction> {
            {"+", Instruction.BINARY_ADD},
            {"-", Instruction.BINARY_SUB},
            {"*", Instruction.BINARY_MUL},
            {"/", Instruction.BINARY_DIV},

            {"==", Instruction.BINARY_EQL},
            {"!=", Instruction.BINARY_NEQ},
            {">", Instruction.BINARY_GT},
            {"<", Instruction.BINARY_LT},
            {">=", Instruction.BINARY_GTE},
            {"<=",  Instruction.BINARY_LTE},

            {"&&", Instruction.BINARY_AND},
            {"||", Instruction.BINARY_OR},
        }; 
        public IEnumerable<Opcode> Visit(BinaryOp binaryOp) {
            var bytecode = Visit(binaryOp.Lhs).Concat(Visit(binaryOp.Rhs)).ToList();
            bytecode.Add(new Opcode(binOpTable[binaryOp.Op]));
            return bytecode;
        }

        public IEnumerable<Opcode> Visit(BindingDeclaration declaration) {
            //We are a code generator. We don't really care about types.
            return new List<Opcode>(); 
        }

        public IEnumerable<Opcode> Visit(Boolean boolean) {
            return new[] {new Opcode(Instruction.LOAD_CONST, AddConstant(boolean.Flag))};
        }

        public IEnumerable<Opcode> Visit(Constraint constraint) {
            return new List<Opcode>();
        }

        public IEnumerable<Opcode> Visit(Float number) {
            var constReference = AddConstant(number.Num);
            return new[] {new Opcode(Instruction.LOAD_CONST, constReference)};
        }

        public IEnumerable<Opcode> Visit(For forStatement) {
            var loopIter = GetNameEntry("LOOP_ITER");
            var loopCount = GetNameEntry("LOOP_COUNT");

            var func = AddFunction(new FunctionDefinition(
                new FunctionSignature("LOOP_FUNC", new List<BindingDeclaration> {forStatement.Binding},
                    new Type(new TypeName("Void"))), forStatement.Executable));

            //Get the enumerable
            var bytecode = Visit(forStatement.Enumerable).ToList();
            //Store it in the heap for now
            bytecode.Add(new Opcode(Instruction.STORE_NEW_NAME, loopIter, -1));
            //Make and load the counter
            var zero = AddConstant(0);
            bytecode.Add(new Opcode(Instruction.LOAD_CONST, zero));
            bytecode.Add(new Opcode(Instruction.STORE_NEW_NAME, loopCount, -1));
            //Check to see if the loop is iterated
            bytecode.Add(new Opcode(Instruction.LOAD_NAME, loopIter));
            bytecode.Add(new Opcode(Instruction.LOAD_ATTR, 0));
            bytecode.Add(new Opcode(Instruction.LOAD_NAME, loopCount));
            bytecode.Add(new Opcode(Instruction.BINARY_EQL));
            bytecode.Add(new Opcode(Instruction.JUMP_TRUE, 9)); //Loop body is in a function, so this is actually constant
            //Set up and run loop body
            bytecode.Add(new Opcode(Instruction.LOAD_NAME, loopIter));
            bytecode.Add(new Opcode(Instruction.LOAD_NAME, loopCount));
            bytecode.Add(new Opcode(Instruction.BINARY_INDEX));
            bytecode.Add(new Opcode(Instruction.CALL_FUNCTION, func, 1));
            //Increment counter and iterate
            bytecode.Add(new Opcode(Instruction.LOAD_NAME, loopCount));
            bytecode.Add(new Opcode(Instruction.LOAD_CONST, AddConstant(1)));
            bytecode.Add(new Opcode(Instruction.BINARY_ADD));
            bytecode.Add(new Opcode(Instruction.STORE_NAME, loopCount));
            //Jump back
            bytecode.Add(new Opcode(Instruction.JUMP, -14));


            return bytecode;
        }

        public IEnumerable<Opcode> Visit(FunctionCall call) {
            var bytecode = call.Parameters.SelectMany(Visit).ToList();
            var func = GetFunctionByName(call.Name);
            bytecode.Add(new Opcode(Instruction.CALL_FUNCTION, func.Key, func.Value.Signature.Parameters.Count()));
            return bytecode;
        }

        public IEnumerable<Opcode> Visit(FunctionDefinition def) {
            AddFunction(def);
            return new List<Opcode>();
        }

        public IEnumerable<Opcode> Visit(FunctionSignature sig) {
            return new List<Opcode>();
        }

        public IEnumerable<Opcode> Visit(If ifStatement) {
            var bytecode = Visit(ifStatement.Condition).ToList();
            var trueBranch = Visit(ifStatement.Concequent).ToList();
            var falseBranch = Visit(ifStatement.Otherwise).ToList();

            bytecode.Add(new Opcode(Instruction.JUMP_FALSE, trueBranch.Count() + 1));
            bytecode.AddRange(trueBranch);
            bytecode.Add(new Opcode(Instruction.JUMP, falseBranch.Count()));

            bytecode.AddRange(falseBranch);

            return bytecode;
        }

        public IEnumerable<Opcode> Visit(Instance instance) {
            var typeclass = typeclasses[instance.TypeclassName];
            foreach (var member in instance.Definitions) {
                AddFunction(member);
            }
            return new List<Opcode>();
        }

        public IEnumerable<Opcode> Visit(Integer integer) {
            var constReference = AddConstant(integer.Num);
            return new[] {new Opcode(Instruction.LOAD_CONST, constReference)};
        }


        public IEnumerable<Opcode> Visit(Name name) {
            if (name.IsWrite)
                return new[] {new Opcode(Instruction.STORE_NAME, GetNameEntry(name.Value))};
            return new[] { new Opcode(Instruction.LOAD_NAME, GetNameEntry(name.Value)) };

        }

        public IEnumerable<Opcode> Visit(NewAssignment assignment) {
            var bytecode = Visit(assignment.Assignment).ToList();
            var nameTableEntry = GetNameEntry(assignment.Declaration.Name.Value);
            var nameConstant = AddConstant(assignment.Declaration.Name.Value);
            bytecode.Add(new Opcode(Instruction.STORE_NEW_NAME, nameTableEntry, nameConstant));
            return bytecode;
        }

        public IEnumerable<Opcode> Visit(AST_Nodes.Program program) {
            return program.Statements.SelectMany(Visit).ToList();
        }

        public IEnumerable<Opcode> Visit(Record record) {
            var name = record.Name;
            var typeParams = record.TypeParams;
            var members = record.Members.ToList();

            var ctorSignature = new FunctionSignature(name, members, new Type(new TypeName(name)));
            var ctorBytecode = new List<Opcode>();
            var paramCount = 0;
            foreach (var member in members) {
                ctorBytecode.Add(new Opcode(Instruction.LOAD_NAME, paramCount++));
            }
            ctorBytecode.AddRange(new List<Opcode> {
                new Opcode(Instruction.MAKE_RECORD, members.Count()),
                new Opcode(Instruction.RETURN)
            });
            var ctorFunc = new FunctionDefinition(ctorSignature, new AST_Nodes.Bytecode(ctorBytecode));

            AddFunction(ctorFunc);

            var attrCount = 0;
            foreach (var member in members) {
                var memberSignature = new FunctionSignature(member.Name.Value,
                    new List<BindingDeclaration> {
                        new BindingDeclaration(new Name("record", false), new Type(new TypeName(name)))
                    }, member.Type);
                var memberFunc = new FunctionDefinition(memberSignature, new AST_Nodes.Bytecode(new List<Opcode> {
                    new Opcode(Instruction.LOAD_NAME, 0),
                    new Opcode(Instruction.LOAD_ATTR, attrCount++),
                    new Opcode(Instruction.RETURN)
                }));
                AddFunction(memberFunc);
            }

            return new List<Opcode>();
        }

        public IEnumerable<Opcode> Visit(Return returnStatement) {
            var bytecode = Visit(returnStatement.Expression).ToList();
            bytecode.Add(new Opcode(Instruction.RETURN));
            return bytecode;
        }

        public IEnumerable<Opcode> Visit(String str) {
            var constReference = AddConstant(str.Str);
            return new[] {new Opcode(Instruction.LOAD_CONST, constReference)};
        }

        public IEnumerable<Opcode> Visit(Type type) {
            return new List<Opcode>();
        }

        public IEnumerable<Opcode> Visit(TypeClass typeClass) {
            typeclasses[typeClass.Name] = typeClass;
            return new List<Opcode>();
        }

        public IEnumerable<Opcode> Visit(TypeName typeName) {
            return new List<Opcode>();
        }

        public IEnumerable<Opcode> Visit(While whileStatement) {
            var funcReference =
                AddFunction(
                    new FunctionDefinition(
                        new FunctionSignature("WHILE_FUNC", new List<BindingDeclaration>(),
                            new Type(new TypeName("Void"))), whileStatement.Executable));

            var condition = Visit(whileStatement.Expression).ToList();
            var bytecode = condition.ToList();
            bytecode.Add(new Opcode(Instruction.JUMP_FALSE, 2));

            bytecode.Add(new Opcode(Instruction.CALL_FUNCTION, funcReference, 0));
            bytecode.Add(new Opcode(Instruction.JUMP, -(3 + condition.Count())));
            return bytecode;
        }

        public IEnumerable<Opcode> Visit(AST_Nodes.Bytecode code) {
            return code.Code;
        }
    }
}
