using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Speedycloud.Compiler.Lexer;

namespace Speedycloud.Compiler.Parser {
    public static class ParserExtensions {
        public delegate List<ParseResult<T>> Parser<T>(IEnumerable<Token> tokens);

        public delegate Parser<T> ParserMap<T, K>(K token);

        public static Parser<T> Result<T>(T result) {
            return tokens => new List<ParseResult<T>>{new ParseResult<T>(result, tokens)};
        }

        public static Parser<T> Fail<T>() {
            return t => new List<ParseResult<T>>();
        }

        public static Parser<Token> Shift() {
            return t => {
                var tokens = t.ToList();
                return new List<ParseResult<Token>>{new ParseResult<Token>(tokens.First(), tokens.Skip(1))};
            };
        }

        public static Parser<T> Bind<T>(this Parser<T> self, ParserMap<T, T> map) {
            return tokens => {
                var t = tokens.ToList();
                var parse = self(t);
                var result = new List<ParseResult<T>>();
                parse.ForEach(p => map(p.Value)(p.RemainingTokens).ForEach(result.Add));
                return result;
            };
        }

        public static Parser<T> Match<T>(this Parser<T> super, Predicate<T> f) {
            return super.Bind(tokens => {
                if (f(tokens)) {
                    return Result(tokens);
                }
                return Fail<T>();
            });
        }

        public static Parser<K> Apply<T, K>(this Parser<T> self, Func<T, K> applicator) {
            return matches => {
                var result = self(matches);
                return result.Select(match => new ParseResult<K>(applicator(match.Value), match.RemainingTokens)).ToList();
            };
        }

        public static Parser<T> Or<T>(this Parser<T> self, Parser<T> other) {
            return tokens => {
                var t = tokens.ToList();
                var result = self(t);
                if (result.Any()) {
                    return result;
                }
                return other(t);
            };
        }

        public static Parser<T> And<T>(this Parser<T> self, Parser<T> next) {
            return tokens => {
                var t = tokens.ToList();
                var result = self(t);
                if (!result.Any()) {
                    return result;
                }
                var nextResults = result.SelectMany(r => next(r.RemainingTokens)).ToList();
                return result.Concat(nextResults).ToList();
            };
        }

        public static Parser<T> Surrounded<T, K, V>(this Parser<T> self, Parser<K> left, Parser<V> right) {
            return tokens => {
                var l = left(tokens);
                var middle = l.SelectMany(match => self(match.RemainingTokens));
                if (!middle.Any(match => right(match.RemainingTokens).Any())) {
                    return Fail<T>()(tokens);
                }
                return middle.ToList();
            };
        }

        public static Parser<T> Many<T>(this Parser<T> self) {
            return tokens => {
                var results = new List<ParseResult<T>>();
                var initialMatch = self(tokens);
                results.AddRange(initialMatch);
                while (initialMatch.Any()) {
                    initialMatch = initialMatch.SelectMany(match => self(match.RemainingTokens)).ToList();
                    results.AddRange(initialMatch);
                }
                return results;
            };
        }

        public static Parser<T> Interspersed<T, K>(this Parser<T> self, Parser<K> interspersed) {
            return tokens => {
                var results = new List<ParseResult<T>>();
                var t = tokens.ToList();
                var initialMatch = self(t);
                results.AddRange(initialMatch);
                var remainingTokens = initialMatch.Select(m => m.RemainingTokens);
                while (true) {
                    var trailers = remainingTokens.SelectMany(match => self(match));
                    results.AddRange(trailers);

                    var newMatches = remainingTokens.SelectMany(match => interspersed(match));
                    remainingTokens = newMatches.Select(m => m.RemainingTokens);

                    if (!newMatches.Any()) {
                        break;
                    }
                }
                return results;
            };
        }

        public static Parser<K> Slurp<T, K>(this Parser<T> self, Func<IEnumerable<T>, K> slurper) {
            return tokens => {
                var results = self(tokens).ToList();
                return new List<ParseResult<K>> {
                    new ParseResult<K>(slurper(results.Select(r => r.Value)),
                        results.OrderByDescending(r => r.RemainingTokens.Count()).First().RemainingTokens)
                };
            };
        }
    }

    public class ParseResult<T> {
        public T Value { get; private set; }
        public IEnumerable<Token> RemainingTokens { get; private set; }

        public ParseResult(T value, IEnumerable<Token> remainingTokens) {
            Value = value;
            RemainingTokens = remainingTokens;
        }
    }
}
