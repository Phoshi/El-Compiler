using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speedycloud.Compiler.TypeChecker.Constraints {
    public class OrConstraint : ITypeConstraint {
        public List<ITypeConstraint> Constraints { get; set; }

        public override string ToString() {
            return string.Format("(Or {0})", string.Join(" || ", Constraints));
        }

        public OrConstraint(params ITypeConstraint[] constraints) {
            Constraints = constraints.ToList();
        }

        public bool Equals(ITypeConstraint constraint) {
            if (constraint is OrConstraint) {
                return Constraints.All(c => ((OrConstraint) constraint).Constraints.Any(c.Equals));
            }
            return Constraints.Any(c => c.Equals(constraint));
        }

        public bool IsAssignableTo(ITypeConstraint constraint) {
            return Constraints.All(c => c.IsAssignableTo(constraint));
        }

        public bool IsSupertypeOf(ITypeConstraint constraint) {
            return Constraints.Any(c => c.IsSupertypeOf(constraint)) ||
                Constraints.Any(c=>c.IsAssignableTo(constraint));
        }

        public bool IsSubtypeOf(ITypeConstraint constraint) {
            return Constraints.Any(c => c.IsSubtypeOf(constraint));
        }
    }
}
