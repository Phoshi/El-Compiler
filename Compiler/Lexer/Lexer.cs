using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speedycloud.Compiler.Lexer {
    public class Lexer {
        private readonly Dictionary<string, Token> consts = new Dictionary<string, Token> {
            {"(", new Token(TokenType.OpenBracket, "(")},
            {")", new Token(TokenType.CloseBracket, ")")},
            {"[", new Token(TokenType.OpenSquareBracket, "[")},
            {"]", new Token(TokenType.CloseSquareBracket, "]")},
            {"{", new Token(TokenType.OpenBrace, "{")},
            {"}", new Token(TokenType.CloseBrace, "}")},
            {"=", new Token(TokenType.Assignment, "=")},
            {";", new Token(TokenType.LineSeperator, ";")},

            {"+", new Token(TokenType.Operator, "+")},
            {"-", new Token(TokenType.Operator, "-")},
            {"/", new Token(TokenType.Operator, "/")},
            {"*", new Token(TokenType.Operator, "*")},
            {"%", new Token(TokenType.Operator, "%")},
            {"==", new Token(TokenType.Operator, "==")},
            {"!=", new Token(TokenType.Operator, "!=")},
            {">", new Token(TokenType.Operator, ">")},
            {"<", new Token(TokenType.Operator, "<")},
            {"<=", new Token(TokenType.Operator, "<=")},
            {">=", new Token(TokenType.Operator, ">=")},
            {"&&", new Token(TokenType.Operator, "&&")},
            {"||", new Token(TokenType.Operator, "||")},

            {"true", new Token(TokenType.True, "true")},
            {"false", new Token(TokenType.False, "false")},

            {"def", new Token(TokenType.Def, "def")},
            {"for", new Token(TokenType.For, "for")},
            {"while", new Token(TokenType.While, "while")},
        };

        /*var ops = new List<string> {
                "+",
                "-",
                "/",
                "*",
                "%",
                "==",
                "!=",
                ">",
                "<",
                "<=",
                "<=",
                "&&",
                "||"
            };*/

        private LexModes mode = LexModes.Normal;
        public List<Token> Lex(string input) {
            var tokens = new List<Token>();
            var accumulator = "";
            foreach (var character in input) {
                if (mode == LexModes.Normal) {
                    if (character == ' ') {
                        Tokenise(accumulator);
                        accumulator = "";
                        continue;
                    }
                    accumulator += character;

                    if (character == '"') {
                        mode = LexModes.String;
                        continue;
                    }
                }
                else if (mode == LexModes.String) {
                    accumulator += character;
                    if (character == '"') {
                        mode = LexModes.Normal;
                        tokens.Add(new Token(TokenType.String, accumulator));
                        accumulator = "";
                    }
                }
            }

            if (accumulator != "") {
                tokens.Add(Tokenise(accumulator));
            }

            return tokens;
        }

        public Token Tokenise(string str) {
            if (consts.ContainsKey(str)) {
                return consts[str];
            }
            if (str.All(c => char.IsDigit(c) || c == '.')) {
                return new Token(TokenType.Number, str);
            }
            return new Token(TokenType.Name, str);
        }
    }

    enum LexModes { Normal, String }
}
