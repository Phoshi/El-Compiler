using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Speedycloud.Compiler.AST_Nodes;
using Speedycloud.Compiler.TypeChecker;
using Speedycloud.Compiler.TypeChecker.Constraints;
using Array = Speedycloud.Compiler.AST_Nodes.Array;
using Boolean = Speedycloud.Compiler.AST_Nodes.Boolean;
using String = Speedycloud.Compiler.AST_Nodes.String;
using Type = Speedycloud.Compiler.AST_Nodes.Type;

namespace CompilerTests {
    [TestClass]
    public class TypeChecker {
        [TestMethod]
        public void EqualBaseTypes() {
            var int1 = new IntegerType();
            var int2 = new IntegerType();

            Assert.AreEqual(int1, int2);
            Assert.IsTrue(int1.IsAssignableTo(int2));
        }

        [TestMethod]
        public void AssignableSubTypes() {
            var int1 = new IntegerType();
            var double1 = new DoubleType();

            Assert.AreNotEqual(int1, double1, "Non-equal types are equal");
            Assert.IsTrue(int1.IsAssignableTo(double1), "Int not assignable to double");
            Assert.IsFalse(double1.IsAssignableTo(int1), "Double assignable to int");
            Assert.IsTrue(double1.IsSuperType(int1), "Double not supertype of int");
            Assert.IsFalse(int1.IsSuperType(double1), "Int supertype of double");
            Assert.IsFalse(double1.IsSubType(int1), "Double subtype of int");
            Assert.IsTrue(int1.IsSubType(double1), "Int not subtype of double");
            Assert.AreEqual(int1.Union(double1), new DoubleType(), "Union of int|double not double");
            Assert.AreEqual(double1.Union(int1), new DoubleType(), "Union of double|int not double");
        }

        [TestMethod]
        public void NonassignableSubtypes() {
            var int1 = new IntegerType();
            var double1 = new DoubleType();
            var string1 = new StringType();

            Assert.IsTrue(string1.Equals(string1) && string1.IsAssignableTo(string1), "String does not equal string or is unassignable to string");
            Assert.IsFalse(
                string1.Equals(int1) || 
                string1.Equals(double1) ||
                string1.IsAssignableTo(int1) ||
                string1.IsAssignableTo(double1)
                , "String equals or is assignable to non-string type");
        }

        [TestMethod]
        public void BooleanType() {
            var int1 = new IntegerType();
            var double1 = new DoubleType();
            var string1 = new StringType();
            var boolean1 = new BooleanType();

            Assert.IsTrue(boolean1.Equals(boolean1));
            Assert.IsTrue(boolean1.IsAssignableTo(boolean1));
            Assert.IsFalse(boolean1.IsAssignableTo(int1));
            Assert.IsFalse(double1.IsAssignableTo(boolean1));
            Assert.IsFalse(string1.IsSubType(boolean1));
        }

        [TestMethod]
        public void ArrayType() {
            var arr = new ArrayType(new IntegerType());
            var int1 = new IntegerType();

            Assert.IsTrue(arr.Equals(arr));
            Assert.IsTrue(arr.IsAssignableTo(arr));
            Assert.IsFalse(int1.Equals(arr));
            Assert.IsFalse(arr.IsAssignableTo(int1));
            Assert.IsFalse(int1.IsAssignableTo(arr));
        }

        [TestMethod]
        public void ComplexArrayTyping() {
            var arr = new ArrayType(new IntegerType());
            var arr2 = new ArrayType(new ConstrainedType(new IntegerType(), new Eq(5)));

            Assert.IsTrue(arr.IsSuperType(arr2));
            Assert.IsTrue(arr2.IsSubType(arr));
            Assert.IsTrue(arr2.IsAssignableTo(arr));
            Assert.IsFalse(arr.IsAssignableTo(arr2));
        }

        [TestMethod]
        public void ConstrainedInt() {
            var eq7 = new ConstrainedType(new IntegerType(), new Eq(7));
            var gt6 = new ConstrainedType(new IntegerType(), new Gt(6));
            var gt8 = new ConstrainedType(new IntegerType(), new Gt(8));

            Assert.IsFalse(eq7.Equals(gt6) || gt6.Equals(gt8) || gt8.Equals(eq7));
            Assert.IsTrue(eq7.IsAssignableTo(gt6));
            Assert.IsTrue(gt8.IsAssignableTo(gt6));
            Assert.IsFalse(eq7.IsAssignableTo(gt8));
        }

        [TestMethod]
        [ExpectedException(typeof(TypeCheckException))]
        public void InvalidUnion() {
            new StringType().Union(new IntegerType());
        }

        [TestMethod]
        public void TypeofInteger() {
            var tree = new Integer(1);

            var tc = new Typechecker();

            var type = tc.Visit(tree);

            Assert.IsTrue(type.Equals(new ConstrainedType(new IntegerType(), new Eq(1))));
        }

        [TestMethod]
        public void TypeofString() {
            var tree = new String("foo");

            var tc = new Typechecker();

            var type = tc.Visit(tree);

            Assert.IsTrue(type.Equals(new ConstrainedType(new StringType(), new Eq(3))));
        }

        [TestMethod]
        public void TypeofFloat() {
            var tree = new Float(1.5);

            var tc = new Typechecker();

            var type = tc.Visit(tree);

            Assert.IsTrue(type.Equals(new ConstrainedType(new DoubleType(), new Eq(1.5m))));
        }

        [TestMethod]
        public void TypeofBool() {
            var tree = new Boolean(true);

            var tc = new Typechecker();

            var type = tc.Visit(tree);

            Assert.IsTrue(type.Equals(new BooleanType()));
        }

        [TestMethod]
        public void TypeofArray() {
            var tree = new Array(new List<IExpression>{new Integer(3), new Integer(5)});

            var tc = new Typechecker();

            var type = tc.Visit(tree);

            Assert.IsTrue(
                type.Equals(
                    new ConstrainedType(
                        new ArrayType(
                            new ConstrainedType(new IntegerType(), new OrConstraint(new Eq(3), new Eq(5)))),
                        new Eq(2))));
        }

        [TestMethod]
        public void TypeofArrayIndex() {
            var arr = new Array(new List<IExpression>{new Integer(3), new Integer(5)});
            var tree = new ArrayIndex(arr, new Integer(1));

            var tc = new Typechecker();
            var type = tc.Visit(tree);
            Assert.IsTrue(type.Equals(new ConstrainedType(new IntegerType(), new OrConstraint(new Eq(3), new Eq(5)))));
        }

        [TestMethod]
        [ExpectedException(typeof(TypeCheckException))]
        public void TypeofArrayIndexIncorrectIndex() {
            var arr = new Array(new List<IExpression> { new Integer(3), new Integer(5) });
            var tree = new ArrayIndex(arr, new String("foo"));

            var tc = new Typechecker();
            var type = tc.Visit(tree);
        }

        [TestMethod]
        [ExpectedException(typeof(TypeCheckException))]
        public void TypeofArrayIndexIncorrectArray() {
            var tree = new ArrayIndex(new Integer(1), new Integer(1));

            var tc = new Typechecker();
            var type = tc.Visit(tree);
        }

        [TestMethod]
        public void TypeofArrayAssignment() {
            var arr = new Array(new List<IExpression> { new Integer(3), new Integer(5) });
            var tree = new ArrayAssignment(arr, new Integer(0), new Integer(5));

            var tc = new Typechecker();
            var type = tc.Visit(tree);
            Assert.IsTrue(type is UnknownType);
        }

        [TestMethod]
        [ExpectedException(typeof(TypeCheckException))]
        public void TypeofArrayAssignmentIncorrectArray() {
            var tree = new ArrayAssignment(new Integer(1), new Integer(0), new Integer(5));

            var tc = new Typechecker();
            var type = tc.Visit(tree);
        }

        [TestMethod]
        [ExpectedException(typeof(TypeCheckException))]
        public void TypeofArrayAssignmentOutofBounds() {
            var arr = new Array(new List<IExpression> { new Integer(3), new Integer(5) });
            var tree = new ArrayAssignment(arr, new Integer(2), new Integer(5));

            var tc = new Typechecker();
            var type = tc.Visit(tree);
        }
        [TestMethod]
        [ExpectedException(typeof(TypeCheckException))]
        public void TypeofArrayAssignmentOutofLowerBounds() {
            var arr = new Array(new List<IExpression> { new Integer(3), new Integer(5) });
            var tree = new ArrayAssignment(arr, new Integer(-1), new Integer(5));

            var tc = new Typechecker();
            var type = tc.Visit(tree);
        }

        [TestMethod]
        [ExpectedException(typeof(TypeCheckException))]
        public void TypeofArrayAssignmentIncorrectIndex() {
            var arr = new Array(new List<IExpression> { new Integer(3), new Integer(5) });
            var tree = new ArrayAssignment(arr, new String("foo"), new Integer(5));

            var tc = new Typechecker();
            var type = tc.Visit(tree);
        }

        [TestMethod]
        [ExpectedException(typeof(TypeCheckException))]
        public void TypeofArrayAssignmentIncorrectAssignment() {
            var arr = new Array(new List<IExpression> { new Integer(3), new Integer(5) });
            var tree = new ArrayAssignment(arr, new Integer(1), new String("foo"));

            var tc = new Typechecker();
            var type = tc.Visit(tree);
        }

        [TestMethod]
        public void TypeofUnary() {
            var tree = new UnaryOp("-", new Integer(3));
            var tc = new Typechecker();

            var type = tc.Visit(tree);
            
            Assert.IsTrue(type.Equals(new ConstrainedType(new IntegerType(), new Eq(-3))));
        }

        [TestMethod]
        public void TypeofUnaryBool() {
            var treeb = new UnaryOp("!", new Boolean(false));
            var tc = new Typechecker();

            var typeb = tc.Visit(treeb);

            Assert.IsTrue(typeb.Equals(new BooleanType()));
        }

        [TestMethod]
        public void TypeofBinaryAdd() {
            var tc = new Typechecker();

            var ints = new BinaryOp("+", new Integer(2), new Integer(2));
            var intAndFloat = new BinaryOp("+", new Integer(2), new Float(2));
            var floatAndFloat = new BinaryOp("+", new Float(2), new Float(2));

            Assert.IsTrue(tc.Visit(ints).Equals(new ConstrainedType(new IntegerType(), new Eq(4))));
            Assert.IsTrue(tc.Visit(intAndFloat).Equals(new ConstrainedType(new DoubleType(), new Eq(4))));
            Assert.IsTrue(tc.Visit(floatAndFloat).Equals(new ConstrainedType(new DoubleType(), new Eq(4))));
        }

        [TestMethod]
        public void TypeofBinarySub() {
            var tc = new Typechecker();

            var ints = new BinaryOp("-", new Integer(2), new Integer(1));
            var intAndFloat = new BinaryOp("-", new Integer(2), new Float(1));
            var floatAndFloat = new BinaryOp("-", new Float(2), new Float(1));

            Assert.IsTrue(tc.Visit(ints).Equals(new ConstrainedType(new IntegerType(), new Eq(1))));
            Assert.IsTrue(tc.Visit(intAndFloat).Equals(new ConstrainedType(new DoubleType(), new Eq(1))));
            Assert.IsTrue(tc.Visit(floatAndFloat).Equals(new ConstrainedType(new DoubleType(), new Eq(1))));
        }

        [TestMethod]
        public void TypeofBinaryMul() {
            var tc = new Typechecker();

            var ints = new BinaryOp("*", new Integer(2), new Integer(3));
            var intAndFloat = new BinaryOp("*", new Integer(2), new Float(3));
            var floatAndFloat = new BinaryOp("*", new Float(2), new Float(3));

            Assert.IsTrue(tc.Visit(ints).Equals(new ConstrainedType(new IntegerType(), new Eq(6))));
            Assert.IsTrue(tc.Visit(intAndFloat).Equals(new ConstrainedType(new DoubleType(), new Eq(6))));
            Assert.IsTrue(tc.Visit(floatAndFloat).Equals(new ConstrainedType(new DoubleType(), new Eq(6))));
        }

        [TestMethod]
        public void TypeofBinaryDiv() {
            var tc = new Typechecker();

            var ints = new BinaryOp("/", new Integer(2), new Integer(3));
            var intAndFloat = new BinaryOp("/", new Integer(2), new Float(3));
            var floatAndFloat = new BinaryOp("/", new Float(2), new Float(3));

            Assert.IsTrue(tc.Visit(ints).Equals(new ConstrainedType(new IntegerType(), new Eq(0))));
            Assert.IsTrue(tc.Visit(intAndFloat).Equals(new ConstrainedType(new DoubleType(), new Eq(2/3m))));
            Assert.IsTrue(tc.Visit(floatAndFloat).Equals(new ConstrainedType(new DoubleType(), new Eq(2/3m))));
        }

        [TestMethod]
        [ExpectedException(typeof(TypeCheckException))]
        public void TypeofBinaryIntWithBool() {
            var tc = new Typechecker();

            var op = new BinaryOp("+", new Integer(3), new Boolean(true));
            tc.Visit(op);
        }
        [TestMethod]
        [ExpectedException(typeof(TypeCheckException))]
        public void TypeofBinaryFloatWithBool() {
            var tc = new Typechecker();

            var op = new BinaryOp("+", new Float(3), new Boolean(true));
            tc.Visit(op);
        }

        [TestMethod]
        public void TypeofBinaryOpBooleans() {
            var tc = new Typechecker();

            var successCases = new List<string> {"==", "!=", ">", "<", "<=", ">=", "&&", "||"};
            foreach (var successCase in successCases) {
                var bin = new BinaryOp(successCase, new Boolean(true), new Boolean(false));

                Assert.IsTrue(tc.Visit(bin).Equals(new BooleanType()));    
            }
        }

        [TestMethod]
        [ExpectedException(typeof (TypeCheckException))]
        public void TypeofBinaryOpBooleanFailure() {
            var tc = new Typechecker();

            var failureCases = new List<string> { "+", "-", "/", "*", /*and all others, really*/ };
            foreach (var failureCase in failureCases) {
                var bin = new BinaryOp(failureCase, new Boolean(true), new Boolean(false));

                Assert.IsTrue(tc.Visit(bin).Equals(new BooleanType()));
            }
        }

        [TestMethod]
        public void BindingDeclarationPlainInteger() {
            var tc = new Typechecker();

            var decl = new BindingDeclaration(new Name("Foo", true), new Type(new TypeName("Integer")));
            var result = tc.Visit(decl);

            Assert.IsTrue(result is UnknownType);
            Assert.IsTrue(tc.Names["Foo"].Type.Equals(new IntegerType()));
        }

        [TestMethod]
        public void BindingDeclarationPlainFloat() {
            var tc = new Typechecker();

            var decl = new BindingDeclaration(new Name("Foo", true), new Type(new TypeName("Double")));
            var result = tc.Visit(decl);

            Assert.IsTrue(result is UnknownType);
            Assert.IsTrue(tc.Names["Foo"].Type.Equals(new DoubleType()));
        }

        [TestMethod]
        public void BindingDeclarationPlainBool() {
            var tc = new Typechecker();

            var decl = new BindingDeclaration(new Name("Foo", true), new Type(new TypeName("Boolean")));
            var result = tc.Visit(decl);

            Assert.IsTrue(result is UnknownType);
            Assert.IsTrue(tc.Names["Foo"].Type.Equals(new BooleanType()));
        }

        [TestMethod]
        public void BindingDeclarationPlainString() {
            var tc = new Typechecker();

            var decl = new BindingDeclaration(new Name("Foo", true), new Type(new TypeName("String")));
            var result = tc.Visit(decl);

            Assert.IsTrue(result is UnknownType);
            Assert.IsTrue(tc.Names["Foo"].Type.Equals(new StringType()));
        }

        [TestMethod]
        public void BindingDeclarationPlainIntegerArray() {
            var tc = new Typechecker();

            var decl = new BindingDeclaration(new Name("Foo", true), new Type(new TypeName("Integer"), isArrayType: true));
            var result = tc.Visit(decl);

            Assert.IsTrue(result is UnknownType);
            Assert.IsTrue(tc.Names["Foo"].Type.Equals(new ArrayType(new IntegerType())));
        }

        [TestMethod]
        public void BindingDeclarationConstrainedInteger() {
            var tc = new Typechecker();

            var decl = new BindingDeclaration(new Name("Foo", true),
                new Type(new TypeName("Integer"), new List<Type>(), constraints: new List<Constraint> {
                    new Constraint("Eq", new Integer(3))
                }));
            var result = tc.Visit(decl);

            Assert.IsTrue(result is UnknownType);
            Assert.IsTrue(tc.Names["Foo"].Type.Equals(new ConstrainedType(new IntegerType(), new Eq(3))));
        }
        [TestMethod]
        public void BindingDeclarationMultiConstrainedInteger() {
            var tc = new Typechecker();

            var decl = new BindingDeclaration(new Name("Foo", true),
                new Type(new TypeName("Integer"), new List<Type>(), constraints: new List<Constraint> {
                    new Constraint("Gt", new Integer(3)),
                    new Constraint("Lt", new Integer(10))
                }));
            var result = tc.Visit(decl);

            Assert.IsTrue(result is UnknownType);
            Assert.IsTrue(tc.Names["Foo"].Type.IsAssignableTo(new ConstrainedType(new IntegerType(), new AndConstraint(new Gt(3), new Lt(10)))));
        }
        [TestMethod]
        public void BindingDeclarationMultiConstrainedIntegerArray() {
            var tc = new Typechecker();

            var decl = new BindingDeclaration(new Name("Foo", true),
                new Type(new TypeName("Integer"), new List<Type>(), constraints: new List<Constraint> {
                    new Constraint("Gt", new Integer(3)),
                    new Constraint("Lt", new Integer(10))
                }, isArrayType: true));
            var result = tc.Visit(decl);

            Assert.IsTrue(result is UnknownType);
            Assert.IsTrue(
                tc.Names["Foo"].Type.IsAssignableTo(
                    new ArrayType(new ConstrainedType(new IntegerType(), new AndConstraint(new Gt(3), new Lt(10))))));
        }
        [TestMethod]
        public void BindingDeclarationMultiConstrainedIntegerArrayAtRuntime() {
            var tc = new Typechecker();

            var decl = new BindingDeclaration(new Name("Foo", true),
                new Type(new TypeName("Integer"), new List<Type>(), constraints: new List<Constraint> {
                    new Constraint("Gt", new Integer(3)),
                    new Constraint("Lt", new Integer(10))
                }, isArrayType: true, isRuntimeCheck: true));
            var result = tc.Visit(decl);

            Assert.IsTrue(result is UnknownType);
            Assert.IsTrue(tc.Names["Foo"].Type.Equals(new AnyType()));
        }

        [TestMethod]
        public void IfStatementWithBool() {
            var tc = new Typechecker();

            var ifStatement = new If(new Boolean(true), new Integer(3), new Integer(2));
            tc.Visit(ifStatement);
        }
        [TestMethod]
        [ExpectedException(typeof(TypeCheckException))]
        public void IfStatementWithoutBool() {
            var tc = new Typechecker();

            var ifStatement = new If(new Integer(3), new Integer(3), new Integer(2));
            tc.Visit(ifStatement);
        }

        [TestMethod]
        [ExpectedException(typeof(TypeCheckException))]
        public void IfStatementCheckConcequent() {
            var tc = new Typechecker();

            var ifStatement = new If(new Boolean(true), new NewAssignment(
                new BindingDeclaration(new Name("f", true), new Type(new TypeName("Integer"))), new Boolean(true), false),
                new Integer(2));
            tc.Visit(ifStatement);
        }

        [TestMethod]
        public void NewAssignment() {
            var tc = new Typechecker();
            var assignment = new NewAssignment(
                new BindingDeclaration(new Name("f", true), new Type(new TypeName("Integer"))), new Integer(3), false);
            tc.Visit(assignment);

            Assert.IsTrue(tc.Names["f"].Type.Equals(new IntegerType()));
        }

        [TestMethod]
        [ExpectedException(typeof(TypeCheckException))]
        public void NewAssignmentFailure() {
            var tc = new Typechecker();
            var assignment = new NewAssignment(
                new BindingDeclaration(new Name("f", true), new Type(new TypeName("Integer"))), new Boolean(true), false);
            tc.Visit(assignment);
        }

        [TestMethod]
        public void Assignment() {
            var tc = new Typechecker();
            tc.Names["f"] = new BindingInformation("f", new IntegerType(), true);
            var assignment = new Assignment(new Name("f", true), new Integer(3));
            tc.Visit(assignment);
        }

        [TestMethod]
        [ExpectedException(typeof(TypeCheckException))]
        public void AssignmentFailure() {
            var tc = new Typechecker();
            tc.Names["f"] = new BindingInformation("f", new ConstrainedType(new IntegerType(), new Eq(4)), true);
            var assignment = new Assignment(new Name("f", true), new Integer(3));
            tc.Visit(assignment);
        }

        [TestMethod]
        public void Program() {
            var tc = new Typechecker();
            var newAssignment = new NewAssignment(
                new BindingDeclaration(new Name("f", true), new Type(new TypeName("Integer"))), new Integer(3), true);
            var assignment = new Assignment(new Name("f", true), new Integer(5));
            var program = new Block(new List<IStatement> {newAssignment, assignment});
            tc.Visit(program);

            Assert.IsTrue(tc.Names["f"].Type.Equals(new IntegerType()));
        }

        [TestMethod]
        [ExpectedException(typeof(TypeCheckException))]
        public void ProgramFailure() {
            var tc = new Typechecker();
            var newAssignment = new NewAssignment(
                new BindingDeclaration(new Name("f", true),
                    new Type(new TypeName("Integer"), new List<Type>(), new List<Constraint> { new Constraint("Eq", new Integer(3)) })),
                new Integer(3), false);
            var assignment = new Assignment(new Name("f", true), new Integer(5));
            var program = new Block(new List<IStatement> { newAssignment, assignment });
            tc.Visit(program);
        }

        [TestMethod]
        public void For() {
            var tc = new Typechecker();
            var tree = new For(new BindingDeclaration(new Name("i", true), new Type(new TypeName("Integer"))),
                new Array(new List<IExpression> {new Integer(3)}), new Boolean(true));

            tc.Visit(tree);
        }

        [TestMethod]
        [ExpectedException(typeof(TypeCheckException))]
        public void ForInvalidBinding() {
            var tc = new Typechecker();
            var tree = new For(new BindingDeclaration(new Name("i", true), new Type(new TypeName("Boolean"))),
                new Array(new List<IExpression> { new Integer(3) }), new Boolean(true));

            tc.Visit(tree);
        }

        [TestMethod]
        [ExpectedException(typeof(TypeCheckException))]
        public void ForInvalidEnumerable() {
            var tc = new Typechecker();
            var tree = new For(new BindingDeclaration(new Name("i", true), new Type(new TypeName("Integer"))),
                new Integer(3), new Boolean(true));

            tc.Visit(tree);
        }

        [TestMethod]
        [ExpectedException(typeof(TypeCheckException))]
        public void ForInvalidBlock() {
            var tc = new Typechecker();
            var tree = new For(new BindingDeclaration(new Name("i", true), new Type(new TypeName("Integer"))),
                new Array(new List<IExpression> {new Integer(3)}), new NewAssignment(
                    new BindingDeclaration(new Name("f", true), new Type(new TypeName("Integer"))), new Boolean(true),
                    false));

            tc.Visit(tree);
        }

        [TestMethod]
        public void While() {
            var tc = new Typechecker();
            var tree = new While(new Boolean(true), new Integer(2));

            tc.Visit(tree);
        }

        [TestMethod]
        [ExpectedException(typeof(TypeCheckException))]
        public void WhileInvalidCondition() {
            var tc = new Typechecker();
            var tree = new While(new Integer(3), new Integer(2));

            tc.Visit(tree);
        }

        [TestMethod]
        [ExpectedException(typeof(TypeCheckException))]
        public void WhileInvalidBlock() {
            var tc = new Typechecker();
            var tree = new While(new Boolean(true), new NewAssignment(
                    new BindingDeclaration(new Name("f", true), new Type(new TypeName("Integer"))), new Boolean(true),
                    false));

            tc.Visit(tree);
        }

        [TestMethod]
        public void FunctionDefinition() {
            var tc = new Typechecker();
            var func =
                new FunctionDefinition(
                    new FunctionSignature("add",
                        new List<BindingDeclaration> {
                            new BindingDeclaration(new Name("a", true), new Type(new TypeName("Integer"))),
                            new BindingDeclaration(new Name("b", true), new Type(new TypeName("Integer")))
                        },
                        new Type(new TypeName("Integer"))),
                    new Return(new BinaryOp("+", new Name("a", false), new Name("b", false))));
            tc.Visit(func);
        }


        [TestMethod]
        [ExpectedException(typeof(TypeCheckException))]
        public void FunctionDefinitionInvalidReturn() {
            var tc = new Typechecker();
            var func =
                new FunctionDefinition(
                    new FunctionSignature("add",
                        new List<BindingDeclaration> {
                            new BindingDeclaration(new Name("a", true), new Type(new TypeName("Integer"))),
                            new BindingDeclaration(new Name("b", true), new Type(new TypeName("Integer")))
                        },
                        new Type(new TypeName("Boolean"))),
                    new Return(new BinaryOp("+", new Name("a", false), new Name("b", false))));
            tc.Visit(func);
        }

        [TestMethod]
        public void FunctionCall() {
            var tc = new Typechecker();
            var func =
                new FunctionDefinition(
                    new FunctionSignature("add",
                        new List<BindingDeclaration> {
                            new BindingDeclaration(new Name("a", true), new Type(new TypeName("Integer"))),
                            new BindingDeclaration(new Name("b", true), new Type(new TypeName("Integer")))
                        },
                        new Type(new TypeName("Integer"))),
                    new Return(new BinaryOp("+", new Name("a", false), new Name("b", false))));
            var call = new FunctionCall("add", new List<IExpression> {new Integer(2), new Integer(2)});
            var tree = new Block(new List<IStatement> {func, call});

            tc.Visit(tree);
        }

        [TestMethod]
        [ExpectedException(typeof(TypeCheckException))]
        public void FunctionCallFailure() {
            var tc = new Typechecker();
            var func =
                new FunctionDefinition(
                    new FunctionSignature("add",
                        new List<BindingDeclaration> {
                            new BindingDeclaration(new Name("a", true), new Type(new TypeName("Integer"))),
                            new BindingDeclaration(new Name("b", true), new Type(new TypeName("Integer")))
                        },
                        new Type(new TypeName("Integer"))),
                    new Return(new BinaryOp("+", new Name("a", false), new Name("b", false))));
            var call = new FunctionCall("add", new List<IExpression> { new Boolean(true), new Integer(2) });
            var tree = new Block(new List<IStatement> { func, call });

            tc.Visit(tree);
        }

        [TestMethod]
        public void FunctionCallAssignment() {
            var tc = new Typechecker();
            var func =
                new FunctionDefinition(
                    new FunctionSignature("add",
                        new List<BindingDeclaration> {
                            new BindingDeclaration(new Name("a", true), new Type(new TypeName("Integer"))),
                            new BindingDeclaration(new Name("b", true), new Type(new TypeName("Integer")))
                        },
                        new Type(new TypeName("Integer"))),
                    new Return(new BinaryOp("+", new Name("a", false), new Name("b", false))));
            var call = new FunctionCall("add", new List<IExpression> { new Integer(2), new Integer(2) });
            var assign =
                new NewAssignment(new BindingDeclaration(new Name("x", true), new Type(new TypeName("Integer"))), call,
                    false);
            var tree = new Block(new List<IStatement> { func, assign });

            tc.Visit(tree);
        }

        [TestMethod]
        [ExpectedException(typeof(TypeCheckException))]
        public void FunctionCallAssignmentFailure() {
            var tc = new Typechecker();
            var func =
                new FunctionDefinition(
                    new FunctionSignature("add",
                        new List<BindingDeclaration> {
                            new BindingDeclaration(new Name("a", true), new Type(new TypeName("Integer"))),
                            new BindingDeclaration(new Name("b", true), new Type(new TypeName("Integer")))
                        },
                        new Type(new TypeName("Integer"))),
                    new Return(new BinaryOp("+", new Name("a", false), new Name("b", false))));
            var call = new FunctionCall("add", new List<IExpression> { new Integer(2), new Integer(2) });
            var assign =
                new NewAssignment(new BindingDeclaration(new Name("x", true), new Type(new TypeName("Boolean"))), call,
                    false);
            var tree = new Block(new List<IStatement> { func, assign });

            tc.Visit(tree);
        }

        [TestMethod]
        public void FunctionDefOverload() {
            var func =
                new FunctionDefinition(
                    new FunctionSignature("add",
                        new List<BindingDeclaration> {
                            new BindingDeclaration(new Name("a", true), new Type(new TypeName("Integer"))),
                            new BindingDeclaration(new Name("b", true), new Type(new TypeName("Integer")))
                        },
                        new Type(new TypeName("Integer"))),
                    new Return(new BinaryOp("+", new Name("a", false), new Name("b", false))));
            var func2 =
                new FunctionDefinition(
                    new FunctionSignature("add",
                        new List<BindingDeclaration> {
                            new BindingDeclaration(new Name("a", true), new Type(new TypeName("Boolean"))),
                            new BindingDeclaration(new Name("b", true), new Type(new TypeName("Boolean")))
                        },
                        new Type(new TypeName("Boolean"))),
                    new Return(new BinaryOp("&&", new Name("a", false), new Name("b", false))));
            var tree = new Block(new List<IStatement> {func, func2});

            var tc = new Typechecker();
            tc.Visit(tree);

            Assert.AreEqual(2, tc.Functions.Count);
            Assert.IsTrue(tc.Functions.Count(f => f.Name == "add" && f.Parameters.All(p=>p.Equals(new IntegerType()))) == 1);
            Assert.IsTrue(tc.Functions.Count(f => f.Name == "add" && f.Parameters.All(p => p.Equals(new BooleanType()))) == 1);
        }

        [TestMethod]
        public void FunctionCallOverload() {
            var func =
                new FunctionDefinition(
                    new FunctionSignature("add",
                        new List<BindingDeclaration> {
                            new BindingDeclaration(new Name("a", true), new Type(new TypeName("Integer"))),
                            new BindingDeclaration(new Name("b", true), new Type(new TypeName("Integer")))
                        },
                        new Type(new TypeName("Integer"))),
                    new Return(new BinaryOp("+", new Name("a", false), new Name("b", false))));
            var func2 =
                new FunctionDefinition(
                    new FunctionSignature("add",
                        new List<BindingDeclaration> {
                            new BindingDeclaration(new Name("a", true), new Type(new TypeName("Boolean"))),
                            new BindingDeclaration(new Name("b", true), new Type(new TypeName("Boolean")))
                        },
                        new Type(new TypeName("Boolean"))),
                    new Return(new BinaryOp("&&", new Name("a", false), new Name("b", false))));
            var call1 = new FunctionCall("add", new List<IExpression> {new Integer(2), new Integer(2)});
            var call2 = new FunctionCall("add", new List<IExpression> { new Boolean(true), new Boolean(false) });
            var tree = new Block(new List<IStatement> {func, func2, call1, call2});

            var tc = new Typechecker();
            tc.Visit(tree);

            Assert.AreEqual(tc.FunctionDefinitions[func], tc.FunctionCalls[call1]);
            Assert.AreEqual(tc.FunctionDefinitions[func2], tc.FunctionCalls[call2]);
        }

        [TestMethod]
        [ExpectedException(typeof(TypeCheckException))]
        public void FunctionCallOverloadFailure() {
            var func =
                new FunctionDefinition(
                    new FunctionSignature("add",
                        new List<BindingDeclaration> {
                            new BindingDeclaration(new Name("a", true), new Type(new TypeName("Integer"))),
                            new BindingDeclaration(new Name("b", true), new Type(new TypeName("Integer")))
                        },
                        new Type(new TypeName("Integer"))),
                    new Return(new BinaryOp("+", new Name("a", false), new Name("b", false))));
            var func2 =
                new FunctionDefinition(
                    new FunctionSignature("add",
                        new List<BindingDeclaration> {
                            new BindingDeclaration(new Name("a", true), new Type(new TypeName("Boolean"))),
                            new BindingDeclaration(new Name("b", true), new Type(new TypeName("Boolean")))
                        },
                        new Type(new TypeName("Boolean"))),
                    new Return(new BinaryOp("&&", new Name("a", false), new Name("b", false))));
            var call1 = new FunctionCall("add", new List<IExpression> { new Integer(2), new Integer(2) });
            var call2 = new FunctionCall("add", new List<IExpression> { new String("foo"), new Boolean(false) });
            var tree = new Block(new List<IStatement> { func, func2, call1, call2 });

            var tc = new Typechecker();
            tc.Visit(tree);
        }

        [TestMethod]
        public void ComplexType() {
            var comp1 = new ComplexType("", new IntegerType());
            var comp2 = new ComplexType("", new IntegerType(), new IntegerType());
            var comp3 = new ComplexType("", new StringType());
            var comp4 = new ComplexType("", comp1, comp2, comp3);
            var comp5 = new ComplexType("", new ConstrainedType(new IntegerType(), new Eq(5)));
            var comp6 = new ComplexType("", comp5, comp2, comp3);

            Assert.IsTrue(comp1.IsAssignableTo(comp1));
            Assert.IsTrue(comp5.IsAssignableTo(comp1));
            Assert.IsFalse(comp1.IsAssignableTo(comp2));
            Assert.IsTrue(comp4.Equals(comp4));

            Assert.IsTrue(comp5.IsSubType(comp1));
            Assert.IsTrue(comp1.IsSuperType(comp5));
            Assert.IsTrue(comp6.IsSubType(comp4));
            Assert.IsTrue(comp6.IsAssignableTo(comp4));
        }

        [TestMethod]
        public void Record() {
            var record = new Record("Point", new List<TypeName>(), new List<BindingDeclaration> {
                new BindingDeclaration(new Name("x", true), new Type(new TypeName("Integer"))),
                new BindingDeclaration(new Name("y", true), new Type(new TypeName("Integer"))),
            });
            var forceInstantiation = new Type(new TypeName("Point"));

            var tc = new Typechecker();
            tc.Visit(record);
            tc.Visit(forceInstantiation);

            Assert.AreEqual(3, tc.Functions.Count);
            Assert.IsTrue(
                tc.Functions.Count(
                    f =>
                        f.Name == "Point" && 
                        f.Parameters.Count() == 2 &&
                        f.Parameters.All(p => p.Equals(new IntegerType())) &&
                        f.ReturnType.Equals(new ComplexType("Point", new IntegerType(), new IntegerType()))) == 1);

            Assert.IsTrue(
                tc.Functions.Count(
                    f =>
                        f.Name == "x" &&
                        f.Parameters.Count() == 1 &&
                        f.Parameters.All(p => p.Equals(new ComplexType("Point", new IntegerType(), new IntegerType()))) &&
                        f.ReturnType.Equals(new IntegerType())) == 1);

            Assert.IsTrue(
                tc.Functions.Count(
                    f =>
                        f.Name == "y" &&
                        f.Parameters.Count() == 1 &&
                        f.Parameters.All(p => p.Equals(new ComplexType("Point", new IntegerType(), new IntegerType()))) &&
                        f.ReturnType.Equals(new IntegerType())) == 1);

            Assert.IsTrue(tc.Records["Point"].GetType(new Dictionary<TypeName, ITypeInformation>()).Equals(new ComplexType("Point", new IntegerType(), new IntegerType())));
        }

        [TestMethod]
        public void RecordUsage() {
            var record = new Record("Point", new List<TypeName>(), new List<BindingDeclaration> {
                new BindingDeclaration(new Name("x", true), new Type(new TypeName("Integer"))),
                new BindingDeclaration(new Name("y", true), new Type(new TypeName("Integer"))),
            });
            var newRecord = new FunctionCall("Point", new List<IExpression> {new Integer(5), new Integer(10)});
            var assign = new NewAssignment(
                new BindingDeclaration(new Name("p", true), new Type(new TypeName("Point"))), newRecord, false);
            var access = new FunctionCall("x", new List<IExpression> {new Name("p", false)});
            var assignAccess =
                new NewAssignment(new BindingDeclaration(new Name("xVal", true), new Type(new TypeName("Integer"))),
                    access, false);

            var tree = new Block(new List<IStatement> {record, assign, assignAccess});

            var tc = new Typechecker();
            tc.Visit(tree);
        }

        [TestMethod]
        [ExpectedException(typeof(TypeCheckException))]
        public void RecordUsageFailure() {
            var record = new Record("Point", new List<TypeName>(), new List<BindingDeclaration> {
                new BindingDeclaration(new Name("x", true), new Type(new TypeName("Integer"))),
                new BindingDeclaration(new Name("y", true), new Type(new TypeName("Integer"))),
            });
            var newRecord = new FunctionCall("Point", new List<IExpression> { new Integer(5), new Integer(10) });
            var assign = new NewAssignment(
                new BindingDeclaration(new Name("p", true), new Type(new TypeName("Point"))), newRecord, false);
            var access = new FunctionCall("x", new List<IExpression> { new Name("p", false) });
            var assignAccess =
                new NewAssignment(new BindingDeclaration(new Name("xVal", true), new Type(new TypeName("Boolean"))),
                    access, false);

            var tree = new Block(new List<IStatement> { record, assign, assignAccess });

            var tc = new Typechecker();
            tc.Visit(tree);
        }
    }
}
