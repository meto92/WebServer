using System;

namespace SIS.MvcFramework.Attributes.Action
{
    [AttributeUsage(AttributeTargets.Method)]
    public class NonActionAttribute : Attribute
    { }
}