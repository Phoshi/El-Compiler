using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Speedycloud.Compiler.AST_Nodes;
using Speedycloud.Compiler.Lexer;
using Array = Speedycloud.Compiler.AST_Nodes.Array;
using Boolean = Speedycloud.Compiler.AST_Nodes.Boolean;
using String = Speedycloud.Compiler.AST_Nodes.String;
using Type = Speedycloud.Compiler.AST_Nodes.Type;

namespace Speedycloud.Compiler.Parser {
    public class Parser {
        private readonly List<Token> tokens;
        private int position = 0;

        private Token GetCurrentToken() {
            return tokens[position];
        }

        private Token GetNextToken() {
            return tokens[++position];
        }

        public Parser(IEnumerable<Token> tokens) {
            this.tokens = new List<Token>(tokens);
        }

        public Integer ParseInt() {
            throw new NotImplementedException();
        }

        public AST_Nodes.Array ParseArray() {
            throw new NotImplementedException();
        }

        public ArrayAssignment ParseArrayAssignment() {
            throw new NotImplementedException();
        }

        public ArrayIndex ParseArrayIndex() {
            throw new NotImplementedException();
        }

        public Assignment ParseAssignment() {
            throw new NotImplementedException();
        }

        public BinaryOp ParseBinaryOperator() {
            throw new NotImplementedException();
        }

        public BindingDeclaration ParseBindingDeclaration() {
            throw new NotImplementedException();
        }

        public Boolean ParseBoolean() {
            throw new NotImplementedException();
        }

        public Constraint ParseTypeConstraint() {
            throw new NotImplementedException();
        }

        public Float ParseFloat() {
            throw new NotImplementedException();
        }

        public For ParseFor() {
            throw new NotImplementedException();
        }

        public FunctionCall ParseFunctionCall() {
            throw new NotImplementedException();
        }

        public FunctionDefinition ParseFunctionDefinition() {
            throw new NotImplementedException();
        }

        public If ParseIf() {
            throw new NotImplementedException();
        }

        public Name ParseName() {
            throw new NotImplementedException();
        }

        public NewAssignment ParseNewAssignment() {
            throw new NotImplementedException();
        }

        public Record ParseRecord() {
            throw new NotImplementedException();
        }

        public Return ParseReturn() {
            throw new NotImplementedException();
        }

        public String ParseString() {
            throw new NotImplementedException();
        }

        public Type ParseType() {
            throw new NotImplementedException();
        }

        public TypeName ParseTypeName() {
            throw new NotImplementedException();
        }

        public UnaryOp ParseUnaryOp() {
            throw new NotImplementedException();
        }

        public While ParseWhile() {
            throw new NotImplementedException();
        }
    }
}
