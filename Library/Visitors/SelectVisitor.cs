using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Runtime.Serialization;
using Visitor;
using SoqlTests.Entities;

namespace SOQL
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
                ExpressionType.MemberAccess => Namer.GetMemberQualifiedName(exp as MemberExpression),
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
                    parameters.Add(Namer.GetMemberQualifiedName(memberExpression));
                }
            }

            return string.Join(SELECT_FIELDS_SEPARATOR, parameters);
        }
    }
}
