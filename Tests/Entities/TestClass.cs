using SoqlLibrary.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SoqlTests
{
    
    [SalesForce("TestClass__c")]
    public class TestClass
    {
        public int MyIntProperty { get; set; }
        public string MyStringProperty { get; set; }
        public bool MyBoolProperty { get; set; }
        public DateTime MyDateTimeProperty { get; set; }

        public DateTimeOffset MyDateTimeOffsetProperty { get; set; }

        public TestEnum MyEnumProperty { get; set; }

        public TestClass MyChild { get; set; }
    }
}
