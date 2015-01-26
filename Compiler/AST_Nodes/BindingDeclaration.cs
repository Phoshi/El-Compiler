namespace Speedycloud.Compiler.AST_Nodes {
    class BindingDeclaration : INode{
        public Name Name { get; private set; }
        public Type Type { get; private set; }

        public override string ToString() {
            return string.Format("(BindingDeclaration {0} {1})", Name, Type);
        }

        public BindingDeclaration(Name name, Type type) {
            Name = name;
            Type = type;
        }

        public T Accept<T>(IAstVisitor<T> visitor) {
            return visitor.Visit(this);
        }

        protected bool Equals(BindingDeclaration other) {
            return Equals(Name, other.Name) && Equals(Type, other.Type);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((BindingDeclaration) obj);
        }

        public override int GetHashCode() {
            unchecked {
                return ((Name != null ? Name.GetHashCode() : 0)*397) ^ (Type != null ? Type.GetHashCode() : 0);
            }
        }
    }
}