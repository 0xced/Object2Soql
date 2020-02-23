using System.Runtime.Serialization;

namespace SoqlTests
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