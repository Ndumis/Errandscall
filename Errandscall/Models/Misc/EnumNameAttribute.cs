using System;

namespace Errandscall.Models
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    internal class EnumNameAttribute : Attribute
    {
        readonly string name;

        public EnumNameAttribute(string name)
        {
            this.name = name;
        }

        public string Name { get { return this.name; } }
    }
}