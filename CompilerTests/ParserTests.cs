using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Speedycloud.Compiler.AST_Nodes;
using Speedycloud.Compiler.Lexer;
using Speedycloud.Compiler.Parser;
using Array = Speedycloud.Compiler.AST_Nodes.Array;
using Boolean = Speedycloud.Compiler.AST_Nodes.Boolean;
using String = Speedycloud.Compiler.AST_Nodes.String;
using Type = Speedycloud.Compiler.AST_Nodes.Type;

namespace CompilerTests {
    [TestClass]
    public class ParserTests {
        [TestMethod]
        public void ArrayLiteral() {
            var tokens = new List<Token> {
                new Token(TokenType.OpenSquareBracket, "["),
                new Token(TokenType.Number, "5"),
                new Token(TokenType.Comma, ","),
                new Token(TokenType.Number, "15"),
                new Token(TokenType.CloseSquareBracket, "]")
            };

            var parser = new Parser(tokens);
            var tree = parser.Parse();
            Assert.AreEqual(new Array(new List<IExpression> {
                new Integer(5), new Integer(15)
            }), tree);
        }

        [TestMethod]
        public void ArrayAssignment() {
            var tokens = new List<Token> {
                new Token(TokenType.Name, "foo"),
                new Token(TokenType.OpenSquareBracket, "["),
                new Token(TokenType.Name, "bar"),
                new Token(TokenType.CloseSquareBracket, "]"),
                new Token(TokenType.Assignment, "="),
                new Token(TokenType.Name, "baz")
            };

            var parser = new Parser(tokens);
            var tree = parser.Parse();
            Assert.AreEqual(
                new ArrayAssignment(new Name("foo", false), new Name("bar", false), new Name("baz", false)), tree);
        }

        [TestMethod]
        public void ArrayIndex() {
            var tokens = new List<Token> {
                new Token(TokenType.Name, "foo"),
                new Token(TokenType.OpenSquareBracket, "["),
                new Token(TokenType.Name, "bar"),
                new Token(TokenType.CloseSquareBracket, "]")
            };

            var parser = new Parser(tokens);
            var tree = parser.Parse();
            Assert.AreEqual(
                new ArrayIndex(new Name("foo", false), new Name("bar", false)), tree);
        }

        [TestMethod]
        public void Assignment() {
            var tokens = new List<Token> {
                new Token(TokenType.Name, "foo"),
                new Token(TokenType.Assignment, "="),
                new Token(TokenType.Name, "bar")
            };

            var parser = new Parser(tokens);
            var tree = parser.Parse();
            Assert.AreEqual(
                new Assignment(new Name("foo", true), new Name("bar", false)), tree);
        }

        [TestMethod]
        public void BinaryOp() {
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
                var tokens = new List<Token> {
                new Token(TokenType.Name, "foo"),
                new Token(TokenType.Operator, op),
                new Token(TokenType.Name, "bar")
            };

                var parser = new Parser(tokens);
                var tree = parser.Parse();
                Assert.AreEqual(
                    new BinaryOp(op, new Name("foo", false), new Name("bar", false)), tree);
            }

        }

        [TestMethod]
        public void BindingDeclaration() {
            var tokens = new List<Token> {
                new Token(TokenType.Name, "foo"),
                new Token(TokenType.Colon, ":"),
                new Token(TokenType.Name, "bar")
            };

            var parser = new Parser(tokens);
            var tree = parser.ParseBindingDeclaration();
            Assert.AreEqual(
                new BindingDeclaration(new Name("foo", true), new Type(new TypeName("bar"))), tree);
        }

        [TestMethod]
        public void BooleanTrue() {
            var tokens = new List<Token> {
                new Token(TokenType.True, "true")
            };

            var parser = new Parser(tokens);
            var tree = parser.Parse();
            Assert.AreEqual(
                new Boolean(true), tree);
        }

        [TestMethod]
        public void BooleanFalse() {
            var tokens = new List<Token> {
                new Token(TokenType.False, "false")
            };

            var parser = new Parser(tokens);
            var tree = parser.Parse();
            Assert.AreEqual(
                new Boolean(false), tree);
        }

        [TestMethod]
        public void Constraint() {
            var tokens = new List<Token> {
                new Token(TokenType.Name, "Eq"),
                new Token(TokenType.Number, "5")
            };

            var parser = new Parser(tokens);
            var tree = parser.ParseTypeConstraint();
            Assert.AreEqual(
                new Constraint("Eq", new Integer(5)), tree);
        }

        [TestMethod]
        public void Float() {
            var tokens = new List<Token> {
                new Token(TokenType.Number, "3.14")
            };

            var parser = new Parser(tokens);
            var tree = parser.Parse();
            Assert.AreEqual(
                new Float(3.14), tree);
        }

        [TestMethod]
        public void For() {
            var tokens = new List<Token> {
                new Token(TokenType.For, "for"),
                new Token(TokenType.OpenBracket, "("),
                new Token(TokenType.Name, "foo"),
                new Token(TokenType.Colon, ":"),
                new Token(TokenType.Name, "bar"),
                new Token(TokenType.In, "in"),
                new Token(TokenType.Name, "baz"),
                new Token(TokenType.CloseBracket, ")"),
                new Token(TokenType.Name, "bang"),
                new Token(TokenType.OpenBracket, "("),
                new Token(TokenType.Name, "foo"),
                new Token(TokenType.CloseBracket, ")")
            };

            var parser = new Parser(tokens);
            var tree = parser.Parse();
            Assert.AreEqual(
                new For(new BindingDeclaration(new Name("foo", true), new Type(new TypeName("bar"))),
                    new Name("baz", false), new FunctionCall("bang", new List<IExpression> {new Name("foo", false)})),
                tree);
        }

        [TestMethod]
        public void FunctionCall() {
            var tokens = new List<Token> {
                new Token(TokenType.Name, "foo"),
                new Token(TokenType.OpenBracket, "("),
                new Token(TokenType.Number, "2"),
                new Token(TokenType.CloseBracket, ")"),
            };

            var parser = new Parser(tokens);
            var tree = parser.Parse();
            Assert.AreEqual(
                new FunctionCall("foo", new List<IExpression>{new Integer(2)}), tree);
        }

        [TestMethod]
        public void FunctionDefinition() {
            var tokens = new List<Token> {
                new Token(TokenType.Def, "def"),
                new Token(TokenType.Name, "add"),
                new Token(TokenType.OpenBracket, "("),
                new Token(TokenType.Name, "x"),
                new Token(TokenType.Colon, ":"),
                new Token(TokenType.Name, "Integer"),
                new Token(TokenType.Comma, ","),
                new Token(TokenType.Name, "y"),
                new Token(TokenType.Colon, ":"),
                new Token(TokenType.Name, "Integer"),
                new Token(TokenType.CloseBracket, ")"),
                new Token(TokenType.Colon, ":"),
                new Token(TokenType.Name, "Integer"),
                new Token(TokenType.Return, "return"),
                new Token(TokenType.Name, "x"),
                new Token(TokenType.Operator, "+"),
                new Token(TokenType.Name, "y")
            };

            var parser = new Parser(tokens);
            var tree = parser.Parse();
            Assert.AreEqual(
                new FunctionDefinition(
                    new FunctionSignature("add",
                        new List<BindingDeclaration> {
                            new BindingDeclaration(new Name("x", true), new Type(new TypeName("Integer"))),
                            new BindingDeclaration(new Name("y", true), new Type(new TypeName("Integer")))
                        },
                        new Type(new TypeName("Integer"))),
                    new Return(new BinaryOp("+", new Name("x", false), new Name("y", false)))), tree);
        }


        [TestMethod]
        public void FunctionSignature() {
            var tokens = new List<Token> {
                new Token(TokenType.Name, "add"),
                new Token(TokenType.OpenBracket, "("),
                new Token(TokenType.Name, "x"),
                new Token(TokenType.Colon, ":"),
                new Token(TokenType.Name, "Integer"),
                new Token(TokenType.Comma, ","),
                new Token(TokenType.Name, "y"),
                new Token(TokenType.Colon, ":"),
                new Token(TokenType.Name, "Integer"),
                new Token(TokenType.CloseBracket, ")"),
                new Token(TokenType.Colon, ":"),
                new Token(TokenType.Name, "Integer"),
            };

            var parser = new Parser(tokens);
            var tree = parser.ParseFunctionSignature();

            Assert.AreEqual(new FunctionSignature("add",
                new List<BindingDeclaration> {
                    new BindingDeclaration(new Name("x", true), new Type(new TypeName("Integer"))),
                    new BindingDeclaration(new Name("y", true), new Type(new TypeName("Integer")))
                },
                new Type(new TypeName("Integer"))), tree);
        }

        [TestMethod]
        public void If() {
            var tokens = new List<Token> {
                new Token(TokenType.If, "if"),
                new Token(TokenType.OpenBracket, "("),
                new Token(TokenType.True, "true"),
                new Token(TokenType.CloseBracket, ")"),
                new Token(TokenType.Name, "foo"),
                new Token(TokenType.OpenBracket, "("),
                new Token(TokenType.Number, "5"),
                new Token(TokenType.CloseBracket, ")"),
                new Token(TokenType.Else, "else"),
                new Token(TokenType.Name, "foo"),
                new Token(TokenType.OpenBracket, "("),
                new Token(TokenType.Number, "4"),
                new Token(TokenType.CloseBracket, ")"),
            };

            var parser = new Parser(tokens);
            var tree = parser.Parse();

            Assert.AreEqual(new If(new Boolean(true), new FunctionCall("foo", new List<IExpression> { new Integer(5) }), new FunctionCall("foo", new List<IExpression> { new Integer(4) })), tree);
        }

        [TestMethod]
        public void IntegerLiteral() {
            var tokens = new List<Token> {
                new Token(TokenType.Number, "5")
            };

            var parser = new Parser(tokens);
            var tree = parser.Parse();

            Assert.AreEqual(new Integer(5), tree);
        }

        [TestMethod]
        public void Name() {
            var tokens = new List<Token> {
                new Token(TokenType.Name, "foo")
            };

            var parser = new Parser(tokens);
            var tree = parser.Parse();

            Assert.AreEqual(new Name("foo", false), tree);
        }

        [TestMethod]
        public void NewAssignmentWritable() {
            var tokens = new List<Token> {
                new Token(TokenType.Var, "var"),
                new Token(TokenType.Name, "x"),
                new Token(TokenType.Colon, ":"),
                new Token(TokenType.Name, "Integer"),
                new Token(TokenType.Assignment, "="),
                new Token(TokenType.Number, "5")
            };

            var parser = new Parser(tokens);
            var tree = parser.Parse();

            Assert.AreEqual(
                new NewAssignment(new BindingDeclaration(new Name("x", true), new Type(new TypeName("Integer"))),
                    new Integer(5), true), tree);
        }

        [TestMethod]
        public void NewAssignmentReadonly() {
            var tokens = new List<Token> {
                new Token(TokenType.Val, "val"),
                new Token(TokenType.Name, "x"),
                new Token(TokenType.Colon, ":"),
                new Token(TokenType.Name, "Integer"),
                new Token(TokenType.Assignment, "="),
                new Token(TokenType.Number, "5")
            };

            var parser = new Parser(tokens);
            var tree = parser.Parse();

            Assert.AreEqual(
                new NewAssignment(new BindingDeclaration(new Name("x", true), new Type(new TypeName("Integer"))),
                    new Integer(5), false), tree);
        }

        [TestMethod]
        public void Block() {
            var tokens = new List<Token> {
                new Token(TokenType.OpenBrace, "{"),
                new Token(TokenType.Var, "var"),
                new Token(TokenType.Name, "x"),
                new Token(TokenType.Colon, ":"),
                new Token(TokenType.Name, "Integer"),
                new Token(TokenType.Assignment, "="),
                new Token(TokenType.Number, "5"),
                new Token(TokenType.LineSeperator, ";"),
                new Token(TokenType.Var, "var"),
                new Token(TokenType.Name, "y"),
                new Token(TokenType.Colon, ":"),
                new Token(TokenType.Name, "Integer"),
                new Token(TokenType.Assignment, "="),
                new Token(TokenType.Number, "4"),
                new Token(TokenType.LineSeperator, ";"),
                new Token(TokenType.CloseBrace, "}")
            };

            var parser = new Parser(tokens);
            var tree = parser.Parse();

            Assert.AreEqual(new Block(new List<IStatement> {
                new NewAssignment(new BindingDeclaration(new Name("x", true), new Type(new TypeName("Integer"))),
                    new Integer(5), true),
                new NewAssignment(new BindingDeclaration(new Name("y", true), new Type(new TypeName("Integer"))),
                    new Integer(4), true)
            }), tree);
        }

        [TestMethod]
        public void Program() {
            var tokens = new List<Token> {
                new Token(TokenType.Record, "record"),
                new Token(TokenType.Name, "Point"),
                new Token(TokenType.Assignment, "="),
                new Token(TokenType.OpenBracket, "("),
                new Token(TokenType.Name, "x"),
                new Token(TokenType.Colon, ":"),
                new Token(TokenType.Name, "Integer"),
                new Token(TokenType.Name, "y"),
                new Token(TokenType.Colon, ":"),
                new Token(TokenType.Name, "Integer"),
                new Token(TokenType.CloseBracket, ")"),
                new Token(TokenType.OpenBrace, "{"),
                new Token(TokenType.Var, "var"),
                new Token(TokenType.Name, "x"),
                new Token(TokenType.Colon, ":"),
                new Token(TokenType.Name, "Integer"),
                new Token(TokenType.Assignment, "="),
                new Token(TokenType.Number, "5"),
                new Token(TokenType.LineSeperator, ";"),
                new Token(TokenType.Var, "var"),
                new Token(TokenType.Name, "y"),
                new Token(TokenType.Colon, ":"),
                new Token(TokenType.Name, "Integer"),
                new Token(TokenType.Assignment, "="),
                new Token(TokenType.Number, "4"),
                new Token(TokenType.LineSeperator, ";"),
                new Token(TokenType.CloseBrace, "}")
            };

            var parser = new Parser(tokens);
            var tree = parser.ParseProgram();

            Assert.AreEqual(new Program(new List<INode> {
                new Record("Point", new List<TypeName> {}, new List<BindingDeclaration> {
                    new BindingDeclaration(new Name("x", true), new Type(new TypeName("Integer"))),
                    new BindingDeclaration(new Name("y", true), new Type(new TypeName("Integer"))),
                }),
                new Block(new List<IStatement> {
                    new NewAssignment(new BindingDeclaration(new Name("x", true), new Type(new TypeName("Integer"))),
                        new Integer(5), true),
                    new NewAssignment(new BindingDeclaration(new Name("y", true), new Type(new TypeName("Integer"))),
                        new Integer(4), true)
                })
            }), tree);
        }

        [TestMethod]
        public void Record() {
            var tokens = new List<Token> {
                new Token(TokenType.Record, "record"),
                new Token(TokenType.Name, "Point"),
                new Token(TokenType.Assignment, "="),
                new Token(TokenType.OpenBracket, "("),
                new Token(TokenType.Name, "x"),
                new Token(TokenType.Colon, ":"),
                new Token(TokenType.Name, "Integer"),
                new Token(TokenType.Name, "y"),
                new Token(TokenType.Colon, ":"),
                new Token(TokenType.Name, "Integer"),
                new Token(TokenType.CloseBracket, ")"),
            };

            var parser = new Parser(tokens);
            var tree = parser.Parse();

            Assert.AreEqual(new Record("Point", new List<TypeName>{}, new List<BindingDeclaration> {
                new BindingDeclaration(new Name("x", true), new Type(new TypeName("Integer"))),
                new BindingDeclaration(new Name("y", true), new Type(new TypeName("Integer"))),
            }), tree);
        }

        [TestMethod]
        public void Return() {
            var tokens = new List<Token> {
                new Token(TokenType.Return, "return"),
                new Token(TokenType.Number, "5")
            };

            var parser = new Parser(tokens);
            var tree = parser.Parse();

            Assert.AreEqual(new Return(new Integer(5)), tree);
        }

        [TestMethod]
        public void String() {
            var tokens = new List<Token> {
                new Token(TokenType.String, "\"Hello, world!\"")
            };

            var parser = new Parser(tokens);
            var tree = parser.Parse();

            Assert.AreEqual(new String("Hello, world!"), tree);
        }

        [TestMethod]
        public void RawType() {
            var tokens = new List<Token> {
                new Token(TokenType.Name, "Integer")
            };

            var parser = new Parser(tokens);
            var tree = parser.ParseType();

            Assert.AreEqual(new Type(new TypeName("Integer")), tree);
        }

        [TestMethod]
        public void ArrayType() {
            var tokens = new List<Token> {
                new Token(TokenType.OpenSquareBracket, "["),
                new Token(TokenType.Name, "Integer"),
                new Token(TokenType.CloseSquareBracket, "]")
            };

            var parser = new Parser(tokens);
            var tree = parser.ParseType();

            Assert.AreEqual(new Type(new TypeName("Integer"), isArrayType: true), tree);
        }

        [TestMethod]
        public void ConstrainedType() {
            var tokens = new List<Token> {
                new Token(TokenType.Name, "Integer"),
                new Token(TokenType.OpenAngleBracket, "<"),
                new Token(TokenType.Name, "Eq"),
                new Token(TokenType.Number, "5"),
                new Token(TokenType.CloseAngleBracket, ">")
            };

            var parser = new Parser(tokens);
            var tree = parser.ParseType();

            Assert.AreEqual(
                new Type(new TypeName("Integer"), new List<Constraint> {new Constraint("Eq", new Integer(5))}), tree);
        }

        [TestMethod]
        public void RuntimeType() {
            var tokens = new List<Token> {
                new Token(TokenType.Name, "Integer"),
                new Token(TokenType.RuntimeCheck, "?")
            };

            var parser = new Parser(tokens);
            var tree = parser.ParseType();

            Assert.AreEqual(new Type(new TypeName("Integer"), isRuntimeCheck: true), tree);
        }

        [TestMethod]
        public void TypeName() {
            var tokens = new List<Token> {
                new Token(TokenType.Name, "Integer")
            };

            var parser = new Parser(tokens);
            var tree = parser.ParseTypeName();

            Assert.AreEqual(new TypeName("Integer"), tree);
        }

        [TestMethod]
        public void UnaryOp() {
            var tokens = new List<Token> {
                new Token(TokenType.Operator, "-"),
                new Token(TokenType.Name, "foo")
            };

            var parser = new Parser(tokens);
            var tree = parser.Parse();

            Assert.AreEqual(new UnaryOp("-", new Name("foo", false)), tree);
        }

        [TestMethod]
        public void While() {
            var tokens = new List<Token> {
                new Token(TokenType.While, "while"),
                new Token(TokenType.OpenBracket, "("),
                new Token(TokenType.True, "true"),
                new Token(TokenType.CloseBracket, ")"),
                new Token(TokenType.Name, "foo"),
                new Token(TokenType.OpenBracket, "("),
                new Token(TokenType.Number, "5"),
                new Token(TokenType.CloseBracket, ")"),
            };

            var parser = new Parser(tokens);
            var tree = parser.Parse();

            Assert.AreEqual(new While(new Boolean(true), new FunctionCall("foo", new List<IExpression>{new Integer(5)})), tree);
        }
    }
}
