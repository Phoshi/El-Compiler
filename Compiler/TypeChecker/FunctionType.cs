using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speedycloud.Compiler.TypeChecker {
    public class FunctionType {
        private readonly string name;
        private readonly IEnumerable<ITypeInformation> parameters;
        private readonly ITypeInformation returnType;

        public FunctionType(string name, IEnumerable<ITypeInformation> parameters, ITypeInformation returnType) {
            this.name = name;
            this.parameters = parameters;
            this.returnType = returnType;
        }

        protected bool Equals(FunctionType other) {
            return string.Equals(name, other.name) && parameters.SequenceEqual(other.parameters) && Equals(returnType, other.returnType);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((FunctionType) obj);
        }

        public override int GetHashCode() {
            unchecked {
                int hashCode = (name != null ? name.GetHashCode() : 0);
                hashCode = parameters.Aggregate(hashCode, (current, e) => (current*397) ^ (e.GetHashCode()));
                hashCode = (hashCode*397) ^ (returnType != null ? returnType.GetHashCode() : 0);
                return hashCode;
            }
        }

        public IEnumerable<ITypeInformation> Parameters {
            get { return parameters; }
        }

        public ITypeInformation ReturnType {
            get { return returnType; }
        }

        public string Name {
            get { return name; }
        }

        public override string ToString() {
            return string.Format("({0}({1}): {2}", name, string.Join(", ", parameters), returnType);
        }
    }
}
