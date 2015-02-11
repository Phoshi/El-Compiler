using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Speedycloud.Compiler.AST_Nodes;
using Speedycloud.Compiler.TypeChecker;
using Speedycloud.Compiler.TypeChecker.Constraints;
using Array = Speedycloud.Compiler.AST_Nodes.Array;
using Boolean = Speedycloud.Compiler.AST_Nodes.Boolean;
using String = Speedycloud.Compiler.AST_Nodes.String;

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
    }
}
