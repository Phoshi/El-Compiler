using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Speedycloud.Compiler.AST_Nodes;
using Speedycloud.Compiler.TypeChecker.Constraints;

namespace Speedycloud.Compiler.TypeChecker {
    public class ConstrainedType : ITypeInformation {
        public ITypeInformation Type { get; set; }
        public ITypeConstraint Constraint { get; set; }

        public ConstrainedType(ITypeInformation type, ITypeConstraint constraint) {
            Type = type;
            Constraint = constraint;
        }

        public bool IsAssignableTo(ITypeInformation other) {
            if (other is AnyType) return true;
            if (other is ConstrainedType) {
                var otherType = other as ConstrainedType;
                return Type.IsAssignableTo(otherType.Type) && Constraint.IsAssignableTo(otherType.Constraint);
            }
            return Type.IsAssignableTo(other);
        }

        public bool Equals(ITypeInformation other) {
            if (other is AnyType) return true;
            if (other is ConstrainedType) {
                var otherType = other as ConstrainedType;
                return Type.Equals(otherType.Type) && Constraint.Equals(otherType.Constraint);
            }
            return false;
        }

        public override string ToString() {
            return string.Format("(Constrained {0} {1})", Type, Constraint);
        }

        public bool IsSubType(ITypeInformation other) {
            if (other is AnyType) return true;
            if (other is ConstrainedType) {
                var otherType = other as ConstrainedType;
                return Type.IsSubType(otherType.Type) && Constraint.IsSubtypeOf(otherType.Constraint);
            }
            return Type.IsSubType(other) || Type.IsAssignableTo(other);
        }

        public bool IsSuperType(ITypeInformation other) {
            if (other is AnyType) return true;
            if (other is ConstrainedType) {
                var otherType = other as ConstrainedType;
                return Type.IsSuperType(otherType.Type) && Constraint.IsSupertypeOf(otherType.Constraint);
            }
            return Type.IsSuperType(other);
        }

        public ITypeInformation Union(ITypeInformation other) {
            if (other is AnyType) return this;
            if (other is ConstrainedType) {
                var otherType = other as ConstrainedType;
                return new ConstrainedType(Type.Union(otherType.Type), new OrConstraint(Constraint, otherType.Constraint));
            }
            return Type.Union(other);
        }

        public ITypeInformation UnaryOp(string op) {
            return new ConstrainedType(Type.UnaryOp(op), Constraint.UnaryOp(op));
        }

        public ITypeInformation BinaryOp(string op, ITypeInformation rhs) {
            if (rhs is ConstrainedType) {
                var other = rhs as ConstrainedType;
                var newBaseType = Type.BinaryOp(op, other.Type);
                if (op == "/" && !newBaseType.Equals(new IntegerType())) op = "//";
                try {
                    var newConstraintType = Constraint.BinaryOp(op, other.Constraint);
                    return new ConstrainedType(newBaseType, newConstraintType);
                }
                catch (NotImplementedException) {
                    return Type.BinaryOp(op, rhs);
                }
            }
            return Type.BinaryOp(op, rhs);
        }

        public ITypeInformation LeastSpecificType() {
            if (Type is ArrayType)
                return this;
            return Type;
        }
    }
}
