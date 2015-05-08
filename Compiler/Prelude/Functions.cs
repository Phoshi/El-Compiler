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
                 },

                                  {
                     new FunctionDefinition(
                         new FunctionSignature("getc",
                             new List<BindingDeclaration> {},
                             new Type(new TypeName("Integer"))),
                         new AST_Nodes.Bytecode(new List<Opcode> {
                             new Opcode(Instruction.SYSCALL, 1),
                             new Opcode(Instruction.RETURN, 1)
                         })),
                     new FunctionType("getc", new List<ITypeInformation> {}, new IntegerType())
                 },
             };

        public IEnumerable<FunctionDefinition> Definitions() {
            return funcs.Keys;
        }

        public Dictionary<FunctionDefinition, FunctionType> Types() {
            return funcs;
        }

         public string RegisterAdditionalFunctions(string code) {
             var prelude = @"
def print(str: String){
    for (c in str){
        putc(c);
    };
}

def println(str: String){
    print(str);
    putc(10);
}

def print(num: Integer){
    if (num == 0){
        putc(48);
    } else {
        iprint(num);
    };
}

def iprint(num: Integer){
	var n = num;
    if (n < 0){
        print(""-"");
        n = -n;
    };
	if (n > 0){
		var digit = n % 10;
		n = n / 10;
		iprint(n);
		putc(48 + digit);
	};
}

def println(num: Integer){
    print(num);
    putc(10);
}

def print(flag: Boolean){
    if (flag)
        print(""True"")
    else
        print(""False"");
}

def println(flag: Boolean){
    print(flag);
    putc(10);
}
";
             return prelude + code;
         }
    }
}
