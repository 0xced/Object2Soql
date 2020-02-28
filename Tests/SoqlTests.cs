#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8603 // Possibly returning a null reference.

using Object2Soql.Entities;
using Object2Soql.Tests.Entities;
using System;
using Xunit;

namespace Object2Soql.Tests
{
    public partial class SoqlTests
    {
        [Fact]
        public void TakeTest()
        {
            // Arrange
            var expected = "SELECT MyBoolProperty__c FROM TestClass__c LIMIT 2";

            // Act
            var actual = Soql
                .From<TestClass>()
                .Select((x) => x.MyBoolProperty)
                .Take(2)
                .Build();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void SkipTest()
        {
            // Arrange
            var expected = "SELECT MyBoolProperty__c FROM TestClass__c LIMIT 2 OFFSET 3";

            // Act
            var actual = Soql
                .From<TestClass>()
                .Select((x) => x.MyBoolProperty)
                .Take(2)
                .Skip(3)
                .Build();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void SkipWithoutTakeTest()
        {
            // Arrange
            var expected = "SELECT MyBoolProperty__c FROM TestClass__c";

            // Act
            var actual = Soql
                .From<TestClass>()
                .Select((x) => x.MyBoolProperty )
                .Skip(2)
                .Build();


            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void SkipAboveLimitTest()
        {
            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => Soql.From<TestClass>().Select((x) => x.MyBoolProperty).Skip(int.MaxValue));
        }

        [Fact]
        public void OrderByTest()
        {
            // Arrange
            var expected = "SELECT MyBoolProperty__c FROM TestClass__c ORDER BY MyDateTimeProperty__c ASCENDING NULLS FIRST";

            // Act
            var actual = Soql
                 .From<TestClass>()
                 .Select((x) => x.MyBoolProperty)
                 .OrderBy(x => x.MyDateTimeProperty)
                 .Build();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void OrderByDescendingTest()
        {
            // Arrange
            var expected = "SELECT MyBoolProperty__c FROM TestClass__c ORDER BY MyDateTimeProperty__c DESCENDING NULLS FIRST";

            // Act
            var actual = Soql
                 .From<TestClass>()
                 .Select((x) => x.MyBoolProperty)
                 .OrderBy(x => x.MyDateTimeProperty, OrderByOption.Descending)
                 .Build();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void OrderByNullLastTest()
        {
            // Arrange
            var expected = "SELECT MyBoolProperty__c FROM TestClass__c ORDER BY MyDateTimeProperty__c ASCENDING NULLS LAST";

            // Act
            var actual = Soql
                 .From<TestClass>()
                 .Select((x) => x.MyBoolProperty)
                 .OrderBy(x => x.MyDateTimeProperty, OrderByOption.NullLast)
                 .Build();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void OrderByNullDescendingLastTest()
        {
            // Arrange
            var expected = "SELECT MyBoolProperty__c FROM TestClass__c ORDER BY MyDateTimeProperty__c DESCENDING NULLS LAST";

            // Act
            var actual = Soql
                 .From<TestClass>()
                 .Select((x) => x.MyBoolProperty)
                 .OrderBy(x => x.MyDateTimeProperty, OrderByOption.NullLast | OrderByOption.Descending)
                 .Build();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void OrderByThenByTest()
        {
            // Arrange
            var expected = "SELECT MyBoolProperty__c FROM TestClass__c ORDER BY MyDateTimeProperty__c, MyIntProperty, MyStringProperty ASCENDING NULLS FIRST";

            // Act
            var actual = Soql
                 .From<TestClass>()
                 .Select((x) => x.MyBoolProperty)
                 .OrderBy(x => x.MyDateTimeProperty)
                 .ThenBy(x => x.MyIntProperty)
                 .ThenBy(x => x.MyStringProperty)
                 .Build();

            // Assert
            Assert.Equal(expected, actual);
        }
    }
}
