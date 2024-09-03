using System;
using System.Collections.Generic;
using System.Windows;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maths_Software_with_Interpreter
{
    class Interpreter
    {
        private static string output;
        // Whether to output the result to the interpreter or not
        private static bool bShowOutput = true;
        // Calculates the result from the user input from the interpreter
        public static string InterpretInput(string input, bool bPrintResult = true)
        {
            output = "";
            bShowOutput = bPrintResult;
            string inputCompare = Globals.input;
            MainWindow MainWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
            // Resetting the token and symbol table arrays
            for (int i = 0; i < Globals.MAX_TOKENS; i++)
            {
                Globals.tokens[i] = 0;
                Globals.symTable[i] = new Operand();
            }

            // INPUT TEST
            // START
            Console.WriteLine("INPUT TEST:\nInput = " + input);
            // END

            // LEXER TEST
            // START
            Console.WriteLine("LEXING START");
            try
            {
                if ((Globals.numTokens = Lex.Lexer(input)) > 0)
                {
                    // Stores the interpreter input in a variable to be used for variable references
                    if (Globals.input == inputCompare)
                    {
                        Globals.input = input;
                        Console.WriteLine("Input in if is: " + Globals.input);
                        int equalSign = Globals.input.IndexOf('=') + 1;
                        int lParenSign = Globals.input.IndexOf('(') + 1;
                        int rParenSign = Globals.input.LastIndexOf(')') + 1;
                        if (equalSign > 0)
                        {
                            // Store input without the left had side of the equals sign
                            Console.WriteLine("Equals found");
                            Globals.input = Globals.input.Substring(equalSign);
                        }
                        else if (lParenSign > 0 && rParenSign > 0)
                        {
                            // Store input within the function's parentheses
                            Console.WriteLine("Parenthesis found");
                            Globals.input = Globals.input.Substring(lParenSign, rParenSign - lParenSign - 1);
                        }
                    }
                    else Globals.input = inputCompare;
                    Console.WriteLine("Input is: " + Globals.input);

                    // Print all the tokens lexed from the input
                    Console.WriteLine("Tokens Read: " + Globals.numTokens);
                    for (int i = 0; i < Globals.numTokens; i++)
                    {
                        if (Globals.symTable[i].GetOpType() == OpType.var)
                            Console.WriteLine("Token " + i + ": ID = " + Globals.tokens[i] + ", Name = "
                                + Globals.GetTokName(Globals.tokens[i]) + ", Variable Name = " + Globals.symTable[i].GetName());
                        else if (Globals.symTable[i].GetOpType() == OpType.func)
                            Console.WriteLine("Token " + i + ": ID = " + Globals.tokens[i] + ", Name = "
                                + Globals.GetTokName(Globals.tokens[i]) + ", Function Name = " + Globals.symTable[i].GetName());
                        else
                            Console.WriteLine("Token " + i + ": ID = " + Globals.tokens[i] + ", Name = "
                                + Globals.GetTokName(Globals.tokens[i]) + ", Value = " + Globals.symTable[i].GetVal());
                    }
                    Console.WriteLine("LEXING SUCCESS");
                }
                else
                {
                    output = "No tokens read";
                    Console.WriteLine("No tokens read");
                    MainWindow.OutputTextBox.Text += ">> " + output + "\n";
                    return output;
                }
            }
            // Catch thrown exceptions from lexer
            catch (InvalidTokenException e)
            {
                output = "Invalid Token";
                Console.Error.WriteLine(e.Message);
                MainWindow.OutputTextBox.Text += ">> " + e.Message + "\n";
                return output;
            }
            catch (InvalidSyntaxException e)
            {
                output = "Invalid Syntax";
                Console.Error.WriteLine(e.Message);
                MainWindow.OutputTextBox.Text += ">> " + e.Message + "\n";
                return output;
            }
            // END

            if (Globals.numTokens > 0)
            {
                // PARSER TEST
                // START
                Console.WriteLine("PARSING START");
                try
                {
                    Parse.Parser();
                    Console.WriteLine("PARSING SUCCESS");
                }
                // Catch thrown exceptions from parser
                catch (InvalidSyntaxException e)
                {
                    output = "Syntax Error";
                    Console.Error.WriteLine(e.Message);
                    MainWindow.OutputTextBox.Text += ">> " + e.Message + "\n";
                    return output;
                }
                catch (InvalidUnaryException e)
                {
                    output = "Syntax Error";
                    Console.Error.WriteLine(e.Message);
                    MainWindow.OutputTextBox.Text += ">> " + e.Message + "\n";
                    return output;
                }
                // END

                // EXEC TEST
                // START
                // Execute the inputted expression and get the result
                try
                {
                    output = Exec.ShuntYard();
                }
                // Catch thrown exceptions from exec
                catch (UnknownFunctionException e)
                {
                    output = "Runtime Error";
                    Console.Error.WriteLine(e.Message);
                    MainWindow.OutputTextBox.Text += ">> " + e.Message + "\n";
                    return output;
                }
                catch (NonVariableException e)
                {
                    output = "Runtime Error";
                    Console.Error.WriteLine(e.Message);
                    MainWindow.OutputTextBox.Text += ">> " + e.Message + "\n";
                    return output;
                }
                // END

                Console.WriteLine("Result = " + output);
            }
            // Return result to window
            return output;
        }
        public static string GetOutput() { return output; }
        public static bool GetbShowOutput() { return bShowOutput; }


    }
}
