using System;

namespace Speedycloud.Compiler {
    class Program {
        static void Main(string[] args) {
            const string code = @"def print(foo: String): String {return foo;}
if (true)
	print(""True"")";

            var lexer = new Lexer.Lexer();
            var parser = new Parser.Parser(lexer.Lex(code));
            var typechecker = new TypeChecker.Typechecker();
            var compiler = new AST_Visitors.BytecodeGenerator();

            var tree = parser.ParseProgram();

            typechecker.Visit(tree);
            var bytecode = compiler.Finalise(compiler.Visit(tree));

            Console.WriteLine("Input:");
            Console.WriteLine("\t" + code);

            Console.WriteLine("Constants:");
            foreach (var constant in compiler.Constants) {
                Console.WriteLine("\t{0} = {1}", constant.Key, constant.Value);
            }

            Console.WriteLine("Functions:");
            foreach (var constant in compiler.Functions) {
                Console.WriteLine("\t{0} = {1}", constant.Key, constant.Value);
            }

            Console.WriteLine("Bytecode:");
            foreach (var opcode in bytecode) {
                Console.WriteLine("\t" + opcode);
            }
            Console.ReadKey();
        }
    }
}
