using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        private int AddConstant(long num) {
            var c = new IntValue(num);
            constTable[constTable.Count] = c;
            return constTable.Count - 1;
        }

        public Dictionary<int, FunctionDefinition> Functions { get { return new Dictionary<int, FunctionDefinition>(funcTable);} }
        private readonly Dictionary<int, FunctionDefinition> funcTable = new Dictionary<int, FunctionDefinition>(); 

        public IEnumerable<Opcode> Visit(INode node) {
            throw new NotImplementedException();
        }

        public IEnumerable<Opcode> Visit(Array array) {
            throw new NotImplementedException();
        }

        public IEnumerable<Opcode> Visit(ArrayIndex arrayIndex) {
            throw new NotImplementedException();
        }

        public IEnumerable<Opcode> Visit(Assignment assignment) {
            throw new NotImplementedException();
        }

        public IEnumerable<Opcode> Visit(BinaryOp binaryOp) {
            throw new NotImplementedException();
        }

        public IEnumerable<Opcode> Visit(BindingDeclaration declaration) {
            throw new NotImplementedException();
        }

        public IEnumerable<Opcode> Visit(Boolean boolean) {
            throw new NotImplementedException();
        }

        public IEnumerable<Opcode> Visit(Constraint constraint) {
            throw new NotImplementedException();
        }

        public IEnumerable<Opcode> Visit(Float number) {
            throw new NotImplementedException();
        }

        public IEnumerable<Opcode> Visit(For missing_name) {
            throw new NotImplementedException();
        }

        public IEnumerable<Opcode> Visit(FunctionCall call) {
            throw new NotImplementedException();
        }

        public IEnumerable<Opcode> Visit(FunctionDefinition def) {
            throw new NotImplementedException();
        }

        public IEnumerable<Opcode> Visit(FunctionSignature sig) {
            throw new NotImplementedException();
        }

        public IEnumerable<Opcode> Visit(If ifStatement) {
            throw new NotImplementedException();
        }

        public IEnumerable<Opcode> Visit(Instance instance) {
            throw new NotImplementedException();
        }

        public IEnumerable<Opcode> Visit(Integer integer) {
            var constReference = AddConstant(integer.Num);
            return new[] {new Opcode(Instruction.LOAD_CONST, constReference)};
        }


        public IEnumerable<Opcode> Visit(Name name) {
            throw new NotImplementedException();
        }

        public IEnumerable<Opcode> Visit(NewAssignment assignment) {
            throw new NotImplementedException();
        }

        public IEnumerable<Opcode> Visit(AST_Nodes.Program program) {
            throw new NotImplementedException();
        }

        public IEnumerable<Opcode> Visit(Record record) {
            throw new NotImplementedException();
        }

        public IEnumerable<Opcode> Visit(Return returnStatement) {
            throw new NotImplementedException();
        }

        public IEnumerable<Opcode> Visit(String str) {
            throw new NotImplementedException();
        }

        public IEnumerable<Opcode> Visit(Type type) {
            throw new NotImplementedException();
        }

        public IEnumerable<Opcode> Visit(TypeClass typeClass) {
            throw new NotImplementedException();
        }

        public IEnumerable<Opcode> Visit(TypeName typeName) {
            throw new NotImplementedException();
        }

        public IEnumerable<Opcode> Visit(While whileStatement) {
            throw new NotImplementedException();
        }
    }
}
