using System;
using System.Collections.Generic;
using System.Windows;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maths_Software_with_Interpreter
{
    class Exec
    {
        // Stack for operators with operator token IDs
        private static Stack<int> OpStack;
        // Stack for numbers and variable names
        private static Stack<Operand> NumStack;
        private static string result;

        private static void Calculate()
        {
            // '?' represents nullable data types (can have no value)
            int? opID;
            Operand op1, op2;

            Console.WriteLine("Calculation started");

            // Operator for current operation
            try
            {
                opID = OpStack.Pop();
                Console.WriteLine("Operator ID = " + opID + ", Name = " + Globals.GetTokName(opID.Value));
            }
            catch (InvalidOperationException)
            {
                opID = null;
                Console.WriteLine("Operator ID is null");
            }

            // Operands for current operation
            op2 = NumStack.Pop();

            // Avoids error when only one Operand object is in the NumStack
            try
            {
                op1 = NumStack.Pop();
                Console.WriteLine("Operands are " + op1.GetName() + ", " + op1.GetVal() + " and " + op2.GetName() + ", " + op2.GetVal());

                // If the operand is a pre-existing variable, retrieve its value from the variable list
                if (op1.GetOpType() == OpType.var)
                {
                    foreach (Operand o in Globals.variables)
                    {
                        if (o.GetName() == op1.GetName())
                        {
                            op1.SetVal(o.GetVal());
                            break;
                        }
                    }
                }
            }
            catch (InvalidOperationException)
            {
                op1 = null;
                Console.WriteLine("Operand 1 is null");
                Console.WriteLine("Operand 2 is " + op2.GetName() + ", " + op2.GetVal());
            }

            // If the operand is a pre-existing variable, retrieve its value from the variable list
            if (op2.GetOpType() == OpType.var)
            {
                foreach (Operand o in Globals.variables)
                {
                    if (o.GetName() == op2.GetName())
                    {
                        op2.SetVal(o.GetVal());
                        break;
                    }
                }
            }

            int op2Index = Globals.variables.FindIndex(item => item.GetName() == op2.GetName());

            switch (opID)
            {
                // '+'
                case Globals.TOK_PLUS:
                    // Result pushed to number stack
                    NumStack.Push(new Operand(OpType.num, "", op1.GetVal() + op2.GetVal()));
                    result = NumStack.Peek().GetVal().ToString();
                    break;
                // '-'
                case Globals.TOK_SUB:
                    // Result pushed to number stack
                    NumStack.Push(new Operand(OpType.num, "", op1.GetVal() - op2.GetVal()));
                    result = NumStack.Peek().GetVal().ToString();
                    break;
                // '*'
                case Globals.TOK_TIMES:
                    // Result pushed to number stack
                    NumStack.Push(new Operand(OpType.num, "", op1.GetVal() * op2.GetVal()));
                    result = NumStack.Peek().GetVal().ToString();
                    break;
                // '/'
                case Globals.TOK_DIV:
                    // Result pushed to number stack
                    NumStack.Push(new Operand(OpType.num, "", op1.GetVal() / op2.GetVal()));
                    result = NumStack.Peek().GetVal().ToString();
                    break;
                // '%'
                case Globals.TOK_MOD:
                    // Result pushed to number stack
                    NumStack.Push(new Operand(OpType.num, "", op1.GetVal() - (op2.GetVal() * Math.Truncate(op1.GetVal() / op2.GetVal()))));
                    result = NumStack.Peek().GetVal().ToString();
                    break;
                // '^'
                case Globals.TOK_POW:
                    // Result pushed to number stack
                    NumStack.Push(new Operand(OpType.num, "", Math.Pow(op1.GetVal(), op2.GetVal())));
                    result = NumStack.Peek().GetVal().ToString();
                    break;
                // 'FUNCOP' is an operator that represents a function
                case Globals.TOK_FUNCOP:
                    // Throws exception if the argument for the function is a non-defined variable
                    if (op2.GetOpType() == OpType.var && op2Index == -1)
                    {
                        Console.Error.WriteLine("RUNTIME ERROR: Function argument " + op2.GetName() +" does not exist");
                        throw new NonVariableException("RUNTIME ERROR: Function argument " + op2.GetName() + " does not exist");
                    }
                    Console.WriteLine("Performing function " + op1.GetName());
                    double degree = op2.GetVal();
                    if (!Globals.rad)
                    {
                        degree *= Math.PI / 180;
                    }
                    switch (op1.GetName().ToLower())
                    {
                        case "sin":
                            NumStack.Push(new Operand(OpType.num, "", Math.Sin(degree)));
                            break;
                        case "cos":
                            NumStack.Push(new Operand(OpType.num, "", Math.Cos(degree)));
                            break;
                        case "tan":
                            NumStack.Push(new Operand(OpType.num, "", Math.Tan(degree)));
                            break;
                        case "arcsin":
                            NumStack.Push(new Operand(OpType.num, "", Math.Asin(degree)));
                            break;
                        case "arccos":
                            NumStack.Push(new Operand(OpType.num, "", Math.Acos(degree)));
                            break;
                        case "arctan":
                            NumStack.Push(new Operand(OpType.num, "", Math.Atan(degree)));
                            break;
                        case "ln":
                            NumStack.Push(new Operand(OpType.num, "", Math.Log(op2.GetVal())));
                            break;
                        case "log":
                            NumStack.Push(new Operand(OpType.num, "", Math.Log10(op2.GetVal())));
                            break;
                        case "sqrt":
                            NumStack.Push(new Operand(OpType.num, "", Math.Sqrt(op2.GetVal())));
                            break;
                        case "plot":
                            // now draw the graph
                            MainWindow MainWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
                            op1.SetExpression(Globals.input);
                            if (op1.GetExpression() != "")  // plot expression
                            {
                                Console.WriteLine("Plot function expression (plot): "+ op1.GetExpression());
                                MainWindow.InputTextBox.Text = op1.GetExpression();
                            }
                            else
                            {
                                Console.WriteLine("Plot function value (plot): " + op1.GetVal().ToString());
                                MainWindow.InputTextBox.Text = op1.GetVal().ToString(); ;
                            }
                            MainWindow.DrawGraph(new object(), new RoutedEventArgs());
                            NumStack.Push(new Operand(OpType.num, "", 0));
                            break;
                        default:
                            // throw Unknown Function Exception
                            Console.Error.WriteLine("RUNTIME ERROR: Unknown function " + op1.GetName());
                            throw new UnknownFunctionException("RUNTIME ERROR: Unknown function " + op1.GetName());
                    }
                    result = NumStack.Peek().GetVal().ToString();
                    break;
                // If no operator is in the OpStack
                case null:
                    // Throws exception if the variable is non-defined
                    if (op2.GetOpType() == OpType.var && op2Index == -1)
                    {
                        Console.Error.WriteLine("RUNTIME ERROR: Variable " + op2.GetName() + " does not exist");
                        throw new NonVariableException("RUNTIME ERROR: Variable " + op2.GetName() + " does not exist");
                    }
                    if (op1 == null)
                    {
                        Console.WriteLine("Only one variable " + op2.GetName());
                        // Return the second operand as the result
                        result = op2.GetVal().ToString();
                    }
                    break;
                // '='
                default:
                    Console.WriteLine("Two variables " + op2.GetName());
                    int op1Index = Globals.variables.FindIndex(item => item.GetName() == op1.GetName());

                    // If operand 2 isn't an existing variable, throw exception
                    if (op2.GetOpType() == OpType.var && op2Index == -1)
                    {
                        Console.Error.WriteLine("RUNTIME ERROR: Variable " + op2.GetName() + " does not exist");
                        throw new NonVariableException("RUNTIME ERROR: Variable " + op2.GetName() + " does not exist");
                    }
                    // If operator 1 is a variable
                    if (op1.GetOpType() == OpType.var)
                    {
                        // If operator 1 is an existing variable
                        if (op1Index != -1)
                        {
                            if (op1.GetName() == "e" || op1.GetName() == "π")
                            {
                                // Throw Non Variable Exception
                                Console.Error.WriteLine("RUNTIME ERROR: Cannot change the value of π or e");
                                throw new NonVariableException("RUNTIME ERROR: Cannot change the value of π or e");
                            }
                            Console.WriteLine("Updating variable " + op1.GetName());
                            // Set value of operator 1 to operator 2
                            Globals.variables[op1Index].SetVal(op2.GetVal());
                        }
                        else
                        {
                            // Otherwise add a new variable
                            Console.WriteLine("Adding variable " + op1.GetName());
                            // Add new variable to the variable array
                            Globals.variables.Add(new Operand(OpType.var, op1.GetName(), op2.GetVal()));
                            // Redefine the index for the new variable in the global variable array
                            op1Index = Globals.variables.FindIndex(item => item.GetName() == op1.GetName());
                        }

                        // If operand 1 has a pre-existing reference, set it to an empty string to reset its value
                        if (Globals.variables[op1Index].GetRefer() != "" && Globals.variables[op1Index].GetName() == op1.GetName())
                        {
                            Globals.variables[op1Index].SetRefer("");
                        }
                        // Else set the reference of operand 1 to the user input
                        else
                        {
                            Globals.variables[op1Index].SetRefer(Globals.input);
                        }

                        foreach (Operand o in Globals.variables)
                        {
                            Console.WriteLine("Variable Name: " + o.GetName() + ", Reference: " + o.GetRefer());
                            // If the existing variables refer to operator 2
                            if (o.GetRefer() != "")
                            {
                                Console.WriteLine("Get Refer not empty");
                                // Change the value of the variables that refer to operator 2
                                o.SetVal(double.Parse(Interpreter.InterpretInput(o.GetRefer())));
                            }
                            else Console.WriteLine("Get Refer empty");
                        }

                        result = op2.GetVal().ToString();

                        // update variable window
                        App.Current.Dispatcher.Invoke(new Action(() =>
                        {
                            var mainWindow = App.Current.MainWindow as MainWindow;
                            mainWindow.UpdateAllVariablesInWindow();
                        }));
                        break;
                    }
                    else
                    {
                        // Throw Non Variable Exception
                        Console.Error.WriteLine("RUNTIME ERROR: Cannot assign number/variable to non-variable " + op1.GetVal());
                        throw new NonVariableException("RUNTIME ERROR: Cannot assign number/variable to non-variable " + op1.GetVal());
                    }
            }
            Console.WriteLine("New result is " + result);
        }

        // Returns the boolean value for the bidmas conditions for the specified operator
        private static bool Bidmas(int tok)
        {
            switch (tok)
            {
                case Globals.TOK_PLUS:
                case Globals.TOK_SUB:
                    return OpStack.Peek() != Globals.TOK_LPAR && OpStack.Peek() != Globals.TOK_EQUAL;
                case Globals.TOK_TIMES:
                case Globals.TOK_DIV:
                case Globals.TOK_MOD:
                    return OpStack.Peek() != Globals.TOK_LPAR && OpStack.Peek() != Globals.TOK_PLUS
                        && OpStack.Peek() != Globals.TOK_SUB && OpStack.Peek() != Globals.TOK_EQUAL;
                case Globals.TOK_POW:
                    return OpStack.Peek() != Globals.TOK_LPAR && OpStack.Peek() != Globals.TOK_PLUS
                            && OpStack.Peek() != Globals.TOK_SUB && OpStack.Peek() != Globals.TOK_TIMES
                            && OpStack.Peek() != Globals.TOK_DIV && OpStack.Peek() != Globals.TOK_MOD
                            && OpStack.Peek() != Globals.TOK_EQUAL;
            }
            return true;
        }

        // Add the operator to the OpStack
        private static void StackOperator(int i)
        {
            // Check OpStack count and peek
            Console.WriteLine("OpStack Count = " + OpStack.Count());
            if (OpStack.Count() > 0) Console.WriteLine("OpStack Peek = " + OpStack.Peek());
            // If BIDMAS conditions are met for the operator to be calculated, then calculate
            while (OpStack.Count != 0 && Bidmas(i))
                Calculate();
            OpStack.Push(i);
            Console.WriteLine("Operator " + OpStack.Peek() + " added to stack");
        }

        // If unary operators are '++, or '+-', remove one plus sign
        public static void RemovePlus(int i)
        {
            for (int j = i; j < Globals.MAX_TOKENS - 1; j++)
            {
                Globals.tokens[j] = Globals.tokens[j + 1];
                Globals.symTable[j] = Globals.symTable[j + 1];
            }
            Globals.tokens[Globals.MAX_TOKENS - 1] = 0;
            Globals.symTable[Globals.MAX_TOKENS - 1] = new Operand();
        }

        // If unary operators are '-+' or '--', replace one minus sign with '-1 *'
        public static void RemoveMinus(int i)
        {
            // Temporary arrays for global tokens and symbol table
            int[] newTokens = new int[Globals.MAX_TOKENS + 1];
            Operand[] newSymTable = new Operand[Globals.MAX_TOKENS + 1];

            for (int j = 0; j < i; j++)
            {
                newTokens[j] = Globals.tokens[j];
                newSymTable[j] = Globals.symTable[j];
            }

            newTokens[i] = Globals.TOK_INT;
            newSymTable[i] = new Operand();
            newTokens[i + 1] = Globals.TOK_TIMES;
            newSymTable[i + 1] = new Operand();
            Globals.numTokens++;

            for (int j = i + 1; j < Globals.MAX_TOKENS - 2; j++)
            {
                newTokens[j + 1] = Globals.tokens[j];
                newSymTable[j + 1] = Globals.symTable[j];
            }

            // Store new values into original arrays
            Globals.tokens = newTokens;
            Globals.symTable = newSymTable;
        }

        public static string ShuntYard()
        {
            // Initialise the OpStack and NumStack
            OpStack = new Stack<int>();
            NumStack = new Stack<Operand>();
            result = null;

            int i = 0;

            while (i < Globals.numTokens)
            {
                switch (Globals.tokens[i])
                {
                    case Globals.TOK_INT:
                    case Globals.TOK_DEC:
                        // Check NumStack count and peek
                        Console.WriteLine("NumStack Count = " + NumStack.Count());
                        if (NumStack.Count() > 0)
                        {
                            if (NumStack.Peek().GetOpType() == OpType.num)
                            {
                                Console.WriteLine("NumStack Peek = " + NumStack.Peek().GetVal());
                            }
                            else
                            {
                                Console.WriteLine("NumStack Peek = " + NumStack.Peek().GetName());
                            }
                        }
                        // Push number to the NumStack
                        NumStack.Push(Globals.symTable[i]);
                        Console.WriteLine("Number " + NumStack.Peek().GetVal() + " added to stack");
                        break;
                    case Globals.TOK_VAR:
                        // Check NumStack count and peek
                        Console.WriteLine("NumStack Count = " + NumStack.Count());
                        if (NumStack.Count() > 0)
                        {
                            if (NumStack.Peek().GetOpType() == OpType.num)
                            {
                                Console.WriteLine("NumStack Peek = " + NumStack.Peek().GetVal());
                            }
                            else
                            {
                                Console.WriteLine("NumStack Peek = " + NumStack.Peek().GetName());
                            }
                        }
                        // Push variable to the NumStack
                        NumStack.Push(Globals.symTable[i]);
                        Console.WriteLine("Variable " + NumStack.Peek().GetName() + " added to stack");
                        break;
                    case Globals.TOK_FUNC:
                        // Check OpStack count and peek
                        Console.WriteLine("OpStack Count = " + OpStack.Count());
                        if (OpStack.Count() > 0) Console.WriteLine("OpStack Peek = " + OpStack.Peek());
                        // Check NumStack count and peek
                        Console.WriteLine("NumStack Count = " + NumStack.Count());
                        if (NumStack.Count() > 0)
                        {
                            if (NumStack.Peek().GetOpType() == OpType.num)
                            {
                                Console.WriteLine("NumStack Peek = " + NumStack.Peek().GetVal());
                            }
                            else
                            {
                                Console.WriteLine("NumStack Peek = " + NumStack.Peek().GetName());
                            }
                        }
                        // Push FUNCOP operator to OpStack and the object containing the function's name to NumStack
                        OpStack.Push(Globals.TOK_FUNCOP);
                        NumStack.Push(Globals.symTable[i]);
                        Console.WriteLine("Function " + NumStack.Peek().GetName() + " added to stack");
                        break;
                    case Globals.TOK_PLUS:
                        // If unary operator is the first token or not
                        if (i > 0)
                        {
                            // If the expression contains '++' or '-+'
                            if (Globals.tokens[i - 1] < Globals.TOK_INT)
                            {
                                // Treat like a positive sign
                                RemovePlus(i);
                                i--;
                            }
                            // Treat like a plus operator
                            else
                            {
                                StackOperator(Globals.tokens[i]);
                            }
                        }
                        else
                        {
                            // Treat like a positive sign
                            RemovePlus(i);
                            i--;
                        }
                        break;
                    case Globals.TOK_SUB:
                        // If unary operator is the first token or not
                        if (i > 0)
                        {
                            // If the expression contains '+-' or '--'
                            if (Globals.tokens[i - 1] < Globals.TOK_INT)
                            {
                                // Treat like a negative sign
                                RemoveMinus(i);
                                i--;
                            }
                            else
                            {
                                // Treat like a subtract operator
                                StackOperator(Globals.tokens[i]);
                            }
                        }
                        else
                        {
                            // Treat like a negative sign
                            RemoveMinus(i);
                            i--;
                        }
                        break;
                    case Globals.TOK_TIMES:
                        StackOperator(Globals.tokens[i]);
                        break;
                    case Globals.TOK_DIV:
                        StackOperator(Globals.tokens[i]);
                        break;
                    case Globals.TOK_MOD:
                        StackOperator(Globals.tokens[i]);
                        break;
                    case Globals.TOK_POW:
                        StackOperator(Globals.tokens[i]);
                        break;
                    case Globals.TOK_LPAR:
                        // Check OpStack count and peek
                        Console.WriteLine("OpStack Count = " + OpStack.Count());
                        if (OpStack.Count() > 0) Console.WriteLine("OpStack Peek = " + OpStack.Peek());
                        // Push operator to OpStack
                        OpStack.Push(Globals.tokens[i]);
                        Console.WriteLine("Operator " + OpStack.Peek() + " added to stack");
                        break;
                    case Globals.TOK_RPAR:
                        // Check OpStack count and peek
                        Console.WriteLine("OpStack Count = " + OpStack.Count());
                        if (OpStack.Count() > 0) Console.WriteLine("OpStack Peek = " + OpStack.Peek());
                        // Perform calculations on OpStack if BIDMAS conditions are met
                        while (OpStack.Count != 0 && OpStack.Peek() != Globals.TOK_LPAR && OpStack.Peek() != Globals.TOK_EQUAL)
                            Calculate();
                        // Remove left parentheses (bracketed expression)
                        if (OpStack.Peek() == Globals.TOK_LPAR) OpStack.Pop();
                        break;
                    case Globals.TOK_EQUAL:
                        StackOperator(Globals.tokens[i]);
                        break;
                }
                i++;
            }

            // Once all the operators and numbers/variables/functions have been added, perform the calculations until the stacks are empty
            while (OpStack.Count != 0 || NumStack.Count != 0) Calculate();
            // Reset the stacks
            OpStack.Clear();
            NumStack.Clear();
            // Return the result
            return result;
        }
    }
}
