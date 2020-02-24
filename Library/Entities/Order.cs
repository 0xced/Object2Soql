using System;

namespace Object2Soql.Entities
{
    [Flags]
    public enum OrderByOption
    {
        Ascending = 1,
        Descending = 2,
        NullFirst = 4,
        NullLast = 8
    }
}