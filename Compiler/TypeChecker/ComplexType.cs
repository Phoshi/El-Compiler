using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Speedycloud.Compiler.AST_Nodes;

namespace Speedycloud.Compiler.TypeChecker {
    public class ComplexType : ITypeInformation {
        private readonly List<ITypeInformation> attributes;

        public ComplexType(params ITypeInformation[] attributes) {
            this.attributes = attributes.ToList();
        }

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
