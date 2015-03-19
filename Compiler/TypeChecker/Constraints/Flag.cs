using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speedycloud.Compiler.TypeChecker.Constraints {
    class Flag : ITypeConstraint {
        public string FlagName { get; private set; }
        private bool noFlag = false;

        public Flag(String flagName) {
            FlagName = flagName;
        }

        public Flag() {
            noFlag = true;
        }

        public bool Equals(ITypeConstraint constraint) {
            if (constraint is Flag) {
                if (((Flag) constraint).noFlag) {
                    return true;
                }
                return ((Flag) constraint).FlagName == FlagName;
            }
            return noFlag;
        }

        public bool IsAssignableTo(ITypeConstraint constraint) {
            return Equals(constraint);
        }

        public bool IsSupertypeOf(ITypeConstraint constraint) {
            return false;
        }

        public bool IsSubtypeOf(ITypeConstraint constraint) {
            return false;
        }

        public ITypeConstraint UnaryOp(string op) {
            return this;
        }

        public ITypeConstraint BinaryOp(string op, ITypeConstraint constraint) {
            if (!Equals(constraint)) {
                throw new NotImplementedException();
            }
            return this;
        }

        public override string ToString() {
            return string.Format("(Flag #{0})", FlagName);
        }

        protected bool Equals(Flag other) {
            return string.Equals(FlagName, other.FlagName);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Flag) obj);
        }

        public override int GetHashCode() {
            return (FlagName != null ? FlagName.GetHashCode() : 0);
        }
    }
}
