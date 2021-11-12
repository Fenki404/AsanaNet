using System;

namespace AsanaNet.Objects
{
    [Serializable]
    public class EnumValue : AsanaObject, IAsanaData
    {
        [AsanaData("resource_type", SerializationFlags.Optional)]
        public string ResourceType { get; private set; }

        [AsanaData("name", SerializationFlags.Required)]
        public string Name { get; set; }

        [AsanaData("color", SerializationFlags.Optional)]
        public string Color { get; set; }

        [AsanaData("enabled", SerializationFlags.Optional)]
        public bool Enabled { get; set; }



        // ------------------------------------------------------
        public EnumValue()
        {

        }
        public EnumValue(long id)
        {
            ID = id;
        }

        public bool IsObjectLocal { get { return ID == 0; } }

        public void Complete()
        {
            throw new NotImplementedException();
        }

        public bool Is(long id)
        {
            return Enabled && id == ID;
        }
    }
}