﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Speedycloud.Compiler.AST_Nodes {
    public class Type : INode{
        public TypeName Name { get; private set; }
        public IEnumerable<IEnumerable<Constraint>> Constraints { get; private set; }
        public bool IsRuntimeCheck { get; private set; }
        public bool IsArrayType { get; private set; }
        public string Flag { get; private set; }
        public List<Type> TypeParameters { get; private set; } 

        public override string ToString() {
            return string.Format("(Type {0} [{5}] <{1}> {2} {3} #{4})", Name, string.Join(", ", Constraints),
                IsRuntimeCheck ? "RUNTIME" : "STATIC", IsArrayType ? "ARRAY" : "SCALAR", Flag, string.Join(" ", TypeParameters));
        }

        protected bool Equals(Type other) {
            return Equals(Name, other.Name) && Constraints.Zip(other.Constraints, (a, b) => a.SequenceEqual(b)).All(t=>t) && IsRuntimeCheck.Equals(other.IsRuntimeCheck) && IsArrayType.Equals(other.IsArrayType) && Flag.Equals(other.Flag) && TypeParameters.SequenceEqual(other.TypeParameters);
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


        public Type(TypeName name, IEnumerable<Type> typeParams, IEnumerable<IEnumerable<Constraint>> constraints, bool isRuntimeCheck = false, bool isArrayType = false, string flag = "") {
            Name = name;
            Constraints = constraints ?? new List<List<Constraint>>();
            IsRuntimeCheck = isRuntimeCheck;
            IsArrayType = isArrayType;
            Flag = flag;
            TypeParameters = new List<Type>(typeParams);
        }

        public Type(TypeName name, IEnumerable<Type> typeParams, IEnumerable<Constraint> constraints, bool isRuntimeCheck = false,
            bool isArrayType = false) :
            this(name, typeParams, new List<IEnumerable<Constraint>> {constraints}, isRuntimeCheck, isArrayType){}

        public Type(TypeName name) : this(name, new List<Type>(), (IEnumerable<IEnumerable<Constraint>>) null) {}

        public Type(TypeName name, bool isRuntimeCheck = false, bool isArrayType = false)
            : this(name, new List<Type>(), (IEnumerable<IEnumerable<Constraint>>) null, isRuntimeCheck, isArrayType) {}

        public T Accept<T>(IAstVisitor<T> visitor) {
            return visitor.Visit(this);
        }
    }
}