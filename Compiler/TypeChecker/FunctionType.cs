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
