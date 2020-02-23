using SoqlLibrary.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;

namespace Visitor
{
    public static class Namer
    {
        public const char QUALIFIED_NAME_SEPARATOR = '.';


        public static string GetMemberQualifiedName(MemberExpression memberExpression)
        {
            if(memberExpression == null)
            {
                throw new ArgumentNullException(nameof(memberExpression));
            }

            var qualifiedNames = new Stack<string>();
            qualifiedNames.Push(GetNameOf(memberExpression.Member));

            var parentExpression = memberExpression.Expression;
            while (parentExpression != null && parentExpression is MemberExpression parentMemberExpression)
            {
                qualifiedNames.Push(parentMemberExpression.Member.Name);
                parentExpression = parentMemberExpression.Expression;
            }

            return string.Join(QUALIFIED_NAME_SEPARATOR, qualifiedNames);
        }

        public static string GetNameOf(MemberInfo member)
        {
            var jsonAttr = member.GetCustomAttribute<JsonPropertyNameAttribute>();
            if (jsonAttr != null)
            {
                return jsonAttr.Name;
            }

            var forceAttr = member.GetCustomAttribute<SalesForceAttribute>();
            if (forceAttr != null)
            {
                return forceAttr.Name;
            }

            return member.Name;
        }
    }
}
