using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExpressionHelper
{
    public static class Utility
    {
        public static IDictionary<char, int> Operators = new Dictionary<char, int>();

        static Utility()
        {
            Operators.Add('+', 0);
            Operators.Add('-', 0);
            Operators.Add('*', 1);
            Operators.Add('/', 1);
            Operators.Add('(', 2);
            Operators.Add(')', 2);
        }

        /// <summary>
        /// 判断字符是否为有效的操作数起始位字符
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static bool IsValidOperandChar(this char c)
        {
            if (char.IsDigit(c) ||
                c.IsVariableIndicator() ||
                c == '.' ||
                c == '-')
                return true;

            return false;
        }

        /// <summary>
        /// 判断字符是否为有效的变量起始标志字符
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static bool IsVariableIndicator(this char c)
        {
            return (c == 'A' || c == 'S' || c == 'P');
        }

        /// <summary>
        /// 判断字符是否为有效的运算符字符
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static bool IsValidOperatorChar(this char c)
        {
            return Operators.ContainsKey(c);
        }

        /// <summary>
        /// 判断表达式当前是否可以拾取操作数
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static bool CanPickOperand(this MathExpression expression)
        {
            char currentChar = expression.Peek();

            if (char.IsDigit(currentChar))
                return true;

            if (currentChar == '.')
                return true;

            if (currentChar == '-' && expression.CurrentIndex == 0)
                return true;

            if (currentChar == '-' && expression.CurrentIndex > 0 && expression.GetChar(expression.CurrentIndex - 1) == '(')
                return true;

            return false;
        }

        /// <summary>
        /// 清除字符串中所有的空格
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static string ClearAllSpace(string expression)
        {
            return expression.Replace(" ", string.Empty);
        }

        /// <summary>
        /// 检验数学表达式合法性
        /// </summary>
        /// <param name="expression">数学表达式</param>
        /// <returns>ExpressionException异常的集合</returns>
        public static IList<ExpressionException> ValidateExpression(string expression)
        {
            IList<ExpressionException> errors = new List<ExpressionException>();

            // 当表达式为空时，直接返回空异常
            if (string.IsNullOrWhiteSpace(expression))
            {
                errors.Add(new ExpressionException("表达式不能为空", 0));
                return errors;
            }

            MathExpression mathExpression = new MathExpression(expression);
            OperandPicker operandPicker = new OperandPicker(mathExpression);

            bool validateFinished = false;
            int bracketCount = 0;
            char currentChar, lastOperator = ' ';
            MathExpressionElementType lastElementType = MathExpressionElementType.Others;

            while (validateFinished == false)
            {
                currentChar = mathExpression.Peek();

                // 当前位字符为合法操作数时（不考虑负数情形）
                if (currentChar.IsValidOperandChar() && currentChar != '-')
                {
                    if (mathExpression.EOF)
                    {
                        validateFinished = true;
                        continue;
                    }

                    operandPicker.Pick();

                    if (operandPicker.Result[operandPicker.Result.Length - 1] == '.')
                        errors.Add(new ExpressionException("小数点后必须是数字", mathExpression.CurrentIndex + 1));

                    if (lastOperator == ')')
                        errors.Add(new ExpressionException("右括号后不可直接跟操作数", operandPicker.StartIndex));

                    if (lastElementType == MathExpressionElementType.Operand)
                        errors.Add(new ExpressionException("不可以连续出现两个操作数", operandPicker.StartIndex));

                    lastElementType = MathExpressionElementType.Operand;
                }
                // 当前位字符为合法运算符时
                else if (currentChar.IsValidOperatorChar())
                {
                    if (currentChar == '(')
                        bracketCount++;

                    if (currentChar == ')')
                        bracketCount--;

                    if (bracketCount < 0)
                        errors.Add(new ExpressionException("括号不匹配，右括号比左括号多", mathExpression.CurrentIndex + 1));

                    if (mathExpression.CurrentIndex == 0 && currentChar != '(' && currentChar != '-')
                        errors.Add(new ExpressionException("表达式起始字符无效", 1));

                    if (currentChar == '(' && lastElementType == MathExpressionElementType.Operand)
                        errors.Add(new ExpressionException("操作数后不可直接跟左括号", mathExpression.CurrentIndex + 1));

                    if (currentChar == ')' && lastElementType == MathExpressionElementType.Operator)
                        errors.Add(new ExpressionException("右括号前必须是操作数", mathExpression.CurrentIndex + 1));

                    if (lastElementType == MathExpressionElementType.Operator && currentChar != '(' && currentChar != ')')
                        errors.Add(new ExpressionException("不可以连续出现两个运算符", mathExpression.CurrentIndex + 1));

                    if (mathExpression.EOF && currentChar != ')')
                        errors.Add(new ExpressionException("表达式不可以运算符结尾", mathExpression.CurrentIndex + 1));


                    if (currentChar == '(' || currentChar == ')')
                        lastElementType = MathExpressionElementType.Others;
                    else
                        lastElementType = MathExpressionElementType.Operator;


                    if (mathExpression.EOF)
                        validateFinished = true;
                    else
                        mathExpression.MoveNext();

                    lastOperator = currentChar;
                }
                // 当前为既非操作数也非运算符时，为无法识别的字符
                else
                {
                    errors.Add(new ExpressionException("无法识别的字符", mathExpression.CurrentIndex + 1));

                    if (mathExpression.EOF)
                        validateFinished = true;
                    else
                        mathExpression.MoveNext();
                }
            }

            if(bracketCount > 0)
                errors.Add(new ExpressionException("括号不匹配，左括号比右括号多", 0));

            return errors;
        }

        public static double Evaluate(double x, double y, string operators)
        {
            double result = 0;
            switch (operators)
            {
                case "+":
                    result = x + y;
                    break;
                case "-":
                    result = x - y;
                    break;
                case "*":
                    result = x * y;
                    break;
                case "/":
                    result = x / y;
                    break;
            }

            return result;
        }
    }
}