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

        public Token(TokenType type, string tokenText) {
            Type = type;
            TokenText = tokenText;
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
        OpenAngleBracket,
        CloseAngleBracket,
        OpenSquareBracket,
        CloseSquareBracket,
        Operator,
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
