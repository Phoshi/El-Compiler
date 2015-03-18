using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Speedycloud.Compiler.Lexer {
    public class Token {
        public TokenType Type { get; private set; }
        public string TokenText { get; private set; }
        public InputPosition Position { get; private set; }

        protected bool Equals(Token other) {
            return Type == other.Type && string.Equals(TokenText, other.TokenText);
        }

        public override string ToString() {
            return string.Format("({0} {1})", Type, TokenText);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Token) obj);
        }

        public override int GetHashCode() {
            unchecked {
                return ((int) Type*397) ^ (TokenText != null ? TokenText.GetHashCode() : 0);
            }
        }

        public Token(TokenType type, string tokenText, InputPosition pos = default(InputPosition)) {
            Type = type;
            TokenText = tokenText;
            Position = pos;
        }

        public Token AtPosition(InputPosition pos) {
            return new Token(Type, TokenText, pos);
        }
    }

    public enum TokenType {
        Number,
        String,
        Name,
        OpenBrace,
        CloseBrace,
        OpenBracket,
        CloseBracket,
        OpenSquareBracket,
        CloseSquareBracket,
        Symbol,
        LineSeperator,
        True,
        False,
        If,
        Else,
        For,
        While,
        Var,
        Val,
        Def,
        Return,
        Record,
        Instance,
        Typeclass,
        Comma,
        Colon,
        Failure,
        Assignment,
        In,
        RuntimeCheck,

        Eof
    }
}
