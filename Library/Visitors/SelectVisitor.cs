using Object2Soql.Entities;
using Object2Soql.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;


namespace Object2Soql.Visitors
{
    public static class SelectVisitor
    {
        public static IEnumerable<string> Visit(Expression exp)
        {
            if (exp == null)
            {
                return Enumerable.Empty<string>();
            }

            return exp.NodeType switch
            {
                ExpressionType.MemberAccess => new List<string>() { Reflection.GetMemberQualifiedName((exp as MemberExpression)!) },
                ExpressionType.New => VisitNew((exp as NewExpression)!),
                ExpressionType.Convert => VisitConvert((exp as UnaryExpression)!),
                _ => throw new IlegalExpressionException(exp.NodeType),
            };
        }

        private static IEnumerable<string> VisitConvert(UnaryExpression unaryExpression)
        {
            return Visit(unaryExpression.Operand);
        }

        private static IEnumerable<string> VisitNew(NewExpression newExpression)
        {
            var parameters = new List<string>();
            foreach(var parameter in newExpression.Arguments)
            {
                if(parameter is MemberExpression memberExpression)
                {
                    parameters.Add(Reflection.GetMemberQualifiedName(memberExpression));
                }
            }

            return parameters;
        }
    }
}
