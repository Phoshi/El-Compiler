using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Speedycloud.Compiler.AST_Nodes;
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

        public static Exception InvalidBinaryOp(ITypeInformation lhs, string op, ITypeInformation rhs) {
            return
                new TypeCheckException(string.Format(
                    "Attempted to use binary operator {0} on invalid types {1} and {2}", op, lhs, rhs));
        }

        public static Exception UnknownOverload(FunctionCall call, List<FunctionType> definitions) {
            return new TypeCheckException(string.Format(
                "Attempted to call function {0} with parameters {1}, but no suitable overload was found. Known overloads are {2}",
                call.Name, string.Join(", ", call.Parameters), string.Join("; ", definitions)));
        }

        public static Exception ReadonlyAssignment(Assignment assignment) {
            return new TypeCheckException(string.Format("Attempted to reassign readonly binding {0} with {1}", assignment.Binding, assignment.Expression));
        }
    }
}