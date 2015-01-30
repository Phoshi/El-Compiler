using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speedycloud.Compiler.TypeChecker.Constraints {
    public class Eq : ITypeConstraint {
        public int Num { get; private set; }

        public Eq(int num) {
            Num = num;
        }

        public bool Equals(ITypeConstraint constraint) {
            if (constraint is Eq) {
                return Num == ((Eq) constraint).Num;
            }
            return false;
        }

        public bool IsAssignableTo(ITypeConstraint constraint) {
            if (constraint is Lt) {
                return Num < ((Lt) constraint).Num;
            }
            if (constraint is Gt) {
                return Num > ((Gt)constraint).Num;
            }
            return Equals(constraint);
        }

        public bool IsSupertypeOf(ITypeConstraint constraint) {
            return false;
        }

        public bool IsSubtypeOf(ITypeConstraint constraint) {
            if (constraint is Lt) {
                return Num < ((Lt)constraint).Num;
            }
            if (constraint is Gt) {
                return Num > ((Gt)constraint).Num;
            }
            return false;
        }
    }
}
