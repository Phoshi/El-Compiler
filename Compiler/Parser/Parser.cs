using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
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
        public Dictionary<INode, InputPosition> NodeLocations = new Dictionary<INode, InputPosition>(); 
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

            {"(", 60},
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
            var token = GetCurrentToken();
            Program.LogIn("Parser", "Parsing" + token);
            var expr = ParseMain();
            Program.LogOut("Parser", "Parsed " + expr);
            NodeLocations[expr] = token.Position;
            return ParseBinaryOperator(0, expr);
        }

        public INode ParseBinaryOperator(int expressionPrecidence, INode LHS) {
            while (true) {
                int tokenPrecidence = GetTokenPrecidence(GetCurrentToken());

                if (tokenPrecidence < expressionPrecidence)
                    return LHS;
                Token binOp = GetCurrentToken();
                if (binOp.Type == TokenType.OpenBracket) {
                    Program.LogIn("Parser", "Parsing function call...");
                    LHS = new FunctionCall(((Name)LHS).Value, ParseParameterList());
                    Program.LogOut("Parser", "Parsed " + LHS);
                    NodeLocations[LHS] = binOp.Position;
                    continue;
                }
                GetNextToken();
                var RHS = ParseMain();

                var nextPrecidence = GetTokenPrecidence(GetCurrentToken());
                if (tokenPrecidence < nextPrecidence)
                    RHS = ParseBinaryOperator(tokenPrecidence + 1, RHS);
                if (binOp.Type == TokenType.Symbol) {
                    Program.LogIn("Parser", "Parsing binary operator...");
                    LHS = new BinaryOp(binOp.TokenText, (IExpression)LHS, (IExpression)RHS);
                    Program.LogOut("Parser", "Parsed " + LHS);
                    NodeLocations[LHS] = binOp.Position;
                }
                else if (binOp.Type == TokenType.OpenSquareBracket) {
                    Program.LogIn("Parser", "Parsing array index...");
                    LHS = new ArrayIndex((IExpression)LHS, (IExpression)RHS);
                    Program.LogOut("Parser", "Parsed " + LHS);
                    NodeLocations[LHS] = binOp.Position;
                    ConsumeCurrentToken();
                }
                else if (binOp.Type == TokenType.Assignment) {
                    if (LHS is Name) {
                        Program.LogIn("Parser", "Parsing variable assignment...");
                        var name = (Name) LHS;
                        name = new Name(name.Value, true);
                        LHS = new Assignment(name, (IExpression) RHS);
                        Program.LogOut("Parser", "Parsed " + LHS);
                        NodeLocations[LHS] = binOp.Position;
                    }
                    else if (LHS is ArrayIndex) {
                        Program.LogIn("Parser", "Parsing array index assignment...");
                        var index = (ArrayIndex) LHS;
                        LHS = new ArrayAssignment(index.Array, index.Index, (IExpression)RHS);
                        Program.LogOut("Parser", "Parsed " + LHS);
                        NodeLocations[LHS] = binOp.Position;
                    }
                }
            }
        }

        private IEnumerable<IExpression> ParseParameterList() {
            Program.LogIn("Parser", "Parsing parameter list...");
            ParseException.AssertType(ConsumeCurrentToken(), TokenType.OpenBracket);
            var parameters = new List<IExpression>();
            while (GetCurrentToken().Type != TokenType.CloseBracket) {
                parameters.Add((IExpression)Parse());
                if (GetCurrentToken().Type == TokenType.Comma) {
                    ConsumeCurrentToken();
                }
            }
            ParseException.AssertType(ConsumeCurrentToken(), TokenType.CloseBracket);
            Program.LogOut("Parser", "Parsed (" + string.Join(", ", parameters) + ")");
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
                Program.Log("Parser", "Parsed integer " + intNum);
                ConsumeCurrentToken();
                return new Integer(intNum);
            }
            double doubleNum;
            if (double.TryParse(GetCurrentToken().TokenText, out doubleNum)) {
                Program.Log("Parser", "Parsed double " + doubleNum);
                ConsumeCurrentToken();
                return new Float(doubleNum);
            }
            throw ParseException.UnexpectedToken(GetCurrentToken(), TokenType.Number);
        }

        public Integer ParseInt() {
            return (Integer)ParseNumber();
        }

        public AST_Nodes.Array ParseArray() {
            Program.LogIn("Parser", "Parsing array...");
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
            Program.LogOut("Parser", "Parsed [" + string.Join(", ", exprs) + "]");
            return new Array(exprs);
        }

        public IEnumerable<BindingDeclaration> ParseBindingDeclarationList() {
            Program.LogIn("Parser", "Parsing binding declaration list...");
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
            Program.LogOut("Parser", "Parsed " + string.Join(", ", decls));
            return decls;
        } 

        public BindingDeclaration ParseBindingDeclaration() {
            ParseException.AssertType(GetCurrentToken(), TokenType.Name);
            var name = ConsumeCurrentToken().TokenText;
            Program.Log("Parser", "Parsing binding declaration for " + name);
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
                Program.Log("Parser", "Parsed True");
                ConsumeCurrentToken(); 
                return new Boolean(true);
            }
            else if (GetCurrentToken().Type == TokenType.False) {
                Program.Log("Parser", "Parsed False");
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

            Program.Log("Parser", "Parsed type constraint " + constraintType + " " + constraintArg);
            return new Constraint(constraintType, constraintArg);
        }

        public Float ParseFloat() {
            return (Float)ParseNumber();
        }

        public For ParseFor() {
            Program.LogIn("Parser", "Parsing for loop...");
            ParseException.AssertType(ConsumeCurrentToken(), TokenType.For);
            ParseException.AssertType(ConsumeCurrentToken(), TokenType.OpenBracket);
            var bind = ParseBindingDeclaration();
            ParseException.AssertType(ConsumeCurrentToken(), TokenType.In);
            var collection = Parse();
            ParseException.AssertType(ConsumeCurrentToken(), TokenType.CloseBracket);
            var executable = Parse();

            var loop = new For(bind, (IExpression)collection, (IStatement)executable);
            Program.LogOut("Parser", "Parsed " + loop);
            return loop;
        }

        public FunctionDefinition ParseFunctionDefinition() {
            Program.LogIn("Parser", "Parsing function definition...");
            ParseException.AssertType(ConsumeCurrentToken(), TokenType.Def);
            var sig = ParseFunctionSignature();
            var executable = Parse();

            var def = new FunctionDefinition(sig, (IStatement)executable);
            Program.LogOut("Parser", "Parsed " + def);
            return def;
        }

        public If ParseIf() {
            Program.LogIn("Parser", "Parsing if...");
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

            var conditional = new If(cond, concequent, otherwise);
            Program.LogOut("Parser", "Parsed " + conditional);
            return conditional;

        }

        public Name ParseName() {
            ParseException.AssertType(GetCurrentToken(), TokenType.Name);
            var name = new Name(ConsumeCurrentToken().TokenText, false);
            Program.Log("Parser", "Parsed " + name);
            return name;
        }

        public NewAssignment ParseNewAssignment() {
            Program.LogIn("Parser", "Parsing new assignment...");
            ParseException.AssertType(GetCurrentToken(), TokenType.Var, TokenType.Val);
            var isWritable = ConsumeCurrentToken().Type == TokenType.Var;
            var name = ParseBindingDeclaration();
            ParseException.AssertType(ConsumeCurrentToken(), TokenType.Assignment);
            var value = (IExpression)Parse();

            var assignment = new NewAssignment(name, value, isWritable);
            Program.LogOut("Parser", "Parsed " + assignment);
            return assignment;
        }

        public Record ParseRecord() {
            Program.LogIn("Parser", "Parsing record type...");
            ParseException.AssertType(ConsumeCurrentToken(), TokenType.Record);
            ParseException.AssertType(GetCurrentToken(), TokenType.Name);
            var recordName = ConsumeCurrentToken().TokenText;

            var typeParams = new List<TypeName>();

            while (GetCurrentToken().Type == TokenType.Name) {
                typeParams.Add(new TypeName(ConsumeCurrentToken().TokenText));
            }

            ParseException.AssertType(ConsumeCurrentToken(), TokenType.Assignment);
            var bindings = ParseBindingDeclarationList();

            var record = new Record(recordName, typeParams, bindings);
            Program.LogOut("Parser", "Parsed " + record);
            return record;
        }

        public Return ParseReturn() {
            ParseException.AssertType(GetCurrentToken(), TokenType.Return);
            ConsumeCurrentToken();

            var @return = new Return((IExpression)Parse());
            Program.Log("Parser", "Parsed " + @return); 
            return @return;
        }

        public String ParseString() {
            ParseException.AssertType(GetCurrentToken(), TokenType.String);
            var @string = new String(string.Join("", ConsumeCurrentToken().TokenText.Skip(1).Reverse().Skip(1).Reverse()));
            Program.Log("Parser", "Parsed " + @string);
            return @string;
        }

        public Type ParseType() {
            Program.LogIn("Parser", "Parsing type...");
            Type t;
            if (GetCurrentToken().Type == TokenType.OpenSquareBracket) {
                ConsumeCurrentToken();
                var arrType = ParseType();
                ParseException.AssertType(ConsumeCurrentToken(), TokenType.CloseSquareBracket);
                t = new Type(arrType.Name, arrType.TypeParameters, arrType.Constraints, isRuntimeCheck: arrType.IsRuntimeCheck,
                    isArrayType: true);
            }
            else {
                ParseException.AssertType(GetCurrentToken(), TokenType.Name);
                t = new Type(ParseTypeName());
            }

            var typeParams = new List<Type>();
            while (GetCurrentToken().Type == TokenType.Name) {
                typeParams.Add(ParseType());
                if (GetCurrentToken().Type == TokenType.Comma) {
                    ConsumeCurrentToken();
                }
            }
            t = new Type(t.Name, typeParams, t.Constraints, t.IsRuntimeCheck, t.IsArrayType, t.Flag);

            if (GetCurrentToken().Equals(new Token(TokenType.Symbol, "<"))) {
                var constraints = ParseTypeConstraintList();
                t = new Type(t.Name, t.TypeParameters, constraints, isRuntimeCheck: t.IsRuntimeCheck, isArrayType: t.IsArrayType, flag: t.Flag);
            }

            if (GetCurrentToken().Type == TokenType.RuntimeCheck) {
                ConsumeCurrentToken();
                t = new Type(t.Name, t.TypeParameters, t.Constraints, isRuntimeCheck: true, isArrayType: t.IsArrayType, flag: t.Flag);
            }

            if (GetCurrentToken().Equals(new Token(TokenType.Symbol, "#"))) {
                ConsumeCurrentToken();
                ParseException.AssertType(GetCurrentToken(), TokenType.Name);
                var flag = ConsumeCurrentToken().TokenText;
                t = new Type(t.Name, t.TypeParameters, t.Constraints, t.IsRuntimeCheck, t.IsArrayType, flag);
            }
            Program.LogOut("Parser", "Parsed " + t);
            return t;
        }

        private IEnumerable<IEnumerable<Constraint>> ParseTypeConstraintList() {
            Program.LogIn("Parser", "Parsing type constraint list...");
            ParseException.Assert(ConsumeCurrentToken(), new Token(TokenType.Symbol, "<"));
            var returns = new List<IEnumerable<Constraint>>();
            var constraints = ParseInnerConstraintList();
            returns.Add(constraints);
            while (GetCurrentToken().Equals(new Token(TokenType.Symbol, "|"))) {
                ConsumeCurrentToken();
                constraints = ParseInnerConstraintList();
                returns.Add(constraints);
            }
            ParseException.Assert(ConsumeCurrentToken(), new Token(TokenType.Symbol, ">"));
            Program.LogOut("Parser", "Parsed " + string.Join(", ", returns));
            return returns;
        }

        private IEnumerable<Constraint> ParseInnerConstraintList() {
            Program.LogIn("Parser", "Parsing inner constraint list...");
            var constraints = new List<Constraint>();
            while (!GetCurrentToken().Equals(new Token(TokenType.Symbol, ">")) && !(GetCurrentToken().Equals(new Token(TokenType.Symbol, "|")))) {
                constraints.Add(ParseTypeConstraint());
            }
            Program.LogOut("Parser", "Parsed " + string.Join(", ", constraints));
            return constraints;
        }

        public TypeName ParseTypeName() {
            ParseException.AssertType(GetCurrentToken(), TokenType.Name);
            var typename = new TypeName(ConsumeCurrentToken().TokenText);
            Program.Log("Parser", "Parsed " + typename);
            return typename;
        }

        public UnaryOp ParseUnaryOp() {
            ParseException.AssertType(GetCurrentToken(), TokenType.Symbol);
            var op = ConsumeCurrentToken().TokenText;
            var operand = (IExpression)Parse();
            var unaryop = new UnaryOp(op, operand);
            Program.Log("Parser", "Parsed " + unaryop);
            return unaryop;
        }

        public While ParseWhile() {
            Program.LogIn("Parser", "Parsing while...");
            ParseException.AssertType(ConsumeCurrentToken(), TokenType.While);
            ParseException.AssertType(ConsumeCurrentToken(), TokenType.OpenBracket);
            var condition = Parse();
            ParseException.AssertType(ConsumeCurrentToken(), TokenType.CloseBracket);
            var concequence = Parse();

            var @while = new While((IExpression)condition, (IStatement)concequence);
            Program.LogOut("Parser", "Parsed " + @while);
            return @while;

        }

        public AST_Nodes.Block ParseBlock() {
            Program.LogIn("Parser", "Parsing block...");
            ParseException.AssertType(ConsumeCurrentToken(), TokenType.OpenBrace);
            var statements = new List<IStatement>();
            while (GetCurrentToken().Type != TokenType.CloseBrace) {
                statements.Add((IStatement)Parse());
                ParseException.AssertType(ConsumeCurrentToken(), TokenType.LineSeperator);
            }

            ParseException.AssertType(ConsumeCurrentToken(), TokenType.CloseBrace);
            var block = new Block(statements);
            Program.LogOut("Parser", "Parsed " + block);
            return block;
        }

        public FunctionSignature ParseFunctionSignature() {
            Program.LogIn("Parser", "Parsing function signature...");
            ParseException.AssertType(GetCurrentToken(), TokenType.Name);
            var funcName = ConsumeCurrentToken().TokenText;
            var parameters = ParseBindingDeclarationList();

            Type type;
            if (GetCurrentToken().Type == TokenType.Colon) {
                ConsumeCurrentToken();
                type = ParseType();
            }
            else {
                type = new Type(new TypeName("Void"));
            }

            var signature = new FunctionSignature(funcName, parameters, type);
            Program.LogOut("Parser", "Parsed " + signature);
            return signature;
        }

        public AST_Nodes.Program ParseProgram() {
            Program.LogIn("Parser", "Parsing program...");
            var nodes = new List<INode>();
            while (!Eof()) {
                if (GetCurrentToken().Type == TokenType.LineSeperator) {
                    ConsumeCurrentToken();
                    continue;
                }
                nodes.Add(Parse());
            }
            var program = new AST_Nodes.Program(nodes);
            Program.LogOut("Parser", "Parsed " + program);
            return program;
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
            return new ParseException(string.Format("{0} ({2}) unexpected at this time. Expected: {1}", unexpected, string.Join(" or ", expected.ToList()), unexpected.Position));
        }
        public static ParseException UnexpectedToken(Token unexpected, params Token[] expected) {
            return new ParseException(string.Format("{0} ({2}) unexpected at this time. Expected: {1}", unexpected, string.Join(" or ", expected.ToList()), unexpected.Position));
        }

        public ParseException(string message) : base(message) {}
    }
}
