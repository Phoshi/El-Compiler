﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Speedycloud.Compiler.AST_Nodes;

namespace Speedycloud.Compiler.TypeChecker {
    public class BooleanType : ITypeInformation{
        public bool IsAssignableTo(ITypeInformation other) {
            if (other is AnyType) return true;
            return other is BooleanType;
        }

        public bool Equals(ITypeInformation other) {
            if (other is AnyType) return true;
            return other is BooleanType;
        }

        public override string ToString() {
            return "(Boolean)";
        }

        public bool IsSubType(ITypeInformation other) {
            if (other is AnyType) return true;
            return false;
        }

        protected bool Equals(BooleanType other) {
            return true;
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((BooleanType) obj);
        }

        public override int GetHashCode() {
            return GetType().Name.GetHashCode();
        }

        public bool IsSuperType(ITypeInformation other) {
            if (other is AnyType) return true;
            return false;
        }

        public ITypeInformation Union(ITypeInformation other) {
            if (other is AnyType) return this;
            if (other is BooleanType) {
                return new BooleanType();
            }
            throw TypeCheckException.UnresolvedUnion(this, other);
        }

        public ITypeInformation UnaryOp(string op) {
            return new BooleanType();
        }

        public ITypeInformation BinaryOp(string op, ITypeInformation rhs) {
            var validOps = new List<string> { "==", "!=", ">", "<", "<=", ">=", "&&", "||" };
            if (!validOps.Contains(op)) {
                throw TypeCheckException.InvalidBinaryOp(this, op, rhs);
            }
            if (rhs is BooleanType)
                return new BooleanType();
            throw new NotImplementedException();
        }

        public ITypeInformation LeastSpecificType() {
            return new BooleanType();
        }
    }
}
