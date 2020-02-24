using System;
using System.Linq.Expressions;
using System.Text;
using System.Linq;
using Object2Soql.Entities;
using Object2Soql.Visitors;
using Object2Soql.Helpers;

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
        public string ConditionExpression { get; private set; }
        public string SelectExpression { get; private set; }

        public string OrderByExpression { get; private set; }
        public OrderByOption OrderByFlags { get; private set; }
        public int? Offset { get; private set; }
        public int? Limit { get; private set; }
        public string GroupByExpression { get; private set; }

        public Soql<TSource> Where(Expression<Func<TSource, bool>> exp)
        {
            var evaluatedExpression = Evaluator.PartialEval(exp) as LambdaExpression;
            this.ConditionExpression = WhereVisitor.Visit(evaluatedExpression.Body);
            if (this.ConditionExpression.Length > MAX_CONDITION_SIZE)
            {
                throw new ArgumentException();
            }

            return this;
        }

        public Soql<TSource> OrderBy(Expression<Func<TSource, object>> expression, OrderByOption orderByOptions = OrderByOption.Ascending | OrderByOption.NullFirst)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            this.OrderByExpression = SimpleMemberVistor(expression);
            this.OrderByFlags = orderByOptions;
            return this;
        }
        public Soql<TSource> ThenBy(Expression<Func<TSource, object>> exression)
        {
            if (exression == null)
            {
                throw new ArgumentNullException(nameof(exression));
            }

            var orderBy = SimpleMemberVistor(exression);
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
                throw new ArgumentOutOfRangeException("nElements must be greater than 0.");
            }

            this.Limit = nElements;
            return this;
        }

        public Soql<TSource> Skip(int nElements)
        {
            if (nElements <= 0 || nElements > MAX_OFFSET)
            {
                throw new ArgumentOutOfRangeException("nElements must be between 0 and MAX_OFFSET.");
            }

            this.Offset = nElements;
            return this;
        }

        public Soql<TSource> GroupBy(Expression<Func<TSource, object>> exp)
        {
            if (exp == null)
            {
                throw new ArgumentNullException(nameof(exp));
            }

            this.GroupByExpression = SimpleMemberVistor(exp);
            return this; 
        }

      

        public Soql<TSource> Select(Expression<Func<TSource, object>> exp)
        {
            this.SelectExpression = SelectVisitor.Visit(exp.Body);
            return this;
        }
        public string Build()
        {
            var query = new StringBuilder();

            if (string.IsNullOrEmpty(SelectExpression))
            {
                var fields = Reflection.Describe<TSource>();
                SelectExpression = string.Join(SelectVisitor.SELECT_FIELDS_SEPARATOR, fields);
            }

            query
                .Append("SELECT ")
                .Append(SelectExpression)
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

        private string SimpleMemberVistor(Expression<Func<TSource, object>> exp)
        {
            if(exp == null)
            {
                throw new ArgumentNullException(nameof(exp));
            }

            return exp.Body switch
            {
                UnaryExpression unaryExpression when unaryExpression.NodeType == ExpressionType.Convert => Reflection.GetMemberQualifiedName(unaryExpression.Operand as MemberExpression),
                MemberExpression memberExpression => Reflection.GetMemberQualifiedName(memberExpression),
                _ => throw new IlegalExpressionException(exp.Body.NodeType),
            };
        }
    }
}
