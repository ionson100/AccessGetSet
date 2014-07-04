using System;

namespace AccessGetSet
{
    [AttributeUsage(AttributeTargets.Enum | AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class NotInitializeAttribute : Attribute { }
}