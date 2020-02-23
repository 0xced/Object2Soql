using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using Visitor;
using Xunit;

namespace SoqlTests
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
