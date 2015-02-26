using System;
using System.IO;
using System.Linq;
using Speedycloud.Bytecode;

namespace Speedycloud.Compiler {
    class Program {
        static void Main(string[] args) {
            var code = File.ReadAllText(args[0]);

            Console.WriteLine("Input:");
            Console.WriteLine("\t" + code);

            var prelude = new Prelude.Functions();
            code = prelude.RegisterAdditionalFunctions(code);

            var lexer = new Lexer.Lexer();
            var parser = new Parser.Parser(lexer.Lex(code));
            var typechecker = new TypeChecker.Typechecker(prelude.Types());
            var compiler = new AST_Visitors.BytecodeGenerator(prelude.Definitions(), typechecker);

            var tree = parser.ParseProgram();

            typechecker.Visit(tree);
            var bytecode = compiler.Finalise(compiler.Visit(tree)).ToList();

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
            var saver = new BytecodeSerialiser();

            var bytecodeData = saver.Dump(bytecode, compiler.Constants.Values);
            //Console.WriteLine("Data:");
            //Console.Write(bytecodeData);
            File.WriteAllText(args[1], bytecodeData);
            Console.ReadKey();
        }
    }
}
