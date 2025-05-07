using Object2Soql.Entities;
using Object2Soql.Helpers;
using System.Collections.Generic;
using System.Linq.Expressions;


namespace Object2Soql.Visitors
{
    public static class SelectVisitor
    {
        public static IEnumerable<string> Visit(Expression? exp)
        {
            if (exp == null)
            {
                return [];
            }

            return exp switch
            {
                MemberExpression expression => new List<string> { Reflection.GetMemberQualifiedName(expression) },
                NewExpression expression => VisitNew(expression),
                UnaryExpression expression => VisitConvert(expression),
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
