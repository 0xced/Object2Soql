using System;
using System.Linq.Expressions;
using System.Text;
using System.Linq;
using Object2Soql.Entities;
using Object2Soql.Visitors;
using Object2Soql.Helpers;
using System.Collections.Generic;

namespace Object2Soql
{
    public static class Soql
    {
        public static Soql<TSource> From<TSource>() {
            return new Soql<TSource>();
        }
    }

    public class Soql<TSource>
    {
        public static readonly string SELECT_FIELDS_SEPARATOR = ", ";

        /// <summary>
        /// WHERE clauses cannot exceed 4,000 characters.
        /// </summary>
        public const int MAX_CONDITION_SIZE = 4_000;

        /// <summary>
        /// By default, SOQL statements cannot exceed 20,000 characters in length.
        /// For SOQL statements that exceed this maximum length, the API returns a MALFORMED_QUERY
        /// exception code and no result rows are returned. 
        /// </summary>
        public const int MAX_STATEMENT_SIZE = 20_000;

        /// <summary>
        /// The maximum offset is 2,000 rows. 
        /// Requesting an offset greater than 2,000 results in a NUMBER_OUTSIDE_VALID_RANGE error.
        /// </summary>
        public const int MAX_OFFSET = 2_000;
        public string? ConditionExpression { get; private set; }
        public List<string> SelectExpression { get; }

        public string? OrderByExpression { get; private set; }
        public OrderByOption OrderByFlags { get; private set; }
        public int? Offset { get; private set; }
        public int? Limit { get; private set; }
        public string? GroupByExpression { get; private set; }

        public Soql()
        {
            SelectExpression = new List<string>();
        }

        public Soql<TSource> Where(Expression<Func<TSource, bool>> exp)
        {
            var evaluatedExpression = Evaluator.PartialEval(exp) as LambdaExpression;
            this.ConditionExpression = WhereVisitor.Visit(evaluatedExpression?.Body);
            if (this.ConditionExpression.Length > MAX_CONDITION_SIZE)
            {
                throw new ArgumentException($"The condition is too long: {ConditionExpression.Length} but the maximum allowed is {MAX_CONDITION_SIZE}.", nameof(exp));
            }

            return this;
        }

        public Soql<TSource> OrderBy(Expression<Func<TSource, object>> expression, OrderByOption orderByOptions = OrderByOption.Ascending | OrderByOption.NullFirst)
        {
            this.OrderByExpression = SimpleMemberVisitor(expression);
            this.OrderByFlags = orderByOptions;
            return this;
        }

        public Soql<TSource> ThenBy(Expression<Func<TSource, object>> expression)
        {
            ArgumentNullException.ThrowIfNull(expression);

            var orderBy = SimpleMemberVisitor(expression);
            if (string.IsNullOrEmpty(OrderByExpression))
            {
                this.OrderByExpression = orderBy;
            }
            else
            {
                this.OrderByExpression = $"{OrderByExpression}, {orderBy}";
            }

            return this;
        }

        public Soql<TSource> Take(int nElements)
        {
            if (nElements <= 0 )
            {
                throw new ArgumentOutOfRangeException(nameof(nElements), "nElements must be greater than 0.");
            }

            this.Limit = nElements;
            return this;
        }

        public Soql<TSource> Skip(int nElements)
        {
            if (nElements is <= 0 or > MAX_OFFSET)
            {
                throw new ArgumentOutOfRangeException(nameof(nElements), "nElements must be between 0 and MAX_OFFSET.");
            }

            this.Offset = nElements;
            return this;
        }

        public Soql<TSource> GroupBy(Expression<Func<TSource, object>> exp)
        {
            ArgumentNullException.ThrowIfNull(exp);

            this.GroupByExpression = SimpleMemberVisitor(exp);
            return this; 
        }

        public Soql<TSource> Select(Expression<Func<TSource, object>> exp)
        {
            this.SelectExpression.AddRange(SelectVisitor.Visit(exp.Body));
            return this;
        }

        public Soql<TSource> Include<TChild>(Expression<Func<TSource, TChild?>> exp) where TChild: class
        {
            if (!SelectExpression.Any())
            {
                SelectExpression.AddRange(Reflection.Describe(typeof(TSource)));
            }

            var child = SimpleMemberVisitor(exp);
            this.SelectExpression.AddRange(Reflection.Describe(exp.Body.Type).Select(x => $"{child}.{x}"));
            return this;
        }

        public string Build()
        {
            var query = new StringBuilder();

            if (!SelectExpression.Any())
            {
                SelectExpression.AddRange(Reflection.Describe(typeof(TSource)));
            }

            query
                .Append("SELECT ")
                .Append(string.Join(SELECT_FIELDS_SEPARATOR, SelectExpression.Distinct()))
                .Append(" FROM ")
                .Append(Reflection.GetNameOf(typeof(TSource)));

            if (!string.IsNullOrEmpty(ConditionExpression))
            {
                query
                    .Append(" WHERE ")
                    .Append(ConditionExpression);
            }

            if (!string.IsNullOrEmpty(GroupByExpression))
            {
                query
                    .Append(" GROUP BY ")
                    .Append(GroupByExpression);
            }

            if (!string.IsNullOrEmpty(OrderByExpression))
            {
                query
                    .Append(" ORDER BY ")
                    .Append(OrderByExpression);

                if((OrderByFlags & OrderByOption.Descending)  == OrderByOption.Descending)
                {
                    query.Append(" DESCENDING");
                }
                else 
                {
                    query.Append(" ASCENDING");
                }

                if ((OrderByFlags & OrderByOption.NullLast) == OrderByOption.NullLast)
                {
                    query.Append(" NULLS LAST");
                }
                else
                {
                    query.Append(" NULLS FIRST");
                }
            }

            if (Limit.HasValue)
            {
                query
                    .Append(" LIMIT ")
                    .Append(Limit);
            }


            if (Limit.HasValue && Offset.HasValue)
            {
                query
                    .Append(" OFFSET ")
                    .Append(Offset);
            }

            return query.ToString();
        }

        private static string SimpleMemberVisitor<TValue>(Expression<Func<TSource, TValue>> exp)
        {
            return exp.Body switch
            {
                UnaryExpression { NodeType: ExpressionType.Convert, Operand: MemberExpression memberExpression } => Reflection.GetMemberQualifiedName(memberExpression),
                MemberExpression memberExpression => Reflection.GetMemberQualifiedName(memberExpression),
                _ => throw new IlegalExpressionException(exp.Body.NodeType),
            };
        }
    }
}
