using Object2Soql.Entities;
using Object2Soql.Helpers;
using System.Collections.Generic;
using System.Linq.Expressions;


namespace Object2Soql.Visitors
{
    public static class SelectVisitor
    {
        public static readonly string SELECT_FIELDS_SEPARATOR = ", ";

        public static string Visit(Expression exp)
        {
            if (exp == null)
            {
                return string.Empty;
            }

            return exp.NodeType switch
            {
                ExpressionType.MemberAccess => Reflection.GetMemberQualifiedName(exp as MemberExpression),
                ExpressionType.New => VisitNew(exp as NewExpression),
                ExpressionType.Convert => VisitConvert(exp as UnaryExpression),
                _ => throw new IlegalExpressionException(exp.NodeType),
            };
        }

        private static string VisitConvert(UnaryExpression unaryExpression)
        {
            return Visit(unaryExpression.Operand);
        }

        private static string VisitNew(NewExpression newExpression)
        {
            var parameters = new List<string>();
            foreach(var parameter in newExpression.Arguments)
            {
                if(parameter is MemberExpression memberExpression)
                {
                    parameters.Add(Reflection.GetMemberQualifiedName(memberExpression));
                }
            }

            return string.Join(SELECT_FIELDS_SEPARATOR, parameters);
        }
    }
}
