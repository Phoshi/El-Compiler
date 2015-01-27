using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Speedycloud.Compiler.AST_Nodes;
using Speedycloud.Compiler.AST_Visitors;
using Speedycloud.Runtime;

namespace CompilerTests {
    [TestClass]
    public class CodeGenerator {
        [TestMethod]
        public void TestConstant() {
            var tree = new Integer(5);
            var gen = new BytecodeGenerator();

            var bytecode = gen.Visit(tree).ToList();
            var constant = gen.Constants.First();

            Assert.IsTrue(new []{new Opcode(Instruction.LOAD_CONST, constant.Key)}.SequenceEqual(bytecode));
            Assert.AreEqual(1, gen.Constants.Count);
            Assert.AreEqual(5, constant.Value.Integer);
            Assert.AreEqual(constant.Key, bytecode.First().OpArgs[0]);
        }
    }
}
