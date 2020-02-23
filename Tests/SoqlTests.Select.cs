using SoqlTests.Entities;
using System.Globalization;
using Visitor;
using Xunit;

namespace SoqlTests
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
    }
}
