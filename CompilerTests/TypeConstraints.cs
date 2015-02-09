using System;
using System.Linq.Expressions;
using System.Reflection.Emit;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Speedycloud.Compiler.TypeChecker.Constraints;

namespace CompilerTests {
    [TestClass]
    public class TypeConstraints {
        [TestMethod]
        public void Eq() {
            var eq5 = new Eq(5);
            var eq10 = new Eq(10);

            Assert.IsTrue(eq5.IsAssignableTo(eq5));
            Assert.IsFalse(eq5.IsAssignableTo(eq10));
            Assert.IsFalse(eq5.IsSubtypeOf(eq10));
            Assert.IsFalse(eq10.IsSubtypeOf(eq5));
            Assert.IsFalse(eq10.IsSupertypeOf(eq5));
            Assert.IsFalse(eq5.IsSupertypeOf(eq10));
        }

        [TestMethod]
        public void Lt() {
            var lt5 = new Lt(5);
            var lt10 = new Lt(10);

            Assert.IsTrue(lt5.IsAssignableTo(lt10));
            Assert.IsFalse(lt10.IsAssignableTo(lt5));
            Assert.IsFalse(lt5.Equals(lt10));
            Assert.IsTrue(lt5.Equals(lt5));
            Assert.IsTrue(lt5.IsSubtypeOf(lt10));
        }

        [TestMethod]
        public void Gt() {
            var gt5 = new Gt(5);
            var gt10 = new Gt(10);

            Assert.IsFalse(gt5.IsAssignableTo(gt10));
            Assert.IsTrue(gt10.IsAssignableTo(gt5));
            Assert.IsFalse(gt5.Equals(gt10));
            Assert.IsTrue(gt5.Equals(gt5));
            Assert.IsFalse(gt5.IsSubtypeOf(gt10));
        }

        [TestMethod]
        public void Mod() {
            var mod5 = new Mod(5);
            var mod10 = new Mod(10);
            var mod7 = new Mod(7);

            Assert.IsTrue(mod10.IsAssignableTo(mod5));
            Assert.IsFalse(mod7.IsAssignableTo(mod5));

            Assert.IsTrue(mod10.Equals(mod10));
            Assert.IsFalse(mod5.Equals(mod10));

            Assert.IsTrue(mod5.IsSupertypeOf(mod10));
        }

        [TestMethod]
        public void ModOnLtGtEq() {
            var mod5 = new Mod(5);
            var gt3 = new Gt(3);
            var lt3 = new Lt(3);
            var eq10 = new Eq(10);

            Assert.IsTrue(eq10.IsAssignableTo(mod5));
            Assert.IsFalse(gt3.IsAssignableTo(mod5));
            Assert.IsFalse(mod5.IsAssignableTo(lt3));
        }

        [TestMethod]
        public void EqOnLtGt() {
            var eq7 = new Eq(7);
            var gt5 = new Gt(5);
            var gt10 = new Gt(10);
            var lt5 = new Lt(5);
            var lt10 = new Lt(10);

            Assert.IsTrue(eq7.IsAssignableTo(gt5));
            Assert.IsTrue(eq7.IsAssignableTo(lt10));

            Assert.IsFalse(eq7.IsAssignableTo(gt10));
            Assert.IsFalse(eq7.IsAssignableTo(lt5));

            Assert.IsTrue(eq7.IsSubtypeOf(gt5));
            Assert.IsTrue(eq7.IsSubtypeOf(lt10));

            Assert.IsFalse(eq7.IsSubtypeOf(gt10));
            Assert.IsFalse(eq7.IsSubtypeOf(lt5));
        }

        [TestMethod]
        public void AndConstraint() {
            var bt5and10 = new AndConstraint(new Gt(5), new Lt(10));
            var eq7 = new Eq(7);
            var eq13 = new Eq(13);

            Assert.IsTrue(eq7.IsAssignableTo(bt5and10));
            Assert.IsFalse(eq13.IsAssignableTo(bt5and10));

            Assert.IsTrue(eq7.IsSubtypeOf(bt5and10));
            Assert.IsTrue(bt5and10.IsSupertypeOf(eq7));

            Assert.IsFalse(eq13.IsSubtypeOf(bt5and10));
            Assert.IsFalse(bt5and10.IsSupertypeOf(eq13));
        }

        [TestMethod]
        public void AndOnAnd() {
            var bt5and10 = new AndConstraint(new Gt(5), new Lt(10));
            var bt6and9 = new AndConstraint(new Gt(6), new Lt(9));

            Assert.IsTrue(bt6and9.IsAssignableTo(bt5and10));
            Assert.IsFalse(bt5and10.IsAssignableTo(bt6and9));

            Assert.IsTrue(bt6and9.IsSubtypeOf(bt5and10));
            Assert.IsTrue(bt5and10.IsSupertypeOf(bt6and9));

            Assert.IsFalse(bt6and9.IsSupertypeOf(bt5and10));
            Assert.IsFalse(bt5and10.IsSubtypeOf(bt6and9));
        }

        [TestMethod]
        public void AndOnSmallerAnd() {
            var bt5and10 = new AndConstraint(new Gt(5), new Lt(10));
            var eq7 = new AndConstraint(new Eq(7));
            var gt6 = new AndConstraint(new Gt(6));

            Assert.IsTrue(eq7.IsAssignableTo(bt5and10));
            Assert.IsTrue(bt5and10.IsSupertypeOf(eq7));

            Assert.IsFalse(gt6.IsAssignableTo(bt5and10));
            Assert.IsTrue(eq7.IsAssignableTo(gt6));
            Assert.IsFalse(gt6.IsSupertypeOf(bt5and10));
        }

        [TestMethod]
        public void ComplexAnd() {
            var compound = new AndConstraint(new Gt(0), new Lt(100), new Mod(5));
            
            Assert.IsTrue(new Eq(5).IsAssignableTo(compound));
            Assert.IsFalse(new Eq(1).IsAssignableTo(compound));

            Assert.IsTrue(compound.IsSupertypeOf(new Eq(95)));

            Assert.IsFalse(new Eq(0).IsAssignableTo(compound));
            Assert.IsFalse(new Eq(100).IsAssignableTo(compound));
        }

        [TestMethod]
        public void SimpleAnd() {
            var gt5 = new AndConstraint(new Gt(5));
            var eq10 = new AndConstraint(new Eq(10));

            Assert.IsTrue(eq10.IsAssignableTo(gt5));
            Assert.IsTrue(gt5.IsSupertypeOf(eq10));
            Assert.IsTrue(eq10.IsSubtypeOf(gt5));
        }
    }
}
