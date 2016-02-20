using System;
using System.IO;
using System.Linq;
using System.Text;
using Speedycloud.Bytecode;
using Speedycloud.Compiler.AST_Nodes;
using Speedycloud.Compiler.Parser;
using Speedycloud.Compiler.TypeChecker;

namespace Speedycloud.Compiler {
    class Program {
        private readonly static string LOG_PATH = "compile-" + DateTime.Now.ToString("yy-MM-dd-H-mm-ss") + ".log";
        private static int level = 0;

        private static StringBuilder logFile = new StringBuilder();

        public static void Log(string category, string details) {
            var tabs = new string('\t', level);
            var logLine = tabs + "[{0}] {1}: {2}\n";
            var time = DateTime.Now.ToString("yy-MM-dd H:mm:ss.fffffff");
            logFile.AppendLine(string.Format(logLine, time, category, details));
        }

        public static void LogIn(string category, string details) {
            Log(category, details);
            level++;
        }

        public static void LogOut(string category, string details) {
            if (level > 0) {
                level--;
            }
            Log(category, details);
        }

        static int Main(string[] args) {
            var code = File.ReadAllText(args[0]);

            Console.WriteLine("Input:");
            Console.WriteLine("\t" + code);

            var prelude = new Prelude.Functions();
            code = prelude.RegisterAdditionalFunctions(code);

            var lexer = new Lexer.Lexer();
            var parser = new Parser.Parser(lexer.Lex(code));
            var typechecker = new Typechecker(prelude.Types());
            var compiler = new AST_Visitors.BytecodeGenerator(prelude.Definitions(), typechecker);
            INode tree;
            try {
                tree = parser.ParseProgram();
            }
            catch (ParseException ex) {
                Console.Write(ex);
                File.WriteAllText(LOG_PATH, logFile.ToString());
                return 1;
            }

            try {
                typechecker.Visit(tree);
            }
            catch (TypeCheckException ex) {
                Console.Write(ex.Message);
                File.WriteAllText(LOG_PATH, logFile.ToString());
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
            var savename = args.Count() > 1 ? args[1] : args[0].Substring(0, args[0].IndexOf('.')) + ".elc";
            File.WriteAllText(savename, bytecodeData);
            File.WriteAllText(LOG_PATH, logFile.ToString());
            return 0;
        }
    }
}
