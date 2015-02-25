using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Speedycloud.Compiler.Lexer;

namespace CompilerTests {
    [TestClass]
    public class LexerTests {
        private string Str(IEnumerable<Token> expected, IEnumerable<Token> strs) {
            var expect = "[ " + string.Join(",", expected) + " ]";
            var actual = "[ " + string.Join(",", strs) + " ]";

            return string.Format("Expected: <{0}>\nActual: <{1}>", expect, actual);
        }

        [TestMethod]
        public void SingleDigit() {
            var lexer = new Lexer();
            var tokens = lexer.Lex("0");

            var expected = new Token[] {
                new Token(TokenType.Number, "0"), 
            };
            Assert.IsTrue(expected.SequenceEqual(tokens), Str(expected, tokens));
        }

        [TestMethod]
        public void ManyDigit() {
            var lexer = new Lexer();
            var tokens = lexer.Lex("12345");

            var expected = new Token[] {
                new Token(TokenType.Number, "12345"), 
            };
            Assert.IsTrue(expected.SequenceEqual(tokens), Str(expected, tokens));
        }

        [TestMethod]
        public void DecimalPoint() {
            var lexer = new Lexer();
            var tokens = lexer.Lex("123.456");

            var expected = new Token[] {
                new Token(TokenType.Number, "123.456"), 
            };
            Assert.IsTrue(expected.SequenceEqual(tokens), Str(expected, tokens));
        }

        [TestMethod]
        public void String() {
            var lexer = new Lexer();
            var tokens = lexer.Lex("\"hello, world!\"");

            var expected = new Token[] {
                new Token(TokenType.String, "\"hello, world!\""), 
            };
            Assert.IsTrue(expected.SequenceEqual(tokens), Str(expected, tokens));
        }

        [TestMethod]
        public void Name() {
            var lexer = new Lexer();
            var tokens = lexer.Lex("helloWorld");

            var expected = new Token[] {
                new Token(TokenType.Name, "helloWorld"), 
            };
            Assert.IsTrue(expected.SequenceEqual(tokens), Str(expected, tokens));
        }

        [TestMethod]
        public void Assignment() {
            var lexer = new Lexer();
            var tokens = lexer.Lex("=");

            var expected = new Token[] {
                new Token(TokenType.Assignment, "="), 
            };
            Assert.IsTrue(expected.SequenceEqual(tokens), Str(expected, tokens));
        }

        [TestMethod]
        public void Braces() {
            var lexer = new Lexer();
            var tokens = lexer.Lex("{}");

            var expected = new Token[] {
                new Token(TokenType.OpenBrace, "{"), 
                new Token(TokenType.CloseBrace, "}"), 
            };
            Assert.IsTrue(expected.SequenceEqual(tokens), Str(expected, tokens));
        }

        [TestMethod]
        public void Brackets() {
            var lexer = new Lexer();
            var tokens = lexer.Lex("()");

            var expected = new Token[] {
                new Token(TokenType.OpenBracket, "("), 
                new Token(TokenType.CloseBracket, ")"), 
            };
            Assert.IsTrue(expected.SequenceEqual(tokens), Str(expected, tokens));
        }

        [TestMethod]
        public void AngleBrackets() {
            var lexer = new Lexer();
            var tokens = lexer.Lex("<>");

            var expected = new Token[] {
                new Token(TokenType.OpenAngleBracket, "<"), 
                new Token(TokenType.CloseAngleBracket, ">"), 
            };
            Assert.IsTrue(expected.SequenceEqual(tokens), Str(expected, tokens));
        }

        [TestMethod]
        public void SquareBrackets() {
            var lexer = new Lexer();
            var tokens = lexer.Lex("[]");

            var expected = new Token[] {
                new Token(TokenType.OpenSquareBracket, "["), 
                new Token(TokenType.CloseSquareBracket, "]"), 
            };
            Assert.IsTrue(expected.SequenceEqual(tokens), Str(expected, tokens));
        }

        [TestMethod]
        public void Operator() {
            var ops = new List<string> {
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
            };
            foreach (var op in ops) {
                var lexer = new Lexer();
                var tokens = lexer.Lex(op);

                var expected = new Token[] {
                    new Token(TokenType.Operator, op),
                };
                Assert.IsTrue(expected.SequenceEqual(tokens), Str(expected, tokens));
            }
        }

        [TestMethod]
        public void LineSeperator() {
            var lexer = new Lexer();
            var tokens = lexer.Lex(";");

            var expected = new Token[] {
                new Token(TokenType.LineSeperator, ";"), 
            };
            Assert.IsTrue(expected.SequenceEqual(tokens), Str(expected, tokens));
        }

        [TestMethod]
        public void Bools() {
            var lexer = new Lexer();
            var tokens = lexer.Lex("true false");

            var expected = new Token[] {
                new Token(TokenType.True, "true"), 
                new Token(TokenType.False, "false"), 
            };
            Assert.IsTrue(expected.SequenceEqual(tokens), Str(expected, tokens));
        }

        [TestMethod]
        public void If() {
            var lexer = new Lexer();
            var tokens = lexer.Lex("if (true) { foo(bar); } else { baz(bang); }");

            var expected = new Token[] {
                new Token(TokenType.If, "if"), 
                new Token(TokenType.OpenBracket, "("), 
                new Token(TokenType.True, "true"), 
                new Token(TokenType.CloseBracket, ")"), 
                new Token(TokenType.OpenBrace, "{"), 
                new Token(TokenType.Name, "foo"), 
                new Token(TokenType.OpenBracket, "("), 
                new Token(TokenType.Name, "bar"), 
                new Token(TokenType.CloseBracket, ")"), 
                new Token(TokenType.LineSeperator, ";"), 
                new Token(TokenType.CloseBrace, "}"), 
                new Token(TokenType.Else, "else"), 
                new Token(TokenType.OpenBrace, "{"), 
                new Token(TokenType.Name, "baz"), 
                new Token(TokenType.OpenBracket, "("), 
                new Token(TokenType.Name, "bang"), 
                new Token(TokenType.CloseBracket, ")"), 
                new Token(TokenType.LineSeperator, ";"), 
                new Token(TokenType.CloseBrace, "}"), 
            };
            Assert.IsTrue(expected.SequenceEqual(tokens), Str(expected, tokens));
        }

        [TestMethod]
        public void For() {
            var lexer = new Lexer();
            var tokens = lexer.Lex("for (foo: Int in foos) {bar(foo);}");

            var expected = new Token[] {
                new Token(TokenType.For, "for"), 
                new Token(TokenType.OpenBracket, "("), 
                new Token(TokenType.Name, "foo"), 
                new Token(TokenType.Colon, ":"), 
                new Token(TokenType.Name, "Int"), 
                new Token(TokenType.In, "in"), 
                new Token(TokenType.Name, "foos"), 
                new Token(TokenType.CloseBracket, ")"), 
                new Token(TokenType.OpenBrace, "{"), 
                new Token(TokenType.Name, "bar"), 
                new Token(TokenType.OpenBracket, "("), 
                new Token(TokenType.Name, "foo"), 
                new Token(TokenType.CloseBracket, ")"), 
                new Token(TokenType.LineSeperator, ";"), 
                new Token(TokenType.CloseBrace, "}"), 
            };
            Assert.IsTrue(expected.SequenceEqual(tokens), Str(expected, tokens));
        }

        [TestMethod]
        public void While() {
            var lexer = new Lexer();
            var tokens = lexer.Lex("while (true) { foo(bar); }");

            var expected = new Token[] {
                new Token(TokenType.While, "while"), 
                new Token(TokenType.OpenBracket, "("), 
                new Token(TokenType.True, "true"), 
                new Token(TokenType.CloseBracket, ")"), 
                new Token(TokenType.OpenBrace, "{"), 
                new Token(TokenType.Name, "foo"), 
                new Token(TokenType.OpenBracket, "("), 
                new Token(TokenType.Name, "bar"), 
                new Token(TokenType.CloseBracket, ")"), 
                new Token(TokenType.LineSeperator, ";"), 
                new Token(TokenType.CloseBrace, "}"), 
            };
            Assert.IsTrue(expected.SequenceEqual(tokens), Str(expected, tokens));
        }

        [TestMethod]
        public void Var() {
            var lexer = new Lexer();
            var tokens = lexer.Lex("var foo = 3");

            var expected = new Token[] {
                new Token(TokenType.Var, "var"), 
                new Token(TokenType.Name, "foo"), 
                new Token(TokenType.Assignment, "="), 
                new Token(TokenType.Number, "3"), 
            };
            Assert.IsTrue(expected.SequenceEqual(tokens), Str(expected, tokens));
        }

        [TestMethod]
        public void Val() {
            var lexer = new Lexer();
            var tokens = lexer.Lex("val foo = 3");

            var expected = new Token[] {
                new Token(TokenType.Val, "val"), 
                new Token(TokenType.Name, "foo"), 
                new Token(TokenType.Assignment, "="), 
                new Token(TokenType.Number, "3"), 
            };
            Assert.IsTrue(expected.SequenceEqual(tokens), Str(expected, tokens));
        }

        [TestMethod]
        public void Def() {
            var lexer = new Lexer();
            var tokens = lexer.Lex("def foo(bar: Int): Int {return bar;}");

            var expected = new Token[] {
                new Token(TokenType.Def, "def"), 
                new Token(TokenType.Name, "foo"), 
                new Token(TokenType.OpenBracket, "("), 
                new Token(TokenType.Name, "bar"), 
                new Token(TokenType.Colon, ":"), 
                new Token(TokenType.Name, "Int"), 
                new Token(TokenType.CloseBracket, ")"), 
                new Token(TokenType.Colon, ":"), 
                new Token(TokenType.Name, "Int"), 
                new Token(TokenType.OpenBrace, "{"), 
                new Token(TokenType.Return, "return"), 
                new Token(TokenType.Name, "bar"), 
                new Token(TokenType.LineSeperator, ";"), 
                new Token(TokenType.CloseBrace, "}"), 

            };
            Assert.IsTrue(expected.SequenceEqual(tokens), Str(expected, tokens));
        }

        [TestMethod]
        public void Record() {
            var lexer = new Lexer();
            var tokens = lexer.Lex("record foo = (bar: Int, baz: Int)");

            var expected = new Token[] {
                new Token(TokenType.Record, "record"), 
                new Token(TokenType.Name, "foo"), 
                new Token(TokenType.Assignment, "="), 
                new Token(TokenType.OpenBracket, "("), 
                new Token(TokenType.Name, "bar"), 
                new Token(TokenType.Colon, ":"), 
                new Token(TokenType.Name, "Int"), 
                new Token(TokenType.Comma, ","), 
                new Token(TokenType.Name, "baz"), 
                new Token(TokenType.Colon, ":"), 
                new Token(TokenType.Name, "Int"), 
                new Token(TokenType.CloseBracket, ")"), 
            };
            Assert.IsTrue(expected.SequenceEqual(tokens), Str(expected, tokens));
        }

        [TestMethod]
        public void RuntimeType() {
            var lexer = new Lexer();
            var tokens = lexer.Lex("Int?");

            var expected = new Token[] {
                new Token(TokenType.Name, "Int"), 
                new Token(TokenType.RuntimeCheck, "?"), 
            };
            Assert.IsTrue(expected.SequenceEqual(tokens), Str(expected, tokens));
        }



    }
}
