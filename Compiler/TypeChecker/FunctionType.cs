using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speedycloud.Compiler.TypeChecker {
    class FunctionType {
        private readonly IEnumerable<ITypeInformation> parameters;
        private readonly ITypeInformation returnType;

        public FunctionType(IEnumerable<ITypeInformation> parameters, ITypeInformation returnType) {
            this.parameters = parameters;
            this.returnType = returnType;
        }

        public IEnumerable<ITypeInformation> Parameters {
            get { return parameters; }
        }

        public ITypeInformation ReturnType {
            get { return returnType; }
        }
    }
}
