using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Speedycloud.Bytecode;
using Speedycloud.Bytecode.ValueTypes;
using Speedycloud.Compiler.AST_Nodes;
using Speedycloud.Compiler.AST_Visitors;
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
        [TestMethod]
        public void BinaryDivision() {
            var tree =
                new BinaryOp("/", new Integer(2), new Integer(3));
            var gen = new BytecodeGenerator();

            var bytecode = gen.Visit(tree).ToList();

            Assert.IsTrue(new[] {
                new Opcode(Instruction.LOAD_CONST, gen.Constants.ElementAt(0).Key),
                new Opcode(Instruction.LOAD_CONST, gen.Constants.ElementAt(1).Key),
                new Opcode(Instruction.BINARY_DIV), 
            }.SequenceEqual(bytecode));
        }
        [TestMethod]
        public void BinaryEquals() {
            var tree =
                new BinaryOp("==", new Integer(2), new Integer(3));
            var gen = new BytecodeGenerator();

            var bytecode = gen.Visit(tree).ToList();

            Assert.IsTrue(new[] {
                new Opcode(Instruction.LOAD_CONST, gen.Constants.ElementAt(0).Key),
                new Opcode(Instruction.LOAD_CONST, gen.Constants.ElementAt(1).Key),
                new Opcode(Instruction.BINARY_EQL), 
            }.SequenceEqual(bytecode));
        }

        [TestMethod]
        public void BinaryNotEquals() {
            var tree =
                new BinaryOp("!=", new Integer(2), new Integer(3));
            var gen = new BytecodeGenerator();

            var bytecode = gen.Visit(tree).ToList();

            Assert.IsTrue(new[] {
                new Opcode(Instruction.LOAD_CONST, gen.Constants.ElementAt(0).Key),
                new Opcode(Instruction.LOAD_CONST, gen.Constants.ElementAt(1).Key),
                new Opcode(Instruction.BINARY_NEQ), 
            }.SequenceEqual(bytecode));
        }

        [TestMethod]
        public void BinaryGreaterThan() {
            var tree =
                new BinaryOp(">", new Integer(2), new Integer(3));
            var gen = new BytecodeGenerator();

            var bytecode = gen.Visit(tree).ToList();

            Assert.IsTrue(new[] {
                new Opcode(Instruction.LOAD_CONST, gen.Constants.ElementAt(0).Key),
                new Opcode(Instruction.LOAD_CONST, gen.Constants.ElementAt(1).Key),
                new Opcode(Instruction.BINARY_GT), 
            }.SequenceEqual(bytecode));
        }

        [TestMethod]
        public void BinaryLessThan() {
            var tree =
                new BinaryOp("<", new Integer(2), new Integer(3));
            var gen = new BytecodeGenerator();

            var bytecode = gen.Visit(tree).ToList();

            Assert.IsTrue(new[] {
                new Opcode(Instruction.LOAD_CONST, gen.Constants.ElementAt(0).Key),
                new Opcode(Instruction.LOAD_CONST, gen.Constants.ElementAt(1).Key),
                new Opcode(Instruction.BINARY_LT), 
            }.SequenceEqual(bytecode));
        }

        [TestMethod]
        public void BinaryGreaterThanOrEqual() {
            var tree =
                new BinaryOp(">=", new Integer(2), new Integer(3));
            var gen = new BytecodeGenerator();

            var bytecode = gen.Visit(tree).ToList();

            Assert.IsTrue(new[] {
                new Opcode(Instruction.LOAD_CONST, gen.Constants.ElementAt(0).Key),
                new Opcode(Instruction.LOAD_CONST, gen.Constants.ElementAt(1).Key),
                new Opcode(Instruction.BINARY_GTE), 
            }.SequenceEqual(bytecode));
        }

        [TestMethod]
        public void BinaryLessThanOrEqual() {
            var tree =
                new BinaryOp("<=", new Integer(2), new Integer(3));
            var gen = new BytecodeGenerator();

            var bytecode = gen.Visit(tree).ToList();

            Assert.IsTrue(new[] {
                new Opcode(Instruction.LOAD_CONST, gen.Constants.ElementAt(0).Key),
                new Opcode(Instruction.LOAD_CONST, gen.Constants.ElementAt(1).Key),
                new Opcode(Instruction.BINARY_LTE), 
            }.SequenceEqual(bytecode));
        }

        [TestMethod]
        public void BinaryAnd() {
            var tree =
                new BinaryOp("&&", new Integer(2), new Integer(3));
            var gen = new BytecodeGenerator();

            var bytecode = gen.Visit(tree).ToList();

            Assert.IsTrue(new[] {
                new Opcode(Instruction.LOAD_CONST, gen.Constants.ElementAt(0).Key),
                new Opcode(Instruction.LOAD_CONST, gen.Constants.ElementAt(1).Key),
                new Opcode(Instruction.BINARY_AND), 
            }.SequenceEqual(bytecode));
        }

        [TestMethod]
        public void BinaryOr() {
            var tree =
                new BinaryOp("||", new Integer(2), new Integer(3));
            var gen = new BytecodeGenerator();

            var bytecode = gen.Visit(tree).ToList();

            Assert.IsTrue(new[] {
                new Opcode(Instruction.LOAD_CONST, gen.Constants.ElementAt(0).Key),
                new Opcode(Instruction.LOAD_CONST, gen.Constants.ElementAt(1).Key),
                new Opcode(Instruction.BINARY_OR), 
            }.SequenceEqual(bytecode));
        }

        [TestMethod]
        public void BindingDeclaration() {
            var tree = new BindingDeclaration(new Name("x", true), new Type(new TypeName("Integer"), new List<Constraint>(), false, false));
            var gen = new BytecodeGenerator();

            var bytecode = gen.Visit(tree).ToList();

            Assert.IsTrue(new Opcode[] {}.SequenceEqual(bytecode));
        }

        [TestMethod]
        public void Constraint() {
            var tree = new Constraint("Eq", new Integer(3));
            var gen = new BytecodeGenerator();

            var bytecode = gen.Visit(tree).ToList();

            Assert.IsTrue(new Opcode[] { }.SequenceEqual(bytecode));
        }

        [TestMethod]
        public void For() {
            var tree = new For(new BindingDeclaration(new Name("i", true), new Type(new TypeName("Integer"))),
                new Array(new List<IExpression>()),
                new NewAssignment(new BindingDeclaration(new Name("foo", true), new Type(new TypeName("Integer"))),
                    new Name("i", false), false));

            var gen = new BytecodeGenerator();
            var bytecode = gen.Visit(tree).ToList();
            var zero = gen.Constants.First(kv => kv.Value.Type == ValueType.Integer && kv.Value.Integer == 0);
            var one = gen.Constants.First(kv => kv.Value.Type == ValueType.Integer && kv.Value.Integer == 1);
            var func = gen.Functions.First();

            var loopIter = "LOOP_ITER".GetHashCode();
            var loopCount = "LOOP_COUNT".GetHashCode();

            Assert.IsTrue(new [] {
                new Opcode(Instruction.MAKE_ARR, 0),
                new Opcode(Instruction.STORE_NEW_NAME, loopIter, -1),
                new Opcode(Instruction.LOAD_CONST, zero.Key),
                new Opcode(Instruction.STORE_NEW_NAME, loopCount, -1),
                new Opcode(Instruction.LOAD_NAME, loopIter), 
                new Opcode(Instruction.LOAD_ATTR, 0),
                new Opcode(Instruction.LOAD_NAME, loopCount), 
                new Opcode(Instruction.BINARY_EQL), 
                new Opcode(Instruction.JUMP_TRUE, 9),

                new Opcode(Instruction.LOAD_NAME, loopIter),
                new Opcode(Instruction.LOAD_NAME, loopCount),
                new Opcode(Instruction.BINARY_INDEX),

                new Opcode(Instruction.CALL_FUNCTION, func.Key, 1),

                new Opcode(Instruction.LOAD_NAME, loopCount),
                new Opcode(Instruction.LOAD_CONST, one.Key),
                new Opcode(Instruction.BINARY_ADD),
                new Opcode(Instruction.STORE_NAME, loopCount),

                new Opcode(Instruction.JUMP, -14), 
            }.SequenceEqual(bytecode), string.Join("\n", bytecode));

            Assert.AreEqual("i", gen.Functions[0].Signature.Parameters.First().Name.Value);
            Assert.AreEqual(new NewAssignment(new BindingDeclaration(new Name("foo", true), new Type(new TypeName("Integer"))),
                    new Name("i", false), false), gen.Functions[0].Statement);
        }

        [TestMethod]
        public void FinaliseFor() {
            var tree = new For(new BindingDeclaration(new Name("i", true), new Type(new TypeName("Integer"))),
    new Array(new List<IExpression>()),
    new NewAssignment(new BindingDeclaration(new Name("foo", true), new Type(new TypeName("Integer"))),
        new Name("i", false), false));

            var gen = new BytecodeGenerator();
            var bytecode = gen.Visit(tree).ToList();
            bytecode = gen.Finalise(bytecode).ToList();
            var zero = gen.Constants.Where(kv => kv.Value.Type == ValueType.Integer && kv.Value.Integer == 0).ElementAt(1); //The first constant 0 is actually the function reference
            var foo = gen.Constants.First(kv => kv.Value.Type == ValueType.String && kv.Value.String == "foo");
            var one = gen.Constants.First(kv => kv.Value.Type == ValueType.Integer && kv.Value.Integer == 1);
            var func = gen.Functions.First();

            var loopIter = "LOOP_ITER".GetHashCode();
            var loopCount = "LOOP_COUNT".GetHashCode();

            Assert.IsTrue(new[] {
                new Opcode(Instruction.LOAD_NAME, gen.Names["i"]), 
                new Opcode(Instruction.STORE_NEW_NAME, gen.Names["foo"], foo.Key), 
                new Opcode(Instruction.RETURN), 
                new Opcode(Instruction.CODE_START),
                new Opcode(Instruction.MAKE_ARR, 0),
                new Opcode(Instruction.STORE_NEW_NAME, loopIter, -1),
                new Opcode(Instruction.LOAD_CONST, zero.Key),
                new Opcode(Instruction.STORE_NEW_NAME, loopCount, -1),
                new Opcode(Instruction.LOAD_NAME, loopIter), 
                new Opcode(Instruction.LOAD_ATTR, 0),
                new Opcode(Instruction.LOAD_NAME, loopCount), 
                new Opcode(Instruction.BINARY_EQL), 
                new Opcode(Instruction.JUMP_TRUE, 9),

                new Opcode(Instruction.LOAD_NAME, loopIter),
                new Opcode(Instruction.LOAD_NAME, loopCount),
                new Opcode(Instruction.BINARY_INDEX),

                new Opcode(Instruction.CALL_FUNCTION, func.Key, 1),

                new Opcode(Instruction.LOAD_NAME, loopCount),
                new Opcode(Instruction.LOAD_CONST, one.Key),
                new Opcode(Instruction.BINARY_ADD),
                new Opcode(Instruction.STORE_NAME, loopCount),

                new Opcode(Instruction.JUMP, -14), 
                new Opcode(Instruction.CODE_STOP), 
            }.SequenceEqual(bytecode), string.Join("\n", bytecode));

            Assert.AreEqual(0, gen.Constants[func.Key].Integer);
        }
    }
}
