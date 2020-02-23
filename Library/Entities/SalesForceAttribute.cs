using System;
using System.Collections.Generic;
using System.Text;

namespace SoqlLibrary.Entities
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
