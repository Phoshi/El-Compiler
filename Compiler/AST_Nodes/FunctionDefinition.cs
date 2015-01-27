namespace Speedycloud.Compiler.AST_Nodes {
    public class FunctionDefinition : INode{
        public FunctionSignature Signature { get; private set; }
        public IStatement Statement { get; private set; }

        public FunctionDefinition(FunctionSignature signature, IStatement statement) {
            Signature = signature;
            Statement = statement;
        }

        public override string ToString() {
            return string.Format("(FunctionDefinition {0} {1})", Signature, Statement);
        }

        protected bool Equals(FunctionDefinition other) {
            return Equals(Signature, other.Signature) && Equals(Statement, other.Statement);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((FunctionDefinition) obj);
        }

        public override int GetHashCode() {
            unchecked {
                return ((Signature != null ? Signature.GetHashCode() : 0)*397) ^ (Statement != null ? Statement.GetHashCode() : 0);
            }
        }

        public T Accept<T>(IAstVisitor<T> visitor) {
            return visitor.Visit(this);
        }
    }
}