using System;

namespace Speedycloud.Compiler.TypeChecker {
    public class TypeCheckException : Exception {
        public static TypeCheckException UnresolvedUnion(ITypeInformation a, ITypeInformation b) {
            return
                new TypeCheckException(string.Format("Attempted union between non-unifiable types {0} and {1}", a,
                    b));
        }
        public TypeCheckException(string error) : base(error){}
    }
}