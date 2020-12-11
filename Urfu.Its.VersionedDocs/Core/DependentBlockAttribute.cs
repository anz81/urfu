using System;

namespace Urfu.Its.VersionedDocs.Core
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class DependentBlockAttribute : Attribute
    {
        public DependentBlockAttribute(string blockName)
        {
            BlockName = blockName;            
        }

        public string BlockName { get; }        
    }
}