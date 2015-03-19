using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Speedycloud.Compiler.AST_Nodes;
using Type = Speedycloud.Compiler.AST_Nodes.Type;

namespace Speedycloud.Compiler.TypeChecker {
    public class RecordTypeInformation {
        public Func<Dictionary<TypeName, ITypeInformation>, Tuple<ComplexType, FunctionType, IEnumerable<FunctionType>>> BuildType { get; private set; }
        public IEnumerable<TypeName> TypeParameters { get; private set; }
        public List<Type> ParameterTypes { get; private set; }

        public RecordTypeInformation(IEnumerable<TypeName> typeParams, List<Type> parameterTypes, Func<Dictionary<TypeName, ITypeInformation>, Tuple<ComplexType, FunctionType, IEnumerable<FunctionType>>> builderFunction) {
            BuildType = builderFunction;
            TypeParameters = typeParams;
            ParameterTypes = parameterTypes;
        }

        public ComplexType GetType(Dictionary<TypeName, ITypeInformation> parameters) {
            return BuildType(parameters).Item1;
        }

        public FunctionType GetConstructor(Dictionary<TypeName, ITypeInformation> parameters) {
            return BuildType(parameters).Item2;
        }

        public IEnumerable<FunctionType> GetAccessors(Dictionary<TypeName, ITypeInformation> parameters) {
            return BuildType(parameters).Item3;
        }
    }
}
