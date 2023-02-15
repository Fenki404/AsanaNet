using System;

namespace AsanaNet
{
    [Flags]
    public enum SerializationFlags
    {
        Optional = 1,
        Required = 2,
        Omit = 4,
        ReadOnly = 8,
        WriteField = 16,
        WriteNull = 32,
        DateOnly = 64,
        PropertyArgsSuffixFields = 128
    }

    [AttributeUsage(AttributeTargets.Property)]
    internal class AsanaDataAttribute : Attribute
    {
        public string Name { get; private set; }
        public SerializationFlags Flags { get; private set; }
        public string[] Fields { get; private set; }

        public AsanaDataAttribute(string name, SerializationFlags flags = SerializationFlags.Optional, params string[] fieldsToSerialize)
        {
            Name = name;
            Flags = flags;
            Fields = fieldsToSerialize;
        }
    }
}
