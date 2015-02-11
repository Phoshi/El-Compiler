﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speedycloud.Compiler.TypeChecker {
    public class StringType : ITypeInformation{
        public bool IsAssignableTo(ITypeInformation other) {
            if (other is AnyType) return true;
            return other is StringType;
        }

        public bool Equals(ITypeInformation other) {
            if (other is AnyType) return true;
            return other is StringType;
        }

        public bool IsSubType(ITypeInformation other) {
            if (other is AnyType) return true;
            return false;
        }

        public bool IsSuperType(ITypeInformation other) {
            if (other is AnyType) return true;
            return false;
        }

        public override string ToString() {
            return "(String)";
        }

        public ITypeInformation Union(ITypeInformation other) {
            if (other is AnyType) return this;
            if (other is StringType) {
                return new StringType();
            }
            throw TypeCheckException.UnresolvedUnion(this, other);
        }
    }
}
