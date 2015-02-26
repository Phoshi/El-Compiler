using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Speedycloud.Bytecode;
using Speedycloud.Compiler.AST_Nodes;
using Speedycloud.Compiler.TypeChecker;
using Type = Speedycloud.Compiler.AST_Nodes.Type;

namespace Speedycloud.Compiler.Prelude {
     class Functions {
         private readonly Dictionary<FunctionDefinition, FunctionType> funcs =
             new Dictionary<FunctionDefinition, FunctionType> {
                 {
                     new FunctionDefinition(
                         new FunctionSignature("putc",
                             new List<BindingDeclaration> {
                                 new BindingDeclaration(new Name("c", false), new Type(new TypeName("Integer")))
                             },
                             new Type(new TypeName("Void"))),
                         new AST_Nodes.Bytecode(new List<Opcode> {
                             new Opcode(Instruction.LOAD_NAME, 0),
                             new Opcode(Instruction.SYSCALL, 0)
                         })),
                     new FunctionType("putc", new List<ITypeInformation> {new IntegerType()}, new UnknownType())
                 }, {
                     new FunctionDefinition(
                         new FunctionSignature("print",
                             new List<BindingDeclaration> {
                                 new BindingDeclaration(new Name("str", false), new Type(new TypeName("String")))
                             },
                             new Type(new TypeName("Void"))),
                         new For(new BindingDeclaration(new Name("c", false), new Type(new TypeName("Integer"))),
                             new Name("str", false),
                             new FunctionCall("putc", new List<IExpression> {new Name("c", false)}))),
                     new FunctionType("print", new List<ITypeInformation> {new StringType()}, new UnknownType())
                 },
             };

        public IEnumerable<FunctionDefinition> Definitions() {
            return funcs.Keys;
        }

        public Dictionary<FunctionDefinition, FunctionType> Types() {
            return funcs;
        }

         public string RegisterAdditionalFunctions(string code) {
             var prelude = @"def print(num: Integer){
	                            var n = num;
	                            if (n > 0){
		                            var digit = n % 10;
		                            n = n / 10;
		                            print(n);
		                            putc(48 + digit);
	                            };
                            }";
             return prelude + code;
         }
    }
}
