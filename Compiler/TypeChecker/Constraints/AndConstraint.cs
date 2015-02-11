using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speedycloud.Compiler.TypeChecker.Constraints {
    public class AndConstraint : ITypeConstraint {
        public List<ITypeConstraint> Constraints { get; set; }

        public AndConstraint(params ITypeConstraint[] constraints) {
            Constraints = constraints.ToList();
        }

        private Tuple<List<Tuple<ITypeConstraint, ITypeConstraint>>, List<ITypeConstraint>> MatchPairs(AndConstraint other) {
            var matches = new List<Tuple<ITypeConstraint, ITypeConstraint>>();

            var selfConstraints = new List<ITypeConstraint>(Constraints);
            var otherConstraints = new List<ITypeConstraint>(other.Constraints);
            foreach (var typeConstraint in Constraints) {
                foreach (var otherConstraint in other.Constraints) {
                    if (typeConstraint.GetType() == otherConstraint.GetType()){
                        matches.Add(Tuple.Create(typeConstraint, otherConstraint));
                        selfConstraints.Remove(typeConstraint);
                        otherConstraints.Remove(otherConstraint);
                    }
                    else if (typeConstraint.GetType() == typeof (Eq) || otherConstraint.GetType() == typeof (Eq)) {
                        matches.Add(Tuple.Create(typeConstraint, otherConstraint));
                        selfConstraints.Remove(typeConstraint);
                        otherConstraints.Remove(otherConstraint);
                    }

                }
            }
            return Tuple.Create(matches, selfConstraints.Concat(otherConstraints).ToList());
        }

        public override string ToString() {
            return string.Format("(And {0})", string.Join(" && ", Constraints));
        }

        public bool Equals(ITypeConstraint constraint) {
            if (constraint is AndConstraint) {
                var pairs = MatchPairs((AndConstraint) constraint);
                if (pairs.Item2.Count != 0) {
                    return false;
                }
                return pairs.Item1.All(p => p.Item1.Equals(p.Item2));
            }
            return false;
        }

        public bool IsAssignableTo(ITypeConstraint constraint) {
            if (constraint is AndConstraint) {
                var pairs = MatchPairs((AndConstraint)constraint);
                if (pairs.Item2.Count != 0) {
                    return false;
                }
                return pairs.Item1.All(p => p.Item1.IsAssignableTo(p.Item2));
            }
            return Constraints.All(c => c.IsAssignableTo(constraint) || (c is Lt && constraint is Gt) || (c is Gt && constraint is Lt));
        }

        public bool IsSupertypeOf(ITypeConstraint constraint) {
            if (constraint is AndConstraint) {
                var pairs = MatchPairs((AndConstraint)constraint);
                if (pairs.Item2.Count != 0) {
                    return false;
                }
                return pairs.Item1.All(p => p.Item1.IsSupertypeOf(p.Item2));
            }
            return Constraints.All(c => c.IsSupertypeOf(constraint));
        }

        public bool IsSubtypeOf(ITypeConstraint constraint) {
            if (constraint is AndConstraint) {
                var pairs = MatchPairs((AndConstraint)constraint);
                if (pairs.Item2.Count != 0) {
                    return false;
                }
                return pairs.Item1.All(p => p.Item1.IsSubtypeOf(p.Item2));
            }
            return false;
        }
    }
}
