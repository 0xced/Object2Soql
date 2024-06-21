#pragma warning disable CA1307 // Specify StringComparison
#pragma warning disable CS8602 // Dereference of a possibly null reference.

using Object2Soql.Entities;
using Object2Soql.Tests.Entities;
using System;
using System.Globalization;
using System.Linq;
using Xunit;

namespace Object2Soql.Tests
{
    
    public partial class SoqlTests
    {
        private const string EXPECTED_FORMAT = "SELECT MyIntProperty FROM TestClass__c WHERE {0}";

        public static string SetUpExpectedWhere(string whereCondition)
        {
            return string.Format(CultureInfo.InvariantCulture, EXPECTED_FORMAT, whereCondition);
        }

        [Fact]
        public void InvalidAddingExpression()
        {
            // Act and Assert
            Assert.Throws<IlegalExpressionException>(() => Soql
                 .From<TestClass>()
                 .Select((x) => x.MyIntProperty)
                 .Where((x) => x.MyIntProperty + 1 == 12));
        }

        [Fact]
        public void InvalidSubctratingExpression()
        {
            // Act and Assert
            Assert.Throws<IlegalExpressionException>(() => Soql
                 .From<TestClass>()
                 .Select((x) => x.MyIntProperty)
                 .Where((x) => x.MyIntProperty - 1 == 12));
        }

        [Fact]
        public void SimpleIntegerComparision()
        {
            // Arrange
            var expected = SetUpExpectedWhere("MyIntProperty = 12");

            // Act
            var actual = Soql
                 .From<TestClass>()
                 .Select((x) =>x.MyIntProperty)
                 .Where((x)=>x.MyIntProperty == 12)
                 .Build();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void SimpleDateTimeComparision()
        {
            // Arrange
            var expected = SetUpExpectedWhere("MyDateTimeProperty__c = 2020-02-12T12-14-41Z");

            // Act
            var actual = Soql
                  .From<TestClass>()
                  .Select((x) => x.MyIntProperty)
                  .Where((x) => x.MyDateTimeProperty == new DateTime(2020, 02, 12, 12, 14, 41))
                  .Build();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void SimpleDateOnlyComparision()
        {
            // Arrange
            var expected = SetUpExpectedWhere("MyDateOnlyProperty__c = 2020-02-12");

            // Act
            var actual = Soql
                  .From<TestClass>()
                  .Select((x) => x.MyIntProperty)
                  .Where((x) => x.MyDateOnlyProperty == new DateOnly(2020, 02, 12))
                  .Build();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void StringInList()
        {
            // Arrange
            var values = new[] { "value_underscopre_a", "value space b" };
            var expected = SetUpExpectedWhere("MyStringProperty IN ('value_underscopre_a', 'value space b')");

            // Act
            var actual = Soql
                  .From<TestClass>()
                  .Select((x) => x.MyIntProperty)
                  .Where((x) => values.Contains(x.MyStringProperty))
                  .Build();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void StringStartsWith()
        {
            // Arrange
            var expected = SetUpExpectedWhere(@"MyStringProperty LIKE '%\%potato\%'");

            // Act
            var actual = Soql
                  .From<TestClass>()
                  .Select((x) => x.MyIntProperty)
                  .Where((x) => x.MyStringProperty.StartsWith("%potato%"))

                  .Build();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void StringEndsWith()
        {
            // Arrange
            var expected = SetUpExpectedWhere(@"MyStringProperty LIKE '\%potato\%%'");

            // Act
            var actual = Soql
                  .From<TestClass>()
                  .Select((x) => x.MyIntProperty)
                  .Where((x) => x.MyStringProperty.EndsWith("%potato%"))
                  .Build();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void StringContains()
        {
            // Arrange
            var expected = SetUpExpectedWhere(@"MyStringProperty LIKE '%\%potato\%%'");

            // Act
            var actual = Soql
                  .From<TestClass>()
                  .Select((x) => x.MyIntProperty)
                  .Where((x) => x.MyStringProperty.Contains("%potato%"))
                  .Build();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void StringEscaping()
        {
            // Arrange
            var expected = SetUpExpectedWhere(@"MyStringProperty = '%hello\' \\\' w\\\\\'\'o%r\\ld \\\'\'\'\\\\\'\\%'");

            // Act
            var actual = Soql
                  .From<TestClass>()
                  .Select((x) => x.MyIntProperty)
                  .Where((x) => x.MyStringProperty == @"%hello' \' w\\''o%r\ld \'''\\'\%")
                  .Build();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void EnumWithAttributeComparision()
        {
            // Arrange
            var expected = SetUpExpectedWhere(@"MyEnumProperty = 'Case A'");

            // Act
            var actual = Soql
                  .From<TestClass>()
                  .Select((x) => x.MyIntProperty)
                  .Where((x) => x.MyEnumProperty == TestEnum.CaseA)
                  .Build();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void EnumWithoutAttributeComparision()
        {
            // Arrange
            var expected = SetUpExpectedWhere(@"MyEnumProperty = 'CaseC'");

            // Act
            var actual = Soql
                  .From<TestClass>()
                  .Select((x) => x.MyIntProperty)
                  .Where((x) => x.MyEnumProperty == TestEnum.CaseC)
                  .Build();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void AndCondition()
        {
            // Arrange
            var expected = SetUpExpectedWhere(@"(MyChild__r.MyIntProperty = 22 AND MyStringProperty != null)");

            // Act
            var actual = Soql
                  .From<TestClass>()
                  .Select((x) => x.MyIntProperty)
                  .Where((x) => x.MyChild.MyIntProperty == 22 && x.MyStringProperty != null)
                  .Build();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void OrCondition()
        {
            // Arrange
            var expected = SetUpExpectedWhere(@"(MyChild__r.MyIntProperty = 22 OR MyChild__r.MyChild__r.MyEnumProperty = 'CaseC')");

            // Act
            var actual = Soql
                  .From<TestClass>()
                  .Select((x) => x.MyIntProperty)
                  .Where((x) => x.MyChild.MyIntProperty == 22 || x.MyChild.MyChild.MyEnumProperty == TestEnum.CaseC)
                  .Build();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void MultipleCondition()
        {
            // Arrange
            var expected = SetUpExpectedWhere(@"(MyChild__r.MyIntProperty = 22 OR (MyChild__r.MyChild__r.MyEnumProperty = 'CaseC' AND MyEnumProperty = 'Case A'))");

            // Act
            var actual = Soql
                  .From<TestClass>()
                  .Select((x) => x.MyIntProperty)
                  .Where((x) => x.MyChild.MyIntProperty == 22 || (x.MyChild.MyChild.MyEnumProperty == TestEnum.CaseC && x.MyEnumProperty == TestEnum.CaseA))
                  .Build();

            // Assert
            Assert.Equal(expected, actual);
        }
    }
}
