using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExpressionHelper
{
    public class MathExpression
    {
        private string expression;
        private int index;

        public MathExpression() : this("") { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="expression">表达式</param>
        public MathExpression(string expression)
        {
            this.Expression = expression;
            this.index = 0;
        }

        /// <summary>
        /// 获取或设置表达式
        /// </summary>
        public string Expression
        {
            get { return expression; }
            set { expression = Utility.ClearAllSpace(value); }
        }

        /// <summary>
        /// 表达式长度
        /// </summary>
        public int Length
        {
            get { return expression.Length; }
        }

        /// <summary>
        /// 到达表达式结尾
        /// </summary>
        public bool EOF
        {
            get { return CurrentIndex == expression.Length - 1; }
        }

        /// <summary>
        /// 当前指针位置
        /// </summary>
        public int CurrentIndex
        {
            get { return index; }
            set
            {
                if (value < 0 || value > expression.Length - 1)
                    throw new IndexOutOfRangeException("字符索引超出范围");

                index = value;
            }
        }

        /// <summary>
        /// 获取当前索引指向的字符，指针不动
        /// </summary>
        /// <returns></returns>
        public char Peek()
        {
            return GetChar(CurrentIndex);
        }

        /// <summary>
        /// 获取当前索引指向的字符，指针后移
        /// </summary>
        /// <returns></returns>
        public char Read()
        {
            return GetChar(CurrentIndex++);
        }

        /// <summary>
        /// 获取指定索引指向的字符
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public char GetChar(int index)
        {
            if(index <0 || index> expression.Length-1)
                throw new IndexOutOfRangeException("字符索引超出范围");

            return expression[index];
        }

        /// <summary>
        /// 指针后移
        /// </summary>
        public void MoveNext()
        {
            if (this.EOF)
                throw new IndexOutOfRangeException("已到达表达式结尾，索引超出范围");

            CurrentIndex = CurrentIndex + 1;
        }
    }
}
