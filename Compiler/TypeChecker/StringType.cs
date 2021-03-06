﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Speedycloud.Compiler.AST_Nodes;

namespace Speedycloud.Compiler.TypeChecker {
    public class StringType : ITypeInformation{
        public bool IsAssignableTo(ITypeInformation other) {
            if (other is AnyType) return true;
            if (other.IsAssignableTo(new ArrayType(new AnyType()))) return true;
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

        protected bool Equals(StringType other) {
            return true;
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((StringType) obj);
        }

        public override int GetHashCode() {
            return GetType().Name.GetHashCode();
        }

        public ITypeInformation Union(ITypeInformation other) {
            if (other is AnyType) return this;
            if (other is StringType) {
                return new StringType();
            }
            throw TypeCheckException.UnresolvedUnion(this, other);
        }

        public ITypeInformation UnaryOp(string op) {
            throw new NotImplementedException();
        }

        public ITypeInformation BinaryOp(string op, ITypeInformation rhs) {
            if (op == "+") {
                return new StringType();
            }
            throw new NotImplementedException();
        }

        public ITypeInformation LeastSpecificType() {
            return new StringType();
        }
    }
}
