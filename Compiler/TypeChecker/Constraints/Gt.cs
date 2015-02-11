using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speedycloud.Compiler.TypeChecker.Constraints {
    public class Gt : ITypeConstraint{
        public int Num { get; private set; }

        public Gt(int num) {
            Num = num;
        }

        public bool Equals(ITypeConstraint constraint) {
            if (constraint is Gt) {
                return Num == ((Gt) constraint).Num;
            }
            return false;
        }

        public bool IsAssignableTo(ITypeConstraint constraint) {
            if (constraint is Gt) {
                return Num >= ((Gt) constraint).Num;
            }
            if (constraint is AndConstraint) {
                return constraint.IsSupertypeOf(this);
            }
            if (constraint is OrConstraint) {
                return constraint.IsSupertypeOf(this);
            }
            return false;
        }

        public bool IsSupertypeOf(ITypeConstraint constraint) {
            if (constraint is Gt) {
                return Num < ((Gt)constraint).Num;
            }
            if (constraint is Eq) {
                return Num < ((Eq) constraint).Num;
            }
            if (constraint is AndConstraint) {
                return constraint.IsSupertypeOf(this);
            }
            if (constraint is OrConstraint) {
                return constraint.IsSupertypeOf(this);
            }
            return false;
        }

        public bool IsSubtypeOf(ITypeConstraint constraint) {
            if (constraint is Gt) {
                return Num > ((Gt)constraint).Num;
            }
            if (constraint is AndConstraint) {
                return constraint.IsSubtypeOf(this);
            }
            if (constraint is OrConstraint) {
                return constraint.IsSubtypeOf(this);
            }
            return false;
        }
    }
}
