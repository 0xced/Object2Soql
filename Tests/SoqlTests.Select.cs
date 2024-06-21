#pragma warning disable CS8602 // Dereference of a possibly null reference.

using System.Globalization;
using Xunit;
using Object2Soql.Entities;
using Object2Soql.Tests.Entities;

namespace Object2Soql.Tests
{
    public partial class SoqlTests
    {
        public const string EXPECTED_SELECT_FORMAT = "SELECT {0} FROM TestClass__c";

        public static string SetUpExpectedSelect(string selectStatment)
        {
            return string.Format(CultureInfo.InvariantCulture, EXPECTED_SELECT_FORMAT, selectStatment);
        }

        [Fact]
        public void SingleBoolMember()
        {
            // Arrange
            var expected = SetUpExpectedSelect("MyBoolProperty__c");

            // Act
            var actual = Soql
                .From<TestClass>()
                .Select((x) => x.MyBoolProperty)
                .Build();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void SingleDateMember()
        {
            // Arrange
            var expected = SetUpExpectedSelect("MyChild__r.MyBoolProperty__c");

            // Act
            var actual = Soql
                .From<TestClass>()
                .Select((x) => x.MyChild.MyBoolProperty)
                .Build();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void SingleNestedMember()
        {
            // Arrange
            var expected = SetUpExpectedSelect("MyChild__r.MyBoolProperty__c");

            // Act
            var actual = Soql
                .From<TestClass>()
                .Select((x) => x.MyChild.MyBoolProperty)
                .Build();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void MultipleMember()
        {
            // Arrange
            var expected = SetUpExpectedSelect("MyDateTimeProperty__c, MyBoolProperty__c");

            // Act
            var actual = Soql
                .From<TestClass>()
                .Select((x) => new { x.MyDateTimeProperty, x.MyBoolProperty })
                .Build();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void MultipleMemberWithNested()
        {
            // Arrange
            var expected = SetUpExpectedSelect("MyDateTimeProperty__c, MyChild__r.MyBoolProperty__c");

            // Act
            var actual = Soql
                .From<TestClass>()
                .Select((x) => new { x.MyDateTimeProperty, x.MyChild.MyBoolProperty })
                .Build();

            // Assert
            Assert.Equal(expected, actual);
        }


        [Fact]
        public void InvalidBinaryExpression()
        {
            // Act and Assert
            Assert.Throws<IlegalExpressionException>(() => Soql.From<TestClass>().Select((x) => x.MyBoolProperty == x.MyChild.MyBoolProperty ));
        }

        [Fact]
        public void NoSelectTest()
        {
            // Arrange
            var expected = SetUpExpectedSelect("MyIntProperty, MyStringProperty, MyBoolProperty__c, MyDateTimeProperty__c, MyDateTimeOffsetProperty, MyDateOnlyProperty__c, MyEnumProperty, MyNullableEnumProperty__c");

            // Act 
            var actual = Soql
                .From<TestClass>()
                .Build();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IncludeWithNoSelectTest()
        {
            // Arrange
            var expected = SetUpExpectedSelect("MyIntProperty, MyStringProperty, MyBoolProperty__c, MyDateTimeProperty__c, MyDateTimeOffsetProperty, MyDateOnlyProperty__c, MyEnumProperty, MyNullableEnumProperty__c, MyChild__r.MyChild__r.MyIntProperty, MyChild__r.MyChild__r.MyStringProperty, MyChild__r.MyChild__r.MyBoolProperty__c, MyChild__r.MyChild__r.MyDateTimeProperty__c, MyChild__r.MyChild__r.MyDateTimeOffsetProperty, MyChild__r.MyChild__r.MyDateOnlyProperty__c, MyChild__r.MyChild__r.MyEnumProperty, MyChild__r.MyChild__r.MyNullableEnumProperty__c");

            // Act 
            var actual = Soql
                .From<TestClass>()
                .Include(x => x.MyChild.MyChild)
                .Build();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IncludeWithSelectTest()
        {
            // Arrange
            var expected = SetUpExpectedSelect("MyBoolProperty__c, MyChild__r.MyChild__r.MyIntProperty, MyChild__r.MyChild__r.MyStringProperty, MyChild__r.MyChild__r.MyBoolProperty__c, MyChild__r.MyChild__r.MyDateTimeProperty__c, MyChild__r.MyChild__r.MyDateTimeOffsetProperty, MyChild__r.MyChild__r.MyDateOnlyProperty__c, MyChild__r.MyChild__r.MyEnumProperty, MyChild__r.MyChild__r.MyNullableEnumProperty__c");

            // Act 
            var actual = Soql
                .From<TestClass>()
                .Select(x => x.MyBoolProperty)
                .Include(x => x.MyChild.MyChild)
                .Build();

            // Assert
            Assert.Equal(expected, actual);
        }
    }
}
