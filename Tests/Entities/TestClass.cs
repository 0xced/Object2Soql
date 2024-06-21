using Object2Soql.Entities;
using System;
using System.Text.Json.Serialization;

namespace Object2Soql.Tests.Entities
{
    [SalesForce("TestClass__c")]
    public class TestClass
    {
        public int MyIntProperty { get; set; }

        public string? MyStringProperty { get; set; }

        [JsonPropertyName("MyBoolProperty__c")]
        public bool MyBoolProperty { get; set; }

        [JsonPropertyName("MyDateTimeProperty__c")]
        public DateTime MyDateTimeProperty { get; set; }

        public DateTimeOffset MyDateTimeOffsetProperty { get; set; }

        [JsonPropertyName("MyDateOnlyProperty__c")]
        public DateOnly MyDateOnlyProperty { get; set; }

        public TestEnum MyEnumProperty { get; set; }

        [JsonPropertyName("MyNullableEnumProperty__c")]
        public TestEnum? MyNullableEnumProperty { get; set; }

        [JsonPropertyName("MyChild__r")]
        public TestClass? MyChild { get; set; }
    }
}
