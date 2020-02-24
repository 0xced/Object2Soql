using Object2Soql.Helpers;
using Object2Soql.Tests.Entities;
using System;
using System.Linq.Expressions;
using Xunit;

namespace Object2Soql.Tests.HelperTests
{
    public class ExpressionUtilsTests
    {
        [Fact]
        public void GetMemberQualifiedNameSimple()
        {
            // Arrange
            var expected = "MyBoolProperty__c";
            Expression<Func<TestClass, bool>> expression = (x) => x.MyBoolProperty;

            // Act
            var actual = Namer.GetMemberQualifiedName(expression.Body as MemberExpression);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetMemberQualifiedNameTestNested()
        {
            // Arrange
            var expected = "MyChild__r.MyBoolProperty__c";
            Expression<Func<TestClass, bool>> expression = (x) => x.MyChild.MyBoolProperty;

            // Act
            var actual = Namer.GetMemberQualifiedName(expression.Body as MemberExpression);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetMemberQualifiedNameTestDoubleNested()
        {
            // Arrange
            var expected = "MyChild__r.MyChild__r.MyBoolProperty__c";
            Expression<Func<TestClass, bool>> expression = (x) => x.MyChild.MyChild.MyBoolProperty;

            // Act
            var actual = Namer.GetMemberQualifiedName(expression.Body as MemberExpression);

            // Assert
            Assert.Equal(expected, actual);
        }

    }
}
