using System;

namespace AsanaNet.Objects
{
    [Serializable]
    public class AsanaReference : AsanaObject, IAsanaData
    {
        [AsanaData("resource_type", SerializationFlags.Optional)]
        public string ResourceType { get; private set; }

        [AsanaData("name", SerializationFlags.Required)]
        public string Name { get; set; }

        // ------------------------------------------------------
        public AsanaReference()
        {

        }
        public AsanaReference(long id)
        {
            ID = id;
        }

        public bool IsObjectLocal { get { return ID == 0; } }

        public void Complete()
        {
            throw new NotImplementedException();
        }
    }
}