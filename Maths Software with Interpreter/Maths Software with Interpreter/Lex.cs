using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maths_Software_with_Interpreter
{
    class Lex
    {
        // Input lexeme
        private static string lexeme;
        // Iterator variable
        private static int i;

        // Lexes words with one or more characters into a single token
        private static string getWord()
        {
            StringBuilder sb = new StringBuilder(Globals.MAX_LEN);
            while (char.IsLetterOrDigit(lexeme[i]) || lexeme[i] == '_')
            {
                sb.Append(lexeme[i]);
                i++;
                if (i == lexeme.Length) break;
            }
            i--;
            return sb.ToString();

        }
        public static int Lexer(string input)
        {
            lexeme = input;
            // Token ID
            int tok_i = 0;
            int decimalPointCount = 0;

            for (i = 0; i < lexeme.Length; i++)
            {
                switch (lexeme[i])
                {
                    case ' ':
                        break;
                    case '+':
                        Globals.tokens[tok_i++] = Globals.TOK_PLUS;
                        break;
                    case '-':
                        Globals.tokens[tok_i++] = Globals.TOK_SUB;
                        break;
                    case '*':
                        Globals.tokens[tok_i++] = Globals.TOK_TIMES;
                        break;
                    case '/':
                        Globals.tokens[tok_i++] = Globals.TOK_DIV;
                        break;
                    case '%':
                        Globals.tokens[tok_i++] = Globals.TOK_MOD;
                        break;
                    case '^':
                        Globals.tokens[tok_i++] = Globals.TOK_POW;
                        break;
                    case '(':
                        Globals.tokens[tok_i++] = Globals.TOK_LPAR;
                        break;
                    case ')':
                        Globals.tokens[tok_i++] = Globals.TOK_RPAR;
                        break;
                    case '=':
                        Globals.tokens[tok_i++] = Globals.TOK_EQUAL;
                        break;
                    default:
                        // Variable and function lexer
                        if (char.IsLetter(lexeme[i]) || lexeme[i] == '_')
                        {
                            string str = getWord();

                            // Avoids i from being larger than lexeme length
                            if (i + 1 == lexeme.Length)
                            {
                                Console.WriteLine("Is a variable");
                                Globals.tokens[tok_i] = Globals.TOK_VAR;
                                Globals.symTable[tok_i++] = new Operand(OpType.var, str.ToString(), -1);
                                break;
                            }
                            else
                            {
                                // All functions have parentheses to take in arguments
                                if (lexeme[i + 1] == '(')
                                {
                                    Console.WriteLine("Is a function");
                                    Globals.tokens[tok_i] = Globals.TOK_FUNC;
                                    Globals.symTable[tok_i++] = new Operand(OpType.func, str.ToString(), -1);
                                }
                                else
                                {
                                    Console.WriteLine("Is a variable");
                                    Globals.tokens[tok_i] = Globals.TOK_VAR;
                                    Globals.symTable[tok_i++] = new Operand(OpType.var, str.ToString(), -1);
                                }
                            }
                        }
                        // Number Lexer
                        else if (char.IsDigit(lexeme[i]))
                        {
                            StringBuilder num = new StringBuilder(Globals.MAX_LEN);
                            bool isDecimal = false;

                            // Creates a temporary array to store numbers of all digit ranges
                            while (char.IsDigit(lexeme[i]) | lexeme[i].Equals('.'))
                            {
                                if (lexeme[i].Equals('.'))
                                {
                                    isDecimal = true;
                                    decimalPointCount++;
                                }
                                num.Append(lexeme[i]);
                                i++;
                                if (i == lexeme.Length) break;
                            }

                            if (isDecimal) Globals.tokens[tok_i] = Globals.TOK_DEC;
                            else Globals.tokens[tok_i] = Globals.TOK_INT;

                            // If there are multiple decimal points in the decimals
                            if (decimalPointCount > 1)
                            {
                                Console.Error.WriteLine("SYNTAX ERROR: Multiple periods in decimal number at token " + tok_i + ", value is " + num.ToString());
                                throw new InvalidSyntaxException("SYNTAX ERROR: Multiple periods in decimal number at token " + tok_i + ", value is " + num.ToString());
                            }
                            Globals.symTable[tok_i++] = new Operand(OpType.num, "", double.Parse(num.ToString()));
                            i--;
                        }
                        else
                        {
                            Console.Error.WriteLine("SYNTAX ERROR: Invalid token " + lexeme[i]);
                            throw new InvalidTokenException("SYNTAX ERROR: Invalid token " + lexeme[i]);
                        }
                        break;
                }
            }
            return tok_i;
        }
    }
}
