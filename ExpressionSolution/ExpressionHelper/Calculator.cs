using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExpressionHelper
{
    public class Calculator
    {
        private Stack<string> operatorStack = new Stack<string>();
        private Stack<string> operandStack = new Stack<string>();

        public double Calculate(string expression)
        {
            expression = Utility.ClearAllSpace(expression);

            MathExpression mathExpression = new MathExpression(expression);
            OperandPicker operandPicker = new OperandPicker(mathExpression);

            operatorStack.Clear();
            operandStack.Clear();

            char currentChar, tempOperator;

            while (true)
            {
                currentChar = mathExpression.Peek();

                if (mathExpression.CanPickOperand())
                {
                    operandPicker.Pick();
                    operandStack.Push(operandPicker.Result);

                    if(mathExpression.EOF && char.IsDigit(mathExpression.Peek()))
                        break;
                }
                else
                {
                    if (currentChar.IsValidOperatorChar())
                    {
                        if (operatorStack.Count == 0)
                            operatorStack.Push(currentChar.ToString());
                        else
                        {
                            tempOperator = operatorStack.Peek()[0];
                            if (currentChar == ')')
                            {
                                CalculateWithRightBracket();
                            }
                            else
                            {
                                if (Utility.Operators[currentChar] > Utility.Operators[tempOperator] || tempOperator == '(')
                                {
                                    operatorStack.Push(currentChar.ToString());
                                }
                                else
                                {
                                    do
                                    {
                                        CalculateSubExpression();
                                        if (operatorStack.Count == 0)
                                            break;
                                        tempOperator = operatorStack.Peek()[0];
                                    } while (Utility.Operators[currentChar] <= Utility.Operators[tempOperator]);

                                    operatorStack.Push(currentChar.ToString());
                                }
                            }
                        }
                    }

                    if (mathExpression.EOF)
                        break;
                    else
                        mathExpression.MoveNext();

                }
            }

            if (operatorStack.Count == 0)
            {
                return Convert.ToDouble(operandStack.Pop());
            }
            else
            {
                while (operatorStack.Count > 0)
                    CalculateSubExpression();

                return Convert.ToDouble(operandStack.Pop());
            }
        }

        private void CalculateWithRightBracket()
        {
            char tempOperator;

            tempOperator = operatorStack.Peek()[0];

            while (tempOperator != '(')
            {
                CalculateSubExpression();
                tempOperator = operatorStack.Peek()[0];
            }

            operatorStack.Pop();
        }

        private void CalculateSubExpression()
        {
            char tempOperator;
            double operand1, operand2;

            tempOperator = operatorStack.Pop()[0];

            operand2 = Convert.ToDouble(operandStack.Pop());
            operand1 = Convert.ToDouble(operandStack.Pop());

            operandStack.Push(Utility.Evaluate(operand1, operand2, tempOperator.ToString()).ToString());
        }
    }
}
