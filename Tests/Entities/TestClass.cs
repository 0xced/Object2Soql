using SoqlLibrary.Entities;
using System;
using System.Text.Json.Serialization;

namespace SoqlTests
{
    [SalesForce("TestClass__c")]
    public class TestClass
    {
        public int MyIntProperty { get; set; }

        public string MyStringProperty { get; set; }

        [JsonPropertyName("MyBoolProperty__c")]
        public bool MyBoolProperty { get; set; }

        [JsonPropertyName("MyDateTimeProperty__c")]
        public DateTime MyDateTimeProperty { get; set; }

        public DateTimeOffset MyDateTimeOffsetProperty { get; set; }

        public TestEnum MyEnumProperty { get; set; }

        [JsonPropertyName("MyChild__r")]
        public TestClass MyChild { get; set; }
    }
}
