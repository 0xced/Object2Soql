using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace SoqlTests.Entities
{
    public class IlegalExpressionException:Exception
    {
        public ExpressionType ExpressionType { get; }

        public IlegalExpressionException(ExpressionType expressionType):base($"The expression type '{expressionType}' is not supported!")
        {
            ExpressionType = expressionType;
        }

    }
}
