﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Speedycloud.Compiler.AST_Nodes;

namespace Speedycloud.Compiler.TypeChecker {
    public class DoubleType : ITypeInformation{
        public bool IsAssignableTo(ITypeInformation other) {
            if (other is AnyType) return true;
            return other is DoubleType;
        }

        public bool Equals(ITypeInformation other) {
            if (other is AnyType) return true;
            return other is DoubleType;
        }

        public bool IsSubType(ITypeInformation other) {
            if (other is AnyType) return true;
            return false;
        }

        public override string ToString() {
            return "(Double)";
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((DoubleType) obj);
        }

        public override int GetHashCode() {
            return GetType().Name.GetHashCode();
        }

        public bool IsSuperType(ITypeInformation other) {
            if (other is AnyType) return true;
            return other is IntegerType;
        }

        public ITypeInformation Union(ITypeInformation other) {
            if (other is AnyType) return this;
            if (other is DoubleType) {
                return new DoubleType();
            }
            if (other is IntegerType) {
                return new DoubleType();
            }
            throw TypeCheckException.UnresolvedUnion(this, other);
        }

        public ITypeInformation UnaryOp(string op) {
            return new DoubleType();
        }

        public ITypeInformation BinaryOp(string op, ITypeInformation rhs) {
            if (rhs is DoubleType || rhs is IntegerType)
                return new DoubleType();
            throw TypeCheckException.InvalidBinaryOp(this, op, rhs);
        }

        public ITypeInformation LeastSpecificType() {
            return new DoubleType();
        }
    }
}
