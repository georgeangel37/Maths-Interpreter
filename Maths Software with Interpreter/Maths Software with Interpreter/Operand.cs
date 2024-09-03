using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maths_Software_with_Interpreter
{
    // Type of operand that the operand object is
    enum OpType
    {
        num,
        var,
        func
    }
    // Object used for operands to store value, operand type and if a variable/function, the name, reference and expression
    class Operand
    {
        private OpType type;
        private string name;
        private double val;
        private string refer;
        private string expression;

        // Default constructor
        public Operand()
        {
            type = OpType.num;
            name = "";
            val = -1;
            refer = "";
            expression = "";
        }

        public Operand(OpType type, string name, double val, string expression = "")
        {
            this.type = type;
            this.name = name;
            this.val = val;
            refer = "";
            this.expression = expression;
        }

        // Accessor methods
        public OpType GetOpType() { return type; }
        public void SetOpType(OpType type) { this.type = type; }
        public string GetName() { return name; }
        public void SetName(string name) { this.name = name; }
        public double GetVal() { return val; }
        public void SetVal(double val) { this.val = val; }
        public string GetRefer() { return refer; }
        public void SetRefer(string refer) { this.refer = refer; }
        public string GetExpression() { return expression; }
        public void SetExpression(string expression) { this.expression = expression; }
    }
}
