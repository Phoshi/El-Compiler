using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speedycloud.Compiler.TypeChecker.Constraints {
    public class Mod : ITypeConstraint{
        public int Num { get; private set; }

        public Mod(int num) {
            Num = num;
        }

        public override string ToString() {
            return string.Format("(Mod {0})", Num);
        }

        public bool Equals(ITypeConstraint constraint) {
            if (constraint is Mod) {
                return Num == ((Mod) constraint).Num;
            }
            return false;
        }

        public bool IsAssignableTo(ITypeConstraint constraint) {
            if (constraint is Mod) {
                return (Num % ((Mod)constraint).Num)==0;
            }
            return false;
        }

        public bool IsSupertypeOf(ITypeConstraint constraint) {
            if (constraint is Mod) {
                return (((Mod)constraint).Num % Num) == 0;
            }
            if (constraint is Eq) {
                return (((Eq) constraint).Num%Num) == 0;
            }
            return false;
        }

        public bool IsSubtypeOf(ITypeConstraint constraint) {
            if (constraint is Mod) {
                return (Num % ((Mod)constraint).Num) == 0;
            }
            return false;
        }

        public ITypeConstraint UnaryOp(string op) {
            if (op == "-") {
                return new Mod(-Num);
            }
            throw TypeCheckException.InvalidUnaryOp(op, this);
        }

        public ITypeConstraint BinaryOp(string op, ITypeConstraint constraint) {
            throw new NotImplementedException();
        }
    }
}
