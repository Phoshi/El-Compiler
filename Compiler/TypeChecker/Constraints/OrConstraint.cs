using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speedycloud.Compiler.TypeChecker.Constraints {
    public class OrConstraint : ITypeConstraint {
        public List<ITypeConstraint> Constraints { get; set; }

        public OrConstraint(params ITypeConstraint[] constraints) {
            Constraints = constraints.ToList();
        }

        public bool Equals(ITypeConstraint constraint) {
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
