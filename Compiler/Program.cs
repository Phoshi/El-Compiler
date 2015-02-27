using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Speedycloud.Bytecode;
using Speedycloud.Compiler.AST_Nodes;
using Speedycloud.Compiler.Parser;
using Speedycloud.Compiler.TypeChecker;

namespace Speedycloud.Compiler {
    class Program {
        static int Main(string[] args) {
            var code = File.ReadAllText(args[0]);

            Console.WriteLine("Input:");
            Console.WriteLine("\t" + code);

            var prelude = new Prelude.Functions();
            code = prelude.RegisterAdditionalFunctions(code);

            var lexer = new Lexer.Lexer();
            var parser = new Parser.Parser(lexer.Lex(code));
            var typechecker = new TypeChecker.Typechecker(prelude.Types());
            var compiler = new AST_Visitors.BytecodeGenerator(prelude.Definitions(), typechecker);
            INode tree;
            try {
                tree = parser.ParseProgram();
            }
            catch (ParseException ex) {
                Console.Write(ex);
                return 1;
            }

            try {
                typechecker.Visit(tree);
            }
            catch (TypeCheckException ex) {
                Console.Write(ex);
                return 1;
            }

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
            var savename = args.Count() > 1 ? args[1] : args[0].Substring(0, args[0].IndexOf('.')) + ".elc";
            File.WriteAllText(savename, bytecodeData);
            return 0;
        }
    }
}
