using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExpressionHelper
{
    /// <summary>
    /// 操作数拾取器状态
    /// </summary>
    public enum OperandPickerState
    {
        Begin,          // 起始状态
        Variable,       // 变量
        Negative,       // 负数
        Integer,        // 整数
        Float,          // 浮点
        End,            // 结束状态
        ResultReady     // 结果可用
    }

    public class OperandPicker
    {
        private MathExpression expression;
        private OperandPickerState currentState;
        private int startIndex;

        private IDictionary<OperandPickerState, Action> actions;

        /// <summary>
        /// 获取操作数拾取结果
        /// </summary>
        public string Result { get; private set; }

        /// <summary>
        /// 获取操作数拾取起始位置
        /// </summary>
        public int StartIndex { get { return this.startIndex; } }

        /// <summary>
        /// 操作数拾取器构造函数
        /// </summary>
        /// <param name="expression">进行操作的数学表达式</param>
        public OperandPicker(MathExpression expression)
        {
            this.expression = expression;
            this.InitializeActions();
        }

        /// <summary>
        /// 初始化状态机方法体
        /// </summary>
        private void InitializeActions()
        {
            this.actions = new Dictionary<OperandPickerState, Action>();

            // 起始状态方法体
            actions[OperandPickerState.Begin] = () =>
            {
                char currentChar = expression.Peek();

                if (char.IsDigit(currentChar))
                    currentState = OperandPickerState.Integer;

                else if (currentChar == '.')
                    currentState = OperandPickerState.Float;

                else if (currentChar == '-')
                    currentState = OperandPickerState.Negative;

                else if (currentChar == 'A' || currentChar == 'S' || currentChar == 'P')
                    currentState = OperandPickerState.Variable;

                else
                    throw new InvalidOperationException("不能以无效的字符作为起始位置");

                if (expression.EOF == false)
                    expression.MoveNext();
            };

            // 负数状态方法体
            actions[OperandPickerState.Negative] = () =>
            {
                if (expression.EOF)
                {
                    currentState = OperandPickerState.End;
                    return;
                }

                char currentChar = expression.Peek();

                if (char.IsDigit(currentChar))
                {
                    currentState = OperandPickerState.Integer;
                    expression.MoveNext();
                }
                else
                {
                    currentState = OperandPickerState.End;
                }

            };

            // 变量状态方法体
            actions[OperandPickerState.Variable] = () =>
            {
                if (expression.EOF)
                {
                    currentState = OperandPickerState.End;
                    return;
                }

                char currentChar = expression.Peek();

                if (char.IsLetterOrDigit(currentChar))
                    expression.MoveNext();
                else
                    currentState = OperandPickerState.End;
            };

            // 浮点状态方法体
            actions[OperandPickerState.Float] = () =>
            {
                if (expression.EOF)
                {
                    currentState = OperandPickerState.End;
                    return;
                }

                char currentChar = expression.Peek();

                if (char.IsDigit(currentChar))
                    expression.MoveNext();
                else
                    currentState = OperandPickerState.End;
            };

            // 整数状态方法体
            actions[OperandPickerState.Integer] = () =>
            {
                if (expression.EOF)
                {
                    currentState = OperandPickerState.End;
                    return;
                }

                char currentChar = expression.Peek();

                if (currentChar == '.')
                {
                    currentState = OperandPickerState.Float;
                    expression.MoveNext();
                }
                else if (char.IsDigit(currentChar))
                {
                    expression.MoveNext();
                }
                else
                {
                    currentState = OperandPickerState.End;
                }
            };

            // 结束状态方法体
            actions[OperandPickerState.End] = () =>
            {
                // 当指针移动到表达式末尾时，所取操作数需要包括最后一位字符
                int length = expression.EOF ? (expression.CurrentIndex - startIndex + 1) : (expression.CurrentIndex - startIndex);
                Result = expression.Expression.Substring(this.startIndex, length);
                currentState = OperandPickerState.ResultReady;
            };
        }

        /// <summary>
        /// 根据公式当前指针位置开始拾取操作数，结果存放在Result中
        /// </summary>
        public void Pick()
        {
            this.currentState = OperandPickerState.Begin;
            this.startIndex = expression.CurrentIndex;

            while (this.currentState != OperandPickerState.ResultReady)
            {
                actions[this.currentState]();
            }
        }
    }
}
