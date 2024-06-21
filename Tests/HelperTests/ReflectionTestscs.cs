using Object2Soql.Helpers;
using Object2Soql.Tests.Entities;
using System.Collections.Generic;
using Xunit;

namespace Object2Soql.Tests.HelperTests
{
    public class ReflectionTestscs
    {
        [Fact]
        public void DescribeTestClass()
        {
            // Arrange
            var expected = new List<string>()
            {
                "MyIntProperty",
                "MyStringProperty",
                "MyBoolProperty__c",
                "MyDateTimeProperty__c",
                "MyDateTimeOffsetProperty",
                "MyDateOnlyProperty__c",
                "MyEnumProperty",
            };

            // Act
            var actual = Reflection.Describe(typeof(TestClass));

            // Assert
            Assert.Equal(expected, actual);
        }
    }
}
