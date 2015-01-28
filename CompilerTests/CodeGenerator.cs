using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Speedycloud.Bytecode;
using Speedycloud.Compiler.AST_Nodes;
using Speedycloud.Compiler.AST_Visitors;
using Array = Speedycloud.Compiler.AST_Nodes.Array;
using Boolean = Speedycloud.Compiler.AST_Nodes.Boolean;
using String = Speedycloud.Compiler.AST_Nodes.String;
using Type = Speedycloud.Compiler.AST_Nodes.Type;
using ValueType = Speedycloud.Bytecode.ValueTypes.ValueType;

namespace CompilerTests {
    [TestClass]
    public class CodeGenerator {
        [TestMethod]
        public void IntegerConstant() {
            var tree = new Integer(5);
            var gen = new BytecodeGenerator();

            var bytecode = gen.Visit(tree).ToList();
            var constant = gen.Constants.First();

            Assert.IsTrue(new[] {new Opcode(Instruction.LOAD_CONST, constant.Key)}.SequenceEqual(bytecode));
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

            Assert.IsTrue(new[] {new Opcode(Instruction.LOAD_CONST, constant.Key)}.SequenceEqual(bytecode));
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

            Assert.IsTrue(new[] {new Opcode(Instruction.LOAD_CONST, constant.Key)}.SequenceEqual(bytecode));
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

            Assert.IsTrue(new[] {new Opcode(Instruction.LOAD_CONST, constant.Key)}.SequenceEqual(bytecode));
            Assert.AreEqual(1, gen.Constants.Count);
            Assert.AreEqual(true, constant.Value.Boolean);
            Assert.AreEqual(constant.Key, bytecode.First().OpArgs[0]);
        }

        [TestMethod]
        public void ArrayConstant() {
            var tree = new Array(new List<IExpression> {new Integer(3), new Integer(5)});
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
            var tree = new BindingDeclaration(new Name("x", true),
                new Type(new TypeName("Integer"), new List<Constraint>(), false, false));
            var gen = new BytecodeGenerator();

            var bytecode = gen.Visit(tree).ToList();

            Assert.IsTrue(new Opcode[] {}.SequenceEqual(bytecode));
        }

        [TestMethod]
        public void Constraint() {
            var tree = new Constraint("Eq", new Integer(3));
            var gen = new BytecodeGenerator();

            var bytecode = gen.Visit(tree).ToList();

            Assert.IsTrue(new Opcode[] {}.SequenceEqual(bytecode));
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

            Assert.IsTrue(new[] {
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
            Assert.AreEqual(
                new NewAssignment(new BindingDeclaration(new Name("foo", true), new Type(new TypeName("Integer"))),
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
            var zero =
                gen.Constants.Where(kv => kv.Value.Type == ValueType.Integer && kv.Value.Integer == 0).ElementAt(1);
                //The first constant 0 is actually the function reference
            var foo = gen.Constants.First(kv => kv.Value.Type == ValueType.String && kv.Value.String == "foo");
            var one = gen.Constants.First(kv => kv.Value.Type == ValueType.Integer && kv.Value.Integer == 1);
            var func = gen.Functions.First();

            var loopIter = "LOOP_ITER".GetHashCode();
            var loopCount = "LOOP_COUNT".GetHashCode();

            Assert.IsTrue(new[] {
                new Opcode(Instruction.LOAD_NAME, 0),
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

        [TestMethod]
        public void FunctionDefinition() {
            var tree =
                new FunctionDefinition(
                    new FunctionSignature("foo",
                        new List<BindingDeclaration> {
                            new BindingDeclaration(new Name("x", false), new Type(new TypeName("Integer")))
                        },
                        new Type(new TypeName("Integer"))),
                    new Return(new Name("x", false)));
            var gen = new BytecodeGenerator();
            var bytecode = gen.Visit(tree);

            Assert.IsTrue(new List<Opcode> {}.SequenceEqual(bytecode));
            Assert.AreEqual(new Return(new Name("x", false)), gen.Functions.First().Value.Statement);
            Assert.AreEqual("foo", gen.Functions.First().Value.Signature.Name);
            Assert.AreEqual("x", gen.Functions.First().Value.Signature.Parameters.First().Name.Value);
        }

        [TestMethod]
        public void FinaliseFunctionDefinition() {
            var tree =
                new FunctionDefinition(
                    new FunctionSignature("foo",
                        new List<BindingDeclaration> {
                            new BindingDeclaration(new Name("x", false), new Type(new TypeName("Integer")))
                        },
                        new Type(new TypeName("Integer"))),
                    new Return(new Name("x", false)));
            var gen = new BytecodeGenerator();
            var bytecode = gen.Finalise(gen.Visit(tree));

            Assert.IsTrue(new List<Opcode> {
                new Opcode(Instruction.LOAD_NAME, 0),
                new Opcode(Instruction.RETURN),
                new Opcode(Instruction.CODE_START),
                new Opcode(Instruction.CODE_STOP),
            }.SequenceEqual(bytecode));
            Assert.AreEqual(new Return(new Name("x", false)), gen.Functions.First().Value.Statement);
            Assert.AreEqual("foo", gen.Functions.First().Value.Signature.Name);
            Assert.AreEqual("x", gen.Functions.First().Value.Signature.Parameters.First().Name.Value);
            Assert.AreEqual(0, gen.Constants[gen.Functions.First().Key].Integer);
        }

        [TestMethod]
        public void FunctionCall() {
            var def = new FunctionDefinition(
                new FunctionSignature("foo",
                    new List<BindingDeclaration> {
                        new BindingDeclaration(new Name("x", false), new Type(new TypeName("Integer")))
                    },
                    new Type(new TypeName("Integer"))),
                new Return(new Name("x", false)));
            var call = new FunctionCall("foo", new List<IExpression> {new Integer(3)});
            var tree = new Program(new List<IStatement> {def, call});

            var gen = new BytecodeGenerator();
            var bytecode = gen.Visit(tree);

            var func = gen.Functions.First();
            var three = gen.Constants.ElementAt(1);


            Assert.IsTrue(new List<Opcode> {
                new Opcode(Instruction.LOAD_CONST, three.Key),
                new Opcode(Instruction.CALL_FUNCTION, func.Key, 1)
            }.SequenceEqual(bytecode));
        }

        [TestMethod]
        public void FunctionSignature() {
            var tree = new FunctionSignature("foo",
                new List<BindingDeclaration> {
                    new BindingDeclaration(new Name("x", false), new Type(new TypeName("Integer")))
                },
                new Type(new TypeName("Integer")));
            var gen = new BytecodeGenerator();
            var bytecode = gen.Visit(tree);
            Assert.AreEqual(0, bytecode.Count());
        }

        [TestMethod]
        public void If() {
            var tree = new If(new Boolean(true), new Integer(1), new Integer(2));
            var gen = new BytecodeGenerator();

            var bytecode = gen.Visit(tree);

            Assert.IsTrue(new List<Opcode> {
                new Opcode(Instruction.LOAD_CONST, 0),
                new Opcode(Instruction.JUMP_FALSE, 2),
                new Opcode(Instruction.LOAD_CONST, 1),
                new Opcode(Instruction.JUMP, 1),
                new Opcode(Instruction.LOAD_CONST, 2),
            }.SequenceEqual(bytecode));
        }

        [TestMethod]
        public void While() {
            var tree = new While(new BinaryOp("==", new Integer(3), new Integer(4)), new Integer(6));
            var gen = new BytecodeGenerator();

            var bytecode = gen.Visit(tree).ToList();

            Assert.IsTrue(new List<Opcode> {
                new Opcode(Instruction.LOAD_CONST, 1),
                new Opcode(Instruction.LOAD_CONST, 2),
                new Opcode(Instruction.BINARY_EQL),
                new Opcode(Instruction.JUMP_FALSE, 2),
                new Opcode(Instruction.CALL_FUNCTION, gen.Functions.First().Key, 0),
                new Opcode(Instruction.JUMP, -6)
            }.SequenceEqual(bytecode), string.Join("\n", bytecode));
        }

        [TestMethod]
        public void Record() {
            var tree = new Record("Point", new List<TypeName>(), new List<BindingDeclaration> {
                new BindingDeclaration(new Name("x", false), new Type(new TypeName("Integer"))),
                new BindingDeclaration(new Name("y", false), new Type(new TypeName("Integer"))),
            });

            var gen = new BytecodeGenerator();

            var bytecode = gen.Visit(tree).ToList();
            Assert.AreEqual(0, bytecode.Count);
            Assert.AreEqual(3, gen.Functions.Count);
            var ctor = gen.Functions.First(fn => fn.Value.Signature.Name == "Point").Value;
            var x = gen.Functions.First(fn => fn.Value.Signature.Name == "x").Value;
            var y = gen.Functions.First(fn => fn.Value.Signature.Name == "y").Value;

            Assert.AreEqual("Point", ctor.Signature.ReturnType.Name.Name);
            Assert.AreEqual("Integer", x.Signature.ReturnType.Name.Name);
            Assert.AreEqual("Integer", y.Signature.ReturnType.Name.Name);

            Assert.AreEqual(2, ctor.Signature.Parameters.Count());
            Assert.AreEqual(1, x.Signature.Parameters.Count());
            Assert.AreEqual(1, y.Signature.Parameters.Count());

            Assert.IsTrue(ctor.Signature.Parameters.All(param=>param.Type.Name.Name == "Integer"));
            Assert.IsTrue(x.Signature.Parameters.All(param => param.Type.Name.Name == "Point"));
            Assert.IsTrue(y.Signature.Parameters.All(param => param.Type.Name.Name == "Point"));
        }

        [TestMethod]
        public void FinalizeRecord() {
            var tree = new Record("Point", new List<TypeName>(), new List<BindingDeclaration> {
                new BindingDeclaration(new Name("x", false), new Type(new TypeName("Integer"))),
                new BindingDeclaration(new Name("y", false), new Type(new TypeName("Integer"))),
            });
            var gen = new BytecodeGenerator();

            var bytecode = gen.Finalise(gen.Visit(tree)).ToList();
            Assert.IsTrue(new List<Opcode> {
                new Opcode(Instruction.LOAD_NAME, 0),
                new Opcode(Instruction.LOAD_NAME, 1),
                new Opcode(Instruction.MAKE_RECORD, 2),
                new Opcode(Instruction.RETURN),
                new Opcode(Instruction.LOAD_NAME, 0),
                new Opcode(Instruction.LOAD_ATTR, 0),
                new Opcode(Instruction.RETURN),
                new Opcode(Instruction.LOAD_NAME, 0),
                new Opcode(Instruction.LOAD_ATTR, 1),
                new Opcode(Instruction.RETURN),
                new Opcode(Instruction.CODE_START),
                new Opcode(Instruction.CODE_STOP)
            }.SequenceEqual(bytecode), string.Join("\n", bytecode));
        }
    }
}
