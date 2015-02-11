using System;
using Speedycloud.Compiler.TypeChecker.Constraints;

namespace Speedycloud.Compiler.TypeChecker {
    public class TypeCheckException : Exception {
        public static TypeCheckException UnresolvedUnion(ITypeInformation a, ITypeInformation b) {
            return
                new TypeCheckException(string.Format("Attempted union between non-unifiable types {0} and {1}", a,
                    b));
        }
        public TypeCheckException(string error) : base(error){}

        public static TypeCheckException UnknowableTypeUsage(ITypeInformation a) {
            return new TypeCheckException("Attempted to use type information in unknowable context.");
        }

        public static TypeCheckException TypeMismatch(ITypeInformation expected, ITypeInformation actual) {
            return new TypeCheckException(string.Format("Attempted to use type {0} where {1} expected.", actual, expected));
        }

        public static Exception InvalidUnaryOp(string op, ITypeConstraint typeConstraint) {
            return
                new TypeCheckException(string.Format("Attempted to use unary operator {0} on invalid constraint {1}", op,
                    typeConstraint));
        }
    }
}