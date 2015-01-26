namespace Speedycloud.Compiler.AST_Nodes {
    class Constraint : INode{
        public string Name { get; private set; }

        public override string ToString() {
            return string.Format("(Constraint {0} {1})", Name, Expression);
        }

        public IExpression Expression { get; private set; }

        public Constraint(string name, IExpression expression) {
            Name = name;
            Expression = expression;
        }

        public T Accept<T>(IAstVisitor<T> visitor) {
            return visitor.Visit(this);
        }

        protected bool Equals(Constraint other) {
            return string.Equals(Name, other.Name) && Equals(Expression, other.Expression);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Constraint) obj);
        }

        public override int GetHashCode() {
            unchecked {
                return ((Name != null ? Name.GetHashCode() : 0)*397) ^ (Expression != null ? Expression.GetHashCode() : 0);
            }
        }
    }
}