using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Speedycloud.Compiler.AST_Nodes;
using Speedycloud.Compiler.Lexer;
using Speedycloud.Compiler.TypeChecker;
using Array = Speedycloud.Compiler.AST_Nodes.Array;
using Boolean = Speedycloud.Compiler.AST_Nodes.Boolean;
using String = Speedycloud.Compiler.AST_Nodes.String;
using Type = Speedycloud.Compiler.AST_Nodes.Type;

namespace Speedycloud.Compiler.Parser {
    public class Parser {
        private readonly List<Token> tokens;
        private int position = 0;

        private Token GetCurrentToken() {
            if (position >= tokens.Count) {
                return new Token(TokenType.Eof, "");
            }
            return tokens[position];
        }

        private Token ConsumeCurrentToken() {
            if (position >= tokens.Count) {
                return new Token(TokenType.Eof, "");
            }
            return tokens[position++];
        }

        private Token GetNextToken() {
            if (position >= tokens.Count) {
                return new Token(TokenType.Eof, "");
            }
            return tokens[++position];
        }

        private bool Eof() {
            return position >= tokens.Count;
        }

        private readonly Dictionary<string, int> precidences = new Dictionary<string, int> {
            {"(", 10},
            {"[", 10},
            {"=", 15},
            {"+", 20},
            {"-", 20},
            {"*", 30},
            {"/", 30},
            {"%", 30},

            {"==", 50},
            {"!=", 50},
            {">", 50},
            {"<", 50},
            {"<=", 50},
            {">=", 50},

            {"&&", 40},
            {"||", 40},
        };
        private int GetTokenPrecidence(Token tok) {
            if (precidences.ContainsKey(tok.TokenText)) {
                return precidences[tok.TokenText];
            }

            return -1;
        }

        public Parser(IEnumerable<Token> tokens) {
            this.tokens = new List<Token>(tokens);
        }

        public INode Parse() {
            var expr = ParseMain();
            return ParseBinaryOperator(0, expr);
        }

        public INode ParseBinaryOperator(int expressionPrecidence, INode LHS) {
            while (true) {
                int tokenPrecidence = GetTokenPrecidence(GetCurrentToken());

                if (tokenPrecidence < expressionPrecidence)
                    return LHS;
                Token binOp = GetCurrentToken();
                if (binOp.Type == TokenType.OpenBracket) {
                    LHS = new FunctionCall(((Name)LHS).Value, ParseParameterList());
                    continue;
                }
                GetNextToken();
                var RHS = ParseMain();

                var nextPrecidence = GetTokenPrecidence(GetCurrentToken());
                if (tokenPrecidence < nextPrecidence)
                    RHS = ParseBinaryOperator(tokenPrecidence + 1, RHS);
                if (binOp.Type == TokenType.Symbol) {
                    LHS = new BinaryOp(binOp.TokenText, (IExpression)LHS, (IExpression)RHS);
                }
                else if (binOp.Type == TokenType.OpenSquareBracket) {
                    LHS = new ArrayIndex((IExpression)LHS, (IExpression)RHS);
                    ConsumeCurrentToken();
                }
                else if (binOp.Type == TokenType.Assignment) {
                    if (LHS is Name) {
                        var name = (Name) LHS;
                        name = new Name(name.Value, true);
                        LHS = new Assignment(name, (IExpression) RHS);
                    }
                    else if (LHS is ArrayIndex) {
                        var index = (ArrayIndex) LHS;
                        LHS = new ArrayAssignment(index.Array, index.Index, (IExpression)RHS);
                    }
                }
            }
        }

        private IEnumerable<IExpression> ParseParameterList() {
            ParseException.AssertType(ConsumeCurrentToken(), TokenType.OpenBracket);
            var parameters = new List<IExpression>();
            while (GetCurrentToken().Type != TokenType.CloseBracket) {
                parameters.Add((IExpression)Parse());
                if (GetCurrentToken().Type == TokenType.Comma) {
                    ConsumeCurrentToken();
                }
            }
            ParseException.AssertType(ConsumeCurrentToken(), TokenType.CloseBracket);
            return parameters;
        }

        public INode ParseMain() {
            switch (GetCurrentToken().Type) {
                case TokenType.Number: return ParseNumber();
                case TokenType.OpenSquareBracket: return ParseArray();
                case TokenType.String: return ParseString();
                case TokenType.Name: return ParseName();
                case TokenType.True: case TokenType.False: return ParseBoolean();
                case TokenType.While: return ParseWhile();
                case TokenType.If: return ParseIf();
                case TokenType.Symbol: return ParseUnaryOp();
                case TokenType.Var: case TokenType.Val: return ParseNewAssignment();
                case TokenType.OpenBrace: return ParseBlock();
                case TokenType.Def: return ParseFunctionDefinition();
                case TokenType.Return: return ParseReturn();
                case TokenType.For: return ParseFor();
                case TokenType.Record: return ParseRecord();
            }
            throw ParseException.UnexpectedToken(GetCurrentToken(), TokenType.Number, TokenType.OpenSquareBracket,
                TokenType.String, TokenType.Name, TokenType.True, TokenType.False, TokenType.While, TokenType.If,
                TokenType.Symbol, TokenType.Var, TokenType.Val, TokenType.OpenBrace, TokenType.Def, TokenType.Return,
                TokenType.For, TokenType.Record);
        }

        public IExpression ParseNumber() {
            ParseException.AssertType(GetCurrentToken(), TokenType.Number);
            int intNum;
            if (int.TryParse(GetCurrentToken().TokenText, out intNum)) {
                ConsumeCurrentToken();
                return new Integer(intNum);
            }
            double doubleNum;
            if (double.TryParse(GetCurrentToken().TokenText, out doubleNum)) {
                ConsumeCurrentToken();
                return new Float(doubleNum);
            }
            throw ParseException.UnexpectedToken(GetCurrentToken(), TokenType.Number);
        }

        public Integer ParseInt() {
            return (Integer)ParseNumber();
        }

        public AST_Nodes.Array ParseArray() {
            ParseException.AssertType(GetCurrentToken(), TokenType.OpenSquareBracket);
            ConsumeCurrentToken();

            var exprs = new List<IExpression>();
            while (GetCurrentToken().Type != TokenType.CloseSquareBracket) {
                exprs.Add((IExpression)Parse());
                if (GetCurrentToken().Type == TokenType.Comma) {
                    ConsumeCurrentToken();
                }
            }
            ConsumeCurrentToken();
            return new Array(exprs);
        }

        public IEnumerable<BindingDeclaration> ParseBindingDeclarationList() {
            ParseException.AssertType(ConsumeCurrentToken(), TokenType.OpenBracket);
            var decls = new List<BindingDeclaration>();
            while (GetCurrentToken().Type != TokenType.CloseBracket) {
                decls.Add(ParseBindingDeclaration());
                if (GetCurrentToken().Type == TokenType.Comma) {
                    ConsumeCurrentToken();
                }
                else if (GetCurrentToken().Type == TokenType.CloseBracket) {
                    break;
                }
                else {
                    ParseException.UnexpectedToken(GetCurrentToken(), TokenType.Comma, TokenType.CloseBracket);
                }
            }
            ParseException.AssertType(ConsumeCurrentToken(), TokenType.CloseBracket);
            return decls;
        } 

        public BindingDeclaration ParseBindingDeclaration() {
            ParseException.AssertType(GetCurrentToken(), TokenType.Name);
            var name = ConsumeCurrentToken().TokenText;

            if (GetCurrentToken().Type == TokenType.Colon) {
                ConsumeCurrentToken();
                var type = ParseType();
                return new BindingDeclaration(new Name(name, true), type);
            }
            else {
                return new BindingDeclaration(new Name(name, true), new Type(new TypeName("Any")));
            }
        }
          
        public Boolean ParseBoolean() {
            if (GetCurrentToken().Type == TokenType.True) {
                ConsumeCurrentToken(); 
                return new Boolean(true);
            }
            else if (GetCurrentToken().Type == TokenType.False) {
                ConsumeCurrentToken(); 
                return new Boolean(false);
            }
            throw ParseException.UnexpectedToken(GetCurrentToken(), TokenType.True, TokenType.False);
        }

        public Constraint ParseTypeConstraint() {
            ParseException.AssertType(GetCurrentToken(), TokenType.Name);
            var constraintType = ConsumeCurrentToken().TokenText;
            ParseException.AssertType(GetCurrentToken(), TokenType.Number);
            var constraintArg = ParseNumber();

            return new Constraint(constraintType, constraintArg);
        }

        public Float ParseFloat() {
            return (Float)ParseNumber();
        }

        public For ParseFor() {
            ParseException.AssertType(ConsumeCurrentToken(), TokenType.For);
            ParseException.AssertType(ConsumeCurrentToken(), TokenType.OpenBracket);
            var bind = ParseBindingDeclaration();
            ParseException.AssertType(ConsumeCurrentToken(), TokenType.In);
            var collection = Parse();
            ParseException.AssertType(ConsumeCurrentToken(), TokenType.CloseBracket);
            var executable = Parse();

            return new For(bind, (IExpression)collection, (IStatement)executable);
        }

        public FunctionDefinition ParseFunctionDefinition() {
            ParseException.AssertType(ConsumeCurrentToken(), TokenType.Def);
            var sig = ParseFunctionSignature();
            var executable = Parse();

            return new FunctionDefinition(sig, (IStatement)executable);
        }

        public If ParseIf() {
            ParseException.AssertType(ConsumeCurrentToken(), TokenType.If);
            ParseException.AssertType(ConsumeCurrentToken(), TokenType.OpenBracket);
            var cond = (IExpression)Parse();
            ParseException.AssertType(ConsumeCurrentToken(), TokenType.CloseBracket);
            var concequent = (IStatement)Parse();
            IStatement otherwise = new AST_Nodes.Block(new List<IStatement>());
            if (GetCurrentToken().Type == TokenType.Else) {
                ConsumeCurrentToken();
                otherwise = (IStatement)Parse();
            }

            return new If(cond, concequent, otherwise);

        }

        public Name ParseName() {
            ParseException.AssertType(GetCurrentToken(), TokenType.Name);
            return new Name(ConsumeCurrentToken().TokenText, false);
        }

        public NewAssignment ParseNewAssignment() {
            ParseException.AssertType(GetCurrentToken(), TokenType.Var, TokenType.Val);
            var isWritable = ConsumeCurrentToken().Type == TokenType.Var;
            var name = ParseBindingDeclaration();
            ParseException.AssertType(ConsumeCurrentToken(), TokenType.Assignment);
            var value = (IExpression)Parse();

            return new NewAssignment(name, value, isWritable);
        }

        public Record ParseRecord() {
            ParseException.AssertType(ConsumeCurrentToken(), TokenType.Record);
            ParseException.AssertType(GetCurrentToken(), TokenType.Name);
            var recordName = ConsumeCurrentToken().TokenText;

            ParseException.AssertType(ConsumeCurrentToken(), TokenType.Assignment);
            var bindings = ParseBindingDeclarationList();

            return new Record(recordName, new List<TypeName>(), bindings);
        }

        public Return ParseReturn() {
            ParseException.AssertType(GetCurrentToken(), TokenType.Return);
            ConsumeCurrentToken();

            return new Return((IExpression)Parse());
        }

        public String ParseString() {
            ParseException.AssertType(GetCurrentToken(), TokenType.String);
            return new String(string.Join("", ConsumeCurrentToken().TokenText.Skip(1).Reverse().Skip(1).Reverse()));
        }

        public Type ParseType() {
            Type t;
            if (GetCurrentToken().Type == TokenType.OpenSquareBracket) {
                ConsumeCurrentToken();
                var arrType = ParseType();
                ParseException.AssertType(ConsumeCurrentToken(), TokenType.CloseSquareBracket);
                t = new Type(arrType.Name, arrType.Constraints, isRuntimeCheck: arrType.IsRuntimeCheck,
                    isArrayType: true);
            }
            else {
                ParseException.AssertType(GetCurrentToken(), TokenType.Name);
                t = new Type(ParseTypeName());
            }

            if (GetCurrentToken().Equals(new Token(TokenType.Symbol, "<"))) {
                var constraints = ParseTypeConstraintList();
                t = new Type(t.Name, constraints, isRuntimeCheck: t.IsRuntimeCheck, isArrayType: t.IsArrayType);
            }

            if (GetCurrentToken().Type == TokenType.RuntimeCheck) {
                ConsumeCurrentToken();
                t = new Type(t.Name, t.Constraints, isRuntimeCheck: true, isArrayType: t.IsArrayType);
            }

            return t;
        }

        private IEnumerable<Constraint> ParseTypeConstraintList() {
            ParseException.Assert(ConsumeCurrentToken(), new Token(TokenType.Symbol, "<"));
            var constraints = new List<Constraint>();
            while (!GetCurrentToken().Equals(new Token(TokenType.Symbol, ">"))) {
                constraints.Add(ParseTypeConstraint());
            }
            ParseException.Assert(ConsumeCurrentToken(), new Token(TokenType.Symbol, ">"));
            return constraints;
        }

        public TypeName ParseTypeName() {
            ParseException.AssertType(GetCurrentToken(), TokenType.Name);
            return new TypeName(ConsumeCurrentToken().TokenText);
        }

        public UnaryOp ParseUnaryOp() {
            ParseException.AssertType(GetCurrentToken(), TokenType.Symbol);
            var op = ConsumeCurrentToken().TokenText;
            var operand = (IExpression)Parse();
            return new UnaryOp(op, operand);
        }

        public While ParseWhile() {
            ParseException.AssertType(ConsumeCurrentToken(), TokenType.While);
            ParseException.AssertType(ConsumeCurrentToken(), TokenType.OpenBracket);
            var condition = Parse();
            ParseException.AssertType(ConsumeCurrentToken(), TokenType.CloseBracket);
            var concequence = Parse();

            return new While((IExpression)condition, (IExpression)concequence);

        }

        public AST_Nodes.Block ParseBlock() {
            ParseException.AssertType(ConsumeCurrentToken(), TokenType.OpenBrace);
            var statements = new List<IStatement>();
            while (GetCurrentToken().Type != TokenType.CloseBrace) {
                statements.Add((IStatement)Parse());
                ParseException.AssertType(ConsumeCurrentToken(), TokenType.LineSeperator);
            }

            ParseException.AssertType(ConsumeCurrentToken(), TokenType.CloseBrace);
            return new Block(statements);
        }

        public FunctionSignature ParseFunctionSignature() {
            ParseException.AssertType(GetCurrentToken(), TokenType.Name);
            var funcName = ConsumeCurrentToken().TokenText;
            var parameters = ParseBindingDeclarationList();

            ParseException.AssertType(ConsumeCurrentToken(), TokenType.Colon);
            var type = ParseType();

            return new FunctionSignature(funcName, parameters, type);
        }

        public AST_Nodes.Program ParseProgram() {
            var nodes = new List<INode>();
            while (!Eof()) {
                nodes.Add(Parse());
            }
            return new AST_Nodes.Program(nodes);
        }
    }

    public class ParseException : Exception {
        public static void AssertType(Token actual, params TokenType[] expected) {
            if (!expected.Any(type => actual.Type == type)) {
                throw UnexpectedToken(actual, expected);
            }
        }

        public static void Assert(Token actual, params Token[] expected) {
            if (!expected.Any(actual.Equals)) {
                throw UnexpectedToken(actual, expected);
            }
        }

        public static ParseException UnexpectedToken(Token unexpected, params TokenType[] expected) {
            return new ParseException(string.Format("{0} unexpected at this time. Expected: {1}", unexpected, string.Join(" or ", expected.ToList())));
        }
        public static ParseException UnexpectedToken(Token unexpected, params Token[] expected) {
            return new ParseException(string.Format("{0} unexpected at this time. Expected: {1}", unexpected, string.Join(" or ", expected.ToList())));
        }

        public ParseException(string message) : base(message) {}
    }
}
