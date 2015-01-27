using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Speedycloud.Bytecode;
using Speedycloud.Bytecode.ValueTypes;
using Speedycloud.Compiler.AST_Nodes;
using Speedycloud.Compiler.AST_Visitors;
using Speedycloud.Runtime;
using Array = Speedycloud.Compiler.AST_Nodes.Array;
using Boolean = Speedycloud.Compiler.AST_Nodes.Boolean;
using String = Speedycloud.Compiler.AST_Nodes.String;
using Type = Speedycloud.Compiler.AST_Nodes.Type;

namespace CompilerTests {
    [TestClass]
    public class CodeGenerator {
        [TestMethod]
        public void IntegerConstant() {
            var tree = new Integer(5);
            var gen = new BytecodeGenerator();

            var bytecode = gen.Visit(tree).ToList();
            var constant = gen.Constants.First();

            Assert.IsTrue(new []{new Opcode(Instruction.LOAD_CONST, constant.Key)}.SequenceEqual(bytecode));
            Assert.AreEqual(1, gen.Constants.Count);
            Assert.AreEqual(5, constant.Value.Integer);
            Assert.AreEqual(constant.Key, bytecode.First().OpArgs[0]);
        }

        [TestMethod]
        public void StringConstant() {
            var tree = new String("Hello, world!");
            var gen = new BytecodeGenerator();

            var bytecode = gen.Visit(tree).ToList();
            var constant = gen.Constants.First();

            Assert.IsTrue(new[] { new Opcode(Instruction.LOAD_CONST, constant.Key) }.SequenceEqual(bytecode));
            Assert.AreEqual(1, gen.Constants.Count);
            Assert.AreEqual("Hello, world!", constant.Value.String);
            Assert.AreEqual(constant.Key, bytecode.First().OpArgs[0]);
        }

        [TestMethod]
        public void FloatConstant() {
            var tree = new Float(2.5);
            var gen = new BytecodeGenerator();

            var bytecode = gen.Visit(tree).ToList();
            var constant = gen.Constants.First();

            Assert.IsTrue(new[] { new Opcode(Instruction.LOAD_CONST, constant.Key) }.SequenceEqual(bytecode));
            Assert.AreEqual(1, gen.Constants.Count);
            Assert.AreEqual(2.5, constant.Value.Double);
            Assert.AreEqual(constant.Key, bytecode.First().OpArgs[0]);
        }

        [TestMethod]
        public void BoolConstant() {
            var tree = new Boolean(true);
            var gen = new BytecodeGenerator();

            var bytecode = gen.Visit(tree).ToList();
            var constant = gen.Constants.First();

            Assert.IsTrue(new[] { new Opcode(Instruction.LOAD_CONST, constant.Key) }.SequenceEqual(bytecode));
            Assert.AreEqual(1, gen.Constants.Count);
            Assert.AreEqual(true, constant.Value.Boolean);
            Assert.AreEqual(constant.Key, bytecode.First().OpArgs[0]);
        }

        [TestMethod]
        public void ArrayConstant() {
            var tree = new Array(new List<IExpression>{new Integer(3), new Integer(5)});
            var gen = new BytecodeGenerator();

            var bytecode = gen.Visit(tree).ToList();
            var constant1 = gen.Constants.First();
            var constant2 = gen.Constants.ElementAt(1);

            Assert.IsTrue(new[] {
                new Opcode(Instruction.LOAD_CONST, constant1.Key),
                new Opcode(Instruction.LOAD_CONST, constant2.Key),
                new Opcode(Instruction.MAKE_ARR, 2),
            }.SequenceEqual(bytecode));
            Assert.AreEqual(2, gen.Constants.Count);
            Assert.AreEqual(3, constant1.Value.Integer);
            Assert.AreEqual(5, constant2.Value.Integer);
            Assert.AreEqual(constant1.Key, bytecode.First().OpArgs[0]);
            Assert.AreEqual(constant2.Key, bytecode.ElementAt(1).OpArgs[0]);
        }

        [TestMethod]
        public void ArrayIndex() {
            var tree = new ArrayIndex(new Array(new List<IExpression> {new Integer(3), new Integer(5)}), new Integer(0));
            var gen = new BytecodeGenerator();

            var bytecode = gen.Visit(tree).ToList();
            var constant1 = gen.Constants.First();
            var constant2 = gen.Constants.ElementAt(1);
            var constant3 = gen.Constants.ElementAt(2);

            Assert.IsTrue(new[] {
                new Opcode(Instruction.LOAD_CONST, constant1.Key),
                new Opcode(Instruction.LOAD_CONST, constant2.Key),
                new Opcode(Instruction.MAKE_ARR, 2),
                new Opcode(Instruction.LOAD_CONST, constant3.Key), 
                new Opcode(Instruction.BINARY_INDEX), 
            }.SequenceEqual(bytecode));
        }

        [TestMethod]
        public void NewAssignment() {
            var tree =
                new NewAssignment(
                    new BindingDeclaration(new Name("x", true),
                        new Type(new TypeName("Integer"), new List<Constraint>(), isRuntimeCheck: false,
                            isArrayType: false)), new Integer(3), isWritable: false);
            var gen = new BytecodeGenerator();

            var bytecode = gen.Visit(tree).ToList();
            var const1 = gen.Constants.First();

            Assert.IsTrue(new[] {
                new Opcode(Instruction.LOAD_CONST, const1.Key),
                new Opcode(Instruction.STORE_NEW_NAME, gen.Names["x"], gen.Constants.ElementAt(1).Key), 
            }.SequenceEqual(bytecode));
        }

        [TestMethod]
        public void Assignment() {
            var tree =
                new Assignment(new Name("x", true), new Integer(3));
            var gen = new BytecodeGenerator();

            var bytecode = gen.Visit(tree).ToList();
            var const1 = gen.Constants.First();

            Assert.IsTrue(new[] {
                new Opcode(Instruction.LOAD_CONST, const1.Key),
                new Opcode(Instruction.STORE_NAME, gen.Names["x"]), 
            }.SequenceEqual(bytecode));
        }

        [TestMethod]
        public void BinaryAddition() {
            var tree =
                new BinaryOp("+", new Integer(2), new Integer(3));
            var gen = new BytecodeGenerator();

            var bytecode = gen.Visit(tree).ToList();

            Assert.IsTrue(new[] {
                new Opcode(Instruction.LOAD_CONST, gen.Constants.ElementAt(0).Key),
                new Opcode(Instruction.LOAD_CONST, gen.Constants.ElementAt(1).Key),
                new Opcode(Instruction.BINARY_ADD), 
            }.SequenceEqual(bytecode));
        }

        [TestMethod]
        public void BinarySubtraction() {
            var tree =
                new BinaryOp("-", new Integer(2), new Integer(3));
            var gen = new BytecodeGenerator();

            var bytecode = gen.Visit(tree).ToList();

            Assert.IsTrue(new[] {
                new Opcode(Instruction.LOAD_CONST, gen.Constants.ElementAt(0).Key),
                new Opcode(Instruction.LOAD_CONST, gen.Constants.ElementAt(1).Key),
                new Opcode(Instruction.BINARY_SUB), 
            }.SequenceEqual(bytecode));
        }

        [TestMethod]
        public void BinaryMultiplication() {
            var tree =
                new BinaryOp("*", new Integer(2), new Integer(3));
            var gen = new BytecodeGenerator();

            var bytecode = gen.Visit(tree).ToList();

            Assert.IsTrue(new[] {
                new Opcode(Instruction.LOAD_CONST, gen.Constants.ElementAt(0).Key),
                new Opcode(Instruction.LOAD_CONST, gen.Constants.ElementAt(1).Key),
                new Opcode(Instruction.BINARY_MUL), 
            }.SequenceEqual(bytecode));
        }
    }
}
