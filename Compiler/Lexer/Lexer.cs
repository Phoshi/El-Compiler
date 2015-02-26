﻿using System;
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

            {"+", new Token(TokenType.Symbol, "+")},
            {"-", new Token(TokenType.Symbol, "-")},
            {"/", new Token(TokenType.Symbol, "/")},
            {"*", new Token(TokenType.Symbol, "*")},
            {"%", new Token(TokenType.Symbol, "%")},
            {"==", new Token(TokenType.Symbol, "==")},
            {"!=", new Token(TokenType.Symbol, "!=")},
            {">", new Token(TokenType.Symbol, ">")},
            {"<", new Token(TokenType.Symbol, "<")},
            {"<=", new Token(TokenType.Symbol, "<=")},
            {">=", new Token(TokenType.Symbol, ">=")},
            {"&&", new Token(TokenType.Symbol, "&&")},
            {"||", new Token(TokenType.Symbol, "||")},

            {"true", new Token(TokenType.True, "true")},
            {"false", new Token(TokenType.False, "false")},

            {"def", new Token(TokenType.Def, "def")},
            {"for", new Token(TokenType.For, "for")},
            {"in", new Token(TokenType.In, "in")},
            {"if", new Token(TokenType.If, "if")},
            {"else", new Token(TokenType.Else, "else")},
            {"while", new Token(TokenType.While, "while")},
            {"return", new Token(TokenType.Return, "return")},

            {"val", new Token(TokenType.Val, "val")},
            {"var", new Token(TokenType.Var, "var")},

            {"?", new Token(TokenType.RuntimeCheck, "?")},

            {"record", new Token(TokenType.Record, "record")},
            {",", new Token(TokenType.Comma, ",")},

            {":", new Token(TokenType.Colon, ":")},
        };

        private LexModes mode = LexModes.Normal;
        public List<Token> Lex(string input) {
            var tokens = new List<Token>();
            var accumulator = "";
            foreach (var character in input) {
                if (mode == LexModes.Normal) {
                    if (char.IsWhiteSpace(character)) {
                        if (accumulator != "") {
                            tokens.Add(Tokenise(accumulator));
                            accumulator = "";
                        }
                        continue;
                    }
                    if (accumulator != "" && accumulator.All(char.IsLetterOrDigit) != char.IsLetterOrDigit(character)) {
                        if (!(accumulator.All(c => char.IsDigit(c) || c == '.') && (character == '.' || char.IsDigit(character)))) {
                            tokens.Add(Tokenise(accumulator));
                            accumulator = "";    
                        }
                    }

                    accumulator += character;

                    if (consts.ContainsKey(accumulator) &&
                        !consts.Keys.Any(key => key.StartsWith(accumulator) && key != accumulator)) {
                        tokens.Add(Tokenise(accumulator));
                        accumulator = "";
                    }
                    else if (consts.ContainsKey(accumulator.Substring(0, accumulator.Length - 1))){
                        tokens.Add(Tokenise(accumulator.Substring(0, accumulator.Length - 1)));
                        accumulator = accumulator.Last().ToString();
                    }

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
