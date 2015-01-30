using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Speedycloud.Compiler.TypeChecker;

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
        [ExpectedException(typeof(TypeCheckException))]
        public void InvalidUnion() {
            new StringType().Union(new IntegerType());
        }
    }
}
