using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speedycloud.Compiler.TypeChecker {
    public class RecordTypeInformation {
        public ITypeInformation Type { get; private set; }
        public FunctionType Constructor { get; private set; }
        public IEnumerable<FunctionType> Accessors { get; private set; }

        public RecordTypeInformation(ITypeInformation type, FunctionType constructor,
            IEnumerable<FunctionType> accessors) {
            Type = type;
            Constructor = constructor;
            Accessors = accessors;
        }
    }
}
