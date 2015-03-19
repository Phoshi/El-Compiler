using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Speedycloud.Compiler.AST_Nodes;

namespace Speedycloud.Compiler.TypeChecker {
    public class ComplexType : ITypeInformation {
        public string Name { get; private set; }
        public Dictionary<TypeName, ITypeInformation> Parameters { get; private set; }
        private readonly List<ITypeInformation> attributes;

        public ComplexType(string name, Dictionary<TypeName, ITypeInformation> parameters, params ITypeInformation[] attributes) {
            Name = name;
            Parameters = parameters;
            this.attributes = attributes.ToList();
        }

        public ComplexType(string name, params ITypeInformation[] attributes)
            : this(name, new Dictionary<TypeName, ITypeInformation>(), attributes) {}

        public bool IsAssignableTo(ITypeInformation other) {
            if (other is ComplexType) {
                var otherType = (ComplexType) other;
                if (attributes.Count != otherType.attributes.Count) {
                    return false;
                }
                return attributes.Zip(otherType.attributes, (a, b) => a.IsAssignableTo(b)).All(t => t);
            }
            return false;
        }

        protected bool Equals(ComplexType other) {
            return attributes.SequenceEqual(other.attributes) && string.Equals(Name, other.Name) && Parameters.SequenceEqual(other.Parameters);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ComplexType) obj);
        }

        public override int GetHashCode() {
            unchecked {
                int hashCode = (attributes != null ? attributes.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Parameters != null ? Parameters.GetHashCode() : 0);
                return hashCode;
            }
        }

        public override string ToString() {
            return string.Format("(Record {0} [{1}])", Name, string.Join(" ", Parameters.Select(p=>p.Key.Name + "=" + p.Value)));
        }

        public bool Equals(ITypeInformation other) {
            if (other is ComplexType) {
                var otherType = (ComplexType)other;
                if (attributes.Count != otherType.attributes.Count) {
                    return false;
                }
                return attributes.Zip(otherType.attributes, (a, b) => a.Equals(b)).All(t => t);
            }
            return false;
        }

        public bool IsSubType(ITypeInformation other) {
            if (other is ComplexType) {
                var otherType = (ComplexType)other;
                if (attributes.Count != otherType.attributes.Count) {
                    return false;
                }
                var subtypeRelations = attributes.Zip(otherType.attributes, (a, b) => a.IsSubType(b)).ToList();
                var assignableRelations = attributes.Zip(otherType.attributes, (a, b) => a.IsAssignableTo(b)).ToList();
                if (assignableRelations.All(t => t) && subtypeRelations.Any(t => t)) {
                    return true;
                }
                    
                return subtypeRelations.All(t => t);
            }
            return false;
        }

        public bool IsSuperType(ITypeInformation other) {
            if (other is ComplexType) {
                var otherType = (ComplexType)other;
                if (attributes.Count != otherType.attributes.Count) {
                    return false;
                }
                return attributes.Zip(otherType.attributes, (a, b) => a.IsSuperType(b)).All(t => t);
            }
            return false;
        }

        public ITypeInformation Union(ITypeInformation other) {
            throw new NotImplementedException();
        }

        public ITypeInformation UnaryOp(string op) {
            throw new NotImplementedException();
        }

        public ITypeInformation BinaryOp(string op, ITypeInformation rhs) {
            throw new NotImplementedException();
        }

        public ITypeInformation LeastSpecificType() {
            return this;
        }
    }
}
