using Object2Soql.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json.Serialization;

namespace Object2Soql.Helpers
{
    public static class Reflection
    {
        public const char QUALIFIED_NAME_SEPARATOR = '.';

        public static string GetMemberQualifiedName(MemberExpression memberExpression)
        {
            var qualifiedNames = new Stack<string>();
            qualifiedNames.Push(GetNameOf(memberExpression.Member));

            var parentExpression = memberExpression.Expression;
            while (parentExpression is MemberExpression parentMemberExpression)
            {
                qualifiedNames.Push(GetNameOf(parentMemberExpression.Member));
                parentExpression = parentMemberExpression.Expression;
            }

            return string.Join(QUALIFIED_NAME_SEPARATOR, qualifiedNames);
        }

        public static IEnumerable<string> Describe(Type type)
        {
            var fields = new List<string>();
            foreach(var property in type.GetProperties())
            {
                if (property.PropertyType.IsClass && property.PropertyType != typeof(string))
                {
                    continue;
                }

                fields.Add(GetNameOf(property));
            }

            return fields;
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
