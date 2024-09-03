using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Maths_Software_with_Interpreter
{
    class Globals
    {
        // Token IDs for valid tokens:
        //  '+'  = 1
        //  '-'  = 2
        //  '*'  = 3
        //  '/'  = 4
        //  '%'  = 5
        //  '^'  = 6
        //  '('  = 7
        //  ')'  = 8
        //  '.'  = 9
        //  '='  = 10
        //  int  = 11
        //  dec  = 12
        //  var  = 13
        //  func = 14

        // Special tokens:
        //  funcOp = 15

        public const int TOK_PLUS = 1;
        public const int TOK_SUB = 2;
        public const int TOK_TIMES = 3;
        public const int TOK_DIV = 4;
        public const int TOK_MOD = 5;
        public const int TOK_POW = 6;
        public const int TOK_LPAR = 7;
        public const int TOK_RPAR = 8;
        public const int TOK_DOT = 9;
        public const int TOK_EQUAL = 10;
        public const int TOK_INT = 11;
        public const int TOK_DEC = 12;
        public const int TOK_VAR = 13;
        public const int TOK_FUNC = 14;
        public const int TOK_FUNCOP = 15;

        public const int MAX_TOKENS = 32;
        public const int MAX_LEN = 32;

        // Token array, symbol table array, and variables dictionary
        public static int[] tokens = new int[MAX_TOKENS];
        public static Operand[] symTable = new Operand[MAX_TOKENS];
        public static int numTokens;
        public static List<Operand> variables = new List<Operand>{
            new Operand(OpType.var, "x", 10),
            new Operand(OpType.var, "var", 1),
            new Operand(OpType.var, "number20", 20),
            new Operand(OpType.var, "y", 24),
            new Operand(OpType.var, "e", Math.E),
            new Operand(OpType.var, "π", Math.PI)
        };
        // If the angle type is radian or degree
        public static bool rad = true;
        public static string input = "";

        // Return the names of the tokens
        public static string GetTokName(int op)
        {
            switch (op)
            {
                case TOK_PLUS:
                    return "Plus";
                case TOK_SUB:
                    return "Subtract";
                case TOK_TIMES:
                    return "Mulitply";
                case TOK_DIV:
                    return "Divide";
                case TOK_MOD:
                    return "Modulus";
                case TOK_POW:
                    return "Power";
                case TOK_LPAR:
                    return "Left Parenthesis";
                case TOK_RPAR:
                    return "Right Parenthesis";
                case TOK_EQUAL:
                    return "Equals";
                case TOK_INT:
                    return "Integer";
                case TOK_DEC:
                    return "Decimal";
                case TOK_VAR:
                    return "Variable";
                case TOK_FUNC:
                    return "Function";
                default:
                    return "Unknown Token";
            }
        }
    }
}
