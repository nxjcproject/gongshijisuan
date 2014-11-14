using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExpressionHelper
{
    public class ExpressionException : ApplicationException
    {
        public int Location { get; set; }

        public ExpressionException(string message) : base(message) { }

        public ExpressionException(string message, int location)
            : this(message)
        {
            this.Location = location;
        }
    }
}








