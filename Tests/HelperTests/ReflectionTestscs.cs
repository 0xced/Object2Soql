using Object2Soql.Helpers;
using Object2Soql.Tests.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SoqlTests.HelperTests
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
                "MyEnumProperty",
            };

            // Act
            var actual = Reflection.Describe<TestClass>();

            // Assert
            Assert.Equal(expected, actual);
        }
    }
}
