using System;

namespace Object2Soql.Entities
{
    [AttributeUsage(AttributeTargets.Class)]
    public class SalesForceAttribute:Attribute
    {
        public string Name { get; }

        public SalesForceAttribute(string objectName)
        {
            Name = objectName;
        }
    }
}
