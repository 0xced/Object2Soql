using System.Runtime.Serialization;

namespace Object2Soql.Tests.Entities
{
    public enum TestEnum
    {
        [EnumMember(Value = "Case A")]
        CaseA,

        [EnumMember(Value = "Case B")]
        CaseB,

        CaseC
    }
}