using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Speedycloud.Compiler.AST_Nodes {
    public class Type : INode{
        public TypeName Name { get; private set; }
        public IEnumerable<Constraint> Constraints { get; private set; }
        public bool IsRuntimeCheck { get; private set; }
        public bool IsArrayType { get; private set; }

        public override string ToString() {
            return string.Format("(Type {0} [{1}] {2} {3})", Name, string.Join(", ", Constraints),
                IsRuntimeCheck ? "RUNTIME" : "STATIC", IsArrayType ? "ARRAY" : "SCALAR");
        }

        protected bool Equals(Type other) {
            return Equals(Name, other.Name) && Constraints.SequenceEqual(other.Constraints) && IsRuntimeCheck.Equals(other.IsRuntimeCheck) && IsArrayType.Equals(other.IsArrayType);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Type) obj);
        }

        public override int GetHashCode() {
            unchecked {
                int hashCode = (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Constraints != null ? Constraints.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ IsRuntimeCheck.GetHashCode();
                hashCode = (hashCode*397) ^ IsArrayType.GetHashCode();
                return hashCode;
            }
        }

        public Type(TypeName name, IEnumerable<Constraint> constraints, bool isRuntimeCheck, bool isArrayType) {
            Name = name;
            Constraints = constraints;
            IsRuntimeCheck = isRuntimeCheck;
            IsArrayType = isArrayType;
        }

        public T Accept<T>(IAstVisitor<T> visitor) {
            return visitor.Visit(this);
        }
    }
}