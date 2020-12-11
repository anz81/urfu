using System.Collections.Generic;

namespace Urfu.Its.VersionedDocs.Tests.TestData
{
    public class Arrays1
    {
        public ICollection<string> StringArray1 { get; set; } = new List<string> { "s1", "s2" };
        public ICollection<bool> BooleanArray1 { get; set; } = new List<bool> { true, false, true };
        public ICollection<int> IntegerArray1 { get; set; } = new List<int> { 1,2,3,4 };
        public ICollection<double> NumberArray1 { get; set; } = new List<double> {1.2};
        public ICollection<string> StringArrayWithNulls1 { get; set; } = new List<string>{null, "s1", null,"s2"};
        public ICollection<string> EmptyStringArray1 { get; set; } = new List<string>();
        public ICollection<Simple1> ObjectArray1 { get; set; } = new List<Simple1>{new Simple1(), new Simple1()};
        public ICollection<ICollection<int>> ArrayOfIntArrays1 { get; set; } = new List<ICollection<int>>{new List<int>{11,12}, new List<int>{21,22}};
    }
}