using System;
using System.Linq;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;
using Object2Soql.Entities;
using Object2Soql.Helpers;

namespace Object2Soql.Visitors
{
    public static class WhereVisitor
    {
        public static readonly char SPACE = ' ';

        public static string Visit(Expression exp)
        {
            if (exp == null)
            {
                return string.Empty;
            }

            return exp.NodeType switch
            {
                ExpressionType.Convert => VisitUnary((exp as UnaryExpression)!),
                ExpressionType.AndAlso => $"({VisitBinary((exp as BinaryExpression)!, "AND")})",
                ExpressionType.OrElse => $"({VisitBinary((exp as BinaryExpression)!, "OR")})",
                ExpressionType.LessThan => VisitBinary((exp as BinaryExpression)!, "<"),
                ExpressionType.LessThanOrEqual => VisitBinary((exp as BinaryExpression)!, "<="),
                ExpressionType.GreaterThan => VisitBinary((exp as BinaryExpression)!, ">"),
                ExpressionType.GreaterThanOrEqual => VisitBinary((exp as BinaryExpression)!, ">="),
                ExpressionType.Equal => VisitBinary((exp as BinaryExpression)!, "="),
                ExpressionType.NotEqual => VisitBinary((exp as BinaryExpression)!, "!="),
                ExpressionType.ExclusiveOr => VisitBinary((exp as BinaryExpression)!, ""),
                ExpressionType.Constant => VisitConstant((exp as ConstantExpression)!),
                ExpressionType.MemberAccess => VisitMemberAccess((exp as MemberExpression)!),
                ExpressionType.Call => VisitMethodCall((exp as MethodCallExpression)!),
                _ => throw new IlegalExpressionException(exp.NodeType),
            };
        }

        private static string VisitUnary(UnaryExpression u)
        {
            return Visit(u.Operand);
        }

        private static string VisitBinary(BinaryExpression binaryExpession, string simbol)
        {
            var left = Visit(binaryExpession.Left);
            var right = Visit(binaryExpession.Right);

            if (TryGetEnum(binaryExpession.Left, right, out var fixedRight))
            {
                right = fixedRight;
            }
            else if (TryGetEnum(binaryExpession.Right, left, out var fixedLeft))
            {
                left = fixedLeft;
            }

            return $"{left} {simbol} {right}";
        }

        private static bool TryGetEnum(Expression expression, string value, out string fixedValue)
        {
            if (expression.NodeType != ExpressionType.Convert || !int.TryParse(value, out var enumInt))
            {
                fixedValue = value;
                return false;
            }

            if (!(expression is UnaryExpression conversion) || !conversion.Operand.Type.IsEnum)
            {
                fixedValue = value;
                return false;
            }

            var enumName = conversion.Operand.Type.GetEnumName(enumInt);
            if(enumName == null)
            {
                fixedValue = value;
                return false;
            }
            var enumAttrs = conversion.Operand.Type.GetField(enumName)?.GetCustomAttributes<EnumMemberAttribute>().FirstOrDefault();
            if (enumAttrs == null)
            {
                fixedValue = $"'{enumName}'";
            }
            else
            {
                fixedValue = $"'{enumAttrs.Value}'";
            }
            
            return true;
        }

        private static string VisitConstant(ConstantExpression constantExpression)
        {
            var enumerable = constantExpression.Value as System.Collections.IEnumerable;

            if (enumerable == null || constantExpression.Value is string)
            {
                return GetValue(constantExpression.Value);
            }

            var values = new List<string>();
            foreach (var item in enumerable)
            {
                values.Add(GetValue(item));
            }

            return string.Join(", ", values);
        }

        private static string VisitMemberAccess(MemberExpression memberExpression)
        {
            return Reflection.GetMemberQualifiedName(memberExpression);
        }

        private static string VisitMethodCall(MethodCallExpression methodCallExpression)
        {
            // if the object operand is null then this is a static method and is likely to be
            // the Linq's Contains extension
            if (methodCallExpression.Object== null && methodCallExpression.Method.Name == "Contains")
            {
                return VisitListContainsCall(methodCallExpression);
            }
            // string.Contains however is not static and this we check the operand's type
            else if (methodCallExpression?.Object?.Type == typeof(string) && methodCallExpression.Method.Name == "Contains")
            {
                return VisitStringContainsCall(methodCallExpression);
            }
            else if (methodCallExpression.Method.Name == "StartsWith")
            {
                return VisitStartsWithCall(methodCallExpression);
            }
            else if (methodCallExpression.Method.Name == "EndsWith")
            {
                return VisitEndsWithCall(methodCallExpression);
            }
            else if (methodCallExpression.Method.Name == "Equals")
            {
                return VisitEqualsCall(methodCallExpression);
            }

            throw new NotImplementedException(nameof(VisitMemberAccess));
        }

        private static string VisitStartsWithCall(MethodCallExpression methodCallExpression)
        {
            var haystack = Visit(methodCallExpression.Object);
            var needle = Visit(methodCallExpression.Arguments[0]);

            // needle already has the reserved characters: \ and '
            // escaped but in this case we also need to escape %
            needle = needle.Replace("%", @"\%");
            return $"{haystack} LIKE {needle.Insert(1, "%")}";
        }

        private static string VisitEndsWithCall(MethodCallExpression methodCallExpression)
        {
            var haystack = Visit(methodCallExpression.Object);
            var needle = Visit(methodCallExpression.Arguments[0]);

            // needle already has the reserved characters: \ and '
            // escaped but in this case we also need to escape %
            needle = needle.Replace("%", @"\%");
            return $"{haystack} LIKE {needle.Insert(needle.Length -1, "%")}";
        }

        private static string VisitEqualsCall(MethodCallExpression methodCallExpression)
        {
            var haystack = Visit(methodCallExpression.Object);
            var needle = Visit(methodCallExpression.Arguments[0]);
            return $"{haystack} = {needle}";
        }

        private static string VisitStringContainsCall(MethodCallExpression methodCallExpression)
        {
            var haystack = Visit(methodCallExpression.Object);
            var needle = Visit(methodCallExpression.Arguments[0]);

            // needle already has the reserved characters: \ and '
            // escaped but in this case we also need to escape %
            needle = needle.Replace("%", @"\%");
            needle = needle.Insert(1, "%");
            needle = needle.Insert(needle.Length - 1, "%");
            return $"{haystack} LIKE {needle}";
        }

        private static string VisitListContainsCall(MethodCallExpression methodCallExpression)
        {
            if (methodCallExpression.Object != null)
            {
                var haystack = Visit(methodCallExpression.Object);
                var needle = Visit(methodCallExpression.Arguments[0]);
                return $"{needle} IN ({haystack})";
            }
            else
            {
                var haystack = Visit(methodCallExpression.Arguments[0]);
                var needle = Visit(methodCallExpression.Arguments[1]);
                return $"{needle} IN ({haystack})";
            }
        }

        private static string GetValue(object? value)
        {
            if (value == null)
            {
                return "null";
            }
            else if (value is string stringValue)
            {
                return $"'{Escape(stringValue)}'";
            }
            else if (value is DateTime dateTime)
            {
                return dateTime.ToString("yyyy-MM-ddTHH-mm-ssZ");
            }
            else if (value is DateTimeOffset dateTimeOffset)
            {
                return dateTimeOffset.ToString("yyyy-MM-ddTHH-mm-ssK");
            }
            else
            {
                return value.ToString() ?? string.Empty;
            }
        }

        /// <summary>
        /// Escapes salesforce's reserved characters to be properly interpreted.
        /// </summary>
        /// <param name="input">The string to be escaped.</param>
        /// <returns>The escaped string.</returns>
        private static string Escape(string input)
        {
            if(input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            return input.Replace(@"\", @"\\").Replace("'", @"\'");
        }
    }
}
