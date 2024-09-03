using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maths_Software_with_Interpreter
{
    class Parse
    {
        // BNF rules
        // <expr>	    ::= <term> <expr'>
        // <expr'>	    ::= + <term> <expr'> | - <term> <expr'> | = <term> <expr'> | <empty>
        // <term>       ::= <factor> <term'>
        // <term'>	    ::= * <factor> <term'> | / <factor> <term'> | % <factor> <term> | ^ <factor> <term'> | <empty>
        // <factor>     ::= <int> | <decimal> | <var> | <func>(<expr>) | (<expr>) | + <factor> | - <factor>

        private static int lookAhead;
        private static int currentToken;
        private static int unaryCount;

        private static bool Match(int token)
        {
            bool result;
            // Initialize the lookAhead to the first token
            if (lookAhead == -1) lookAhead = Globals.tokens[currentToken];
            // result = true if the token value of the lookahead equates to the token ID
            result = token == lookAhead;
            if (result) Console.WriteLine("Token[" + currentToken + "] = " + token + " matched, token ID name is " + Globals.GetTokName(token));
            else Console.WriteLine("Token[" + currentToken + "] = " + token + " NOT matched, token ID name is " + Globals.GetTokName(token));
            return result;
        }
        // Increments lookahead
        private static void Advance(int level)
        {
            lookAhead = Globals.tokens[++currentToken];
            Console.WriteLine("Advance() called at level " + level + " with next token " + lookAhead);
        }

        // By default, parser success is true
        public static void Parser()
        {
            // Index of current token in Globals.tokens[]
            currentToken = 0;
            // Lookahead set to non-existing token index
            lookAhead = -1;
            // Consecutive unary operator count set to 0
            unaryCount = 0;
            Expression(0);
            if (currentToken < Globals.numTokens)
            {
                // If a token hasn't been matched, exception is thrown
                Console.Error.WriteLine("SYNTAX ERROR - Token " + currentToken + " is of type " + Globals.GetTokName(Globals.tokens[currentToken]));
                throw new InvalidSyntaxException("SYNTAX ERROR - Token " + currentToken + " is of type " + Globals.GetTokName(Globals.tokens[currentToken]));
            }
        }

        private static void Expression(int level)
        {
            Console.WriteLine("Expression() called at level " + level);
            // <expr>	::=	<term> <expr'>
            Term(level + 1);
            Expression_Prm(level + 1);
        }

        private static void Term(int level)
        {
            Console.WriteLine("Term() called at level " + level);
            // <term>	::= <factor> <term'>
            Factor(level + 1);
            Term_Prm(level + 1);
        }

        private static void Expression_Prm(int level)
        {
            Console.WriteLine("Expression_Prm() called at level " + level);
            // <expr'>	    ::= + <term> <expr'> | - <term> <expr'> | = <term> <expr'> | <empty>
            if (Match(Globals.TOK_PLUS) | Match(Globals.TOK_SUB) | Match(Globals.TOK_EQUAL))
            {
                Advance(level + 1);
                Term(level + 1);
                Expression_Prm(level + 1);
            }
        }

        private static void Term_Prm(int level)
        {
            Console.WriteLine("Term_Prm() called at level " + level);
            // <term'>	    ::= * <factor> <term'> | / <factor> <term'> | % <factor> <term> | ^ <factor> <term'> | <empty>
            if (Match(Globals.TOK_TIMES) | Match(Globals.TOK_DIV) | Match(Globals.TOK_MOD) | Match(Globals.TOK_POW))
            {
                Advance(level + 1);
                Factor(level + 1);
                Term_Prm(level + 1);
            }
        }

        private static void Factor(int level)
        {
            Console.WriteLine("Factor() called at level " + level);
            // <factor>     ::= <int> | <decimal> | <var> | <func>(<expr>) | (<expr>) | + <factor> | - <factor>
            if (Match(Globals.TOK_INT) | Match(Globals.TOK_VAR)) Advance(level + 1);
            else if (Match(Globals.TOK_DEC))
            {
                Advance(level + 1);
            }
            else if (Match(Globals.TOK_FUNC))
            {
                Advance(level + 1);
                Expression(level + 1);
            }
            else if (Match(Globals.TOK_LPAR))
            {
                Advance(level + 1);
                Expression(level + 1);
                // Check if parentheses are matched correctly
                if (Match(Globals.TOK_RPAR)) Advance(level + 1);
                else
                {
                    Console.Error.WriteLine("SYNTAX ERROR: Mismatched parentheses at token " + currentToken + ", value = " + Globals.GetTokName(Globals.tokens[currentToken]));
                    throw new InvalidSyntaxException("SYNTAX ERROR: Mismatched parentheses at token " + currentToken + ", value = " + Globals.GetTokName(Globals.tokens[currentToken]));
                }
            }
            else if (Match(Globals.TOK_PLUS) | Match(Globals.TOK_SUB))
            {
                // Check if the unary operator is used syntactically correctly
                if (unaryCount == 0)
                {
                    unaryCount++;
                    Advance(level + 1);
                    Factor(level + 1);
                }
                else
                {
                    Console.Error.WriteLine("SYNTAX ERROR: More than one unary operator used at token " + currentToken + ", value = " + Globals.GetTokName(Globals.tokens[currentToken]));
                    throw new InvalidUnaryException("SYNTAX ERROR: More than one unary operator used at token " + currentToken + ", value = " + Globals.GetTokName(Globals.tokens[currentToken]));
                }
            }
            // If operator symbol is not followed by a number, exception is thrown
            else
            {
                Console.Error.WriteLine("SYNTAX ERROR: Number expected after token " + (currentToken - 1) + ", value = " + Globals.GetTokName(Globals.tokens[currentToken - 1]));
                throw new InvalidSyntaxException("SYNTAX ERROR: Number expected after token " + (currentToken - 1) + ", value = " + Globals.GetTokName(Globals.tokens[currentToken - 1]));
            }

            if (unaryCount == 1)
            {
                unaryCount = 0;
            }
        }
    }
}
